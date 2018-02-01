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
    public abstract class FileSystemWatcherTest : FileCleanupTestBase
    {
        // Events are reported asynchronously by the OS, so allow an amount of time for
        // them to arrive before testing an assertion.  If we expect an event to occur,
        // we can wait for it for a relatively long time, as if it doesn't arrive, we're
        // going to fail the test.  If we don't expect an event to occur, then we need
        // to keep the timeout short, as in a successful run we'll end up waiting for
        // the entire timeout specified.
        public const int WaitForExpectedEventTimeout = 500;         // ms to wait for an event to happen
        public const int LongWaitTimeout = 50000;                   // ms to wait for an event that takes a longer time than the average operation
        public const int SubsequentExpectedWait = 10;               // ms to wait for checks that occur after the first.
        public const int WaitForExpectedEventTimeout_NoRetry = 3000;// ms to wait for an event that isn't surrounded by a retry.
        public const int DefaultAttemptsForExpectedEvent = 3;       // Number of times an expected event should be retried if failing.
        public const int DefaultAttemptsForUnExpectedEvent = 2;     // Number of times an unexpected event should be retried if failing.

        /// <summary>
        /// Watches the Changed WatcherChangeType and unblocks the returned AutoResetEvent when a
        /// Changed event is thrown by the watcher.
        /// </summary>
        public static AutoResetEvent WatchChanged(FileSystemWatcher watcher, string[] expectedPaths = null)
        {
            AutoResetEvent eventOccurred = new AutoResetEvent(false);

            watcher.Changed += (o, e) =>
            {
                Assert.Equal(WatcherChangeTypes.Changed, e.ChangeType);
                if (expectedPaths != null)
                {
                    Assert.Contains(Path.GetFullPath(e.FullPath), expectedPaths);
                }
                eventOccurred.Set();
            };

            return eventOccurred;
        }

        /// <summary>
        /// Watches the Created WatcherChangeType and unblocks the returned AutoResetEvent when a
        /// Created event is thrown by the watcher.
        /// </summary>
        public static AutoResetEvent WatchCreated(FileSystemWatcher watcher, string[] expectedPaths = null)
        {
            AutoResetEvent eventOccurred = new AutoResetEvent(false);

            watcher.Created += (o, e) =>
            {
                Assert.Equal(WatcherChangeTypes.Created, e.ChangeType);
                if (expectedPaths != null)
                {
                    Assert.Contains(Path.GetFullPath(e.FullPath), expectedPaths);
                }
                eventOccurred.Set();
            };

            return eventOccurred;
        }

        /// <summary>
        /// Watches the Renamed WatcherChangeType and unblocks the returned AutoResetEvent when a
        /// Renamed event is thrown by the watcher.
        /// </summary>
        public static AutoResetEvent WatchDeleted(FileSystemWatcher watcher, string[] expectedPaths = null)
        {
            AutoResetEvent eventOccurred = new AutoResetEvent(false);

            watcher.Deleted += (o, e) =>
            {
                Assert.Equal(WatcherChangeTypes.Deleted, e.ChangeType);
                if (expectedPaths != null)
                {
                    Assert.Contains(Path.GetFullPath(e.FullPath), expectedPaths);
                }
                eventOccurred.Set();
            };

            return eventOccurred;
        }

        /// <summary>
        /// Watches the Renamed WatcherChangeType and unblocks the returned AutoResetEvent when a
        /// Renamed event is thrown by the watcher.
        /// </summary>
        public static AutoResetEvent WatchRenamed(FileSystemWatcher watcher, string[] expectedPaths = null)
        {
            AutoResetEvent eventOccurred = new AutoResetEvent(false);

            watcher.Renamed += (o, e) =>
            {
                Assert.Equal(WatcherChangeTypes.Renamed, e.ChangeType);
                if (expectedPaths != null)
                {
                    Assert.Contains(Path.GetFullPath(e.FullPath), expectedPaths);
                }
                eventOccurred.Set();
            };

            return eventOccurred;
        }

        /// <summary>
        /// Asserts that the given handle will be signaled within the default timeout.
        /// </summary>
        public static void ExpectEvent(WaitHandle eventOccurred, string eventName_NoRetry)
        {
            string message = String.Format("Didn't observe a {0} event within {1}ms", eventName_NoRetry, WaitForExpectedEventTimeout_NoRetry);
            Assert.True(eventOccurred.WaitOne(WaitForExpectedEventTimeout_NoRetry), message);
        }

        /// <summary>
        /// Does verification that the given watcher will throw exactly/only the events in "expectedEvents" when
        /// "action" is executed.
        /// </summary>
        /// <param name="watcher">The FileSystemWatcher to test</param>
        /// <param name="expectedEvents">All of the events that are expected to be raised by this action</param>
        /// <param name="action">The Action that will trigger events.</param>
        /// <param name="cleanup">Optional. Undoes the action and cleans up the watcher so the test may be run again if necessary.</param>
        public static void ExpectEvent(FileSystemWatcher watcher, WatcherChangeTypes expectedEvents, Action action, Action cleanup = null)
        {
            ExpectEvent(watcher, expectedEvents, action, cleanup, (string[])null);
        }

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
        public static void ExpectEvent(FileSystemWatcher watcher, WatcherChangeTypes expectedEvents, Action action, Action cleanup = null, string expectedPath = null, int attempts = DefaultAttemptsForExpectedEvent, int timeout = WaitForExpectedEventTimeout)
        {
            ExpectEvent(watcher, expectedEvents, action, cleanup, expectedPath == null ? null : new string[] { expectedPath }, attempts, timeout);
        }

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
        public static void ExpectEvent(FileSystemWatcher watcher, WatcherChangeTypes expectedEvents, Action action, Action cleanup = null, string[] expectedPaths = null, int attempts = DefaultAttemptsForExpectedEvent, int timeout = WaitForExpectedEventTimeout)
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
                        Filter = watcher.Filter,
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

        /// <summary>
        /// Helper for the ExpectEvent function. 
        /// </summary>
        /// <param name="watcher">The FileSystemWatcher to test</param>
        /// <param name="expectedEvents">All of the events that are expected to be raised by this action</param>
        /// <param name="action">The Action that will trigger events.</param>
        /// <param name="assertExpected">True if results should be asserted. Used if there is no retry.</param>
        /// <param name="expectedPath"> Adds path verification to all expected events.</param>
        /// <returns>True if the events raised correctly; else, false.</returns>
        public static bool ExecuteAndVerifyEvents(FileSystemWatcher watcher, WatcherChangeTypes expectedEvents, Action action, bool assertExpected, string[] expectedPaths, int timeout)
        {
            bool result = true, verifyChanged = true, verifyCreated = true, verifyDeleted = true, verifyRenamed = true;
            AutoResetEvent changed = null, created = null, deleted = null, renamed = null;
            string[] expectedFullPaths = expectedPaths == null ? null : expectedPaths.Select(e => Path.GetFullPath(e)).ToArray();

            if (verifyChanged = ((expectedEvents & WatcherChangeTypes.Changed) > 0))
                changed = WatchChanged(watcher, expectedPaths);
            if (verifyCreated = ((expectedEvents & WatcherChangeTypes.Created) > 0))
                created = WatchCreated(watcher, expectedPaths);
            if (verifyDeleted = ((expectedEvents & WatcherChangeTypes.Deleted) > 0))
                deleted = WatchDeleted(watcher, expectedPaths);
            if (verifyRenamed = ((expectedEvents & WatcherChangeTypes.Renamed) > 0))
                renamed = WatchRenamed(watcher, expectedPaths);

            watcher.EnableRaisingEvents = true;
            action();

            // Verify Changed
            if (verifyChanged)
            {
                bool Changed_expected = ((expectedEvents & WatcherChangeTypes.Changed) > 0);
                bool Changed_actual = changed.WaitOne(timeout);
                result = Changed_expected == Changed_actual;
                if (assertExpected)
                    Assert.True(Changed_expected == Changed_actual, "Changed event did not occur as expected");
            }

            // Verify Created
            if (verifyCreated)
            {
                bool Created_expected = ((expectedEvents & WatcherChangeTypes.Created) > 0);
                bool Created_actual = created.WaitOne(verifyChanged ? SubsequentExpectedWait : timeout);
                result = result && Created_expected == Created_actual;
                if (assertExpected)
                    Assert.True(Created_expected == Created_actual, "Created event did not occur as expected");
            }

            // Verify Deleted
            if (verifyDeleted)
            {
                bool Deleted_expected = ((expectedEvents & WatcherChangeTypes.Deleted) > 0);
                bool Deleted_actual = deleted.WaitOne(verifyChanged || verifyCreated ? SubsequentExpectedWait : timeout);
                result = result && Deleted_expected == Deleted_actual;
                if (assertExpected)
                    Assert.True(Deleted_expected == Deleted_actual, "Deleted event did not occur as expected");
            }

            // Verify Renamed
            if (verifyRenamed)
            {
                bool Renamed_expected = ((expectedEvents & WatcherChangeTypes.Renamed) > 0);
                bool Renamed_actual = renamed.WaitOne(verifyChanged || verifyCreated  || verifyDeleted? SubsequentExpectedWait : timeout);
                result = result && Renamed_expected == Renamed_actual;
                if (assertExpected)
                    Assert.True(Renamed_expected == Renamed_actual, "Renamed event did not occur as expected");
            }

            watcher.EnableRaisingEvents = false;
            return result;
        }

        /// <summary>
        /// Does verification that the given watcher will throw an Error when the given action is executed.
        /// </summary>
        /// <param name="watcher">The FileSystemWatcher to test</param>
        /// <param name="action">The Action that will trigger a failure.</param>
        /// <param name="cleanup">Undoes the action and cleans up the watcher so the test may be run again if necessary.</param>
        /// <param name="attempts">Optional. Number of times the test should be executed if it's failing.</param>
        public static void ExpectError(FileSystemWatcher watcher, Action action, Action cleanup, int attempts = DefaultAttemptsForExpectedEvent)
        {
            string message = string.Format("Did not observe an error event within {0}ms and {1} attempts.", WaitForExpectedEventTimeout, attempts);
            Assert.True(TryErrorEvent(watcher, action, cleanup, attempts, expected: true), message);
        }

        /// <summary>
        /// Does verification that the given watcher will <b>not</b> throw an Error when the given action is executed.
        /// </summary>
        /// <param name="watcher">The FileSystemWatcher to test</param>
        /// <param name="action">The Action that will not trigger a failure.</param>
        /// <param name="cleanup">Undoes the action and cleans up the watcher so the test may be run again if necessary.</param>
        /// <param name="attempts">Optional. Number of times the test should be executed if it's failing.</param>
        public static void ExpectNoError(FileSystemWatcher watcher, Action action, Action cleanup, int attempts = DefaultAttemptsForUnExpectedEvent)
        {
            string message = string.Format("Should not observe an error event within {0}ms. Attempted {1} times and received the event each time.", WaitForExpectedEventTimeout, attempts);
            Assert.False(TryErrorEvent(watcher, action, cleanup, attempts, expected: true), message);
        }

        /// /// <summary>
        /// Helper method for the ExpectError/ExpectNoError functions.
        /// </summary>
        /// <param name="watcher">The FileSystemWatcher to test</param>
        /// <param name="action">The Action to execute.</param>
        /// <param name="cleanup">Undoes the action and cleans up the watcher so the test may be run again if necessary.</param>
        /// <param name="attempts">Number of times the test should be executed if it's failing.</param>
        /// <param name="expected">Whether it is expected that an error event will be arisen.</param>
        /// <returns>True if an Error event was raised by the watcher when the given action was executed; else, false.</returns>
        public static bool TryErrorEvent(FileSystemWatcher watcher, Action action, Action cleanup, int attempts, bool expected)
        {
            int attemptsCompleted = 0;
            bool result = !expected;
            while (result != expected && attemptsCompleted++ < attempts)
            {
                if (attemptsCompleted > 1)
                {
                    // Re-create the watcher to get a clean iteration.
                    watcher = new FileSystemWatcher()
                    {
                        IncludeSubdirectories = watcher.IncludeSubdirectories,
                        NotifyFilter = watcher.NotifyFilter,
                        Filter = watcher.Filter,
                        Path = watcher.Path,
                        InternalBufferSize = watcher.InternalBufferSize
                    };
                    // Most intermittent failures in FSW are caused by either a shortage of resources (e.g. inotify instances)
                    // or by insufficient time to execute (e.g. CI gets bogged down). Immediately re-running a failed test
                    // won't resolve the first issue, so we wait a little while hoping that things clear up for the next run.
                    Thread.Sleep(500);
                }

                AutoResetEvent errorOccurred = new AutoResetEvent(false);
                watcher.Error += (o, e) =>
                {
                    errorOccurred.Set();
                };

                // Enable raising events but be careful with the possibility of the max user inotify instances being reached already.
                if (attemptsCompleted <= attempts)
                {
                    try
                    {
                        watcher.EnableRaisingEvents = true;
                    }
                    catch (IOException) // Max User INotify instances. Isn't the type of error we're checking for.
                    {
                        continue;
                    }
                }
                else
                {
                    watcher.EnableRaisingEvents = true;
                }

                action();
                result = errorOccurred.WaitOne(WaitForExpectedEventTimeout);
                watcher.EnableRaisingEvents = false;
                cleanup();
            }
            return result;
        }

        /// <summary>
        /// In some cases (such as when running without elevated privileges),
        /// the symbolic link may fail to create. Only run this test if it creates
        /// links successfully.
        /// </summary>
        protected static bool CanCreateSymbolicLinks
        {
            get
            {
                bool success = true;

                // Verify file symlink creation
                string path = Path.GetTempFileName();
                string linkPath = path + ".link";
                success = CreateSymLink(path, linkPath, isDirectory: false);
                try { File.Delete(path); } catch { }
                try { File.Delete(linkPath); } catch { }

                // Verify directory symlink creation
                path = Path.GetTempFileName();
                linkPath = path + ".link";
                success = success && CreateSymLink(path, linkPath, isDirectory: true);
                try { Directory.Delete(path); } catch { }
                try { Directory.Delete(linkPath); } catch { }

                return success;
            }
        }

        public static bool CreateSymLink(string targetPath, string linkPath, bool isDirectory)
        {
            Process symLinkProcess = new Process();
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                symLinkProcess.StartInfo.FileName = "cmd";
                symLinkProcess.StartInfo.Arguments = string.Format("/c mklink{0} \"{1}\" \"{2}\"", isDirectory ? " /D" : "", Path.GetFullPath(linkPath), Path.GetFullPath(targetPath));
            }
            else
            {
                symLinkProcess.StartInfo.FileName = "/bin/ln";
                symLinkProcess.StartInfo.Arguments = string.Format("-s \"{0}\" \"{1}\"", Path.GetFullPath(targetPath), Path.GetFullPath(linkPath));
            }
            symLinkProcess.StartInfo.RedirectStandardOutput = true;
            symLinkProcess.Start();

            if (symLinkProcess != null)
            {
                symLinkProcess.WaitForExit();
                return (0 == symLinkProcess.ExitCode);
            }
            else
            {
                return false;
            }
        }


        public static IEnumerable<object[]> FilterTypes()
        {
            foreach (NotifyFilters filter in Enum.GetValues(typeof(NotifyFilters)))
                yield return new object[] { filter };
        }
        
        // Linux and OSX systems have less precise filtering systems than Windows, so most
        // metadata filters are effectively equivalent to each other on those systems. For example
        // there isn't a way to filter only LastWrite events on either system; setting
        // Filters to LastWrite will allow events from attribute change, creation time
        // change, size change, etc.
        public const NotifyFilters LinuxFiltersForAttribute = NotifyFilters.Attributes |
                                                                NotifyFilters.CreationTime |
                                                                NotifyFilters.LastAccess |
                                                                NotifyFilters.LastWrite |
                                                                NotifyFilters.Security |
                                                                NotifyFilters.Size;
        public const NotifyFilters LinuxFiltersForModify = NotifyFilters.LastAccess |
                                                            NotifyFilters.LastWrite |
                                                            NotifyFilters.Security |
                                                            NotifyFilters.Size;
        public const NotifyFilters OSXFiltersForModify = NotifyFilters.Attributes |
                                                        NotifyFilters.CreationTime |
                                                        NotifyFilters.LastAccess |
                                                        NotifyFilters.LastWrite |
                                                        NotifyFilters.Size;
    }
}
