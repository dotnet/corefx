﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.IO;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Sources;

namespace System.Net.Http
{
    internal sealed partial class Http2Connection 
    {
        private sealed class Http2Stream : IValueTaskSource, IDisposable
        {
            private enum StreamState : byte
            {
                ExpectingHeaders,
                ExpectingData,
                Complete,
                Aborted
            }

            private const int InitialStreamBufferSize =
#if DEBUG
                10;
#else
                1024;
#endif

            private readonly Http2Connection _connection;
            private readonly int _streamId;
            private readonly CreditManager _streamWindow;
            private readonly HttpRequestMessage _request;
            private readonly HttpResponseMessage _response;

            private ArrayBuffer _responseBuffer; // mutable struct, do not make this readonly
            private int _pendingWindowUpdate;

            private StreamState _state;
            private bool _disposed;

            /// <summary>The core logic for the IValueTaskSource implementation.</summary>
            private ManualResetValueTaskSourceCore<bool> _waitSource = new ManualResetValueTaskSourceCore<bool> { RunContinuationsAsynchronously = true }; // mutable struct, do not make this readonly
            /// <summary>
            /// Whether code has requested or is about to request a wait be performed and thus requires a call to SetResult to complete it.
            /// This is read and written while holding the lock so that most operations on _waitSourceCore don't need to be.
            /// </summary>
            private bool _hasWaiter;

            private const int StreamWindowSize = DefaultInitialWindowSize;

            // See comment on ConnectionWindowThreshold.
            private const int StreamWindowThreshold = StreamWindowSize / 8;

            public Http2Stream(HttpRequestMessage request, Http2Connection connection, int streamId, int initialWindowSize)
            {
                _connection = connection;
                _streamId = streamId;

                _state = StreamState.ExpectingHeaders;

                _request = request;
                _response = new HttpResponseMessage()
                {
                    Version = HttpVersion.Version20,
                    RequestMessage = request,
                    Content = new HttpConnectionResponseContent()
                };

                _disposed = false;

                _responseBuffer = new ArrayBuffer(InitialStreamBufferSize, usePool: true);

                _pendingWindowUpdate = 0;

                _streamWindow = new CreditManager(initialWindowSize);
            }

            private object SyncObject => _streamWindow;

            public int StreamId => _streamId;
            public HttpRequestMessage Request => _request;
            public HttpResponseMessage Response => _response;

            public async Task SendRequestBodyAsync()
            {
                // TODO: ISSUE 31312: Expect: 100-continue and early response handling
                // Note that in an "early response" scenario, where we get a response before we've finished sending the request body
                // (either with a 100-continue that timed out, or without 100-continue),
                // we can stop send a RST_STREAM on the request stream and stop sending the request without tearing down the entire connection.

                // Send request body, if any
                if (_request.Content != null)
                {
                    using (Http2WriteStream writeStream = new Http2WriteStream(this))
                    {
                        await _request.Content.CopyToAsync(writeStream).ConfigureAwait(false);
                    }
                }
            }

            public void OnWindowUpdate(int amount)
            {
                _streamWindow.AdjustCredit(amount);
            }

            private static readonly byte[] s_statusHeaderName = Encoding.ASCII.GetBytes(":status");

            // Copied from HttpConnection
            // TODO: Consolidate this logic?
            private static bool IsDigit(byte c) => (uint)(c - '0') <= '9' - '0';

            public void OnResponseHeader(ReadOnlySpan<byte> name, ReadOnlySpan<byte> value)
            {
                // TODO: ISSUE 31309: Optimize HPACK static table decoding

                lock (SyncObject)
                {
                    if (_state != StreamState.ExpectingHeaders)
                    {
                        throw new Http2ProtocolException(Http2ProtocolErrorCode.ProtocolError);
                    }

                    if (name.SequenceEqual(s_statusHeaderName))
                    {
                        if (value.Length != 3)
                            throw new Exception("Invalid status code");

                        // Copied from HttpConnection
                        byte status1 = value[0], status2 = value[1], status3 = value[2];
                        if (!IsDigit(status1) || !IsDigit(status2) || !IsDigit(status3))
                        {
                            throw new HttpRequestException(SR.net_http_invalid_response);
                        }

                        _response.SetStatusCodeWithoutValidation((HttpStatusCode)(100 * (status1 - '0') + 10 * (status2 - '0') + (status3 - '0')));
                    }
                    else
                    {
                        if (!HeaderDescriptor.TryGet(name, out HeaderDescriptor descriptor))
                        {
                            // Invalid header name
                            throw new HttpRequestException(SR.net_http_invalid_response);
                        }

                        string headerValue = descriptor.GetHeaderValue(value);

                        // Note we ignore the return value from TryAddWithoutValidation; 
                        // if the header can't be added, we silently drop it.
                        if (descriptor.HeaderType == HttpHeaderType.Content)
                        {
                            _response.Content.Headers.TryAddWithoutValidation(descriptor, headerValue);
                        }
                        else
                        {
                            _response.Headers.TryAddWithoutValidation(descriptor, headerValue);
                        }
                    }
                }
            }

            public void OnResponseHeadersComplete(bool endStream)
            {
                bool signalWaiter;
                lock (SyncObject)
                {
                    if (_state != StreamState.ExpectingHeaders)
                    {
                        throw new Http2ProtocolException(Http2ProtocolErrorCode.ProtocolError);
                    }

                    _state = endStream ? StreamState.Complete : StreamState.ExpectingData;

                    signalWaiter = _hasWaiter;
                    _hasWaiter = false;
                }

                if (signalWaiter)
                {
                    _waitSource.SetResult(true);
                }
            }

            public void OnResponseData(ReadOnlySpan<byte> buffer, bool endStream)
            {
                bool signalWaiter;
                lock (SyncObject)
                {
                    if (_disposed)
                    {
                        return;
                    }

                    if (_state != StreamState.ExpectingData)
                    {
                        throw new Http2ProtocolException(Http2ProtocolErrorCode.ProtocolError);
                    }

                    if (_responseBuffer.ActiveSpan.Length + buffer.Length > StreamWindowSize)
                    {
                        // Window size exceeded.
                        throw new Http2ProtocolException(Http2ProtocolErrorCode.FlowControlError);
                    }

                    _responseBuffer.EnsureAvailableSpace(buffer.Length);
                    buffer.CopyTo(_responseBuffer.AvailableSpan);
                    _responseBuffer.Commit(buffer.Length);

                    if (endStream)
                    {
                        _state = StreamState.Complete;
                    }

                    signalWaiter = _hasWaiter;
                    _hasWaiter = false;
                }

                if (signalWaiter)
                {
                    _waitSource.SetResult(true);
                }
            }

            public void OnResponseAbort()
            {
                bool signalWaiter;
                lock (SyncObject)
                {
                    if (_disposed)
                    {
                        return;
                    }

                    if (_state == StreamState.Aborted)
                    {
                        return;
                    }

                    _state = StreamState.Aborted;

                    signalWaiter = _hasWaiter;
                    _hasWaiter = false;
                }

                if (signalWaiter)
                {
                    _waitSource.SetResult(true);
                }
            }

            private (bool wait, bool isEmptyResponse) TryEnsureHeaders()
            {
                lock (SyncObject)
                {
                    if (_disposed)
                    {
                        throw new ObjectDisposedException(nameof(Http2Stream));
                    }

                    if (_state == StreamState.Aborted)
                    {
                        throw new IOException(SR.net_http_invalid_response);
                    }
                    else if (_state == StreamState.ExpectingHeaders)
                    {
                        Debug.Assert(!_hasWaiter);
                        _hasWaiter = true;
                        _waitSource.Reset();
                        return (true, false);
                    }
                    else if (_state == StreamState.ExpectingData)
                    {
                        return (false, false);
                    }
                    else
                    {
                        Debug.Assert(_state == StreamState.Complete);
                        return (false, _responseBuffer.ActiveSpan.Length == 0);
                    }
                }
            }

            public async Task ReadResponseHeadersAsync()
            {
                // Wait for response headers to be read.
                (bool wait, bool emptyResponse) = TryEnsureHeaders();
                if (wait)
                {
                    await GetWaiterTask().ConfigureAwait(false);
                    (wait, emptyResponse) = TryEnsureHeaders();
                    Debug.Assert(!wait);
                }

                // Start to process the response body.
                ((HttpConnectionResponseContent)_response.Content).SetStream(emptyResponse ?
                    EmptyReadStream.Instance :
                    (Stream)new Http2ReadStream(this));

                // Process Set-Cookie headers.
                if (_connection._pool.Settings._useCookies)
                {
                    CookieHelper.ProcessReceivedCookies(_response, _connection._pool.Settings._cookieContainer);
                }
            }

            private void ExtendWindow(int amount)
            {
                Debug.Assert(amount > 0);
                Debug.Assert(_pendingWindowUpdate < StreamWindowThreshold);

                if (_state != StreamState.ExpectingData)
                {
                    // We are not expecting any more data (because we've either completed or aborted).
                    // So no need to send any more WINDOW_UPDATEs.
                    return;
                }

                _pendingWindowUpdate += amount;
                if (_pendingWindowUpdate < StreamWindowThreshold)
                {
                    return;
                }

                int windowUpdateSize = _pendingWindowUpdate;
                _pendingWindowUpdate = 0;

                ValueTask ignored = _connection.SendWindowUpdateAsync(_streamId, windowUpdateSize);
            }

            private (bool wait, int bytesRead) TryReadFromBuffer(Span<byte> buffer)
            {
                Debug.Assert(buffer.Length > 0);

                lock (SyncObject)
                {
                    if (_disposed)
                    {
                        throw new ObjectDisposedException(nameof(Http2Stream));
                    }

                    if (_responseBuffer.ActiveSpan.Length > 0)
                    {
                        int bytesRead = Math.Min(buffer.Length, _responseBuffer.ActiveSpan.Length);
                        _responseBuffer.ActiveSpan.Slice(0, bytesRead).CopyTo(buffer);
                        _responseBuffer.Discard(bytesRead);

                        return (false, bytesRead);
                    }
                    else if (_state == StreamState.Complete)
                    {
                        return (false, 0);
                    }
                    else if (_state == StreamState.Aborted)
                    {
                        throw new IOException(SR.net_http_invalid_response);
                    }

                    Debug.Assert(_state == StreamState.ExpectingData);

                    Debug.Assert(!_hasWaiter);
                    _hasWaiter = true;
                    _waitSource.Reset();
                    return (true, 0);
                }
            }

            public async ValueTask<int> ReadDataAsync(Memory<byte> buffer, CancellationToken cancellationToken)
            {
                if (buffer.Length == 0)
                {
                    return 0;
                }

                (bool wait, int bytesRead) = TryReadFromBuffer(buffer.Span);
                if (wait)
                {
                    Debug.Assert(bytesRead == 0);
                    await GetWaiterTask().ConfigureAwait(false);
                    (wait, bytesRead) = TryReadFromBuffer(buffer.Span);
                    Debug.Assert(!wait);
                }

                if (bytesRead != 0)
                {
                    ExtendWindow(bytesRead);
                    _connection.ExtendWindow(bytesRead);
                }

                return bytesRead;
            }

            private async ValueTask SendDataAsync(ReadOnlyMemory<byte> buffer)
            {
                ReadOnlyMemory<byte> remaining = buffer;

                while (remaining.Length > 0)
                {
                    int sendSize = await _streamWindow.RequestCreditAsync(remaining.Length).ConfigureAwait(false);

                    ReadOnlyMemory<byte> current;
                    (current, remaining) = SplitBuffer(remaining, sendSize);

                    await _connection.SendStreamDataAsync(_streamId, current).ConfigureAwait(false);
                }
            }

            public void Dispose()
            {
                lock (SyncObject)
                {
                    if (!_disposed)
                    {
                        _disposed = true;

                        _streamWindow.Dispose();
                        _responseBuffer.Dispose();

                        // TODO: ISSUE 31310: If the stream is not complete, we should send RST_STREAM
                    }
                }
            }

            // This object is itself usable as a backing source for ValueTask.  Since there's only ever one awaiter
            // for this object's state transitions at a time, we allow the object to be awaited directly. All functionality
            // associated with the implementation is just delegated to the ManualResetValueTaskSourceCore.
            private ValueTask GetWaiterTask() => new ValueTask(this, _waitSource.Version);
            ValueTaskSourceStatus IValueTaskSource.GetStatus(short token) => _waitSource.GetStatus(token);
            void IValueTaskSource.OnCompleted(Action<object> continuation, object state, short token, ValueTaskSourceOnCompletedFlags flags) => _waitSource.OnCompleted(continuation, state, token, flags);
            void IValueTaskSource.GetResult(short token) => _waitSource.GetResult(token);

            private sealed class Http2ReadStream : BaseAsyncStream
            {
                private Http2Stream _http2Stream;

                public Http2ReadStream(Http2Stream http2Stream)
                {
                    Debug.Assert(http2Stream != null);
                    _http2Stream = http2Stream;
                }

                protected override void Dispose(bool disposing)
                {
                    Http2Stream http2Stream = Interlocked.Exchange(ref _http2Stream, null);
                    if (http2Stream == null)
                    {
                        return;
                    }

                    if (disposing)
                    {
                        http2Stream.Dispose();
                    }

                    base.Dispose(disposing);
                }

                public override bool CanRead => true;
                public override bool CanWrite => false;

                public override ValueTask<int> ReadAsync(Memory<byte> destination, CancellationToken cancellationToken)
                {
                    Http2Stream http2Stream = _http2Stream;
                    if (http2Stream == null)
                    {
                        return new ValueTask<int>(Task.FromException<int>(new ObjectDisposedException(nameof(Http2ReadStream))));
                    }

                    return http2Stream.ReadDataAsync(destination, cancellationToken);
                }

                public override ValueTask WriteAsync(ReadOnlyMemory<byte> destination, CancellationToken cancellationToken) => throw new NotSupportedException();

                public override Task FlushAsync(CancellationToken cancellationToken) => throw new NotSupportedException();
            }


            private sealed class Http2WriteStream : BaseAsyncStream
            {
                private Http2Stream _http2Stream;

                public Http2WriteStream(Http2Stream http2Stream)
                {
                    Debug.Assert(http2Stream != null);
                    _http2Stream = http2Stream;
                }

                protected override void Dispose(bool disposing)
                {
                    Http2Stream http2Stream = Interlocked.Exchange(ref _http2Stream, null);
                    if (http2Stream == null)
                    {
                        return;
                    }

                    // Don't wait for completion, which could happen asynchronously.
                    ValueTask ignored = http2Stream._connection.SendEndStreamAsync(http2Stream.StreamId);

                    base.Dispose(disposing);
                }

                public override bool CanRead => false;
                public override bool CanWrite => true;

                public override ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken) => throw new NotSupportedException();

                public override ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken)
                {
                    Http2Stream http2Stream = _http2Stream;
                    if (http2Stream == null)
                    {
                        return new ValueTask(Task.FromException(new ObjectDisposedException(nameof(Http2WriteStream))));
                    }

                    return http2Stream.SendDataAsync(buffer);
                }

                public override Task FlushAsync(CancellationToken cancellationToken) => Task.CompletedTask;
            }
        }
    }
}
