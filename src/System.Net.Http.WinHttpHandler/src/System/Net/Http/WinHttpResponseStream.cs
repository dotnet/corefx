// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

using SafeWinHttpHandle = Interop.WinHttp.SafeWinHttpHandle;

namespace System.Net.Http
{
    internal class WinHttpResponseStream : Stream
    {
        private volatile bool _disposed;
        private readonly WinHttpRequestState _state;
        private SafeWinHttpHandle _requestHandle;
        
        internal WinHttpResponseStream(SafeWinHttpHandle requestHandle, WinHttpRequestState state)
        {
            _state = state;
            _requestHandle = requestHandle;
        }

        public override bool CanRead
        {
            get
            {
                return !_disposed;
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
                return false;
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

        public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken token)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException(nameof(buffer));
            }

            if (offset < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(offset));
            }

            if (count < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(count));
            }

            if (count > buffer.Length - offset)
            {
                throw new ArgumentException(SR.net_http_buffer_insufficient_length, nameof(buffer));
            }

            if (token.IsCancellationRequested)
            {
                return Task.FromCanceled<int>(token);
            }

            CheckDisposed();

            if (_state.TcsReadFromResponseStream != null && !_state.TcsReadFromResponseStream.Task.IsCompleted)
            {
                throw new InvalidOperationException(SR.net_http_no_concurrent_io_allowed);
            }

            _state.PinReceiveBuffer(buffer);

            _state.TcsReadFromResponseStream =
                new TaskCompletionSource<int>(TaskCreationOptions.RunContinuationsAsynchronously);

            _state.TcsQueryDataAvailable =
                new TaskCompletionSource<int>(TaskCreationOptions.RunContinuationsAsynchronously);
            
            _state.TcsQueryDataAvailable.Task.ContinueWith((previousTask, state) => 
            {
                var tuple = (Tuple<WinHttpResponseStream, byte[], int, int, CancellationToken>)state;
                
                var self = tuple.Item1;
                WinHttpRequestState requestState = self._state;
                SafeWinHttpHandle requestHandle = self._requestHandle;
                
                byte[] array = tuple.Item2;
                int index = tuple.Item3;
                int length = tuple.Item4;
                CancellationToken cancellationToken = tuple.Item5;
                
                if (previousTask.IsFaulted)
                {
                    requestState.TcsReadFromResponseStream.TrySetException(previousTask.Exception.InnerException);
                }
                else if (previousTask.IsCanceled || cancellationToken.IsCancellationRequested)
                {
                    requestState.TcsReadFromResponseStream.TrySetCanceled(cancellationToken);
                }
                else
                {
                    int bytesAvailable = previousTask.Result;
                    int bytesToRead = Math.Min(length, bytesAvailable);
                    
                    lock (requestState.Lock)
                    {
                        if (!Interop.WinHttp.WinHttpReadData(
                            requestHandle,
                            Marshal.UnsafeAddrOfPinnedArrayElement(array, index),
                            (uint)bytesToRead,
                            IntPtr.Zero))
                        {
                            requestState.TcsReadFromResponseStream.TrySetException(
                                new IOException(SR.net_http_io_read, WinHttpException.CreateExceptionUsingLastError()));
                        }
                    }
                }
            },
            Tuple.Create(this, buffer, offset, count, token),
            CancellationToken.None, TaskContinuationOptions.None, TaskScheduler.Default);

            // TODO: Issue #2165. Register callback on cancellation token to cancel WinHTTP operation.
                
            lock (_state.Lock)
            {
                if (!Interop.WinHttp.WinHttpQueryDataAvailable(_requestHandle, IntPtr.Zero))
                {
                    _state.TcsReadFromResponseStream.TrySetException(
                        new IOException(SR.net_http_io_read, WinHttpException.CreateExceptionUsingLastError()));
                }
            }

            return _state.TcsReadFromResponseStream.Task;
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return ReadAsync(buffer, offset, count, CancellationToken.None).GetAwaiter().GetResult();
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

        public override void Write(byte[] buffer, int offset, int count)
        {
            CheckDisposed();
            throw new NotSupportedException();
        }

        protected override void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                _disposed = true;

                if (disposing)
                {
                    if (_requestHandle != null)
                    {
                        _requestHandle.Dispose();
                        _requestHandle = null;
                    }
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
    }
}
