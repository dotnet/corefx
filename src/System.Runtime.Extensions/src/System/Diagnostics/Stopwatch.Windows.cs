// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.CompilerServices;

namespace System.Diagnostics
{
    public partial class Stopwatch
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static unsafe int QueryPerformanceFrequency(long* lpFrequency)
        {
            return Interop.Kernel32.QueryPerformanceFrequency(lpFrequency);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static unsafe int QueryPerformanceCounter(long* lpPerformanceCount)
        {
            return Interop.Kernel32.QueryPerformanceCounter(lpPerformanceCount);
        }
    }
}
