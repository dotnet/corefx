// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Threading;

namespace System.Diagnostics.Tests
{
    internal class Helpers
    {
        public static bool IsElevatedAndSupportsEventLogs { get => AdminHelpers.IsProcessElevated() && SupportsEventLogs; }
        public static bool SupportsEventLogs { get => PlatformDetection.IsNotWindowsNanoServer; }

        public static void RetryAvailable<EventLog>(Action func)
        {
            if (!PlatformDetection.IsWindows7)
            {
                func();
                return;
            }

            int retries = 3;
            while (retries > 0)
            {
                try
                {
                    func();
                    break;
                }
                catch (Win32Exception)
                {
                    Thread.Sleep(100);
                    retries--;
                }
            }
            return;
        }

        public static void CopyCollection<EventLogEntryCollection>(Action func)
        {
            if (!PlatformDetection.IsWindows7)
            {
                func();
                return;
            }

            int retries = 3;
            while (retries > 0)
            {
                try
                {
                    func();
                    break;
                }
                catch (Win32Exception)
                {
                    Thread.Sleep(100);
                    retries--;
                }
            }
            return;
        }

        public static EventLogEntry RetrieveEntry<EventLog>(Func<EventLogEntry> func)
        {
            EventLogEntry eventLogEntry = null;
            if (!PlatformDetection.IsWindows7)
            {
                return func();
            }

            int retries = 3;
            while (retries > 0)
            {
                try
                {
                    eventLogEntry = func();
                    break;
                }
                catch (Win32Exception)
                {
                    Thread.Sleep(100);
                    retries--;
                }
            }
            return eventLogEntry;
        }

        public static EventLogEntry RetrieveEntryFromCollection<EventLog>(Func<EventLogEntry> func)
        {
            EventLogEntry eventLogEntry = null;
            if (!PlatformDetection.IsWindows7)
            {
                return func();
            }

            int retries = 3;
            while (retries > 0)
            {
                try
                {
                    eventLogEntry = func();
                    break;
                }
                catch (Win32Exception)
                {
                    Thread.Sleep(100);
                    retries--;
                }
            }
            return eventLogEntry;
        }

        public static string RetrieveMessage<EventLogEntry>(Func<string> func)
        {
            string message = null;
            if (!PlatformDetection.IsWindows7)
            {
                return func();
            }

            int retries = 3;
            while (retries > 0)
            {
                try
                {
                    message = func();
                    break;
                }
                catch (Win32Exception)
                {
                    Thread.Sleep(100);
                    retries--;
                }
            }
            return message;
        }
    }
}
