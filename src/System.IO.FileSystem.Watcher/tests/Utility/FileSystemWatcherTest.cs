// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
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
        public const int WaitForExpectedEventTimeout = 10;
        public const int WaitForExpectedEventTimeout_NoRetry = 3000;
        public const int WaitForUnexpectedEventTimeout = 10;

        public static AutoResetEvent WatchForEvents(FileSystemWatcher watcher, WatcherChangeTypes actions, string expectedPath = null)
        {
            AutoResetEvent eventOccurred = new AutoResetEvent(false);

            if (0 != (actions & WatcherChangeTypes.Changed))
            {
                watcher.Changed += (o, e) =>
                {
                    Assert.Equal(WatcherChangeTypes.Changed, e.ChangeType);
                    if (expectedPath != null)
                    {
                        Assert.Equal(Path.GetFullPath(expectedPath), Path.GetFullPath(e.FullPath));
                    }
                    eventOccurred.Set();
                };
            }

            if (0 != (actions & WatcherChangeTypes.Created))
            {
                watcher.Created += (o, e) =>
                {
                    Assert.Equal(WatcherChangeTypes.Created, e.ChangeType);
                    if (expectedPath != null)
                    {
                        Assert.Equal(Path.GetFullPath(expectedPath), Path.GetFullPath(e.FullPath));
                    }
                    eventOccurred.Set();
                };
            }

            if (0 != (actions & WatcherChangeTypes.Deleted))
            {
                watcher.Deleted += (o, e) =>
                {
                    Assert.Equal(WatcherChangeTypes.Deleted, e.ChangeType);
                    if (expectedPath != null)
                    {
                        Assert.Equal(Path.GetFullPath(expectedPath), Path.GetFullPath(e.FullPath));
                    }
                    eventOccurred.Set();
                };
            }

            if (0 != (actions & WatcherChangeTypes.Renamed))
            {
                watcher.Renamed += (o, e) =>
                {
                    Assert.Equal(WatcherChangeTypes.Renamed, e.ChangeType);
                    if (expectedPath != null)
                    {
                        Assert.Equal(Path.GetFullPath(expectedPath), Path.GetFullPath(e.FullPath));
                    }
                    eventOccurred.Set();
                };
            }

            return eventOccurred;
        }

        public static void ExpectEvent(WaitHandle eventOccurred, string eventName, int timeout = WaitForExpectedEventTimeout_NoRetry)
        {
            string message = String.Format("Didn't observe a {0} event within {1}ms", eventName, timeout);
            Assert.True(eventOccurred.WaitOne(timeout), message);
        }

        public static void ExpectEvent(FileSystemWatcher watcher, WatcherChangeTypes changeType, Action action, string expectedPath = null, int attempts = 5, int timeout = WaitForExpectedEventTimeout)
        {
            string message = string.Format("Didn't observe a {0} event within {1}ms and {2} attempts.", changeType, timeout, attempts);
            Assert.True(TryEvent(watcher, changeType, action, attempts, timeout, true, expectedPath), message);
        }

        public static void ExpectNoEvent(FileSystemWatcher watcher, WatcherChangeTypes changeType, Action action, int attempts = 2, int timeout = WaitForExpectedEventTimeout)
        {
            string message = string.Format("Should not observe a {0} event within {1}ms. Attempted {2} times and received the event each time.", changeType, timeout, attempts);
            Assert.False(TryEvent(watcher, changeType, action, attempts, timeout, false, null), message);
        }

        public static void ExpectEvent(FileSystemWatcher watcher, WatcherChangeTypes changeType, Action action, Action cleanup, string expectedPath = null, int attempts = 5, int timeout = WaitForExpectedEventTimeout)
        {
            string message = string.Format("Didn't observe a {0} event within {1}ms and {2} attempts.", changeType, timeout, attempts);
            Assert.True(TryEvent(watcher, changeType, action, cleanup, attempts, timeout, true, expectedPath), message);
        }

        public static void ExpectNoEvent(FileSystemWatcher watcher, WatcherChangeTypes changeType, Action action, Action cleanup, int attempts = 2, int timeout = WaitForExpectedEventTimeout)
        {
            string message = string.Format("Should not observe a {0} event within {1}ms. Attempted {2} times and received the event each time.", changeType, timeout, attempts);
            Assert.False(TryEvent(watcher, changeType, action, cleanup, attempts, timeout, false, null), message);
        }

        public static void ExpectError(FileSystemWatcher watcher, Action action, Action cleanup, int attempts = 2, int timeout = WaitForExpectedEventTimeout)
        {
            string message = string.Format("Did not observe an error event within {0}ms and {1} attempts.", timeout, attempts);
            Assert.True(TryErrorEvent(watcher, action, cleanup, attempts, timeout, expected: true), message);
        }

        public static void ExpectNoError(FileSystemWatcher watcher, Action action, Action cleanup, int attempts = 2, int timeout = WaitForExpectedEventTimeout)
        {
            string message = string.Format("Should not observe an error event within {0}ms. Attempted {1} times and received the event each time.", timeout, attempts);
            Assert.False(TryErrorEvent(watcher, action, cleanup, attempts, timeout, expected: true), message);
        }

        private static bool TryEvent(FileSystemWatcher watcher, WatcherChangeTypes changeType, Action action, int attempts, int timeout, bool expected, string expectedPath)
        {
            AutoResetEvent eventOccurred = WatchForEvents(watcher, changeType, expectedPath);
            bool result = !expected;
            int attemptsCompleted = 0;
            while (result != expected && attemptsCompleted++ < attempts)
            {
                watcher.EnableRaisingEvents = false;
                watcher.EnableRaisingEvents = true;

                action();
                result = eventOccurred.WaitOne(timeout * ((attemptsCompleted - 1) * 10));
            }
            return result;
        }

        private static bool TryEvent(FileSystemWatcher watcher, WatcherChangeTypes changeType, Action action, Action cleanup, int attempts, int timeout, bool expected, string expectedPath)
        {
            AutoResetEvent eventOccurred = WatchForEvents(watcher, changeType, expectedPath);
            bool result = !expected;
            int attemptsCompleted = 0;
            while (result != expected && attemptsCompleted++ < attempts)
            {
                watcher.EnableRaisingEvents = false;
                watcher.EnableRaisingEvents = true;

                try
                {
                    action();
                    result = eventOccurred.WaitOne(timeout * ((attemptsCompleted - 1) * 10));
                }
                finally
                {
                    cleanup();
                }
            }
            return result;
        }

        public static bool TryErrorEvent(FileSystemWatcher watcher, Action action, Action cleanup, int attempts, int timeout, bool expected)
        {
            AutoResetEvent errorOccured = new AutoResetEvent(false);
            watcher.Error += (o, e) =>
            {
                errorOccured.Set();
            };

            bool result = !expected;
            int attemptsCompleted = 0;
            while (result != expected && attemptsCompleted++ < attempts)
            {
                watcher.EnableRaisingEvents = false;
                watcher.EnableRaisingEvents = true;

                try
                {
                    action();
                    result = errorOccured.WaitOne(timeout);
                }
                finally
                {
                    cleanup();
                }
            }
            return result;
        }

        // In some cases (such as when running without elevated privileges),
        // the symbolic link may fail to create. Only run this test if it creates
        // links successfully.
        protected static bool CanCreateSymbolicLinks
        {
            get
            {
                var path = Path.GetTempFileName();
                var linkPath = path + ".link";
                var ret = CreateSymLink(path, linkPath, isDirectory: false);
                ret = ret && File.Exists(linkPath);
                try { File.Delete(path); } catch { }
                try { File.Delete(linkPath); } catch { }
                return ret;
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
                symLinkProcess.StartInfo.FileName = "ln";
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
    }
}