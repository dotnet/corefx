// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Security;
using System.Threading;

namespace System.IO.Pipes
{
    internal unsafe sealed class PipeAsyncResult : IAsyncResult
    {
        internal readonly ThreadPoolBoundHandle _threadPoolBinding;
        internal readonly AsyncCallback _userCallback;   // User code callback
        private readonly Object _userStateObject;
        private readonly ManualResetEventSlim _waitHandle;
        [SecurityCritical]
        internal NativeOverlapped* _overlapped;

        internal int _EndXxxCalled;             // Whether we've called EndXxx already.
        internal int _errorCode;

        internal bool _isComplete;              // Value for IsCompleted property        
        internal bool _completedSynchronously;  // Which thread called callback

        public PipeAsyncResult(ThreadPoolBoundHandle handle, AsyncCallback callback, Object state)
        {
            _threadPoolBinding = handle;
            _userCallback = callback;
            _userStateObject = state;
            _waitHandle = new ManualResetEventSlim(false);
        }

        public Object AsyncState
        {
            get { return _userStateObject; }
        }

        public bool IsCompleted
        {
            get { return _isComplete; }
        }

        public WaitHandle AsyncWaitHandle
        {
            [SecurityCritical]
            get
            {
                return _waitHandle.WaitHandle;
            }
        }

        public bool CompletedSynchronously
        {
            get { return _completedSynchronously; }
        }

        internal ManualResetEventSlim WaitHandle
        {
            get { return _waitHandle; }
        }

        internal void CallUserCallback()
        {
            if (_userCallback != null)
            {
                _completedSynchronously = false;
                ThreadPool.QueueUserWorkItem(s =>
                {
                    PipeAsyncResult ar = (PipeAsyncResult)s;
                    ar._isComplete = true;
                    if (ar._waitHandle != null)
                    {
                        ar._waitHandle.Set();
                    }
                    ar._userCallback(ar);
                }, this);
            }
            else
            {
                _isComplete = true;
                if (_waitHandle != null)
                {
                    _waitHandle.Set();
                }
            }
        }
    }
}
