// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;

namespace System.IO.Pipelines
{
    [DebuggerDisplay("CanceledState: {_canceledState}, IsCompleted: {IsCompleted}")]
    internal struct PipeAwaitable
    {
        private static readonly Action<object> _awaitableIsCompleted = _ => { };
        private static readonly Action<object> _awaitableIsNotCompleted = _ => { };

        private CanceledState _canceledState;
        private Action<object> _completion;
        private object _completionState;
        private CancellationToken _cancellationToken;
        private CancellationTokenRegistration _cancellationTokenRegistration;
        private PipeScheduler _scheduler;

        public PipeAwaitable(bool completed)
        {
            _canceledState = CanceledState.NotCanceled;
            _completion = completed ? _awaitableIsCompleted : _awaitableIsNotCompleted;
            _completionState = null;
            _scheduler = null;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public CancellationTokenRegistration AttachToken(CancellationToken cancellationToken, Action<object> callback, object state)
        {
            CancellationTokenRegistration oldRegistration;
            if (!cancellationToken.Equals(_cancellationToken))
            {
                oldRegistration = _cancellationTokenRegistration;
                _cancellationToken = cancellationToken;
                if (_cancellationToken.CanBeCanceled)
                {
                    _cancellationToken.ThrowIfCancellationRequested();
                    _cancellationTokenRegistration = _cancellationToken.Register(callback, state);
                }
            }
            return oldRegistration;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Complete(out Action<object> completion, out object completionState, out PipeScheduler scheduler)
        {
            Action<object> currentCompletion = _completion;
            _completion = _awaitableIsCompleted;
            object currentState = _completionState;
            _completionState = null;
            PipeScheduler currentScheduler = _scheduler;
            _scheduler = null;

            completionState = null;
            completion = null;
            scheduler = null;

            if (!ReferenceEquals(currentCompletion, _awaitableIsCompleted) &&
                !ReferenceEquals(currentCompletion, _awaitableIsNotCompleted))
            {
                completion = currentCompletion;
                completionState = currentState;
                scheduler = currentScheduler;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Reset()
        {
            if (ReferenceEquals(_completion, _awaitableIsCompleted) &&
                _canceledState < CanceledState.CancellationPreRequested)
            {
                _completion = _awaitableIsNotCompleted;
                _completionState = null;
                _scheduler = null;
            }

            // Change the state from observed -> not cancelled.
            // We only want to reset the cancelled state if it was observed
            if (_canceledState == CanceledState.CancelationObserved)
            {
                _canceledState = CanceledState.NotCanceled;
            }
        }

        public bool IsCompleted => ReferenceEquals(_completion, _awaitableIsCompleted);
        internal bool HasContinuation => !ReferenceEquals(_completion, _awaitableIsNotCompleted);

        public void OnCompleted(Action<object> continuation, object state, ValueTaskSourceOnCompletedFlags flags, out Action<object> completion, out object completionState, out bool doubleCompletion)
        {
            completionState = null;
            completion = null;

            doubleCompletion = false;
            Action<object> awaitableState = _completion;
            if (ReferenceEquals(awaitableState, _awaitableIsNotCompleted))
            {
                _completion = continuation;
                _completionState = state;

                if ((flags & ValueTaskSourceOnCompletedFlags.UseSchedulingContext) != 0)
                {
                    // Set the scheduler to the current synchronization context if there is one
                    // otherwise we delegate to what the pipe was configured with.
                    // REVIEW: Should the sync context override the current scheduler if it was explicitly specifed
                    // in the pipe options?
                    if (SynchronizationContext.Current != null)
                    {
                        _scheduler = PipeScheduler.SynchronizationContext;
                    }
                }
            }

            if (ReferenceEquals(awaitableState, _awaitableIsCompleted))
            {
                completion = continuation;
                completionState = state;
            }

            if (!ReferenceEquals(awaitableState, _awaitableIsNotCompleted))
            {
                doubleCompletion = true;
                completion = continuation;
                completionState = state;
            }
        }

        public void Cancel(out Action<object> completion, out object completionState, out PipeScheduler scheduler)
        {
            Complete(out completion, out completionState, out scheduler);
            _canceledState = completion == null ?
                CanceledState.CancellationPreRequested :
                CanceledState.CancellationRequested;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool ObserveCancelation()
        {
            if (_canceledState == CanceledState.NotCanceled)
            {
                return false;
            }

            bool isPrerequested = _canceledState == CanceledState.CancellationPreRequested;

            if (_canceledState >= CanceledState.CancellationPreRequested)
            {
                _canceledState = CanceledState.CancelationObserved;

                // Do not reset awaitable if we were not awaiting in the first place
                if (!isPrerequested)
                {
                    Reset();
                }

                _cancellationToken.ThrowIfCancellationRequested();

                return true;
            }

            return false;
        }

        private enum CanceledState
        {
            NotCanceled = 0,
            CancelationObserved = 1,
            CancellationPreRequested = 2,
            CancellationRequested = 3,
        }
    }
}
