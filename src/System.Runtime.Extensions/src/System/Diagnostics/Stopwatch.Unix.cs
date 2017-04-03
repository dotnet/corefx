// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Diagnostics
{
#if !MONO
    public partial class Stopwatch
#else
    internal static class StopwatchUnix
#endif
    {
        internal static bool QueryPerformanceFrequency(out long value)
        {
            return Interop.Sys.GetTimestampResolution(out value);
        }

        internal static bool QueryPerformanceCounter(out long value)
        {
            return Interop.Sys.GetTimestamp(out value);
        }
    }
}
