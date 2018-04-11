// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.IO.Ports;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

internal static partial class Interop
{
    internal static partial class Sys
    {
        [DllImport(Libraries.SystemNative, EntryPoint = "SystemNative_TermiosReset", SetLastError = true)]
        internal static extern int TerminalReset(SafeFileHandle fd, int speed, int data, StopBits stop, Parity parity, Handshake flow);

        [DllImport(Libraries.SystemNative, EntryPoint = "SystemNative_TermiosGetDcd", SetLastError = true)]
        internal static extern int TerminalGetCd(SafeFileHandle fd);

        [DllImport(Libraries.SystemNative, EntryPoint = "SystemNative_TermiosGetCts", SetLastError = true)]
        internal static extern int TerminalGetCts(SafeFileHandle fd);

        [DllImport(Libraries.SystemNative, EntryPoint = "SystemNative_TermiosGetRts", SetLastError = true)]
        internal static extern int TerminalGetRts(SafeFileHandle fd);

        [DllImport(Libraries.SystemNative, EntryPoint = "SystemNative_TermiosGetDsr", SetLastError = true)]
        internal static extern int TerminalGetDsr(SafeFileHandle fd);

        [DllImport(Libraries.SystemNative, EntryPoint = "SystemNative_TermiosGetDtr", SetLastError = true)]
        internal static extern int TerminalGetDtr(SafeFileHandle fd);

        [DllImport(Libraries.SystemNative, EntryPoint = "SystemNative_TermiosSetDtr", SetLastError = true)]
        internal static extern int TerminalSetDtr(SafeFileHandle fd, int value);

        [DllImport(Libraries.SystemNative, EntryPoint = "SystemNative_TermiosSetRts", SetLastError = true)]
        internal static extern int TerminalSetRts(SafeFileHandle fd, int value);

        [DllImport(Libraries.SystemNative, EntryPoint = "SystemNative_TermiosSetSpeed", SetLastError = true)]
        internal static extern int TerminalSetSpeed(SafeFileHandle fd, int speed);

        [DllImport(Libraries.SystemNative, EntryPoint = "SystemNative_TermiosGetSpeed", SetLastError = true)]
        internal static extern int TerminalGetSpeed(SafeFileHandle fd);

        [DllImport(Libraries.SystemNative, EntryPoint = "SystemNative_TermiosAvailableBytes", SetLastError = true)]
        internal static extern int TerminalGetAvailableBytes(SafeFileHandle fd, int input);

        [DllImport(Libraries.SystemNative, EntryPoint = "SystemNative_TermiosDiscard", SetLastError = true)]
        internal static extern int TerminalDiscard(SafeFileHandle fd, int input);

        [DllImport(Libraries.SystemNative, EntryPoint = "SystemNative_TermiosDrain", SetLastError = true)]
        internal static extern int TerminalDrain(SafeFileHandle fd);

        [DllImport(Libraries.SystemNative, EntryPoint = "SystemNative_TermiosSendBreak", SetLastError = true)]
        internal static extern int TerminalSendBreak(SafeFileHandle fd, int duration);
    }
}
