﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading;
using Xunit;

namespace System.Management.Tests
{
    [SkipOnTargetFramework(TargetFrameworkMonikers.Uap, "WMI not supported via UAP")]
    [Collection("Mof Collection")]
    public class ManagementEventWatcherTests
    {
        private const string Query = "SELECT * FROM __TimerEvent WHERE TimerID = 'MyEvent'";

        [ConditionalFact(typeof(WmiTestHelper), nameof(WmiTestHelper.IsElevatedAndNotNanoServer))]
        [OuterLoop]
        public void Receive_Events_Sync()
        {
            ManagementEventWatcher watcher = null;
            try
            {
                // Setup: Timer event is already set up by mof file to fire every 0.1 seconds.
                // Run: Subscribe __TimerEvent with the specified TimerID
                watcher = new ManagementEventWatcher(
                    WmiTestHelper.Namespace,
                    Query,
                    new EventWatcherOptions(null, TimeSpan.FromSeconds(5), 1));

                ManagementBaseObject evt = watcher.WaitForNextEvent();

                Assert.True(evt["TimerID"].ToString() == "MyEvent", $"Unexpected TimerID value {evt["TimerID"]}");
            }
            finally
            {
                if (watcher != null)
                    watcher.Stop();
            }
        }

        [ConditionalFact(typeof(WmiTestHelper), nameof(WmiTestHelper.IsElevatedAndNotNanoServer))]
        [OuterLoop]
        public void Receive_Events_ASync()
        {
            ManagementEventWatcher watcher = null;
            var resetEvent = new ManualResetEvent(false);

            try
            {
                // Setup: Timer event is already set up by mof file to fire every 0.1 seconds.
                // Run: Subscribe __TimerEvent with the specified TimerID
                watcher = new ManagementEventWatcher(
                    WmiTestHelper.Namespace,
                    Query,
                    new EventWatcherOptions(null, TimeSpan.FromSeconds(5), 1));

                watcher.EventArrived += new EventArrivedEventHandler(EventArrived);
                watcher.Start();

                Assert.True(resetEvent.WaitOne(TimeSpan.FromSeconds(5), true), "Timeout waiting for receive event.");
            }
            finally
            {
                if (watcher != null)
                    watcher.Stop();
            }

            void EventArrived(object sender, EventArrivedEventArgs args)
            {
                ManagementBaseObject newEvent = args.NewEvent;
                Assert.True(newEvent["TimerID"].ToString() == "MyEvent", $"Unexpected TimerID value {newEvent["TimerID"]}");
                resetEvent.Set();
            }
        }
    }
}
