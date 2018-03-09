﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Buffers;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;

namespace System.IO.Pipelines
{
    [DebuggerDisplay("IsCompleted: {" + nameof(IsCompleted) + "}")]
    internal struct PipeCompletion
    {
        private static readonly ArrayPool<PipeCompletionCallback> s_completionCallbackPool = ArrayPool<PipeCompletionCallback>.Shared;
        private static readonly Exception s_completedNoException = new Exception();

        private const int InitialCallbacksSize = 1;

        private Exception _exception;

        private PipeCompletionCallback[] _callbacks;
        private int _callbackCount;

        public bool IsCompleted => _exception != null;

        public bool IsFaulted => IsCompleted && _exception != s_completedNoException;

        public PipeCompletionCallbacks TryComplete(Exception exception = null)
        {
            if (_exception == null)
            {
                // Set the exception object to the exception passed in or a sentinel value
                _exception = exception ?? s_completedNoException;
            }
            return GetCallbacks();
        }

        public PipeCompletionCallbacks AddCallback(Action<Exception, object> callback, object state)
        {
            if (_callbacks == null)
            {
                _callbacks = s_completionCallbackPool.Rent(InitialCallbacksSize);
            }

            int newIndex = _callbackCount;
            _callbackCount++;

            if (newIndex == _callbacks.Length)
            {
                PipeCompletionCallback[] newArray = s_completionCallbackPool.Rent(_callbacks.Length * 2);
                Array.Copy(_callbacks, newArray, _callbacks.Length);
                s_completionCallbackPool.Return(_callbacks, clearArray: true);
                _callbacks = newArray;
            }

            _callbacks[newIndex].Callback = callback;
            _callbacks[newIndex].State = state;

            if (IsCompleted)
            {
                return GetCallbacks();
            }

            return null;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsCompletedOrThrow()
        {
            if (_exception == null)
            {
                return false;
            }

            if (_exception != s_completedNoException)
            {
                ThrowLatchedException();
            }

            return true;
        }

        private PipeCompletionCallbacks GetCallbacks()
        {
            Debug.Assert(IsCompleted);
            if (_callbackCount == 0)
            {
                return null;
            }

            var callbacks = new PipeCompletionCallbacks(s_completionCallbackPool,
                _callbackCount,
                _exception == s_completedNoException ? null : _exception,
                _callbacks);

            _callbacks = null;
            _callbackCount = 0;
            return callbacks;
        }

        public void Reset()
        {
            Debug.Assert(IsCompleted);
            Debug.Assert(_callbacks == null);
            _exception = null;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private void ThrowLatchedException()
        {
            ExceptionDispatchInfo.Capture(_exception).Throw();
        }

        public override string ToString()
        {
            return $"{nameof(IsCompleted)}: {IsCompleted}";
        }
    }
}
