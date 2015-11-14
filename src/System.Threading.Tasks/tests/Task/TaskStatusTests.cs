// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using Xunit;

namespace System.Threading.Tasks.Tests
{
    public static class TaskStatusTests
    {
        // Maximum wait time to avoid permanent deadlock in tests.
        private const int MaxWaitTime = 1000;

        [Fact]
        public static void Promise_WaitingForActivation()
        {
            Assert.Equal(TaskStatus.WaitingForActivation, new TaskCompletionSource<int>().Task.Status);
        }

        [Fact]
        public static void Promise_Completed()
        {
            TaskCompletionSource<int> completion = new TaskCompletionSource<int>();
            completion.SetResult(1);
            Assert.Equal(TaskStatus.RanToCompletion, completion.Task.Status);
            Assert.Equal(1, completion.Task.Result);
        }

        [Fact]
        public static void Promise_Faulted()
        {
            TaskCompletionSource<int> completion = new TaskCompletionSource<int>();
            completion.SetException(new DeliberateTestException());
            Assert.Equal(TaskStatus.Faulted, completion.Task.Status);
            Assert.NotNull(completion.Task.Exception);
            Assert.All(completion.Task.Exception.InnerExceptions, e => Assert.IsType<DeliberateTestException>(e));
        }

        [Fact]
        public static void Task_Cancel_Scheduled()
        {
            // Custom scheduler to control timing of cancel
            CancelWaitingToRunTaskScheduler scheduler = new CancelWaitingToRunTaskScheduler();
            Task task = Task.Factory.StartNew(() => { }, scheduler.Token, TaskCreationOptions.None, scheduler);

            // This version is used for clarity:
            //   - Assert.ThrowsAsync<TaskCanceledException>(() => task); would appear mysterious
            Assert.Throws<TaskCanceledException>(() => task.GetAwaiter().GetResult());
        }

        [Fact]
        public static void Task_Cancel_Created()
        {
            CancellationTokenSource source = new CancellationTokenSource();

            Task task = new Task(() => { }, source.Token);

            Assert.Equal(TaskStatus.Created, task.Status);
            Assert.False(task.IsCanceled);

            source.Cancel();

            Assert.Equal(TaskStatus.Canceled, task.Status);
            Assert.True(task.IsCanceled);
        }

        [Fact]
        public static void Task_Cancel_Running()
        {
            CancellationTokenSource source = new CancellationTokenSource();

            Task task = Task.Factory.StartNew(() => source.Cancel(), source.Token);

            Assert.True(task.Wait(MaxWaitTime));

            Assert.Equal(TaskStatus.RanToCompletion, task.Status);
            Assert.True(task.IsCompleted);
            Assert.False(task.IsCanceled);
        }

        [Fact]
        public static void Task_Faulted()
        {
            Task task = Task.Factory.StartNew(() => { throw new DeliberateTestException(); });
            Functions.AssertThrowsWrapped<DeliberateTestException>(() => task.Wait());
        }

        [Fact]
        public static void Task_Completed()
        {
            using (ManualResetEventSlim outer = new ManualResetEventSlim(false))
            using (ManualResetEventSlim inner = new ManualResetEventSlim(false))
            {
                Task task = Task.Factory.StartNew(() =>
                {
                    // Signal the test thread the task started running,
                    // then wait for it's check to complete
                    outer.Set();
                    Assert.True(inner.Wait(MaxWaitTime));
                });

                Assert.True(outer.Wait(MaxWaitTime));

                // Check task is running, then let it complete
                Assert.Equal(TaskStatus.Running, task.Status);
                inner.Set();

                // Wait for task to complete before checking final status.
                Assert.True(task.Wait(MaxWaitTime));
                Assert.Equal(TaskStatus.RanToCompletion, task.Status);
                Assert.True(task.IsCompleted);
            }
        }

        [Fact]
        public static void Task_ChildTask_Completed()
        {
            using (ManualResetEventSlim test = new ManualResetEventSlim(false))
            using (ManualResetEventSlim parent = new ManualResetEventSlim(false))
            using (ManualResetEventSlim child = new ManualResetEventSlim(false))
            {
                Task childTask = null;

                Task parentTask = Task.Factory.StartNew(() =>
                {
                    // Start the child thread,
                    // then wait for it's check to complete
                    childTask = Task.Factory.StartNew(() =>
                    {
                        test.Set();
                        Assert.True(child.Wait(MaxWaitTime));
                    }, TaskCreationOptions.AttachedToParent);
                    Assert.True(parent.Wait(MaxWaitTime));
                });

                Assert.True(test.Wait(MaxWaitTime));

                // Check tasks are running, then let them complete
                Assert.Equal(TaskStatus.Running, parentTask.Status);
                Assert.Equal(TaskStatus.Running, childTask.Status);
                // Let parent task 'finish', and wait for child.
                parent.Set();
                while (parentTask.Status == TaskStatus.Running) { /* do nothing */ };
                Assert.Equal(TaskStatus.WaitingForChildrenToComplete, parentTask.Status);

                child.Set();

                // Wait for tasks to complete before checking final status.
                Assert.True(Task.WaitAll(new[] { parentTask, childTask }, MaxWaitTime));
                Assert.Equal(TaskStatus.RanToCompletion, parentTask.Status);
                Assert.True(parentTask.IsCompleted);
                Assert.Equal(TaskStatus.RanToCompletion, childTask.Status);
                Assert.True(childTask.IsCompleted);
            }
        }

        [Fact]
        public static void Task_ChildTask_Completed_SourceCanceled()
        {
            CancellationTokenSource source = new CancellationTokenSource();

            Task childTask = null;

            Task parentTask = Task.Factory.StartNew(() =>
            {
                // Start the child thread,
                // then wait for it's check to complete
                childTask = Task.Factory.StartNew(() =>
                {
                    // Need to cancel once child task started, or created canceled.
                    source.Cancel();
                }, source.Token, TaskCreationOptions.AttachedToParent, TaskScheduler.Default);
            }, source.Token);

            // Wait for tasks to complete before checking final status.
            Assert.True(parentTask.Wait(MaxWaitTime));
            Assert.Equal(TaskStatus.RanToCompletion, parentTask.Status);
            Assert.True(parentTask.IsCompleted);
            Assert.False(parentTask.IsCanceled);
            Assert.Equal(TaskStatus.RanToCompletion, childTask.Status);
            Assert.True(childTask.IsCompleted);
            Assert.False(childTask.IsCanceled);
        }

        [Fact]
        public static void Task_ChildTask_Canceled()
        {
            CancellationTokenSource source = new CancellationTokenSource();

            Task childTask = null;

            Task parentTask = Task.Factory.StartNew(() =>
            {
                // Start the child thread,
                // then wait for it's check to complete
                childTask = Task.Factory.StartNew(() =>
                {
                    source.Cancel();
                    // Cancel child task by manually throwing OCE
                    throw new OperationCanceledException(source.Token);
                }, source.Token, TaskCreationOptions.AttachedToParent, TaskScheduler.Default);
            }, source.Token);

            // Wait for tasks to complete before checking final status.
            Assert.True(parentTask.Wait(MaxWaitTime));
            Assert.Equal(TaskStatus.RanToCompletion, parentTask.Status);
            Assert.True(parentTask.IsCompleted);
            Assert.False(parentTask.IsCanceled);
            Assert.Equal(TaskStatus.Canceled, childTask.Status);
            Assert.True(childTask.IsCompleted);
            Assert.True(childTask.IsCanceled);
        }

        [Theory]
        [InlineData(TaskCreationOptions.None)]
        [InlineData(TaskCreationOptions.AttachedToParent)]
        public static void Task_Faulted_ChildTask_Completed(TaskCreationOptions childOptions)
        {
            Task childTask = null;

            Task parentTask = Task.Factory.StartNew(() =>
            {
                childTask = Task.Factory.StartNew(() => { }, childOptions);

                throw new DeliberateTestException();
            });

            // Wait for tasks to complete before checking final status.
            Functions.AssertThrowsWrapped<DeliberateTestException>(() => parentTask.Wait(MaxWaitTime));
            Assert.Equal(TaskStatus.Faulted, parentTask.Status);
            Assert.True(parentTask.IsCompleted);
            Assert.True(parentTask.IsFaulted);
            Assert.Equal(TaskStatus.RanToCompletion, childTask.Status);
            Assert.True(childTask.IsCompleted);
        }

        [Theory]
        [InlineData(TaskCreationOptions.None)]
        [InlineData(TaskCreationOptions.AttachedToParent)]
        public static void Task_ChildTask_Faulted(TaskCreationOptions childOptions)
        {
            Task childTask = null;

            Task parentTask = Task.Factory.StartNew(() =>
            {
                childTask = Task.Factory.StartNew(() => { throw new DeliberateTestException(); }, childOptions);
            });

            if (childOptions == TaskCreationOptions.AttachedToParent)
            {
                AggregateException ae = Assert.Throws<AggregateException>(() => parentTask.Wait(MaxWaitTime));
                // Picks up internal exception from child task
                AggregateException inner = Assert.IsType<AggregateException>(Assert.Single(ae.InnerExceptions));
                Assert.IsType<DeliberateTestException>(Assert.Single(inner.InnerExceptions));
                Assert.Equal(TaskStatus.Faulted, parentTask.Status);
                Assert.True(parentTask.IsFaulted);
            }
            else
            {
                parentTask.Wait(MaxWaitTime);
                Assert.Equal(TaskStatus.RanToCompletion, parentTask.Status);
                Assert.False(parentTask.IsFaulted);
                // If not attatched, child task may run after/longer
                while (childTask.Status == TaskStatus.Running) { /* Do nothing */ }
            }
            Assert.True(parentTask.IsCompleted);
            Assert.Equal(TaskStatus.Faulted, childTask.Status);
            Assert.IsType<DeliberateTestException>(Assert.Single(childTask.Exception.InnerExceptions));
            Assert.True(childTask.IsCompleted);
            Assert.True(childTask.IsFaulted);
        }

        [Fact]
        public static void Task_Canceled_ChildTask_Completed()
        {
            CancellationTokenSource source = new CancellationTokenSource();

            Task childTask = null;

            Task parentTask = Task.Factory.StartNew(() =>
            {
                childTask = Task.Factory.StartNew(() => { }, new CancellationToken(), TaskCreationOptions.AttachedToParent, TaskScheduler.Default);
                source.Cancel();
                // Cancel task by manually throwing OCE
                throw new OperationCanceledException(source.Token);
            }, source.Token);

            // Wait for tasks to complete before checking final status.
            Functions.AssertThrowsWrapped<TaskCanceledException>(() => parentTask.Wait(MaxWaitTime));
            Assert.Equal(TaskStatus.Canceled, parentTask.Status);
            Assert.True(parentTask.IsCompleted);
            Assert.True(parentTask.IsCanceled);
            Assert.Equal(TaskStatus.RanToCompletion, childTask.Status);
            Assert.True(childTask.IsCompleted);
            Assert.False(childTask.IsCanceled);
        }

        [Fact]
        public static void Task_Canceled_ChildTask_Canceled()
        {
            CancellationTokenSource source = new CancellationTokenSource();

            Task childTask = null;

            Task parentTask = Task.Factory.StartNew(() =>
            {
                childTask = Task.Factory.StartNew(() =>
                {
                    source.Cancel();
                    throw new OperationCanceledException(source.Token);
                }, source.Token, TaskCreationOptions.AttachedToParent, TaskScheduler.Default);

                // Require source.Cancel() in both places to ward against race conditions.
                source.Cancel();
                // Cancel task by manually throwing OCE
                throw new OperationCanceledException(source.Token);
            }, source.Token);

            // Type made more specific by task platform
            Functions.AssertThrowsWrapped<TaskCanceledException>(() => parentTask.Wait(MaxWaitTime));
            Assert.Equal(TaskStatus.Canceled, parentTask.Status);
            Assert.True(parentTask.IsCompleted);
            Assert.True(parentTask.IsCanceled);
            Assert.Equal(TaskStatus.Canceled, childTask.Status);
            Assert.True(childTask.IsCompleted);
            Assert.True(childTask.IsCanceled);
        }

        /// <summary>
        /// Custom task scheduler that cancels tasks after queuing but before execution.
        /// </summary>
        /// This scheduler is intended to be used only once.
        private class CancelWaitingToRunTaskScheduler : TaskScheduler
        {
            private CancellationTokenSource _cancellation = new CancellationTokenSource();

            public CancellationToken Token { get { return _cancellation.Token; } }

            protected override void QueueTask(Task task)
            {
                _cancellation.Cancel();
                TryExecuteTask(task);
            }

            protected override bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued)
            {
                return false;
            }

            protected override IEnumerable<Task> GetScheduledTasks()
            {
                yield break;
            }
        }
    }
}
