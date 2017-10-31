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

        public static void RetryAvailable(Action func)
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
                    retries = -1;
                }
                catch (Win32Exception)
                {
                    Thread.Sleep(100);
                    retries--;
                }
            }
            return;
        }

        public static T RetrieveEntryOrMessage<T>(Func<T> func)
        {
            T entry = default(T);
            if (!PlatformDetection.IsWindows7)
            {
                return func();
            }

            int retries = 3;
            while (retries > 0)
            {
                try
                {
                    entry = func();
                    retries = -1;
                }
                catch (Win32Exception)
                {
                    Thread.Sleep(100);
                    retries--;
                }
            }
            return entry;
        }
    }
}
