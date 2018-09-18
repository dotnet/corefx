// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Diagnostics
{
    internal static partial class TraceListenerHelpers
    {
        private static volatile int s_processId;

        internal static int GetProcessId()
        {
            EnsureProcessInfo();
            return s_processId;
        }

        internal static string GetProcessName()
        {
            EnsureProcessInfo();
            return s_processName;
        }

        private static void EnsureProcessInfo()
        {
            if (s_processName == null)
            {
                using (var process = Process.GetCurrentProcess())
                {
                    s_processName = process.ProcessName;
                    s_processId = process.Id;
                }
            }
        }

    }
}
