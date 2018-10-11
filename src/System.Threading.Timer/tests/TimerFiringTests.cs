// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

public partial class TimerFiringTests
{
    internal const int MaxPositiveTimeoutInMs = 30000;

    [Fact]
    public void Timer_Fires_After_DueTime_Ellapses()
    {
        AutoResetEvent are = new AutoResetEvent(false);

        using (var t = new Timer(new TimerCallback((object s) =>
        {
            are.Set();
        }), null, TimeSpan.FromMilliseconds(250), TimeSpan.FromMilliseconds(Timeout.Infinite) /* not relevant */))
        {
            Assert.True(are.WaitOne(TimeSpan.FromMilliseconds(MaxPositiveTimeoutInMs)));
        }
    }

    [Fact]
    public void Timer_Fires_AndPassesStateThroughCallback()
    {
        AutoResetEvent are = new AutoResetEvent(false);

        object state = new object();
        using (var t = new Timer(new TimerCallback((object s) =>
        {
            Assert.Same(s, state);
            are.Set();
        }), state, TimeSpan.FromMilliseconds(100), TimeSpan.FromMilliseconds(Timeout.Infinite) /* not relevant */))
        {
            Assert.True(are.WaitOne(TimeSpan.FromMilliseconds(MaxPositiveTimeoutInMs)));
        }
    }

    [Fact]
    public void Timer_Fires_AndPassesNullStateThroughCallback()
    {
        AutoResetEvent are = new AutoResetEvent(false);

        using (var t = new Timer(new TimerCallback((object s) =>
        {
            Assert.Null(s);
            are.Set();
        }), null, TimeSpan.FromMilliseconds(100), TimeSpan.FromMilliseconds(Timeout.Infinite) /* not relevant */))
        {
            Assert.True(are.WaitOne(TimeSpan.FromMilliseconds(MaxPositiveTimeoutInMs)));
        }
    }

    [Fact]
    public void Timer_Fires_After_DueTime_AndOn_Period()
    {
        int count = 0;
        AutoResetEvent are = new AutoResetEvent(false);

        using (var t = new Timer(new TimerCallback((object s) =>
        {
            if (Interlocked.Increment(ref count) >= 2)
            {
                are.Set();
            }
        }), null, TimeSpan.FromMilliseconds(250), TimeSpan.FromMilliseconds(50)))
        {
            Assert.True(are.WaitOne(TimeSpan.FromMilliseconds(MaxPositiveTimeoutInMs)));
        }
    }

    [Fact]
    public void Timer_FiresOnlyOnce_OnDueTime_With_InfinitePeriod()
    {
        int count = 0;
        AutoResetEvent are = new AutoResetEvent(false);

        using (var t = new Timer(new TimerCallback((object s) =>
        {
            if (Interlocked.Increment(ref count) >= 2)
            {
                are.Set();
            }
        }), null, TimeSpan.FromMilliseconds(100), TimeSpan.FromMilliseconds(Timeout.Infinite) /* not relevant */))
        {
            Assert.False(are.WaitOne(TimeSpan.FromMilliseconds(250 /*enough for 2 fires + buffer*/)));
        }
    }

    [Fact]
    public void Timer_CanDisposeSelfInCallback()
    {
        Timer t = null;
        AutoResetEvent are = new AutoResetEvent(false);
        TimerCallback tc = new TimerCallback((object o) =>
        {
            t.Dispose();
            are.Set();
        });
        t = new Timer(tc, null, -1, -1);
        t.Change(1, -1);
        Assert.True(are.WaitOne(MaxPositiveTimeoutInMs));
        GC.KeepAlive(t);
    }

    [Fact]
    public void Timer_CanBeDisposedMultipleTimes()
    {
        // There's nothing to validate besides that we don't throw an exception, so rely on xunit 
        // to catch any exception that would be thrown and signal a test failure
        TimerCallback tc = new TimerCallback((object o) => { });
        var t = new Timer(tc, null, 100, -1);
        for (int i = 0; i < 10; i++)
            t.Dispose();
    }

    [Fact]
    public void NonRepeatingTimer_ThatHasAlreadyFired_CanChangeAndFireAgain()
    {
        AutoResetEvent are = new AutoResetEvent(false);
        TimerCallback tc = new TimerCallback((object o) => are.Set());
        using (var t = new Timer(tc, null, 1, Timeout.Infinite))
        {
            Assert.True(are.WaitOne(MaxPositiveTimeoutInMs), "Should have received first timer event");
            Assert.False(are.WaitOne(500), "Should not have received a second timer event");
            t.Change(10, Timeout.Infinite);
            Assert.True(are.WaitOne(MaxPositiveTimeoutInMs), "Should have received a second timer event after changing it");
        }
    }

    [Fact]
    public void MultpleTimers_PeriodicTimerIsntBlockedByBlockedCallback()
    {
        int callbacks = 2;
        Barrier b = new Barrier(callbacks + 1);
        Timer t = null;
        t = new Timer(_ =>
        {
            if (Interlocked.Decrement(ref callbacks) >= 0)
            {
                Assert.True(b.SignalAndWait(MaxPositiveTimeoutInMs));
            }
            t.Dispose();
        }, null, -1, -1);
        t.Change(1, 50);

        Assert.True(b.SignalAndWait(MaxPositiveTimeoutInMs));
        GC.KeepAlive(t);
    }

    [Fact]
    public void ManyTimers_EachTimerDoesFire()
    {
        int maxTimers = 10000;
        CountdownEvent ce = new CountdownEvent(maxTimers);
        Timer[] timers = System.Linq.Enumerable.Range(0, maxTimers).Select(_ => new Timer(s => ce.Signal(), null, 100 /* enough time to wait on the are */, -1)).ToArray();
        try
        {
            Assert.True(ce.Wait(MaxPositiveTimeoutInMs), string.Format("Not all timers fired, {0} left of {1}", ce.CurrentCount, maxTimers));
        }
        finally
        {
            foreach (Timer t in timers)
                t.Dispose();
        }
    }

    [Fact]
    public void Timer_Constructor_CallbackOnly_Change()
    {
        var e = new ManualResetEvent(false);
        using (var t = new Timer(s => e.Set()))
        {
            t.Change(0u, 0u);
            Assert.True(e.WaitOne(MaxPositiveTimeoutInMs));
        }
    }

    [Fact]
    public void Timer_Dispose_WaitHandle_Negative()
    {
        Assert.Throws<ArgumentNullException>(() => new Timer(s => { }).Dispose(null));
    }

    [Fact]
    public void Timer_Dispose_WaitHandle()
    {
        int tickCount = 0;
        var someTicksPending = new ManualResetEvent(false);
        var completeTicks = new ManualResetEvent(false);
        var allTicksCompleted = new ManualResetEvent(false);
        var t =
            new Timer(s =>
            {
                if (Interlocked.Increment(ref tickCount) == 2)
                    someTicksPending.Set();
                Assert.True(completeTicks.WaitOne(MaxPositiveTimeoutInMs));
                Interlocked.Decrement(ref tickCount);
            }, null, 0, 1);
        Assert.True(someTicksPending.WaitOne(MaxPositiveTimeoutInMs));
        completeTicks.Set();
        t.Dispose(allTicksCompleted);
        Assert.True(allTicksCompleted.WaitOne(MaxPositiveTimeoutInMs));
        Assert.Equal(0, tickCount);
        Assert.Throws<ObjectDisposedException>(() => t.Change(0, 0));
    }

    [OuterLoop("Takes several seconds")]
    [Fact]
    public async Task Timer_ManyDifferentSingleDueTimes_AllFireSuccessfully()
    {
        await Task.WhenAll(from p in Enumerable.Range(0, Environment.ProcessorCount)
                           select Task.Run(async () =>
                           {
                               await Task.WhenAll(from i in Enumerable.Range(1, 1_000) select DueTimeAsync(i));
                               await Task.WhenAll(from i in Enumerable.Range(1, 1_000) select DueTimeAsync(1_001 - i));
                           }));
    }

    [OuterLoop("Takes several seconds")]
    [Fact]
    public async Task Timer_ManyDifferentPeriodicTimes_AllFireSuccessfully()
    {
        await Task.WhenAll(from p in Enumerable.Range(0, Environment.ProcessorCount)
                           select Task.Run(async () =>
                           {
                               await Task.WhenAll(from i in Enumerable.Range(1, 400) select PeriodAsync(period: i, iterations: 3));
                               await Task.WhenAll(from i in Enumerable.Range(1, 400) select PeriodAsync(period: 401 - i, iterations: 3));
                           }));
    }

    [PlatformSpecific(~TestPlatforms.OSX)] // macOS in CI appears to have a lot more variation
    [OuterLoop("Takes several seconds")]
    [Theory] // selection based on 333ms threshold used by implementation
    [InlineData(new int[] { 15 })]
    [InlineData(new int[] { 333 })]
    [InlineData(new int[] { 332, 333, 334 })]
    [InlineData(new int[] { 200, 300, 400 })]
    [InlineData(new int[] { 200, 250, 300 })]
    [InlineData(new int[] { 400, 450, 500 })]
    [InlineData(new int[] { 1000 })]
    public async Task Timer_ManyDifferentSerialSingleDueTimes_AllFireWithinAllowedRange(int[] dueTimes)
    {
        const int MillisecondsPadding = 100; // for each timer, out of range == Math.Abs(actualTime - dueTime) > MillisecondsPadding
        const int MaxAllowedOutOfRangePercentage = 20; // max % allowed out of range to pass test

        var outOfRange = new ConcurrentQueue<KeyValuePair<int, long>>();

        long totalTimers = 0;
        await Task.WhenAll(from p in Enumerable.Range(0, Environment.ProcessorCount)
                           select Task.Run(async () =>
                           {
                               await Task.WhenAll(from dueTimeTemplate in dueTimes
                                                  from dueTime in Enumerable.Repeat(dueTimeTemplate, 10)
                                                  select Task.Run(async () =>
                                                  {
                                                      var sw = new Stopwatch();
                                                      for (int i = 1; i <= 1_000 / dueTime; i++)
                                                      {
                                                          sw.Restart();
                                                          await DueTimeAsync(dueTime);
                                                          sw.Stop();

                                                          Interlocked.Increment(ref totalTimers);
                                                          if (Math.Abs(sw.ElapsedMilliseconds - dueTime) > MillisecondsPadding)
                                                          {
                                                              outOfRange.Enqueue(new KeyValuePair<int, long>(dueTime, sw.ElapsedMilliseconds));
                                                          }
                                                      }
                                                  }));
                           }));

        double percOutOfRange = (double)outOfRange.Count / totalTimers * 100;
        if (percOutOfRange > MaxAllowedOutOfRangePercentage)
        {
            IOrderedEnumerable<IGrouping<int, KeyValuePair<int, long>>> results =
                from sample in outOfRange
                group sample by sample.Key into groupedByDueTime
                orderby groupedByDueTime.Key
                select groupedByDueTime;

            var sb = new StringBuilder();
            sb.AppendFormat("{0}% out of {1} timer firings were off by more than {2}ms",
                percOutOfRange, totalTimers, MillisecondsPadding);
            foreach (IGrouping<int, KeyValuePair<int, long>> result in results)
            {
                sb.AppendLine();
                sb.AppendFormat("Expected: {0}, Actuals: {1}", result.Key, string.Join(", ", result.Select(k => k.Value)));
            }

            Assert.True(false, sb.ToString());
        }
    }

    private static Task DueTimeAsync(int dueTime)
    {
        // We could just use Task.Delay, but it only uses Timer as an implementation detail.
        // Since these are Timer tests, we use an implementation that explicitly uses Timer.
        var tcs = new TaskCompletionSource<bool>();
        var t = new Timer(_ => tcs.SetResult(true)); // rely on Timer(TimerCallback) rooting itself
        t.Change(dueTime, -1);
        return tcs.Task;
    }

    private static async Task PeriodAsync(int period, int iterations)
    {
        var tcs = new TaskCompletionSource<bool>();
        using (var t = new Timer(_ => { if (Interlocked.Decrement(ref iterations) == 0) tcs.SetResult(true); })) // rely on Timer(TimerCallback) rooting itself
        {
            t.Change(period, period);
            await tcs.Task.ConfigureAwait(false);
        }
    }
}
