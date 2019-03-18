// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Diagnostics
{
    public partial class Stopwatch
    {
        private static unsafe Interop.BOOL QueryPerformanceFrequency(long* resolution)
        {
            return Interop.Sys.GetTimestampResolution(resolution);
        }

        private static unsafe Interop.BOOL QueryPerformanceCounter(long* timestamp)
        {
            return Interop.Sys.GetTimestamp(timestamp);
        }
    }
}
