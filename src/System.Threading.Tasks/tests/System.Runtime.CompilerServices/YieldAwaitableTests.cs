// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Runtime.CompilerServices;
using Xunit;

namespace System.Threading.Tasks.Tests
{
    public class YieldAwaitableTests
    {
        // awaiting Task.Yield
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public static void AsyncYieldAwaiter_Direct_Test(bool useYieldAwaiter)
        {
            SynchronizationContext previous = SynchronizationContext.Current;
            try
            {
                SynchronizationContext.SetSynchronizationContext(new ValidateCorrectContextSynchronizationContext());
                var ya = useYieldAwaiter ? new YieldAwaitable.YieldAwaiter() : new YieldAwaitable().GetAwaiter();
                var mres = new ManualResetEventSlim();
                Assert.False(ya.IsCompleted);
                ya.OnCompleted(() =>
                {
                    Assert.True(ValidateCorrectContextSynchronizationContext.IsPostedInContext);
                    mres.Set();
                });
                mres.Wait();
                ya.GetResult();
            }
            finally
            {
                SynchronizationContext.SetSynchronizationContext(previous);
            }
        }

        [Fact]
        public static void AsyncYieldAwaiter_Indirect_Test()
        {
            SynchronizationContext previous = SynchronizationContext.Current;
            try
            {
                // Yield when there's a current sync context
                SynchronizationContext.SetSynchronizationContext(new ValidateCorrectContextSynchronizationContext());
                var ya = Task.Yield().GetAwaiter();
                ya.GetResult();
                var mres = new ManualResetEventSlim();
                Assert.False(ya.IsCompleted);
                ya.OnCompleted(() =>
                {
                    Assert.True(ValidateCorrectContextSynchronizationContext.IsPostedInContext);
                    mres.Set();
                });
                mres.Wait();
                ya.GetResult();
            }
            finally
            {
                SynchronizationContext.SetSynchronizationContext(previous);
            }
        }

        [Fact]
        public static void AsyncYieldAwaiter_WithContext_Test()
        {
            SynchronizationContext previous = SynchronizationContext.Current;
            try
            {
                // TODO: Figure out why the test locks without the next line.
                SynchronizationContext.SetSynchronizationContext(null);

                // Yield when there's a current TaskScheduler
                Task.Factory.StartNew(() =>
                {
                    var ya = Task.Yield().GetAwaiter();
                    ya.GetResult();
                    var mres = new ManualResetEventSlim();
                    Assert.False(ya.IsCompleted);
                    ya.OnCompleted(() =>
                    {
                        Assert.IsType<QUWITaskScheduler>(TaskScheduler.Current);
                        mres.Set();
                    });
                    mres.Wait();
                    ya.GetResult();
                }, CancellationToken.None, TaskCreationOptions.None, new QUWITaskScheduler()).Wait();
            }
            finally
            {
                SynchronizationContext.SetSynchronizationContext(previous);
            }
        }

        [Fact]
        public static void AsyncYieldAwaiter_SetInsideTask_Test()
        {
            SynchronizationContext previous = SynchronizationContext.Current;
            try
            {
                // Yield when there's a current TaskScheduler and SynchronizationContext.Current is the base SynchronizationContext
                Task.Factory.StartNew(() =>
                {
                    SynchronizationContext.SetSynchronizationContext(new SynchronizationContext());

                    var ya = Task.Yield().GetAwaiter();
                    ya.GetResult();
                    var mres = new ManualResetEventSlim();
                    Assert.False(ya.IsCompleted);
                    ya.OnCompleted(() =>
                    {
                        Assert.IsType<QUWITaskScheduler>(TaskScheduler.Current);
                        mres.Set();
                    });
                    mres.Wait();
                    ya.GetResult();

                    SynchronizationContext.SetSynchronizationContext(null);
                }, CancellationToken.None, TaskCreationOptions.None, new QUWITaskScheduler()).Wait();
            }
            finally
            {
                SynchronizationContext.SetSynchronizationContext(previous);
            }
        }

        [Fact]
        public static void AsyncYieldAwaiter_GrabbedByOncompleted_Test()
        {
            SynchronizationContext previous = SynchronizationContext.Current;
            try
            {
                // OnCompleted grabs the current context, not Task.Yield nor GetAwaiter
                SynchronizationContext.SetSynchronizationContext(new SynchronizationContext());
                var ya = Task.Yield().GetAwaiter();
                SynchronizationContext.SetSynchronizationContext(new ValidateCorrectContextSynchronizationContext());
                ya.GetResult();
                var mres = new ManualResetEventSlim();
                Assert.False(ya.IsCompleted);
                ya.OnCompleted(() =>
                {
                    Assert.True(ValidateCorrectContextSynchronizationContext.IsPostedInContext);
                    mres.Set();
                });
                mres.Wait();
                ya.GetResult();
            }
            finally
            {
                SynchronizationContext.SetSynchronizationContext(previous);
            }
        }

        // awaiting Task.Yield
        [Fact]
        public static void RunAsyncYieldAwaiterTests_Negative()
        {
            var ya = Task.Yield().GetAwaiter();
            Assert.Throws<ArgumentNullException>(() => { ya.OnCompleted(null); });
        }
    }
}
