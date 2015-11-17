// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;

namespace System.Net
{
    // The callback type used with below AsyncProtocolRequest class
    internal delegate void AsyncProtocolCallback(AsyncProtocolRequest asyncRequest);

    //
    // The class mimics LazyAsyncResult although it does not need to be thread safe nor it does need an Event.
    // We use this to implement iterative protocol logic:
    // 1) it can be reused for handshake-like or multi-IO request protocols
    // 2) it won't block async IO since there is NO event handler exposed
    //
    // UserAsyncResult property is a link into original user IO request (could be a BufferAsyncResult).
    //
    internal class AsyncProtocolRequest
    {
#if DEBUG
        internal object _DebugAsyncChain;         // Optionally used to track chains of async calls.
#endif

        private AsyncProtocolCallback _Callback;
        private int _CompletionStatus;

        private const int StatusNotStarted = 0;
        private const int StatusCompleted = 1;
        private const int StatusCheckedOnSyncCompletion = 2;


        public LazyAsyncResult UserAsyncResult;
        public int Result;
        public object AsyncState;

        public byte[] Buffer;                  // temp buffer reused by a protocol.
        public int Offset;
        public int Count;

        public AsyncProtocolRequest(LazyAsyncResult userAsyncResult)
        {
            GlobalLog.Assert(userAsyncResult != null, "AsyncProtocolRequest()|userAsyncResult == null");
            GlobalLog.Assert(!userAsyncResult.InternalPeekCompleted, "AsyncProtocolRequest()|userAsyncResult is already completed.");
            UserAsyncResult = userAsyncResult;
        }

        public void SetNextRequest(byte[] buffer, int offset, int count, AsyncProtocolCallback callback)
        {
            if (_CompletionStatus != StatusNotStarted)
            {
                throw new InternalException(); // pending op is in progress
            }

            Buffer = buffer;
            Offset = offset;
            Count = count;
            _Callback = callback;
        }

        internal object AsyncObject
        {
            get
            {
                return UserAsyncResult.AsyncObject;
            }
        }

        //
        // Notify protocol so a next stage could be started
        //
        internal void CompleteRequest(int result)
        {
            Result = result;
            int status = Interlocked.Exchange(ref _CompletionStatus, StatusCompleted);
            if (status == StatusCompleted)
            {
                throw new InternalException(); // only allow one call
            }

            if (status == StatusCheckedOnSyncCompletion)
            {
                _CompletionStatus = StatusNotStarted;
                _Callback(this);
            }
        }

        public bool MustCompleteSynchronously
        {
            get
            {
                int status = Interlocked.Exchange(ref _CompletionStatus, StatusCheckedOnSyncCompletion);
                if (status == StatusCheckedOnSyncCompletion)
                {
                    throw new InternalException(); // only allow one call
                }

                if (status == StatusCompleted)
                {
                    _CompletionStatus = StatusNotStarted;
                    return true;
                }
                return false;
            }
        }

        //
        // Important: This will abandon _Callback and directly notify UserAsyncResult.
        //
        internal void CompleteWithError(Exception e)
        {
            UserAsyncResult.InvokeCallback(e);
        }

        internal void CompleteUser()
        {
            UserAsyncResult.InvokeCallback();
        }

        internal void CompleteUser(object userResult)
        {
            UserAsyncResult.InvokeCallback(userResult);
        }

        internal bool IsUserCompleted
        {
            get { return UserAsyncResult.InternalPeekCompleted; }
        }
    }
}
