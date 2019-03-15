// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;

internal partial class Interop
{
    internal partial class Kernel32
    {
        // The actual native signature is:
        //      BOOL WINAPI QueryPerformanceFrequency(
        //          _Out_ LARGE_INTEGER* lpFrequency
        //      );
        //
        // We return an int (rather than a bool) to avoid the marshalling overhead.
        // We take a long* (rather than a out long) to avoid the pinning overhead.
        // We don't set last error since we don't need the extended error info.

        [DllImport(Libraries.Kernel32, BestFitMapping = false, CallingConvention = CallingConvention.Winapi, EntryPoint = "QueryPerformanceFrequency", ExactSpelling = true, PreserveSig = true, SetLastError = false)]
        internal static extern unsafe int QueryPerformanceFrequency(long* lpFrequency);
    }
}
