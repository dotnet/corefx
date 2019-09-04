// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;

namespace System.Net.Http
{
    public partial class HttpClientHandler : HttpMessageHandler
    {
        // This partial implementation contains members common to Windows and Unix running on .NET Core.

        private bool _operationStarted = false;
        private volatile bool _disposed = false;

        public long MaxRequestContentBufferSize
        {
            // This property is not supported. In the .NET Framework it was only used when the handler needed to
            // automatically buffer the request content. That only happened if neither 'Content-Length' nor
            // 'Transfer-Encoding: chunked' request headers were specified. So, the handler thus needed to buffer
            // in the request content to determine its length and then would choose 'Content-Length' semantics when
            // POST'ing. In .NET Core and UAP platforms, the handler will resolve the ambiguity by always choosing
            // 'Transfer-Encoding: chunked'. The handler will never automatically buffer in the request content.
            get
            {
                return 0; // Returning zero is appropriate since in .NET Framework it means no limit.
            }

            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(value));
                }

                if (value > HttpContent.MaxBufferSize)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), value,
                        SR.Format(CultureInfo.InvariantCulture, SR.net_http_content_buffersize_limit,
                        HttpContent.MaxBufferSize));
                }

                CheckDisposedOrStarted();

                // No-op on property setter.
            }
        }

        protected internal override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (request == null)
            {
                return Task.FromException<HttpResponseMessage>(new ArgumentNullException(nameof(request), SR.net_http_handler_norequest));
            }

            if (ShouldBufferContent(request))
            {
                return SendWithBufferedContentAsync(request, cancellationToken);
            }

            return SendWithPlatformHandlerAsync(request, cancellationToken);
        }

        private async Task<HttpResponseMessage> SendWithBufferedContentAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            Debug.Assert(request != null);
            Debug.Assert(request.Content != null);
            Debug.Assert(request.Content.Headers.ContentLength == null);

            long? computedLength = request.Content.GetComputedOrBufferLength();

            if (!computedLength.HasValue)
            {
                await request.Content.LoadIntoBufferAsync().ConfigureAwait(false);

                computedLength = request.Content.GetComputedOrBufferLength();
            }

            Debug.Assert(computedLength.HasValue, "Buffering should necessarily ensure that GetComputedOrBufferLength() will return a non-null value.");

            request.Headers.TransferEncodingChunked = false;
            request.Content.Headers.ContentLength = computedLength;

            return await SendWithPlatformHandlerAsync(request, cancellationToken)
                .ConfigureAwait(false);
        }

        private void CheckDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(GetType().ToString());
            }
        }

        private void CheckDisposedOrStarted()
        {
            CheckDisposed();
            if (_operationStarted)
            {
                throw new InvalidOperationException(SR.net_http_operation_started);
            }
        }

        private static bool ShouldBufferContent(HttpRequestMessage request)
        {
            // Don't buffer if there's nothing to buffer or the length is already defined.
            if (request.Content == null || request.Content.Headers.ContentLength != null)
            {
                return false;
            }

            // Buffer HTTP 1.0 regardless of TransferEncodingChunked.
            if (request.Version.Minor == 0 && request.Version.Major == 1)
            {
                return true;
            }

            // Buffer if TransferEncodingChunked explicitly set to false.
            if (request.HasHeaders && request.Headers.TransferEncodingChunked == false)
            {
                return true;
            }

            return false;
        }
    }
}
