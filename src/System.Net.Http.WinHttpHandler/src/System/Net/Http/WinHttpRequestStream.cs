// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using SafeWinHttpHandle = Interop.WinHttp.SafeWinHttpHandle;

namespace System.Net.Http
{
    internal class WinHttpRequestStream : Stream
    {
        private static byte[] s_crLfTerminator = new byte[] { 0x0d, 0x0a }; // "\r\n"
        private static byte[] s_endChunk = new byte[] { 0x30, 0x0d, 0x0a, 0x0d, 0x0a }; // "0\r\n\r\n"

        private volatile bool _disposed;
        private WinHttpRequestState _state;
        private bool _chunkedMode;

        // TODO (Issue 2505): temporary pinned buffer caches of 1 item. Will be replaced by PinnableBufferCache.
        private GCHandle _cachedSendPinnedBuffer;

        internal WinHttpRequestStream(WinHttpRequestState state, bool chunkedMode)
        {
            _state = state;
            _chunkedMode = chunkedMode;
        }

        public override bool CanRead
        {
            get
            {
                return false;
            }
        }

        public override bool CanSeek
        {
            get
            {
                return false;
            }
        }

        public override bool CanWrite
        {
            get
            {
                return !_disposed;
            }
        }

        public override long Length
        {
            get
            {
                CheckDisposed();
                throw new NotSupportedException();
            }
        }

        public override long Position
        {
            get
            {
                CheckDisposed();
                throw new NotSupportedException();
            }

            set
            {
                CheckDisposed();
                throw new NotSupportedException();
            }
        }

        public override void Flush()
        {
            // Nothing to do.
        }

        public override Task FlushAsync(CancellationToken cancellationToken)
        {
            return cancellationToken.IsCancellationRequested ?
                Task.FromCanceled(cancellationToken) :
                Task.CompletedTask;
        }

        public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken token)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException("buffer");
            }

            if (offset < 0)
            {
                throw new ArgumentOutOfRangeException("offset");
            }

            if (count < 0)
            {
                throw new ArgumentOutOfRangeException("count");
            }

            if (count > buffer.Length - offset)
            {
                throw new ArgumentException("buffer");
            }

            if (token.IsCancellationRequested)
            {
                var tcs = new TaskCompletionSource<int>();
                tcs.TrySetCanceled(token);
                return tcs.Task;
            }

            CheckDisposed();

            if (_state.TcsInternalWriteDataToRequestStream != null && 
                !_state.TcsInternalWriteDataToRequestStream.Task.IsCompleted)
            {
                throw new InvalidOperationException(SR.net_http_no_concurrent_io_allowed);
            }

            return InternalWriteAsync(buffer, offset, count, token);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            WriteAsync(buffer, offset, count, CancellationToken.None).GetAwaiter().GetResult();
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            CheckDisposed();
            throw new NotSupportedException();
        }

        public override void SetLength(long value)
        {
            CheckDisposed();
            throw new NotSupportedException();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            CheckDisposed();
            throw new NotSupportedException();
        }

        internal async Task EndUploadAsync(CancellationToken token)
        {
            if (_chunkedMode)
            {
                await InternalWriteDataAsync(s_endChunk, 0, s_endChunk.Length, token);
            }
        }
        
        protected override void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                _disposed = true;

                // TODO (Issue 2508): Pinned buffers must be released in the callback, when it is guaranteed no further
                // operations will be made to the send/receive buffers.
                if (_cachedSendPinnedBuffer.IsAllocated)
                {
                    _cachedSendPinnedBuffer.Free();
                }
            }

            base.Dispose(disposing);
        }

        private void CheckDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(this.GetType().FullName);
            }
        }

        private Task InternalWriteAsync(byte[] buffer, int offset, int count, CancellationToken token)
        {
            return _chunkedMode ?
                InternalWriteChunkedModeAsync(buffer, offset, count, token) :
                InternalWriteDataAsync(buffer, offset, count, token);            
        }

        private async Task InternalWriteChunkedModeAsync(byte[] buffer, int offset, int count, CancellationToken token)
        {
            // WinHTTP does not fully support chunked uploads. It simply allows one to omit the 'Content-Length' header
            // and instead use the 'Transfer-Encoding: chunked' header. The caller is still required to encode the
            // request body according to chunking rules.
            Debug.Assert(_chunkedMode);

            string chunkSizeString = String.Format("{0:x}\r\n", count);
            byte[] chunkSize = Encoding.UTF8.GetBytes(chunkSizeString);

            await InternalWriteDataAsync(chunkSize, 0, chunkSize.Length, token);

            await InternalWriteDataAsync(buffer, offset, count, token);
            await InternalWriteDataAsync(s_crLfTerminator, 0, s_crLfTerminator.Length, token);
        }

        private Task<bool> InternalWriteDataAsync(byte[] buffer, int offset, int count, CancellationToken token)
        {
            // TODO (Issue 2505): replace with PinnableBufferCache.
            if (!_cachedSendPinnedBuffer.IsAllocated || _cachedSendPinnedBuffer.Target != buffer)
            {
                if (_cachedSendPinnedBuffer.IsAllocated)
                {
                    _cachedSendPinnedBuffer.Free();
                }

                _cachedSendPinnedBuffer = GCHandle.Alloc(buffer, GCHandleType.Pinned);
            }

            _state.TcsInternalWriteDataToRequestStream = new TaskCompletionSource<bool>();
            
            lock (_state.Lock)
            {
                if (!Interop.WinHttp.WinHttpWriteData(
                    _state.RequestHandle,
                    Marshal.UnsafeAddrOfPinnedArrayElement(buffer, offset),
                    (uint)count,
                    IntPtr.Zero))
                {
                    _state.TcsInternalWriteDataToRequestStream.TrySetException(
                        new IOException(SR.net_http_io_write, WinHttpException.CreateExceptionUsingLastError()));
                }
            }

            // TODO: Issue #2165. Register callback on cancellation token to cancel WinHTTP operation.

            return _state.TcsInternalWriteDataToRequestStream.Task;
        }
    }
}
