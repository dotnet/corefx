// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Diagnostics
{
    public partial class Stopwatch
    {
        private static unsafe Interop.BOOL QueryPerformanceFrequency(long* lpFrequency)
        {
            return Interop.Kernel32.QueryPerformanceFrequency(lpFrequency);
        }

        private static unsafe Interop.BOOL QueryPerformanceCounter(long* lpPerformanceCount)
        {
            return Interop.Kernel32.QueryPerformanceCounter(lpPerformanceCount);
        }
    }
}
