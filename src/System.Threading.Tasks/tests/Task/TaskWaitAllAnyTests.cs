// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

// Summary: Test suite for the below scenario:
// An array of tasks that can have different workloads
// WaitAny and WaitAll
//      - with/without timeout is performed on them
//      - the allowed delta for cases with timeout:10 ms
// Scheduler used : current ( ThreadPool)
//
// Observations:
// 1. The input data for tasks does not contain any Exceptional or Cancelled tasks.
// 2. WaitAll on array with cancelled tasks can be found at: Functional\TPL\YetiTests\TaskWithYeti\TaskWithYeti.cs
// 3. WaitAny/WaitAll with token tests can be found at:Functional\TPL\YetiTests\TaskCancellation\TaskCancellation.cs

using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Xunit;

namespace System.Threading.Tasks.Tests
{
    public static class TaskWaitAllAnyTests
    {
        private static readonly TimeSpan MaxSafeTimeout = TimeSpan.FromSeconds(5);
        private static readonly TimeSpan DelayRange = TimeSpan.FromMilliseconds(3);

        /// <summary>
        /// Gets a row of data for task tests.
        /// </summary>
        /// <returns>A row of test data.</returns>
        public static IEnumerable<object[]> Task_Data()
        {
            yield return new object[] { new TimeSpan[] { /* nothing */ } };

            foreach (TimeSpan work in new[] { Workloads.VeryLight, Workloads.Light, Workloads.Medium })
            {
                yield return new object[] { new[] { work } };

                foreach (TimeSpan load in new[] { Workloads.VeryLight, Workloads.Light, Workloads.Medium })
                {
                    yield return new object[] { new[] { work, load } };
                }
            }

            yield return new object[] { new[] { Workloads.Medium, Workloads.Light, Workloads.Medium, Workloads.VeryLight, Workloads.Medium } };
        }

        /// <summary>
        /// Gets a row of data for task tests.
        /// </summary>
        /// The format is:
        ///  1. A list of task workloads
        ///  2. The maximum duration to wait (-1 means no timeout)
        /// <returns>A row of test data.</returns>
        public static IEnumerable<object[]> Task_Wait_Data()
        {
            foreach (object[] data in Task_Data())
            {
                foreach (TimeSpan wait in new[] { Waits.Infinite, Waits.Long, Waits.Short, Waits.Instant })
                {
                    yield return new object[] { data[0], wait };
                }
            }
        }

        /// <summary>
        /// Gets a row of data for task tests.
        /// </summary>
        /// <returns>A row of test data.</returns>
        public static IEnumerable<object[]> Task_Data_Longrunning()
        {
            foreach (TimeSpan work in new[] { Workloads.Heavy, Workloads.VeryHeavy })
            {
                yield return new object[] { new[] { work } };

                foreach (TimeSpan load in new[] { Workloads.VeryLight, Workloads.Light, Workloads.Medium })
                {
                    yield return new object[] { new[] { work, load } };
                }
            }

            yield return new object[] { new[] { Workloads.Medium, Workloads.Light, Workloads.Heavy, Workloads.Medium, Workloads.VeryHeavy, Workloads.VeryLight, Workloads.Medium } };
        }

        /// <summary>
        /// Gets a row of data for task tests.
        /// </summary>
        /// The format is:
        ///  1. A list of task workloads
        ///  2. The maximum duration to wait (-1 means no timeout)
        /// <returns>A row of test data.</returns>
        public static IEnumerable<object[]> Task_Wait_Data_Longrunning()
        {
            foreach (object[] data in Task_Data_Longrunning())
            {
                foreach (TimeSpan wait in new[] { Waits.Infinite, Waits.Long, Waits.Short })
                {
                    yield return new object[] { data[0], wait };
                }
            }
        }

        [Theory]
        [MemberData("Task_Data")]
        public static void Task_WaitAll(TimeSpan[] loads)
        {
            WaitAll(loads, Waits.Infinite, (tasks, i) => { Task.WaitAll(tasks); return true; });
        }

        [Theory]
        [OuterLoop]
        [MemberData("Task_Data_Longrunning")]
        public static void Task_WaitAll_Longrunning(TimeSpan[] loads)
        {
            Task_WaitAll(loads);
        }

        [Theory]
        [MemberData("Task_Wait_Data")]
        public static void Task_WaitAll_TimeSpan(TimeSpan[] loads, TimeSpan wait)
        {
            WaitAll(loads, wait, (tasks, w) => Task.WaitAll(tasks, w));
        }

        [Theory]
        [OuterLoop]
        [MemberData("Task_Wait_Data_Longrunning")]
        public static void Task_WaitAll_TimeSpan_Longrunning(TimeSpan[] loads, TimeSpan wait)
        {
            Task_WaitAll_TimeSpan(loads, wait);
        }

        [Theory]
        [MemberData("Task_Wait_Data")]
        public static void Task_WaitAll_Millisecond(TimeSpan[] loads, TimeSpan wait)
        {
            WaitAll(loads, wait, (tasks, w) => Task.WaitAll(tasks, w.Milliseconds));
        }

        [Theory]
        [OuterLoop]
        [MemberData("Task_Wait_Data_Longrunning")]
        public static void Task_WaitAll_Millisecond_Longrunning(TimeSpan[] loads, TimeSpan wait)
        {
            Task_WaitAll_Millisecond(loads, wait);
        }

        private static void WaitAll(TimeSpan[] loads, TimeSpan wait, Func<Task[], TimeSpan, bool> call)
        {
            TimeSpan minimum = loads.Any() ? loads.Max() : TimeSpan.Zero;
            TimeSpan expected = loads.Aggregate(TimeSpan.Zero, (acc, load) => acc + load);

            Stopwatch timer = null;
            bool completed = false;
            Task[] tasks = null;
            // tracker for times a particular task is entered
            int[] called = new int[loads.Length];

            using (Barrier b = new Barrier(loads.Length + 1))
            {
                tasks = CreateAndStartTasks(loads, b, called);
                b.SignalAndWait(MaxSafeTimeout);
                timer = Stopwatch.StartNew();

                completed = call(tasks, wait);
                timer.Stop();
            }

            if (wait == Waits.Infinite || completed)
            {
                Assert.True(completed);
                Assert.All(tasks, task => AssertTaskComplete(task));
                Assert.All(called, run => Assert.Equal(1, run));
                ExpectAndReport(timer.Elapsed, minimum, expected);
            }
            else
            {
                // Given how scheduling and threading in general works,
                // the only guarantee is that WaitAll returned false.
                //   Any of the following may be true:
                //     - Between WaitAll timing out and Asserts, all tasks may start AND complete.
                //     - Tasks may complete after WaitAll times out internally, but before it returns.
                Assert.False(completed);
                Assert.All(called, run => Assert.True(run == 0 || run == 1));
            }
        }

        [Theory]
        [MemberData("Task_Data")]
        public static void Task_WaitAny(TimeSpan[] loads)
        {
            WaitAny(loads, Waits.Infinite, (tasks, i) => Task.WaitAny(tasks));
        }

        [Theory]
        [OuterLoop]
        [MemberData("Task_Data_Longrunning")]
        public static void Task_WaitAny_Longrunning(TimeSpan[] loads)
        {
            Task_WaitAny(loads);
        }

        [Theory]
        [MemberData("Task_Wait_Data")]
        public static void Task_WaitAny_TimeSpan(TimeSpan[] loads, TimeSpan wait)
        {
            WaitAny(loads, wait, (tasks, w) => Task.WaitAny(tasks, w));
        }

        [Theory]
        [OuterLoop]
        [MemberData("Task_Wait_Data_Longrunning")]
        public static void Task_WaitAny_TimeSpan_Longrunning(TimeSpan[] loads, TimeSpan wait)
        {
            Task_WaitAny_TimeSpan(loads, wait);
        }

        [Theory]
        [MemberData("Task_Wait_Data")]
        public static void Task_WaitAny_Millisecond(TimeSpan[] loads, TimeSpan wait)
        {
            WaitAny(loads, wait, (tasks, w) => Task.WaitAny(tasks, w.Milliseconds));
        }

        [Theory]
        [OuterLoop]
        [MemberData("Task_Wait_Data_Longrunning")]
        public static void Task_WaitAny_Millisecond_Longrunning(TimeSpan[] loads, TimeSpan wait)
        {
            Task_WaitAny_Millisecond(loads, wait);
        }

        private static void WaitAny(TimeSpan[] loads, TimeSpan wait, Func<Task[], TimeSpan, int> call)
        {
            TimeSpan expected = loads.Any() ? loads.Min() : TimeSpan.Zero;
            TimeSpan maximum = loads.Any() ? loads.Max() : TimeSpan.Zero;

            Stopwatch timer = null;
            int completed = -1;
            Task[] tasks = null;
            // tracker for times a particular task is entered
            int[] called = new int[loads.Length];

            using (Barrier b = new Barrier(loads.Length + 1))
            {
                tasks = CreateAndStartTasks(loads, b, called);
                b.SignalAndWait(MaxSafeTimeout);
                timer = Stopwatch.StartNew();

                completed = call(tasks, wait);
                timer.Stop();
            }

            if (loads.Any() && (wait == Waits.Infinite || completed >= 0))
            {
                // A task was returned, but any of the following may be true:
                //     - The task returned may not be the first to 'finish'.
                //     - The task returned may not be the earliest in the list (of those that finished).
                Assert.InRange(completed, 0, loads.Length - 1);
                AssertTaskComplete(tasks[completed]);
                Assert.Equal(1, called[completed]);
                ExpectAndReport(timer.Elapsed, expected, maximum);
            }
            else
            {
                // Given how scheduling and threading in general works,
                // the only guarantee is that WaitAny returned "not found" (-1).
                //   Any of the following may be true:
                //     - Between WaitAny timing out and Asserts, any/all tasks may start AND complete.
                //     - Tasks may complete after WaitAny times out internally, but before it returns.
                Assert.Equal(-1, completed);
            }
            Assert.All(called, run => Assert.True(run == 0 || run == 1));
        }

        private static void AssertTaskComplete(Task task)
        {
            Assert.Equal(TaskStatus.RanToCompletion, task.Status);
            Assert.True(task.IsCompleted);
            Assert.False(task.IsCanceled);
            Assert.False(task.IsFaulted);
            Assert.Null(task.Exception);
        }

        private static Task[] CreateAndStartTasks(TimeSpan[] loads, Barrier b, int[] called)
        {
            return loads.Select((load, index) => Task.Factory.StartNew(() => Work(load, b, ref called[index]))).ToArray();
        }

        private static void Work(TimeSpan duration, Barrier b, ref int called)
        {
            Assert.Equal(1, Interlocked.Increment(ref called));
            b.SignalAndWait(MaxSafeTimeout);
            Stopwatch timer = Stopwatch.StartNew();
            Assert.True(SpinWait.SpinUntil(() => timer.Elapsed >= duration, MaxSafeTimeout));
        }

        private static void ExpectAndReport(TimeSpan actual, TimeSpan minimum, TimeSpan maximum)
        {
            if (actual < minimum - DelayRange || actual > maximum + DelayRange)
            {
                Debug.WriteLine("Elapsed time outside of expected range: ({0} - {1}), Actual: {2}", minimum, maximum, actual);
            }
        }
    }
}
