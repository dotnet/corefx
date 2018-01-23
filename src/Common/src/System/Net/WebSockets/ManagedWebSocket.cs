// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Buffers;
using System.Diagnostics;
using System.IO;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace System.Net.WebSockets
{
    /// <summary>A managed implementation of a web socket that sends and receives data via a <see cref="Stream"/>.</summary>
    /// <remarks>
    /// Thread-safety:
    /// - It's acceptable to call ReceiveAsync and SendAsync in parallel.  One of each may run concurrently.
    /// - It's acceptable to have a pending ReceiveAsync while CloseOutputAsync or CloseAsync is called.
    /// - Attempting to invoke any other operations in parallel may corrupt the instance.  Attempting to invoke
    ///   a send operation while another is in progress or a receive operation while another is in progress will
    ///   result in an exception.
    /// </remarks>
    internal sealed partial class ManagedWebSocket : WebSocket
    {
        /// <summary>Creates a <see cref="ManagedWebSocket"/> from a <see cref="Stream"/> connected to a websocket endpoint.</summary>
        /// <param name="stream">The connected Stream.</param>
        /// <param name="isServer">true if this is the server-side of the connection; false if this is the client-side of the connection.</param>
        /// <param name="subprotocol">The agreed upon subprotocol for the connection.</param>
        /// <param name="keepAliveInterval">The interval to use for keep-alive pings.</param>
        /// <param name="receiveBufferSize">The buffer size to use for received data.</param>
        /// <param name="receiveBuffer">Optional buffer to use for receives.</param>
        /// <returns>The created <see cref="ManagedWebSocket"/> instance.</returns>
        public static ManagedWebSocket CreateFromConnectedStream(
            Stream stream, bool isServer, string subprotocol, TimeSpan keepAliveInterval, Memory<byte> buffer)
        {
            return new ManagedWebSocket(stream, isServer, subprotocol, keepAliveInterval, buffer);
        }

        /// <summary>Thread-safe random number generator used to generate masks for each send.</summary>
        private static readonly RandomNumberGenerator s_random = RandomNumberGenerator.Create();
        /// <summary>Encoding for the payload of text messages: UTF8 encoding that throws if invalid bytes are discovered, per the RFC.</summary>
        private static readonly UTF8Encoding s_textEncoding = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false, throwOnInvalidBytes: true);

        /// <summary>Valid states to be in when calling SendAsync.</summary>
        private static readonly WebSocketState[] s_validSendStates = { WebSocketState.Open, WebSocketState.CloseReceived };
        /// <summary>Valid states to be in when calling ReceiveAsync.</summary>
        private static readonly WebSocketState[] s_validReceiveStates = { WebSocketState.Open, WebSocketState.CloseSent };
        /// <summary>Valid states to be in when calling CloseOutputAsync.</summary>
        private static readonly WebSocketState[] s_validCloseOutputStates = { WebSocketState.Open, WebSocketState.CloseReceived };
        /// <summary>Valid states to be in when calling CloseAsync.</summary>
        private static readonly WebSocketState[] s_validCloseStates = { WebSocketState.Open, WebSocketState.CloseReceived, WebSocketState.CloseSent };

        /// <summary>Successfully completed task representing a close message.</summary>
        private static readonly Task<WebSocketReceiveResult> s_cachedCloseTask = Task.FromResult(new WebSocketReceiveResult(0, WebSocketMessageType.Close, true));

        /// <summary>The maximum size in bytes of a message frame header that includes mask bytes.</summary>
        internal const int MaxMessageHeaderLength = 14;
        /// <summary>The maximum size of a control message payload.</summary>
        private const int MaxControlPayloadLength = 125;
        /// <summary>Length of the mask XOR'd with the payload data.</summary>
        private const int MaskLength = 4;
        /// <summary>Default length of a receive buffer to create when an invalid scratch buffer is provided.</summary>
        private const int DefaultReceiveBufferSize = 0x1000;

        /// <summary>The stream used to communicate with the remote server.</summary>
        private readonly Stream _stream;
        /// <summary>
        /// true if this is the server-side of the connection; false if it's client.
        /// This impacts masking behavior: clients always mask payloads they send and
        /// expect to always receive unmasked payloads, whereas servers always send
        /// unmasked payloads and expect to always receive masked payloads.
        /// </summary>
        private readonly bool _isServer = false;
        /// <summary>The agreed upon subprotocol with the server.</summary>
        private readonly string _subprotocol;
        /// <summary>Timer used to send periodic pings to the server, at the interval specified</summary>
        private readonly Timer _keepAliveTimer;
        /// <summary>CancellationTokenSource used to abort all current and future operations when anything is canceled or any error occurs.</summary>
        private readonly CancellationTokenSource _abortSource = new CancellationTokenSource();
        /// <summary>Buffer used for reading data from the network.</summary>
        private Memory<byte> _receiveBuffer;
        /// <summary>
        /// Tracks the state of the validity of the UTF8 encoding of text payloads.  Text may be split across fragments.
        /// </summary>
        private readonly Utf8MessageState _utf8TextState = new Utf8MessageState();
        /// <summary>
        /// Semaphore used to ensure that calls to SendFrameAsync don't run concurrently.  While <see cref="_lastSendAsync"/>
        /// is used to fail if a caller tries to issue another SendAsync while a previous one is running, internally
        /// we use SendFrameAsync as an implementation detail, and it should not cause user requests to SendAsync to fail,
        /// nor should such internal usage be allowed to run concurrently with other internal usage or with SendAsync.
        /// </summary>
        private readonly SemaphoreSlim _sendFrameAsyncLock = new SemaphoreSlim(1, 1);

        // We maintain the current WebSocketState in _state.  However, we separately maintain _sentCloseFrame and _receivedCloseFrame
        // as there isn't a strict ordering between CloseSent and CloseReceived.  If we receive a close frame from the server, we need to
        // transition to CloseReceived even if we're currently in CloseSent, and if we send a close frame, we need to transition to
        // CloseSent even if we're currently in CloseReceived.

        /// <summary>The current state of the web socket in the protocol.</summary>
        private WebSocketState _state = WebSocketState.Open;
        /// <summary>true if Dispose has been called; otherwise, false.</summary>
        private bool _disposed;
        /// <summary>Whether we've ever sent a close frame.</summary>
        private bool _sentCloseFrame;
        /// <summary>Whether we've ever received a close frame.</summary>
        private bool _receivedCloseFrame;
        /// <summary>The reason for the close, as sent by the server, or null if not yet closed.</summary>
        private WebSocketCloseStatus? _closeStatus = null;
        /// <summary>A description of the close reason as sent by the server, or null if not yet closed.</summary>
        private string _closeStatusDescription = null;

        /// <summary>
        /// The last header received in a ReceiveAsync.  If ReceiveAsync got a header but then
        /// returned fewer bytes than was indicated in the header, subsequent ReceiveAsync calls
        /// will use the data from the header to construct the subsequent receive results, and
        /// the payload length in this header will be decremented to indicate the number of bytes
        /// remaining to be received for that header.  As a result, between fragments, the payload
        /// length in this header should be 0.
        /// </summary>
        private MessageHeader _lastReceiveHeader = new MessageHeader { Opcode = MessageOpcode.Text, Fin = true };
        /// <summary>The offset of the next available byte in the _receiveBuffer.</summary>
        private int _receiveBufferOffset = 0;
        /// <summary>The number of bytes available in the _receiveBuffer.</summary>
        private int _receiveBufferCount = 0;
        /// <summary>
        /// When dealing with partially read fragments of binary/text messages, a mask previously received may still
        /// apply, and the first new byte received may not correspond to the 0th position in the mask.  This value is
        /// the next offset into the mask that should be applied.
        /// </summary>
        private int _receivedMaskOffsetOffset = 0;
        /// <summary>
        /// Temporary send buffer.  This should be released back to the ArrayPool once it's
        /// no longer needed for the current send operation.  It is stored as an instance
        /// field to minimize needing to pass it around and to avoid it becoming a field on
        /// various async state machine objects.
        /// </summary>
        private byte[] _sendBuffer;
        /// <summary>
        /// Whether the last SendAsync had endOfMessage==false. We need to track this so that we
        /// can send the subsequent message with a continuation opcode if the last message was a fragment.
        /// </summary>
        private bool _lastSendWasFragment;
        /// <summary>
        /// The task returned from the last SendAsync operation to not complete synchronously.
        /// If this is not null and not completed when a subsequent SendAsync is issued, an exception occurs.
        /// </summary>
        private Task _lastSendAsync;
        /// <summary>
        /// The task returned from the last ReceiveAsync operation to not complete synchronously.
        /// If this is not null and not completed when a subsequent ReceiveAsync is issued, an exception occurs.
        /// </summary>
        private Task _lastReceiveAsync;

        /// <summary>Lock used to protect update and check-and-update operations on _state.</summary>
        private object StateUpdateLock => _abortSource;
        /// <summary>
        /// We need to coordinate between receives and close operations happening concurrently, as a ReceiveAsync may
        /// be pending while a Close{Output}Async is issued, which itself needs to loop until a close frame is received.
        /// As such, we need thread-safety in the management of <see cref="_lastReceiveAsync"/>. 
        /// </summary>
        private object ReceiveAsyncLock => _utf8TextState; // some object, as we're simply lock'ing on it

        /// <summary>Initializes the websocket.</summary>
        /// <param name="stream">The connected Stream.</param>
        /// <param name="isServer">true if this is the server-side of the connection; false if this is the client-side of the connection.</param>
        /// <param name="subprotocol">The agreed upon subprotocol for the connection.</param>
        /// <param name="keepAliveInterval">The interval to use for keep-alive pings.</param>
        /// <param name="receiveBufferSize">The buffer size to use for received data.</param>
        /// <param name="buffer">Optional buffer to use for receives</param>
        private ManagedWebSocket(Stream stream, bool isServer, string subprotocol, TimeSpan keepAliveInterval, Memory<byte> buffer)
        {
            Debug.Assert(StateUpdateLock != null, $"Expected {nameof(StateUpdateLock)} to be non-null");
            Debug.Assert(ReceiveAsyncLock != null, $"Expected {nameof(ReceiveAsyncLock)} to be non-null");
            Debug.Assert(StateUpdateLock != ReceiveAsyncLock, "Locks should be different objects");

            Debug.Assert(stream != null, $"Expected non-null stream");
            Debug.Assert(stream.CanRead, $"Expected readable stream");
            Debug.Assert(stream.CanWrite, $"Expected writeable stream");
            Debug.Assert(keepAliveInterval == Timeout.InfiniteTimeSpan || keepAliveInterval >= TimeSpan.Zero, $"Invalid keepalive interval: {keepAliveInterval}");

            _stream = stream;
            _isServer = isServer;
            _subprotocol = subprotocol;

            // If we were provided with a buffer to use, use it as long as it's big enough for our needs.
            // If it doesn't meet our criteria, just create a new one. If we need to create a new one,
            // we avoid using the pool because often many web sockets will be used concurrently, and if each web
            // socket rents a similarly sized buffer from the pool for its duration, we'll end up draining
            // the pool, such that other web sockets will allocate anyway, as will anyone else in the process using the
            // pool.  If someone wants to pool, they can do so by passing in the buffer they want to use, and they can
            // get it from whatever pool they like.
            _receiveBuffer = buffer.Length >= MaxMessageHeaderLength ? buffer : new byte[DefaultReceiveBufferSize];

            // Set up the abort source so that if it's triggered, we transition the instance appropriately.
            _abortSource.Token.Register(s =>
            {
                var thisRef = (ManagedWebSocket)s;

                lock (thisRef.StateUpdateLock)
                {
                    WebSocketState state = thisRef._state;
                    if (state != WebSocketState.Closed && state != WebSocketState.Aborted)
                    {
                        thisRef._state = state != WebSocketState.None && state != WebSocketState.Connecting ?
                            WebSocketState.Aborted :
                            WebSocketState.Closed;
                    }
                }
            }, this);

            // Now that we're opened, initiate the keep alive timer to send periodic pings
            if (keepAliveInterval > TimeSpan.Zero)
            {
                _keepAliveTimer = new Timer(s => ((ManagedWebSocket)s).SendKeepAliveFrameAsync(), this, keepAliveInterval, keepAliveInterval);
            }
        }

        public override void Dispose()
        {
            lock (StateUpdateLock)
            {
                DisposeCore();
            }
        }

        private void DisposeCore()
        {
            Debug.Assert(Monitor.IsEntered(StateUpdateLock), $"Expected {nameof(StateUpdateLock)} to be held");
            if (!_disposed)
            {
                _disposed = true;
                _keepAliveTimer?.Dispose();
                _stream?.Dispose();
                if (_state < WebSocketState.Aborted)
                {
                    _state = WebSocketState.Closed;
                }
            }
        }

        public override WebSocketCloseStatus? CloseStatus => _closeStatus;

        public override string CloseStatusDescription => _closeStatusDescription;

        public override WebSocketState State => _state;

        public override string SubProtocol => _subprotocol;

        public override Task SendAsync(ArraySegment<byte> buffer, WebSocketMessageType messageType, bool endOfMessage, CancellationToken cancellationToken)
        {
            if (messageType != WebSocketMessageType.Text && messageType != WebSocketMessageType.Binary)
            {
                throw new ArgumentException(SR.Format(
                    SR.net_WebSockets_Argument_InvalidMessageType,
                    nameof(WebSocketMessageType.Close), nameof(SendAsync), nameof(WebSocketMessageType.Binary), nameof(WebSocketMessageType.Text), nameof(CloseOutputAsync)),
                    nameof(messageType));
            }

            WebSocketValidate.ValidateArraySegment(buffer, nameof(buffer));

            return SendPrivateAsync((ReadOnlyMemory<byte>)buffer, messageType, endOfMessage, cancellationToken);
        }

        private Task SendPrivateAsync(ReadOnlyMemory<byte> buffer, WebSocketMessageType messageType, bool endOfMessage, CancellationToken cancellationToken)
        {
            if (messageType != WebSocketMessageType.Text && messageType != WebSocketMessageType.Binary)
            {
                throw new ArgumentException(SR.Format(
                    SR.net_WebSockets_Argument_InvalidMessageType,
                    nameof(WebSocketMessageType.Close), nameof(SendAsync), nameof(WebSocketMessageType.Binary), nameof(WebSocketMessageType.Text), nameof(CloseOutputAsync)),
                    nameof(messageType));
            }

            try
            {
                WebSocketValidate.ThrowIfInvalidState(_state, _disposed, s_validSendStates);
                ThrowIfOperationInProgress(_lastSendAsync);
            }
            catch (Exception exc)
            {
                return Task.FromException(exc);
            }

            MessageOpcode opcode =
                _lastSendWasFragment ? MessageOpcode.Continuation :
                messageType == WebSocketMessageType.Binary ? MessageOpcode.Binary :
                MessageOpcode.Text;

            Task t = SendFrameAsync(opcode, endOfMessage, buffer, cancellationToken);
            _lastSendWasFragment = !endOfMessage;
            _lastSendAsync = t;
            return t;
        }

        public override Task<WebSocketReceiveResult> ReceiveAsync(ArraySegment<byte> buffer, CancellationToken cancellationToken)
        {
            WebSocketValidate.ValidateArraySegment(buffer, nameof(buffer));

            try
            {
                WebSocketValidate.ThrowIfInvalidState(_state, _disposed, s_validReceiveStates);

                Debug.Assert(!Monitor.IsEntered(StateUpdateLock), $"{nameof(StateUpdateLock)} must never be held when acquiring {nameof(ReceiveAsyncLock)}");
                lock (ReceiveAsyncLock) // synchronize with receives in CloseAsync
                {
                    ThrowIfOperationInProgress(_lastReceiveAsync);
                    Task<WebSocketReceiveResult> t = ReceiveAsyncPrivate<WebSocketReceiveResultGetter,WebSocketReceiveResult>(buffer, cancellationToken).AsTask();
                    _lastReceiveAsync = t;
                    return t;
                }
            }
            catch (Exception exc)
            {
                return Task.FromException<WebSocketReceiveResult>(exc);
            }
        }

        public override Task CloseAsync(WebSocketCloseStatus closeStatus, string statusDescription, CancellationToken cancellationToken)
        {
            WebSocketValidate.ValidateCloseStatus(closeStatus, statusDescription);

            try
            {
                WebSocketValidate.ThrowIfInvalidState(_state, _disposed, s_validCloseStates);
            }
            catch (Exception exc)
            {
                return Task.FromException(exc);
            }

            return CloseAsyncPrivate(closeStatus, statusDescription, cancellationToken);
        }

        public override Task CloseOutputAsync(WebSocketCloseStatus closeStatus, string statusDescription, CancellationToken cancellationToken)
        {
            WebSocketValidate.ValidateCloseStatus(closeStatus, statusDescription);

            try
            {
                WebSocketValidate.ThrowIfInvalidState(_state, _disposed, s_validCloseOutputStates);
            }
            catch (Exception exc)
            {
                return Task.FromException(exc);
            }

            return SendCloseFrameAsync(closeStatus, statusDescription, cancellationToken);
        }

        public override void Abort()
        {
            _abortSource.Cancel();
            Dispose(); // forcibly tear down connection
        }

        /// <summary>Sends a websocket frame to the network.</summary>
        /// <param name="opcode">The opcode for the message.</param>
        /// <param name="endOfMessage">The value of the FIN bit for the message.</param>
        /// <param name="payloadBuffer">The buffer containing the payload data fro the message.</param>
        /// <param name="cancellationToken">The CancellationToken to use to cancel the websocket.</param>
        private Task SendFrameAsync(MessageOpcode opcode, bool endOfMessage, ReadOnlyMemory<byte> payloadBuffer, CancellationToken cancellationToken)
        {
            // TODO: #4900 SendFrameAsync should in theory typically complete synchronously, making it fast and allocation free.
            // However, due to #4900, it almost always yields, resulting in all of the allocations involved in an async method
            // yielding, e.g. the boxed state machine, the Action delegate, the MoveNextRunner, and the resulting Task, plus it's
            // common that the awaited operation completes so fast after the await that we may end up allocating an AwaitTaskContinuation
            // inside of the TaskAwaiter.  Since SendFrameAsync is such a core code path, until that can be fixed, we put some
            // optimizations in place to avoid a few of those expenses, at the expense of more complicated code; for the common case,
            // this code has fewer than half the number and size of allocations.  If/when that issue is fixed, this method should be deleted
            // and replaced by SendFrameFallbackAsync, which is the same logic but in a much more easily understand flow.

            // If a cancelable cancellation token was provided, that would require registering with it, which means more state we have to
            // pass around (the CancellationTokenRegistration), so if it is cancelable, just immediately go to the fallback path.
            // Similarly, it should be rare that there are multiple outstanding calls to SendFrameAsync, but if there are, again
            // fall back to the fallback path.
            return cancellationToken.CanBeCanceled || !_sendFrameAsyncLock.Wait(0) ?
                SendFrameFallbackAsync(opcode, endOfMessage, payloadBuffer, cancellationToken) :
                SendFrameLockAcquiredNonCancelableAsync(opcode, endOfMessage, payloadBuffer);
        }

        /// <summary>Sends a websocket frame to the network. The caller must hold the sending lock.</summary>
        /// <param name="opcode">The opcode for the message.</param>
        /// <param name="endOfMessage">The value of the FIN bit for the message.</param>
        /// <param name="payloadBuffer">The buffer containing the payload data fro the message.</param>
        private Task SendFrameLockAcquiredNonCancelableAsync(MessageOpcode opcode, bool endOfMessage, ReadOnlyMemory<byte> payloadBuffer)
        {
            Debug.Assert(_sendFrameAsyncLock.CurrentCount == 0, "Caller should hold the _sendFrameAsyncLock");

            // If we get here, the cancellation token is not cancelable so we don't have to worry about it,
            // and we own the semaphore, so we don't need to asynchronously wait for it.
            Task writeTask = null;
            bool releaseSemaphoreAndSendBuffer = true;
            try
            {
                // Write the payload synchronously to the buffer, then write that buffer out to the network.
                int sendBytes = WriteFrameToSendBuffer(opcode, endOfMessage, payloadBuffer.Span);
                writeTask = _stream.WriteAsync(_sendBuffer, 0, sendBytes, CancellationToken.None);

                // If the operation happens to complete synchronously (or, more specifically, by
                // the time we get from the previous line to here), release the semaphore, return
                // the task, and we're done.
                if (writeTask.IsCompleted)
                {
                    return writeTask;
                }

                // Up until this point, if an exception occurred (such as when accessing _stream or when
                // calling GetResult), we want to release the semaphore and the send buffer. After this point,
                // both need to be held until writeTask completes.
                releaseSemaphoreAndSendBuffer = false;
            }
            catch (Exception exc)
            {
                return Task.FromException(_state == WebSocketState.Aborted ?
                    CreateOperationCanceledException(exc) :
                    new WebSocketException(WebSocketError.ConnectionClosedPrematurely, exc));
            }
            finally
            {
                if (releaseSemaphoreAndSendBuffer)
                {
                    _sendFrameAsyncLock.Release();
                    ReleaseSendBuffer();
                }
            }

            // The write was not yet completed.  Create and return a continuation that will
            // release the semaphore and translate any exception that occurred.
            return writeTask.ContinueWith((t, s) =>
            {
                var thisRef = (ManagedWebSocket)s;
                thisRef._sendFrameAsyncLock.Release();
                thisRef.ReleaseSendBuffer();

                try { t.GetAwaiter().GetResult(); }
                catch (Exception exc)
                {
                    throw thisRef._state == WebSocketState.Aborted ?
                        CreateOperationCanceledException(exc) :
                        new WebSocketException(WebSocketError.ConnectionClosedPrematurely, exc);
                }
            }, this, CancellationToken.None, TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default);
        }

        private async Task SendFrameFallbackAsync(MessageOpcode opcode, bool endOfMessage, ReadOnlyMemory<byte> payloadBuffer, CancellationToken cancellationToken)
        {
            await _sendFrameAsyncLock.WaitAsync().ConfigureAwait(false);
            try
            {
                int sendBytes = WriteFrameToSendBuffer(opcode, endOfMessage, payloadBuffer.Span);
                using (cancellationToken.Register(s => ((ManagedWebSocket)s).Abort(), this))
                {
                    await _stream.WriteAsync(_sendBuffer, 0, sendBytes, cancellationToken).ConfigureAwait(false);
                }
            }
            catch (Exception exc)
            {
                throw _state == WebSocketState.Aborted ?
                    CreateOperationCanceledException(exc, cancellationToken) :
                    new WebSocketException(WebSocketError.ConnectionClosedPrematurely, exc);
            }
            finally
            {
                _sendFrameAsyncLock.Release();
                ReleaseSendBuffer();
            }
        }

        /// <summary>Writes a frame into the send buffer, which can then be sent over the network.</summary>
        private int WriteFrameToSendBuffer(MessageOpcode opcode, bool endOfMessage, ReadOnlySpan<byte> payloadBuffer)
        {
            // Ensure we have a _sendBuffer.
            AllocateSendBuffer(payloadBuffer.Length + MaxMessageHeaderLength);

            // Write the message header data to the buffer.
            int headerLength;
            int? maskOffset = null;
            if (_isServer)
            {
                // The server doesn't send a mask, so the mask offset returned by WriteHeader
                // is actually the end of the header.
                headerLength = WriteHeader(opcode, _sendBuffer, payloadBuffer, endOfMessage, useMask: false);
            }
            else
            {
                // We need to know where the mask starts so that we can use the mask to manipulate the payload data,
                // and we need to know the total length for sending it on the wire.
                maskOffset = WriteHeader(opcode, _sendBuffer, payloadBuffer, endOfMessage, useMask: true);
                headerLength = maskOffset.GetValueOrDefault() + MaskLength;
            }

            // Write the payload
            if (payloadBuffer.Length > 0)
            {
                payloadBuffer.CopyTo(new Span<byte>(_sendBuffer, headerLength, payloadBuffer.Length));

                // If we added a mask to the header, XOR the payload with the mask.  We do the manipulation in the send buffer so as to avoid
                // changing the data in the caller-supplied payload buffer.
                if (maskOffset.HasValue)
                {
                    ApplyMask(new Span<byte>(_sendBuffer, headerLength, payloadBuffer.Length), _sendBuffer, maskOffset.Value, 0);
                }
            }

            // Return the number of bytes in the send buffer
            return headerLength + payloadBuffer.Length;
        }

        private void SendKeepAliveFrameAsync()
        {
            bool acquiredLock = _sendFrameAsyncLock.Wait(0);
            if (acquiredLock)
            {
                // This exists purely to keep the connection alive; don't wait for the result, and ignore any failures.
                // The call will handle releasing the lock.
                Task t = SendFrameLockAcquiredNonCancelableAsync(MessageOpcode.Ping, true, Memory<byte>.Empty);

                // "Observe" any exception, ignoring it to prevent the unobserved exception event from being raised.
                if (t.Status != TaskStatus.RanToCompletion)
                {
                    t.ContinueWith(p => { Exception ignored = p.Exception; },
                        CancellationToken.None,
                        TaskContinuationOptions.OnlyOnFaulted | TaskContinuationOptions.ExecuteSynchronously,
                        TaskScheduler.Default);
                }
            }
            else
            {
                // If the lock is already held, something is already getting sent,
                // so there's no need to send a keep-alive ping.
            }
        }

        private static int WriteHeader(MessageOpcode opcode, byte[] sendBuffer, ReadOnlySpan<byte> payload, bool endOfMessage, bool useMask)
        {
            // Client header format:
            // 1 bit - FIN - 1 if this is the final fragment in the message (it could be the only fragment), otherwise 0
            // 1 bit - RSV1 - Reserved - 0
            // 1 bit - RSV2 - Reserved - 0
            // 1 bit - RSV3 - Reserved - 0
            // 4 bits - Opcode - How to interpret the payload
            //     - 0x0 - continuation
            //     - 0x1 - text
            //     - 0x2 - binary
            //     - 0x8 - connection close
            //     - 0x9 - ping
            //     - 0xA - pong
            //     - (0x3 to 0x7, 0xB-0xF - reserved)
            // 1 bit - Masked - 1 if the payload is masked, 0 if it's not.  Must be 1 for the client
            // 7 bits, 7+16 bits, or 7+64 bits - Payload length
            //     - For length 0 through 125, 7 bits storing the length
            //     - For lengths 126 through 2^16, 7 bits storing the value 126, followed by 16 bits storing the length
            //     - For lengths 2^16+1 through 2^64, 7 bits storing the value 127, followed by 64 bytes storing the length
            // 0 or 4 bytes - Mask, if Masked is 1 - random value XOR'd with each 4 bytes of the payload, round-robin
            // Length bytes - Payload data

            Debug.Assert(sendBuffer.Length >= MaxMessageHeaderLength, $"Expected sendBuffer to be at least {MaxMessageHeaderLength}, got {sendBuffer.Length}");

            sendBuffer[0] = (byte)opcode; // 4 bits for the opcode
            if (endOfMessage)
            {
                sendBuffer[0] |= 0x80; // 1 bit for FIN
            }

            // Store the payload length.
            int maskOffset;
            if (payload.Length <= 125)
            {
                sendBuffer[1] = (byte)payload.Length;
                maskOffset = 2; // no additional payload length
            }
            else if (payload.Length <= ushort.MaxValue)
            {
                sendBuffer[1] = 126;
                sendBuffer[2] = (byte)(payload.Length / 256);
                sendBuffer[3] = unchecked((byte)payload.Length);
                maskOffset = 2 + sizeof(ushort); // additional 2 bytes for 16-bit length
            }
            else
            {
                sendBuffer[1] = 127;
                int length = payload.Length;
                for (int i = 9; i >= 2; i--)
                {
                    sendBuffer[i] = unchecked((byte)length);
                    length = length / 256;
                }
                maskOffset = 2 + sizeof(ulong); // additional 8 bytes for 64-bit length
            }

            if (useMask)
            {
                // Generate the mask.
                sendBuffer[1] |= 0x80;
                WriteRandomMask(sendBuffer, maskOffset);
            }

            // Return the position of the mask.
            return maskOffset;
        }

        /// <summary>Writes a 4-byte random mask to the specified buffer at the specified offset.</summary>
        /// <param name="buffer">The buffer to which to write the mask.</param>
        /// <param name="offset">The offset into the buffer at which to write the mask.</param>
        private static void WriteRandomMask(byte[] buffer, int offset) =>
            s_random.GetBytes(buffer, offset, MaskLength);

        /// <summary>
        /// Receive the next text, binary, continuation, or close message, returning information about it and
        /// writing its payload into the supplied buffer.  Other control messages may be consumed and processed
        /// as part of this operation, but data about them will not be returned.
        /// </summary>
        /// <param name="payloadBuffer">The buffer into which payload data should be written.</param>
        /// <param name="cancellationToken">The CancellationToken used to cancel the websocket.</param>
        /// <param name="resultGetter">Used to get the result.  Allows the same method to be used with both <see cref="WebSocketReceiveResult"/> and <see cref="ValueWebSocketReceiveResult"/>.</param>
        /// <returns>Information about the received message.</returns>
        private async ValueTask<TWebSocketReceiveResult> ReceiveAsyncPrivate<TWebSocketReceiveResultGetter, TWebSocketReceiveResult>(
            Memory<byte> payloadBuffer,
            CancellationToken cancellationToken,
            TWebSocketReceiveResultGetter resultGetter = default)
            where TWebSocketReceiveResultGetter : struct, IWebSocketReceiveResultGetter<TWebSocketReceiveResult> // constrained to avoid boxing and enable inlining
        {
            // This is a long method.  While splitting it up into pieces would arguably help with readability, doing so would
            // also result in more allocations, as each async method that yields ends up with multiple allocations.  The impact
            // of those allocations is amortized across all of the awaits in the method, and since we generally expect a receive
            // operation to require at most a single yield (while waiting for data to arrive), it's more efficient to have
            // everything in the one method.  We do separate out pieces for handling close and ping/pong messages, as we expect
            // those to be much less frequent (e.g. we should only get one close per websocket), and thus we can afford to pay
            // a bit more for readability and maintainability.

            CancellationTokenRegistration registration = cancellationToken.Register(s => ((ManagedWebSocket)s).Abort(), this);
            try
            {
                while (true) // in case we get control frames that should be ignored from the user's perspective
                {
                    // Get the last received header.  If its payload length is non-zero, that means we previously
                    // received the header but were only able to read a part of the fragment, so we should skip
                    // reading another header and just proceed to use that same header and read more data associated
                    // with it.  If instead its payload length is zero, then we've completed the processing of
                    // thta message, and we should read the next header.
                    MessageHeader header = _lastReceiveHeader;
                    if (header.PayloadLength == 0)
                    {
                        if (_receiveBufferCount < (_isServer ? (MaxMessageHeaderLength - MaskLength) : MaxMessageHeaderLength))
                        {
                            // Make sure we have the first two bytes, which includes the start of the payload length.
                            if (_receiveBufferCount < 2)
                            {
                                await EnsureBufferContainsAsync(2, cancellationToken, throwOnPrematureClosure: true).ConfigureAwait(false);
                            }

                            // Then make sure we have the full header based on the payload length.
                            // If this is the server, we also need room for the received mask.
                            long payloadLength = _receiveBuffer.Span[_receiveBufferOffset + 1] & 0x7F;
                            if (_isServer || payloadLength > 125)
                            {
                                int minNeeded =
                                    2 +
                                    (_isServer ? MaskLength : 0) +
                                    (payloadLength <= 125 ? 0 : payloadLength == 126 ? sizeof(ushort) : sizeof(ulong)); // additional 2 or 8 bytes for 16-bit or 64-bit length
                                await EnsureBufferContainsAsync(minNeeded, cancellationToken).ConfigureAwait(false);
                            }
                        }

                        if (!TryParseMessageHeaderFromReceiveBuffer(out header))
                        {
                            await CloseWithReceiveErrorAndThrowAsync(WebSocketCloseStatus.ProtocolError, WebSocketError.Faulted, cancellationToken).ConfigureAwait(false);
                        }
                        _receivedMaskOffsetOffset = 0;
                    }

                    // If the header represents a ping or a pong, it's a control message meant
                    // to be transparent to the user, so handle it and then loop around to read again.
                    // Alternatively, if it's a close message, handle it and exit.
                    if (header.Opcode == MessageOpcode.Ping || header.Opcode == MessageOpcode.Pong)
                    {
                        await HandleReceivedPingPongAsync(header, cancellationToken).ConfigureAwait(false);
                        continue;
                    }
                    else if (header.Opcode == MessageOpcode.Close)
                    {
                        await HandleReceivedCloseAsync(header, cancellationToken).ConfigureAwait(false);
                        return resultGetter.GetResult(0, WebSocketMessageType.Close, true, _closeStatus, _closeStatusDescription);
                    }

                    // If this is a continuation, replace the opcode with the one of the message it's continuing
                    if (header.Opcode == MessageOpcode.Continuation)
                    {
                        header.Opcode = _lastReceiveHeader.Opcode;
                    }

                    // The message should now be a binary or text message.  Handle it by reading the payload and returning the contents.
                    Debug.Assert(header.Opcode == MessageOpcode.Binary || header.Opcode == MessageOpcode.Text, $"Unexpected opcode {header.Opcode}");

                    // If there's no data to read, return an appropriate result.
                    if (header.PayloadLength == 0 || payloadBuffer.Length == 0)
                    {
                        _lastReceiveHeader = header;
                        return resultGetter.GetResult(
                            0,
                            header.Opcode == MessageOpcode.Text ? WebSocketMessageType.Text : WebSocketMessageType.Binary,
                            header.Fin && header.PayloadLength == 0,
                            null, null);
                    }

                    // Otherwise, read as much of the payload as we can efficiently, and upate the header to reflect how much data
                    // remains for future reads.
                    int bytesToCopy = Math.Min(payloadBuffer.Length, (int)Math.Min(header.PayloadLength, _receiveBuffer.Length));
                    Debug.Assert(bytesToCopy > 0, $"Expected {nameof(bytesToCopy)} > 0");
                    if (_receiveBufferCount < bytesToCopy)
                    {
                        await EnsureBufferContainsAsync(bytesToCopy, cancellationToken, throwOnPrematureClosure: true).ConfigureAwait(false);
                    }

                    if (_isServer)
                    {
                        _receivedMaskOffsetOffset = ApplyMask(_receiveBuffer.Span.Slice(_receiveBufferOffset, bytesToCopy), header.Mask, _receivedMaskOffsetOffset);
                    }
                    _receiveBuffer.Span.Slice(_receiveBufferOffset, bytesToCopy).CopyTo(payloadBuffer.Span.Slice(0, bytesToCopy));
                    ConsumeFromBuffer(bytesToCopy);
                    header.PayloadLength -= bytesToCopy;

                    // If this a text message, validate that it contains valid UTF8.
                    if (header.Opcode == MessageOpcode.Text &&
                        !TryValidateUtf8(payloadBuffer.Span.Slice(0, bytesToCopy), header.Fin, _utf8TextState))
                    {
                        await CloseWithReceiveErrorAndThrowAsync(WebSocketCloseStatus.InvalidPayloadData, WebSocketError.Faulted, cancellationToken).ConfigureAwait(false);
                    }

                    _lastReceiveHeader = header;
                    return resultGetter.GetResult(
                        bytesToCopy,
                        header.Opcode == MessageOpcode.Text ? WebSocketMessageType.Text : WebSocketMessageType.Binary,
                        header.Fin && header.PayloadLength == 0,
                        null, null);
                }
            }
            catch (Exception exc)
            {
                if (_state == WebSocketState.Aborted)
                {
                    throw new OperationCanceledException(nameof(WebSocketState.Aborted), exc);
                }
                throw new WebSocketException(WebSocketError.ConnectionClosedPrematurely, exc);
            }
            finally
            {
                registration.Dispose();
            }
        }

        /// <summary>Processes a received close message.</summary>
        /// <param name="header">The message header.</param>
        /// <param name="cancellationToken">The cancellation token to use to cancel the websocket.</param>
        /// <returns>The received result message.</returns>
        private async Task HandleReceivedCloseAsync(
            MessageHeader header, CancellationToken cancellationToken)
        {
            lock (StateUpdateLock)
            {
                _receivedCloseFrame = true;
                if (_state < WebSocketState.CloseReceived)
                {
                    _state = WebSocketState.CloseReceived;
                }
            }

            WebSocketCloseStatus closeStatus = WebSocketCloseStatus.NormalClosure;
            string closeStatusDescription = string.Empty;

            // Handle any payload by parsing it into the close status and description.
            if (header.PayloadLength == 1)
            {
                // The close payload length can be 0 or >= 2, but not 1.
                await CloseWithReceiveErrorAndThrowAsync(WebSocketCloseStatus.ProtocolError, WebSocketError.Faulted, cancellationToken).ConfigureAwait(false);
            }
            else if (header.PayloadLength >= 2)
            {
                if (_receiveBufferCount < header.PayloadLength)
                {
                    await EnsureBufferContainsAsync((int)header.PayloadLength, cancellationToken).ConfigureAwait(false);
                }

                if (_isServer)
                {
                    ApplyMask(_receiveBuffer.Span.Slice(_receiveBufferOffset, (int)header.PayloadLength), header.Mask, 0);
                }

                closeStatus = (WebSocketCloseStatus)(_receiveBuffer.Span[_receiveBufferOffset] << 8 | _receiveBuffer.Span[_receiveBufferOffset + 1]);
                if (!IsValidCloseStatus(closeStatus))
                {
                    await CloseWithReceiveErrorAndThrowAsync(WebSocketCloseStatus.ProtocolError, WebSocketError.Faulted, cancellationToken).ConfigureAwait(false);
                }

                if (header.PayloadLength > 2)
                {
                    try
                    {
                        closeStatusDescription = s_textEncoding.GetString(_receiveBuffer.Span.Slice(_receiveBufferOffset + 2, (int)header.PayloadLength - 2));
                    }
                    catch (DecoderFallbackException exc)
                    {
                        await CloseWithReceiveErrorAndThrowAsync(WebSocketCloseStatus.ProtocolError, WebSocketError.Faulted, cancellationToken, exc).ConfigureAwait(false);
                    }
                }
                ConsumeFromBuffer((int)header.PayloadLength);
            }

            // Store the close status and description onto the instance.
            _closeStatus = closeStatus;
            _closeStatusDescription = closeStatusDescription;
        }

        /// <summary>Processes a received ping or pong message.</summary>
        /// <param name="header">The message header.</param>
        /// <param name="cancellationToken">The cancellation token to use to cancel the websocket.</param>
        private async Task HandleReceivedPingPongAsync(MessageHeader header, CancellationToken cancellationToken)
        {
            // Consume any (optional) payload associated with the ping/pong.
            if (header.PayloadLength > 0 && _receiveBufferCount < header.PayloadLength)
            {
                await EnsureBufferContainsAsync((int)header.PayloadLength, cancellationToken).ConfigureAwait(false);
            }

            // If this was a ping, send back a pong response.
            if (header.Opcode == MessageOpcode.Ping)
            {
                if (_isServer)
                {
                    ApplyMask(_receiveBuffer.Span.Slice(_receiveBufferOffset, (int)header.PayloadLength), header.Mask, 0);
                }

                await SendFrameAsync(
                    MessageOpcode.Pong, true,
                    _receiveBuffer.Slice(_receiveBufferOffset, (int)header.PayloadLength), cancellationToken).ConfigureAwait(false);
            }

            // Regardless of whether it was a ping or pong, we no longer need the payload.
            if (header.PayloadLength > 0)
            {
                ConsumeFromBuffer((int)header.PayloadLength);
            }
        }

        /// <summary>Check whether a close status is valid according to the RFC.</summary>
        /// <param name="closeStatus">The status to validate.</param>
        /// <returns>true if the status if valid; otherwise, false.</returns>
        private static bool IsValidCloseStatus(WebSocketCloseStatus closeStatus)
        {
            // 0-999: "not used"
            // 1000-2999: reserved for the protocol; we need to check individual codes manually
            // 3000-3999: reserved for use by higher-level code
            // 4000-4999: reserved for private use
            // 5000-: not mentioned in RFC

            if (closeStatus < (WebSocketCloseStatus)1000 || closeStatus >= (WebSocketCloseStatus)5000)
            {
                return false;
            }

            if (closeStatus >= (WebSocketCloseStatus)3000)
            {
                return true;
            }

            switch (closeStatus) // check for the 1000-2999 range known codes
            {
                case WebSocketCloseStatus.EndpointUnavailable:
                case WebSocketCloseStatus.InternalServerError:
                case WebSocketCloseStatus.InvalidMessageType:
                case WebSocketCloseStatus.InvalidPayloadData:
                case WebSocketCloseStatus.MandatoryExtension:
                case WebSocketCloseStatus.MessageTooBig:
                case WebSocketCloseStatus.NormalClosure:
                case WebSocketCloseStatus.PolicyViolation:
                case WebSocketCloseStatus.ProtocolError:
                    return true;

                default:
                    return false;
            }
        }

        /// <summary>Send a close message to the server and throw an exception, in response to getting bad data from the server.</summary>
        /// <param name="closeStatus">The close status code to use.</param>
        /// <param name="error">The error reason.</param>
        /// <param name="cancellationToken">The CancellationToken used to cancel the websocket.</param>
        /// <param name="innerException">An optional inner exception to include in the thrown exception.</param>
        private async Task CloseWithReceiveErrorAndThrowAsync(
            WebSocketCloseStatus closeStatus, WebSocketError error, CancellationToken cancellationToken, Exception innerException = null)
        {
            // Close the connection if it hasn't already been closed
            if (!_sentCloseFrame)
            {
                await CloseOutputAsync(closeStatus, string.Empty, cancellationToken).ConfigureAwait(false);
            }

            // Dump our receive buffer; we're in a bad state to do any further processing
            _receiveBufferCount = 0;

            // Let the caller know we've failed
            throw new WebSocketException(error, innerException);
        }

        /// <summary>Parses a message header from the buffer.  This assumes the header is in the buffer.</summary>
        /// <param name="resultHeader">The read header.</param>
        /// <returns>true if a header was read; false if the header was invalid.</returns>
        private bool TryParseMessageHeaderFromReceiveBuffer(out MessageHeader resultHeader)
        {
            Debug.Assert(_receiveBufferCount >= 2, $"Expected to at least have the first two bytes of the header.");

            var header = new MessageHeader();
            Span<byte> receiveBufferSpan = _receiveBuffer.Span;

            header.Fin = (receiveBufferSpan[_receiveBufferOffset] & 0x80) != 0;
            bool reservedSet = (receiveBufferSpan[_receiveBufferOffset] & 0x70) != 0;
            header.Opcode = (MessageOpcode)(receiveBufferSpan[_receiveBufferOffset] & 0xF);

            bool masked = (receiveBufferSpan[_receiveBufferOffset + 1] & 0x80) != 0;
            header.PayloadLength = receiveBufferSpan[_receiveBufferOffset + 1] & 0x7F;

            ConsumeFromBuffer(2);

            // Read the remainder of the payload length, if necessary
            if (header.PayloadLength == 126)
            {
                Debug.Assert(_receiveBufferCount >= 2, $"Expected to have two bytes for the payload length.");
                header.PayloadLength = (receiveBufferSpan[_receiveBufferOffset] << 8) | receiveBufferSpan[_receiveBufferOffset + 1];
                ConsumeFromBuffer(2);
            }
            else if (header.PayloadLength == 127)
            {
                Debug.Assert(_receiveBufferCount >= 8, $"Expected to have eight bytes for the payload length.");
                header.PayloadLength = 0;
                for (int i = 0; i < 8; i++)
                {
                    header.PayloadLength = (header.PayloadLength << 8) | receiveBufferSpan[_receiveBufferOffset + i];
                }
                ConsumeFromBuffer(8);
            }

            bool shouldFail = reservedSet;
            if (masked)
            {
                if (!_isServer)
                {
                    shouldFail = true;
                }
                header.Mask = CombineMaskBytes(receiveBufferSpan, _receiveBufferOffset);

                // Consume the mask bytes
                ConsumeFromBuffer(4);
            }

            // Do basic validation of the header
            switch (header.Opcode)
            {
                case MessageOpcode.Continuation:
                    if (_lastReceiveHeader.Fin)
                    {
                        // Can't continue from a final message
                        shouldFail = true;
                    }
                    break;

                case MessageOpcode.Binary:
                case MessageOpcode.Text:
                    if (!_lastReceiveHeader.Fin)
                    {
                        // Must continue from a non-final message
                        shouldFail = true;
                    }
                    break;

                case MessageOpcode.Close:
                case MessageOpcode.Ping:
                case MessageOpcode.Pong:
                    if (header.PayloadLength > MaxControlPayloadLength || !header.Fin)
                    {
                        // Invalid control messgae
                        shouldFail = true;
                    }
                    break;

                default:
                    // Unknown opcode
                    shouldFail = true;
                    break;
            }

            // Return the read header
            resultHeader = header;
            return !shouldFail;
        }

        /// <summary>Send a close message, then receive until we get a close response message.</summary>
        /// <param name="closeStatus">The close status to send.</param>
        /// <param name="statusDescription">The close status description to send.</param>
        /// <param name="cancellationToken">The CancellationToken to use to cancel the websocket.</param>
        private async Task CloseAsyncPrivate(WebSocketCloseStatus closeStatus, string statusDescription, CancellationToken cancellationToken)
        {
            // Send the close message.  Skip sending a close frame if we're currently in a CloseSent state,
            // for example having just done a CloseOutputAsync.
            if (!_sentCloseFrame)
            {
                await SendCloseFrameAsync(closeStatus, statusDescription, cancellationToken).ConfigureAwait(false);
            }

            // We should now either be in a CloseSent case (because we just sent one), or in a CloseReceived state, in case
            // there was a concurrent receive that ended up handling an immediate close frame response from the server.
            // Of course it could also be Aborted if something happened concurrently to cause things to blow up.
            Debug.Assert(
                State == WebSocketState.CloseSent ||
                State == WebSocketState.CloseReceived ||
                State == WebSocketState.Aborted,
                $"Unexpected state {State}.");

            // Wait until we've received a close response
            byte[] closeBuffer = ArrayPool<byte>.Shared.Rent(MaxMessageHeaderLength + MaxControlPayloadLength);
            try
            {
                while (!_receivedCloseFrame)
                {
                    Debug.Assert(!Monitor.IsEntered(StateUpdateLock), $"{nameof(StateUpdateLock)} must never be held when acquiring {nameof(ReceiveAsyncLock)}");
                    Task receiveTask;
                    lock (ReceiveAsyncLock)
                    {
                        // Now that we're holding the ReceiveAsyncLock, double-check that we've not yet received the close frame.
                        // It could have been received between our check above and now due to a concurrent receive completing.
                        if (_receivedCloseFrame)
                        {
                            break;
                        }

                        // We've not yet processed a received close frame, which means we need to wait for a received close to complete.
                        // There may already be one in flight, in which case we want to just wait for that one rather than kicking off
                        // another (we don't support concurrent receive operations).  We need to kick off a new receive if either we've
                        // never issued a receive or if the last issued receive completed for reasons other than a close frame.  There is
                        // a race condition here, e.g. if there's a in-flight receive that completes after we check, but that's fine: worst
                        // case is we then await it, find that it's not what we need, and try again.
                        receiveTask = _lastReceiveAsync;
                        _lastReceiveAsync = receiveTask = ValidateAndReceiveAsync(receiveTask, closeBuffer, cancellationToken);
                    }

                    // Wait for whatever receive task we have.  We'll then loop around again to re-check our state.
                    Debug.Assert(receiveTask != null);
                    await receiveTask.ConfigureAwait(false);
                }
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(closeBuffer);
            }

            // We're closed.  Close the connection and update the status.
            lock (StateUpdateLock)
            {
                DisposeCore();
                if (_state < WebSocketState.Closed)
                {
                    _state = WebSocketState.Closed;
                }
            }
        }

        /// <summary>Sends a close message to the server.</summary>
        /// <param name="closeStatus">The close status to send.</param>
        /// <param name="closeStatusDescription">The close status description to send.</param>
        /// <param name="cancellationToken">The CancellationToken to use to cancel the websocket.</param>
        private async Task SendCloseFrameAsync(WebSocketCloseStatus closeStatus, string closeStatusDescription, CancellationToken cancellationToken)
        {
            // Close payload is two bytes containing the close status followed by a UTF8-encoding of the status description, if it exists.

            byte[] buffer = null;
            try
            {
                int count = 2;
                if (string.IsNullOrEmpty(closeStatusDescription))
                {
                    buffer = ArrayPool<byte>.Shared.Rent(count);
                }
                else
                {
                    count += s_textEncoding.GetByteCount(closeStatusDescription);
                    buffer = ArrayPool<byte>.Shared.Rent(count);
                    int encodedLength = s_textEncoding.GetBytes(closeStatusDescription, 0, closeStatusDescription.Length, buffer, 2);
                    Debug.Assert(count - 2 == encodedLength, $"GetByteCount and GetBytes encoded count didn't match");
                }

                ushort closeStatusValue = (ushort)closeStatus;
                buffer[0] = (byte)(closeStatusValue >> 8);
                buffer[1] = (byte)(closeStatusValue & 0xFF);

                await SendFrameAsync(MessageOpcode.Close, true, new Memory<byte>(buffer, 0, count), cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                if (buffer != null)
                {
                    ArrayPool<byte>.Shared.Return(buffer);
                }
            }

            lock (StateUpdateLock)
            {
                _sentCloseFrame = true;
                if (_state <= WebSocketState.CloseReceived)
                {
                    _state = WebSocketState.CloseSent;
                }
            }
        }

        private void ConsumeFromBuffer(int count)
        {
            Debug.Assert(count >= 0, $"Expected non-negative count, got {count}");
            Debug.Assert(count <= _receiveBufferCount, $"Trying to consume {count}, which is more than exists {_receiveBufferCount}");
            _receiveBufferCount -= count;
            _receiveBufferOffset += count;
        }

        private async Task EnsureBufferContainsAsync(int minimumRequiredBytes, CancellationToken cancellationToken, bool throwOnPrematureClosure = true)
        {
            Debug.Assert(minimumRequiredBytes <= _receiveBuffer.Length, $"Requested number of bytes {minimumRequiredBytes} must not exceed {_receiveBuffer.Length}");

            // If we don't have enough data in the buffer to satisfy the minimum required, read some more.
            if (_receiveBufferCount < minimumRequiredBytes)
            {
                // If there's any data in the buffer, shift it down.  
                if (_receiveBufferCount > 0)
                {
                    _receiveBuffer.Span.Slice(_receiveBufferOffset, _receiveBufferCount).CopyTo(_receiveBuffer.Span);
                }
                _receiveBufferOffset = 0;

                // While we don't have enough data, read more.
                while (_receiveBufferCount < minimumRequiredBytes)
                {
                    int numRead = await _stream.ReadAsync(_receiveBuffer.Slice(_receiveBufferCount, _receiveBuffer.Length - _receiveBufferCount), cancellationToken).ConfigureAwait(false);
                    Debug.Assert(numRead >= 0, $"Expected non-negative bytes read, got {numRead}");
                    _receiveBufferCount += numRead;
                    if (numRead == 0)
                    {
                        // The connection closed before we were able to read everything we needed.
                        // If it was due to use being disposed, fail.  If it was due to the connection
                        // being closed and it wasn't expected, fail.  If it was due to the connection
                        // being closed and that was expected, exit gracefully.
                        if (_disposed)
                        {
                            throw new ObjectDisposedException(nameof(WebSocket));
                        }
                        else if (throwOnPrematureClosure)
                        {
                            throw new WebSocketException(WebSocketError.ConnectionClosedPrematurely);
                        }
                        break;
                    }
                }
            }
        }

        /// <summary>Gets a send buffer from the pool.</summary>
        private void AllocateSendBuffer(int minLength)
        {
            Debug.Assert(_sendBuffer == null); // would only fail if had some catastrophic error previously that prevented cleaning up
            _sendBuffer = ArrayPool<byte>.Shared.Rent(minLength);
        }

        /// <summary>Releases the send buffer to the pool.</summary>
        private void ReleaseSendBuffer()
        {
            byte[] old = _sendBuffer;
            if (old != null)
            {
                _sendBuffer = null;
                ArrayPool<byte>.Shared.Return(old);
            }
        }

        private static unsafe int CombineMaskBytes(Span<byte> buffer, int maskOffset) =>
            BitConverter.ToInt32(buffer.Slice(maskOffset));

        /// <summary>Applies a mask to a portion of a byte array.</summary>
        /// <param name="toMask">The buffer to which the mask should be applied.</param>
        /// <param name="mask">The array containing the mask to apply.</param>
        /// <param name="maskOffset">The offset into <paramref name="mask"/> of the mask to apply of length <see cref="MaskLength"/>.</param>
        /// <param name="maskOffsetIndex">The next position offset from <paramref name="maskOffset"/> of which by to apply next from the mask.</param>
        /// <returns>The updated maskOffsetOffset value.</returns>
        private static int ApplyMask(Span<byte> toMask, byte[] mask, int maskOffset, int maskOffsetIndex)
        {
            Debug.Assert(maskOffsetIndex < MaskLength, $"Unexpected {nameof(maskOffsetIndex)}: {maskOffsetIndex}");
            Debug.Assert(mask.Length >= MaskLength + maskOffset, $"Unexpected inputs: {mask.Length}, {maskOffset}");
            return ApplyMask(toMask, CombineMaskBytes(mask, maskOffset), maskOffsetIndex);
        }

        /// <summary>Applies a mask to a portion of a byte array.</summary>
        /// <param name="toMask">The buffer to which the mask should be applied.</param>
        /// <param name="mask">The four-byte mask, stored as an Int32.</param>
        /// <param name="maskIndex">The index into the mask.</param>
        /// <returns>The next index into the mask to be used for future applications of the mask.</returns>
        private static unsafe int ApplyMask(Span<byte> toMask, int mask, int maskIndex)
        {
            Debug.Assert(maskIndex < sizeof(int));

            int maskShift = maskIndex * 8;
            int shiftedMask = (int)(((uint)mask >> maskShift) | ((uint)mask << (32 - maskShift)));

            // Try to use SIMD.  We can if the number of bytes we're trying to mask is at least as much
            // as the width of a vector and if the width is an even multiple of the mask.
            if (Vector.IsHardwareAccelerated &&
                Vector<byte>.Count % sizeof(int) == 0 &&
                toMask.Length >= Vector<byte>.Count)
            {
                Vector<byte> maskVector = Vector.AsVectorByte(new Vector<int>(shiftedMask));
                Span<Vector<byte>> toMaskVector = toMask.NonPortableCast<byte, Vector<byte>>();
                for (int i = 0; i < toMaskVector.Length; i++)
                {
                    toMaskVector[i] ^= maskVector;
                }

                // Fall through to processing any remaining bytes that were less than a vector width.
                toMask = toMask.Slice(Vector<byte>.Count * toMaskVector.Length);
            }

            // If there are any bytes remaining (either we couldn't use vectors, or the count wasn't
            // an even multiple of the vector width), process them without vectors.
            int count = toMask.Length;
            if (count > 0)
            {
                fixed (byte* toMaskPtr = &MemoryMarshal.GetReference(toMask))
                {
                    byte* p = toMaskPtr;

                    // Try to go an int at a time if the remaining data is 4-byte aligned and there's enough remaining.
                    if (((long)p % sizeof(int)) == 0)
                    {
                        while (count >= sizeof(int))
                        {
                            count -= sizeof(int);
                            *((int*)p) ^= shiftedMask;
                            p += sizeof(int);
                        }

                        // We don't need to update the maskIndex, as its mod-4 value won't have changed.
                        // `p` points to the remainder.
                    }

                    // Process any remaining data a byte at a time.
                    if (count > 0)
                    {
                        byte* maskPtr = (byte*)&mask;
                        byte* end = p + count;
                        while (p < end)
                        {
                            *p++ ^= maskPtr[maskIndex];
                            maskIndex = (maskIndex + 1) & 3;
                        }
                    }
                }
            }

            // Return the updated index.
            return maskIndex;
        }

        /// <summary>Aborts the websocket and throws an exception if an existing operation is in progress.</summary>
        private void ThrowIfOperationInProgress(Task operationTask, [CallerMemberName] string methodName = null)
        {
            if (operationTask != null && !operationTask.IsCompleted)
            {
                Abort();
                throw new InvalidOperationException(SR.Format(SR.net_Websockets_AlreadyOneOutstandingOperation, methodName));
            }
        }

        /// <summary>Creates an OperationCanceledException instance, using a default message and the specified inner exception and token.</summary>
        private static Exception CreateOperationCanceledException(Exception innerException, CancellationToken cancellationToken = default(CancellationToken))
        {
            return new OperationCanceledException(
                new OperationCanceledException().Message,
                innerException,
                cancellationToken);
        }

        // From https://raw.githubusercontent.com/aspnet/WebSockets/dev/src/Microsoft.AspNetCore.WebSockets.Protocol/Utilities.cs
        // Performs a stateful validation of UTF-8 bytes.
        // It checks for valid formatting, overlong encodings, surrogates, and value ranges.
        private static bool TryValidateUtf8(Span<byte> span, bool endOfMessage, Utf8MessageState state)
        {
            for (int i = 0; i < span.Length;)
            {
                // Have we started a character sequence yet?
                if (!state.SequenceInProgress)
                {
                    // The first byte tells us how many bytes are in the sequence.
                    state.SequenceInProgress = true;
                    byte b = span[i];
                    i++;
                    if ((b & 0x80) == 0) // 0bbbbbbb, single byte
                    {
                        state.AdditionalBytesExpected = 0;
                        state.CurrentDecodeBits = b & 0x7F;
                        state.ExpectedValueMin = 0;
                    }
                    else if ((b & 0xC0) == 0x80)
                    {
                        // Misplaced 10bbbbbb continuation byte. This cannot be the first byte.
                        return false;
                    }
                    else if ((b & 0xE0) == 0xC0) // 110bbbbb 10bbbbbb
                    {
                        state.AdditionalBytesExpected = 1;
                        state.CurrentDecodeBits = b & 0x1F;
                        state.ExpectedValueMin = 0x80;
                    }
                    else if ((b & 0xF0) == 0xE0) // 1110bbbb 10bbbbbb 10bbbbbb
                    {
                        state.AdditionalBytesExpected = 2;
                        state.CurrentDecodeBits = b & 0xF;
                        state.ExpectedValueMin = 0x800;
                    }
                    else if ((b & 0xF8) == 0xF0) // 11110bbb 10bbbbbb 10bbbbbb 10bbbbbb
                    {
                        state.AdditionalBytesExpected = 3;
                        state.CurrentDecodeBits = b & 0x7;
                        state.ExpectedValueMin = 0x10000;
                    }
                    else // 111110bb & 1111110b & 11111110 && 11111111 are not valid
                    {
                        return false;
                    }
                }
                while (state.AdditionalBytesExpected > 0 && i < span.Length)
                {
                    byte b = span[i];
                    if ((b & 0xC0) != 0x80)
                    {
                        return false;
                    }

                    i++;
                    state.AdditionalBytesExpected--;

                    // Each continuation byte carries 6 bits of data 0x10bbbbbb.
                    state.CurrentDecodeBits = (state.CurrentDecodeBits << 6) | (b & 0x3F);

                    if (state.AdditionalBytesExpected == 1 && state.CurrentDecodeBits >= 0x360 && state.CurrentDecodeBits <= 0x37F)
                    {
                        // This is going to end up in the range of 0xD800-0xDFFF UTF-16 surrogates that are not allowed in UTF-8;
                        return false;
                    }
                    if (state.AdditionalBytesExpected == 2 && state.CurrentDecodeBits >= 0x110)
                    {
                        // This is going to be out of the upper Unicode bound 0x10FFFF.
                        return false;
                    }
                }
                if (state.AdditionalBytesExpected == 0)
                {
                    state.SequenceInProgress = false;
                    if (state.CurrentDecodeBits < state.ExpectedValueMin)
                    {
                        // Overlong encoding (e.g. using 2 bytes to encode something that only needed 1).
                        return false;
                    }
                }
            }
            if (endOfMessage && state.SequenceInProgress)
            {
                return false;
            }
            return true;
        }

        private sealed class Utf8MessageState
        {
            internal bool SequenceInProgress;
            internal int AdditionalBytesExpected;
            internal int ExpectedValueMin;
            internal int CurrentDecodeBits;
        }

        private enum MessageOpcode : byte
        {
            Continuation = 0x0,
            Text = 0x1,
            Binary = 0x2,
            Close = 0x8,
            Ping = 0x9,
            Pong = 0xA
        }

        [StructLayout(LayoutKind.Auto)]
        private struct MessageHeader
        {
            internal MessageOpcode Opcode;
            internal bool Fin;
            internal long PayloadLength;
            internal int Mask;
        }

        /// <summary>
        /// Interface used by <see cref="ReceiveAsyncPrivate"/> to enable it to return
        /// different result types in an efficient manner.
        /// </summary>
        /// <typeparam name="TResult">The type of the result</typeparam>
        private interface IWebSocketReceiveResultGetter<TResult>
        {
            TResult GetResult(int count, WebSocketMessageType messageType, bool endOfMessage, WebSocketCloseStatus? closeStatus, string closeDescription);
        }

        /// <summary><see cref="IWebSocketReceiveResultGetter{TResult}"/> implementation for <see cref="WebSocketReceiveResult"/>.</summary>
        private readonly struct WebSocketReceiveResultGetter : IWebSocketReceiveResultGetter<WebSocketReceiveResult>
        {
            public WebSocketReceiveResult GetResult(int count, WebSocketMessageType messageType, bool endOfMessage, WebSocketCloseStatus? closeStatus, string closeDescription) =>
                new WebSocketReceiveResult(count, messageType, endOfMessage, closeStatus, closeDescription);
        }
    }
}
