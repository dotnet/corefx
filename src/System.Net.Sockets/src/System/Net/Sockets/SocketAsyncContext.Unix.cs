// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;

namespace System.Net.Sockets
{
    // Note on asynchronous behavior here:

    // The asynchronous socket operations here generally do the following:
    // (1) If the operation queue is empty, try to perform the operation immediately, non-blocking.
    // If this completes (i.e. does not return EWOULDBLOCK), then we return the results immediately
    // for both success (SocketError.Success) or failure.
    // No callback will happen; callers are expected to handle these synchronous completions themselves.
    // (2) If EWOULDBLOCK is returned, or the queue is not empty, then we enqueue an operation to the 
    // appropriate queue and return SocketError.IOPending.
    // Enqueuing itself may fail because the socket is closed before the operation can be enqueued;
    // in this case, we return SocketError.OperationAborted (which matches what Winsock would return in this case).
    // (3) When the queue completes the operation, it will post a work item to the threadpool
    // to call the callback with results (either success or failure).

    // Synchronous operations generally do the same, except that instead of returning IOPending,
    // they block on an event handle until the operation is processed by the queue.
    // Also, synchronous methods return SocketError.Interrupted when enqueuing fails
    // (which again matches Winsock behavior).

    internal sealed class SocketAsyncContext
    {
        private abstract class AsyncOperation
        {
            private enum State
            {
                Waiting = 0,
                Running = 1,
                Complete = 2,
                Cancelled = 3
            }

            private int _state; // Actually AsyncOperation.State.

#if DEBUG
            private int _callbackQueued; // When non-zero, the callback has been queued.
#endif

            public AsyncOperation Next;
            protected object CallbackOrEvent;
            public SocketError ErrorCode;
            public byte[] SocketAddress;
            public int SocketAddressLen;

            public ManualResetEventSlim Event
            {
                private get { return (ManualResetEventSlim)CallbackOrEvent; }
                set { CallbackOrEvent = value; }
            }

            public AsyncOperation()
            {
                _state = (int)State.Waiting;
                Next = this;
            }

            public bool TryComplete(SocketAsyncContext context)
            {
                Debug.Assert(_state == (int)State.Waiting, $"Unexpected _state: {_state}");

                return DoTryComplete(context);
            }

            public bool TryCompleteAsync(SocketAsyncContext context)
            {
                return TryCompleteOrAbortAsync(context, abort: false);
            }

            public void AbortAsync()
            {
                bool completed = TryCompleteOrAbortAsync(null, abort: true);
                Debug.Assert(completed, $"Expected TryCompleteOrAbortAsync to return true");
            }

            private bool TryCompleteOrAbortAsync(SocketAsyncContext context, bool abort)
            {
                Debug.Assert(context != null || abort, $"Unexpected values: context={context}, abort={abort}");

                State oldState = (State)Interlocked.CompareExchange(ref _state, (int)State.Running, (int)State.Waiting);
                if (oldState == State.Cancelled)
                {
                    // This operation has been cancelled. The canceller is responsible for
                    // correctly updating any state that would have been handled by
                    // AsyncOperation.Abort.
                    return true;
                }

                Debug.Assert(oldState != State.Complete && oldState != State.Running, $"Unexpected oldState: {oldState}");

                bool completed;
                if (abort)
                {
                    Abort();
                    ErrorCode = SocketError.OperationAborted;
                    completed = true;
                }
                else
                {
                    completed = DoTryComplete(context);
                }

                if (completed)
                {
                    var @event = CallbackOrEvent as ManualResetEventSlim;
                    if (@event != null)
                    {
                        @event.Set();
                    }
                    else
                    {
                        Debug.Assert(_state != (int)State.Cancelled, $"Unexpected _state: {_state}");
#if DEBUG
                        Debug.Assert(Interlocked.CompareExchange(ref _callbackQueued, 1, 0) == 0, $"Unexpected _callbackQueued: {_callbackQueued}");
#endif

                        ThreadPool.QueueUserWorkItem(o => ((AsyncOperation)o).InvokeCallback(), this);
                    }

                    Volatile.Write(ref _state, (int)State.Complete);
                    return true;
                }

                Volatile.Write(ref _state, (int)State.Waiting);
                return false;
            }

            public bool Wait(int timeout)
            {
                if (Event.Wait(timeout))
                {
                    return true;
                }

                var spinWait = new SpinWait();
                for (;;)
                {
                    int state = Interlocked.CompareExchange(ref _state, (int)State.Cancelled, (int)State.Waiting);
                    switch ((State)state)
                    {
                        case State.Running:
                            // A completion attempt is in progress. Keep busy-waiting.
                            spinWait.SpinOnce();
                            break;

                        case State.Complete:
                            // A completion attempt succeeded. Consider this operation as having completed within the timeout.
                            return true;

                        case State.Waiting:
                            // This operation was successfully cancelled.
                            return false;
                    }
                }
            }

            protected abstract void Abort();

            protected abstract bool DoTryComplete(SocketAsyncContext context);

            protected abstract void InvokeCallback();
        }

        // These two abstract classes differentiate the operations that go in the
        // read queue vs the ones that go in the write queue.
        private abstract class ReadOperation : AsyncOperation 
        {
        }

        private abstract class WriteOperation : AsyncOperation 
        {
        }        

        private sealed class SendOperation : WriteOperation
        {
            public byte[] Buffer;
            public int Offset;
            public int Count;
            public SocketFlags Flags;
            public int BytesTransferred;
            public IList<ArraySegment<byte>> Buffers;
            public int BufferIndex;

            protected sealed override void Abort() { }

            public Action<int, byte[], int, SocketFlags, SocketError> Callback
            {
                private get { return (Action<int, byte[], int, SocketFlags, SocketError>)CallbackOrEvent; }
                set { CallbackOrEvent = value; }
            }

            protected sealed override void InvokeCallback()
            {
                Callback(BytesTransferred, SocketAddress, SocketAddressLen, SocketFlags.None, ErrorCode);
            }
            protected override bool DoTryComplete(SocketAsyncContext context)
            {
                return SocketPal.TryCompleteSendTo(context._socket, Buffer, Buffers, ref BufferIndex, ref Offset, ref Count, Flags, SocketAddress, SocketAddressLen, ref BytesTransferred, out ErrorCode);
            }
        }

        private sealed class ReceiveOperation : ReadOperation
        {
            public byte[] Buffer;
            public int Offset;
            public int Count;
            public SocketFlags Flags;
            public int BytesTransferred;
            public SocketFlags ReceivedFlags;
            public IList<ArraySegment<byte>> Buffers;

            protected sealed override void Abort() { }

            public Action<int, byte[], int, SocketFlags, SocketError> Callback
            {
                private get { return (Action<int, byte[], int, SocketFlags, SocketError>)CallbackOrEvent; }
                set { CallbackOrEvent = value; }
            }

            protected sealed override void InvokeCallback()
            {
                Callback(BytesTransferred, SocketAddress, SocketAddressLen, ReceivedFlags, ErrorCode);
            }
            protected override bool DoTryComplete(SocketAsyncContext context)
            {
                return SocketPal.TryCompleteReceiveFrom(context._socket, Buffer, Buffers, Offset, Count, Flags, SocketAddress, ref SocketAddressLen, out BytesTransferred, out ReceivedFlags, out ErrorCode);
            }
        }

        private sealed class ReceiveMessageFromOperation : ReadOperation
        {
            public byte[] Buffer;
            public int Offset;
            public int Count;
            public SocketFlags Flags;
            public int BytesTransferred;
            public SocketFlags ReceivedFlags;

            public bool IsIPv4;
            public bool IsIPv6;
            public IPPacketInformation IPPacketInformation;

            protected sealed override void Abort() { }

            public Action<int, byte[], int, SocketFlags, IPPacketInformation, SocketError> Callback
            {
                private get { return (Action<int, byte[], int, SocketFlags, IPPacketInformation, SocketError>)CallbackOrEvent; }
                set { CallbackOrEvent = value; }
            }

            protected override bool DoTryComplete(SocketAsyncContext context)
            {
                return SocketPal.TryCompleteReceiveMessageFrom(context._socket, Buffer, Offset, Count, Flags, SocketAddress, ref SocketAddressLen, IsIPv4, IsIPv6, out BytesTransferred, out ReceivedFlags, out IPPacketInformation, out ErrorCode);
            }

            protected override void InvokeCallback()
            {
                Callback(BytesTransferred, SocketAddress, SocketAddressLen, ReceivedFlags, IPPacketInformation, ErrorCode);
            }
        }

        private sealed class AcceptOperation : ReadOperation
        {
            public IntPtr AcceptedFileDescriptor;

            public Action<IntPtr, byte[], int, SocketError> Callback
            {
                private get { return (Action<IntPtr, byte[], int, SocketError>)CallbackOrEvent; }
                set { CallbackOrEvent = value; }
            }

            protected override void Abort()
            {
                AcceptedFileDescriptor = (IntPtr)(-1);
            }

            protected override bool DoTryComplete(SocketAsyncContext context)
            {
                bool completed = SocketPal.TryCompleteAccept(context._socket, SocketAddress, ref SocketAddressLen, out AcceptedFileDescriptor, out ErrorCode);
                Debug.Assert(ErrorCode == SocketError.Success || AcceptedFileDescriptor == (IntPtr)(-1), $"Unexpected values: ErrorCode={ErrorCode}, AcceptedFileDescriptor={AcceptedFileDescriptor}");
                return completed;
            }

            protected override void InvokeCallback()
            {
                Callback(AcceptedFileDescriptor, SocketAddress, SocketAddressLen, ErrorCode);
            }
        }

        private sealed class ConnectOperation : WriteOperation
        {
            public Action<SocketError> Callback
            {
                private get { return (Action<SocketError>)CallbackOrEvent; }
                set { CallbackOrEvent = value; }
            }

            protected override void Abort() { }

            protected override bool DoTryComplete(SocketAsyncContext context)
            {
                bool result = SocketPal.TryCompleteConnect(context._socket, SocketAddressLen, out ErrorCode);
                context._socket.RegisterConnectResult(ErrorCode);
                return result;
            }

            protected override void InvokeCallback()
            {
                Callback(ErrorCode);
            }
        }

        private sealed class SendFileOperation : WriteOperation
        {
            public SafeFileHandle FileHandle;
            public long Offset;
            public long Count;
            public long BytesTransferred;

            protected override void Abort() { }

            public Action<long, SocketError> Callback
            {
                private get { return (Action<long, SocketError>)CallbackOrEvent; }
                set { CallbackOrEvent = value; }
            }

            protected override void InvokeCallback()
            {
                Callback(BytesTransferred, ErrorCode);
            }

            protected override bool DoTryComplete(SocketAsyncContext context)
            {
                return SocketPal.TryCompleteSendFile(context._socket, FileHandle, ref Offset, ref Count, ref BytesTransferred, out ErrorCode);
            }
        }

        private enum QueueState
        {
            Clear = 0,
            Set = 1,
            Stopped = 2,
        }

        private struct OperationQueue<TOperation>
            where TOperation : AsyncOperation
        {
            private object _queueLock;
            private AsyncOperation _tail;

            public QueueState State { get; set; }
            public bool IsStopped { get { return State == QueueState.Stopped; } }
            public bool IsEmpty { get { return _tail == null; } }
            public object QueueLock { get { return _queueLock; } }

            public void Init()
            {
                Debug.Assert(_queueLock == null);
                _queueLock = new object();
            }

            public void Enqueue(TOperation operation)
            {
                Debug.Assert(!IsStopped, "Expected !IsStopped");
                Debug.Assert(operation.Next == operation, "Expected operation.Next == operation");

                if (!IsEmpty)
                {
                    operation.Next = _tail.Next;
                    _tail.Next = operation;
                }

                _tail = operation;
            }

            private bool TryDequeue(out TOperation operation)
            {
                if (_tail == null)
                {
                    operation = null;
                    return false;
                }

                AsyncOperation head = _tail.Next;
                if (head == _tail)
                {
                    _tail = null;
                }
                else
                {
                    _tail.Next = head.Next;
                }

                head.Next = null;
                operation = (TOperation)head;
                return true;
            }

            private void Requeue(TOperation operation)
            {
                // Insert at the head of the queue
                Debug.Assert(!IsStopped, "Expected !IsStopped");
                Debug.Assert(operation.Next == null, "Operation already in queue");

                if (IsEmpty)
                {
                    operation.Next = operation;
                    _tail = operation;
                }
                else
                {
                    operation.Next = _tail.Next;
                    _tail.Next = operation;
                }
            }

            public void Complete(SocketAsyncContext context)
            {
                lock (_queueLock)
                {
                    if (IsStopped)
                        return;

                    State = QueueState.Set;

                    TOperation op;
                    while (TryDequeue(out op))
                    {
                        if (!op.TryCompleteAsync(context))
                        {
                            Requeue(op);
                            return;
                        }
                    }
                }
            }

            public void StopAndAbort()
            {
                lock (_queueLock)
                {
                    State = QueueState.Stopped;

                    TOperation op;
                    while (TryDequeue(out op))
                    {
                        op.AbortAsync();
                    }
                }
            }
        }

        private readonly SafeCloseSocket _socket;
        private OperationQueue<ReadOperation> _receiveQueue;
        private OperationQueue<WriteOperation> _sendQueue;
        private SocketAsyncEngine.Token _asyncEngineToken;
        private Interop.Sys.SocketEvents _registeredEvents;
        private bool _nonBlockingSet;

        private readonly object _registerLock = new object();

        public SocketAsyncContext(SafeCloseSocket socket)
        {
            _socket = socket;

            _receiveQueue.Init();
            _sendQueue.Init();
        }

        private void Register(Interop.Sys.SocketEvents events)
        {
            lock (_registerLock)
            {
                Debug.Assert((_registeredEvents & events) == Interop.Sys.SocketEvents.None, $"Unexpected values: _registeredEvents={_registeredEvents}, events={events}");

                if (!_asyncEngineToken.WasAllocated)
                {
                    _asyncEngineToken = new SocketAsyncEngine.Token(this);
                }

                events |= _registeredEvents;

                Interop.Error errorCode;
                if (!_asyncEngineToken.TryRegister(_socket, _registeredEvents, events, out errorCode))
                {
                    if (errorCode == Interop.Error.ENOMEM || errorCode == Interop.Error.ENOSPC)
                    {
                        throw new OutOfMemoryException();
                    }
                    else
                    {
                        throw new InternalException();                        
                    }
                }

                _registeredEvents = events;
            }
        }

        public void Close()
        {
            // Drain queues
            _sendQueue.StopAndAbort();
            _receiveQueue.StopAndAbort();

            lock (_registerLock)
            { 
                // Freeing the token will prevent any future event delivery.  This socket will be unregistered
                // from the event port automatically by the OS when it's closed.
                _asyncEngineToken.Free();
            }
        }

        public void SetNonBlocking()
        {
            //
            // Our sockets may start as blocking, and later transition to non-blocking, either because the user
            // explicitly requested non-blocking mode, or because we need non-blocking mode to support async
            // operations.  We never transition back to blocking mode, to avoid problems synchronizing that
            // transition with the async infrastructure.
            //
            // Note that there's no synchronization here, so we may set the non-blocking option multiple times
            // in a race.  This should be fine.
            //
            if (!_nonBlockingSet)
            {
                if (Interop.Sys.Fcntl.SetIsNonBlocking(_socket, 1) != 0)
                {
                    throw new SocketException((int)SocketPal.GetSocketErrorForErrorCode(Interop.Sys.GetLastError()));
                }

                _nonBlockingSet = true;
            }
        }

        private bool TryBeginOperation<TOperation>(ref OperationQueue<TOperation> queue, TOperation operation, Interop.Sys.SocketEvents events, bool maintainOrder, out bool isStopped)
            where TOperation : AsyncOperation
        {
            // Exactly one of the two queue locks must be held by the caller
            Debug.Assert(Monitor.IsEntered(_sendQueue.QueueLock) ^ Monitor.IsEntered(_receiveQueue.QueueLock));

            switch (queue.State)
            {
                case QueueState.Stopped:
                    isStopped = true;
                    return false;

                case QueueState.Clear:
                    break;

                case QueueState.Set:
                    if (queue.IsEmpty || !maintainOrder)
                    {
                        isStopped = false;
                        queue.State = QueueState.Clear;
                        return false;
                    }
                    break;
            }

            if ((_registeredEvents & events) == Interop.Sys.SocketEvents.None)
            {
                Register(events);
            }

            queue.Enqueue(operation);
            isStopped = false;
            return true;
        }

        public SocketError Accept(byte[] socketAddress, ref int socketAddressLen, int timeout, out IntPtr acceptedFd)
        {
            Debug.Assert(socketAddress != null, "Expected non-null socketAddress");
            Debug.Assert(socketAddressLen > 0, $"Unexpected socketAddressLen: {socketAddressLen}");
            Debug.Assert(timeout == -1 || timeout > 0, $"Unexpected timeout: {timeout}");

            SocketError errorCode;
            if (SocketPal.TryCompleteAccept(_socket, socketAddress, ref socketAddressLen, out acceptedFd, out errorCode))
            {
                Debug.Assert(errorCode == SocketError.Success || acceptedFd == (IntPtr)(-1), $"Unexpected values: errorCode={errorCode}, acceptedFd={acceptedFd}");
                return errorCode;
            }

            using (var @event = new ManualResetEventSlim(false, 0))
            {
                var operation = new AcceptOperation {
                    Event = @event,
                    SocketAddress = socketAddress,
                    SocketAddressLen = socketAddressLen
                };

                bool isStopped;
                while (true)
                {
                    lock (_receiveQueue.QueueLock)
                    {
                        if (TryBeginOperation(ref _receiveQueue, operation, Interop.Sys.SocketEvents.Read, maintainOrder: false, isStopped: out isStopped))
                        {
                            break;
                        }
                    }

                    if (isStopped)
                    {
                        acceptedFd = (IntPtr)(-1);
                        return SocketError.Interrupted;
                    }

                    if (operation.TryComplete(this))
                    {
                        socketAddressLen = operation.SocketAddressLen;
                        acceptedFd = operation.AcceptedFileDescriptor;
                        return operation.ErrorCode;
                    }
                }

                if (!operation.Wait(timeout))
                {
                    acceptedFd = (IntPtr)(-1);
                    return SocketError.TimedOut;
                }

                socketAddressLen = operation.SocketAddressLen;
                acceptedFd = operation.AcceptedFileDescriptor;
                return operation.ErrorCode;
            }
        }

        public SocketError AcceptAsync(byte[] socketAddress, ref int socketAddressLen, out IntPtr acceptedFd, Action<IntPtr, byte[], int, SocketError> callback)
        {
            Debug.Assert(socketAddress != null, "Expected non-null socketAddress");
            Debug.Assert(socketAddressLen > 0, $"Unexpected socketAddressLen: {socketAddressLen}");
            Debug.Assert(callback != null, "Expected non-null callback");

            SetNonBlocking();

            SocketError errorCode;
            if (SocketPal.TryCompleteAccept(_socket, socketAddress, ref socketAddressLen, out acceptedFd, out errorCode))
            {
                Debug.Assert(errorCode == SocketError.Success || acceptedFd == (IntPtr)(-1), $"Unexpected values: errorCode={errorCode}, acceptedFd={acceptedFd}");

                return errorCode;
            }

            var operation = new AcceptOperation {
                Callback = callback,
                SocketAddress = socketAddress,
                SocketAddressLen = socketAddressLen
            };

            bool isStopped;
            while (true)
            {
                lock (_receiveQueue.QueueLock)
                {
                    if (TryBeginOperation(ref _receiveQueue, operation, Interop.Sys.SocketEvents.Read, maintainOrder: false, isStopped: out isStopped))
                    {
                        break;
                    }
                }

                if (isStopped)
                {
                    return SocketError.OperationAborted;
                }

                if (operation.TryComplete(this))
                {
                    socketAddressLen = operation.SocketAddressLen;
                    acceptedFd = operation.AcceptedFileDescriptor;
                    return operation.ErrorCode;
                }
            }
            return SocketError.IOPending;
        }

        public SocketError Connect(byte[] socketAddress, int socketAddressLen, int timeout)
        {
            Debug.Assert(socketAddress != null, "Expected non-null socketAddress");
            Debug.Assert(socketAddressLen > 0, $"Unexpected socketAddressLen: {socketAddressLen}");
            Debug.Assert(timeout == -1 || timeout > 0, $"Unexpected timeout: {timeout}");

            SocketError errorCode;
            if (SocketPal.TryStartConnect(_socket, socketAddress, socketAddressLen, out errorCode))
            {
                _socket.RegisterConnectResult(errorCode);
                return errorCode;
            }

            using (var @event = new ManualResetEventSlim(false, 0))
            {
                var operation = new ConnectOperation {
                    Event = @event,
                    SocketAddress = socketAddress,
                    SocketAddressLen = socketAddressLen
                };

                bool isStopped;
                while (true)
                {
                    lock (_sendQueue.QueueLock)
                    {
                        if (TryBeginOperation(ref _sendQueue, operation, Interop.Sys.SocketEvents.Write, maintainOrder: false, isStopped: out isStopped))
                        {
                            break;
                        }
                    }

                    if (isStopped)
                    {
                        return SocketError.Interrupted;
                    }

                    if (operation.TryComplete(this))
                    {
                        return operation.ErrorCode;
                    }
                }

                return operation.Wait(timeout) ? operation.ErrorCode : SocketError.TimedOut;
            }
        }

        public SocketError ConnectAsync(byte[] socketAddress, int socketAddressLen, Action<SocketError> callback)
        {
            Debug.Assert(socketAddress != null, "Expected non-null socketAddress");
            Debug.Assert(socketAddressLen > 0, $"Unexpected socketAddressLen: {socketAddressLen}");
            Debug.Assert(callback != null, "Expected non-null callback");

            SetNonBlocking();

            SocketError errorCode;
            if (SocketPal.TryStartConnect(_socket, socketAddress, socketAddressLen, out errorCode))
            {
                _socket.RegisterConnectResult(errorCode);

                return errorCode;
            }

            var operation = new ConnectOperation {
                Callback = callback,
                SocketAddress = socketAddress,
                SocketAddressLen = socketAddressLen
            };

            bool isStopped;
            while (true)
            {
                lock (_sendQueue.QueueLock)
                {
                    if (TryBeginOperation(ref _sendQueue, operation, Interop.Sys.SocketEvents.Write, maintainOrder: false, isStopped: out isStopped))
                    {
                        break;
                    }
                }

                if (isStopped)
                {
                    return SocketError.OperationAborted;
                }

                if (operation.TryComplete(this))
                {
                    return operation.ErrorCode;
                }
            }
            return SocketError.IOPending;
        }

        public SocketError Receive(byte[] buffer, int offset, int count, ref SocketFlags flags, int timeout, out int bytesReceived)
        {
            int socketAddressLen = 0;
            return ReceiveFrom(buffer, offset, count, ref flags, null, ref socketAddressLen, timeout, out bytesReceived);
        }

        public SocketError ReceiveAsync(byte[] buffer, int offset, int count, SocketFlags flags, out int bytesReceived, out SocketFlags receivedFlags, Action<int, byte[], int, SocketFlags, SocketError> callback)
        {
            int socketAddressLen = 0;
            return ReceiveFromAsync(buffer, offset, count, flags, null, ref socketAddressLen, out bytesReceived, out receivedFlags, callback);
        }

        public SocketError ReceiveFrom(byte[] buffer, int offset, int count, ref SocketFlags flags, byte[] socketAddress, ref int socketAddressLen, int timeout, out int bytesReceived)
        {
            Debug.Assert(timeout == -1 || timeout > 0, $"Unexpected timeout: {timeout}");

            ManualResetEventSlim @event = null;
            try
            {
                ReceiveOperation operation;
                lock (_receiveQueue.QueueLock)
                {
                    SocketFlags receivedFlags;
                    SocketError errorCode;

                    if (_receiveQueue.IsEmpty &&
                        SocketPal.TryCompleteReceiveFrom(_socket, buffer, offset, count, flags, socketAddress, ref socketAddressLen, out bytesReceived, out receivedFlags, out errorCode))
                    {
                        flags = receivedFlags;
                        return errorCode;
                    }

                    @event = new ManualResetEventSlim(false, 0);

                    operation = new ReceiveOperation
                    {
                        Event = @event,
                        Buffer = buffer,
                        Offset = offset,
                        Count = count,
                        Flags = flags,
                        SocketAddress = socketAddress,
                        SocketAddressLen = socketAddressLen,
                    };

                    bool isStopped;
                    while (!TryBeginOperation(ref _receiveQueue, operation, Interop.Sys.SocketEvents.Read, maintainOrder: true, isStopped: out isStopped))
                    {
                        if (isStopped)
                        {
                            flags = operation.ReceivedFlags;
                            bytesReceived = operation.BytesTransferred;
                            return SocketError.Interrupted;
                        }

                        if (operation.TryComplete(this))
                        {
                            socketAddressLen = operation.SocketAddressLen;
                            flags = operation.ReceivedFlags;
                            bytesReceived = operation.BytesTransferred;
                            return operation.ErrorCode;
                        }
                    }
                }

                bool signaled = operation.Wait(timeout);
                socketAddressLen = operation.SocketAddressLen;
                flags = operation.ReceivedFlags;
                bytesReceived = operation.BytesTransferred;
                return signaled ? operation.ErrorCode : SocketError.TimedOut;
            }
            finally
            {
                if (@event != null) @event.Dispose();
            }
        }

        public SocketError ReceiveFromAsync(byte[] buffer, int offset, int count, SocketFlags flags, byte[] socketAddress, ref int socketAddressLen, out int bytesReceived, out SocketFlags receivedFlags, Action<int, byte[], int, SocketFlags, SocketError> callback)
        {
            SetNonBlocking();

            lock (_receiveQueue.QueueLock)
            {
                SocketError errorCode;

                if (_receiveQueue.IsEmpty &&
                    SocketPal.TryCompleteReceiveFrom(_socket, buffer, offset, count, flags, socketAddress, ref socketAddressLen, out bytesReceived, out receivedFlags, out errorCode))
                {
                    // Synchronous success or failure
                    return errorCode;
                }

                var operation = new ReceiveOperation
                {
                    Callback = callback,
                    Buffer = buffer,
                    Offset = offset,
                    Count = count,
                    Flags = flags,
                    SocketAddress = socketAddress,
                    SocketAddressLen = socketAddressLen,
                };

                bool isStopped;
                while (!TryBeginOperation(ref _receiveQueue, operation, Interop.Sys.SocketEvents.Read, maintainOrder: true, isStopped: out isStopped))
                {
                    if (isStopped)
                    {
                        bytesReceived = 0;
                        receivedFlags = SocketFlags.None;
                        return SocketError.OperationAborted;
                    }

                    if (operation.TryComplete(this))
                    {
                        socketAddressLen = operation.SocketAddressLen;
                        receivedFlags = operation.ReceivedFlags;
                        bytesReceived = operation.BytesTransferred;
                        return operation.ErrorCode;
                    }
                }

                bytesReceived = 0;
                receivedFlags = SocketFlags.None;
                return SocketError.IOPending;
            }
        }

        public SocketError Receive(IList<ArraySegment<byte>> buffers, ref SocketFlags flags, int timeout, out int bytesReceived)
        {
            return ReceiveFrom(buffers, ref flags, null, 0, timeout, out bytesReceived);
        }

        public SocketError ReceiveAsync(IList<ArraySegment<byte>> buffers, SocketFlags flags, out int bytesReceived, out SocketFlags receivedFlags, Action<int, byte[], int, SocketFlags, SocketError> callback)
        {
            int socketAddressLen = 0;
            return ReceiveFromAsync(buffers, flags, null, ref socketAddressLen, out bytesReceived, out receivedFlags, callback);
        }

        public SocketError ReceiveFrom(IList<ArraySegment<byte>> buffers, ref SocketFlags flags, byte[] socketAddress, int socketAddressLen, int timeout, out int bytesReceived)
        {
            Debug.Assert(timeout == -1 || timeout > 0, $"Unexpected timeout: {timeout}");

            ManualResetEventSlim @event = null;
            try
            {
                ReceiveOperation operation;

                lock (_receiveQueue.QueueLock)
                {
                    SocketFlags receivedFlags;
                    SocketError errorCode;
                    if (_receiveQueue.IsEmpty &&
                        SocketPal.TryCompleteReceiveFrom(_socket, buffers, flags, socketAddress, ref socketAddressLen, out bytesReceived, out receivedFlags, out errorCode))
                    {
                        flags = receivedFlags;
                        return errorCode;
                    }

                    @event = new ManualResetEventSlim(false, 0);

                    operation = new ReceiveOperation
                    {
                        Event = @event,
                        Buffers = buffers,
                        Flags = flags,
                        SocketAddress = socketAddress,
                        SocketAddressLen = socketAddressLen,
                    };

                    bool isStopped;
                    while (!TryBeginOperation(ref _receiveQueue, operation, Interop.Sys.SocketEvents.Read, maintainOrder: true, isStopped: out isStopped))
                    {
                        if (isStopped)
                        {
                            flags = operation.ReceivedFlags;
                            bytesReceived = operation.BytesTransferred;
                            return SocketError.Interrupted;
                        }

                        if (operation.TryComplete(this))
                        {
                            socketAddressLen = operation.SocketAddressLen;
                            flags = operation.ReceivedFlags;
                            bytesReceived = operation.BytesTransferred;
                            return operation.ErrorCode;
                        }
                    }

                }

                bool signaled = operation.Wait(timeout);
                socketAddressLen = operation.SocketAddressLen;
                flags = operation.ReceivedFlags;
                bytesReceived = operation.BytesTransferred;
                return signaled ? operation.ErrorCode : SocketError.TimedOut;
            }
            finally
            {
                if (@event != null) @event.Dispose();
            }
        }

        public SocketError ReceiveFromAsync(IList<ArraySegment<byte>> buffers, SocketFlags flags, byte[] socketAddress, ref int socketAddressLen, out int bytesReceived, out SocketFlags receivedFlags, Action<int, byte[], int, SocketFlags, SocketError> callback)
        {
            SetNonBlocking();

            ReceiveOperation operation;

            lock (_receiveQueue.QueueLock)
            {
                SocketError errorCode;
                if (_receiveQueue.IsEmpty &&
                    SocketPal.TryCompleteReceiveFrom(_socket, buffers, flags, socketAddress, ref socketAddressLen, out bytesReceived, out receivedFlags, out errorCode))
                {
                    // Synchronous success or failure
                    return errorCode;
                }

                operation = new ReceiveOperation
                {
                    Callback = callback,
                    Buffers = buffers,
                    Flags = flags,
                    SocketAddress = socketAddress,
                    SocketAddressLen = socketAddressLen,
                };

                bool isStopped;
                while (!TryBeginOperation(ref _receiveQueue, operation, Interop.Sys.SocketEvents.Read, maintainOrder: true, isStopped: out isStopped))
                {
                    if (isStopped)
                    {
                        bytesReceived = 0;
                        receivedFlags = SocketFlags.None;
                        return SocketError.OperationAborted;
                    }

                    if (operation.TryComplete(this))
                    {
                        socketAddressLen = operation.SocketAddressLen;
                        receivedFlags = operation.ReceivedFlags;
                        bytesReceived = operation.BytesTransferred;
                        return operation.ErrorCode;
                    }
                }

                bytesReceived = 0;
                receivedFlags = SocketFlags.None;
                return SocketError.IOPending;
            }
        }

        public SocketError ReceiveMessageFrom(byte[] buffer, int offset, int count, ref SocketFlags flags, byte[] socketAddress, ref int socketAddressLen, bool isIPv4, bool isIPv6, int timeout, out IPPacketInformation ipPacketInformation, out int bytesReceived)
        {
            Debug.Assert(timeout == -1 || timeout > 0, $"Unexpected timeout: {timeout}");

            ManualResetEventSlim @event = null;
            try
            {
                ReceiveMessageFromOperation operation;

                lock (_receiveQueue.QueueLock)
                {
                    SocketFlags receivedFlags;
                    SocketError errorCode;
                    if (_receiveQueue.IsEmpty &&
                        SocketPal.TryCompleteReceiveMessageFrom(_socket, buffer, offset, count, flags, socketAddress, ref socketAddressLen, isIPv4, isIPv6, out bytesReceived, out receivedFlags, out ipPacketInformation, out errorCode))
                    {
                        flags = receivedFlags;
                        return errorCode;
                    }

                    @event = new ManualResetEventSlim(false, 0);

                    operation = new ReceiveMessageFromOperation
                    {
                        Event = @event,
                        Buffer = buffer,
                        Offset = offset,
                        Count = count,
                        Flags = flags,
                        SocketAddress = socketAddress,
                        SocketAddressLen = socketAddressLen,
                        IsIPv4 = isIPv4,
                        IsIPv6 = isIPv6,
                    };

                    bool isStopped;
                    while (!TryBeginOperation(ref _receiveQueue, operation, Interop.Sys.SocketEvents.Read, maintainOrder: true, isStopped: out isStopped))
                    {
                        if (isStopped)
                        {
                            socketAddressLen = operation.SocketAddressLen;
                            flags = operation.ReceivedFlags;
                            ipPacketInformation = operation.IPPacketInformation;
                            bytesReceived = operation.BytesTransferred;
                            return SocketError.Interrupted;
                        }

                        if (operation.TryComplete(this))
                        {
                            socketAddressLen = operation.SocketAddressLen;
                            flags = operation.ReceivedFlags;
                            ipPacketInformation = operation.IPPacketInformation;
                            bytesReceived = operation.BytesTransferred;
                            return operation.ErrorCode;
                        }
                    }
                }

                bool signaled = operation.Wait(timeout);
                socketAddressLen = operation.SocketAddressLen;
                flags = operation.ReceivedFlags;
                ipPacketInformation = operation.IPPacketInformation;
                bytesReceived = operation.BytesTransferred;
                return signaled ? operation.ErrorCode : SocketError.TimedOut;
            }
            finally
            {
                if (@event != null) @event.Dispose();
            }
        }

        public SocketError ReceiveMessageFromAsync(byte[] buffer, int offset, int count, SocketFlags flags, byte[] socketAddress, ref int socketAddressLen, bool isIPv4, bool isIPv6, out int bytesReceived, out SocketFlags receivedFlags, out IPPacketInformation ipPacketInformation, Action<int, byte[], int, SocketFlags, IPPacketInformation, SocketError> callback)
        {
            SetNonBlocking();

            lock (_receiveQueue.QueueLock)
            {
                SocketError errorCode;

                if (_receiveQueue.IsEmpty &&
                    SocketPal.TryCompleteReceiveMessageFrom(_socket, buffer, offset, count, flags, socketAddress, ref socketAddressLen, isIPv4, isIPv6, out bytesReceived, out receivedFlags, out ipPacketInformation, out errorCode))
                {
                    // Synchronous success or failure
                    return errorCode;
                }

                var operation = new ReceiveMessageFromOperation
                {
                    Callback = callback,
                    Buffer = buffer,
                    Offset = offset,
                    Count = count,
                    Flags = flags,
                    SocketAddress = socketAddress,
                    SocketAddressLen = socketAddressLen,
                    IsIPv4 = isIPv4,
                    IsIPv6 = isIPv6,
                };

                bool isStopped;
                while (!TryBeginOperation(ref _receiveQueue, operation, Interop.Sys.SocketEvents.Read, maintainOrder: true, isStopped: out isStopped))
                {
                    if (isStopped)
                    {
                        ipPacketInformation = default(IPPacketInformation);
                        bytesReceived = 0;
                        receivedFlags = SocketFlags.None;
                        return SocketError.OperationAborted;
                    }

                    if (operation.TryComplete(this))
                    {
                        socketAddressLen = operation.SocketAddressLen;
                        receivedFlags = operation.ReceivedFlags;
                        ipPacketInformation = operation.IPPacketInformation;
                        bytesReceived = operation.BytesTransferred;
                        return operation.ErrorCode;
                    }
                }

                ipPacketInformation = default(IPPacketInformation);
                bytesReceived = 0;
                receivedFlags = SocketFlags.None;
                return SocketError.IOPending;
            }
        }

        public SocketError Send(byte[] buffer, int offset, int count, SocketFlags flags, int timeout, out int bytesSent)
        {
            return SendTo(buffer, offset, count, flags, null, 0, timeout, out bytesSent);
        }

        public SocketError SendAsync(byte[] buffer, int offset, int count, SocketFlags flags, out int bytesSent, Action<int, byte[], int, SocketFlags, SocketError> callback)
        {
            int socketAddressLen = 0;
            return SendToAsync(buffer, offset, count, flags, null, ref socketAddressLen, out bytesSent, callback);
        }

        public SocketError SendTo(byte[] buffer, int offset, int count, SocketFlags flags, byte[] socketAddress, int socketAddressLen, int timeout, out int bytesSent)
        {
            Debug.Assert(timeout == -1 || timeout > 0, $"Unexpected timeout: {timeout}");

            ManualResetEventSlim @event = null;
            try
            {
                SendOperation operation;

                lock (_sendQueue.QueueLock)
                {
                    bytesSent = 0;
                    SocketError errorCode;

                    if (_sendQueue.IsEmpty &&
                        SocketPal.TryCompleteSendTo(_socket, buffer, ref offset, ref count, flags, socketAddress, socketAddressLen, ref bytesSent, out errorCode))
                    {
                        return errorCode;
                    }

                    @event = new ManualResetEventSlim(false, 0);

                    operation = new SendOperation
                    {
                        Event = @event,
                        Buffer = buffer,
                        Offset = offset,
                        Count = count,
                        Flags = flags,
                        SocketAddress = socketAddress,
                        SocketAddressLen = socketAddressLen,
                        BytesTransferred = bytesSent
                    };

                    bool isStopped;
                    while (!TryBeginOperation(ref _sendQueue, operation, Interop.Sys.SocketEvents.Write, maintainOrder: true, isStopped: out isStopped))
                    {
                        if (isStopped)
                        {
                            bytesSent = operation.BytesTransferred;
                            return SocketError.Interrupted;
                        }

                        if (operation.TryComplete(this))
                        {
                            bytesSent = operation.BytesTransferred;
                            return operation.ErrorCode;
                        }
                    }
                }

                bool signaled = operation.Wait(timeout);
                bytesSent = operation.BytesTransferred;
                return signaled ? operation.ErrorCode : SocketError.TimedOut;
            }
            finally
            {
                if (@event != null) @event.Dispose();
            }
        }

        public SocketError SendToAsync(byte[] buffer, int offset, int count, SocketFlags flags, byte[] socketAddress, ref int socketAddressLen, out int bytesSent, Action<int, byte[], int, SocketFlags, SocketError> callback)
        {
            SetNonBlocking();

            lock (_sendQueue.QueueLock)
            {
                bytesSent = 0;
                SocketError errorCode;

                if (_sendQueue.IsEmpty &&
                    SocketPal.TryCompleteSendTo(_socket, buffer, ref offset, ref count, flags, socketAddress, socketAddressLen, ref bytesSent, out errorCode))
                {
                    // Synchronous success or failure
                    return errorCode;
                }

                var operation = new SendOperation
                {
                    Callback = callback,
                    Buffer = buffer,
                    Offset = offset,
                    Count = count,
                    Flags = flags,
                    SocketAddress = socketAddress,
                    SocketAddressLen = socketAddressLen,
                    BytesTransferred = bytesSent
                };

                bool isStopped;
                while (!TryBeginOperation(ref _sendQueue, operation, Interop.Sys.SocketEvents.Write, maintainOrder: true, isStopped: out isStopped))
                {
                    if (isStopped)
                    {
                        return SocketError.OperationAborted;
                    }

                    if (operation.TryComplete(this))
                    {
                        bytesSent = operation.BytesTransferred;
                        return operation.ErrorCode;
                    }
                }
                
                return SocketError.IOPending;
            }
        }

        public SocketError Send(IList<ArraySegment<byte>> buffers, SocketFlags flags, int timeout, out int bytesSent)
        {
            return SendTo(buffers, flags, null, 0, timeout, out bytesSent);
        }

        public SocketError SendAsync(IList<ArraySegment<byte>> buffers, SocketFlags flags, out int bytesSent, Action<int, byte[], int, SocketFlags, SocketError> callback)
        {
            int socketAddressLen = 0;
            return SendToAsync(buffers, flags, null, ref socketAddressLen, out bytesSent, callback);
        }

        public SocketError SendTo(IList<ArraySegment<byte>> buffers, SocketFlags flags, byte[] socketAddress, int socketAddressLen, int timeout, out int bytesSent)
        {
            Debug.Assert(timeout == -1 || timeout > 0, $"Unexpected timeout: {timeout}");

            ManualResetEventSlim @event = null;
            try
            {
                SendOperation operation;

                lock (_sendQueue.QueueLock)
                {
                    bytesSent = 0;
                    int bufferIndex = 0;
                    int offset = 0;
                    SocketError errorCode;

                    if (_sendQueue.IsEmpty &&
                        SocketPal.TryCompleteSendTo(_socket, buffers, ref bufferIndex, ref offset, flags, socketAddress, socketAddressLen, ref bytesSent, out errorCode))
                    {
                        return errorCode;
                    }

                    @event = new ManualResetEventSlim(false, 0);

                    operation = new SendOperation
                    {
                        Event = @event,
                        Buffers = buffers,
                        BufferIndex = bufferIndex,
                        Offset = offset,
                        Flags = flags,
                        SocketAddress = socketAddress,
                        SocketAddressLen = socketAddressLen,
                        BytesTransferred = bytesSent
                    };

                    bool isStopped;
                    while (!TryBeginOperation(ref _sendQueue, operation, Interop.Sys.SocketEvents.Write, maintainOrder: true, isStopped: out isStopped))
                    {
                        if (isStopped)
                        {
                            bytesSent = operation.BytesTransferred;
                            return SocketError.Interrupted;
                        }

                        if (operation.TryComplete(this))
                        {
                            bytesSent = operation.BytesTransferred;
                            return operation.ErrorCode;
                        }
                    }
                }

                bool signaled = operation.Wait(timeout);
                bytesSent = operation.BytesTransferred;
                return signaled ? operation.ErrorCode : SocketError.TimedOut;
            }
            finally
            {
                if (@event != null) @event.Dispose();
            }
        }



        public SocketError SendToAsync(IList<ArraySegment<byte>> buffers, SocketFlags flags, byte[] socketAddress, ref int socketAddressLen, out int bytesSent, Action<int, byte[], int, SocketFlags, SocketError> callback)
        {
            SetNonBlocking();

            lock (_sendQueue.QueueLock)
            {
                bytesSent = 0;
                int bufferIndex = 0;
                int offset = 0;
                SocketError errorCode;

                if (_sendQueue.IsEmpty &&
                    SocketPal.TryCompleteSendTo(_socket, buffers, ref bufferIndex, ref offset, flags, socketAddress, socketAddressLen, ref bytesSent, out errorCode))
                {
                    // Synchronous success or failure
                    return errorCode;
                }

                var operation = new SendOperation
                {
                    Callback = callback,
                    Buffers = buffers,
                    BufferIndex = bufferIndex,
                    Offset = offset,
                    Flags = flags,
                    SocketAddress = socketAddress,
                    SocketAddressLen = socketAddressLen,
                };

                bool isStopped;
                while (!TryBeginOperation(ref _sendQueue, operation, Interop.Sys.SocketEvents.Write, maintainOrder: true, isStopped: out isStopped))
                {
                    if (isStopped)
                    {
                        return SocketError.OperationAborted;
                    }

                    if (operation.TryComplete(this))
                    {
                        bytesSent = operation.BytesTransferred;
                        return operation.ErrorCode;
                    }
                }

                return SocketError.IOPending;
            }
        }

        public SocketError SendFile(SafeFileHandle fileHandle, long offset, long count, int timeout, out long bytesSent)
        {
            Debug.Assert(timeout == -1 || timeout > 0, $"Unexpected timeout: {timeout}");

            ManualResetEventSlim @event = null;
            try
            {
                SendFileOperation operation;

                lock (_sendQueue.QueueLock)
                {
                    bytesSent = 0;
                    SocketError errorCode;

                    if (_sendQueue.IsEmpty &&
                        SocketPal.TryCompleteSendFile(_socket, fileHandle, ref offset, ref count, ref bytesSent, out errorCode))
                    {
                        return errorCode;
                    }

                    @event = new ManualResetEventSlim(false, 0);

                    operation = new SendFileOperation
                    {
                        Event = @event,
                        FileHandle = fileHandle,
                        Offset = offset,
                        Count = count,
                        BytesTransferred = bytesSent
                    };

                    bool isStopped;
                    while (!TryBeginOperation(ref _sendQueue, operation, Interop.Sys.SocketEvents.Write, maintainOrder: true, isStopped: out isStopped))
                    {
                        if (isStopped)
                        {
                            bytesSent = operation.BytesTransferred;
                            return SocketError.Interrupted;
                        }

                        if (operation.TryComplete(this))
                        {
                            bytesSent = operation.BytesTransferred;
                            return operation.ErrorCode;
                        }
                    }
                }

                bool signaled = operation.Wait(timeout);
                bytesSent = operation.BytesTransferred;
                return signaled ? operation.ErrorCode : SocketError.TimedOut;
            }
            finally
            {
                if (@event != null) @event.Dispose();
            }
        }

        public SocketError SendFileAsync(SafeFileHandle fileHandle, long offset, long count, out long bytesSent, Action<long, SocketError> callback)
        {
            SetNonBlocking();

            lock (_sendQueue.QueueLock)
            {
                bytesSent = 0;
                SocketError errorCode;

                if (_sendQueue.IsEmpty &&
                    SocketPal.TryCompleteSendFile(_socket, fileHandle, ref offset, ref count, ref bytesSent, out errorCode))
                {
                    // Synchronous success or failure
                    return errorCode;
                }

                var operation = new SendFileOperation
                {
                    Callback = callback,
                    FileHandle = fileHandle,
                    Offset = offset,
                    Count = count,
                    BytesTransferred = bytesSent
                };

                bool isStopped;
                while (!TryBeginOperation(ref _sendQueue, operation, Interop.Sys.SocketEvents.Write, maintainOrder: true, isStopped: out isStopped))
                {
                    if (isStopped)
                    {
                        return SocketError.OperationAborted;
                    }

                    if (operation.TryComplete(this))
                    {
                        bytesSent = operation.BytesTransferred;
                        return operation.ErrorCode;
                    }
                }

                return SocketError.IOPending;
            }
        }

        public unsafe void HandleEvents(Interop.Sys.SocketEvents events)
        {
            if ((events & Interop.Sys.SocketEvents.Error) != 0)
            {
                // Set the Read and Write flags as well; the processing for these events
                // will pick up the error.
                events |= Interop.Sys.SocketEvents.Read | Interop.Sys.SocketEvents.Write;
            }

            if ((events & Interop.Sys.SocketEvents.Read) != 0)
            {
                _receiveQueue.Complete(this);
            }

            if ((events & Interop.Sys.SocketEvents.Write) != 0)
            {
                _sendQueue.Complete(this);
            }
        }
    }
}
