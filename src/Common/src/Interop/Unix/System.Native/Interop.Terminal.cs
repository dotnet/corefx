// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;
using System.IO.Ports;

internal static partial class Interop
{
    internal static partial class Sys
    {
        [DllImport(Libraries.SystemNative, EntryPoint = "SystemNative_Termios_Reset", SetLastError = true)]
        internal static extern int TerminalReset(SafeFileHandle fd, int speed, int data, StopBits stop, Parity parity, Handshake flow);

        [DllImport(Libraries.SystemNative, EntryPoint = "SystemNative_Termios_GetDcd", SetLastError = true)]
        internal static extern int TerminalGetCd(SafeFileHandle fd);

        [DllImport(Libraries.SystemNative, EntryPoint = "SystemNative_Termios_GetCts", SetLastError = true)]
        internal static extern int TerminalGetCts(SafeFileHandle fd);

        [DllImport(Libraries.SystemNative, EntryPoint = "SystemNative_Termios_GetRts", SetLastError = true)]
        internal static extern int TerminalGetRts(SafeFileHandle fd);

        [DllImport(Libraries.SystemNative, EntryPoint = "SystemNative_Termios_GetDsr", SetLastError = true)]
        internal static extern int TerminalGetDsr(SafeFileHandle fd);

        [DllImport(Libraries.SystemNative, EntryPoint = "SystemNative_Termios_GetDtr", SetLastError = true)]
        internal static extern int TerminalGetDtr(SafeFileHandle fd);

        [DllImport(Libraries.SystemNative, EntryPoint = "SystemNative_Termios_SetDtr", SetLastError = true)]
        internal static extern int TerminalSetDtr(SafeFileHandle fd, int value);

        [DllImport(Libraries.SystemNative, EntryPoint = "SystemNative_Termios_SetRts", SetLastError = true)]
        internal static extern int TerminalSetRts(SafeFileHandle fd, int value);

        [DllImport(Libraries.SystemNative, EntryPoint = "SystemNative_Termios_SetSpeed", SetLastError = true)]
        internal static extern int TerminalSetSpeed(SafeFileHandle fd, int speed);

        [DllImport(Libraries.SystemNative, EntryPoint = "SystemNative_Termios_GetSpeed", SetLastError = true)]
        internal static extern int TerminalGetSpeed(SafeFileHandle fd);
    }
}
