// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
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
            _state.TcsQueryDataAvailable.Task.ContinueWith((previousTask) => 
                {
                    if (previousTask.IsFaulted)
                    {
                        _state.DisposeCtrReadFromResponseStream();
                        _state.TcsReadFromResponseStream.TrySetException(previousTask.Exception.InnerException);
                    }
                    else if (previousTask.IsCanceled || token.IsCancellationRequested)
                    {
                        _state.DisposeCtrReadFromResponseStream();
                        _state.TcsReadFromResponseStream.TrySetCanceled(token);
                    }
                    else
                    {
                        int bytesToRead;
                        int bytesAvailable = previousTask.Result;
                        if (bytesAvailable > count)
                        {
                            bytesToRead = count;
                        }
                        else
                        {
                            bytesToRead = bytesAvailable;
                        }
                        
                        lock (_state.Lock)
                        {
                            Debug.Assert(!_requestHandle.IsInvalid);
                            if (!Interop.WinHttp.WinHttpReadData(
                                _requestHandle,
                                Marshal.UnsafeAddrOfPinnedArrayElement(buffer, offset),
                                (uint)bytesToRead,
                                IntPtr.Zero))
                            {
                                _state.DisposeCtrReadFromResponseStream();
                                _state.TcsReadFromResponseStream.TrySetException(
                                    new IOException(SR.net_http_io_read, WinHttpException.CreateExceptionUsingLastError().InitializeStackTrace()));
                            }
                        }
                    }
                }, 
                CancellationToken.None, TaskContinuationOptions.None, TaskScheduler.Default);

            // Register callback on cancellation token to cancel any pending WinHTTP operation.
            if (token.CanBeCanceled)
            {
                WinHttpTraceHelper.Trace("WinHttpResponseStream.ReadAsync: registering for cancellation token request");
                _state.CtrReadFromResponseStream =
                    token.Register(s => ((WinHttpResponseStream)s).CancelPendingResponseStreamReadOperation(), this);
            }
            else
            {
                WinHttpTraceHelper.Trace("WinHttpResponseStream.ReadAsync: received no cancellation token");
            }

            lock (_state.Lock)
            {
                Debug.Assert(!_requestHandle.IsInvalid);
                if (!Interop.WinHttp.WinHttpQueryDataAvailable(_requestHandle, IntPtr.Zero))
                {
                    _state.DisposeCtrReadFromResponseStream();
                    _state.TcsReadFromResponseStream.TrySetException(
                        new IOException(SR.net_http_io_read, WinHttpException.CreateExceptionUsingLastError().InitializeStackTrace()));
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
        
        // The only way to abort pending async operations in WinHTTP is to close the request handle.
        // This causes WinHTTP to cancel any pending I/O and accelerating its callbacks on the handle.
        // This causes our related TaskCompletionSource objects to move to a terminal state.
        //
        // We only want to dispose the handle if we are actually waiting for a pending WinHTTP I/O to complete,
        // meaning that we are await'ing for a Task to complete. While we could simply call dispose without
        // a pending operation, it would cause random failures in the other threads when we expect a valid handle.
        private void CancelPendingResponseStreamReadOperation()
        {
            WinHttpTraceHelper.Trace("WinHttpResponseStream.CancelPendingResponseStreamReadOperation");
            lock (_state.Lock)
            {
                WinHttpTraceHelper.Trace(
                    string.Format("WinHttpResponseStream.CancelPendingResponseStreamReadOperation: {0} {1}",
                    (int)_state.TcsQueryDataAvailable.Task.Status, (int)_state.TcsReadFromResponseStream.Task.Status));
                if (!_state.TcsQueryDataAvailable.Task.IsCompleted)
                {
                    Debug.Assert(_requestHandle != null);
                    Debug.Assert(!_requestHandle.IsInvalid);
                    
                    WinHttpTraceHelper.Trace("WinHttpResponseStream.CancelPendingResponseStreamReadOperation: before dispose");
                    _requestHandle.Dispose();
                    WinHttpTraceHelper.Trace("WinHttpResponseStream.CancelPendingResponseStreamReadOperation: after dispose");
                }
            }
        }        
    }
}
