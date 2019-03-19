// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Diagnostics
{
    public partial class Stopwatch
    {
        private static unsafe long QueryPerformanceFrequency()
        {
            long resolution;

            Interop.BOOL result = Interop.Kernel32.QueryPerformanceFrequency(&resolution);
            Debug.Assert(result != Interop.BOOL.FALSE);

            return resolution;
        }

        private static unsafe long QueryPerformanceCounter(long* lpPerformanceCount)
        {
            long timestamp;

            Interop.BOOL result = Interop.Kernel32.QueryPerformanceCounter(&timestamp);
            Debug.Assert(result != Interop.BOOL.FALSE);

            return timestamp;
        }
    }
}
