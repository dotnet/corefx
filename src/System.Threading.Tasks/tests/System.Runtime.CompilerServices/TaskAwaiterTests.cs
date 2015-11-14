// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Runtime.CompilerServices;
using Xunit;

namespace System.Threading.Tasks.Tests
{
    public static class TaskAwaiterTests
    {
        [Fact]
        [OuterLoop]
        public static void AsyncTaskAwaiter_OtherContext_Test()
        {
            SynchronizationContext previous = SynchronizationContext.Current;
            try
            {
                // IsCompleted/OnCompleted on a non-completed Task with SyncContext and completes the task in another context
                var vccsc = new ValidateCorrectContextSynchronizationContext();
                SynchronizationContext.SetSynchronizationContext(vccsc);
                ManualResetEventSlim mres = new ManualResetEventSlim();
                var tcs = new TaskCompletionSource<object>();
                bool hasRun = false;
                var awaiter = ((Task)tcs.Task).GetAwaiter();
                Assert.False(awaiter.IsCompleted);
                awaiter.OnCompleted(() =>
                {
                    Assert.True(ValidateCorrectContextSynchronizationContext.IsPostedInContext);
                    hasRun = true;
                    mres.Set();
                });
                Assert.False(hasRun);
                Task.Run(() => tcs.SetResult(null));
                mres.Wait();
                awaiter.GetResult();
                Assert.True(hasRun);
                Assert.Equal(1, vccsc.PostCount);
            }
            finally
            {
                SynchronizationContext.SetSynchronizationContext(previous);
            }
        }

        [Fact]
        [OuterLoop]
        public static void AsyncTaskAwaiter_Scheduler_Test()
        {
            SynchronizationContext previous = SynchronizationContext.Current;
            try
            {
                // TODO: Figure out why the test locks without the next line.
                SynchronizationContext.SetSynchronizationContext(null);

                // IsCompleted/OnCompleted on a non-completed Task with TaskScheduler and completes the task in another context
                var quwi = new QUWITaskScheduler();
                RunWithSchedulerAsCurrent(quwi, () =>
                {
                    ManualResetEventSlim mres = new ManualResetEventSlim();
                    var tcs = new TaskCompletionSource<object>();
                    bool hasRun = false;
                    var awaiter = ((Task)tcs.Task).GetAwaiter();
                    Assert.False(awaiter.IsCompleted);
                    awaiter.OnCompleted(() =>
                    {
                        Assert.Equal(quwi, TaskScheduler.Current);
                        hasRun = true;
                        mres.Set();
                    });
                    Assert.False(hasRun);
                    Task.Run(() => tcs.SetResult(null));
                    mres.Wait();
                    awaiter.GetResult();
                    Assert.True(hasRun);
                });
            }
            finally
            {
                SynchronizationContext.SetSynchronizationContext(previous);
            }
        }

        [Theory]
        [OuterLoop]
        [InlineData(true)]
        [InlineData(false)]
        public static void AsyncTaskAwaiter_Continue_Test(bool continueOnCapturedContext)
        {
            SynchronizationContext previous = SynchronizationContext.Current;
            try
            {
                // Configured IsCompleted/OnCompleted on a non-completed Task with SyncContext

                Task.Factory.StartNew(() =>
                {
                    SynchronizationContext.SetSynchronizationContext(new ValidateCorrectContextSynchronizationContext());
                    ManualResetEventSlim mres = new ManualResetEventSlim();
                    var tcs = new TaskCompletionSource<object>();
                    bool hasRun = false;
                    var awaiter = ((Task)tcs.Task).ConfigureAwait(continueOnCapturedContext).GetAwaiter();
                    Assert.False(awaiter.IsCompleted);
                    awaiter.OnCompleted(() =>
                    {
                        Assert.Equal(continueOnCapturedContext, ValidateCorrectContextSynchronizationContext.IsPostedInContext);
                        hasRun = true;
                        mres.Set();
                    });
                    Assert.False(hasRun);
                    Task.Factory.StartNew(() => tcs.SetResult(null));
                    mres.Wait();
                    awaiter.GetResult();
                    Assert.True(hasRun);
                }, CancellationToken.None, TaskCreationOptions.None, TaskScheduler.Default).Wait();
            }
            finally
            {
                SynchronizationContext.SetSynchronizationContext(previous);
            }
        }

        [Fact]
        [OuterLoop]
        public static void AsyncTaskAwaiter_Future_OtherContext_Test()
        {
            SynchronizationContext previous = SynchronizationContext.Current;
            try
            {
                // IsCompleted/OnCompleted on a non-completed Task<TResult> with SyncContext and completes the task in another context
                var vccsc = new ValidateCorrectContextSynchronizationContext();
                SynchronizationContext.SetSynchronizationContext(vccsc);
                var mres = new ManualResetEventSlim();
                var tcs = new TaskCompletionSource<object>();
                bool hasRun = false;
                var awaiter = ((Task<object>)tcs.Task).GetAwaiter();
                Assert.False(awaiter.IsCompleted);
                awaiter.OnCompleted(() =>
                {
                    Assert.True(ValidateCorrectContextSynchronizationContext.IsPostedInContext);
                    hasRun = true;
                    mres.Set();
                });
                Assert.False(hasRun);
                Task.Run(() => tcs.SetResult(null));
                mres.Wait();
                awaiter.GetResult();
                Assert.True(hasRun);
                Assert.Equal(1, vccsc.PostCount);
            }
            finally
            {
                SynchronizationContext.SetSynchronizationContext(previous);
            }
        }

        [Theory]
        [OuterLoop]
        [InlineData(true)]
        [InlineData(false)]
        public static void AsyncTaskAwaiter_Future_Continue_Test(bool continueOnCapturedContext)
        {
            SynchronizationContext previous = SynchronizationContext.Current;
            try
            {
                // Configured IsCompleted/OnCompleted on a non-completed Task<TResult> with SyncContext

                Task.Factory.StartNew(() =>
                {
                    SynchronizationContext.SetSynchronizationContext(new ValidateCorrectContextSynchronizationContext());
                    var mres = new ManualResetEventSlim();
                    var tcs = new TaskCompletionSource<object>();
                    bool hasRun = false;
                    var awaiter = tcs.Task.ConfigureAwait(continueOnCapturedContext).GetAwaiter();
                    Assert.False(awaiter.IsCompleted);
                    awaiter.OnCompleted(() =>
                    {
                        Assert.Equal(continueOnCapturedContext, ValidateCorrectContextSynchronizationContext.IsPostedInContext);
                        hasRun = true;
                        mres.Set();
                    });
                    Assert.False(hasRun);
                    Task.Factory.StartNew(() => tcs.SetResult(null));
                    mres.Wait();
                    awaiter.GetResult();
                    Assert.True(hasRun);
                }, CancellationToken.None, TaskCreationOptions.None, TaskScheduler.Default).Wait();
            }
            finally
            {
                SynchronizationContext.SetSynchronizationContext(previous);
            }
        }

        [Fact]
        [OuterLoop]
        public static void AsyncTaskAwaiter_Future_Test()
        {
            var completed = new TaskCompletionSource<string>();
            Task task = completed.Task;
            Task<string> taskOfString = completed.Task;
            completed.SetResult("42");

            // Validate successful GetResult
            task.GetAwaiter().GetResult();
            task.ConfigureAwait(false).GetAwaiter().GetResult();
            task.ConfigureAwait(true).GetAwaiter().GetResult();

            Assert.Equal("42", taskOfString.GetAwaiter().GetResult());
            Assert.Equal("42", taskOfString.ConfigureAwait(false).GetAwaiter().GetResult());
            Assert.Equal("42", taskOfString.ConfigureAwait(true).GetAwaiter().GetResult());
        }

        [Fact]
        [OuterLoop]
        public static void AsyncTaskAwaiter_Blocks_Test()
        {
            // Validate GetResult blocks until completion
            var tcs = new TaskCompletionSource<bool>();

            // Kick off tasks that should all block
            var t1 = Task.Factory.StartNew(() => tcs.Task.GetAwaiter().GetResult());
            var t2 = Task.Factory.StartNew(() => ((Task)tcs.Task).GetAwaiter().GetResult());
            var t3 = Task.Factory.StartNew(() => tcs.Task.ConfigureAwait(false).GetAwaiter().GetResult());
            var t4 = Task.Factory.StartNew(() => ((Task)tcs.Task).ConfigureAwait(false).GetAwaiter().GetResult());
            var allTasks = new Task[] { t1, t2, t3, t4 };

            // Wait with timeout should return false
            bool waitCompleted = Task.WaitAll(allTasks, 4000);
            Assert.False(waitCompleted);

            // Now complete the tasks
            tcs.SetResult(true);

            // All tasks should complete successfully
            waitCompleted = Task.WaitAll(allTasks, 4000);
            Assert.True(waitCompleted);
            Assert.All(allTasks, task => Assert.Equal(TaskStatus.RanToCompletion, task.Status));
        }

        [Fact]
        public static void RunAsyncTaskAwaiterTests_Exceptions()
        {
            // Validate cancellation
            var canceled = new TaskCompletionSource<string>();
            canceled.SetCanceled();

            // Task.GetAwaiter and Task<T>.GetAwaiter
            Assert.Throws<TaskCanceledException>(() => { ((Task)canceled.Task).GetAwaiter().GetResult(); });
            Assert.Throws<TaskCanceledException>(() => { ((Task<string>)canceled.Task).GetAwaiter().GetResult(); });

            // w/ ConfigureAwait false and true
            Assert.Throws<TaskCanceledException>(() => { ((Task)canceled.Task).ConfigureAwait(false).GetAwaiter().GetResult(); });
            Assert.Throws<TaskCanceledException>(() => { ((Task)canceled.Task).ConfigureAwait(true).GetAwaiter().GetResult(); });
            Assert.Throws<TaskCanceledException>(() => { ((Task<string>)canceled.Task).ConfigureAwait(false).GetAwaiter().GetResult(); });
            Assert.Throws<TaskCanceledException>(() => { ((Task<string>)canceled.Task).ConfigureAwait(true).GetAwaiter().GetResult(); });
        }

        [Fact]
        public static void AsyncTaskAwaiterTests_EqualityTests()
        {
            var completed = new TaskCompletionSource<string>();
            Task task = completed.Task;

            // Validate awaiter and awaitable equality

            // TaskAwaiter
            task.GetAwaiter().Equals(task.GetAwaiter());
            // ConfiguredTaskAwaitable
            Assert.Equal(((Task)task).ConfigureAwait(false), ((Task)task).ConfigureAwait(false));
            Assert.NotEqual(((Task)task).ConfigureAwait(false), ((Task)task).ConfigureAwait(true));
            Assert.NotEqual(((Task)task).ConfigureAwait(true), ((Task)task).ConfigureAwait(false));

            // ConfiguredTaskAwaitable<T>
            Assert.Equal(task.ConfigureAwait(false), ((Task)task).ConfigureAwait(false));
            Assert.NotEqual(task.ConfigureAwait(false), ((Task)task).ConfigureAwait(true));
            Assert.NotEqual(task.ConfigureAwait(true), ((Task)task).ConfigureAwait(false));

            // ConfiguredTaskAwaitable.ConfiguredTaskAwaiter
            Assert.Equal(((Task)task).ConfigureAwait(false).GetAwaiter(), ((Task)task).ConfigureAwait(false).GetAwaiter());
            Assert.NotEqual(((Task)task).ConfigureAwait(false).GetAwaiter(), ((Task)task).ConfigureAwait(true).GetAwaiter());
            Assert.NotEqual(((Task)task).ConfigureAwait(true).GetAwaiter(), ((Task)task).ConfigureAwait(false).GetAwaiter());

            // ConfiguredTaskAwaitable<T>.ConfiguredTaskAwaiter
            Assert.Equal(task.ConfigureAwait(false).GetAwaiter(), ((Task)task).ConfigureAwait(false).GetAwaiter());
            Assert.NotEqual(task.ConfigureAwait(false).GetAwaiter(), ((Task)task).ConfigureAwait(true).GetAwaiter());
            Assert.NotEqual(task.ConfigureAwait(true).GetAwaiter(), ((Task)task).ConfigureAwait(false).GetAwaiter());
        }

        [Fact]
        public static void AsyncTaskAwaiter_SingleException_Test()
        {
            // Validate GetResult on a single exception
            var oneException = new TaskCompletionSource<string>();
            oneException.SetException(new ArgumentException("uh oh"));

            // Task.GetAwaiter and Task<T>.GetAwaiter
            Assert.Throws<ArgumentException>(() => { ((Task)oneException.Task).GetAwaiter().GetResult(); });
            Assert.Throws<ArgumentException>(() => { ((Task<string>)oneException.Task).GetAwaiter().GetResult(); });

            // w/ ConfigureAwait false and true
            Assert.Throws<ArgumentException>(() => { ((Task)oneException.Task).ConfigureAwait(false).GetAwaiter().GetResult(); });
            Assert.Throws<ArgumentException>(() => { ((Task)oneException.Task).ConfigureAwait(true).GetAwaiter().GetResult(); });
            Assert.Throws<ArgumentException>(() => { ((Task<string>)oneException.Task).ConfigureAwait(false).GetAwaiter().GetResult(); });
            Assert.Throws<ArgumentException>(() => { ((Task<string>)oneException.Task).ConfigureAwait(true).GetAwaiter().GetResult(); });
        }

        [Fact]
        public static void AsyncTaskAwaiter_MultipleExceptions_Test()
        {
            // Validate GetResult on multiple exceptions
            var multiException = new TaskCompletionSource<string>();
            multiException.SetException(new Exception[] { new ArgumentException("uh oh"), new InvalidOperationException("uh oh") });

            // Task.GetAwaiter and Task<T>.GetAwaiter
            Assert.Throws<ArgumentException>(() => { ((Task)multiException.Task).GetAwaiter().GetResult(); });
            Assert.Throws<ArgumentException>(() => { ((Task<string>)multiException.Task).GetAwaiter().GetResult(); });

            // w/ ConfigureAwait false and true
            Assert.Throws<ArgumentException>(() => { ((Task)multiException.Task).ConfigureAwait(false).GetAwaiter().GetResult(); });
            Assert.Throws<ArgumentException>(() => { ((Task)multiException.Task).ConfigureAwait(true).GetAwaiter().GetResult(); });
            Assert.Throws<ArgumentException>(() => { ((Task<string>)multiException.Task).ConfigureAwait(false).GetAwaiter().GetResult(); });
            Assert.Throws<ArgumentException>(() => { ((Task<string>)multiException.Task).ConfigureAwait(true).GetAwaiter().GetResult(); });
        }

        [Fact]
        public static void RunAsyncTaskAwaiterAdditionalBehaviorsTests()
        {
            // Test that base SynchronizationContext is treated the same as no SynchronizationContext for awaits
            SynchronizationContext previous = SynchronizationContext.Current;
            try
            {
                var quwi = new QUWITaskScheduler();
                SynchronizationContext.SetSynchronizationContext(new SynchronizationContext());
                RunWithSchedulerAsCurrent(quwi, () =>
                {
                    ManualResetEventSlim mres = new ManualResetEventSlim();
                    var tcs = new TaskCompletionSource<object>();
                    bool hasRun = false;
                    var awaiter = ((Task)tcs.Task).GetAwaiter();
                    Assert.False(awaiter.IsCompleted);
                    Assert.NotNull(SynchronizationContext.Current);
                    awaiter.OnCompleted(() =>
                    {
                        Assert.Equal(quwi, TaskScheduler.Current);
                        Assert.Null(SynchronizationContext.Current);
                        hasRun = true;
                        mres.Set();
                    });
                    Assert.False(hasRun);
                    Task.Run(() => tcs.SetResult(null));
                    mres.Wait();
                    awaiter.GetResult();
                    Assert.True(hasRun);
                });
            }
            finally
            {
                SynchronizationContext.SetSynchronizationContext(previous);
            }
        }

        [Fact]
        public static void AsyncTaskAwaiter_Token_Test()
        {
            // Test that CancellationToken is correctly flowed through await

            var amb = AsyncTaskMethodBuilder.Create();
            var cts = new CancellationTokenSource();
            var oce = new OperationCanceledException(cts.Token);
            amb.SetException(oce);
            var awt = amb.Task.GetAwaiter();
            OperationCanceledException oceToCheck = Assert.Throws<OperationCanceledException>(() => awt.GetResult());
            Assert.Same(oce, oceToCheck);
            Assert.Equal(cts.Token, oceToCheck.CancellationToken);
        }

        #region Helper Methods / Classes

        /// <summary>Runs the action with TaskScheduler.Current equal to the specified scheduler.</summary>
        private static void RunWithSchedulerAsCurrent(TaskScheduler scheduler, Action action)
        {
            var t = new Task(action);
            t.RunSynchronously(scheduler);
            t.Wait();
        }

        #endregion
    }
}
