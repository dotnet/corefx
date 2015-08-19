// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
/*++
Copyright (c) 2000 Microsoft Corporation

Module Name:

    _HelperAsyncResults.cs

Abstract:

    These are simple helpers to keep track of protocol IO requests

Author:

    Alexei Vopilov  Aug 18 2003

Revision History:

--*/

using System;
using System.IO;
using System.Threading;

namespace System.Net
{
    //
    // Simply said whenever we want to wrap a user IO request we need
    // to preserve the original request buffer & sizes.
    //
    // Usually we would return this class as IAsyncResult to application
    //
    internal class BufferAsyncResult : LazyAsyncResult
    {
        public byte[] Buffer;
        public BufferOffsetSize[] Buffers;
        public int Offset;
        public int Count;
        public bool IsWrite;

        // MultipleWriteOnly
        //
        public BufferAsyncResult(object asyncObject, BufferOffsetSize[] buffers, object asyncState, AsyncCallback asyncCallback)
            : base(asyncObject, asyncState, asyncCallback)
        {
            Buffers = buffers;
            IsWrite = true;
        }

        public BufferAsyncResult(object asyncObject, byte[] buffer, int offset, int count, object asyncState, AsyncCallback asyncCallback)
            : this(asyncObject, buffer, offset, count, false, asyncState, asyncCallback)
        {
        }

        public BufferAsyncResult(object asyncObject, byte[] buffer, int offset, int count, bool isWrite, object asyncState, AsyncCallback asyncCallback)
            : base(asyncObject, asyncState, asyncCallback)
        {
            Buffer = buffer;
            Offset = offset;
            Count = count;
            IsWrite = isWrite;
        }
    }

    // The callback type used with below AsyncProtocolRequest class
    internal delegate void AsyncProtocolCallback(AsyncProtocolRequest asyncRequest);

    //
    // The class mimics LazyAsyncResult although it does not need to be thread safe nor it does need an Event.
    // Usually we would use this class to implement iterative protocol logic
    //
    // The beauty is that
    // 1) it can be reused for handshake-like or multi-IO request protocols
    // 2) it is proven to not block async IO since there is NO event handler exposed
    //
    // UserAsyncResult property is a link into original user IO request (could be a BufferAsyncResult).
    // When underlined protocol is done this guy gets completed
    //
    internal class AsyncProtocolRequest
    {
#if DEBUG
        internal object _DebugAsyncChain;         // Optionally used to track chains of async calls.
#endif

        private AsyncProtocolCallback _Callback;
        private int _CompletionStatus;

        const int StatusNotStarted = 0;
        const int StatusCompleted = 1;
        const int StatusCheckedOnSyncCompletion = 2;


        public LazyAsyncResult UserAsyncResult;
        public int Result;            // it's always about read bytes or alike
        public object AsyncState;        // sometime it's needed to communicate additional info.
                                         // Note that AsyncObject is just a link to UserAsyncResult.AsyncObject

        public byte[] Buffer;                     // temp buffer reused by a protocol.
        public int Offset;                     // ditto
        public int Count;                      // ditto

        //
        //
        public AsyncProtocolRequest(LazyAsyncResult userAsyncResult)
        {
            GlobalLog.Assert(userAsyncResult != null, "AsyncProtocolRequest()|userAsyncResult == null");
            GlobalLog.Assert(!userAsyncResult.InternalPeekCompleted, "AsyncProtocolRequest()|userAsyncResult is already completed.");
            UserAsyncResult = userAsyncResult;
        }
        //
        //
        public void SetNextRequest(byte[] buffer, int offset, int count, AsyncProtocolCallback callback)
        {
            if (_CompletionStatus != StatusNotStarted)
                throw new InternalException(); // pending op is in progress

            Buffer = buffer;
            Offset = offset;
            Count = count;
            _Callback = callback;
        }
        //
        //
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
                throw new InternalException(); // only allow one call

            if (status == StatusCheckedOnSyncCompletion)
            {
                _CompletionStatus = StatusNotStarted;
                _Callback(this);
            }
        }
        //
        public bool MustCompleteSynchronously
        {
            get
            {
                int status = Interlocked.Exchange(ref _CompletionStatus, StatusCheckedOnSyncCompletion);
                if (status == StatusCheckedOnSyncCompletion)
                    throw new InternalException(); // only allow one call
                if (status == StatusCompleted)
                {
                    _CompletionStatus = StatusNotStarted;
                    return true;
                }
                return false;
            }
        }
        //
        // NB: This will abandon _Callback and directly notify UserAsyncResult.
        //
        internal void CompleteWithError(Exception e)
        {
            UserAsyncResult.InvokeCallback(e);
        }
        //
        internal void CompleteUser()
        {
            UserAsyncResult.InvokeCallback();
        }
        //
        internal void CompleteUser(object userResult)
        {
            UserAsyncResult.InvokeCallback(userResult);
        }
        //
        internal bool IsUserCompleted
        {
            get { return UserAsyncResult.InternalPeekCompleted; }
        }
    }
}
