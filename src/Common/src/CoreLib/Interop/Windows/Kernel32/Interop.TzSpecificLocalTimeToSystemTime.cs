// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;

internal static partial class Interop
{
    internal static partial class Kernel32
    {
        [DllImport(Libraries.Kernel32)]
        internal static extern unsafe Interop.BOOL TzSpecificLocalTimeToSystemTime(
            IntPtr lpTimeZoneInformation,
            Interop.Kernel32.SYSTEMTIME* lpLocalTime,
            Interop.Kernel32.SYSTEMTIME* lpUniversalTime);
    }
}
