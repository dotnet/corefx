﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using Xunit;

namespace Microsoft.Win32.SystemEventsTests
{
    public class CreateTimerTests
    {
        /// <summary>
        /// Minimum permitted interval
        /// </summary>
        public const int TimerInterval = 10;

        [ConditionalFact(typeof(PlatformDetection), nameof(PlatformDetection.IsNotWindowsNanoServer))]
        public void CreateTimerInvalidInterval()
        {
            Assert.Throws<ArgumentException>(() => SystemEvents.CreateTimer(0));
            Assert.Throws<ArgumentException>(() => SystemEvents.CreateTimer(-1));
            Assert.Throws<ArgumentException>(() => SystemEvents.CreateTimer(int.MinValue));
        }

        [ConditionalFact(typeof(PlatformDetection), nameof(PlatformDetection.IsNotWindowsNanoServer))]
        public void TimerElapsedSignaled()
        {
            var elapsed = new AutoResetEvent(false);

            object elapsedSender = null;
            IntPtr timer = IntPtr.Zero;

            TimerElapsedEventHandler handler = (sender, args) =>
            {
                if (args?.TimerId == timer)
                {
                    elapsedSender = sender;
                    elapsed.Set();
                }
            };

            SystemEvents.TimerElapsed += handler;
            try
            {
                if (PlatformDetection.IsFullFramework)
                {
                    // desktop has a bug where it will allow EnsureSystemEvents to proceed without actually creating the HWND
                    SystemEventsTest.WaitForSystemEventsWindow();
                }

                timer = SystemEvents.CreateTimer(TimerInterval);

                Assert.True(elapsed.WaitOne(TimerInterval * SystemEventsTest.ExpectedEventMultiplier));
                Assert.IsType<SystemEvents>(elapsedSender);

                // Timer should fire more than once
                Assert.True(elapsed.WaitOne(TimerInterval * SystemEventsTest.ExpectedEventMultiplier));

                SystemEvents.KillTimer(timer);
                elapsed.Reset();

                // Timer should not fire once killed
                Assert.False(elapsed.WaitOne(TimerInterval * SystemEventsTest.UnexpectedEventMultiplier));
            }
            finally
            {
                SystemEvents.TimerElapsed -= handler;
                elapsed.Dispose();
            }
        }

        [ConditionalFact(typeof(PlatformDetection), nameof(PlatformDetection.IsNotWindowsNanoServer))]
        public void ConcurrentTimers()
        {
            const int NumConcurrentTimers = 10;
            var timersSignalled = new Dictionary<IntPtr, bool>();
            int numSignaled = 0;
            var elapsed = new AutoResetEvent(false);

            TimerElapsedEventHandler handler = (sender, args) =>
            {
                bool signaled = false;
                if (timersSignalled.TryGetValue(args.TimerId, out signaled) && !signaled)
                {
                    timersSignalled[args.TimerId] = true;

                    if (Interlocked.Increment(ref numSignaled) == NumConcurrentTimers)
                    {
                        elapsed.Set();
                    }
                }
            };

            SystemEvents.TimerElapsed += handler;
            try
            {
                if (PlatformDetection.IsFullFramework)
                {
                    // desktop has a bug where it will allow EnsureSystemEvents to proceed without actually creating the HWND
                    SystemEventsTest.WaitForSystemEventsWindow();
                }

                for (int i = 0; i < NumConcurrentTimers; i++)
                {
                    timersSignalled[SystemEvents.CreateTimer(TimerInterval)] = false;
                }

                Assert.True(elapsed.WaitOne(TimerInterval * SystemEventsTest.ExpectedEventMultiplier));

                foreach (var timer in timersSignalled.Keys.ToArray())
                {
                    Assert.True(timersSignalled[timer]);
                    SystemEvents.KillTimer(timer);
                }
            }
            finally
            {
                SystemEvents.TimerElapsed -= handler;
                elapsed.Dispose();
            }
        }

        [OuterLoop]
        [ConditionalTheory(typeof(PlatformDetection), nameof(PlatformDetection.IsNotWindowsNanoServer))]
        [InlineData(500)]  // .5s
        [InlineData(1000)]  // 1s
        [InlineData(2000)]  // 2s
        [InlineData(10000)] // 10s
        [InlineData(30000)] // 30s
        public void TimerElapsedIsRoughlyEquivalentToInterval(int interval)
        {
            const double permittedProportionUnder = -0.1;
            const double permittedProportionOver = 0.5;
            var elapsed = new AutoResetEvent(false);
            IntPtr timer = IntPtr.Zero;
            var stopwatch = new Stopwatch();

            TimerElapsedEventHandler handler = (sender, args) =>
            {
                if (args?.TimerId == timer)
                {
                    stopwatch.Stop();
                    elapsed.Set();
                }
            };

            SystemEvents.TimerElapsed += handler;
            try
            {
                timer = SystemEvents.CreateTimer(interval);
                stopwatch.Start();
                Assert.True(elapsed.WaitOne(interval * SystemEventsTest.ExpectedEventMultiplier));

                var proportionDifference = (double)(stopwatch.ElapsedMilliseconds - interval) / interval;
                Assert.True(permittedProportionUnder < proportionDifference && proportionDifference < permittedProportionOver, 
                    $"Timer should fire less than {permittedProportionUnder * 100.0}% before and less than {permittedProportionOver * 100.0}% after expected interval {interval}, actual: {stopwatch.ElapsedMilliseconds}, difference: {proportionDifference * 100.0}%");
            }
            finally
            {
                SystemEvents.TimerElapsed -= handler;
                SystemEvents.KillTimer(timer);
            }
        }
    }
}
