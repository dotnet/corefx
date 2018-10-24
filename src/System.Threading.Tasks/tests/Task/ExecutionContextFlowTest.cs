// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using Xunit;

namespace System.Threading.Tasks.Tests
{
    public class ExecutionContextFlowTest
    {
        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void SuppressFlow_TaskCapturesContextAccordingly(bool suppressFlow)
        {
            Assert.False(ExecutionContext.IsFlowSuppressed());
            if (suppressFlow) ExecutionContext.SuppressFlow();
            try
            {
                var asyncLocal = new AsyncLocal<int>();
                Task.Factory.StartNew(() => asyncLocal.Value = 42, CancellationToken.None, TaskCreationOptions.None, new InlineTaskScheduler()).Wait();
                Assert.Equal(suppressFlow ? 42 : 0, asyncLocal.Value);
            }
            finally
            {
                if (suppressFlow) ExecutionContext.RestoreFlow();
            }
        }

        [SkipOnTargetFramework(TargetFrameworkMonikers.NetFramework, reason: "netfx doesn't have https://github.com/dotnet/coreclr/pull/20294")]
        [Fact]
        public static async Task TaskDropsExecutionContextUponCompletion()
        {
            // Create a finalizable object that'll be referenced by captured ExecutionContext,
            // run a task and wait for it, and then hold on to that task while forcing GCs and finalizers.
            // We want to make sure that holding on to the resulting Task doesn't keep
            // that finalizable object alive.

            var tcs = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);

            Task t = null;
            await Task.Run(delegate // avoid any issues with the stack keeping the object alive
            {
                var state = new InvokeActionOnFinalization { Action = () => tcs.SetResult(true) };
                var al = new AsyncLocal<object> { Value = state }; // ensure the object is stored in ExecutionContext
                t = Task.Run(() => { }); // run a task that'll capture EC
                al.Value = null;
            });

            await t; // wait for the task method to complete and clear out its state

            for (int i = 0; i < 2; i++)
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }

            await tcs.Task.TimeoutAfter(60_000); // finalizable object should have been collected and finalized
            GC.KeepAlive(t); // ensure the object is stored in the state machine
        }

        private sealed class InlineTaskScheduler : TaskScheduler
        {
            protected override void QueueTask(Task task) => TryExecuteTask(task);
            protected override bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued) => TryExecuteTask(task);
            protected override IEnumerable<Task> GetScheduledTasks() => null;
        }
    }
}
