// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.IO;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.Win32.SafeHandles;

namespace System.Net
{
    internal sealed unsafe class HttpResponseStream : Stream
    {
        private readonly HttpListenerContext _httpContext;
        private long _leftToWrite = long.MinValue;
        private bool _closed;
        private bool _inOpaqueMode;
        // The last write needs special handling to cancel.
        private HttpResponseStreamAsyncResult _lastWrite;

        internal HttpResponseStream(HttpListenerContext httpContext)
        {
            if (NetEventSource.IsEnabled) NetEventSource.Info(this, $"httpContect: {httpContext}");
            _httpContext = httpContext;
        }

        internal Interop.HttpApi.HTTP_FLAGS ComputeLeftToWrite()
        {
            if (NetEventSource.IsEnabled) NetEventSource.Info(this, "_LeftToWrite:" + _leftToWrite);
            Interop.HttpApi.HTTP_FLAGS flags = Interop.HttpApi.HTTP_FLAGS.NONE;
            if (!_httpContext.Response.ComputedHeaders)
            {
                flags = _httpContext.Response.ComputeHeaders();
            }
            if (_leftToWrite == long.MinValue)
            {
                Interop.HttpApi.HTTP_VERB method = _httpContext.GetKnownMethod();
                _leftToWrite = method != Interop.HttpApi.HTTP_VERB.HttpVerbHEAD ? _httpContext.Response.ContentLength64 : 0;
                if (NetEventSource.IsEnabled) NetEventSource.Info(this, "_LeftToWrite:" + _leftToWrite);
            }
            return flags;
        }

        public override bool CanSeek => false;

        public override bool CanWrite => true;

        public override bool CanRead => false;

        internal bool Closed => _closed;

        internal HttpListenerContext InternalHttpContext => _httpContext;

        internal void SetClosedFlag()
        {
            _closed = true;
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
            throw new InvalidOperationException(SR.net_writeonlystream);
        }

        public override IAsyncResult BeginRead(byte[] buffer, int offset, int size, AsyncCallback callback, object state)
        {
            throw new InvalidOperationException(SR.net_writeonlystream);
        }

        public override int EndRead(IAsyncResult asyncResult)
        {
            throw new InvalidOperationException(SR.net_writeonlystream);
        }

        public override void Write(byte[] buffer, int offset, int size)
        {
            if (NetEventSource.IsEnabled)
            {
                NetEventSource.Enter(this);
                NetEventSource.Info(this, "buffer.Length:" + buffer.Length + " size:" + size + " offset:" + offset);
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
            Interop.HttpApi.HTTP_FLAGS flags = ComputeLeftToWrite();
            if (_closed || (size == 0 && _leftToWrite != 0))
            {
                if (NetEventSource.IsEnabled) NetEventSource.Exit(this);
                return;
            }
            if (_leftToWrite >= 0 && size > _leftToWrite)
            {
                throw new ProtocolViolationException(SR.net_entitytoobig);
            }

            uint statusCode;
            uint dataToWrite = (uint)size;
            SafeLocalAllocHandle bufferAsIntPtr = null;
            IntPtr pBufferAsIntPtr = IntPtr.Zero;
            bool sentHeaders = _httpContext.Response.SentHeaders;
            try
            {
                if (size == 0)
                {
                    statusCode = _httpContext.Response.SendHeaders(null, null, flags, false);
                }
                else
                {
                    fixed (byte* pDataBuffer = buffer)
                    {
                        byte* pBuffer = pDataBuffer;
                        if (_httpContext.Response.BoundaryType == BoundaryType.Chunked)
                        {
                            string chunkHeader = size.ToString("x", CultureInfo.InvariantCulture);
                            dataToWrite = dataToWrite + (uint)(chunkHeader.Length + 4);
                            bufferAsIntPtr = SafeLocalAllocHandle.LocalAlloc((int)dataToWrite);
                            pBufferAsIntPtr = bufferAsIntPtr.DangerousGetHandle();
                            for (int i = 0; i < chunkHeader.Length; i++)
                            {
                                Marshal.WriteByte(pBufferAsIntPtr, i, (byte)chunkHeader[i]);
                            }
                            Marshal.WriteInt16(pBufferAsIntPtr, chunkHeader.Length, 0x0A0D);
                            Marshal.Copy(buffer, offset, pBufferAsIntPtr + chunkHeader.Length + 2, size);
                            Marshal.WriteInt16(pBufferAsIntPtr, (int)(dataToWrite - 2), 0x0A0D);
                            pBuffer = (byte*)pBufferAsIntPtr;
                            offset = 0;
                        }
                        Interop.HttpApi.HTTP_DATA_CHUNK dataChunk = new Interop.HttpApi.HTTP_DATA_CHUNK();
                        dataChunk.DataChunkType = Interop.HttpApi.HTTP_DATA_CHUNK_TYPE.HttpDataChunkFromMemory;
                        dataChunk.pBuffer = (byte*)(pBuffer + offset);
                        dataChunk.BufferLength = dataToWrite;

                        flags |= _leftToWrite == size ? Interop.HttpApi.HTTP_FLAGS.NONE : Interop.HttpApi.HTTP_FLAGS.HTTP_SEND_RESPONSE_FLAG_MORE_DATA;
                        if (!sentHeaders)
                        {
                            statusCode = _httpContext.Response.SendHeaders(&dataChunk, null, flags, false);
                        }
                        else
                        {
                            if (NetEventSource.IsEnabled) NetEventSource.Info(this, "Calling Interop.HttpApi.HttpSendResponseEntityBody");

                            statusCode =
                                Interop.HttpApi.HttpSendResponseEntityBody(
                                    _httpContext.RequestQueueHandle,
                                    _httpContext.RequestId,
                                    (uint)flags,
                                    1,
                                    &dataChunk,
                                    null,
                                    SafeLocalAllocHandle.Zero,
                                    0,
                                    null,
                                    null);

                            if (NetEventSource.IsEnabled) NetEventSource.Info(this, "Call to Interop.HttpApi.HttpSendResponseEntityBody returned:" + statusCode);
                            if (_httpContext.Listener.IgnoreWriteExceptions)
                            {
                                if (NetEventSource.IsEnabled) NetEventSource.Info(this, "Write() suppressing error");
                                statusCode = Interop.HttpApi.ERROR_SUCCESS;
                            }
                        }
                    }
                }
            }
            finally
            {
                // free unmanaged buffer
                bufferAsIntPtr?.Close();
            }

            if (statusCode != Interop.HttpApi.ERROR_SUCCESS && statusCode != Interop.HttpApi.ERROR_HANDLE_EOF)
            {
                Exception exception = new HttpListenerException((int)statusCode);
                if (NetEventSource.IsEnabled) NetEventSource.Error(this, exception.ToString());
                _closed = true;
                _httpContext.Abort();
                throw exception;
            }
            UpdateAfterWrite(dataToWrite);
            if (NetEventSource.IsEnabled) NetEventSource.DumpBuffer(this, buffer, offset, (int)dataToWrite);
            if (NetEventSource.IsEnabled) NetEventSource.Exit(this);
        }


        public override IAsyncResult BeginWrite(byte[] buffer, int offset, int size, AsyncCallback callback, object state)
        {
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
            Interop.HttpApi.HTTP_FLAGS flags = ComputeLeftToWrite();
            if (_closed || (size == 0 && _leftToWrite != 0))
            {
                if (NetEventSource.IsEnabled) NetEventSource.Exit(this);
                HttpResponseStreamAsyncResult result = new HttpResponseStreamAsyncResult(this, state, callback);
                result.InvokeCallback((uint)0);
                return result;
            }
            if (_leftToWrite >= 0 && size > _leftToWrite)
            {
                throw new ProtocolViolationException(SR.net_entitytoobig);
            }

            uint statusCode;
            uint bytesSent = 0;
            flags |= _leftToWrite == size ? Interop.HttpApi.HTTP_FLAGS.NONE : Interop.HttpApi.HTTP_FLAGS.HTTP_SEND_RESPONSE_FLAG_MORE_DATA;
            bool sentHeaders = _httpContext.Response.SentHeaders;
            HttpResponseStreamAsyncResult asyncResult = new HttpResponseStreamAsyncResult(this, state, callback, buffer, offset, size, _httpContext.Response.BoundaryType == BoundaryType.Chunked, sentHeaders, _httpContext.RequestQueueBoundHandle);

            // Update m_LeftToWrite now so we can queue up additional BeginWrite's without waiting for EndWrite.
            UpdateAfterWrite((uint)((_httpContext.Response.BoundaryType == BoundaryType.Chunked) ? 0 : size));

            try
            {
                if (!sentHeaders)
                {
                    statusCode = _httpContext.Response.SendHeaders(null, asyncResult, flags, false);
                }
                else
                {
                    if (NetEventSource.IsEnabled) NetEventSource.Info(this, "Calling Interop.HttpApi.HttpSendResponseEntityBody");

                    statusCode =
                        Interop.HttpApi.HttpSendResponseEntityBody(
                            _httpContext.RequestQueueHandle,
                            _httpContext.RequestId,
                            (uint)flags,
                            asyncResult.dataChunkCount,
                            asyncResult.pDataChunks,
                            &bytesSent,
                            SafeLocalAllocHandle.Zero,
                            0,
                            asyncResult._pOverlapped,
                            null);

                    if (NetEventSource.IsEnabled) NetEventSource.Info(this, "Call to Interop.HttpApi.HttpSendResponseEntityBody returned:" + statusCode);
                }
            }
            catch (Exception e)
            {
                if (NetEventSource.IsEnabled) NetEventSource.Error(this, e.ToString());
                asyncResult.InternalCleanup();
                _closed = true;
                _httpContext.Abort();
                throw;
            }

            if (statusCode != Interop.HttpApi.ERROR_SUCCESS && statusCode != Interop.HttpApi.ERROR_IO_PENDING)
            {
                asyncResult.InternalCleanup();
                if (_httpContext.Listener.IgnoreWriteExceptions && sentHeaders)
                {
                    if (NetEventSource.IsEnabled) NetEventSource.Info(this, "BeginWrite() Suppressing error");
                }
                else
                {
                    Exception exception = new HttpListenerException((int)statusCode);
                    if (NetEventSource.IsEnabled) NetEventSource.Error(this, exception.ToString());
                    _closed = true;
                    _httpContext.Abort();
                    throw exception;
                }
            }

            if (statusCode == Interop.HttpApi.ERROR_SUCCESS && HttpListener.SkipIOCPCallbackOnSuccess)
            {
                // IO operation completed synchronously - callback won't be called to signal completion.
                asyncResult.IOCompleted(statusCode, bytesSent);
            }

            // Last write, cache it for special cancelation handling.
            if ((flags & Interop.HttpApi.HTTP_FLAGS.HTTP_SEND_RESPONSE_FLAG_MORE_DATA) == 0)
            {
                _lastWrite = asyncResult;
            }

            if (NetEventSource.IsEnabled) NetEventSource.Exit(this);
            return asyncResult;
        }

        public override void EndWrite(IAsyncResult asyncResult)
        {
            if (NetEventSource.IsEnabled)
            {
                NetEventSource.Enter(this);
                NetEventSource.Info(this, $"asyncResult:{asyncResult}");
            }
            if (asyncResult == null)
            {
                throw new ArgumentNullException(nameof(asyncResult));
            }
            HttpResponseStreamAsyncResult castedAsyncResult = asyncResult as HttpResponseStreamAsyncResult;
            if (castedAsyncResult == null || castedAsyncResult.AsyncObject != this)
            {
                throw new ArgumentException(SR.net_io_invalidasyncresult, nameof(asyncResult));
            }
            if (castedAsyncResult.EndCalled)
            {
                throw new InvalidOperationException(SR.Format(SR.net_io_invalidendcall, nameof(EndWrite)));
            }
            castedAsyncResult.EndCalled = true;
            // wait & then check for errors
            object returnValue = castedAsyncResult.InternalWaitForCompletion();

            Exception exception = returnValue as Exception;
            if (exception != null)
            {
                if (NetEventSource.IsEnabled) NetEventSource.Error(this, "Rethrowing exception:" + exception);
                _closed = true;
                _httpContext.Abort();
                ExceptionDispatchInfo.Capture(exception).Throw();
            }

            if (NetEventSource.IsEnabled) NetEventSource.Exit(this);
        }

        private void UpdateAfterWrite(uint dataWritten)
        {
            if (NetEventSource.IsEnabled) NetEventSource.Info(this, "dataWritten:" + dataWritten + " _leftToWrite:" + _leftToWrite + " _closed:" + _closed);
            if (!_inOpaqueMode)
            {
                if (_leftToWrite > 0)
                {
                    // keep track of the data transferred
                    _leftToWrite -= dataWritten;
                }
                if (_leftToWrite == 0)
                {
                    // in this case we already passed 0 as the flag, so we don't need to call HttpSendResponseEntityBody() when we Close()
                    _closed = true;
                }
            }
            if (NetEventSource.IsEnabled) NetEventSource.Info(this, "dataWritten:" + dataWritten + " _leftToWrite:" + _leftToWrite + " _closed:" + _closed);
        }

        private static readonly byte[] s_chunkTerminator = new byte[] { (byte)'0', (byte)'\r', (byte)'\n', (byte)'\r', (byte)'\n' };

        protected override void Dispose(bool disposing)
        {
            if (NetEventSource.IsEnabled) NetEventSource.Enter(this);

            try
            {
                if (disposing)
                {
                    if (NetEventSource.IsEnabled) NetEventSource.Info(this, "_closed:" + _closed);
                    if (_closed)
                    {
                        if (NetEventSource.IsEnabled) NetEventSource.Exit(this);
                        return;
                    }
                    _closed = true;
                    Interop.HttpApi.HTTP_FLAGS flags = ComputeLeftToWrite();
                    if (_leftToWrite > 0 && !_inOpaqueMode)
                    {
                        throw new InvalidOperationException(SR.net_io_notenoughbyteswritten);
                    }
                    bool sentHeaders = _httpContext.Response.SentHeaders;
                    if (sentHeaders && _leftToWrite == 0)
                    {
                        if (NetEventSource.IsEnabled) NetEventSource.Exit(this);
                        return;
                    }

                    uint statusCode = 0;
                    if ((_httpContext.Response.BoundaryType == BoundaryType.Chunked || _httpContext.Response.BoundaryType == BoundaryType.None) && (String.Compare(_httpContext.Request.HttpMethod, "HEAD", StringComparison.OrdinalIgnoreCase) != 0))
                    {
                        if (_httpContext.Response.BoundaryType == BoundaryType.None)
                        {
                            flags |= Interop.HttpApi.HTTP_FLAGS.HTTP_SEND_RESPONSE_FLAG_DISCONNECT;
                        }
                        fixed (void* pBuffer = &s_chunkTerminator[0])
                        {
                            Interop.HttpApi.HTTP_DATA_CHUNK* pDataChunk = null;
                            if (_httpContext.Response.BoundaryType == BoundaryType.Chunked)
                            {
                                Interop.HttpApi.HTTP_DATA_CHUNK dataChunk = new Interop.HttpApi.HTTP_DATA_CHUNK();
                                dataChunk.DataChunkType = Interop.HttpApi.HTTP_DATA_CHUNK_TYPE.HttpDataChunkFromMemory;
                                dataChunk.pBuffer = (byte*)pBuffer;
                                dataChunk.BufferLength = (uint)s_chunkTerminator.Length;
                                pDataChunk = &dataChunk;
                            }
                            if (!sentHeaders)
                            {
                                statusCode = _httpContext.Response.SendHeaders(pDataChunk, null, flags, false);
                            }
                            else
                            {
                                if (NetEventSource.IsEnabled) NetEventSource.Info(this, "Calling Interop.HttpApi.HttpSendResponseEntityBody");

                                statusCode =
                                    Interop.HttpApi.HttpSendResponseEntityBody(
                                        _httpContext.RequestQueueHandle,
                                        _httpContext.RequestId,
                                        (uint)flags,
                                        pDataChunk != null ? (ushort)1 : (ushort)0,
                                        pDataChunk,
                                        null,
                                        SafeLocalAllocHandle.Zero,
                                        0,
                                        null,
                                        null);

                                if (NetEventSource.IsEnabled) NetEventSource.Info(this, "Call to Interop.HttpApi.HttpSendResponseEntityBody returned:" + statusCode);
                                if (_httpContext.Listener.IgnoreWriteExceptions)
                                {
                                    if (NetEventSource.IsEnabled) NetEventSource.Info(this, "Suppressing error");
                                    statusCode = Interop.HttpApi.ERROR_SUCCESS;
                                }
                            }
                        }
                    }
                    else
                    {
                        if (!sentHeaders)
                        {
                            statusCode = _httpContext.Response.SendHeaders(null, null, flags, false);
                        }
                    }
                    if (statusCode != Interop.HttpApi.ERROR_SUCCESS && statusCode != Interop.HttpApi.ERROR_HANDLE_EOF)
                    {
                        Exception exception = new HttpListenerException((int)statusCode);
                        if (NetEventSource.IsEnabled) NetEventSource.Error(this, exception.ToString());
                        _httpContext.Abort();
                        throw exception;
                    }
                    _leftToWrite = 0;
                }
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
            _leftToWrite = long.MaxValue;
        }

        // The final Content-Length async write can only be cancelled by CancelIoEx.
        // Sync can only be cancelled by CancelSynchronousIo, but we don't attempt this right now.
        internal void CancelLastWrite(SafeHandle requestQueueHandle)
        {
            HttpResponseStreamAsyncResult asyncState = _lastWrite;
            if (asyncState != null && !asyncState.IsCompleted)
            {
                // It is safe to ignore the return value on a cancel operation because the connection is being closed
                Interop.Kernel32.CancelIoEx(requestQueueHandle, asyncState._pOverlapped);
            }
        }
    }
}
