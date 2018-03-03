// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks.Sources;

namespace System.IO.Pipelines
{
    [DebuggerDisplay("CanceledState: {_canceledState}, IsCompleted: {IsCompleted}")]
    internal struct PipeAwaitable
    {
        private static readonly Action<object> s_awaitableIsCompleted = _ => { };
        private static readonly Action<object> s_awaitableIsNotCompleted = _ => { };

        private CanceledState _canceledState;
        private Action<object> _completion;
        private object _completionState;
        private CancellationToken _cancellationToken;
        private CancellationTokenRegistration _cancellationTokenRegistration;
        private SynchronizationContext _synchronizationContext;
        private ExecutionContext _executionContext;

        public PipeAwaitable(bool completed)
        {
            _canceledState = CanceledState.NotCanceled;
            _completion = completed ? s_awaitableIsCompleted : s_awaitableIsNotCompleted;
            _completionState = null;
            _synchronizationContext = null;
            _executionContext = null;
        }

        public bool IsCompleted => ReferenceEquals(_completion, s_awaitableIsCompleted);

        public bool HasContinuation => !ReferenceEquals(_completion, s_awaitableIsNotCompleted);

        public bool IsCancelled => _canceledState >= CanceledState.CancellationPreRequested;

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
        public void Complete(out Action<object> completion, out object completionState, out SynchronizationContext synchronizationContext, out ExecutionContext executionContext)
        {
            Action<object> currentCompletion = _completion;
            _completion = s_awaitableIsCompleted;

            completionState = null;
            completion = null;
            synchronizationContext = null;
            executionContext = null;

            if (!ReferenceEquals(currentCompletion, s_awaitableIsCompleted) &&
                !ReferenceEquals(currentCompletion, s_awaitableIsNotCompleted))
            {
                // If we captured the execution context then we need to wrap our completion and state up
                completion = currentCompletion;
                completionState = _completionState;
                executionContext = _executionContext;
                // We only want to use the sync context scheduler if it's non null and is valid. We reuse the object
                // to avoid allocations per async operation so it can be non-null and not have any valid state
                synchronizationContext = _synchronizationContext;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Reset()
        {
            if (ReferenceEquals(_completion, s_awaitableIsCompleted) &&
                _canceledState < CanceledState.CancellationPreRequested)
            {
                _completion = s_awaitableIsNotCompleted;
                _completionState = null;
                _synchronizationContext = null;
                _executionContext = null;
            }

            // Change the state from observed -> not cancelled.
            // We only want to reset the cancelled state if it was observed
            if (_canceledState == CanceledState.CancelationObserved)
            {
                _canceledState = CanceledState.NotCanceled;
            }
        }

        public void OnCompleted(Action<object> continuation, object state, ValueTaskSourceOnCompletedFlags flags, out Action<object> completion, out object completionState, out bool doubleCompletion)
        {
            completionState = null;
            completion = null;

            doubleCompletion = false;
            Action<object> awaitableState = _completion;
            if (ReferenceEquals(awaitableState, s_awaitableIsNotCompleted))
            {
                _completion = continuation;
                _completionState = state;

                if ((flags & ValueTaskSourceOnCompletedFlags.UseSchedulingContext) != 0)
                {
                    // Set the scheduler to the current synchronization context if there is one
                    // otherwise we delegate to what the pipe was configured with.

                    // REVIEW: Should the sync context override the current scheduler if it was explicitly specifed
                    // in the pipe options?

                    // REVIEW: Should we post to the sync context if we're already completed (i.e. in OnCompleted)? Currently we're still delegating to the
                    // scheduler here.
                    SynchronizationContext sc = SynchronizationContext.Current;
                    if (sc != null && sc.GetType() != typeof(SynchronizationContext))
                    {
                        _synchronizationContext = SynchronizationContext.Current;
                    }
                }

                // Capture the execution context
                if ((flags & ValueTaskSourceOnCompletedFlags.FlowExecutionContext) != 0)
                {
                    _executionContext = ExecutionContext.Capture();
                }
            }

            if (ReferenceEquals(awaitableState, s_awaitableIsCompleted))
            {
                completion = continuation;
                completionState = state;
                return;
            }

            if (!ReferenceEquals(awaitableState, s_awaitableIsNotCompleted))
            {
                doubleCompletion = true;
                completion = continuation;
                completionState = state;
            }
        }

        public void Cancel(out Action<object> completion, out object completionState, out SynchronizationContext synchronizationContext, out ExecutionContext executionContext)
        {
            Complete(out completion, out completionState, out synchronizationContext, out executionContext);
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
