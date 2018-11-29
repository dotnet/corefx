// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks.Sources;

namespace System.IO.Pipelines
{
    [DebuggerDisplay("CanceledState: {_awaitableState}, IsCompleted: {IsCompleted}")]
    internal struct PipeAwaitable
    {
        private AwaitableState _awaitableState;
        private Action<object> _completion;
        private object _completionState;
        private CancellationToken _cancellationToken;
        private CancellationTokenRegistration _cancellationTokenRegistration;
        private SynchronizationContext _synchronizationContext;
        private ExecutionContext _executionContext;
        private readonly bool _useSynchronizationContext;

        public PipeAwaitable(bool completed, bool useSynchronizationContext)
        {
            _awaitableState = completed ? AwaitableState.Completed : AwaitableState.None;
            _completion = null;
            _completionState = null;
            _cancellationToken = CancellationToken.None;
            _cancellationTokenRegistration = default;
            _synchronizationContext = null;
            _executionContext = null;
            _useSynchronizationContext = useSynchronizationContext;
        }

        public bool IsCompleted => (_awaitableState & (AwaitableState.Completed | AwaitableState.Cancelled)) > 0;

        public bool IsRunning => (_awaitableState & AwaitableState.Running) == AwaitableState.Running;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public CancellationTokenRegistration BeginOperation(CancellationToken cancellationToken, Action<object> callback, object state)
        {
            _awaitableState |= AwaitableState.Running;

            CancellationTokenRegistration oldRegistration = default;
            if (!cancellationToken.Equals(_cancellationToken))
            {
                oldRegistration = _cancellationTokenRegistration;
                _cancellationToken = cancellationToken;
                if (_cancellationToken.CanBeCanceled)
                {
                    _cancellationToken.ThrowIfCancellationRequested();
                    _cancellationTokenRegistration = _cancellationToken.UnsafeRegister(callback, state);
                }
            }
            return oldRegistration;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Complete(out CompletionData completionData)
        {
            ExtractCompletion(out completionData);

            _awaitableState |= AwaitableState.Completed;
        }

        private void ExtractCompletion(out CompletionData completionData)
        {
            Action<object> currentCompletion = _completion;
            object currentState = _completionState;

            _completion = null;
            _completionState = null;

            if (currentCompletion != null)
            {
                completionData = new CompletionData(currentCompletion, currentState, _executionContext, _synchronizationContext);
            }
            else
            {
                completionData = default;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Reset()
        {
            _completion = null;
            _completionState = null;
            _synchronizationContext = null;
            _executionContext = null;

            _awaitableState &= ~AwaitableState.Completed;
        }

        public void OnCompleted(Action<object> continuation, object state, ValueTaskSourceOnCompletedFlags flags, out CompletionData completionData, out bool doubleCompletion)
        {
            completionData = default;

            doubleCompletion = false;

            if (IsCompleted)
            {
                completionData = new CompletionData(continuation, state, _executionContext, _synchronizationContext);
                return;
            }

            if (_completion != null)
            {
                doubleCompletion = true;
                completionData = new CompletionData(continuation, state, _executionContext, _synchronizationContext);
                return;
            }

            _completion = continuation;
            _completionState = state;

            // Capture the SynchronizationContext if there's any and we're allowing capture (from pipe options)
            if (_useSynchronizationContext && (flags & ValueTaskSourceOnCompletedFlags.UseSchedulingContext) != 0)
            {
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

        public void Cancel(out CompletionData completionData)
        {
            ExtractCompletion(out completionData);

            _awaitableState |= AwaitableState.Cancelled;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool ObserveCancelation()
        {
            bool isCancelled = (_awaitableState & AwaitableState.Cancelled) == AwaitableState.Cancelled;

            _awaitableState &= ~AwaitableState.Cancelled;
            _awaitableState &= ~AwaitableState.Running;

            _cancellationToken.ThrowIfCancellationRequested();

            return isCancelled;
        }

        [Flags]
        private enum AwaitableState
        {
            None = 0,
            // Set in Complete reset in Reset
            Completed = 1,
            // Set in *Async reset in  ObserveCancellation (GetResult)
            Running = 2,
            // Set in Cancel reset in ObserveCancellation (GetResult)
            Cancelled = 4,
        }
    }
}
