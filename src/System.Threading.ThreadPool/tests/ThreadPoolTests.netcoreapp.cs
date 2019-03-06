// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading.Tests;
using Xunit;

namespace System.Threading.ThreadPools.Tests
{
    public partial class ThreadPoolTests
    {
        public static IEnumerable<object[]> OneBool() =>
            from b in new[] { true, false }
            select new object[] { b };

        public static IEnumerable<object[]> TwoBools() =>
            from b1 in new[] { true, false }
            from b2 in new[] { true, false }
            select new object[] { b1, b2 };

        [Theory]
        [MemberData(nameof(TwoBools))]
        public void QueueUserWorkItem_PreferLocal_InvalidArguments_Throws(bool preferLocal, bool useUnsafe)
        {
            AssertExtensions.Throws<ArgumentNullException>("callBack", () => useUnsafe ?
                ThreadPool.UnsafeQueueUserWorkItem(null, new object(), preferLocal) :
                ThreadPool.QueueUserWorkItem(null, new object(), preferLocal));
        }

        [Theory]
        [MemberData(nameof(TwoBools))]
        public async Task QueueUserWorkItem_PreferLocal_NullValidForState(bool preferLocal, bool useUnsafe)
        {
            var tcs = new TaskCompletionSource<int>();
            if (useUnsafe)
            {
                ThreadPool.UnsafeQueueUserWorkItem(s => tcs.SetResult(84), (object)null, preferLocal);
            }
            else
            {
                ThreadPool.QueueUserWorkItem(s => tcs.SetResult(84), (object)null, preferLocal);
            }
            Assert.Equal(84, await tcs.Task);
        }

        [Theory]
        [MemberData(nameof(TwoBools))]
        public async Task QueueUserWorkItem_PreferLocal_ReferenceTypeStateObjectPassedThrough(bool preferLocal, bool useUnsafe)
        {
            var tcs = new TaskCompletionSource<int>();
            if (useUnsafe)
            {
                ThreadPool.UnsafeQueueUserWorkItem(s => s.SetResult(84), tcs, preferLocal);
            }
            else
            {
                ThreadPool.QueueUserWorkItem(s => s.SetResult(84), tcs, preferLocal);
            }
            Assert.Equal(84, await tcs.Task);
        }

        [Theory]
        [MemberData(nameof(TwoBools))]
        public async Task QueueUserWorkItem_PreferLocal_ValueTypeStateObjectPassedThrough(bool preferLocal, bool useUnsafe)
        {
            var tcs = new TaskCompletionSource<int>();
            if (useUnsafe)
            {
                ThreadPool.UnsafeQueueUserWorkItem(s => s.tcs.SetResult(s.value), (tcs, value: 42), preferLocal);
            }
            else
            {
                ThreadPool.QueueUserWorkItem(s => s.tcs.SetResult(s.value), (tcs, value: 42), preferLocal);
            }
            Assert.Equal(42, await tcs.Task);
        }

        [Theory]
        [MemberData(nameof(TwoBools))]
        public async Task QueueUserWorkItem_PreferLocal_RunsAsynchronously(bool preferLocal, bool useUnsafe)
        {
            await Task.Factory.StartNew(() =>
            {
                int origThread = Environment.CurrentManagedThreadId;
                var tcs = new TaskCompletionSource<int>();
                if (useUnsafe)
                {
                    ThreadPool.UnsafeQueueUserWorkItem(s => s.SetResult(Environment.CurrentManagedThreadId), tcs, preferLocal);
                }
                else
                {
                    ThreadPool.QueueUserWorkItem(s => s.SetResult(Environment.CurrentManagedThreadId), tcs, preferLocal);
                }
                Assert.NotEqual(origThread, tcs.Task.GetAwaiter().GetResult());
            }, CancellationToken.None, TaskCreationOptions.LongRunning, TaskScheduler.Default);
        }

        [Theory]
        [MemberData(nameof(TwoBools))]
        public async Task QueueUserWorkItem_PreferLocal_ExecutionContextFlowedIfSafe(bool preferLocal, bool useUnsafe)
        {
            var tcs = new TaskCompletionSource<int>();
            var asyncLocal = new AsyncLocal<int>() { Value = 42 };
            if (useUnsafe)
            {
                ThreadPool.UnsafeQueueUserWorkItem(s => s.SetResult(asyncLocal.Value), tcs, preferLocal);
            }
            else
            {
                ThreadPool.QueueUserWorkItem(s => s.SetResult(asyncLocal.Value), tcs, preferLocal);
            }
            asyncLocal.Value = 0;
            Assert.Equal(useUnsafe ? 0 : 42, await tcs.Task);
        }

        [Theory]
        [MemberData(nameof(OneBool))]
        public void UnsafeQueueUserWorkItem_IThreadPoolWorkItem_Invalid_Throws(bool preferLocal)
        {
            AssertExtensions.Throws<ArgumentNullException>("callBack", () => ThreadPool.UnsafeQueueUserWorkItem(null, preferLocal));
            AssertExtensions.Throws<ArgumentOutOfRangeException>("callBack", () => ThreadPool.UnsafeQueueUserWorkItem(new InvalidWorkItemAndTask(() => { }), preferLocal));
        }

        [Theory]
        [MemberData(nameof(OneBool))]
        public async Task UnsafeQueueUserWorkItem_IThreadPoolWorkItem_ManyIndividualItems_AllInvoked(bool preferLocal)
        {
            TaskCompletionSource<bool>[] tasks = Enumerable.Range(0, 100).Select(_ => new TaskCompletionSource<bool>()).ToArray();
            for (int i = 0; i < tasks.Length; i++)
            {
                int localI = i;
                ThreadPool.UnsafeQueueUserWorkItem(new SimpleWorkItem(() =>
                {
                    tasks[localI].TrySetResult(true);
                }), preferLocal);
            }
            await Task.WhenAll(tasks.Select(t => t.Task));
        }

        [Theory]
        [MemberData(nameof(OneBool))]
        public async Task UnsafeQueueUserWorkItem_IThreadPoolWorkItem_SameObjectReused_AllInvoked(bool preferLocal)
        {
            const int Iters = 100;
            int remaining = Iters;
            var tcs = new TaskCompletionSource<bool>();
            var workItem = new SimpleWorkItem(() =>
            {
                if (Interlocked.Decrement(ref remaining) == 0)
                {
                    tcs.TrySetResult(true);
                }
            });
            for (int i = 0; i < Iters; i++)
            {
                ThreadPool.UnsafeQueueUserWorkItem(workItem, preferLocal);
            }
            await tcs.Task;
            Assert.Equal(0, remaining);
        }

        [Theory]
        [MemberData(nameof(OneBool))]
        public async Task UnsafeQueueUserWorkItem_IThreadPoolWorkItem_ExecutionContextNotFlowed(bool preferLocal)
        {
            var al = new AsyncLocal<int> { Value = 42 };
            var tcs = new TaskCompletionSource<bool>();
            ThreadPool.UnsafeQueueUserWorkItem(new SimpleWorkItem(() =>
            {
                Assert.Equal(0, al.Value);
                tcs.TrySetResult(true);
            }), preferLocal);
            await tcs.Task;
            Assert.Equal(42, al.Value);
        }

        private sealed class SimpleWorkItem : IThreadPoolWorkItem
        {
            private readonly Action _action;
            public SimpleWorkItem(Action action) => _action = action;
            public void Execute() => _action();
        }

        private sealed class InvalidWorkItemAndTask : Task, IThreadPoolWorkItem
        {
            public InvalidWorkItemAndTask(Action action) : base(action) { }
            public void Execute() { }
        }

        [Fact]
        public void MetricsTest()
        {
            int processorCount = Environment.ProcessorCount;

            var workStarted = new AutoResetEvent(false);
            int completeWork = 0;
            int simultaneousWorkCount = 0;
            int simultaneousLocalWorkCount = 0;
            int simultaneousGlobalWorkCount = 0;
            var allWorkCompleted = new ManualResetEvent(false);
            Exception backgroundEx = null;
            Action work = () =>
            {
                workStarted.Set();
                try
                {
                    // Blocking can affect thread pool thread injection heuristics, so don't block, pretend like a
                    // long-running CPU-bound work item
                    ThreadTestHelpers.WaitForConditionWithoutRelinquishingTimeSlice(
                        () => Interlocked.CompareExchange(ref completeWork, 0, 0) != 0);
                }
                catch (Exception ex)
                {
                    Interlocked.CompareExchange(ref backgroundEx, ex, null);
                }
                finally
                {
                    if (Interlocked.Decrement(ref simultaneousWorkCount) == 0)
                    {
                        allWorkCompleted.Set();
                    }
                }
            };
            WaitCallback threadPoolWork = data => work();
            Action<object> threadPoolLocalWork = data => work();
            TimerCallback timerWork = data => work();
            WaitOrTimerCallback waitWork = (data, timedOut) => work();

            var signaledEvent = new ManualResetEvent(true);
            var timers = new List<Timer>();
            int maxSimultaneousWorkCount = 0;
            Action scheduleWork = () =>
            {
                Assert.True(simultaneousWorkCount <= maxSimultaneousWorkCount);

                while (true)
                {
                    if (simultaneousWorkCount >= maxSimultaneousWorkCount)
                    {
                        break;
                    }
                    ++simultaneousWorkCount;
                    ++simultaneousGlobalWorkCount;
                    ThreadPool.QueueUserWorkItem(threadPoolWork);
                    workStarted.CheckedWait();

                    if (simultaneousWorkCount >= maxSimultaneousWorkCount)
                    {
                        break;
                    }
                    ++simultaneousWorkCount;
                    ++simultaneousLocalWorkCount;
                    ThreadPool.QueueUserWorkItem(threadPoolLocalWork, null, preferLocal: true);
                    workStarted.CheckedWait();

                    if (simultaneousWorkCount >= maxSimultaneousWorkCount)
                    {
                        break;
                    }
                    ++simultaneousWorkCount;
                    ++simultaneousGlobalWorkCount;
                    timers.Add(new Timer(timerWork, null, 1, Timeout.Infinite));
                    workStarted.CheckedWait();

                    if (simultaneousWorkCount >= maxSimultaneousWorkCount)
                    {
                        break;
                    }
                    ++simultaneousWorkCount;
                    ++simultaneousGlobalWorkCount;
                    ThreadPool.RegisterWaitForSingleObject(
                        signaledEvent,
                        waitWork,
                        null,
                        ThreadTestHelpers.UnexpectedTimeoutMilliseconds,
                        true);
                    workStarted.CheckedWait();
                }

                Assert.Equal(maxSimultaneousWorkCount, simultaneousWorkCount);
            };

            long initialCompletedWorkItemCount = ThreadPool.CompletedWorkItemCount;

            // Schedule some simultaneous work that would all be scheduled and verify the thread count
            maxSimultaneousWorkCount = Math.Max(1, processorCount - 1); // minus one in case this thread is a thread pool thread
            scheduleWork();
            Assert.True(ThreadPool.ThreadCount >= maxSimultaneousWorkCount);

            // Schedule more work that would not all be scheduled and roughly verify the pending work item count
            maxSimultaneousWorkCount = processorCount * 64;
            scheduleWork();
            // The following is assuming that no more than (processorCount * 8) work items will be scheduled to run
            // simultaneously
            int minExpectedPendingLocalWorkCount = Math.Max(1, simultaneousLocalWorkCount - processorCount * 8);
            int minExpectedPendingGlobalWorkCount = Math.Max(1, simultaneousGlobalWorkCount - processorCount * 8);
            int minExpectedPendingWorkCount = minExpectedPendingLocalWorkCount + minExpectedPendingGlobalWorkCount;
            Assert.True(ThreadPool.PendingLocalWorkItemCount >= minExpectedPendingLocalWorkCount);
            Assert.True(ThreadPool.PendingGlobalWorkItemCount >= minExpectedPendingGlobalWorkCount);
            Assert.True(ThreadPool.PendingWorkItemCount >= minExpectedPendingWorkCount);

            // Complete the work and verify the completed work item count
            Interlocked.Exchange(ref completeWork, 1);
            allWorkCompleted.CheckedWait();
            backgroundEx = Interlocked.CompareExchange(ref backgroundEx, null, null);
            if (backgroundEx != null)
            {
                throw new AggregateException(backgroundEx);
            }
            // Wait for work items to exit, for counting
            ThreadTestHelpers.WaitForCondition(() =>
                ThreadPool.CompletedWorkItemCount - initialCompletedWorkItemCount >= maxSimultaneousWorkCount);
        }
    }
}
