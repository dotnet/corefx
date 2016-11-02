// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Win32.SafeHandles;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace System.Net.WebSockets
{
    internal class WebSocketHttpListenerDuplexStream : Stream, WebSocketBase.IWebSocketStream
    {
        private static readonly EventHandler<HttpListenerAsyncEventArgs> s_OnReadCompleted =
            new EventHandler<HttpListenerAsyncEventArgs>(OnReadCompleted);
        private static readonly EventHandler<HttpListenerAsyncEventArgs> s_OnWriteCompleted =
            new EventHandler<HttpListenerAsyncEventArgs>(OnWriteCompleted);
        private static readonly Func<Exception, bool> s_CanHandleException = new Func<Exception, bool>(CanHandleException);
        private static readonly Action<object> s_OnCancel = new Action<object>(OnCancel);
        private readonly HttpRequestStream m_InputStream;
        private readonly HttpResponseStream m_OutputStream;
        private HttpListenerContext m_Context;
        private bool m_InOpaqueMode;
        private WebSocketBase m_WebSocket;
        private HttpListenerAsyncEventArgs m_WriteEventArgs;
        private HttpListenerAsyncEventArgs m_ReadEventArgs;
        private TaskCompletionSource<object> m_WriteTaskCompletionSource;
        private TaskCompletionSource<int> m_ReadTaskCompletionSource;
        private int m_CleanedUp;

#if DEBUG
        private class OutstandingOperations
        {
            internal int m_Reads;
            internal int m_Writes;
        }

        private readonly OutstandingOperations m_OutstandingOperations = new OutstandingOperations();
#endif //DEBUG

        public WebSocketHttpListenerDuplexStream(HttpRequestStream inputStream,
            HttpResponseStream outputStream,
            HttpListenerContext context)
        {
            Debug.Assert(inputStream != null, "'inputStream' MUST NOT be NULL.");
            Debug.Assert(outputStream != null, "'outputStream' MUST NOT be NULL.");
            Debug.Assert(context != null, "'context' MUST NOT be NULL.");
            Debug.Assert(inputStream.CanRead, "'inputStream' MUST support read operations.");
            Debug.Assert(outputStream.CanWrite, "'outputStream' MUST support write operations.");

            m_InputStream = inputStream;
            m_OutputStream = outputStream;
            m_Context = context;

            if (WebSocketBase.LoggingEnabled)
            {
                NetEventSource.Associate(NetEventSource.ComponentType.WebSocket, inputStream, this);
                NetEventSource.Associate(NetEventSource.ComponentType.WebSocket, outputStream, this);
            }
        }

        public override bool CanRead
        {
            get
            {
                return m_InputStream.CanRead;
            }
        }

        public override bool CanSeek
        {
            get
            {
                return false;
            }
        }

        public override bool CanTimeout
        {
            get
            {
                return m_InputStream.CanTimeout && m_OutputStream.CanTimeout;
            }
        }

        public override bool CanWrite
        {
            get
            {
                return m_OutputStream.CanWrite;
            }
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

        public override int Read(byte[] buffer, int offset, int count)
        {
            return m_InputStream.Read(buffer, offset, count);
        }

        public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            WebSocketHelpers.ValidateBuffer(buffer, offset, count);

            return ReadAsyncCore(buffer, offset, count, cancellationToken);
        }

        private async Task<int> ReadAsyncCore(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            if (WebSocketBase.LoggingEnabled)
            {
                NetEventSource.Enter(NetEventSource.ComponentType.WebSocket, this, Methods.ReadAsyncCore,
                    WebSocketHelpers.GetTraceMsgForParameters(offset, count, cancellationToken));
            }

            CancellationTokenRegistration cancellationTokenRegistration = new CancellationTokenRegistration();

            int bytesRead = 0;
            try
            {
                if (cancellationToken.CanBeCanceled)
                {
                    cancellationTokenRegistration = cancellationToken.Register(s_OnCancel, this, false);
                }

                if (!m_InOpaqueMode)
                {
                    bytesRead = await m_InputStream.ReadAsync(buffer, offset, count, cancellationToken).SuppressContextFlow<int>();
                }
                else
                {
#if DEBUG
                    // When using fast path only one outstanding read is permitted. By switching into opaque mode
                    // via IWebSocketStream.SwitchToOpaqueMode (see more detailed comments in interface definition)
                    // caller takes responsibility for enforcing this constraint.
                    Debug.Assert(Interlocked.Increment(ref m_OutstandingOperations.m_Reads) == 1,
                        "Only one outstanding read allowed at any given time.");
#endif
                    m_ReadTaskCompletionSource = new TaskCompletionSource<int>();
                    m_ReadEventArgs.SetBuffer(buffer, offset, count);
                    if (!ReadAsyncFast(m_ReadEventArgs))
                    {
                        if (m_ReadEventArgs.Exception != null)
                        {
                            throw m_ReadEventArgs.Exception;
                        }

                        bytesRead = m_ReadEventArgs.BytesTransferred;
                    }
                    else
                    {
                        bytesRead = await m_ReadTaskCompletionSource.Task.SuppressContextFlow<int>();
                    }
                }
            }
            catch (Exception error)
            {
                if (s_CanHandleException(error))
                {
                    cancellationToken.ThrowIfCancellationRequested();
                }

                throw;
            }
            finally
            {
                cancellationTokenRegistration.Dispose();

                if (WebSocketBase.LoggingEnabled)
                {
                    NetEventSource.Exit(NetEventSource.ComponentType.WebSocket, this, Methods.ReadAsyncCore, bytesRead);
                }
            }

            return bytesRead;
        }

        // return value indicates sync vs async completion
        // false: sync completion
        // true: async completion
        private unsafe bool ReadAsyncFast(HttpListenerAsyncEventArgs eventArgs)
        {
            if (WebSocketBase.LoggingEnabled)
            {
                NetEventSource.Enter(NetEventSource.ComponentType.WebSocket, this, Methods.ReadAsyncFast, string.Empty);
            }

            eventArgs.StartOperationCommon(this);
            eventArgs.StartOperationReceive();

            uint statusCode = 0;
            bool completedAsynchronously = false;
            try
            {
                Debug.Assert(eventArgs.Buffer != null, "'BufferList' is not supported for read operations.");
                if (eventArgs.Count == 0 || m_InputStream.Closed)
                {
                    eventArgs.FinishOperationSuccess(0, true);
                    return false;
                }

                uint dataRead = 0;
                int offset = eventArgs.Offset;
                int remainingCount = eventArgs.Count;

                if (m_InputStream.BufferedDataChunksAvailable)
                {
                    dataRead = m_InputStream.GetChunks(eventArgs.Buffer, eventArgs.Offset, eventArgs.Count);
                    if (m_InputStream.BufferedDataChunksAvailable && dataRead == eventArgs.Count)
                    {
                        eventArgs.FinishOperationSuccess(eventArgs.Count, true);
                        return false;
                    }
                }

                Debug.Assert(!m_InputStream.BufferedDataChunksAvailable, "'m_InputStream.BufferedDataChunksAvailable' MUST BE 'FALSE' at this point.");
                Debug.Assert(dataRead <= eventArgs.Count, "'dataRead' MUST NOT be bigger than 'eventArgs.Count'.");

                if (dataRead != 0)
                {
                    offset += (int)dataRead;
                    remainingCount -= (int)dataRead;
                    //the http.sys team recommends that we limit the size to 128kb
                    if (remainingCount > HttpRequestStream.MaxReadSize)
                    {
                        remainingCount = HttpRequestStream.MaxReadSize;
                    }

                    eventArgs.SetBuffer(eventArgs.Buffer, offset, remainingCount);
                }
                else if (remainingCount > HttpRequestStream.MaxReadSize)
                {
                    remainingCount = HttpRequestStream.MaxReadSize;
                    eventArgs.SetBuffer(eventArgs.Buffer, offset, remainingCount);
                }

                m_InputStream.InternalHttpContext.EnsureBoundHandle();
                uint flags = 0;
                uint bytesReturned = 0;
                statusCode =
                    Interop.HttpApi.HttpReceiveRequestEntityBody(
                        m_InputStream.InternalHttpContext.RequestQueueHandle,
                        m_InputStream.InternalHttpContext.RequestId,
                        flags,
                        (byte*)m_WebSocket.InternalBuffer.ToIntPtr(eventArgs.Offset),
                        (uint)eventArgs.Count,
                        out bytesReturned,
                        eventArgs.NativeOverlapped);

                if (statusCode != Interop.HttpApi.ERROR_SUCCESS &&
                    statusCode != Interop.HttpApi.ERROR_IO_PENDING &&
                    statusCode != Interop.HttpApi.ERROR_HANDLE_EOF)
                {
                    throw new HttpListenerException((int)statusCode);
                }
                else if (statusCode == Interop.HttpApi.ERROR_SUCCESS &&
                    HttpListener.SkipIOCPCallbackOnSuccess)
                {
                    // IO operation completed synchronously. No IO completion port callback is used because 
                    // it was disabled in SwitchToOpaqueMode()
                    eventArgs.FinishOperationSuccess((int)bytesReturned, true);
                    completedAsynchronously = false;
                }
                else
                {
                    completedAsynchronously = true;
                }
            }
            catch (Exception e)
            {
                m_ReadEventArgs.FinishOperationFailure(e, true);
                m_OutputStream.SetClosedFlag();
                m_OutputStream.InternalHttpContext.Abort();

                throw;
            }
            finally
            {
                if (WebSocketBase.LoggingEnabled)
                {
                    NetEventSource.Exit(NetEventSource.ComponentType.WebSocket, this, Methods.ReadAsyncFast, completedAsynchronously);
                }
            }

            return completedAsynchronously;
        }

        public override int ReadByte()
        {
            return m_InputStream.ReadByte();
        }

        public bool SupportsMultipleWrite
        {
            get
            {
                return true;
            }
        }

        public override IAsyncResult BeginRead(byte[] buffer,
            int offset,
            int count,
            AsyncCallback callback,
            object state)
        {
            return m_InputStream.BeginRead(buffer, offset, count, callback, state);
        }

        public override int EndRead(IAsyncResult asyncResult)
        {
            return m_InputStream.EndRead(asyncResult);
        }

        public Task MultipleWriteAsync(IList<ArraySegment<byte>> sendBuffers, CancellationToken cancellationToken)
        {
            Debug.Assert(m_InOpaqueMode, "The stream MUST be in opaque mode at this point.");
            Debug.Assert(sendBuffers != null, "'sendBuffers' MUST NOT be NULL.");
            Debug.Assert(sendBuffers.Count == 1 || sendBuffers.Count == 2,
                "'sendBuffers.Count' MUST be either '1' or '2'.");

            if (sendBuffers.Count == 1)
            {
                ArraySegment<byte> buffer = sendBuffers[0];
                return WriteAsync(buffer.Array, buffer.Offset, buffer.Count, cancellationToken);
            }

            return MultipleWriteAsyncCore(sendBuffers, cancellationToken);
        }

        private async Task MultipleWriteAsyncCore(IList<ArraySegment<byte>> sendBuffers, CancellationToken cancellationToken)
        {
            Debug.Assert(sendBuffers != null, "'sendBuffers' MUST NOT be NULL.");
            Debug.Assert(sendBuffers.Count == 2, "'sendBuffers.Count' MUST be '2' at this point.");

            if (WebSocketBase.LoggingEnabled)
            {
                NetEventSource.Enter(NetEventSource.ComponentType.WebSocket, this, Methods.MultipleWriteAsyncCore, string.Empty);
            }

            CancellationTokenRegistration cancellationTokenRegistration = new CancellationTokenRegistration();

            try
            {
                if (cancellationToken.CanBeCanceled)
                {
                    cancellationTokenRegistration = cancellationToken.Register(s_OnCancel, this, false);
                }
#if DEBUG
                // When using fast path only one outstanding read is permitted. By switching into opaque mode
                // via IWebSocketStream.SwitchToOpaqueMode (see more detailed comments in interface definition)
                // caller takes responsibility for enforcing this constraint.
                Debug.Assert(Interlocked.Increment(ref m_OutstandingOperations.m_Writes) == 1,
                    "Only one outstanding write allowed at any given time.");
#endif
                m_WriteTaskCompletionSource = new TaskCompletionSource<object>();
                m_WriteEventArgs.SetBuffer(null, 0, 0);
                m_WriteEventArgs.BufferList = sendBuffers;
                if (WriteAsyncFast(m_WriteEventArgs))
                {
                    await m_WriteTaskCompletionSource.Task.SuppressContextFlow();
                }
            }
            catch (Exception error)
            {
                if (s_CanHandleException(error))
                {
                    cancellationToken.ThrowIfCancellationRequested();
                }

                throw;
            }
            finally
            {
                cancellationTokenRegistration.Dispose();

                if (WebSocketBase.LoggingEnabled)
                {
                    NetEventSource.Exit(NetEventSource.ComponentType.WebSocket, this, Methods.MultipleWriteAsyncCore, string.Empty);
                }
            }
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            m_OutputStream.Write(buffer, offset, count);
        }

        public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            WebSocketHelpers.ValidateBuffer(buffer, offset, count);

            return WriteAsyncCore(buffer, offset, count, cancellationToken);
        }

        private async Task WriteAsyncCore(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            if (WebSocketBase.LoggingEnabled)
            {
                NetEventSource.Enter(NetEventSource.ComponentType.WebSocket, this, Methods.WriteAsyncCore,
                    WebSocketHelpers.GetTraceMsgForParameters(offset, count, cancellationToken));
            }

            CancellationTokenRegistration cancellationTokenRegistration = new CancellationTokenRegistration();

            try
            {
                if (cancellationToken.CanBeCanceled)
                {
                    cancellationTokenRegistration = cancellationToken.Register(s_OnCancel, this, false);
                }

                if (!m_InOpaqueMode)
                {
                    await m_OutputStream.WriteAsync(buffer, offset, count, cancellationToken).SuppressContextFlow();
                }
                else
                {
#if DEBUG
                    // When using fast path only one outstanding read is permitted. By switching into opaque mode
                    // via IWebSocketStream.SwitchToOpaqueMode (see more detailed comments in interface definition)
                    // caller takes responsibility for enforcing this constraint.
                    Debug.Assert(Interlocked.Increment(ref m_OutstandingOperations.m_Writes) == 1,
                        "Only one outstanding write allowed at any given time.");
#endif
                    m_WriteTaskCompletionSource = new TaskCompletionSource<object>();
                    m_WriteEventArgs.BufferList = null;
                    m_WriteEventArgs.SetBuffer(buffer, offset, count);
                    if (WriteAsyncFast(m_WriteEventArgs))
                    {
                        await m_WriteTaskCompletionSource.Task.SuppressContextFlow();
                    }
                }
            }
            catch (Exception error)
            {
                if (s_CanHandleException(error))
                {
                    cancellationToken.ThrowIfCancellationRequested();
                }

                throw;
            }
            finally
            {
                cancellationTokenRegistration.Dispose();

                if (WebSocketBase.LoggingEnabled)
                {
                    NetEventSource.Exit(NetEventSource.ComponentType.WebSocket, this, Methods.WriteAsyncCore, string.Empty);
                }
            }
        }

        // return value indicates sync vs async completion
        // false: sync completion
        // true: async completion
        private unsafe bool WriteAsyncFast(HttpListenerAsyncEventArgs eventArgs)
        {
            if (WebSocketBase.LoggingEnabled)
            {
                NetEventSource.Enter(NetEventSource.ComponentType.WebSocket, this, Methods.WriteAsyncFast, string.Empty);
            }

            Interop.HttpApi.HTTP_FLAGS flags = Interop.HttpApi.HTTP_FLAGS.NONE;

            eventArgs.StartOperationCommon(this);
            eventArgs.StartOperationSend();

            uint statusCode;
            bool completedAsynchronously = false;
            try
            {
                if (m_OutputStream.Closed ||
                    (eventArgs.Buffer != null && eventArgs.Count == 0))
                {
                    eventArgs.FinishOperationSuccess(eventArgs.Count, true);
                    return false;
                }

                if (eventArgs.ShouldCloseOutput)
                {
                    flags |= Interop.HttpApi.HTTP_FLAGS.HTTP_SEND_RESPONSE_FLAG_DISCONNECT;
                }
                else
                {
                    flags |= Interop.HttpApi.HTTP_FLAGS.HTTP_SEND_RESPONSE_FLAG_MORE_DATA;
                    // When using HTTP_SEND_RESPONSE_FLAG_BUFFER_DATA HTTP.SYS will copy the payload to
                    // kernel memory (Non-Paged Pool). Http.Sys will buffer up to
                    // Math.Min(16 MB, current TCP window size)
                    flags |= Interop.HttpApi.HTTP_FLAGS.HTTP_SEND_RESPONSE_FLAG_BUFFER_DATA;
                }

                m_OutputStream.InternalHttpContext.EnsureBoundHandle();
                uint bytesSent;
                statusCode =
                    Interop.HttpApi.HttpSendResponseEntityBody(
                        m_OutputStream.InternalHttpContext.RequestQueueHandle,
                        m_OutputStream.InternalHttpContext.RequestId,
                        (uint)flags,
                        eventArgs.EntityChunkCount,
                        (Interop.HttpApi.HTTP_DATA_CHUNK*)eventArgs.EntityChunks,
                        &bytesSent,
                        SafeLocalAllocHandle.Zero,
                        0,
                        eventArgs.NativeOverlapped,
                        null);

                if (statusCode != Interop.HttpApi.ERROR_SUCCESS &&
                    statusCode != Interop.HttpApi.ERROR_IO_PENDING)
                {
                    throw new HttpListenerException((int)statusCode);
                }
                else if (statusCode == Interop.HttpApi.ERROR_SUCCESS &&
                    HttpListener.SkipIOCPCallbackOnSuccess)
                {
                    // IO operation completed synchronously - callback won't be called to signal completion.
                    eventArgs.FinishOperationSuccess((int)bytesSent, true);
                    completedAsynchronously = false;
                }
                else
                {
                    completedAsynchronously = true;
                }
            }
            catch (Exception e)
            {
                m_WriteEventArgs.FinishOperationFailure(e, true);
                m_OutputStream.SetClosedFlag();
                m_OutputStream.InternalHttpContext.Abort();

                throw;
            }
            finally
            {
                if (WebSocketBase.LoggingEnabled)
                {
                    NetEventSource.Exit(NetEventSource.ComponentType.WebSocket, this, Methods.WriteAsyncFast, completedAsynchronously);
                }
            }

            return completedAsynchronously;
        }

        public override void WriteByte(byte value)
        {
            m_OutputStream.WriteByte(value);
        }

        public override IAsyncResult BeginWrite(byte[] buffer,
            int offset,
            int count,
            AsyncCallback callback,
            object state)
        {
            return m_OutputStream.BeginWrite(buffer, offset, count, callback, state);
        }

        public override void EndWrite(IAsyncResult asyncResult)
        {
            m_OutputStream.EndWrite(asyncResult);
        }

        public override void Flush()
        {
            m_OutputStream.Flush();
        }

        public override Task FlushAsync(CancellationToken cancellationToken)
        {
            return m_OutputStream.FlushAsync(cancellationToken);
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException(SR.net_noseek);
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException(SR.net_noseek);
        }

        public async Task CloseNetworkConnectionAsync(CancellationToken cancellationToken)
        {
            // need to yield here to make sure that we don't get any exception synchronously
            await Task.Yield();

            if (WebSocketBase.LoggingEnabled)
            {
                NetEventSource.Enter(NetEventSource.ComponentType.WebSocket, this, Methods.CloseNetworkConnectionAsync, string.Empty);
            }

            CancellationTokenRegistration cancellationTokenRegistration = new CancellationTokenRegistration();

            try
            {
                if (cancellationToken.CanBeCanceled)
                {
                    cancellationTokenRegistration = cancellationToken.Register(s_OnCancel, this, false);
                }
#if DEBUG
                // When using fast path only one outstanding read is permitted. By switching into opaque mode
                // via IWebSocketStream.SwitchToOpaqueMode (see more detailed comments in interface definition)
                // caller takes responsibility for enforcing this constraint.
                Debug.Assert(Interlocked.Increment(ref m_OutstandingOperations.m_Writes) == 1,
                    "Only one outstanding write allowed at any given time.");
#endif
                m_WriteTaskCompletionSource = new TaskCompletionSource<object>();
                m_WriteEventArgs.SetShouldCloseOutput();
                if (WriteAsyncFast(m_WriteEventArgs))
                {
                    await m_WriteTaskCompletionSource.Task.SuppressContextFlow();
                }
            }
            catch (Exception error)
            {
                if (!s_CanHandleException(error))
                {
                    throw;
                }

                // throw OperationCancelledException when canceled by the caller
                // otherwise swallow the exception
                cancellationToken.ThrowIfCancellationRequested();
            }
            finally
            {
                cancellationTokenRegistration.Dispose();

                if (WebSocketBase.LoggingEnabled)
                {
                    NetEventSource.Exit(NetEventSource.ComponentType.WebSocket, this, Methods.CloseNetworkConnectionAsync, string.Empty);
                }
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && Interlocked.Exchange(ref m_CleanedUp, 1) == 0)
            {
                if (m_ReadTaskCompletionSource != null)
                {
                    m_ReadTaskCompletionSource.TrySetCanceled();
                }

                if (m_WriteTaskCompletionSource != null)
                {
                    m_WriteTaskCompletionSource.TrySetCanceled();
                }

                if (m_ReadEventArgs != null)
                {
                    m_ReadEventArgs.Dispose();
                }

                if (m_WriteEventArgs != null)
                {
                    m_WriteEventArgs.Dispose();
                }

                try
                {
                    m_InputStream.Close();
                }
                finally
                {
                    m_OutputStream.Close();
                }
            }
        }

        public void Abort()
        {
            OnCancel(this);
        }

        private static bool CanHandleException(Exception error)
        {
            return error is HttpListenerException ||
                error is ObjectDisposedException ||
                error is IOException;
        }

        private static void OnCancel(object state)
        {
            Debug.Assert(state != null, "'state' MUST NOT be NULL.");
            WebSocketHttpListenerDuplexStream thisPtr = state as WebSocketHttpListenerDuplexStream;
            Debug.Assert(thisPtr != null, "'thisPtr' MUST NOT be NULL.");

            if (WebSocketBase.LoggingEnabled)
            {
                NetEventSource.Enter(NetEventSource.ComponentType.WebSocket, state, nameof(OnCancel), string.Empty);
            }

            try
            {
                thisPtr.m_OutputStream.SetClosedFlag();
                thisPtr.m_Context.Abort();
            }
            catch { }

            TaskCompletionSource<int> readTaskCompletionSourceSnapshot = thisPtr.m_ReadTaskCompletionSource;

            if (readTaskCompletionSourceSnapshot != null)
            {
                readTaskCompletionSourceSnapshot.TrySetCanceled();
            }

            TaskCompletionSource<object> writeTaskCompletionSourceSnapshot = thisPtr.m_WriteTaskCompletionSource;

            if (writeTaskCompletionSourceSnapshot != null)
            {
                writeTaskCompletionSourceSnapshot.TrySetCanceled();
            }

            if (WebSocketBase.LoggingEnabled)
            {
                NetEventSource.Exit(NetEventSource.ComponentType.WebSocket, state, nameof(OnCancel), string.Empty);
            }
        }

        public void SwitchToOpaqueMode(WebSocketBase webSocket)
        {
            Debug.Assert(webSocket != null, "'webSocket' MUST NOT be NULL.");
            Debug.Assert(m_OutputStream != null, "'m_OutputStream' MUST NOT be NULL.");
            Debug.Assert(m_OutputStream.InternalHttpContext != null,
                "'m_OutputStream.InternalHttpContext' MUST NOT be NULL.");
            Debug.Assert(m_OutputStream.InternalHttpContext.Response != null,
                "'m_OutputStream.InternalHttpContext.Response' MUST NOT be NULL.");
            Debug.Assert(m_OutputStream.InternalHttpContext.Response.SentHeaders,
                "Headers MUST have been sent at this point.");
            Debug.Assert(!m_InOpaqueMode, "SwitchToOpaqueMode MUST NOT be called multiple times.");

            if (m_InOpaqueMode)
            {
                throw new InvalidOperationException();
            }

            m_WebSocket = webSocket;
            m_InOpaqueMode = true;
            m_ReadEventArgs = new HttpListenerAsyncEventArgs(webSocket, this);
            m_ReadEventArgs.Completed += s_OnReadCompleted;
            m_WriteEventArgs = new HttpListenerAsyncEventArgs(webSocket, this);
            m_WriteEventArgs.Completed += s_OnWriteCompleted;

            if (WebSocketBase.LoggingEnabled)
            {
                NetEventSource.Associate(NetEventSource.ComponentType.WebSocket, this, webSocket);
            }
        }

        private static void OnWriteCompleted(object sender, HttpListenerAsyncEventArgs eventArgs)
        {
            Debug.Assert(eventArgs != null, "'eventArgs' MUST NOT be NULL.");
            WebSocketHttpListenerDuplexStream thisPtr = eventArgs.CurrentStream;
            Debug.Assert(thisPtr != null, "'thisPtr' MUST NOT be NULL.");
#if DEBUG
            Debug.Assert(Interlocked.Decrement(ref thisPtr.m_OutstandingOperations.m_Writes) >= 0,
                "'thisPtr.m_OutstandingOperations.m_Writes' MUST NOT be negative.");
#endif

            if (WebSocketBase.LoggingEnabled)
            {
                NetEventSource.Enter(NetEventSource.ComponentType.WebSocket, thisPtr, Methods.OnWriteCompleted, string.Empty);
            }

            if (eventArgs.Exception != null)
            {
                thisPtr.m_WriteTaskCompletionSource.TrySetException(eventArgs.Exception);
            }
            else
            {
                thisPtr.m_WriteTaskCompletionSource.TrySetResult(null);
            }

            if (WebSocketBase.LoggingEnabled)
            {
                NetEventSource.Exit(NetEventSource.ComponentType.WebSocket, thisPtr, Methods.OnWriteCompleted, string.Empty);
            }
        }

        private static void OnReadCompleted(object sender, HttpListenerAsyncEventArgs eventArgs)
        {
            Debug.Assert(eventArgs != null, "'eventArgs' MUST NOT be NULL.");
            WebSocketHttpListenerDuplexStream thisPtr = eventArgs.CurrentStream;
            Debug.Assert(thisPtr != null, "'thisPtr' MUST NOT be NULL.");
#if DEBUG
            Debug.Assert(Interlocked.Decrement(ref thisPtr.m_OutstandingOperations.m_Reads) >= 0,
                "'thisPtr.m_OutstandingOperations.m_Reads' MUST NOT be negative.");
#endif

            if (WebSocketBase.LoggingEnabled)
            {
                NetEventSource.Enter(NetEventSource.ComponentType.WebSocket, thisPtr, Methods.OnReadCompleted, string.Empty);
            }

            if (eventArgs.Exception != null)
            {
                thisPtr.m_ReadTaskCompletionSource.TrySetException(eventArgs.Exception);
            }
            else
            {
                thisPtr.m_ReadTaskCompletionSource.TrySetResult(eventArgs.BytesTransferred);
            }

            if (WebSocketBase.LoggingEnabled)
            {
                NetEventSource.Exit(NetEventSource.ComponentType.WebSocket, thisPtr, Methods.OnReadCompleted, string.Empty);
            }
        }

        internal class HttpListenerAsyncEventArgs : EventArgs, IDisposable
        {
            private const int Free = 0;
            private const int InProgress = 1;
            private const int Disposed = 2;
            private int m_Operating;

            private bool m_DisposeCalled;
            private unsafe NativeOverlapped* m_PtrNativeOverlapped;
            private ThreadPoolBoundHandle m_boundHandle;
            private event EventHandler<HttpListenerAsyncEventArgs> m_Completed;
            private byte[] m_Buffer;
            private IList<ArraySegment<byte>> m_BufferList;
            private int m_Count;
            private int m_Offset;
            private int m_BytesTransferred;
            private HttpListenerAsyncOperation m_CompletedOperation;
            private Interop.HttpApi.HTTP_DATA_CHUNK[] m_DataChunks;
            private GCHandle m_DataChunksGCHandle;
            private ushort m_DataChunkCount;
            private Exception m_Exception;
            private bool m_ShouldCloseOutput;
            private readonly WebSocketBase m_WebSocket;
            private readonly WebSocketHttpListenerDuplexStream m_CurrentStream;

            public HttpListenerAsyncEventArgs(WebSocketBase webSocket, WebSocketHttpListenerDuplexStream stream)
                : base()
            {
                m_WebSocket = webSocket;
                m_CurrentStream = stream;
            }

            public int BytesTransferred
            {
                get { return m_BytesTransferred; }
            }

            public byte[] Buffer
            {
                get { return m_Buffer; }
            }

            // BufferList property.
            // Mutually exclusive with Buffer.
            // Setting this property with an existing non-null Buffer will cause an assert.    
            public IList<ArraySegment<byte>> BufferList
            {
                get { return m_BufferList; }
                set
                {
                    Debug.Assert(!m_ShouldCloseOutput, "'m_ShouldCloseOutput' MUST be 'false' at this point.");
                    Debug.Assert(value == null || m_Buffer == null,
                        "Either 'm_Buffer' or 'm_BufferList' MUST be NULL.");
                    Debug.Assert(m_Operating == Free,
                        "This property can only be modified if no IO operation is outstanding.");
                    Debug.Assert(value == null || value.Count == 2,
                        "This list can only be 'NULL' or MUST have exactly '2' items.");
                    m_BufferList = value;
                }
            }

            public bool ShouldCloseOutput
            {
                get { return m_ShouldCloseOutput; }
            }

            public int Offset
            {
                get { return m_Offset; }
            }

            public int Count
            {
                get { return m_Count; }
            }

            public Exception Exception
            {
                get { return m_Exception; }
            }

            public ushort EntityChunkCount
            {
                get
                {
                    if (m_DataChunks == null)
                    {
                        return 0;
                    }

                    return m_DataChunkCount;
                }
            }

            internal unsafe NativeOverlapped* NativeOverlapped
            {
                get { return m_PtrNativeOverlapped; }
            }

            public IntPtr EntityChunks
            {
                get
                {
                    if (m_DataChunks == null)
                    {
                        return IntPtr.Zero;
                    }

                    return Marshal.UnsafeAddrOfPinnedArrayElement(m_DataChunks, 0);
                }
            }

            public WebSocketHttpListenerDuplexStream CurrentStream
            {
                get { return m_CurrentStream; }
            }

            public event EventHandler<HttpListenerAsyncEventArgs> Completed
            {
                add
                {
                    m_Completed += value;
                }
                remove
                {
                    m_Completed -= value;
                }
            }

            protected virtual void OnCompleted(HttpListenerAsyncEventArgs e)
            {
                EventHandler<HttpListenerAsyncEventArgs> handler = m_Completed;
                if (handler != null)
                {
                    handler(e.m_CurrentStream, e);
                }
            }

            public void SetShouldCloseOutput()
            {
                m_BufferList = null;
                m_Buffer = null;
                m_ShouldCloseOutput = true;
            }

            public void Dispose()
            {
                // Remember that Dispose was called.
                m_DisposeCalled = true;

                // Check if this object is in-use for an async socket operation.
                if (Interlocked.CompareExchange(ref m_Operating, Disposed, Free) != Free)
                {
                    // Either already disposed or will be disposed when current operation completes.
                    return;
                }

                // Don't bother finalizing later.
                GC.SuppressFinalize(this);
            }

            private unsafe void InitializeOverlapped(ThreadPoolBoundHandle boundHandle)
            {
                m_boundHandle = boundHandle;
                m_PtrNativeOverlapped = boundHandle.AllocateNativeOverlapped(CompletionPortCallback, null, null);
            }

            // Method to clean up any existing Overlapped object and related state variables.
            private unsafe void FreeOverlapped(bool checkForShutdown)
            {
                if (!checkForShutdown || !Environment.HasShutdownStarted)
                {
                    // Free the overlapped object
                    if (m_PtrNativeOverlapped != null)
                    {
                        m_boundHandle.FreeNativeOverlapped(m_PtrNativeOverlapped);
                        m_PtrNativeOverlapped = null;
                    }

                    if (m_DataChunksGCHandle.IsAllocated)
                    {
                        m_DataChunksGCHandle.Free();
                        m_DataChunks = null;
                    }
                }
            }

            // Method called to prepare for a native async http.sys call.
            // This method performs the tasks common to all http.sys operations.
            internal void StartOperationCommon(WebSocketHttpListenerDuplexStream currentStream, ThreadPoolBoundHandle boundHandle)
            {
                // Change status to "in-use".
                if (Interlocked.CompareExchange(ref m_Operating, InProgress, Free) != Free)
                {
                    // If it was already "in-use" check if Dispose was called.
                    if (m_DisposeCalled)
                    {
                        // Dispose was called - throw ObjectDisposed.
                        throw new ObjectDisposedException(GetType().FullName);
                    }

                    Debug.Assert(false, "Only one outstanding async operation is allowed per HttpListenerAsyncEventArgs instance.");
                    // Only one at a time.
                    throw new InvalidOperationException();
                }

                // HttpSendResponseEntityBody can return ERROR_INVALID_PARAMETER if the InternalHigh field of the overlapped
                // is not IntPtr.Zero, so we have to reset this field because we are reusing the Overlapped.
                // When using the IAsyncResult based approach of HttpListenerResponseStream the Overlapped is reinitialized
                // for each operation by the CLR when returned from the OverlappedDataCache.

                InitializeOverlapped(boundHandle);

                m_Exception = null;
                m_BytesTransferred = 0;
            }

            internal void StartOperationReceive()
            {
                // Remember the operation type.
                m_CompletedOperation = HttpListenerAsyncOperation.Receive;
            }

            internal void StartOperationSend()
            {
                UpdateDataChunk();

                // Remember the operation type.
                m_CompletedOperation = HttpListenerAsyncOperation.Send;
            }

            public void SetBuffer(byte[] buffer, int offset, int count)
            {
                Debug.Assert(!m_ShouldCloseOutput, "'m_ShouldCloseOutput' MUST be 'false' at this point.");
                Debug.Assert(buffer == null || m_BufferList == null, "Either 'm_Buffer' or 'm_BufferList' MUST be NULL.");
                m_Buffer = buffer;
                m_Offset = offset;
                m_Count = count;
            }

            private unsafe void UpdateDataChunk()
            {
                if (m_DataChunks == null)
                {
                    m_DataChunks = new Interop.HttpApi.HTTP_DATA_CHUNK[2];
                    m_DataChunksGCHandle = GCHandle.Alloc(m_DataChunks, GCHandleType.Pinned);
                    m_DataChunks[0] = new Interop.HttpApi.HTTP_DATA_CHUNK();
                    m_DataChunks[0].DataChunkType = Interop.HttpApi.HTTP_DATA_CHUNK_TYPE.HttpDataChunkFromMemory;
                    m_DataChunks[1] = new Interop.HttpApi.HTTP_DATA_CHUNK();
                    m_DataChunks[1].DataChunkType = Interop.HttpApi.HTTP_DATA_CHUNK_TYPE.HttpDataChunkFromMemory;
                }

                Debug.Assert(m_Buffer == null || m_BufferList == null, "Either 'm_Buffer' or 'm_BufferList' MUST be NULL.");
                Debug.Assert(m_ShouldCloseOutput || m_Buffer != null || m_BufferList != null, "Either 'm_Buffer' or 'm_BufferList' MUST NOT be NULL.");

                // The underlying byte[] m_Buffer or each m_BufferList[].Array are pinned already 
                if (m_Buffer != null)
                {
                    UpdateDataChunk(0, m_Buffer, m_Offset, m_Count);
                    UpdateDataChunk(1, null, 0, 0);
                    m_DataChunkCount = 1;
                }
                else if (m_BufferList != null)
                {
                    Debug.Assert(m_BufferList != null && m_BufferList.Count == 2,
                        "'m_BufferList' MUST NOT be NULL and have exactly '2' items at this point.");
                    UpdateDataChunk(0, m_BufferList[0].Array, m_BufferList[0].Offset, m_BufferList[0].Count);
                    UpdateDataChunk(1, m_BufferList[1].Array, m_BufferList[1].Offset, m_BufferList[1].Count);
                    m_DataChunkCount = 2;
                }
                else
                {
                    Debug.Assert(m_ShouldCloseOutput, "'m_ShouldCloseOutput' MUST be 'true' at this point.");
                    m_DataChunks = null;
                }
            }

            private unsafe void UpdateDataChunk(int index, byte[] buffer, int offset, int count)
            {
                if (buffer == null)
                {
                    m_DataChunks[index].pBuffer = null;
                    m_DataChunks[index].BufferLength = 0;
                    return;
                }

                if (m_WebSocket.InternalBuffer.IsInternalBuffer(buffer, offset, count))
                {
                    m_DataChunks[index].pBuffer = (byte*)(m_WebSocket.InternalBuffer.ToIntPtr(offset));
                }
                else
                {
                    m_DataChunks[index].pBuffer =
                        (byte*)m_WebSocket.InternalBuffer.ConvertPinnedSendPayloadToNative(buffer, offset, count);
                }

                m_DataChunks[index].BufferLength = (uint)count;
            }

            // Method to mark this object as no longer "in-use".
            // Will also execute a Dispose deferred because I/O was in progress.  
            internal void Complete()
            {
                FreeOverlapped(false);
                // Mark as not in-use
                Interlocked.Exchange(ref m_Operating, Free);

                // Check for deferred Dispose().
                // The deferred Dispose is not guaranteed if Dispose is called while an operation is in progress. 
                // The m_DisposeCalled variable is not managed in a thread-safe manner on purpose for performance.
                if (m_DisposeCalled)
                {
                    Dispose();
                }
            }

            // Method to update internal state after sync or async completion.
            private void SetResults(Exception exception, int bytesTransferred)
            {
                m_Exception = exception;
                m_BytesTransferred = bytesTransferred;
            }

            internal void FinishOperationFailure(Exception exception, bool syncCompletion)
            {
                SetResults(exception, 0);

                if (WebSocketBase.LoggingEnabled)
                {
                    NetEventSource.PrintError(NetEventSource.ComponentType.WebSocket, m_CurrentStream,
                        m_CompletedOperation == HttpListenerAsyncOperation.Receive ? Methods.ReadAsyncFast : Methods.WriteAsyncFast,
                        exception.ToString());
                }

                Complete();
                OnCompleted(this);
            }

            internal void FinishOperationSuccess(int bytesTransferred, bool syncCompletion)
            {
                SetResults(null, bytesTransferred);

                if (WebSocketBase.LoggingEnabled)
                {
                    if (m_Buffer != null)
                    {
                        NetEventSource.Dump(NetEventSource.ComponentType.WebSocket, m_CurrentStream,
                            m_CompletedOperation == HttpListenerAsyncOperation.Receive ? Methods.ReadAsyncFast : Methods.WriteAsyncFast,
                            m_Buffer, m_Offset, bytesTransferred);
                    }
                    else if (m_BufferList != null)
                    {
                        Debug.Assert(m_CompletedOperation == HttpListenerAsyncOperation.Send,
                            "'BufferList' is only supported for send operations.");

                        foreach (ArraySegment<byte> buffer in BufferList)
                        {
                            NetEventSource.Dump(NetEventSource.ComponentType.WebSocket, this, Methods.WriteAsyncFast, buffer.Array, buffer.Offset, buffer.Count);
                        }
                    }
                }

                if (m_ShouldCloseOutput)
                {
                    m_CurrentStream.m_OutputStream.SetClosedFlag();
                }

                // Complete the operation and raise completion event.
                Complete();
                OnCompleted(this);
            }

            private unsafe void CompletionPortCallback(uint errorCode, uint numBytes, NativeOverlapped* nativeOverlapped)
            {
                if (errorCode == Interop.HttpApi.ERROR_SUCCESS ||
                    errorCode == Interop.HttpApi.ERROR_HANDLE_EOF)
                {
                    FinishOperationSuccess((int)numBytes, false);
                }
                else
                {
                    FinishOperationFailure(new HttpListenerException((int)errorCode), false);
                }
            }

            public enum HttpListenerAsyncOperation
            {
                None,
                Receive,
                Send
            }
        }

        private static class Methods
        {
            public const string CloseNetworkConnectionAsync = "CloseNetworkConnectionAsync";
            public const string OnCancel = "OnCancel";
            public const string OnReadCompleted = "OnReadCompleted";
            public const string OnWriteCompleted = "OnWriteCompleted";
            public const string ReadAsyncFast = "ReadAsyncFast";
            public const string ReadAsyncCore = "ReadAsyncCore";
            public const string WriteAsyncFast = "WriteAsyncFast";
            public const string WriteAsyncCore = "WriteAsyncCore";
            public const string MultipleWriteAsyncCore = "MultipleWriteAsyncCore";
        }
    }
}
