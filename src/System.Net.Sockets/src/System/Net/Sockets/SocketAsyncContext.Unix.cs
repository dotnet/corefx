// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;

namespace System.Net.Sockets
{
    // TODO:
    // - Plumb status through async APIs to avoid callbacks on synchronous completion
    //     - NOTE: this will require refactoring in the *Async APIs to accommodate the lack
    //             of completion posting
    // - Add support for unregistering + reregistering for events
    //     - This will require a new state for each queue, unregistred, to track whether or
    //       not the queue is currently registered to receive events
    // - Audit Close()-related code for the possibility of file descriptor recycling issues
    //     - It might make sense to change _closeLock to a ReaderWriterLockSlim that is
    //       acquired for read by all public methods before attempting a completion and
    //       acquired for write by Close() and HandlEvents()
    // - Audit event-related code for the possibility of GCHandle recycling issues
    //     - There is a potential issue with handle recycling in event loop processing
    //       if the processing of an event races with the close of a GCHandle
    //     - It may be necessary for the event loop thread to do all file descriptor
    //       unregistration in order to avoid this. If so, this would probably happen
    //       by adding a flag that indicates that the event loop is processing events and
    //       a queue of contexts to unregister once processing completes.
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

            public void QueueCompletionCallback()
            {
                Debug.Assert(!(CallbackOrEvent is ManualResetEventSlim));
                Debug.Assert(_state != (int)State.Cancelled);
#if DEBUG
                Debug.Assert(Interlocked.CompareExchange(ref _callbackQueued, 1, 0) == 0);
#endif

                ThreadPool.QueueUserWorkItem(o => ((AsyncOperation)o).InvokeCallback(), this);
            }

            public bool TryComplete(int fileDescriptor)
            {
                Debug.Assert(_state == (int)State.Waiting);

                return DoTryComplete(fileDescriptor);
            }

            public bool TryCompleteAsync(int fileDescriptor)
            {
                return TryCompleteOrAbortAsync(fileDescriptor, abort: false);
            }

            public void AbortAsync()
            {
                bool completed = TryCompleteOrAbortAsync(fileDescriptor: -1, abort: true);
                Debug.Assert(completed);
            }

            private bool TryCompleteOrAbortAsync(int fileDescriptor, bool abort)
            {
                int state = Interlocked.CompareExchange(ref _state, (int)State.Running, (int)State.Waiting);
                if (state == (int)State.Cancelled)
                {
                    // This operation has been cancelled. The canceller is responsible for
                    // correctly updating any state that would have been handled by
                    // AsyncOperation.Abort.
                    return true;
                }

                Debug.Assert(state != (int)State.Complete && state != (int)State.Running);

                bool completed;
                if (abort)
                {
                    Abort();
                    ErrorCode = SocketError.OperationAborted;
                    completed = true;
                }
                else
                {
                    completed = DoTryComplete(fileDescriptor);
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
                        QueueCompletionCallback();
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

            protected abstract bool DoTryComplete(int fileDescriptor);

            protected abstract void InvokeCallback();
        }

        private abstract class TransferOperation : AsyncOperation
        {
            public byte[] Buffer;
            public int Offset;
            public int Count;
            public SocketFlags Flags;
            public int BytesTransferred;
            public SocketFlags ReceivedFlags;

            protected sealed override void Abort() { }
        }

        private abstract class SendReceiveOperation : TransferOperation
        {
            public IList<ArraySegment<byte>> Buffers;
            public int BufferIndex;

            public Action<int, byte[], int, SocketFlags, SocketError> Callback
            {
                private get { return (Action<int, byte[], int, SocketFlags, SocketError>)CallbackOrEvent; }
                set { CallbackOrEvent = value; }
            }

            protected sealed override void InvokeCallback()
            {
                Callback(BytesTransferred, SocketAddress, SocketAddressLen, ReceivedFlags, ErrorCode);
            }
        }

        private sealed class SendOperation : SendReceiveOperation
        {
            protected override bool DoTryComplete(int fileDescriptor)
            {
                return SocketPal.TryCompleteSendTo(fileDescriptor, Buffer, Buffers, ref BufferIndex, ref Offset, ref Count, Flags, SocketAddress, SocketAddressLen, ref BytesTransferred, out ErrorCode);
            }
        }

        private sealed class ReceiveOperation : SendReceiveOperation
        {
            protected override bool DoTryComplete(int fileDescriptor)
            {
                return SocketPal.TryCompleteReceiveFrom(fileDescriptor, Buffer, Buffers, Offset, Count, Flags, SocketAddress, ref SocketAddressLen, out BytesTransferred, out ReceivedFlags, out ErrorCode);
            }
        }

        private sealed class ReceiveMessageFromOperation : TransferOperation
        {
            public bool IsIPv4;
            public bool IsIPv6;
            public IPPacketInformation IPPacketInformation;

            public Action<int, byte[], int, SocketFlags, IPPacketInformation, SocketError> Callback
            {
                private get { return (Action<int, byte[], int, SocketFlags, IPPacketInformation, SocketError>)CallbackOrEvent; }
                set { CallbackOrEvent = value; }
            }

            protected override bool DoTryComplete(int fileDescriptor)
            {
                return SocketPal.TryCompleteReceiveMessageFrom(fileDescriptor, Buffer, Offset, Count, Flags, SocketAddress, ref SocketAddressLen, IsIPv4, IsIPv6, out BytesTransferred, out ReceivedFlags, out IPPacketInformation, out ErrorCode);
            }

            protected override void InvokeCallback()
            {
                Callback(BytesTransferred, SocketAddress, SocketAddressLen, ReceivedFlags, IPPacketInformation, ErrorCode);
            }
        }

        private abstract class AcceptOrConnectOperation : AsyncOperation
        {
        }

        private sealed class AcceptOperation : AcceptOrConnectOperation
        {
            public int AcceptedFileDescriptor;

            public Action<int, byte[], int, SocketError> Callback
            {
                private get { return (Action<int, byte[], int, SocketError>)CallbackOrEvent; }
                set { CallbackOrEvent = value; }
            }

            protected override void Abort()
            {
                AcceptedFileDescriptor = -1;
            }

            protected override bool DoTryComplete(int fileDescriptor)
            {
                bool completed = SocketPal.TryCompleteAccept(fileDescriptor, SocketAddress, ref SocketAddressLen, out AcceptedFileDescriptor, out ErrorCode);
                Debug.Assert(ErrorCode == SocketError.Success || AcceptedFileDescriptor == -1);
                return completed;
            }

            protected override void InvokeCallback()
            {
                Callback(AcceptedFileDescriptor, SocketAddress, SocketAddressLen, ErrorCode);
            }
        }

        private sealed class ConnectOperation : AcceptOrConnectOperation
        {
            public Action<SocketError> Callback
            {
                private get { return (Action<SocketError>)CallbackOrEvent; }
                set { CallbackOrEvent = value; }
            }

            protected override void Abort() { }

            protected override bool DoTryComplete(int fileDescriptor)
            {
                return SocketPal.TryCompleteConnect(fileDescriptor, SocketAddressLen, out ErrorCode);
            }

            protected override void InvokeCallback()
            {
                Callback(ErrorCode);
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
            private AsyncOperation _tail;

            public QueueState State { get; set; }
            public bool IsStopped { get { return State == QueueState.Stopped; } }
            public bool IsEmpty { get { return _tail == null; } }

            public TOperation Head
            {
                get
                {
                    Debug.Assert(!IsStopped);
                    return (TOperation)_tail.Next;
                }
            }

            public TOperation Tail
            {
                get
                {
                    return (TOperation)_tail;
                }
            }

            public void Enqueue(TOperation operation)
            {
                Debug.Assert(!IsStopped);
                Debug.Assert(operation.Next == operation);

                if (!IsEmpty)
                {
                    operation.Next = _tail.Next;
                    _tail.Next = operation;
                }

                _tail = operation;
            }

            public void Dequeue()
            {
                Debug.Assert(!IsStopped);
                Debug.Assert(!IsEmpty);

                AsyncOperation head = _tail.Next;
                if (head == _tail)
                {
                    _tail = null;
                }
                else
                {
                    _tail.Next = head.Next;
                }
            }

            public OperationQueue<TOperation> Stop()
            {
                OperationQueue<TOperation> result = this;
                _tail = null;
                State = QueueState.Stopped;
                return result;
            }
        }

        private int _fileDescriptor;
        private GCHandle _handle;
        private OperationQueue<TransferOperation> _receiveQueue;
        private OperationQueue<SendOperation> _sendQueue;
        private OperationQueue<AcceptOrConnectOperation> _acceptOrConnectQueue;
        private SocketAsyncEngine _engine;
        private Interop.Sys.SocketEvents _registeredEvents;

        // These locks are hierarchical: _closeLock must be acquired before _queueLock in order
        // to prevent deadlock.
        private object _closeLock = new object();
        private object _queueLock = new object();

        public SocketAsyncContext(int fileDescriptor, SocketAsyncEngine engine)
        {
            _fileDescriptor = fileDescriptor;
            _engine = engine;
        }

        private void Register(Interop.Sys.SocketEvents events)
        {
            Debug.Assert(Monitor.IsEntered(_queueLock));
            Debug.Assert(!_handle.IsAllocated || _registeredEvents != Interop.Sys.SocketEvents.None);
            Debug.Assert((_registeredEvents & events) == Interop.Sys.SocketEvents.None);

            if (_registeredEvents == Interop.Sys.SocketEvents.None)
            {
                Debug.Assert(!_handle.IsAllocated);
                _handle = GCHandle.Alloc(this, GCHandleType.Normal);
            }

            events |= _registeredEvents;

            Interop.Error errorCode;
            if (!_engine.TryRegister(_fileDescriptor, _registeredEvents, events, _handle, out errorCode))
            {
                if (_registeredEvents == Interop.Sys.SocketEvents.None)
                {
                    _handle.Free();
                }

                // TODO: throw an appropiate exception
                throw new Exception(string.Format("SocketAsyncContext.Register: {0}", errorCode));
            }

            _registeredEvents = events;
        }

        private void UnregisterRead()
        {
            Debug.Assert(Monitor.IsEntered(_queueLock));
            Debug.Assert((_registeredEvents & Interop.Sys.SocketEvents.Read) != Interop.Sys.SocketEvents.None);

            Interop.Sys.SocketEvents events = _registeredEvents & ~Interop.Sys.SocketEvents.Read;
            if (events == Interop.Sys.SocketEvents.None)
            {
                Unregister();
            }
            else
            {
                Interop.Error errorCode;
                bool unregistered = _engine.TryRegister(_fileDescriptor, _registeredEvents, events, _handle, out errorCode);
                if (unregistered)
                {
                    _registeredEvents = events;
                }
                else
                {
                    Debug.Fail(string.Format("UnregisterRead failed: {0}", errorCode));
                }
            }
        }

        private void Unregister()
        {
            Debug.Assert(Monitor.IsEntered(_queueLock));

            if (_registeredEvents == Interop.Sys.SocketEvents.None)
            {
                Debug.Assert(!_handle.IsAllocated);
                return;
            }

            Interop.Error errorCode;
            bool unregistered = _engine.TryRegister(_fileDescriptor, _registeredEvents, Interop.Sys.SocketEvents.None, _handle, out errorCode);
            _registeredEvents = (Interop.Sys.SocketEvents)(-1);
            if (unregistered)
            {
                _registeredEvents = Interop.Sys.SocketEvents.None;
                _handle.Free();
            }
            else
            {
                Debug.Fail(string.Format("Unregister failed: {0}", errorCode));
            }
        }

        private void CloseInner()
        {
            Debug.Assert(Monitor.IsEntered(_closeLock) && !Monitor.IsEntered(_queueLock));

            OperationQueue<AcceptOrConnectOperation> acceptOrConnectQueue;
            OperationQueue<SendOperation> sendQueue;
            OperationQueue<TransferOperation> receiveQueue;

            lock (_queueLock)
            {
                // Drain queues and unregister events

                acceptOrConnectQueue = _acceptOrConnectQueue.Stop();
                sendQueue = _sendQueue.Stop();
                receiveQueue = _receiveQueue.Stop();

                Unregister();

                // TODO: assert that queues are all empty if _registeredEvents was Interop.Sys.SocketEvents.None?
            }

            // TODO: the error codes on these operations may need to be changed to account for
            //       the close. I think Winsock returns OperationAborted in the case that
            //       the socket for an outstanding operation is closed.

            Debug.Assert(!acceptOrConnectQueue.IsStopped || acceptOrConnectQueue.IsEmpty);
            while (!acceptOrConnectQueue.IsEmpty)
            {
                AcceptOrConnectOperation op = acceptOrConnectQueue.Head;
                op.AbortAsync();
                acceptOrConnectQueue.Dequeue();
            }

            Debug.Assert(!sendQueue.IsStopped || sendQueue.IsEmpty);
            while (!sendQueue.IsEmpty)
            {
                SendReceiveOperation op = sendQueue.Head;
                op.AbortAsync();
                sendQueue.Dequeue();
            }

            Debug.Assert(!receiveQueue.IsStopped || receiveQueue.IsEmpty);
            while (!receiveQueue.IsEmpty)
            {
                TransferOperation op = receiveQueue.Head;
                op.AbortAsync();
                receiveQueue.Dequeue();
            }
        }

        public void Close()
        {
            Debug.Assert(!Monitor.IsEntered(_queueLock));

            lock (_closeLock)
            {
                CloseInner();
            }
        }

        private bool TryBeginOperation<TOperation>(ref OperationQueue<TOperation> queue, TOperation operation, Interop.Sys.SocketEvents events, out bool isStopped)
            where TOperation : AsyncOperation
        {
            lock (_queueLock)
            {
                switch (queue.State)
                {
                    case QueueState.Stopped:
                        isStopped = true;
                        return false;

                    case QueueState.Clear:
                        break;

                    case QueueState.Set:
                        isStopped = false;
                        queue.State = QueueState.Clear;
                        return false;
                }

                if ((_registeredEvents & events) == Interop.Sys.SocketEvents.None)
                {
                    Register(events);
                }

                queue.Enqueue(operation);
                isStopped = false;
                return true;
            }
        }

        private void EndOperation<TOperation>(ref OperationQueue<TOperation> queue)
            where TOperation : AsyncOperation
        {
            lock (_queueLock)
            {
                Debug.Assert(!queue.IsStopped);

                queue.Dequeue();
            }
        }

        public SocketError Accept(byte[] socketAddress, ref int socketAddressLen, int timeout, out int acceptedFd)
        {
            Debug.Assert(socketAddress != null);
            Debug.Assert(socketAddressLen > 0);
            Debug.Assert(timeout == -1 || timeout > 0);

            SocketError errorCode;
            if (SocketPal.TryCompleteAccept(_fileDescriptor, socketAddress, ref socketAddressLen, out acceptedFd, out errorCode))
            {
                Debug.Assert(errorCode == SocketError.Success || acceptedFd == -1);
                return errorCode;
            }

            using (var @event = new ManualResetEventSlim())
            {
                var operation = new AcceptOperation {
                    Event = @event,
                    SocketAddress = socketAddress,
                    SocketAddressLen = socketAddressLen
                };

                bool isStopped;
                while (!TryBeginOperation(ref _acceptOrConnectQueue, operation, Interop.Sys.SocketEvents.Read, out isStopped))
                {
                    if (isStopped)
                    {
                        // TODO: is this error reasonable for a closed socket? Check with Winsock.
                        acceptedFd = -1;
                        return SocketError.Shutdown;
                    }

                    if (operation.TryComplete(_fileDescriptor))
                    {
                        socketAddressLen = operation.SocketAddressLen;
                        acceptedFd = operation.AcceptedFileDescriptor;
                        return operation.ErrorCode;
                    }
                }

                if (!operation.Wait(timeout))
                {
                    acceptedFd = -1;
                    return SocketError.TimedOut;
                }

                socketAddressLen = operation.SocketAddressLen;
                acceptedFd = operation.AcceptedFileDescriptor;
                return operation.ErrorCode;
            }
        }

        public SocketError AcceptAsync(byte[] socketAddress, int socketAddressLen, Action<int, byte[], int, SocketError> callback)
        {
            Debug.Assert(socketAddress != null);
            Debug.Assert(socketAddressLen > 0);
            Debug.Assert(callback != null);

            int acceptedFd;
            SocketError errorCode;
            if (SocketPal.TryCompleteAccept(_fileDescriptor, socketAddress, ref socketAddressLen, out acceptedFd, out errorCode))
            {
                Debug.Assert(errorCode == SocketError.Success || acceptedFd == -1);

                if (errorCode == SocketError.Success)
                {
                    ThreadPool.QueueUserWorkItem(args =>
                    {
                        var tup = (Tuple<Action<int, byte[], int, SocketError>, int, byte[], int>)args;
                        tup.Item1(tup.Item2, tup.Item3, tup.Item4, SocketError.Success);
                    }, Tuple.Create(callback, acceptedFd, socketAddress, socketAddressLen));
                }
                return errorCode;
            }

            var operation = new AcceptOperation {
                Callback = callback,
                SocketAddress = socketAddress,
                SocketAddressLen = socketAddressLen
            };

            bool isStopped;
            while (!TryBeginOperation(ref _acceptOrConnectQueue, operation, Interop.Sys.SocketEvents.Read, out isStopped))
            {
                if (isStopped)
                {
                    // TODO: is this error reasonable for a closed socket? Check with Winsock.
                    operation.AcceptedFileDescriptor = -1;
                    operation.ErrorCode = SocketError.Shutdown;
                    operation.QueueCompletionCallback();
                    return SocketError.Shutdown;
                }

                if (operation.TryComplete(_fileDescriptor))
                {
                    operation.QueueCompletionCallback();
                    break;
                }
            }
            return SocketError.IOPending;
        }

        public SocketError Connect(byte[] socketAddress, int socketAddressLen, int timeout)
        {
            Debug.Assert(socketAddress != null);
            Debug.Assert(socketAddressLen > 0);
            Debug.Assert(timeout == -1 || timeout > 0);

            SocketError errorCode;
            if (SocketPal.TryStartConnect(_fileDescriptor, socketAddress, socketAddressLen, out errorCode))
            {
                return errorCode;
            }

            using (var @event = new ManualResetEventSlim())
            {
                var operation = new ConnectOperation {
                    Event = @event,
                    SocketAddress = socketAddress,
                    SocketAddressLen = socketAddressLen
                };

                bool isStopped;
                while (!TryBeginOperation(ref _acceptOrConnectQueue, operation, Interop.Sys.SocketEvents.Write, out isStopped))
                {
                    if (isStopped)
                    {
                        // TODO: is this error reasonable for a closed socket? Check with Winsock.
                        return SocketError.Shutdown;
                    }

                    if (operation.TryComplete(_fileDescriptor))
                    {
                        return operation.ErrorCode;
                    }
                }

                return operation.Wait(timeout) ? operation.ErrorCode : SocketError.TimedOut;
            }
        }

        public SocketError ConnectAsync(byte[] socketAddress, int socketAddressLen, Action<SocketError> callback)
        {
            Debug.Assert(socketAddress != null);
            Debug.Assert(socketAddressLen > 0);
            Debug.Assert(callback != null);

            SocketError errorCode;
            if (SocketPal.TryStartConnect(_fileDescriptor, socketAddress, socketAddressLen, out errorCode))
            {
                if (errorCode == SocketError.Success)
                {
                    ThreadPool.QueueUserWorkItem(arg => ((Action<SocketError>)arg)(SocketError.Success), callback);
                }
                return errorCode;
            }

            var operation = new ConnectOperation {
                Callback = callback,
                SocketAddress = socketAddress,
                SocketAddressLen = socketAddressLen
            };

            bool isStopped;
            while (!TryBeginOperation(ref _acceptOrConnectQueue, operation, Interop.Sys.SocketEvents.Write, out isStopped))
            {
                if (isStopped)
                {
                    // TODO: is this error code reasonable for a closed socket? Check with Winsock.
                    operation.ErrorCode = SocketError.Shutdown;
                    operation.QueueCompletionCallback();
                    return SocketError.Shutdown;
                }

                if (operation.TryComplete(_fileDescriptor))
                {
                    operation.QueueCompletionCallback();
                    break;
                }
            }
            return SocketError.IOPending;
        }

        public SocketError Receive(byte[] buffer, int offset, int count, ref SocketFlags flags, int timeout, out int bytesReceived)
        {
            int socketAddressLen = 0;
            return ReceiveFrom(buffer, offset, count, ref flags, null, ref socketAddressLen, timeout, out bytesReceived);
        }

        public SocketError ReceiveAsync(byte[] buffer, int offset, int count, SocketFlags flags, Action<int, byte[], int, SocketFlags, SocketError> callback)
        {
            return ReceiveFromAsync(buffer, offset, count, flags, null, 0, callback);
        }

        public SocketError ReceiveFrom(byte[] buffer, int offset, int count, ref SocketFlags flags, byte[] socketAddress, ref int socketAddressLen, int timeout, out int bytesReceived)
        {
            Debug.Assert(timeout == -1 || timeout > 0);

            SocketFlags receivedFlags;
            SocketError errorCode;
            if (SocketPal.TryCompleteReceiveFrom(_fileDescriptor, buffer, offset, count, flags, socketAddress, ref socketAddressLen, out bytesReceived, out receivedFlags, out errorCode))
            {
                flags = receivedFlags;
                return errorCode;
            }

            using (var @event = new ManualResetEventSlim())
            {
                var operation = new ReceiveOperation {
                    Event = @event,
                    Buffer = buffer,
                    Offset = offset,
                    Count = count,
                    Flags = flags,
                    SocketAddress = socketAddress,
                    SocketAddressLen = socketAddressLen,
                    BytesTransferred = bytesReceived,
                    ReceivedFlags = receivedFlags
                };

                bool isStopped;
                while (!TryBeginOperation(ref _receiveQueue, operation, Interop.Sys.SocketEvents.Read, out isStopped))
                {
                    if (isStopped)
                    {
                        // TODO: is this error code reasonable for a closed socket? Check with Winsock.
                        flags = operation.ReceivedFlags;
                        bytesReceived = operation.BytesTransferred;
                        return SocketError.Shutdown;
                    }

                    if (operation.TryComplete(_fileDescriptor))
                    {
                        socketAddressLen = operation.SocketAddressLen;
                        flags = operation.ReceivedFlags;
                        bytesReceived = operation.BytesTransferred;
                        return operation.ErrorCode;
                    }
                }

                bool signaled = operation.Wait(timeout);
                socketAddressLen = operation.SocketAddressLen;
                flags = operation.ReceivedFlags;
                bytesReceived = operation.BytesTransferred;
                return signaled ? operation.ErrorCode : SocketError.TimedOut;
            }
        }

        public SocketError ReceiveFromAsync(byte[] buffer, int offset, int count, SocketFlags flags, byte[] socketAddress, int socketAddressLen, Action<int, byte[], int, SocketFlags, SocketError> callback)
        {
            int bytesReceived;
            SocketFlags receivedFlags;
            SocketError errorCode;
            if (SocketPal.TryCompleteReceiveFrom(_fileDescriptor, buffer, offset, count, flags, socketAddress, ref socketAddressLen, out bytesReceived, out receivedFlags, out errorCode))
            {
                if (errorCode == SocketError.Success)
                {
                    ThreadPool.QueueUserWorkItem(args =>
                    {
                        var tup = (Tuple<Action<int, byte[], int, SocketFlags, SocketError>, int, byte[], int, SocketFlags>)args;
                        tup.Item1(tup.Item2, tup.Item3, tup.Item4, tup.Item5, SocketError.Success);
                    }, Tuple.Create(callback, bytesReceived, socketAddress, socketAddressLen, receivedFlags));
                }
                return errorCode;
            }

            var operation = new ReceiveOperation {
                Callback = callback,
                Buffer = buffer,
                Offset = offset,
                Count = count,
                Flags = flags,
                SocketAddress = socketAddress,
                SocketAddressLen = socketAddressLen,
                BytesTransferred = bytesReceived,
                ReceivedFlags = receivedFlags
            };

            bool isStopped;
            while (!TryBeginOperation(ref _receiveQueue, operation, Interop.Sys.SocketEvents.Read, out isStopped))
            {
                if (isStopped)
                {
                    // TODO: is this error code reasonable for a closed socket? Check with Winsock.
                    operation.ErrorCode = SocketError.Shutdown;
                    operation.QueueCompletionCallback();
                    return SocketError.Shutdown;
                }

                if (operation.TryComplete(_fileDescriptor))
                {
                    operation.QueueCompletionCallback();
                    break;
                }
            }
            return SocketError.IOPending;
        }

        public SocketError Receive(IList<ArraySegment<byte>> buffers, ref SocketFlags flags, int timeout, out int bytesReceived)
        {
            return ReceiveFrom(buffers, ref flags, null, 0, timeout, out bytesReceived);
        }

        public SocketError ReceiveAsync(IList<ArraySegment<byte>> buffers, SocketFlags flags, Action<int, byte[], int, SocketFlags, SocketError> callback)
        {
            return ReceiveFromAsync(buffers, flags, null, 0, callback);
        }

        public SocketError ReceiveFrom(IList<ArraySegment<byte>> buffers, ref SocketFlags flags, byte[] socketAddress, int socketAddressLen, int timeout, out int bytesReceived)
        {
            Debug.Assert(timeout == -1 || timeout > 0);

            SocketFlags receivedFlags;
            SocketError errorCode;
            if (SocketPal.TryCompleteReceiveFrom(_fileDescriptor, buffers, flags, socketAddress, ref socketAddressLen, out bytesReceived, out receivedFlags, out errorCode))
            {
                flags = receivedFlags;
                return errorCode;
            }

            using (var @event = new ManualResetEventSlim())
            {
                var operation = new ReceiveOperation {
                    Event = @event,
                    Buffers = buffers,
                    Flags = flags,
                    SocketAddress = socketAddress,
                    SocketAddressLen = socketAddressLen,
                    BytesTransferred = bytesReceived,
                    ReceivedFlags = receivedFlags
                };

                bool isStopped;
                while (!TryBeginOperation(ref _receiveQueue, operation, Interop.Sys.SocketEvents.Read, out isStopped))
                {
                    if (isStopped)
                    {
                        // TODO: is this error code reasonable for a closed socket? Check with Winsock.
                        flags = operation.ReceivedFlags;
                        bytesReceived = operation.BytesTransferred;
                        return SocketError.Shutdown;
                    }

                    if (operation.TryComplete(_fileDescriptor))
                    {
                        socketAddressLen = operation.SocketAddressLen;
                        flags = operation.ReceivedFlags;
                        bytesReceived = operation.BytesTransferred;
                        return operation.ErrorCode;
                    }
                }

                bool signaled = operation.Wait(timeout);
                socketAddressLen = operation.SocketAddressLen;
                flags = operation.ReceivedFlags;
                bytesReceived = operation.BytesTransferred;
                return signaled ? operation.ErrorCode : SocketError.TimedOut;
            }
        }

        public SocketError ReceiveFromAsync(IList<ArraySegment<byte>> buffers, SocketFlags flags, byte[] socketAddress, int socketAddressLen, Action<int, byte[], int, SocketFlags, SocketError> callback)
        {
            int bytesReceived;
            SocketFlags receivedFlags;
            SocketError errorCode;
            if (SocketPal.TryCompleteReceiveFrom(_fileDescriptor, buffers, flags, socketAddress, ref socketAddressLen, out bytesReceived, out receivedFlags, out errorCode))
            {
                if (errorCode == SocketError.Success)
                {
                    ThreadPool.QueueUserWorkItem(args =>
                    {
                        var tup = (Tuple<Action<int, byte[], int, SocketFlags, SocketError>, int, byte[], int, SocketFlags>)args;
                        tup.Item1(tup.Item2, tup.Item3, tup.Item4, tup.Item5, SocketError.Success);
                    }, Tuple.Create(callback, bytesReceived, socketAddress, socketAddressLen, receivedFlags));
                }
                return errorCode;
            }

            var operation = new ReceiveOperation {
                Callback = callback,
                Buffers = buffers,
                Flags = flags,
                SocketAddress = socketAddress,
                SocketAddressLen = socketAddressLen,
                BytesTransferred = bytesReceived,
                ReceivedFlags = receivedFlags
            };

            bool isStopped;
            while (!TryBeginOperation(ref _receiveQueue, operation, Interop.Sys.SocketEvents.Read, out isStopped))
            {
                if (isStopped)
                {
                    // TODO: is this error code reasonable for a closed socket? Check with Winsock.
                    operation.ErrorCode = SocketError.Shutdown;
                    operation.QueueCompletionCallback();
                    return SocketError.Shutdown;
                }

                if (operation.TryComplete(_fileDescriptor))
                {
                    operation.QueueCompletionCallback();
                    break;
                }
            }
            return SocketError.IOPending;
        }

        public SocketError ReceiveMessageFrom(byte[] buffer, int offset, int count, ref SocketFlags flags, byte[] socketAddress, ref int socketAddressLen, bool isIPv4, bool isIPv6, int timeout, out IPPacketInformation ipPacketInformation, out int bytesReceived)
        {
            Debug.Assert(timeout == -1 || timeout > 0);

            SocketFlags receivedFlags;
            SocketError errorCode;
            if (SocketPal.TryCompleteReceiveMessageFrom(_fileDescriptor, buffer, offset, count, flags, socketAddress, ref socketAddressLen, isIPv4, isIPv6, out bytesReceived, out receivedFlags, out ipPacketInformation, out errorCode))
            {
                flags = receivedFlags;
                return errorCode;
            }

            using (var @event = new ManualResetEventSlim())
            {
                var operation = new ReceiveMessageFromOperation {
                    Event = @event,
                    Buffer = buffer,
                    Offset = offset,
                    Count = count,
                    Flags = flags,
                    SocketAddress = socketAddress,
                    SocketAddressLen = socketAddressLen,
                    IsIPv4 = isIPv4,
                    IsIPv6 = isIPv6,
                    BytesTransferred = bytesReceived,
                    ReceivedFlags = receivedFlags,
                    IPPacketInformation = ipPacketInformation,
                };

                bool isStopped;
                while (!TryBeginOperation(ref _receiveQueue, operation, Interop.Sys.SocketEvents.Read, out isStopped))
                {
                    if (isStopped)
                    {
                        // TODO: is this error code reasonable for a closed socket? Check with Winsock.
                        socketAddressLen = operation.SocketAddressLen;
                        flags = operation.ReceivedFlags;
                        ipPacketInformation = operation.IPPacketInformation;
                        bytesReceived = operation.BytesTransferred;
                        return SocketError.Shutdown;
                    }

                    if (operation.TryComplete(_fileDescriptor))
                    {
                        socketAddressLen = operation.SocketAddressLen;
                        flags = operation.ReceivedFlags;
                        ipPacketInformation = operation.IPPacketInformation;
                        bytesReceived = operation.BytesTransferred;
                        return operation.ErrorCode;
                    }
                }

                bool signaled = operation.Wait(timeout);
                socketAddressLen = operation.SocketAddressLen;
                flags = operation.ReceivedFlags;
                ipPacketInformation = operation.IPPacketInformation;
                bytesReceived = operation.BytesTransferred;
                return signaled ? operation.ErrorCode : SocketError.TimedOut;
            }
        }

        public SocketError ReceiveMessageFromAsync(byte[] buffer, int offset, int count, SocketFlags flags, byte[] socketAddress, int socketAddressLen, bool isIPv4, bool isIPv6, Action<int, byte[], int, SocketFlags, IPPacketInformation, SocketError> callback)
        {
            int bytesReceived;
            SocketFlags receivedFlags;
            IPPacketInformation ipPacketInformation;
            SocketError errorCode;
            if (SocketPal.TryCompleteReceiveMessageFrom(_fileDescriptor, buffer, offset, count, flags, socketAddress, ref socketAddressLen, isIPv4, isIPv6, out bytesReceived, out receivedFlags, out ipPacketInformation, out errorCode))
            {
                if (errorCode == SocketError.Success)
                {
                    ThreadPool.QueueUserWorkItem(args =>
                    {
                        var tup = (Tuple<Action<int, byte[], int, SocketFlags, IPPacketInformation, SocketError>, int, byte[], int, SocketFlags, IPPacketInformation>)args;
                        tup.Item1(tup.Item2, tup.Item3, tup.Item4, tup.Item5, tup.Item6, SocketError.Success);
                    }, Tuple.Create(callback, bytesReceived, socketAddress, socketAddressLen, receivedFlags, ipPacketInformation));
                }
                return errorCode;
            }

            var operation = new ReceiveMessageFromOperation {
                Callback = callback,
                Buffer = buffer,
                Offset = offset,
                Count = count,
                Flags = flags,
                SocketAddress = socketAddress,
                SocketAddressLen = socketAddressLen,
                IsIPv4 = isIPv4,
                IsIPv6 = isIPv6,
                BytesTransferred = bytesReceived,
                ReceivedFlags = receivedFlags,
                IPPacketInformation = ipPacketInformation,
            };

            bool isStopped;
            while (!TryBeginOperation(ref _receiveQueue, operation, Interop.Sys.SocketEvents.Read, out isStopped))
            {
                if (isStopped)
                {
                    // TODO: is this error code reasonable for a closed socket? Check with Winsock.
                    operation.ErrorCode = SocketError.Shutdown;
                    operation.QueueCompletionCallback();
                    return SocketError.Shutdown;
                }

                if (operation.TryComplete(_fileDescriptor))
                {
                    operation.QueueCompletionCallback();
                    break;
                }
            }
            return SocketError.IOPending;
        }

        public SocketError Send(byte[] buffer, int offset, int count, SocketFlags flags, int timeout, out int bytesSent)
        {
            return SendTo(buffer, offset, count, flags, null, 0, timeout, out bytesSent);
        }

        public SocketError SendAsync(byte[] buffer, int offset, int count, SocketFlags flags, Action<int, byte[], int, SocketFlags, SocketError> callback)
        {
            return SendToAsync(buffer, offset, count, flags, null, 0, callback);
        }

        public SocketError SendTo(byte[] buffer, int offset, int count, SocketFlags flags, byte[] socketAddress, int socketAddressLen, int timeout, out int bytesSent)
        {
            Debug.Assert(timeout == -1 || timeout > 0);

            bytesSent = 0;
            SocketError errorCode;
            if (SocketPal.TryCompleteSendTo(_fileDescriptor, buffer, ref offset, ref count, flags, socketAddress, socketAddressLen, ref bytesSent, out errorCode))
            {
                return errorCode;
            }

            using (var @event = new ManualResetEventSlim())
            {
                var operation = new SendOperation {
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
                while (!TryBeginOperation(ref _sendQueue, operation, Interop.Sys.SocketEvents.Write, out isStopped))
                {
                    if (isStopped)
                    {
                        // TODO: is this error code reasonable for a closed socket? Check with Winsock.
                        bytesSent = operation.BytesTransferred;
                        return SocketError.Shutdown;
                    }

                    if (operation.TryComplete(_fileDescriptor))
                    {
                        bytesSent = operation.BytesTransferred;
                        return operation.ErrorCode;
                    }
                }

                bool signaled = operation.Wait(timeout);
                bytesSent = operation.BytesTransferred;
                return signaled ? operation.ErrorCode : SocketError.TimedOut;
            }
        }

        public SocketError SendToAsync(byte[] buffer, int offset, int count, SocketFlags flags, byte[] socketAddress, int socketAddressLen, Action<int, byte[], int, SocketFlags, SocketError> callback)
        {
            int bytesSent = 0;
            SocketError errorCode;
            if (SocketPal.TryCompleteSendTo(_fileDescriptor, buffer, ref offset, ref count, flags, socketAddress, socketAddressLen, ref bytesSent, out errorCode))
            {
                if (errorCode == SocketError.Success)
                {
                    ThreadPool.QueueUserWorkItem(args =>
                    {
                        var tup = (Tuple<Action<int, byte[], int, SocketFlags, SocketError>, int, byte[], int>)args;
                        tup.Item1(tup.Item2, tup.Item3, tup.Item4, 0, SocketError.Success);
                    }, Tuple.Create(callback, bytesSent, socketAddress, socketAddressLen));
                }
                return errorCode;
            }

            var operation = new SendOperation {
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
            while (!TryBeginOperation(ref _sendQueue, operation, Interop.Sys.SocketEvents.Write, out isStopped))
            {
                if (isStopped)
                {
                    // TODO: is this error code reasonable for a closed socket? Check with Winsock.
                    operation.ErrorCode = SocketError.Shutdown;
                    operation.QueueCompletionCallback();
                    return SocketError.Shutdown;
                }

                if (operation.TryComplete(_fileDescriptor))
                {
                    operation.QueueCompletionCallback();
                    break;
                }
            }
            return SocketError.IOPending;
        }

        public SocketError Send(IList<ArraySegment<byte>> buffers, SocketFlags flags, int timeout, out int bytesSent)
        {
            return SendTo(buffers, flags, null, 0, timeout, out bytesSent);
        }

        public SocketError SendAsync(IList<ArraySegment<byte>> buffers, SocketFlags flags, Action<int, byte[], int, SocketFlags, SocketError> callback)
        {
            return SendToAsync(buffers, flags, null, 0, callback);
        }

        public SocketError SendTo(IList<ArraySegment<byte>> buffers, SocketFlags flags, byte[] socketAddress, int socketAddressLen, int timeout, out int bytesSent)
        {
            Debug.Assert(timeout == -1 || timeout > 0);

            bytesSent = 0;
            int bufferIndex = 0;
            int offset = 0;
            SocketError errorCode;
            if (SocketPal.TryCompleteSendTo(_fileDescriptor, buffers, ref bufferIndex, ref offset, flags, socketAddress, socketAddressLen, ref bytesSent, out errorCode))
            {
                return errorCode;
            }

            using (var @event = new ManualResetEventSlim())
            {
                var operation = new SendOperation {
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
                while (!TryBeginOperation(ref _sendQueue, operation, Interop.Sys.SocketEvents.Write, out isStopped))
                {
                    if (isStopped)
                    {
                        // TODO: is this error code reasonable for a closed socket? Check with Winsock.
                        bytesSent = operation.BytesTransferred;
                        return SocketError.Shutdown;
                    }

                    if (operation.TryComplete(_fileDescriptor))
                    {
                        bytesSent = operation.BytesTransferred;
                        return operation.ErrorCode;
                    }
                }

                bool signaled = operation.Wait(timeout);
                bytesSent = operation.BytesTransferred;
                return signaled ? operation.ErrorCode : SocketError.TimedOut;
            }
        }

        public SocketError SendToAsync(IList<ArraySegment<byte>> buffers, SocketFlags flags, byte[] socketAddress, int socketAddressLen, Action<int, byte[], int, SocketFlags, SocketError> callback)
        {
            int bufferIndex = 0;
            int offset = 0;
            int bytesSent = 0;
            SocketError errorCode;
            if (SocketPal.TryCompleteSendTo(_fileDescriptor, buffers, ref bufferIndex, ref offset, flags, socketAddress, socketAddressLen, ref bytesSent, out errorCode))
            {
                if (errorCode == SocketError.Success)
                {
                    ThreadPool.QueueUserWorkItem(args =>
                    {
                        var tup = (Tuple<Action<int, byte[], int, SocketFlags, SocketError>, int, byte[], int>)args;
                        tup.Item1(tup.Item2, tup.Item3, tup.Item4, SocketFlags.None, SocketError.Success);
                    }, Tuple.Create(callback, bytesSent, socketAddress, socketAddressLen));
                }
                return errorCode;
            }

            var operation = new SendOperation {
                Callback = callback,
                Buffers = buffers,
                BufferIndex = bufferIndex,
                Offset = offset,
                Flags = flags,
                SocketAddress = socketAddress,
                SocketAddressLen = socketAddressLen,
                BytesTransferred = bytesSent
            };

            bool isStopped;
            while (!TryBeginOperation(ref _sendQueue, operation, Interop.Sys.SocketEvents.Write, out isStopped))
            {
                if (isStopped)
                {
                    // TODO: is this error code reasonable for a closed socket? Check with Winsock.
                    operation.ErrorCode = SocketError.Shutdown;
                    operation.QueueCompletionCallback();
                    return SocketError.Shutdown;
                }

                if (operation.TryComplete(_fileDescriptor))
                {
                    operation.QueueCompletionCallback();
                    break;
                }
            }
            return SocketError.IOPending;
        }

        public unsafe void HandleEvents(Interop.Sys.SocketEvents events)
        {
            Debug.Assert(!Monitor.IsEntered(_queueLock) || Monitor.IsEntered(_closeLock), "Lock ordering violation");

            lock (_closeLock)
            {
                if (_registeredEvents == (Interop.Sys.SocketEvents)(-1))
                {
                    // This can happen if a previous attempt at unregistration did not succeed.
                    // Retry the unregistration.
                    lock (_queueLock)
                    {
                        Debug.Assert(_acceptOrConnectQueue.IsStopped, "{Accept,Connect} queue should be stopped before retrying unregistration");
                        Debug.Assert(_sendQueue.IsStopped, "Send queue should be stopped before retrying unregistration");
                        Debug.Assert(_receiveQueue.IsStopped, "Receive queue should be stopped before retrying unregistration");

                        Unregister();
                        return;
                    }
                }

                if ((events & Interop.Sys.SocketEvents.Error) != 0)
                {
                    // Set the Read and Write flags as well; the processing for these events
                    // will pick up the error.
                    events |= Interop.Sys.SocketEvents.Read | Interop.Sys.SocketEvents.Write;
                }

                if ((events & Interop.Sys.SocketEvents.Close) != 0)
                {
                    // Drain queues and unregister this fd, then return.
                    CloseInner();
                    return;
                }

                if ((events & Interop.Sys.SocketEvents.ReadClose) != 0)
                {
                    // Drain read queue and unregister read operations
                    Debug.Assert(_acceptOrConnectQueue.IsEmpty, "{Accept,Connect} queue should be empty before ReadClose");

                    OperationQueue<TransferOperation> receiveQueue;
                    lock (_queueLock)
                    {
                        receiveQueue = _receiveQueue.Stop();
                    }

                    while (!receiveQueue.IsEmpty)
                    {
                        TransferOperation op = receiveQueue.Head;
                        bool completed = op.TryCompleteAsync(_fileDescriptor);
                        Debug.Assert(completed);
                        receiveQueue.Dequeue();
                    }

                    lock (_queueLock)
                    {
                        UnregisterRead();
                    }

                    // Any data left in the socket has been received above; skip further processing.
                    events &= ~Interop.Sys.SocketEvents.Read;
                }

                // TODO: optimize locking and completions:
                // - Dequeues (and therefore locking) for multiple contiguous operations can be combined
                // - Contiguous completions can happen in a single thread

                if ((events & Interop.Sys.SocketEvents.Read) != 0)
                {
                    AcceptOrConnectOperation acceptTail;
                    TransferOperation receiveTail;
                    lock (_queueLock)
                    {
                        acceptTail = _acceptOrConnectQueue.Tail as AcceptOperation;
                        _acceptOrConnectQueue.State = QueueState.Set;

                        receiveTail = _receiveQueue.Tail;
                        _receiveQueue.State = QueueState.Set;
                    }

                    if (acceptTail != null)
                    {
                        AcceptOrConnectOperation op;
                        do
                        {
                            op = _acceptOrConnectQueue.Head;
                            if (!op.TryCompleteAsync(_fileDescriptor))
                            {
                                break;
                            }
                            EndOperation(ref _acceptOrConnectQueue);
                        } while (op != acceptTail);
                    }

                    if (receiveTail != null)
                    {
                        TransferOperation op;
                        do
                        {
                            op = _receiveQueue.Head;
                            if (!op.TryCompleteAsync(_fileDescriptor))
                            {
                                break;
                            }
                            EndOperation(ref _receiveQueue);
                        } while (op != receiveTail);
                    }
                }

                if ((events & Interop.Sys.SocketEvents.Write) != 0)
                {
                    AcceptOrConnectOperation connectTail;
                    SendOperation sendTail;
                    lock (_queueLock)
                    {
                        connectTail = _acceptOrConnectQueue.Tail as ConnectOperation;
                        _acceptOrConnectQueue.State = QueueState.Set;

                        sendTail = _sendQueue.Tail;
                        _sendQueue.State = QueueState.Set;
                    }

                    if (connectTail != null)
                    {
                        AcceptOrConnectOperation op;
                        do
                        {
                            op = _acceptOrConnectQueue.Head;
                            if (!op.TryCompleteAsync(_fileDescriptor))
                            {
                                break;
                            }
                            EndOperation(ref _acceptOrConnectQueue);
                        } while (op != connectTail);
                    }

                    if (sendTail != null)
                    {
                        SendOperation op;
                        do
                        {
                            op = _sendQueue.Head;
                            if (!op.TryCompleteAsync(_fileDescriptor))
                            {
                                break;
                            }
                            EndOperation(ref _sendQueue);
                        } while (op != sendTail);
                    }
                }
            }
        }
    }
}
