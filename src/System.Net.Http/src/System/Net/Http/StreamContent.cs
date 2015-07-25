// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace System.Net.Http
{
    public class StreamContent : HttpContent
    {
        private const int defaultBufferSize = 4096;

        private readonly Stream _content;
        private readonly int _bufferSize;
        private bool _contentConsumed;
        private readonly long _start;

        public StreamContent(Stream content)
            : this(content, defaultBufferSize)
        {
        }

        public StreamContent(Stream content, int bufferSize)
        {
            if (content == null)
            {
                throw new ArgumentNullException("content");
            }
            if (bufferSize <= 0)
            {
                throw new ArgumentOutOfRangeException("bufferSize");
            }

            _content = content;
            _bufferSize = bufferSize;
            if (content.CanSeek)
            {
                _start = content.Position;
            }
            if (Logging.On) Logging.Associate(Logging.Http, this, content);
        }

        protected override async Task SerializeToStreamAsync(Stream stream, TransportContext context)
        {
            Debug.Assert(stream != null);

            PrepareContent();

            await _content.CopyToAsync(stream, _bufferSize).ConfigureAwait(false);

            try
            {
                // If the stream can't be re-read, make sure that it gets disposed once it is consumed.
                if (!_content.CanSeek)
                {
                    _content.Dispose();
                }
            }
            catch (Exception e)
            {
                // Dispose() should never throw, but since we're on an async codepath, make sure to catch the exception.
                if (Logging.On) Logging.Exception(Logging.Http, this, "SerializeToStreamAsync", e);
            }
        }

        protected internal override bool TryComputeLength(out long length)
        {
            if (_content.CanSeek)
            {
                length = _content.Length - _start;
                return true;
            }
            else
            {
                length = 0;
                return false;
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _content.Dispose();
            }
            base.Dispose(disposing);
        }

        protected override Task<Stream> CreateContentReadStreamAsync()
        {
            // Wrap the stream with a read-only stream to prevent someone from writing to the stream.
            return Task.FromResult<Stream>(new ReadOnlyStream(_content));
        }

        private void PrepareContent()
        {
            if (_contentConsumed)
            {
                // If the content needs to be written to a target stream a 2nd time, then the stream must support
                // seeking (e.g. a FileStream), otherwise the stream can't be copied a second time to a target 
                // stream (e.g. a NetworkStream).
                if (_content.CanSeek)
                {
                    _content.Position = _start;
                }
                else
                {
                    throw new InvalidOperationException(SR.net_http_content_stream_already_read);
                }
            }

            _contentConsumed = true;
        }

        private class ReadOnlyStream : DelegatingStream
        {
            public override bool CanWrite
            {
                get { return false; }
            }

            public override int WriteTimeout
            {
                get { throw new NotSupportedException(SR.net_http_content_readonly_stream); }
                set { throw new NotSupportedException(SR.net_http_content_readonly_stream); }
            }

            public ReadOnlyStream(Stream innerStream)
                : base(innerStream)
            {
            }

            public override void Flush()
            {
                throw new NotSupportedException(SR.net_http_content_readonly_stream);
            }

            public override Task FlushAsync(CancellationToken cancellationToken)
            {
                throw new NotSupportedException(SR.net_http_content_readonly_stream);
            }

            public override void SetLength(long value)
            {
                throw new NotSupportedException(SR.net_http_content_readonly_stream);
            }

            public override void Write(byte[] buffer, int offset, int count)
            {
                throw new NotSupportedException(SR.net_http_content_readonly_stream);
            }

            public override void WriteByte(byte value)
            {
                throw new NotSupportedException(SR.net_http_content_readonly_stream);
            }

            public override Task WriteAsync(byte[] buffer, int offset, int count, Threading.CancellationToken cancellationToken)
            {
                throw new NotSupportedException(SR.net_http_content_readonly_stream);
            }
        }
    }
}
