// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;

internal static partial class Interop
{
    internal static partial class Sys
    {
        [DllImport(Libraries.SystemNative, BestFitMapping = false, CallingConvention = CallingConvention.Cdecl, EntryPoint = "SystemNative_GetTimestampResolution", ExactSpelling = true, PreserveSig = true, SetLastError = false, ThrowOnUnmappableChar = false)]
        internal static extern unsafe int GetTimestampResolution(long* resolution);

        [DllImport(Libraries.SystemNative, BestFitMapping = false, CallingConvention = CallingConvention.Cdecl, EntryPoint = "SystemNative_GetTimestamp", ExactSpelling = true, PreserveSig = true, SetLastError = false, ThrowOnUnmappableChar = false)]
        internal static extern unsafe int GetTimestamp(long* timestamp);
    }
}
