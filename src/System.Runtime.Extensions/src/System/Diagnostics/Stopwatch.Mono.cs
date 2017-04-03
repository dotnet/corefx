// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Diagnostics
{
    public partial class Stopwatch
    {
        private static bool QueryPerformanceFrequency(out long value)
        {
            if (PlatformHelper.IsWindows)
            {
                return StopwatchWindows.QueryPerformanceFrequency(out value);
            }
            else if (PlatformHelper.IsUnix)
            {
                return StopwatchUnix.QueryPerformanceFrequency(out value);
            }
            else
            {
                throw new PlatformNotSupportedException();
            }
        }

        private static bool QueryPerformanceCounter(out long value)
        {
            if (PlatformHelper.IsWindows)
            {
                return StopwatchWindows.QueryPerformanceCounter(out value);
            }
            else if (PlatformHelper.IsUnix)
            {
                return StopwatchUnix.QueryPerformanceCounter(out value);
            }
            else
            {
                throw new PlatformNotSupportedException();
            }
        }
    }
}
