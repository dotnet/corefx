// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace System.Linq.Expressions
{
    internal sealed class StackGuard
    {
        private const int StackProbingThreshold = 100;
        private const int MaxExecutionStackCount = 1024;

        private int _recursionDepth;
        private int _executionStackCount;

        public bool TryEnterOnCurrentStack()
        {
            if (++_recursionDepth >= StackProbingThreshold)
            {
                try
                {
                    RuntimeHelpers.EnsureSufficientExecutionStack();
                }
                catch (InsufficientExecutionStackException) when (_executionStackCount < MaxExecutionStackCount)
                {
                    return false;
                }
            }

            return true;
        }

        public void Exit()
        {
            _recursionDepth--;
        }

        public void RunOnEmptyStack(Action<object> action, object state)
        {
            _executionStackCount++;

            var recursionDepth = _recursionDepth;
            _recursionDepth = 0;

            try
            {
                // Using default scheduler rather than picking up the current scheduler.
                var task = Task.Factory.StartNew(action, state, CancellationToken.None, TaskCreationOptions.DenyChildAttach, TaskScheduler.Default);

                // Task.Wait has the potential of inlining the task's execution on the current thread; avoid this.
                ((IAsyncResult)task).AsyncWaitHandle.WaitOne();

                // Using awaiter here to unwrap AggregateException.
                task.GetAwaiter().GetResult();
            }
            finally
            {
                _executionStackCount--;
                _recursionDepth = recursionDepth - 1; // also counts as an Exit; caller is assumed to return after calling RunOnEmptyStack
            }
        }

        public R RunOnEmptyStack<R>(Func<object, R> action, object state)
        {
            _executionStackCount++;

            var recursionDepth = _recursionDepth;
            _recursionDepth = 0;

            try
            {
                // Using default scheduler rather than picking up the current scheduler.
                var task = Task.Factory.StartNew(action, state, CancellationToken.None, TaskCreationOptions.DenyChildAttach, TaskScheduler.Default);

                // Task.Wait has the potential of inlining the task's execution on the current thread; avoid this.
                ((IAsyncResult)task).AsyncWaitHandle.WaitOne();

                // Using awaiter here to unwrap AggregateException.
                return task.GetAwaiter().GetResult();
            }
            finally
            {
                _executionStackCount--;
                _recursionDepth = recursionDepth; // also counts as an Exit; caller is assumed to return after calling RunOnEmptyStack
            }
        }
    }
}
