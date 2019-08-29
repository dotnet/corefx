// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Diagnostics.Tracing;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Text;

namespace System.Runtime.CompilerServices
{
    /// <summary>Shared helpers for manipulating state related to async state machines.</summary>
    internal static class AsyncMethodBuilderCore // debugger depends on this exact name
    {
        /// <summary>Initiates the builder's execution with the associated state machine.</summary>
        /// <typeparam name="TStateMachine">Specifies the type of the state machine.</typeparam>
        /// <param name="stateMachine">The state machine instance, passed by reference.</param>
        [DebuggerStepThrough]
        public static void Start<TStateMachine>(ref TStateMachine stateMachine) where TStateMachine : IAsyncStateMachine
        {
            if (stateMachine == null) // TStateMachines are generally non-nullable value types, so this check will be elided
            {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.stateMachine);
            }

            // enregistrer variables with 0 post-fix so they can be used in registers without EH forcing them to stack
            // Capture references to Thread Contexts
            Thread currentThread0 = Thread.CurrentThread;
            Thread currentThread = currentThread0;
            ExecutionContext? previousExecutionCtx0 = currentThread0._executionContext;

            // Store current ExecutionContext and SynchronizationContext as "previousXxx".
            // This allows us to restore them and undo any Context changes made in stateMachine.MoveNext
            // so that they won't "leak" out of the first await.
            ExecutionContext? previousExecutionCtx = previousExecutionCtx0;
            SynchronizationContext? previousSyncCtx = currentThread0._synchronizationContext;

            try
            {
                stateMachine.MoveNext();
            }
            finally
            {
                // Re-enregistrer variables post EH with 1 post-fix so they can be used in registers rather than from stack
                SynchronizationContext? previousSyncCtx1 = previousSyncCtx;
                Thread currentThread1 = currentThread;
                // The common case is that these have not changed, so avoid the cost of a write barrier if not needed.
                if (previousSyncCtx1 != currentThread1._synchronizationContext)
                {
                    // Restore changed SynchronizationContext back to previous
                    currentThread1._synchronizationContext = previousSyncCtx1;
                }

                ExecutionContext? previousExecutionCtx1 = previousExecutionCtx;
                ExecutionContext? currentExecutionCtx1 = currentThread1._executionContext;
                if (previousExecutionCtx1 != currentExecutionCtx1)
                {
                    ExecutionContext.RestoreChangedContextToThread(currentThread1, previousExecutionCtx1, currentExecutionCtx1);
                }
            }
        }

        public static void SetStateMachine(IAsyncStateMachine stateMachine, Task task)
        {
            if (stateMachine == null)
            {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.stateMachine);
            }

            if (task != null)
            {
                ThrowHelper.ThrowInvalidOperationException(ExceptionResource.AsyncMethodBuilder_InstanceNotInitialized);
            }

            // SetStateMachine was originally needed in order to store the boxed state machine reference into
            // the boxed copy.  Now that a normal box is no longer used, SetStateMachine is also legacy.  We need not
            // do anything here, and thus assert to ensure we're not calling this from our own implementations.
            Debug.Fail("SetStateMachine should not be used.");
        }

#if !CORERT
        /// <summary>Gets whether we should be tracking async method completions for eventing.</summary>
        internal static bool TrackAsyncMethodCompletion
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => TplEventSource.Log.IsEnabled(EventLevel.Warning, TplEventSource.Keywords.AsyncMethod);
        }
#endif

        /// <summary>Gets a description of the state of the state machine object, suitable for debug purposes.</summary>
        /// <param name="stateMachine">The state machine object.</param>
        /// <returns>A description of the state machine.</returns>
        internal static string GetAsyncStateMachineDescription(IAsyncStateMachine stateMachine)
        {
            Debug.Assert(stateMachine != null);

            Type stateMachineType = stateMachine.GetType();
            FieldInfo[] fields = stateMachineType.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            var sb = new StringBuilder();
            sb.AppendLine(stateMachineType.FullName);
            foreach (FieldInfo fi in fields)
            {
                sb.AppendLine($"    {fi.Name}: {fi.GetValue(stateMachine)}");
            }
            return sb.ToString();
        }

        internal static Action CreateContinuationWrapper(Action continuation, Action<Action, Task> invokeAction, Task innerTask) =>
            new ContinuationWrapper(continuation, invokeAction, innerTask).Invoke;

        /// <summary>This helper routine is targeted by the debugger. Its purpose is to remove any delegate wrappers introduced by
        /// the framework that the debugger doesn't want to see.</summary>
        internal static Action TryGetStateMachineForDebugger(Action action) // debugger depends on this exact name/signature
        {
            object? target = action.Target;
            return
                target is IAsyncStateMachineBox sm ? sm.GetStateMachineObject().MoveNext :
                target is ContinuationWrapper cw ? TryGetStateMachineForDebugger(cw._continuation) :
                action;
        }

        internal static Task? TryGetContinuationTask(Action continuation) =>
            (continuation?.Target as ContinuationWrapper)?._innerTask;

        /// <summary>
        /// Logically we pass just an Action (delegate) to a task for its action to 'ContinueWith' when it completes.
        /// However debuggers and profilers need more information about what that action is. (In particular what
        /// the action after that is and after that.   To solve this problem we create a 'ContinuationWrapper
        /// which when invoked just does the original action (the invoke action), but also remembers other information
        /// (like the action after that (which is also a ContinuationWrapper and thus form a linked list).
        ///  We also store that task if the action is associate with at task.
        /// </summary>
        private sealed class ContinuationWrapper // SOS DumpAsync command depends on this name
        {
            private readonly Action<Action, Task> _invokeAction; // This wrapper is an action that wraps another action, this is that Action.
            internal readonly Action _continuation;              // This is continuation which will happen after m_invokeAction  (and is probably a ContinuationWrapper). SOS DumpAsync command depends on this name.
            internal readonly Task _innerTask;                   // If the continuation is logically going to invoke a task, this is that task (may be null)

            internal ContinuationWrapper(Action continuation, Action<Action, Task> invokeAction, Task innerTask)
            {
                Debug.Assert(continuation != null, "Expected non-null continuation");
                Debug.Assert(invokeAction != null, "Expected non-null invokeAction");
                Debug.Assert(innerTask != null, "Expected non-null innerTask");

                _invokeAction = invokeAction;
                _continuation = continuation;
                _innerTask = innerTask;
            }

            internal void Invoke() => _invokeAction(_continuation, _innerTask);
        }
    }
}
