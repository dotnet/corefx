// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using Xunit;

namespace System.IO.Tests
{
    public abstract partial class FileSystemWatcherTest : FileCleanupTestBase
    {
        /// <summary>
        /// Does verification that the given watcher will throw exactly/only the events in "expectedEvents" when
        /// "action" is executed.
        /// </summary>
        /// <param name="watcher">The FileSystemWatcher to test</param>
        /// <param name="expectedEvents">All of the events that are expected to be raised by this action</param>
        /// <param name="action">The Action that will trigger events.</param>
        /// <param name="cleanup">Optional. Undoes the action and cleans up the watcher so the test may be run again if necessary.</param>
        /// <param name="expectedPath">Optional. Adds path verification to all expected events.</param>
        /// <param name="attempts">Optional. Number of times the test should be executed if it's failing.</param>
        public static void ExpectEventFilters(FileSystemWatcher watcher, WatcherChangeTypes expectedEvents, Action action, Action cleanup = null, string[] expectedPaths = null, int attempts = DefaultAttemptsForExpectedEvent, int timeout = WaitForExpectedEventTimeout)
        {
            int attemptsCompleted = 0;
            bool result = false;
            while (!result && attemptsCompleted++ < attempts)
            {
                if (attemptsCompleted > 1)
                {
                    // Re-create the watcher to get a clean iteration.
                    watcher = new FileSystemWatcher()
                    {
                        IncludeSubdirectories = watcher.IncludeSubdirectories,
                        NotifyFilter = watcher.NotifyFilter,
                        Path = watcher.Path,
                        InternalBufferSize = watcher.InternalBufferSize
                    };
                    // Most intermittent failures in FSW are caused by either a shortage of resources (e.g. inotify instances)
                    // or by insufficient time to execute (e.g. CI gets bogged down). Immediately re-running a failed test
                    // won't resolve the first issue, so we wait a little while hoping that things clear up for the next run.
                    Thread.Sleep(500);
                }

                result = ExecuteAndVerifyEvents(watcher, expectedEvents, action, attemptsCompleted == attempts, expectedPaths, timeout);

                if (cleanup != null)
                    cleanup();
            }
        }
    }
}
