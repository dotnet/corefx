// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Buffers.Binary;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http.Headers;
using System.Net.Http.HPack;
using System.Net.Security;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace System.Net.Http
{
    internal sealed partial class Http2Connection : HttpConnectionBase, IDisposable
    {
        private readonly HttpConnectionPool _pool;
        private readonly Stream _stream;

        // NOTE: These are mutable structs; do not make these readonly.
        private ArrayBuffer _incomingBuffer;
        private ArrayBuffer _outgoingBuffer;
        private ArrayBuffer _headerBuffer;

        /// <summary>Reusable array used to get the values for each header being written to the wire.</summary>
        private string[] _headerValues = Array.Empty<string>();

        private int _currentWriteSize;      // as passed to StartWriteAsync

        private readonly HPackDecoder _hpackDecoder;

        private readonly Dictionary<int, Http2Stream> _httpStreams;

        private readonly SemaphoreSlim _writerLock;
        private readonly SemaphoreSlim _headerSerializationLock;

        private readonly CreditManager _connectionWindow;
        private readonly CreditManager _concurrentStreams;

        private int _nextStream;
        private bool _expectingSettingsAck;
        private int _initialWindowSize;
        private int _maxConcurrentStreams;
        private int _pendingWindowUpdate;
        private long _idleSinceTickCount;
        private int _pendingWriters;
        private bool _lastPendingWriterShouldFlush;

        // This means that the pool has disposed us, but there may still be
        // requests in flight that will continue to be processed.
        private bool _disposed;

        // This will be set when:
        // (1) We receive GOAWAY -- will be set to the value sent in the GOAWAY frame
        // (2) A connection IO error occurs -- will be set to int.MaxValue
        //     (meaning we must assume all streams have been processed by the server)
        private int _lastStreamId = -1;

        // This will be set when a connection IO error occurs
        private Exception _abortException;

        // If an in-progress write is canceled we need to be able to immediately
        // report a cancellation to the user, but also block the connection until
        // the write completes. We avoid actually canceling the write, as we would
        // then have to close the whole connection.
        private Task _inProgressWrite = null;

        private const int MaxStreamId = int.MaxValue;

        private static readonly byte[] s_http2ConnectionPreface = Encoding.ASCII.GetBytes("PRI * HTTP/2.0\r\n\r\nSM\r\n\r\n");

#if DEBUG
        // In debug builds, start with a very small buffer to induce buffer growing logic.
        private const int InitialConnectionBufferSize = 4;
#else
        private const int InitialConnectionBufferSize = 4096;
#endif

        private const int DefaultInitialWindowSize = 65535;

        // We don't really care about limiting control flow at the connection level.
        // We limit it per stream, and the user controls how many streams are created.
        // So set the connection window size to a large value.
        private const int ConnectionWindowSize = 64 * 1024 * 1024;

        // We hold off on sending WINDOW_UPDATE until we hit thi minimum threshold.
        // This value is somewhat arbitrary; the intent is to ensure it is much smaller than
        // the window size itself, or we risk stalling the server because it runs out of window space.
        // If we want to further reduce the frequency of WINDOW_UPDATEs, it's probably better to
        // increase the window size (and thus increase the threshold proportionally)
        // rather than just increase the threshold.
        private const int ConnectionWindowThreshold = ConnectionWindowSize / 8;

        // When buffering outgoing writes, we will automatically buffer up to this number of bytes.
        // Single writes that are larger than the buffer can cause the buffer to expand beyond
        // this value, so this is not a hard maximum size.
        private const int UnflushedOutgoingBufferSize = 32 * 1024;

        public Http2Connection(HttpConnectionPool pool, Stream stream)
        {
            _pool = pool;
            _stream = stream;
            _incomingBuffer = new ArrayBuffer(InitialConnectionBufferSize);
            _outgoingBuffer = new ArrayBuffer(InitialConnectionBufferSize);
            _headerBuffer = new ArrayBuffer(InitialConnectionBufferSize);

            _hpackDecoder = new HPackDecoder(maxResponseHeadersLength: pool.Settings._maxResponseHeadersLength * 1024);

            _httpStreams = new Dictionary<int, Http2Stream>();

            _writerLock = new SemaphoreSlim(1, 1);
            _headerSerializationLock = new SemaphoreSlim(1, 1);
            _connectionWindow = new CreditManager(this, nameof(_connectionWindow), DefaultInitialWindowSize);
            _concurrentStreams = new CreditManager(this, nameof(_concurrentStreams), int.MaxValue);

            _nextStream = 1;
            _initialWindowSize = DefaultInitialWindowSize;
            _maxConcurrentStreams = int.MaxValue;
            _pendingWindowUpdate = 0;

            if (NetEventSource.IsEnabled) TraceConnection(stream);
        }

        private object SyncObject => _httpStreams;

        public async ValueTask SetupAsync()
        {
            _outgoingBuffer.EnsureAvailableSpace(s_http2ConnectionPreface.Length +
                FrameHeader.Size + FrameHeader.SettingLength +
                FrameHeader.Size + FrameHeader.WindowUpdateLength);

            // Send connection preface
            s_http2ConnectionPreface.AsSpan().CopyTo(_outgoingBuffer.AvailableSpan);
            _outgoingBuffer.Commit(s_http2ConnectionPreface.Length);

            // Send SETTINGS frame
            WriteFrameHeader(new FrameHeader(FrameHeader.SettingLength, FrameType.Settings, FrameFlags.None, 0));

            // Disable push promise
            BinaryPrimitives.WriteUInt16BigEndian(_outgoingBuffer.AvailableSpan, (ushort)SettingId.EnablePush);
            _outgoingBuffer.Commit(2);
            BinaryPrimitives.WriteUInt32BigEndian(_outgoingBuffer.AvailableSpan, 0);
            _outgoingBuffer.Commit(4);

            // Send initial connection-level WINDOW_UPDATE
            WriteFrameHeader(new FrameHeader(FrameHeader.WindowUpdateLength, FrameType.WindowUpdate, FrameFlags.None, 0));
            BinaryPrimitives.WriteUInt32BigEndian(_outgoingBuffer.AvailableSpan, (ConnectionWindowSize - DefaultInitialWindowSize));
            _outgoingBuffer.Commit(4);

            await _stream.WriteAsync(_outgoingBuffer.ActiveMemory).ConfigureAwait(false);
            _outgoingBuffer.Discard(_outgoingBuffer.ActiveLength);

            _expectingSettingsAck = true;

            _ = ProcessIncomingFramesAsync();
        }

        private async ValueTask EnsureIncomingBytesAsync(int minReadBytes)
        {
            if (NetEventSource.IsEnabled) Trace($"{nameof(minReadBytes)}={minReadBytes}");
            if (_incomingBuffer.ActiveLength >= minReadBytes)
            {
                return;
            }

            int bytesNeeded = minReadBytes - _incomingBuffer.ActiveLength;
            _incomingBuffer.EnsureAvailableSpace(bytesNeeded);
            int bytesRead = await ReadAtLeastAsync(_stream, _incomingBuffer.AvailableMemory, bytesNeeded).ConfigureAwait(false);
            _incomingBuffer.Commit(bytesRead);
        }

        private async Task FlushOutgoingBytesAsync()
        {
            if (NetEventSource.IsEnabled) Trace($"{nameof(_outgoingBuffer.ActiveLength)}={_outgoingBuffer.ActiveLength}");
            Debug.Assert(_outgoingBuffer.ActiveLength > 0);

            try
            {
                await _stream.WriteAsync(_outgoingBuffer.ActiveMemory).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                Abort(e);
                throw;
            }
            finally
            {
                _lastPendingWriterShouldFlush = false;
                _outgoingBuffer.Discard(_outgoingBuffer.ActiveLength);
            }
        }

        private async ValueTask<FrameHeader> ReadFrameAsync(bool initialFrame = false)
        {
            if (NetEventSource.IsEnabled) Trace($"{nameof(initialFrame)}={initialFrame}");

            // Read frame header
            await EnsureIncomingBytesAsync(FrameHeader.Size).ConfigureAwait(false);
            FrameHeader frameHeader = FrameHeader.ReadFrom(_incomingBuffer.ActiveSpan);

            if (frameHeader.Length > FrameHeader.MaxLength)
            {
                if (initialFrame && NetEventSource.IsEnabled)
                {
                    string response = Encoding.ASCII.GetString(_incomingBuffer.ActiveSpan.Slice(0, Math.Min(20, _incomingBuffer.ActiveLength)));
                    Trace($"HTTP/2 handshake failed. Server returned {response}");
                }

                _incomingBuffer.Discard(FrameHeader.Size);
                throw new Http2ConnectionException(initialFrame ? Http2ProtocolErrorCode.ProtocolError : Http2ProtocolErrorCode.FrameSizeError);
            }
            _incomingBuffer.Discard(FrameHeader.Size);

            // Read frame contents
            await EnsureIncomingBytesAsync(frameHeader.Length).ConfigureAwait(false);

            return frameHeader;
        }

        private async Task ProcessIncomingFramesAsync()
        {
            try
            {
                FrameHeader frameHeader = await ReadFrameAsync(initialFrame: true).ConfigureAwait(false);
                if (frameHeader.Type != FrameType.Settings || frameHeader.AckFlag)
                {
                    throw new Http2ConnectionException(Http2ProtocolErrorCode.ProtocolError);
                }
                if (NetEventSource.IsEnabled) Trace($"Frame 0: {frameHeader}.");

                // Process the initial SETTINGS frame. This will send an ACK.
                ProcessSettingsFrame(frameHeader);

                // Keep processing frames as they arrive.
                for (long frameNum = 1; ; frameNum++)
                {
                    frameHeader = await ReadFrameAsync().ConfigureAwait(false);
                    if (NetEventSource.IsEnabled) Trace($"Frame {frameNum}: {frameHeader}.");

                    switch (frameHeader.Type)
                    {
                        case FrameType.Headers:
                            await ProcessHeadersFrame(frameHeader).ConfigureAwait(false);
                            break;

                        case FrameType.Data:
                            ProcessDataFrame(frameHeader);
                            break;

                        case FrameType.Settings:
                            ProcessSettingsFrame(frameHeader);
                            break;

                        case FrameType.Priority:
                            ProcessPriorityFrame(frameHeader);
                            break;

                        case FrameType.Ping:
                            ProcessPingFrame(frameHeader);
                            break;

                        case FrameType.WindowUpdate:
                            ProcessWindowUpdateFrame(frameHeader);
                            break;

                        case FrameType.RstStream:
                            ProcessRstStreamFrame(frameHeader);
                            break;

                        case FrameType.GoAway:
                            ProcessGoAwayFrame(frameHeader);
                            break;

                        case FrameType.PushPromise:     // Should not happen, since we disable this in our initial SETTINGS (TODO: ISSUE 31295: We aren't currently, but we should)
                        case FrameType.Continuation:    // Should only be received while processing headers in ProcessHeadersFrame
                        default:
                            throw new Http2ConnectionException(Http2ProtocolErrorCode.ProtocolError);
                    }
                }
            }
            catch (Exception e)
            {
                if (NetEventSource.IsEnabled) Trace($"{nameof(ProcessIncomingFramesAsync)}: {e.Message}");

                Abort(e);
            }
        }

        // Note, this will return null for a streamId that's no longer in use.
        // Callers must check for this and send a RST_STREAM or ignore as appropriate.
        // If the streamId is invalid or the stream is idle, calling this function
        // will result in a connection level error.
        private Http2Stream GetStream(int streamId)
        {
            // TODO: ISSUE 34192: If we implement support for Push Promise, this will
            // need to be updated to track the highest stream ID used by the server in
            // addition to the highest ID used by the client.
            if (streamId <= 0 || streamId >= _nextStream)
            {
                throw new Http2ConnectionException(Http2ProtocolErrorCode.ProtocolError);
            }

            lock (SyncObject)
            {
                if (!_httpStreams.TryGetValue(streamId, out Http2Stream http2Stream))
                {
                    return null;
                }

                return http2Stream;
            }
        }

        private async ValueTask ProcessHeadersFrame(FrameHeader frameHeader)
        {
            if (NetEventSource.IsEnabled) Trace($"{frameHeader}");
            Debug.Assert(frameHeader.Type == FrameType.Headers);

            bool endStream = frameHeader.EndStreamFlag;

            int streamId = frameHeader.StreamId;
            Http2Stream http2Stream = GetStream(streamId);

            // Note, http2Stream will be null if this is a closed stream.
            // We will still process the headers, to ensure the header decoding state is up-to-date,
            // but we will discard the decoded headers.

            http2Stream?.OnHeadersStart();

            _hpackDecoder.Decode(
                GetFrameData(_incomingBuffer.ActiveSpan.Slice(0, frameHeader.Length), frameHeader.PaddedFlag, frameHeader.PriorityFlag),
                frameHeader.EndHeadersFlag,
                http2Stream);
            _incomingBuffer.Discard(frameHeader.Length);

            while (!frameHeader.EndHeadersFlag)
            {
                frameHeader = await ReadFrameAsync().ConfigureAwait(false);
                if (frameHeader.Type != FrameType.Continuation ||
                    frameHeader.StreamId != streamId)
                {
                    throw new Http2ConnectionException(Http2ProtocolErrorCode.ProtocolError);
                }

                _hpackDecoder.Decode(
                    _incomingBuffer.ActiveSpan.Slice(0, frameHeader.Length),
                    frameHeader.EndHeadersFlag,
                    http2Stream);
                _incomingBuffer.Discard(frameHeader.Length);
            }

            _hpackDecoder.CompleteDecode();

            if (http2Stream != null)
            {
                http2Stream.OnHeadersComplete(endStream);
            }
        }

        private ReadOnlySpan<byte> GetFrameData(ReadOnlySpan<byte> frameData, bool hasPad, bool hasPriority)
        {
            if (hasPad)
            {
                if (frameData.Length == 0)
                {
                    throw new Http2ConnectionException(Http2ProtocolErrorCode.ProtocolError);
                }

                int padLength = frameData[0];
                frameData = frameData.Slice(1);

                if (frameData.Length < padLength)
                {
                    throw new Http2ConnectionException(Http2ProtocolErrorCode.ProtocolError);
                }

                frameData = frameData.Slice(0, frameData.Length - padLength);
            }

            if (hasPriority)
            {
                if (frameData.Length < FrameHeader.PriorityInfoLength)
                {
                    throw new Http2ConnectionException(Http2ProtocolErrorCode.ProtocolError);
                }

                // We ignore priority info.
                frameData = frameData.Slice(FrameHeader.PriorityInfoLength);
            }

            return frameData;
        }

        private void ProcessDataFrame(FrameHeader frameHeader)
        {
            Debug.Assert(frameHeader.Type == FrameType.Data);

            Http2Stream http2Stream = GetStream(frameHeader.StreamId);

            // Note, http2Stream will be null if this is a closed stream.
            // Just ignore the frame in this case.

            ReadOnlySpan<byte> frameData = GetFrameData(_incomingBuffer.ActiveSpan.Slice(0, frameHeader.Length), hasPad: frameHeader.PaddedFlag, hasPriority: false);

            if (http2Stream != null)
            {
                bool endStream = frameHeader.EndStreamFlag;

                http2Stream.OnResponseData(frameData, endStream);
            }

            if (frameData.Length > 0)
            {
                ExtendWindow(frameData.Length);
            }

            _incomingBuffer.Discard(frameHeader.Length);
        }

        private void ProcessSettingsFrame(FrameHeader frameHeader)
        {
            Debug.Assert(frameHeader.Type == FrameType.Settings);

            if (frameHeader.StreamId != 0)
            {
                throw new Http2ConnectionException(Http2ProtocolErrorCode.ProtocolError);
            }

            if (frameHeader.AckFlag)
            {
                if (frameHeader.Length != 0)
                {
                    throw new Http2ConnectionException(Http2ProtocolErrorCode.FrameSizeError);
                }

                if (!_expectingSettingsAck)
                {
                    throw new Http2ConnectionException(Http2ProtocolErrorCode.ProtocolError);
                }

                // We only send SETTINGS once initially, so we don't need to do anything in response to the ACK.
                // Just remember that we received one and we won't be expecting any more.
                _expectingSettingsAck = false;
            }
            else
            {
                if ((frameHeader.Length % 6) != 0)
                {
                    throw new Http2ConnectionException(Http2ProtocolErrorCode.FrameSizeError);
                }

                // Parse settings and process the ones we care about.
                ReadOnlySpan<byte> settings = _incomingBuffer.ActiveSpan.Slice(0, frameHeader.Length);
                while (settings.Length > 0)
                {
                    Debug.Assert((settings.Length % 6) == 0);

                    ushort settingId = BinaryPrimitives.ReadUInt16BigEndian(settings);
                    settings = settings.Slice(2);
                    uint settingValue = BinaryPrimitives.ReadUInt32BigEndian(settings);
                    settings = settings.Slice(4);

                    switch ((SettingId)settingId)
                    {
                        case SettingId.MaxConcurrentStreams:
                            ChangeMaxConcurrentStreams(settingValue);
                            break;

                        case SettingId.InitialWindowSize:
                            if (settingValue > 0x7FFFFFFF)
                            {
                                throw new Http2ConnectionException(Http2ProtocolErrorCode.FlowControlError);
                            }

                            ChangeInitialWindowSize((int)settingValue);
                            break;

                        case SettingId.MaxFrameSize:
                            if (settingValue < 16384 || settingValue > 16777215)
                            {
                                throw new Http2ConnectionException(Http2ProtocolErrorCode.ProtocolError);
                            }

                            // We don't actually store this value; we always send frames of the minimum size (16K).
                            break;

                        default:
                            // All others are ignored because we don't care about them.
                            // Note, per RFC, unknown settings IDs should be ignored.
                            break;
                    }
                }

                _incomingBuffer.Discard(frameHeader.Length);

                // Send acknowledgement
                // Don't wait for completion, which could happen asynchronously.
                LogExceptions(SendSettingsAckAsync());
            }
        }

        private void ChangeMaxConcurrentStreams(uint newValue)
        {
            if (NetEventSource.IsEnabled) Trace($"{nameof(newValue)}={newValue}");

            // The value is provided as a uint.
            // Limit this to int.MaxValue since the CreditManager implementation only supports singed values.
            // In practice, we should never reach this value.
            int effectiveValue = (newValue > (uint)int.MaxValue ? int.MaxValue : (int)newValue);
            int delta = effectiveValue - _maxConcurrentStreams;
            _maxConcurrentStreams = effectiveValue;

            _concurrentStreams.AdjustCredit(delta);
        }

        private void ChangeInitialWindowSize(int newSize)
        {
            if (NetEventSource.IsEnabled) Trace($"{nameof(newSize)}={newSize}");
            Debug.Assert(newSize >= 0);

            lock (SyncObject)
            {
                int delta = newSize - _initialWindowSize;
                _initialWindowSize = newSize;

                // Adjust existing streams
                foreach (KeyValuePair<int, Http2Stream> kvp in _httpStreams)
                {
                    kvp.Value.OnWindowUpdate(delta);
                }
            }
        }

        private void ProcessPriorityFrame(FrameHeader frameHeader)
        {
            Debug.Assert(frameHeader.Type == FrameType.Priority);

            if (frameHeader.StreamId == 0 || frameHeader.Length != FrameHeader.PriorityInfoLength)
            {
                throw new Http2ConnectionException(Http2ProtocolErrorCode.ProtocolError);
            }

            // Ignore priority info.

            _incomingBuffer.Discard(frameHeader.Length);
        }

        private void ProcessPingFrame(FrameHeader frameHeader)
        {
            Debug.Assert(frameHeader.Type == FrameType.Ping);

            if (frameHeader.StreamId != 0)
            {
                throw new Http2ConnectionException(Http2ProtocolErrorCode.ProtocolError);
            }

            if (frameHeader.AckFlag)
            {
                // We never send PING, so an ACK indicates a protocol error
                throw new Http2ConnectionException(Http2ProtocolErrorCode.ProtocolError);
            }

            if (frameHeader.Length != FrameHeader.PingLength)
            {
                throw new Http2ConnectionException(Http2ProtocolErrorCode.FrameSizeError);
            }

            // We don't wait for SendPingAckAsync to complete before discarding
            // the incoming buffer, so we need to take a copy of the data. Read
            // it as a big-endian integer here to avoid allocating an array.
            Debug.Assert(sizeof(long) == FrameHeader.PingLength);
            ReadOnlySpan<byte> pingContent = _incomingBuffer.ActiveSpan.Slice(0, FrameHeader.PingLength);
            long pingContentLong = BinaryPrimitives.ReadInt64BigEndian(pingContent);

            LogExceptions(SendPingAckAsync(pingContentLong));

            _incomingBuffer.Discard(frameHeader.Length);
        }

        private void ProcessWindowUpdateFrame(FrameHeader frameHeader)
        {
            Debug.Assert(frameHeader.Type == FrameType.WindowUpdate);

            if (frameHeader.Length != FrameHeader.WindowUpdateLength)
            {
                throw new Http2ConnectionException(Http2ProtocolErrorCode.FrameSizeError);
            }

            int amount = BinaryPrimitives.ReadInt32BigEndian(_incomingBuffer.ActiveSpan) & 0x7FFFFFFF;
            if (NetEventSource.IsEnabled) Trace($"{frameHeader}. {nameof(amount)}={amount}");

            Debug.Assert(amount >= 0);
            if (amount == 0)
            {
                throw new Http2ConnectionException(Http2ProtocolErrorCode.ProtocolError);
            }

            _incomingBuffer.Discard(frameHeader.Length);

            if (frameHeader.StreamId == 0)
            {
                _connectionWindow.AdjustCredit(amount);
            }
            else
            {
                Http2Stream http2Stream = GetStream(frameHeader.StreamId);
                if (http2Stream == null)
                {
                    // Ignore invalid stream ID, as per RFC
                    return;
                }

                http2Stream.OnWindowUpdate(amount);
            }
        }

        private void ProcessRstStreamFrame(FrameHeader frameHeader)
        {
            Debug.Assert(frameHeader.Type == FrameType.RstStream);

            if (frameHeader.Length != FrameHeader.RstStreamLength)
            {
                throw new Http2ConnectionException(Http2ProtocolErrorCode.FrameSizeError);
            }

            if (frameHeader.StreamId == 0)
            {
                throw new Http2ConnectionException(Http2ProtocolErrorCode.ProtocolError);
            }

            Http2Stream http2Stream = GetStream(frameHeader.StreamId);
            if (http2Stream == null)
            {
                // Ignore invalid stream ID, as per RFC
                _incomingBuffer.Discard(frameHeader.Length);
                return;
            }

            var protocolError = (Http2ProtocolErrorCode)BinaryPrimitives.ReadInt32BigEndian(_incomingBuffer.ActiveSpan);
            if (NetEventSource.IsEnabled) Trace(frameHeader.StreamId, $"{nameof(protocolError)}={protocolError}");

            _incomingBuffer.Discard(frameHeader.Length);

            if (protocolError == Http2ProtocolErrorCode.RefusedStream)
            {
                http2Stream.OnReset(new Http2StreamException(protocolError), resetStreamErrorCode: protocolError, canRetry: true);
            }
            else
            {
                http2Stream.OnReset(new Http2StreamException(protocolError), resetStreamErrorCode: protocolError);
            }
        }

        private void ProcessGoAwayFrame(FrameHeader frameHeader)
        {
            Debug.Assert(frameHeader.Type == FrameType.GoAway);

            if (frameHeader.Length < FrameHeader.GoAwayMinLength)
            {
                throw new Http2ConnectionException(Http2ProtocolErrorCode.FrameSizeError);
            }

            // GoAway frames always apply to the whole connection, never to a single stream.
            // According to RFC 7540 section 6.8, this should be a connection error.
            if (frameHeader.StreamId != 0)
            {
                throw new Http2ConnectionException(Http2ProtocolErrorCode.ProtocolError);
            }

            int lastValidStream = (int)(BinaryPrimitives.ReadUInt32BigEndian(_incomingBuffer.ActiveSpan) & 0x7FFFFFFF);
            var errorCode = (Http2ProtocolErrorCode)BinaryPrimitives.ReadInt32BigEndian(_incomingBuffer.ActiveSpan.Slice(sizeof(int)));
            if (NetEventSource.IsEnabled) Trace(frameHeader.StreamId, $"{nameof(lastValidStream)}={lastValidStream}, {nameof(errorCode)}={errorCode}");

            StartTerminatingConnection(lastValidStream, new Http2ConnectionException(errorCode));

            _incomingBuffer.Discard(frameHeader.Length);
        }

        internal async Task FlushAsync(CancellationToken cancellationToken = default)
        {
            await StartWriteAsync(0, cancellationToken).ConfigureAwait(false);
            FinishWrite(FlushTiming.Now);
        }

        private async ValueTask<Memory<byte>> StartWriteAsync(int writeBytes, CancellationToken cancellationToken = default)
        {
            if (NetEventSource.IsEnabled) Trace($"{nameof(writeBytes)}={writeBytes}");
            await AcquireWriteLockAsync(cancellationToken).ConfigureAwait(false);

            try
            {
                // If there is a pending write that was canceled while in progress, wait for it to complete.
                if (_inProgressWrite != null)
                {
                    await _inProgressWrite.ConfigureAwait(false);
                    _inProgressWrite = null;
                }

                int totalBufferLength = _outgoingBuffer.Capacity;
                int activeBufferLength = _outgoingBuffer.ActiveLength;

                if (totalBufferLength >= UnflushedOutgoingBufferSize &&
                    writeBytes >= totalBufferLength - activeBufferLength &&
                    activeBufferLength > 0)
                {
                    // If the buffer has already grown to 32k, does not have room for the next request,
                    // and is non-empty, flush the current contents to the wire.
                    await FlushOutgoingBytesAsync().ConfigureAwait(false); // we explicitly do not pass cancellationToken here, as this flush impacts more than just this operation
                }

                _outgoingBuffer.EnsureAvailableSpace(writeBytes);
                Memory<byte> writeBuffer = _outgoingBuffer.AvailableMemory.Slice(0, writeBytes);
                _currentWriteSize = writeBytes;

                return writeBuffer;
            }
            catch
            {
                _writerLock.Release();
                throw;
            }
        }

        /// <summary>Flushes buffered bytes to the wire.</summary>
        /// <param name="flush">When a flush should be performed for this write.</param>
        /// <remarks>
        /// Writes here need to be atomic, so as to avoid killing the whole connection.
        /// Callers must hold the write lock, which this will release.
        /// </remarks>
        private void FinishWrite(FlushTiming flush)
        {
            if (NetEventSource.IsEnabled) Trace($"{nameof(flush)}={flush}");

            // We can't validate that we hold the semaphore, but we can at least validate that someone is holding it.
            Debug.Assert(_writerLock.CurrentCount == 0);

            _outgoingBuffer.Commit(_currentWriteSize);
            _lastPendingWriterShouldFlush |= (flush == FlushTiming.AfterPendingWrites);
            EndWrite(forceFlush: (flush == FlushTiming.Now));
        }

        private void CancelWrite()
        {
            if (NetEventSource.IsEnabled) Trace("");

            // We can't validate that we hold the semaphore, but we can at least validate that someone is holding it.
            Debug.Assert(_writerLock.CurrentCount == 0);

            EndWrite(forceFlush: false);
        }

        private void EndWrite(bool forceFlush)
        {
            // We can't validate that we hold the semaphore, but we can at least validate that someone is holding it.
            Debug.Assert(_writerLock.CurrentCount == 0);

            try
            {
                // We must flush if the caller requires it or if this or a recent frame wanted to be flushed
                // once there were no more pending writers that themselves could have forced the flush.
                if (forceFlush || (_pendingWriters == 0 && _lastPendingWriterShouldFlush))
                {
                    Debug.Assert(_inProgressWrite == null);
                    if (_outgoingBuffer.ActiveLength > 0)
                    {
                        _inProgressWrite = FlushOutgoingBytesAsync();
                    }
                }
            }
            finally
            {
                _writerLock.Release();
            }
        }

        private async ValueTask AcquireWriteLockAsync(CancellationToken cancellationToken)
        {
            Task acquireLockTask = _writerLock.WaitAsync(cancellationToken);
            if (!acquireLockTask.IsCompletedSuccessfully)
            {
                Interlocked.Increment(ref _pendingWriters);

                try
                {
                    await acquireLockTask.ConfigureAwait(false);
                }
                catch
                {
                    if (Interlocked.Decrement(ref _pendingWriters) == 0)
                    {
                        // If a pending waiter is canceled, we may end up in a situation where a previously written frame
                        // saw that there were pending writers and as such deferred its flush to them, but if/when that pending
                        // writer is canceled, nothing may end up flushing the deferred work (at least not promptly).  To compensate,
                        // if a pending writer does end up being canceled, we flush asynchronously.  We can't check whether there's such
                        // a pending operation because we failed to acquire the lock that protects that state.  But we can at least only
                        // do the flush if our decrement caused the pending count to reach 0: if it's still higher than zero, then there's
                        // at least one other pending writer who can handle the flush.  Worst case, we pay for a flush that ends up being
                        // a nop.  Note: we explicitly do not pass in the cancellationToken; if we're here, it's almost certainly because
                        // cancellation was requested, and it's because of that cancellation that we need to flush.
                        LogExceptions(FlushAsync());
                    }

                    throw;
                }

                Interlocked.Decrement(ref _pendingWriters);
            }

            // If the connection has been aborted, then fail now instead of trying to send more data.
            if (_abortException != null)
            {
                _writerLock.Release();
                throw new IOException(SR.net_http_request_aborted, _abortException);
            }
        }

        private async Task SendSettingsAckAsync()
        {
            Memory<byte> writeBuffer = await StartWriteAsync(FrameHeader.Size).ConfigureAwait(false);
            if (NetEventSource.IsEnabled) Trace("Started writing.");

            FrameHeader frameHeader = new FrameHeader(0, FrameType.Settings, FrameFlags.Ack, 0);
            frameHeader.WriteTo(writeBuffer);

            FinishWrite(FlushTiming.AfterPendingWrites);
        }

        /// <param name="pingContent">The 8-byte ping content to send, read as a big-endian integer.</param>
        private async Task SendPingAckAsync(long pingContent)
        {
            Memory<byte> writeBuffer = await StartWriteAsync(FrameHeader.Size + FrameHeader.PingLength).ConfigureAwait(false);
            if (NetEventSource.IsEnabled) Trace("Started writing.");

            FrameHeader frameHeader = new FrameHeader(FrameHeader.PingLength, FrameType.Ping, FrameFlags.Ack, 0);
            frameHeader.WriteTo(writeBuffer);
            writeBuffer = writeBuffer.Slice(FrameHeader.Size);

            Debug.Assert(sizeof(long) == FrameHeader.PingLength);
            BinaryPrimitives.WriteInt64BigEndian(writeBuffer.Span, pingContent);

            FinishWrite(FlushTiming.AfterPendingWrites);
        }

        private async Task SendRstStreamAsync(int streamId, Http2ProtocolErrorCode errorCode)
        {
            Memory<byte> writeBuffer = await StartWriteAsync(FrameHeader.Size + FrameHeader.RstStreamLength).ConfigureAwait(false);
            if (NetEventSource.IsEnabled) Trace(streamId, $"Started writing. {nameof(errorCode)}={errorCode}");

            FrameHeader frameHeader = new FrameHeader(FrameHeader.RstStreamLength, FrameType.RstStream, FrameFlags.None, streamId);
            frameHeader.WriteTo(writeBuffer);
            writeBuffer = writeBuffer.Slice(FrameHeader.Size);

            BinaryPrimitives.WriteInt32BigEndian(writeBuffer.Span, (int)errorCode);

            FinishWrite(FlushTiming.Now); // ensure cancellation is seen as soon as possible
        }

        private static (ReadOnlyMemory<byte> first, ReadOnlyMemory<byte> rest) SplitBuffer(ReadOnlyMemory<byte> buffer, int maxSize) =>
            buffer.Length > maxSize ?
                (buffer.Slice(0, maxSize), buffer.Slice(maxSize)) :
                (buffer, Memory<byte>.Empty);

        private void WriteIndexedHeader(int index)
        {
            if (NetEventSource.IsEnabled) Trace($"{nameof(index)}={index}");

            int bytesWritten;
            while (!HPackEncoder.EncodeIndexedHeaderField(index, _headerBuffer.AvailableSpan, out bytesWritten))
            {
                _headerBuffer.EnsureAvailableSpace(_headerBuffer.AvailableLength + 1);
            }

            _headerBuffer.Commit(bytesWritten);
        }

        private void WriteIndexedHeader(int index, string value)
        {
            if (NetEventSource.IsEnabled) Trace($"{nameof(index)}={index}, {nameof(value)}={value}");

            int bytesWritten;
            while (!HPackEncoder.EncodeLiteralHeaderFieldWithoutIndexing(index, value, _headerBuffer.AvailableSpan, out bytesWritten))
            {
                _headerBuffer.EnsureAvailableSpace(_headerBuffer.AvailableLength + 1);
            }

            _headerBuffer.Commit(bytesWritten);
        }

        private void WriteLiteralHeader(string name, ReadOnlySpan<string> values)
        {
            if (NetEventSource.IsEnabled) Trace($"{nameof(name)}={name}, {nameof(values)}={string.Join(", ", values.ToArray())}");

            int bytesWritten;
            while (!HPackEncoder.EncodeLiteralHeaderFieldWithoutIndexingNewName(name, values, HttpHeaderParser.DefaultSeparator, _headerBuffer.AvailableSpan, out bytesWritten))
            {
                _headerBuffer.EnsureAvailableSpace(_headerBuffer.AvailableLength + 1);
            }

            _headerBuffer.Commit(bytesWritten);
        }

        private void WriteLiteralHeaderValues(ReadOnlySpan<string> values, string separator)
        {
            if (NetEventSource.IsEnabled) Trace($"{nameof(values)}={string.Join(separator, values.ToArray())}");

            int bytesWritten;
            while (!HPackEncoder.EncodeStringLiterals(values, separator, _headerBuffer.AvailableSpan, out bytesWritten))
            {
                _headerBuffer.EnsureAvailableSpace(_headerBuffer.AvailableLength + 1);
            }

            _headerBuffer.Commit(bytesWritten);
        }

        private void WriteLiteralHeaderValue(string value)
        {
            if (NetEventSource.IsEnabled) Trace($"{nameof(value)}={value}");

            int bytesWritten;
            while (!HPackEncoder.EncodeStringLiteral(value, _headerBuffer.AvailableSpan, out bytesWritten))
            {
                _headerBuffer.EnsureAvailableSpace(_headerBuffer.AvailableLength + 1);
            }

            _headerBuffer.Commit(bytesWritten);
        }

        private void WriteBytes(ReadOnlySpan<byte> bytes)
        {
            if (NetEventSource.IsEnabled) Trace($"{nameof(bytes.Length)}={bytes.Length}");

            if (bytes.Length > _headerBuffer.AvailableLength)
            {
                _headerBuffer.EnsureAvailableSpace(bytes.Length);
            }

            bytes.CopyTo(_headerBuffer.AvailableSpan);
            _headerBuffer.Commit(bytes.Length);
        }

        private void WriteHeaderCollection(HttpHeaders headers)
        {
            if (NetEventSource.IsEnabled) Trace("");

            if (headers.HeaderStore is null)
            {
                return;
            }

            foreach (KeyValuePair<HeaderDescriptor, HttpHeaders.HeaderStoreItemInfo> header in headers.HeaderStore)
            {
                int headerValuesCount = HttpHeaders.GetValuesAsStrings(header.Key, header.Value, ref _headerValues);
                Debug.Assert(headerValuesCount > 0, "No values for header??");
                ReadOnlySpan<string> headerValues = _headerValues.AsSpan(0, headerValuesCount);

                KnownHeader knownHeader = header.Key.KnownHeader;
                if (knownHeader != null)
                {
                    // The Host header is not sent for HTTP2 because we send the ":authority" pseudo-header instead
                    // (see pseudo-header handling below in WriteHeaders).
                    // The Connection, Upgrade and ProxyConnection headers are also not supported in HTTP2.
                    if (knownHeader != KnownHeaders.Host && knownHeader != KnownHeaders.Connection && knownHeader != KnownHeaders.Upgrade && knownHeader != KnownHeaders.ProxyConnection)
                    {
                        if (header.Key.KnownHeader == KnownHeaders.TE)
                        {
                            // HTTP/2 allows only 'trailers' TE header. rfc7540 8.1.2.2
                            foreach (string value in headerValues)
                            {
                                if (string.Equals(value, "trailers", StringComparison.OrdinalIgnoreCase))
                                {
                                    WriteBytes(knownHeader.Http2EncodedName);
                                    WriteLiteralHeaderValue(value);
                                    break;
                                }
                            }
                            continue;
                        }

                        // For all other known headers, send them via their pre-encoded name and the associated value.
                        WriteBytes(knownHeader.Http2EncodedName);
                        string separator = null;
                        if (headerValues.Length > 1)
                        {
                            HttpHeaderParser parser = header.Key.Parser;
                            if (parser != null && parser.SupportsMultipleValues)
                            {
                                separator = parser.Separator;
                            }
                            else
                            {
                                separator = HttpHeaderParser.DefaultSeparator;
                            }
                        }

                        WriteLiteralHeaderValues(headerValues, separator);
                    }
                }
                else
                {
                    // The header is not known: fall back to just encoding the header name and value(s).
                    WriteLiteralHeader(header.Key.Name, headerValues);
                }
            }
        }

        private void WriteHeaders(HttpRequestMessage request)
        {
            if (NetEventSource.IsEnabled) Trace("");
            Debug.Assert(_headerBuffer.ActiveLength == 0);

            // HTTP2 does not support Transfer-Encoding: chunked, so disable this on the request.
            request.Headers.TransferEncodingChunked = false;

            HttpMethod normalizedMethod = HttpMethod.Normalize(request.Method);

            // Method is normalized so we can do reference equality here.
            if (ReferenceEquals(normalizedMethod, HttpMethod.Get))
            {
                WriteIndexedHeader(StaticTable.MethodGet);
            }
            else if (ReferenceEquals(normalizedMethod, HttpMethod.Post))
            {
                WriteIndexedHeader(StaticTable.MethodPost);
            }
            else
            {
                WriteIndexedHeader(StaticTable.MethodGet, normalizedMethod.Method);
            }

            WriteIndexedHeader(_stream is SslStream ? StaticTable.SchemeHttps : StaticTable.SchemeHttp);

            if (request.HasHeaders && request.Headers.Host != null)
            {
                WriteIndexedHeader(StaticTable.Authority, request.Headers.Host);
            }
            else
            {
                WriteBytes(_pool._encodedAuthorityHostHeader);
            }

            string pathAndQuery = request.RequestUri.PathAndQuery;
            if (pathAndQuery == "/")
            {
                WriteIndexedHeader(StaticTable.PathSlash);
            }
            else
            {
                WriteIndexedHeader(StaticTable.PathSlash, pathAndQuery);
            }

            if (request.HasHeaders)
            {
                WriteHeaderCollection(request.Headers);
            }

            // Determine cookies to send.
            if (_pool.Settings._useCookies)
            {
                string cookiesFromContainer = _pool.Settings._cookieContainer.GetCookieHeader(request.RequestUri);
                if (cookiesFromContainer != string.Empty)
                {
                    WriteBytes(KnownHeaders.Cookie.Http2EncodedName);
                    WriteLiteralHeaderValue(cookiesFromContainer);
                }
            }

            if (request.Content == null)
            {
                // Write out Content-Length: 0 header to indicate no body,
                // unless this is a method that never has a body.
                if (normalizedMethod.MustHaveRequestBody)
                {
                    WriteBytes(KnownHeaders.ContentLength.Http2EncodedName);
                    WriteLiteralHeaderValue("0");
                }
            }
            else
            {
                WriteHeaderCollection(request.Content.Headers);
            }
        }

        private HttpRequestException GetShutdownException()
        {
            Debug.Assert(Monitor.IsEntered(SyncObject));

            // Throw a retryable exception that will allow this unprocessed request to be processed on a new connection.
            // In rare cases, such as receiving GOAWAY immediately after connection establishment, we will not
            // actually retry the request, so we must give a useful exception here for these cases.

            Exception innerException;
            if (_abortException != null)
            {
                innerException = _abortException;
            }
            else if (_lastStreamId != -1)
            {
                // We must have received a GOAWAY.
                innerException = new IOException(SR.net_http_server_shutdown);
            }
            else
            {
                // We must either be disposed or out of stream IDs.
                // Note that in this case, the exception should never be visible to the user (it should be retried).
                Debug.Assert(_disposed || _nextStream == MaxStreamId);

                innerException = new ObjectDisposedException(nameof(Http2Connection));
            }

            return new HttpRequestException(SR.net_http_client_execution_error, innerException, allowRetry: RequestRetryType.RetryOnSameOrNextProxy);
        }

        private async ValueTask<Http2Stream> SendHeadersAsync(HttpRequestMessage request, CancellationToken cancellationToken, bool mustFlush)
        {
            // We serialize usage of the header encoder and the header buffer.
            // This also ensures that new streams are always created in ascending order.
            await _headerSerializationLock.WaitAsync(cancellationToken).ConfigureAwait(false);
            try
            {
                // Generate the entire header block, without framing, into the connection header buffer.
                WriteHeaders(request);

                try
                {
                    // Enforce MAX_CONCURRENT_STREAMS setting value.
                    await _concurrentStreams.RequestCreditAsync(1, cancellationToken).ConfigureAwait(false);
                }
                catch (ObjectDisposedException)
                {
                    // We have race condition between shutting down and initiating new requests.
                    // When we are shutting down the connection (e.g. due to receiving GOAWAY, etc)
                    // we will wait until the stream count goes to 0, and then we will close the connetion
                    // and perform clean up, including disposing _concurrentStreams.
                    // So if we get ObjectDisposedException here, we must have shut down the connection.
                    // Throw a retryable request exception if this is not result of some other error.
                    // This will cause retry logic to kick in and perform another connection attempt.
                    // The user should never see this exception.  See similar handling below.
                    // Throw a retryable request exception if this is not result of some other error.
                    // This will cause retry logic to kick in and perform another connection attempt.
                    // The user should never see this exception.  See also below.
                    Debug.Assert(_disposed || _lastStreamId != -1);
                    Debug.Assert(_httpStreams.Count == 0);

                    lock (SyncObject)
                    {
                        throw GetShutdownException();
                    }
                }

                try
                {
                    // Allocate the next available stream ID.
                    // Note that if we fail before sending the headers, we'll just skip this stream ID, which is fine.
                    int streamId;
                    lock (SyncObject)
                    {
                        if (_nextStream == MaxStreamId || _disposed || _lastStreamId != -1)
                        {
                            // We ran out of stream IDs or we raced between acquiring the connection from the pool and shutting down.
                            // Throw a retryable request exception. This will cause retry logic to kick in
                            // and perform another connection attempt. The user should never see this exception.
                            throw GetShutdownException();
                        }

                        streamId = _nextStream;

                        // Client-initiated streams are always odd-numbered, so increase by 2.
                        _nextStream += 2;
                    }

                    ReadOnlyMemory<byte> remaining = _headerBuffer.ActiveMemory;
                    Debug.Assert(remaining.Length > 0);

                    // Calculate the total number of bytes we're going to use (content + headers).
                    int frameCount = ((remaining.Length - 1) / FrameHeader.MaxLength) + 1;
                    int totalSize = remaining.Length + frameCount * FrameHeader.Size;

                    // Note, HEADERS and CONTINUATION frames must be together, so hold the writer lock across sending all of them.
                    Memory<byte> writeBuffer = await StartWriteAsync(totalSize, cancellationToken).ConfigureAwait(false);
                    if (NetEventSource.IsEnabled) Trace(streamId, $"Started writing. {nameof(totalSize)}={totalSize}");

                    // Send the HEADERS frame.
                    ReadOnlyMemory<byte> current;
                    (current, remaining) = SplitBuffer(remaining, FrameHeader.MaxLength);

                    FrameFlags flags =
                        (remaining.Length == 0 ? FrameFlags.EndHeaders : FrameFlags.None) |
                        (request.Content == null ? FrameFlags.EndStream : FrameFlags.None);

                    FrameHeader frameHeader = new FrameHeader(current.Length, FrameType.Headers, flags, streamId);
                    frameHeader.WriteTo(writeBuffer.Span);
                    writeBuffer = writeBuffer.Slice(FrameHeader.Size);

                    current.CopyTo(writeBuffer);
                    writeBuffer = writeBuffer.Slice(current.Length);

                    if (NetEventSource.IsEnabled) Trace(streamId, $"Wrote HEADERS frame. Length={current.Length}, flags={flags}");

                    // Send CONTINUATION frames, if any.
                    while (remaining.Length > 0)
                    {
                        (current, remaining) = SplitBuffer(remaining, FrameHeader.MaxLength);

                        flags = (remaining.Length == 0 ? FrameFlags.EndHeaders : FrameFlags.None);

                        frameHeader = new FrameHeader(current.Length, FrameType.Continuation, flags, streamId);
                        frameHeader.WriteTo(writeBuffer.Span);
                        writeBuffer = writeBuffer.Slice(FrameHeader.Size);

                        current.CopyTo(writeBuffer);
                        writeBuffer = writeBuffer.Slice(current.Length);

                        if (NetEventSource.IsEnabled) Trace(streamId, $"Wrote CONTINUATION frame. Length={current.Length}, flags={flags}");
                    }

                    Debug.Assert(writeBuffer.Length == 0);

                    Http2Stream http2Stream;
                    try
                    {
                        // We're about to write the HEADERS frame, so add the stream to the dictionary now.
                        // The lifetime of the stream is now controlled by the stream itself and the connection.
                        // This can fail if the connection is shutting down, in which case we will cancel sending this frame.
                        http2Stream = AddStream(streamId, request);
                    }
                    catch
                    {
                        CancelWrite();
                        throw;
                    }

                    FinishWrite(mustFlush || (flags & FrameFlags.EndStream) != 0 ? FlushTiming.AfterPendingWrites : FlushTiming.Eventually);

                    return http2Stream;
                }
                catch
                {
                    _concurrentStreams.AdjustCredit(1);
                    throw;
                }
            }
            finally
            {
                _headerBuffer.Discard(_headerBuffer.ActiveLength);
                _headerSerializationLock.Release();
            }
        }

        private async Task SendStreamDataAsync(int streamId, ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken)
        {
            ReadOnlyMemory<byte> remaining = buffer;

            while (remaining.Length > 0)
            {
                int frameSize = Math.Min(remaining.Length, FrameHeader.MaxLength);

                // Once credit had been granted, we want to actually consume those bytes.
                frameSize = await _connectionWindow.RequestCreditAsync(frameSize, cancellationToken).ConfigureAwait(false);

                ReadOnlyMemory<byte> current;
                (current, remaining) = SplitBuffer(remaining, frameSize);

                // It's possible that a cancellation will occur while we wait for the write lock. In that case, we need to
                // return the credit that we have acquired and don't plan to use.
                Memory<byte> writeBuffer;
                try
                {
                    writeBuffer = await StartWriteAsync(FrameHeader.Size + current.Length, cancellationToken).ConfigureAwait(false);
                    if (NetEventSource.IsEnabled) Trace(streamId, $"Started writing. {nameof(writeBuffer.Length)}={writeBuffer.Length}");
                }
                catch (OperationCanceledException)
                {
                    _connectionWindow.AdjustCredit(frameSize);
                    throw;
                }

                FrameHeader frameHeader = new FrameHeader(current.Length, FrameType.Data, FrameFlags.None, streamId);
                frameHeader.WriteTo(writeBuffer);
                writeBuffer = writeBuffer.Slice(FrameHeader.Size);

                current.CopyTo(writeBuffer);
                writeBuffer = writeBuffer.Slice(current.Length);

                Debug.Assert(writeBuffer.Length == 0);

                FinishWrite(FlushTiming.Eventually); // no need to flush, as the request content may do so explicitly, or worst case we'll do so as part of the end data frame
            }
        }

        private async Task SendEndStreamAsync(int streamId)
        {
            Memory<byte> writeBuffer = await StartWriteAsync(FrameHeader.Size).ConfigureAwait(false);
            if (NetEventSource.IsEnabled) Trace(streamId, "Started writing.");

            FrameHeader frameHeader = new FrameHeader(0, FrameType.Data, FrameFlags.EndStream, streamId);
            frameHeader.WriteTo(writeBuffer);

            FinishWrite(FlushTiming.AfterPendingWrites); // finished sending request body, so flush soon (but ok to wait for pending packets)
        }

        private async Task SendWindowUpdateAsync(int streamId, int amount)
        {
            Debug.Assert(amount > 0);

            // We update both the connection-level and stream-level windows at the same time
            Memory<byte> writeBuffer = await StartWriteAsync(FrameHeader.Size + FrameHeader.WindowUpdateLength).ConfigureAwait(false);
            if (NetEventSource.IsEnabled) Trace(streamId, $"Started writing. {nameof(amount)}={amount}");

            FrameHeader frameHeader = new FrameHeader(FrameHeader.WindowUpdateLength, FrameType.WindowUpdate, FrameFlags.None, streamId);
            frameHeader.WriteTo(writeBuffer);
            writeBuffer = writeBuffer.Slice(FrameHeader.Size);

            BinaryPrimitives.WriteInt32BigEndian(writeBuffer.Span, amount);

            FinishWrite(FlushTiming.Now); // make sure window updates are seen as soon as possible
        }

        private void ExtendWindow(int amount)
        {
            if (NetEventSource.IsEnabled) Trace($"{nameof(amount)}={amount}");
            Debug.Assert(amount > 0);

            int windowUpdateSize;
            lock (SyncObject)
            {
                Debug.Assert(_pendingWindowUpdate < ConnectionWindowThreshold);

                _pendingWindowUpdate += amount;
                if (_pendingWindowUpdate < ConnectionWindowThreshold)
                {
                    if (NetEventSource.IsEnabled) Trace($"{nameof(_pendingWindowUpdate)} {_pendingWindowUpdate} < {ConnectionWindowThreshold}.");
                    return;
                }

                windowUpdateSize = _pendingWindowUpdate;
                _pendingWindowUpdate = 0;
            }

            LogExceptions(SendWindowUpdateAsync(0, windowUpdateSize));
        }

        private void WriteFrameHeader(FrameHeader frameHeader)
        {
            if (NetEventSource.IsEnabled) Trace($"{frameHeader}");
            Debug.Assert(_outgoingBuffer.AvailableMemory.Length >= FrameHeader.Size);

            frameHeader.WriteTo(_outgoingBuffer.AvailableSpan);
            _outgoingBuffer.Commit(FrameHeader.Size);
        }

        /// <summary>Abort all streams and cause further processing to fail.</summary>
        /// <param name="abortException">Exception causing Abort to be called.</param>
        private void Abort(Exception abortException)
        {
            // The connection has failed, e.g. failed IO or a connection-level frame error.
            if (Interlocked.CompareExchange(ref _abortException, abortException, null) != null &&
                NetEventSource.IsEnabled &&
                !ReferenceEquals(_abortException, abortException))
            {
                // Lost the race to set the field to another exception, so just trace this one.
                Trace($"{nameof(abortException)}=={abortException}");
            }

            AbortStreams(_abortException);
        }

        /// <summary>Gets whether the connection exceeded any of the connection limits.</summary>
        /// <param name="nowTicks">The current tick count.  Passed in to amortize the cost of calling Environment.TickCount64.</param>
        /// <param name="connectionLifetime">How long a connection can be open to be considered reusable.</param>
        /// <param name="connectionIdleTimeout">How long a connection can have been idle in the pool to be considered reusable.</param>
        /// <returns>
        /// true if we believe the connection is expired; otherwise, false.  There is an inherent race condition here,
        /// in that the server could terminate the connection or otherwise make it unusable immediately after we check it,
        /// but there's not much difference between that and starting to use the connection and then having the server
        /// terminate it, which would be considered a failure, so this race condition is largely benign and inherent to
        /// the nature of connection pooling.
        /// </returns>

        public bool IsExpired(long nowTicks,
                              TimeSpan connectionLifetime,
                              TimeSpan connectionIdleTimeout)

        {
            if (_disposed)
            {
                return true;
            }

            // Check idle timeout when there are not pending requests for a while.
            if ((connectionIdleTimeout != Timeout.InfiniteTimeSpan) &&
                (_httpStreams.Count == 0) &&
                ((nowTicks - _idleSinceTickCount) > connectionIdleTimeout.TotalMilliseconds))
            {
                if (NetEventSource.IsEnabled) Trace($"Connection no longer usable. Idle {TimeSpan.FromMilliseconds((nowTicks - _idleSinceTickCount))} > {connectionIdleTimeout}.");

                return true;
            }

            return LifetimeExpired(nowTicks, connectionLifetime);
        }

        private void AbortStreams(Exception abortException)
        {
            if (NetEventSource.IsEnabled) Trace($"{nameof(abortException)}={abortException}");

            // Invalidate outside of lock to avoid race with HttpPool Dispose()
            // We should not try to grab pool lock while holding connection lock as on disposing pool,
            // we could hold pool lock while trying to grab connection lock in Dispose().
            _pool.InvalidateHttp2Connection(this);

            List<Http2Stream> streamsToAbort = new List<Http2Stream>();

            lock (SyncObject)
            {
                // Set _lastStreamId to int.MaxValue to indicate that we are shutting down
                // and we must assume all active streams have been processed by the server
                _lastStreamId = int.MaxValue;

                foreach (KeyValuePair<int, Http2Stream> kvp in _httpStreams)
                {
                    int streamId = kvp.Key;
                    Debug.Assert(streamId == kvp.Value.StreamId);

                    streamsToAbort.Add(kvp.Value);
                }

                CheckForShutdown();
            }

            // Avoid calling OnAbort under the lock, as it may cause the Http2Stream
            // to call back in to RemoveStream
            foreach (Http2Stream s in streamsToAbort)
            {
                s.OnReset(abortException);
            }
        }

        private void StartTerminatingConnection(int lastValidStream, Exception abortException)
        {
            Debug.Assert(lastValidStream >= 0);

            // Invalidate outside of lock to avoid race with HttpPool Dispose()
            // We should not try to grab pool lock while holding connection lock as on disposing pool,
            // we could hold pool lock while trying to grab connection lock in Dispose().
            _pool.InvalidateHttp2Connection(this);

            List<Http2Stream> streamsToAbort = new List<Http2Stream>();

            lock (SyncObject)
            {
                if (_lastStreamId == -1)
                {
                    _lastStreamId = lastValidStream;
                }
                else
                {
                    // We have already received GOAWAY before
                    // In this case the smaller valid stream is used
                    _lastStreamId = Math.Min(_lastStreamId, lastValidStream);
                }

                if (NetEventSource.IsEnabled) Trace($"{nameof(lastValidStream)}={lastValidStream}, {nameof(_lastStreamId)}={_lastStreamId}");

                foreach (KeyValuePair<int, Http2Stream> kvp in _httpStreams)
                {
                    int streamId = kvp.Key;
                    Debug.Assert(streamId == kvp.Value.StreamId);

                    if (streamId > _lastStreamId)
                    {
                        streamsToAbort.Add(kvp.Value);
                    }
                    else
                    {
                        if (NetEventSource.IsEnabled) Trace($"Found {nameof(streamId)} {streamId} <= {_lastStreamId}.");
                    }
                }

                CheckForShutdown();
            }

            // Avoid calling OnAbort under the lock, as it may cause the Http2Stream
            // to call back in to RemoveStream
            foreach (Http2Stream s in streamsToAbort)
            {
                s.OnReset(abortException, canRetry: true);
            }
        }

        private void CheckForShutdown()
        {
            Debug.Assert(_disposed || _lastStreamId != -1);
            Debug.Assert(Monitor.IsEntered(SyncObject));

            // Check if dictionary has become empty
            if (_httpStreams.Count != 0)
            {
                return;
            }

            // Do shutdown.
            _stream.Close();

            _connectionWindow.Dispose();
            _concurrentStreams.Dispose();
        }

        public void Dispose()
        {
            lock (SyncObject)
            {
                if (!_disposed)
                {
                    _disposed = true;

                    CheckForShutdown();
                }
            }
        }

        private enum FrameType : byte
        {
            Data = 0,
            Headers = 1,
            Priority = 2,
            RstStream = 3,
            Settings = 4,
            PushPromise = 5,
            Ping = 6,
            GoAway = 7,
            WindowUpdate = 8,
            Continuation = 9,

            Last = 9
        }

        private struct FrameHeader
        {
            public int Length;
            public FrameType Type;
            public FrameFlags Flags;
            public int StreamId;

            public const int Size = 9;
            public const int MaxLength = 16384;

            public const int SettingLength = 6;            // per setting (total SETTINGS length must be a multiple of this)
            public const int PriorityInfoLength = 5;       // for both PRIORITY frame and priority info within HEADERS
            public const int PingLength = 8;
            public const int WindowUpdateLength = 4;
            public const int RstStreamLength = 4;
            public const int GoAwayMinLength = 8;

            public FrameHeader(int length, FrameType type, FrameFlags flags, int streamId)
            {
                Debug.Assert(streamId >= 0);

                Length = length;
                Type = type;
                Flags = flags;
                StreamId = streamId;
            }

            public bool PaddedFlag => (Flags & FrameFlags.Padded) != 0;
            public bool AckFlag => (Flags & FrameFlags.Ack) != 0;
            public bool EndHeadersFlag => (Flags & FrameFlags.EndHeaders) != 0;
            public bool EndStreamFlag => (Flags & FrameFlags.EndStream) != 0;
            public bool PriorityFlag => (Flags & FrameFlags.Priority) != 0;

            public static FrameHeader ReadFrom(ReadOnlySpan<byte> buffer)
            {
                Debug.Assert(buffer.Length >= Size);

                return new FrameHeader(
                    (buffer[0] << 16) | (buffer[1] << 8) | buffer[2],
                    (FrameType)buffer[3],
                    (FrameFlags)buffer[4],
                    (int)((uint)((buffer[5] << 24) | (buffer[6] << 16) | (buffer[7] << 8) | buffer[8]) & 0x7FFFFFFF));
            }

            public void WriteTo(Span<byte> buffer)
            {
                Debug.Assert(buffer.Length >= Size);
                Debug.Assert(Type <= FrameType.Last);
                Debug.Assert((Flags & FrameFlags.ValidBits) == Flags);
                Debug.Assert(Length <= MaxLength);

                buffer[0] = (byte)((Length & 0x00FF0000) >> 16);
                buffer[1] = (byte)((Length & 0x0000FF00) >> 8);
                buffer[2] = (byte)(Length & 0x000000FF);

                buffer[3] = (byte)Type;
                buffer[4] = (byte)Flags;

                buffer[5] = (byte)((StreamId & 0xFF000000) >> 24);
                buffer[6] = (byte)((StreamId & 0x00FF0000) >> 16);
                buffer[7] = (byte)((StreamId & 0x0000FF00) >> 8);
                buffer[8] = (byte)(StreamId & 0x000000FF);
            }

            public void WriteTo(Memory<byte> buffer) => WriteTo(buffer.Span);

            public override string ToString() => $"StreamId={StreamId}; Type={Type}; Flags={Flags}; Length={Length}"; // Description for diagnostic purposes
        }

        [Flags]
        private enum FrameFlags : byte
        {
            None = 0,

            // Some frame types define bits differently.  Define them all here for simplicity.

            EndStream =     0b00000001,
            Ack =           0b00000001,
            EndHeaders =    0b00000100,
            Padded =        0b00001000,
            Priority =      0b00100000,

            ValidBits =     0b00101101
        }

        private enum SettingId : ushort
        {
            HeaderTableSize = 0x1,
            EnablePush = 0x2,
            MaxConcurrentStreams = 0x3,
            InitialWindowSize = 0x4,
            MaxFrameSize = 0x5,
            MaxHeaderListSize = 0x6
        }

        /// <summary>Specifies when the data written needs to be flushed.</summary>
        private enum FlushTiming
        {
            /// <summary>No specific requirement.  This can be used when the caller knows another operation will soon force a flush.</summary>
            Eventually,
            /// <summary>The data needs to be flushed soon, but it's ok to wait briefly for pending writes to further populate the buffer.</summary>
            AfterPendingWrites,
            /// <summary>The data must be flushed immediately.</summary>
            Now
        }

        // Note that this is safe to be called concurrently by multiple threads.

        public sealed override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (NetEventSource.IsEnabled) Trace($"{request}");

            try
            {
                // Send request headers
                bool shouldExpectContinue = request.Content != null && request.HasHeaders && request.Headers.ExpectContinue == true;
                Http2Stream http2Stream = await SendHeadersAsync(request, cancellationToken, mustFlush: shouldExpectContinue).ConfigureAwait(false);

                bool duplex = request.Content != null && request.Content.AllowDuplex;

                // If we have duplex content, then don't propagate the cancellation to the request body task.
                // If cancellation occurs before we receive the response headers, then we will cancel the request body anyway.
                CancellationToken requestBodyCancellationToken = duplex ? CancellationToken.None : cancellationToken;

                // Start sending request body, if any.
                Task requestBodyTask = http2Stream.SendRequestBodyAsync(requestBodyCancellationToken);

                // Start receiving the response headers.
                Task responseHeadersTask = http2Stream.ReadResponseHeadersAsync(cancellationToken);

                // Wait for either task to complete.  The best and most common case is when the request body completes
                // before the response headers, in which case we can fully process the sending of the request and then
                // fully process the sending of the response.  WhenAny is not free, so we do a fast-path check to see
                // if the request body completed synchronously, only progressing to do the WhenAny if it didn't. Then
                // if the WhenAny completes and either the WhenAny indicated that the request body completed or
                // both tasks completed, we can proceed to handle the request body as if it completed first.  We also
                // check whether the request content even allows for duplex communication; if it doesn't (none of
                // our built-in content types do), then we can just proceed to wait for the request body content to
                // complete before worrying about response headers completing.
                if (requestBodyTask.IsCompleted ||
                    duplex == false ||
                    requestBodyTask == await Task.WhenAny(requestBodyTask, responseHeadersTask).ConfigureAwait(false) ||
                    requestBodyTask.IsCompleted)
                {
                    // The sending of the request body completed before receiving all of the request headers (or we're
                    // ok waiting for the request body even if it hasn't completed, e.g. because we're not doing duplex).
                    // This is the common and desirable case.
                    try
                    {
                        await requestBodyTask.ConfigureAwait(false);
                    }
                    catch (Exception e)
                    {
                        if (NetEventSource.IsEnabled) Trace($"Sending request content failed: {e}");
                        LogExceptions(responseHeadersTask); // Observe exception (if any) on responseHeadersTask.
                        throw;
                    }
                }
                else
                {
                    // We received the response headers but the request body hasn't yet finished; this most commonly happens
                    // when the protocol is being used to enable duplex communication. If the connection is aborted or if we
                    // get RST or GOAWAY from server, exception will be stored in stream._abortException and propagated up
                    // to caller if possible while processing response, but make sure that we log any exceptions from this task
                    // completing asynchronously).
                    LogExceptions(requestBodyTask);
                }

                // Wait for the response headers to complete if they haven't already, propagating any exceptions.
                await responseHeadersTask.ConfigureAwait(false);

                return http2Stream.GetAndClearResponse();
            }
            catch (Exception e)
            {
                if (e is IOException ||
                    e is ObjectDisposedException ||
                    e is Http2ProtocolException ||
                    e is InvalidOperationException)
                {
                    throw new HttpRequestException(SR.net_http_client_execution_error, e);
                }

                throw;
            }
        }

        private Http2Stream AddStream(int streamId, HttpRequestMessage request)
        {
            lock (SyncObject)
            {
                if (_disposed || _lastStreamId != -1)
                {
                    // The connection is shutting down.
                    // Throw a retryable request exception. This will cause retry logic to kick in
                    // and perform another connection attempt. The user should never see this exception.
                    throw GetShutdownException();
                }

                Http2Stream http2Stream = new Http2Stream(request, this, streamId, _initialWindowSize);

                _httpStreams.Add(streamId, http2Stream);

                return http2Stream;
            }
        }

        private void RemoveStream(Http2Stream http2Stream)
        {
            if (NetEventSource.IsEnabled) Trace(http2Stream.StreamId, "");
            Debug.Assert(http2Stream != null);

            lock (SyncObject)
            {
                if (!_httpStreams.Remove(http2Stream.StreamId, out Http2Stream removed))
                {
                    Debug.Fail($"Stream {http2Stream.StreamId} not found in dictionary during RemoveStream???");
                    return;
                }

                _concurrentStreams.AdjustCredit(1);

                Debug.Assert(removed == http2Stream, "_httpStreams.TryRemove returned unexpected stream");

                if (_httpStreams.Count == 0)
                {
                    // If this was last pending request, get timestamp so we can monitor idle time.
                    _idleSinceTickCount = Environment.TickCount64;
                }

                if (_disposed || _lastStreamId != -1)
                {
                    CheckForShutdown();
                }
            }
        }

        private static async ValueTask<int> ReadAtLeastAsync(Stream stream, Memory<byte> buffer, int minReadBytes)
        {
            Debug.Assert(buffer.Length >= minReadBytes);

            int totalBytesRead = 0;
            while (totalBytesRead < minReadBytes)
            {
                int bytesRead = await stream.ReadAsync(buffer.Slice(totalBytesRead)).ConfigureAwait(false);
                if (bytesRead == 0)
                {
                    throw new IOException(SR.Format(SR.net_http_invalid_response_premature_eof_bytecount, minReadBytes));
                }

                totalBytesRead += bytesRead;
            }

            return totalBytesRead;
        }

        public sealed override string ToString() => $"{nameof(Http2Connection)}({_pool})"; // Description for diagnostic purposes

        public override void Trace(string message, [CallerMemberName] string memberName = null) =>
            Trace(0, message, memberName);

        internal void Trace(int streamId, string message, [CallerMemberName] string memberName = null) =>
            NetEventSource.Log.HandlerMessage(
                _pool?.GetHashCode() ?? 0,    // pool ID
                GetHashCode(),                // connection ID
                streamId,                     // stream ID
                memberName,                   // method name
                message);                     // message

    }
}
