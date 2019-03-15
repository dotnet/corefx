// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.CompilerServices;

namespace System.Diagnostics
{
    public partial class Stopwatch
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static unsafe int QueryPerformanceFrequency(long* resolution)
        {
            return Interop.Sys.GetTimestampResolution(resolution);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static unsafe int QueryPerformanceCounter(long* timestamp)
        {
            return Interop.Sys.GetTimestamp(timestamp);
        }
    }
}
