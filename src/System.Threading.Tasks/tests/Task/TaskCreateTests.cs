// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

// =+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+
//
// Test class using UnitTestDriver that ensures all the public ctor of Task, Future and
// promise are properly working
//
// =-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-

using System.Collections.Generic;
using Xunit;

namespace System.Threading.Tasks.Tests
{
    public static class TaskCreateTests
    {
        /// <summary>
        /// Get FutureT tasks to start
        /// </summary>
        /// Format of returned data is:
        /// 1. String label (ignored)
        /// 2. Task
        /// <returns>Test data</returns>
        public static IEnumerable<object[]> FutureT_Data()
        {
            yield return new object[] { "Task<bool>", new Task<bool>(() => true) };
            yield return new object[] { "Task<bool>|CancellationToken", new Task<bool>(() => true, new CancellationTokenSource().Token) };
            yield return new object[] { "Task<bool>|TaskCreationOptions", new Task<bool>(() => true, TaskCreationOptions.None) };
            yield return new object[] { "Task<bool>|CancellationToken|TaskCreationOptions",
                 new Task<bool>(() => true, new CancellationTokenSource().Token, TaskCreationOptions.None) };
            yield return new object[] { "Task<bool>|state", new Task<bool>(state => { Assert.Equal(1, state); return true; }, 1) };
            yield return new object[] { "Task<bool>|state|CancellationToken",
                new Task<bool>(state => { Assert.Equal(1, state); return true; }, 1, new CancellationTokenSource().Token) };
            yield return new object[] { "Task<bool>|state|TaskCreationOptions",
                new Task<bool>(state => { Assert.Equal(1, state); return true; }, 1, TaskCreationOptions.None) };
            yield return new object[] { "Task<bool>|state|CancellationToken|TaskCreationOptions",
                new Task<bool>(state => { Assert.Equal(1, state); return true; }, 1, new CancellationTokenSource().Token, TaskCreationOptions.None) };
        }

        /// <summary>
        /// Get Tasks to start
        /// </summary>
        /// Format of returned data is:
        /// 1. String label (ignored)
        /// 2. Func to create a task that modifies a flag
        /// <returns>Test data</returns>
        public static IEnumerable<object[]> Task_Data()
        {
            yield return new object[] { "Task", (Func<Flag, Task>)(flag => new Task(() => flag.Trip())) };
            yield return new object[] { "Task|CancellationToken", (Func<Flag, Task>)(flag => new Task(() => flag.Trip(), new CancellationTokenSource().Token)) };
            yield return new object[] { "Task|TaskCreationOptions", (Func<Flag, Task>)(flag => new Task(() => flag.Trip(), TaskCreationOptions.None)) };
            yield return new object[] { "Task|CancellationToken|TaskCreationOptions",
                (Func<Flag, Task>)( flag => new Task(() => flag.Trip(), new CancellationTokenSource().Token, TaskCreationOptions.None)) };
            yield return new object[] { "Task|state", (Func<Flag, Task>)(flag => new Task(state => { Assert.Equal(1, state); flag.Trip(); }, 1)) };
            yield return new object[] { "Task|state|CancellationToken",
                (Func<Flag, Task>)(flag => new Task(state => { Assert.Equal(1, state); flag.Trip(); }, 1, new CancellationTokenSource().Token)) };
            yield return new object[] { "Task|state|TaskCreationOptions",
                (Func<Flag, Task>)(flag => new Task(state => { Assert.Equal(1, state); flag.Trip(); }, 1, TaskCreationOptions.None)) };
            yield return new object[] { "Task|state|CancellationToken|TaskCreationOptions",
                (Func<Flag, Task>)(flag => new Task(state => { Assert.Equal(1, state); flag.Trip(); }, 1, new CancellationTokenSource().Token, TaskCreationOptions.None)) };
        }

        /// <summary>
        /// Get FutureT tasks submitted via StartNew
        /// </summary>
        /// Format of returned data is:
        /// 1. String label (ignored)
        /// 2. Task (already running)
        /// <returns>Test data</returns>
        public static IEnumerable<object[]> FutureT_Started_Data()
        {
            yield return new object[] { "Task<bool>", Task<bool>.Factory.StartNew(() => true) };
            yield return new object[] { "Task<bool>|CancellationToken", Task<bool>.Factory.StartNew(() => true, new CancellationTokenSource().Token) };
            yield return new object[] { "Task<bool>|TaskCreationOptions", Task<bool>.Factory.StartNew(() => true, TaskCreationOptions.None) };
            yield return new object[] { "Task<bool>|CancellationToken|TaskCreationOptions",
                Task<bool>.Factory.StartNew(() => true, new CancellationTokenSource().Token, TaskCreationOptions.None, TaskScheduler.Default) };
            yield return new object[] { "Task<bool>|state", Task<bool>.Factory.StartNew(state => { Assert.Equal(1, state); return true; }, 1) };
            yield return new object[] { "Task<bool>|state|CancellationToken",
                Task<bool>.Factory.StartNew(state => { Assert.Equal(1, state); return true; }, 1, new CancellationTokenSource().Token) };
            yield return new object[] { "Task<bool>|state|TaskCreationOptions",
                Task<bool>.Factory.StartNew(state => { Assert.Equal(1, state); return true; }, 1, TaskCreationOptions.None) };
            yield return new object[] { "Task<bool>|state|CancellationToken|TaskCreationOptions",
                Task<bool>.Factory.StartNew(state => { Assert.Equal(1, state); return true; }, 1, new CancellationTokenSource().Token, TaskCreationOptions.None, TaskScheduler.Default) };

            yield return new object[] { "StartNew<bool>", Task.Factory.StartNew(() => true) };
            yield return new object[] { "StartNew<bool>|CancellationToken", Task.Factory.StartNew(() => true, new CancellationTokenSource().Token) };
            yield return new object[] { "StartNew<bool>|TaskCreationOptions", Task.Factory.StartNew(() => true, TaskCreationOptions.None) };
            yield return new object[] { "StartNew<bool>|CancellationToken|TaskCreationOptions|TaskScheduler",
                Task.Factory.StartNew(() => true, new CancellationTokenSource().Token, TaskCreationOptions.None, TaskScheduler.Default) };
            yield return new object[] { "StartNew<bool>|state", Task.Factory.StartNew(state => { Assert.Equal(1, state); return true; }, 1) };
            yield return new object[] { "StartNew<bool>|state|CancellationToken",
                Task.Factory.StartNew(state => { Assert.Equal(1, state); return true; }, 1, new CancellationTokenSource().Token) };
            yield return new object[] { "StartNew<bool>|state|TaskCreationOptions",
                Task.Factory.StartNew(state => { Assert.Equal(1, state); return true; }, 1, TaskCreationOptions.None) };
            yield return new object[] { "StartNew<bool>|state|CancellationToken|TaskCreationOptions|TaskScheduler",
                Task.Factory.StartNew(state => { Assert.Equal(1, state); return true; }, 1, new CancellationTokenSource().Token, TaskCreationOptions.None, TaskScheduler.Default) };
        }

        /// <summary>
        /// Get Tasks to be submitted via StartNew
        /// </summary>
        /// Format of returned data is:
        /// 1. String label (ignored)
        /// 2. Func to create a running task that modifies a flag
        /// <returns>Test data</returns>
        public static IEnumerable<object[]> Task_Started_Data()
        {
            yield return new object[] { "Task", (Func<Flag, Task>)(flag => Task.Factory.StartNew(() => flag.Trip())) };
            yield return new object[] { "Task|CancellationToken", (Func<Flag, Task>)(flag => Task.Factory.StartNew(() => flag.Trip(), new CancellationTokenSource().Token)) };
            yield return new object[] { "Task|TaskCreationOptions", (Func<Flag, Task>)(flag => Task.Factory.StartNew(() => flag.Trip(), TaskCreationOptions.None)) };
            yield return new object[] { "Task|CancellationToken|TaskCreationOptions|TaskScheduler",
                (Func<Flag, Task>)( flag => Task.Factory.StartNew(() => flag.Trip(), new CancellationTokenSource().Token, TaskCreationOptions.None,TaskScheduler.Default)) };
            yield return new object[] { "Task|state", (Func<Flag, Task>)(flag => Task.Factory.StartNew(state => { Assert.Equal(1, state); flag.Trip(); }, 1)) };
            yield return new object[] { "Task|state|CancellationToken",
                (Func<Flag, Task>)(flag => Task.Factory.StartNew(state => { Assert.Equal(1, state); flag.Trip(); }, 1, new CancellationTokenSource().Token)) };
            yield return new object[] { "Task|state|TaskCreationOptions",
                (Func<Flag, Task>)(flag => Task.Factory.StartNew(state => { Assert.Equal(1, state); flag.Trip(); }, 1, TaskCreationOptions.None)) };
            yield return new object[] { "Task|state|CancellationToken|TaskCreationOptions|TaskScheduler",
                (Func<Flag, Task>)(flag => Task.Factory.StartNew(state => { Assert.Equal(1, state); flag.Trip(); }, 1, new CancellationTokenSource().Token, TaskCreationOptions.None, TaskScheduler.Default)) };
        }

        [Theory]
        [MemberData("Task_Data")]
        public static void Task_Create_Test(string label, Func<Flag, Task> create)
        {
            Future_Create_Test(label, create(new Flag()));
        }

        [Theory]
        [MemberData("FutureT_Data")]
        public static void Future_Create_Test<T>(string label, T task) where T : Task
        {
            Assert.NotNull(task);
            Assert.False(task.IsCanceled);
            Assert.False(task.IsCompleted);
            Assert.False(task.IsFaulted);
            Assert.Equal(TaskStatus.Created, task.Status);
            // Required so Xunit doesn't complain during dispose
            task.RunSynchronously();
        }

        [Fact]
        public static void TaskCancellable_Test()
        {
            AssertCancellable(token => new Task<bool>(() => true, token));
            AssertCancellable(token => new Task<bool>(() => true, token, TaskCreationOptions.None));
            AssertCancellable(token => new Task<bool>(ignored => true, new object(), token, TaskCreationOptions.None));
            AssertCancellable(token => new Task(() => { }, token));
            AssertCancellable(token => new Task(() => { }, token, TaskCreationOptions.None));
            AssertCancellable(token => new Task(ignored => { }, new object(), token, TaskCreationOptions.None));
        }

        private static void AssertCancellable<T>(Func<CancellationToken, T> create) where T : Task
        {
            CancellationTokenSource source = new CancellationTokenSource();
            T task = create(source.Token);
            source.Cancel();

            Assert.True(task.IsCanceled);
            Assert.Equal(TaskStatus.Canceled, task.Status);
        }

        [Fact]
        public static void Create_Promise_Test()
        {
            Assert.NotNull(new TaskCompletionSource<object>(new object(), TaskCreationOptions.None).Task);
            Assert.NotNull(new TaskCompletionSource<object>(TaskCreationOptions.None).Task);
            Assert.NotNull(new TaskCompletionSource<object>(new object()).Task);
            Assert.NotNull(new TaskCompletionSource<object>().Task);
        }

        [Theory]
        [MemberData("FutureT_Started_Data")]
        public async static void Future_StartNew_Test(string label, Task<bool> task)
        {
            Assert.True(await task);
        }

        [Theory]
        [MemberData("Task_Started_Data")]
        public static void Task_StartNew_Test(string label, Func<Flag, Task> create)
        {
            AssertCompletes(create);
        }

        private static void AssertCompletes(Func<Flag, Task> create)
        {
            Flag flag = new Flag();
            Task task = create(flag);
            task.Wait();
            Assert.True(flag.IsTripped);
        }

        [Theory]
        [MemberData("FutureT_Data")]
        public async static void Future_Start_Test(string label, Task<bool> future)
        {
            Assert.True(await Start(future));
        }

        [Theory]
        [MemberData("FutureT_Data")]
        public async static void Future_Start_Scheduler_Test(string label, Task<bool> future)
        {
            Assert.True(await Start(future, TaskScheduler.Default));
        }

        [Theory]
        [MemberData("Task_Data")]
        public static void Task_Start_Test(string label, Func<Flag, Task> create)
        {
            AssertCompletes(flag => Start(create(flag)));
        }

        [Theory]
        [MemberData("Task_Data")]
        public static void Task_Start_Scheduler_Test(string label, Func<Flag, Task> create)
        {
            AssertCompletes(flag => Start(create(flag), TaskScheduler.Default));
        }

        private static T Start<T>(T task) where T : Task
        {
            task.Start();
            return task;
        }

        private static T Start<T>(T task, TaskScheduler scheduler) where T : Task
        {
            task.Start(scheduler);
            return task;
        }

        [Fact]
        public static void StartOnContinueInvalid_Tests()
        {
            Task t = new Task(() => { }).ContinueWith(ignore => { });
            Assert.Throws<InvalidOperationException>(() => t.Start());
        }

        [Fact]
        public static void MultipleStartInvalid_Tests()
        {
            Task t = new Task(() => { });
            t.Start();
            Assert.Throws<InvalidOperationException>(() => t.Start());
        }

        [Fact]
        public static void StartOnPromiseInvalid_Tests()
        {
            TaskCompletionSource<object> completionSource = new TaskCompletionSource<object>();
            Task<object> task = completionSource.Task;
            Assert.Throws<InvalidOperationException>(() => task.Start());
        }

        [Fact]
        public async static void ArgumentNullException_Tests()
        {
            Assert.Throws<ArgumentNullException>(() => { new Task(null); });
            Assert.Throws<ArgumentNullException>(() => { new Task<object>(null); });
            await Assert.ThrowsAsync<ArgumentNullException>(() => Task.Factory.StartNew(null));
            await Assert.ThrowsAsync<ArgumentNullException>(() => Task<object>.Factory.StartNew(null));
            await Assert.ThrowsAsync<ArgumentNullException>(() => Task.Factory.StartNew<object>(null));

            // Task scheduler cannot be null
            Assert.Throws<ArgumentNullException>(() => new Task(() => { }).Start(null));
            Assert.Throws<ArgumentNullException>(() => new Task<object>(() => new object()).Start(null));
            await Assert.ThrowsAsync<ArgumentNullException>(() => Task.Factory.StartNew(() => { }, new CancellationToken(), TaskCreationOptions.None, null));
            await Assert.ThrowsAsync<ArgumentNullException>(() => Task<object>.Factory.StartNew(() => new object(), new CancellationToken(), TaskCreationOptions.None, null));
            await Assert.ThrowsAsync<ArgumentNullException>(() => Task.Factory.StartNew<object>(() => new object(), new CancellationToken(), TaskCreationOptions.None, null));
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(32)]
        [InlineData(128)]
        public async static void InvalidTaskCreationOption_Tests(int option)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => { new Task(() => { }, (TaskCreationOptions)option); });
            Assert.Throws<ArgumentOutOfRangeException>(() => { new Task<object>(() => new object(), (TaskCreationOptions)option); });
            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => Task.Factory.StartNew(() => { }, (TaskCreationOptions)option));
            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => Task<object>.Factory.StartNew(() => new object(), (TaskCreationOptions)option));
            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => Task.Factory.StartNew<object>(() => new object(), (TaskCreationOptions)option));
        }

        public class Flag
        {
            private int _flag = 0;

            public bool IsTripped { get { return _flag == 1; } }

            public void Trip()
            {
                // Flip flag, and make sure it's only done once.
                Assert.Equal(0, Interlocked.CompareExchange(ref _flag, 1, 0));
            }
        }
    }
}
