// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

// =+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+
//
// Test class that verifies the integration with APM (Task => APM) section 2.5.11 in the TPL spec
// "Asynchronous Programming Model", or the "Begin/End" pattern
//
// =-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-

using Xunit;

namespace System.Threading.Tasks.Tests
{
    /// <summary>
    /// A class that implements the APM pattern to ensure that TPL can support APM patttern
    /// </summary>
    public static class TaskAPMTests
    {
        /// <summary>
        /// The constant that defines the number of milliseconds to spinwait (to simulate work)
        /// </summary>
        private const int WorkTimeMilliseconds = 100;

        // Maximum time to wait, for safety
        private const int MaxWaitTime = 1000;

        [Fact]
        [OuterLoop]
        public static void WaitUntilComplete_Task()
        {
            Task task = Task.Factory.StartNew(Spin);
            task.Wait();

            AssertComplete(task);
        }

        [Fact]
        [OuterLoop]
        public static void WaitUntilComplete_FutureT()
        {
            Task<bool> task = Task.Factory.StartNew(() => SpinAndReturn(true));
            task.Wait();

            AssertComplete(task);
            // Checking result after completion means the call to Result won't block.
            Assert.True(task.Result);
        }

        [Fact]
        [OuterLoop]
        public static void PollUntilComplete_Task()
        {
            Task task = Task.Factory.StartNew(Spin);
            while (!task.IsCompleted)
            {
                Task.Delay(300).Wait();
            }

            AssertComplete(task);
        }

        [Fact]
        [OuterLoop]
        public static void PollUntilComplete_Future()
        {
            Task<bool> task = Task.Factory.StartNew(() => SpinAndReturn(true));
            while (!task.IsCompleted)
            {
                Task.Delay(300).Wait();
            }

            AssertComplete(task);
            Assert.True(task.Result);
        }

        [Fact]
        [OuterLoop]
        public static void WaitOnAsyncWaitHandle_Task()
        {
            Task task = Task.Factory.StartNew(Spin);
            ((IAsyncResult)task).AsyncWaitHandle.WaitOne();

            AssertComplete(task);
        }

        [Fact]
        [OuterLoop]
        public static void WaitOnAsyncWaitHandle_Future()
        {
            Task<bool> task = Task.Factory.StartNew(() => SpinAndReturn(true));
            ((IAsyncResult)task).AsyncWaitHandle.WaitOne();

            AssertComplete(task);
            Assert.True(task.Result);
        }

        [Fact]
        [OuterLoop]
        public static void Callback_Task()
        {
            using (ManualResetEventSlim mre = new ManualResetEventSlim(false))
            {
                Task task = Task.Factory.StartNew(Spin);
                task.ContinueWith(ignored =>
                {
                    AssertComplete(task);
                    mre.Set();
                });

                //Block the main thread until async thread finishes executing the call back
                Assert.True(mre.Wait(MaxWaitTime));

                AssertComplete(task);
            }
        }

        [Fact]
        [OuterLoop]
        public static void Callback_Future()
        {
            using (ManualResetEventSlim mre = new ManualResetEventSlim(false))
            {
                Task<bool> task = Task.Factory.StartNew(() => SpinAndReturn(true));
                task.ContinueWith(completed =>
                {
                    AssertComplete(task);
                    Assert.True(((Task<bool>)completed).Result);

                    mre.Set();
                });

                //Block the main thread until async thread finishes executing the call back
                Assert.True(mre.Wait(MaxWaitTime));

                AssertComplete(task);
            }
        }

        private static void AssertComplete<T>(T task) where T : Task
        {
            Assert.True(task.IsCompleted);
            Assert.Equal(TaskStatus.RanToCompletion, task.Status);
            Assert.False(((IAsyncResult)task).CompletedSynchronously, "Should not have completed synchronously.");
        }

        /// <summary>
        /// Simulate workload by spinning for the given time
        /// </summary>
        private static void Spin()
        {
            SpinWait.SpinUntil(() => false, WorkTimeMilliseconds);
        }

        /// <summary>
        /// Simulate workload by spinning for the given time, then returning the given value
        /// </summary>
        /// <typeparam name="T">Type of the given value</typeparam>
        /// <param name="value">The value to return</param>
        /// <returns>Simulated result of work</returns>
        private static T SpinAndReturn<T>(T value)
        {
            Spin();
            return value;
        }
    }
}
