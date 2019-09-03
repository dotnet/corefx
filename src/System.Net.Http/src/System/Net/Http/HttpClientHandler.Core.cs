// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Globalization;
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

        private static Exception ValidateAndNormalizeRequest(HttpRequestMessage request)
        {
            bool shouldBufferContent = false;

            // Add headers to define content transfer, if not present
            if (request.HasHeaders && request.Headers.TransferEncodingChunked.GetValueOrDefault())
            {
                if (request.Content == null)
                {
                    return new HttpRequestException(SR.net_http_client_execution_error,
                        new InvalidOperationException(SR.net_http_chunked_not_allowed_with_empty_content));
                }

                // Since the user explicitly set TransferEncodingChunked to true, we need to remove
                // the Content-Length header if present, as sending both is invalid.
                request.Content.Headers.ContentLength = null;
            }
            else if (request.Content != null && request.Content.Headers.ContentLength == null && request.Headers.TransferEncodingChunked == false)
            {
                // We have content, but Content-Length is not set and Transfer-Encoding was explicitly unset.
                shouldBufferContent = true;
            }
            else if (request.Content != null && request.Content.Headers.ContentLength == null && request.Headers.TransferEncodingChunked == null)
            {
                // We have content, but Content-Length is not set and Transfer-Encoding was not specified.
                request.Headers.TransferEncodingChunked = true;
            }

            if (request.Version.Minor == 0 && request.Version.Major == 1 && request.HasHeaders)
            {
                // HTTP 1.0 does not support chunking
                if (request.Content != null && request.Headers.TransferEncodingChunked == true)
                {
                    shouldBufferContent = true;
                }

                // HTTP 1.0 does not support Expect: 100-continue; just disable it.
                if (request.Headers.ExpectContinue == true)
                {
                    request.Headers.ExpectContinue = false;
                }
            }

            if (shouldBufferContent)
            {
                long? computedLength = request.Content.GetComputedOrBufferLength();

                if (!computedLength.HasValue)
                {
                    request.Content.LoadIntoBufferAsync().GetAwaiter().GetResult();

                    computedLength = request.Content.GetComputedOrBufferLength();
                }


                Debug.Assert(computedLength.HasValue, "Buffering should necessarily ensure that GetComputedOrBufferLength() will return a non-null value.");

                request.Headers.TransferEncodingChunked = false;
                request.Content.Headers.ContentLength = computedLength;
            }

            return null;
        }
    }
}
