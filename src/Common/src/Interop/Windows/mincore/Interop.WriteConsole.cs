// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;

internal partial class Interop
{
    internal partial class mincore
    {
        [DllImport(Libraries.Console_L1, CharSet = CharSet.Unicode, SetLastError = true, EntryPoint = "WriteConsoleW")]
        internal static unsafe extern bool WriteConsole(
            IntPtr hConsoleOutput,
            Byte* lpBuffer,
            Int32 nNumberOfCharsToWrite,
            out Int32 lpNumberOfCharsWritten,
            IntPtr lpReservedMustBeNull);
    }
}
