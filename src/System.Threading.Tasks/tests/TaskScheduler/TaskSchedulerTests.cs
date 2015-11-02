// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Diagnostics;
using System.Security;
using Xunit;

namespace System.Threading.Tasks.Tests
{
    //
    // Task scheduler basics.
    //
    public static class TaskSchedulerTests
    {
        // Just ensure we eventually complete when many blocked tasks are created.
        [Fact]
        public static void RunBlockedInjectionTest()
        {
            Debug.WriteLine("* RunBlockedInjectionTest() -- if it deadlocks, it failed");

            using (ManualResetEvent mre = new ManualResetEvent(false))
            {
                // This test needs to be run with a local task scheduler, because it needs to perform
                // the verification based on a known number of initially available threads.
                //
                //
                // @TODO: When we reach the _planB branch we need to add a trick here using ThreadPool.SetMaxThread
                //        to bring down the TP worker count. This is because previous activity in the test process might have
                //        injected workers.
                TaskScheduler tm = TaskScheduler.Default;

                // Create many tasks blocked on the MRE.

                int processorCount = Environment.ProcessorCount;
                Task[] tasks = new Task[processorCount];
                for (int i = 0; i < tasks.Length; i++)
                {
                    tasks[i] = Task.Factory.StartNew(() => mre.WaitOne(), CancellationToken.None, TaskCreationOptions.None, tm);
                }

                // TODO: Evaluate use of safety valve.
                // Create one task that signals the MRE, and wait for it.
                Assert.True(Task.Factory.StartNew(() => mre.Set(), CancellationToken.None, TaskCreationOptions.None, tm).Wait(TimeSpan.FromMinutes(10)));

                // Lastly, wait for the others to complete.
                Assert.True(Task.WaitAll(tasks, TimeSpan.FromMinutes(10)));
            }
        }

        [Fact]
        public static void BuggyScheduler_Start_Test()
        {
            BuggyTaskScheduler bts = new BuggyTaskScheduler();
            Task task = new Task(() => { /* do nothing */ });

            Assert.Throws<TaskSchedulerException>(() => task.Start(bts));
            Assert.Equal(TaskStatus.Faulted, task.Status);
            AggregateException ae = Assert.Throws<AggregateException>(() => task.Wait());
            Assert.IsType<TaskSchedulerException>(ae.InnerException);
        }

        [Fact]
        public static void BuggyScheduler_RunSynchronously_Test()
        {
            BuggyTaskScheduler bts = new BuggyTaskScheduler();
            Task task = new Task(() => { /* do nothing */ });

            Assert.Throws<TaskSchedulerException>(() => task.RunSynchronously(bts));
            Assert.Equal(TaskStatus.Faulted, task.Status);
            AggregateException ae = Assert.Throws<AggregateException>(() => task.Wait());
            Assert.IsType<TaskSchedulerException>(ae.InnerException);
        }

        [Fact]
        public async static void BuggyScheduler_StartNew_Test()
        {
            BuggyTaskScheduler bts = new BuggyTaskScheduler();

            await Assert.ThrowsAsync<TaskSchedulerException>(() => Task.Factory.StartNew(() => { /* do nothing */ },
                CancellationToken.None, TaskCreationOptions.None, bts));
        }

        [Fact]
        public static void BuggyScheduler_ContinueWith_Synchronous_Test()
        {
            BuggyTaskScheduler bts = new BuggyTaskScheduler();

            Task completedTask = Task.Factory.StartNew(() => { /* do nothing */ });
            completedTask.Wait();

            Task continuation = completedTask.ContinueWith(ignore => { /* do nothing */ }, CancellationToken.None, TaskContinuationOptions.ExecuteSynchronously, bts);

            AggregateException ae = Assert.Throws<AggregateException>(() => continuation.Wait());
            Assert.IsType<TaskSchedulerException>(ae.InnerException);
        }

        [Fact]
        public static void BuggyScheduler_ContinueWith_Test()
        {
            BuggyTaskScheduler bts = new BuggyTaskScheduler();

            Task completedTask = Task.Factory.StartNew(() => { /* do nothing */ });
            completedTask.Wait();

            Task continuation = completedTask.ContinueWith(ignore => { /* do nothing */ }, CancellationToken.None, TaskContinuationOptions.None, bts);

            AggregateException ae = Assert.Throws<AggregateException>(() => continuation.Wait());
            Assert.IsType<TaskSchedulerException>(ae.InnerException);
        }

        [Fact]
        public static void BuggyScheduler_Inlining_Test()
        {
            // won't throw on QueueTask
            BuggyTaskScheduler bts2 = new BuggyTaskScheduler(false);

            Task task = new Task(() => { /* do nothing */ });
            task.Start(bts2);

            Assert.Throws<TaskSchedulerException>(() => task.Wait());
        }

        [Fact]
        [OuterLoop]
        public static void SynchronizationContext_TaskScheduler_Wait_Test()
        {
            // Remember the current SynchronizationContext, so it can be restored
            SynchronizationContext previousSC = SynchronizationContext.Current;
            try
            {
                // Now make up a "real" SynchronizationContext and install it
                SimpleSynchronizationContext newSC = new SimpleSynchronizationContext();
                SynchronizationContext.SetSynchronizationContext(newSC);

                // Create a scheduler based on the current SC
                TaskScheduler scTS = TaskScheduler.FromCurrentSynchronizationContext();

                //
                // Launch a Task on scTS, make sure that it is processed in the expected fashion
                //
                bool sideEffect = false;
                Task task = Task.Factory.StartNew(() => { sideEffect = true; }, CancellationToken.None, TaskCreationOptions.None, scTS);

                task.Wait();

                Assert.True(task.IsCompleted, "Expected task to have completed");
                Assert.True(sideEffect, "Task appears not to have run");
                Assert.Equal(1, newSC.PostCount);

                Assert.Equal(1, scTS.MaximumConcurrencyLevel);
            }
            finally
            {
                // restore original SC
                SynchronizationContext.SetSynchronizationContext(previousSC);
            }
        }

        [Fact]
        [OuterLoop]
        public static void SynchronizationContext_TaskScheduler_Synchronous_Test()
        {
            // Remember the current SynchronizationContext, so it can be restored
            SynchronizationContext previousSC = SynchronizationContext.Current;
            try
            {
                // Now make up a "real" SynchronizationContext and install it
                SimpleSynchronizationContext newSC = new SimpleSynchronizationContext();
                SynchronizationContext.SetSynchronizationContext(newSC);

                // Create a scheduler based on the current SC
                TaskScheduler scTS = TaskScheduler.FromCurrentSynchronizationContext();

                //
                // Run a Task synchronously on scTS, make sure that it completes
                //
                bool sideEffect = false;
                Task syncTask = new Task(() => { sideEffect = true; });

                syncTask.RunSynchronously(scTS);

                Assert.True(syncTask.IsCompleted, "Expected task to have completed");
                Assert.True(sideEffect, "Task appears not to have run");
                Assert.Equal(0, newSC.PostCount);

                //
                // Miscellaneous things to test
                //
                Assert.Equal(1, scTS.MaximumConcurrencyLevel);
            }
            finally
            {
                // restore original SC
                SynchronizationContext.SetSynchronizationContext(previousSC);
            }
        }

        [Fact]
        public static void RunSynchronizationContextTaskSchedulerTests_Negative()
        {
            // Remember the current SynchronizationContext, so it can be restored
            SynchronizationContext previousSC = SynchronizationContext.Current;
            try
            {
                //
                // Test exceptions on construction of SCTaskScheduler
                //
                SynchronizationContext.SetSynchronizationContext(null);
                Assert.Throws<InvalidOperationException>(
                   () => { TaskScheduler.FromCurrentSynchronizationContext(); });
            }
            finally
            {
                // restore original SC
                SynchronizationContext.SetSynchronizationContext(previousSC);
            }
        }

        #region Helper Methods / Helper Classes

        // Buggy task scheduler to make sure that QueueTask()/TryExecuteTaskInline()
        // exceptions are handled correctly.  Used in RunBuggySchedulerTests() below.
        [SecuritySafeCritical]
        private class BuggyTaskScheduler : TaskScheduler
        {
            private bool _faultQueues;

            [SecurityCritical]
            protected override void QueueTask(Task task)
            {
                if (_faultQueues)
                    throw new InvalidOperationException("I don't queue tasks!");
                // else do nothing -- still a pretty buggy scheduler!!
            }

            [SecurityCritical]
            protected override bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued)
            {
                throw new ArgumentException("I am your worst nightmare!");
            }

            [SecurityCritical]
            protected override IEnumerable<Task> GetScheduledTasks()
            {
                return null;
            }

            public BuggyTaskScheduler()
                : this(true)
            {
            }

            public BuggyTaskScheduler(bool faultQueues)
            {
                _faultQueues = faultQueues;
            }
        }

        private class SimpleSynchronizationContext : SynchronizationContext
        {
            private int _postCount = 0;

            public override void Post(SendOrPostCallback d, object state)
            {
                Interlocked.Increment(ref _postCount);
                base.Post(d, state);
            }

            public int PostCount { get { return _postCount; } }
        }

        #endregion
    }
}
