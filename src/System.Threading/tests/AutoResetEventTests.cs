// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Xunit;
using System;
using System.Threading;
using System.Threading.Tasks;

public class AutoResetEventTests
{
    [Fact]
    public void Ctor()
    {
        using (AutoResetEvent are = new AutoResetEvent(false))
            Assert.False(are.WaitOne(0));

        using (AutoResetEvent are = new AutoResetEvent(true))
            Assert.True(are.WaitOne(0));
    }

    [Fact]
    public void SetReset()
    {
        using (AutoResetEvent are = new AutoResetEvent(false))
        {
            Assert.False(are.WaitOne(0));
            are.Set();
            Assert.True(are.WaitOne(0));
            Assert.False(are.WaitOne(0));
            are.Set();
            are.Reset();
            Assert.False(are.WaitOne(0));
        }
    }

    [Fact]
    public void WaitHandleWaitAll()
    {
        AutoResetEvent[] handles = new AutoResetEvent[10];
        for (int i = 0; i < handles.Length; i++)
            handles[i] = new AutoResetEvent(false);

        Task<bool> t = Task.Run(() => WaitHandle.WaitAll(handles));
        for (int i = 0; i < handles.Length; i++)
        {
            Assert.False(t.IsCompleted);
            handles[i].Set();
        }
        Assert.True(t.Result);

        Assert.False(WaitHandle.WaitAll(handles, 0));
    }

    [Fact]
    public void WaitHandleWaitAny()
    {
        AutoResetEvent[] handles = new AutoResetEvent[10];
        for (int i = 0; i < handles.Length; i++)
            handles[i] = new AutoResetEvent(false);

        Task<int> t = Task.Run(() => WaitHandle.WaitAny(handles));
        handles[5].Set();
        Assert.Equal(5, t.Result);

        Assert.Equal(WaitHandle.WaitTimeout, WaitHandle.WaitAny(handles, 0));
    }

    [Fact]
    public void PingPong()
    {
        using (AutoResetEvent are1 = new AutoResetEvent(true), are2 = new AutoResetEvent(false))
        {
            const int Iters = 10;
            Task.WaitAll(
                Task.Factory.StartNew(() =>
                {
                    for (int i = 0; i < Iters; i++)
                    {
                        Assert.True(are1.WaitOne());
                        are2.Set();
                    }
                }, CancellationToken.None, TaskCreationOptions.LongRunning, TaskScheduler.Default),
                Task.Factory.StartNew(() =>
                {
                    for (int i = 0; i < Iters; i++)
                    {
                        Assert.True(are2.WaitOne());
                        are1.Set();
                    }
                }, CancellationToken.None, TaskCreationOptions.LongRunning, TaskScheduler.Default));
        }
    }

}
