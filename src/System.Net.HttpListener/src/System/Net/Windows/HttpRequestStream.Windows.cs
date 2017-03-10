// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.IO;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace System.Net
{
    internal sealed unsafe class HttpRequestStream : Stream
    {
        private readonly HttpListenerContext _httpContext;
        private uint _dataChunkOffset;
        private int _dataChunkIndex;
        private bool _closed;
        internal const int MaxReadSize = 0x20000; //http.sys recommends we limit reads to 128k
        private bool _inOpaqueMode;

        internal HttpRequestStream(HttpListenerContext httpContext)
        {
            if (NetEventSource.IsEnabled) NetEventSource.Info(this, $"httpContextt:{httpContext}");
            _httpContext = httpContext;
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

        public override bool CanRead
        {
            get
            {
                return true;
            }
        }

        internal bool Closed
        {
            get
            {
                return _closed;
            }
        }

        internal bool BufferedDataChunksAvailable
        {
            get
            {
                return _dataChunkIndex > -1;
            }
        }

        // This low level API should only be consumed if the caller can make sure that the state is not corrupted
        // WebSocketHttpListenerDuplexStream (a duplex wrapper around HttpRequestStream/HttpResponseStream)
        // is currenlty the only consumer of this API
        internal HttpListenerContext InternalHttpContext
        {
            get
            {
                return _httpContext;
            }
        }

        public override void Flush()
        {
        }

        public override Task FlushAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public override long Length
        {
            get
            {
                throw new NotSupportedException(SR.net_noseek);
            }
        }

        public override long Position
        {
            get
            {
                throw new NotSupportedException(SR.net_noseek);
            }
            set
            {
                throw new NotSupportedException(SR.net_noseek);
            }
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException(SR.net_noseek);
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException(SR.net_noseek);
        }

        public override int Read(byte[] buffer, int offset, int size)
        {
            if (NetEventSource.IsEnabled)
            {
                NetEventSource.Enter(this);
                NetEventSource.Info(this, "size:" + size + " offset:" + offset);
            }
            if (buffer == null)
            {
                throw new ArgumentNullException(nameof(buffer));
            }
            if (offset < 0 || offset > buffer.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(offset));
            }
            if (size < 0 || size > buffer.Length - offset)
            {
                throw new ArgumentOutOfRangeException(nameof(size));
            }
            if (size == 0 || _closed)
            {
                if (NetEventSource.IsEnabled) NetEventSource.Exit(this, "dataRead:0");
                return 0;
            }

            uint dataRead = 0;

            if (_dataChunkIndex != -1)
            {
                dataRead = Interop.HttpApi.GetChunks(_httpContext.Request.RequestBuffer, _httpContext.Request.OriginalBlobAddress, ref _dataChunkIndex, ref _dataChunkOffset, buffer, offset, size);
            }

            if (_dataChunkIndex == -1 && dataRead < size)
            {
                if (NetEventSource.IsEnabled) NetEventSource.Info(this, "size:" + size + " offset:" + offset);
                uint statusCode = 0;
                uint extraDataRead = 0;
                offset += (int)dataRead;
                size -= (int)dataRead;

                //the http.sys team recommends that we limit the size to 128kb
                if (size > MaxReadSize)
                {
                    size = MaxReadSize;
                }

                fixed (byte* pBuffer = buffer)
                {
                    // issue unmanaged blocking call
                    if (NetEventSource.IsEnabled) NetEventSource.Info(this, "Calling Interop.HttpApi.HttpReceiveRequestEntityBody");

                    uint flags = 0;

                    if (!_inOpaqueMode)
                    {
                        flags = (uint)Interop.HttpApi.HTTP_FLAGS.HTTP_RECEIVE_REQUEST_FLAG_COPY_BODY;
                    }

                    statusCode =
                        Interop.HttpApi.HttpReceiveRequestEntityBody(
                            _httpContext.RequestQueueHandle,
                            _httpContext.RequestId,
                            flags,
                            (void*)(pBuffer + offset),
                            (uint)size,
                            out extraDataRead,
                            null);

                    dataRead += extraDataRead;
                    if (NetEventSource.IsEnabled) NetEventSource.Info(this, "Call to Interop.HttpApi.HttpReceiveRequestEntityBody returned:" + statusCode + " dataRead:" + dataRead);
                }
                if (statusCode != Interop.HttpApi.ERROR_SUCCESS && statusCode != Interop.HttpApi.ERROR_HANDLE_EOF)
                {
                    Exception exception = new HttpListenerException((int)statusCode);
                    if (NetEventSource.IsEnabled) NetEventSource.Error(this, exception.ToString());
                    throw exception;
                }
                UpdateAfterRead(statusCode, dataRead);
            }
            if (NetEventSource.IsEnabled)
            {
                NetEventSource.DumpBuffer(this, buffer, offset, (int)dataRead);
                NetEventSource.Info(this, "returning dataRead:" + dataRead);
                NetEventSource.Exit(this, "dataRead:" + dataRead);
            }
            return (int)dataRead;
        }

        private void UpdateAfterRead(uint statusCode, uint dataRead)
        {
            if (NetEventSource.IsEnabled) NetEventSource.Info(this, "statusCode:" + statusCode + " _closed:" + _closed);
            if (statusCode == Interop.HttpApi.ERROR_HANDLE_EOF || dataRead == 0)
            {
                Close();
            }
            if (NetEventSource.IsEnabled) NetEventSource.Info(this, "statusCode:" + statusCode + " _closed:" + _closed);
        }

        public override IAsyncResult BeginRead(byte[] buffer, int offset, int size, AsyncCallback callback, object state)
        {
            if (NetEventSource.IsEnabled) NetEventSource.Enter(this);
            if (NetEventSource.IsEnabled) NetEventSource.Info(this, "buffer.Length:" + buffer.Length + " size:" + size + " offset:" + offset);
            if (buffer == null)
            {
                throw new ArgumentNullException(nameof(buffer));
            }
            if (offset < 0 || offset > buffer.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(offset));
            }
            if (size < 0 || size > buffer.Length - offset)
            {
                throw new ArgumentOutOfRangeException(nameof(size));
            }
            if (size == 0 || _closed)
            {
                if (NetEventSource.IsEnabled) NetEventSource.Exit(this);
                HttpRequestStreamAsyncResult result = new HttpRequestStreamAsyncResult(this, state, callback);
                result.InvokeCallback((uint)0);
                return result;
            }

            HttpRequestStreamAsyncResult asyncResult = null;

            uint dataRead = 0;
            if (_dataChunkIndex != -1)
            {
                dataRead = Interop.HttpApi.GetChunks(_httpContext.Request.RequestBuffer, _httpContext.Request.OriginalBlobAddress, ref _dataChunkIndex, ref _dataChunkOffset, buffer, offset, size);
                if (_dataChunkIndex != -1 && dataRead == size)
                {
                    asyncResult = new HttpRequestStreamAsyncResult(_httpContext.RequestQueueBoundHandle, this, state, callback, buffer, offset, (uint)size, 0);
                    asyncResult.InvokeCallback(dataRead);
                }
            }

            if (_dataChunkIndex == -1 && dataRead < size)
            {
                if (NetEventSource.IsEnabled) NetEventSource.Info(this, "size:" + size + " offset:" + offset);
                uint statusCode = 0;
                offset += (int)dataRead;
                size -= (int)dataRead;

                //the http.sys team recommends that we limit the size to 128kb
                if (size > MaxReadSize)
                {
                    size = MaxReadSize;
                }

                asyncResult = new HttpRequestStreamAsyncResult(_httpContext.RequestQueueBoundHandle, this, state, callback, buffer, offset, (uint)size, dataRead);
                uint bytesReturned;

                try
                {
                    fixed (byte* pBuffer = buffer)
                    {
                        // issue unmanaged blocking call
                        if (NetEventSource.IsEnabled) NetEventSource.Info(this, "Calling Interop.HttpApi.HttpReceiveRequestEntityBody");

                        uint flags = 0;

                        if (!_inOpaqueMode)
                        {
                            flags = (uint)Interop.HttpApi.HTTP_FLAGS.HTTP_RECEIVE_REQUEST_FLAG_COPY_BODY;
                        }

                        statusCode =
                            Interop.HttpApi.HttpReceiveRequestEntityBody(
                                _httpContext.RequestQueueHandle,
                                _httpContext.RequestId,
                                flags,
                                asyncResult._pPinnedBuffer,
                                (uint)size,
                                out bytesReturned,
                                asyncResult._pOverlapped);

                        if (NetEventSource.IsEnabled) NetEventSource.Info(this, "Call to Interop.HttpApi.HttpReceiveRequestEntityBody returned:" + statusCode + " dataRead:" + dataRead);
                    }
                }
                catch (Exception e)
                {
                    if (NetEventSource.IsEnabled) NetEventSource.Error(this, e.ToString());
                    asyncResult.InternalCleanup();
                    throw;
                }

                if (statusCode != Interop.HttpApi.ERROR_SUCCESS && statusCode != Interop.HttpApi.ERROR_IO_PENDING)
                {
                    asyncResult.InternalCleanup();
                    if (statusCode == Interop.HttpApi.ERROR_HANDLE_EOF)
                    {
                        asyncResult = new HttpRequestStreamAsyncResult(this, state, callback, dataRead);
                        asyncResult.InvokeCallback((uint)0);
                    }
                    else
                    {
                        Exception exception = new HttpListenerException((int)statusCode);
                        if (NetEventSource.IsEnabled) NetEventSource.Error(this, exception.ToString());
                        asyncResult.InternalCleanup();
                        throw exception;
                    }
                }
                else if (statusCode == Interop.HttpApi.ERROR_SUCCESS &&
                         HttpListener.SkipIOCPCallbackOnSuccess)
                {
                    // IO operation completed synchronously - callback won't be called to signal completion.
                    asyncResult.IOCompleted(statusCode, bytesReturned);
                }
            }
            if (NetEventSource.IsEnabled) NetEventSource.Exit(this);
            return asyncResult;
        }

        public override int EndRead(IAsyncResult asyncResult)
        {
            if (NetEventSource.IsEnabled)
            {
                NetEventSource.Enter(this);
                NetEventSource.Info(this, $"asyncResult: {asyncResult}");
            }
            if (asyncResult == null)
            {
                throw new ArgumentNullException(nameof(asyncResult));
            }
            HttpRequestStreamAsyncResult castedAsyncResult = asyncResult as HttpRequestStreamAsyncResult;
            if (castedAsyncResult == null || castedAsyncResult.AsyncObject != this)
            {
                throw new ArgumentException(SR.net_io_invalidasyncresult, nameof(asyncResult));
            }
            if (castedAsyncResult.EndCalled)
            {
                throw new InvalidOperationException(SR.Format(SR.net_io_invalidendcall, nameof(EndRead)));
            }
            castedAsyncResult.EndCalled = true;
            // wait & then check for errors
            object returnValue = castedAsyncResult.InternalWaitForCompletion();
            Exception exception = returnValue as Exception;
            if (exception != null)
            {
                if (NetEventSource.IsEnabled)
                {
                    NetEventSource.Info(this, "Rethrowing exception:" + exception);
                    NetEventSource.Error(this, exception.ToString());
                }
                ExceptionDispatchInfo.Capture(exception).Throw();
            }

            uint dataRead = (uint)returnValue;
            UpdateAfterRead((uint)castedAsyncResult.ErrorCode, dataRead);
            if (NetEventSource.IsEnabled)
            {
                NetEventSource.Info(this, $"returnValue:{returnValue}");
                NetEventSource.Exit(this);
            }

            return (int)dataRead + (int)castedAsyncResult._dataAlreadyRead;
        }

        public override void Write(byte[] buffer, int offset, int size)
        {
            throw new InvalidOperationException(SR.net_readonlystream);
        }


        public override IAsyncResult BeginWrite(byte[] buffer, int offset, int size, AsyncCallback callback, object state)
        {
            throw new InvalidOperationException(SR.net_readonlystream);
        }

        public override void EndWrite(IAsyncResult asyncResult)
        {
            throw new InvalidOperationException(SR.net_readonlystream);
        }

        protected override void Dispose(bool disposing)
        {
            if (NetEventSource.IsEnabled) NetEventSource.Enter(this);
            try
            {
                if (NetEventSource.IsEnabled) NetEventSource.Info(this, "_closed:" + _closed);
                _closed = true;
            }
            finally
            {
                base.Dispose(disposing);
            }
            if (NetEventSource.IsEnabled) NetEventSource.Exit(this);
        }

        internal void SwitchToOpaqueMode()
        {
            if (NetEventSource.IsEnabled) NetEventSource.Info(this);
            _inOpaqueMode = true;
        }

        // This low level API should only be consumed if the caller can make sure that the state is not corrupted
        // WebSocketHttpListenerDuplexStream (a duplex wrapper around HttpRequestStream/HttpResponseStream)
        // is currenlty the only consumer of this API
        internal uint GetChunks(byte[] buffer, int offset, int size)
        {
            return Interop.HttpApi.GetChunks(_httpContext.Request.RequestBuffer,
                _httpContext.Request.OriginalBlobAddress,
                ref _dataChunkIndex,
                ref _dataChunkOffset,
                buffer,
                offset,
                size);
        }

        private sealed unsafe class HttpRequestStreamAsyncResult : LazyAsyncResult
        {
            private ThreadPoolBoundHandle _boundHandle;
            internal NativeOverlapped* _pOverlapped;
            internal void* _pPinnedBuffer;
            internal uint _dataAlreadyRead = 0;

            private static readonly IOCompletionCallback s_IOCallback = new IOCompletionCallback(Callback);

            internal HttpRequestStreamAsyncResult(object asyncObject, object userState, AsyncCallback callback) : base(asyncObject, userState, callback)
            {
            }

            internal HttpRequestStreamAsyncResult(object asyncObject, object userState, AsyncCallback callback, uint dataAlreadyRead) : base(asyncObject, userState, callback)
            {
                _dataAlreadyRead = dataAlreadyRead;
            }

            internal HttpRequestStreamAsyncResult(ThreadPoolBoundHandle boundHandle, object asyncObject, object userState, AsyncCallback callback, byte[] buffer, int offset, uint size, uint dataAlreadyRead) : base(asyncObject, userState, callback)
            {
                _dataAlreadyRead = dataAlreadyRead;
                _boundHandle = boundHandle;
                _pOverlapped = boundHandle.AllocateNativeOverlapped(s_IOCallback, state: this, pinData: buffer);
                _pPinnedBuffer = (void*)(Marshal.UnsafeAddrOfPinnedArrayElement(buffer, offset));
            }

            internal void IOCompleted(uint errorCode, uint numBytes)
            {
                IOCompleted(this, errorCode, numBytes);
            }

            private static void IOCompleted(HttpRequestStreamAsyncResult asyncResult, uint errorCode, uint numBytes)
            {
                if (NetEventSource.IsEnabled) NetEventSource.Info(null, $"asyncResult: {asyncResult} errorCode:0x {errorCode.ToString("x8")} numBytes: {numBytes}");
                object result = null;
                try
                {
                    if (errorCode != Interop.HttpApi.ERROR_SUCCESS && errorCode != Interop.HttpApi.ERROR_HANDLE_EOF)
                    {
                        asyncResult.ErrorCode = (int)errorCode;
                        result = new HttpListenerException((int)errorCode);
                    }
                    else
                    {
                        result = numBytes;
                        if (NetEventSource.IsEnabled) NetEventSource.DumpBuffer(asyncResult, (IntPtr)asyncResult._pPinnedBuffer, (int)numBytes);
                    }
                    if (NetEventSource.IsEnabled) NetEventSource.Info(null, $"asyncResult: {asyncResult} calling Complete()");
                }
                catch (Exception e)
                {
                    result = e;
                }
                asyncResult.InvokeCallback(result);
            }

            private static unsafe void Callback(uint errorCode, uint numBytes, NativeOverlapped* nativeOverlapped)
            {
                HttpRequestStreamAsyncResult asyncResult = (HttpRequestStreamAsyncResult)ThreadPoolBoundHandle.GetNativeOverlappedState(nativeOverlapped);

                if (NetEventSource.IsEnabled) NetEventSource.Info(null, $"asyncResult: {asyncResult} errorCode:0x {errorCode.ToString("x8")} numBytes: {numBytes} nativeOverlapped:0x {((IntPtr)nativeOverlapped).ToString("x8")}");

                IOCompleted(asyncResult, errorCode, numBytes);
            }

            // Will be called from the base class upon InvokeCallback()
            protected override void Cleanup()
            {
                base.Cleanup();
                if (_pOverlapped != null)
                {
                    _boundHandle.FreeNativeOverlapped(_pOverlapped);
                }
            }
        }
    }
}
