﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace System.Net.Http
{
    internal partial class HttpConnection : IDisposable
    {
        private sealed class HttpConnectionResponseContent : HttpContent
        {
            private HttpContentStream _stream;
            private bool _consumedStream;

            public void SetStream(HttpContentStream stream)
            {
                Debug.Assert(stream != null);
                Debug.Assert(stream.CanRead);
                Debug.Assert(!_consumedStream);

                _stream = stream;
            }

            internal bool IsEmpty => (_stream == EmptyReadStream.Instance);

            private HttpContentStream ConsumeStream()
            {
                if (_consumedStream || _stream == null)
                {
                    throw new InvalidOperationException(SR.net_http_content_stream_already_read);
                }
                _consumedStream = true;

                return _stream;
            }

            protected sealed override Task SerializeToStreamAsync(Stream stream, TransportContext context) =>
                SerializeToStreamAsync(stream, context, CancellationToken.None);

            internal sealed override async Task SerializeToStreamAsync(Stream stream, TransportContext context, CancellationToken cancellationToken)
            {
                Debug.Assert(stream != null);

                using (HttpContentStream contentStream = ConsumeStream())
                {
                    const int BufferSize = 8192;
                    await contentStream.CopyToAsync(stream, BufferSize, cancellationToken).ConfigureAwait(false);
                }
            }

            protected internal sealed override bool TryComputeLength(out long length)
            {
                length = 0;
                return false;
            }

            protected sealed override Task<Stream> CreateContentReadStreamAsync() =>
                Task.FromResult<Stream>(ConsumeStream());

            internal sealed override Stream TryCreateContentReadStream() =>
                ConsumeStream();

            protected sealed override void Dispose(bool disposing)
            {
                if (disposing)
                {
                    if (_stream != null)
                    {
                        _stream.Dispose();
                        _stream = null;
                    }
                }

                base.Dispose(disposing);
            }
        }
    }
}
