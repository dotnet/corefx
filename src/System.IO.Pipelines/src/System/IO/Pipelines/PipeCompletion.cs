// Licensed to the .NET Foundation under one or more agreements.
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

        private const int InitialCallbacksSize = 1;

        private bool _isCompleted;
        private Exception _exception;

        private PipeCompletionCallback _firstCallback;
        private PipeCompletionCallback[] _callbacks;
        private int _callbackCount;

        public bool IsCompleted => _isCompleted;

        public bool IsFaulted => _exception != null;

        public PipeCompletionCallbacks TryComplete(Exception exception = null)
        {
            if (!_isCompleted)
            {
                _isCompleted = true;
                if (exception != null)
                {
                    // We really want to capture the preceeding stacktrace into the exception,
                    // if it hasn't already been thrown (and thus isn't a transfer exception),
                    // but there currently isn't a way of doing that.
                    CaptureException(ExceptionDispatchInfo.Capture(exception));
                }
            }

            return GetCallbacks();
        }

        private void CaptureException(ExceptionDispatchInfo edi)
        {
            try
            {
                // We throw and catch to fuse the captured stack into the exception; as it may be used multiple times.
                edi.Throw();
            }
            catch (Exception ex)
            {
                _exception = ex;
            }
        }

        public PipeCompletionCallbacks AddCallback(Action<Exception, object> callback, object state)
        {
            if (_callbackCount == 0)
            {
                _firstCallback = new PipeCompletionCallback(callback, state);
                _callbackCount++;
            }
            else
            {
                EnsureSpace();

                // -1 to adjust for _firstCallback
                var callbackIndex = _callbackCount - 1;
                _callbackCount++;
                _callbacks[callbackIndex] = new PipeCompletionCallback(callback, state);
            }

            if (IsCompleted)
            {
                return GetCallbacks();
            }

            return null;
        }

        private void EnsureSpace()
        {
            if (_callbacks == null)
            {
                _callbacks = s_completionCallbackPool.Rent(InitialCallbacksSize);
            }

            int newLength = _callbackCount - 1;

            if (newLength == _callbacks.Length)
            {
                PipeCompletionCallback[] newArray = s_completionCallbackPool.Rent(_callbacks.Length * 2);
                Array.Copy(_callbacks, 0, newArray, 0, _callbacks.Length);
                s_completionCallbackPool.Return(_callbacks, clearArray: true);
                _callbacks = newArray;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsCompletedOrThrow()
        {
            if (!_isCompleted)
            {
                return false;
            }

            if (_exception != null)
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
                _exception,
                _firstCallback,
                _callbacks);

            _firstCallback = default;
            _callbacks = null;
            _callbackCount = 0;
            return callbacks;
        }

        public void Reset()
        {
            Debug.Assert(IsCompleted);
            Debug.Assert(_callbacks == null);
            _isCompleted = false;
            _exception = null;
        }

        private void ThrowLatchedException()
        {
            // Throw a new exception so as not to corrupt stack trace, as may be thrown multiple times
            throw GetNewException();
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private Exception GetNewException()
        {
            if (_exception is OperationCanceledException oce)
            {
                // Differentate OperationCanceled exceptions so Cancellation signals are propergated.
                return new OperationCanceledException(oce.Message, oce, oce.CancellationToken);
            }
            else if (_exception is InvalidOperationException ioe)
            {
                return new InvalidOperationException(ioe.Message, ioe);
            }
            else if (_exception is ObjectDisposedException ode)
            {
                return new ObjectDisposedException(ode.Message, ode);
            }
            else
            {
                return new IOException(_exception.Message, _exception);
            }
        }

        public override string ToString()
        {
            return $"{nameof(IsCompleted)}: {IsCompleted}";
        }
    }
}
