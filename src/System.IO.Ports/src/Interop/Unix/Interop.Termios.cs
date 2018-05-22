// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.IO.Ports;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

internal static partial class Interop
{
    internal static partial class Termios
    {
        internal enum Signals
        {
            SignalDtr = 1,
            SignalDsr = 2,
            SignalRts = 3,
            SignalCts = 4,
            SignalDcd = 5,
        }

        internal enum Queue
        {
            AllQueues = 0,
            ReceiveQueue = 1,
            SendQueue = 2,
        }

        [DllImport(Libraries.IOPortsNative, EntryPoint = "Termios_Reset", SetLastError = true)]
        internal static extern int TermiosReset(SafeFileHandle fd, int speed, int data, StopBits stop, Parity parity, Handshake flow);

        [DllImport(Libraries.IOPortsNative, EntryPoint = "Termios_GetSignal", SetLastError = true)]
        internal static extern int TermiosGetSignal(SafeFileHandle fd, Signals signal);

        [DllImport(Libraries.IOPortsNative, EntryPoint = "Termios_SetSignal", SetLastError = true)]
        internal static extern int TermiosGetSignal(SafeFileHandle fd, Signals signal, int set);

        [DllImport(Libraries.IOPortsNative, EntryPoint = "Termios_SetSpeed", SetLastError = true)]
        internal static extern int TermiosSetSpeed(SafeFileHandle fd, int speed);

        [DllImport(Libraries.IOPortsNative, EntryPoint = "Termios_GetSpeed", SetLastError = true)]
        internal static extern int TermiosGetSpeed(SafeFileHandle fd);

        [DllImport(Libraries.IOPortsNative, EntryPoint = "Termios_AvailableBytes", SetLastError = true)]
        internal static extern int TermiosGetAvailableBytes(SafeFileHandle fd, int input);

        [DllImport(Libraries.IOPortsNative, EntryPoint = "Termios_Discard", SetLastError = true)]
        internal static extern int TermiosDiscard(SafeFileHandle fd, Queue input);

        [DllImport(Libraries.IOPortsNative, EntryPoint = "Termios_Drain", SetLastError = true)]
        internal static extern int TermiosDrain(SafeFileHandle fd);

        [DllImport(Libraries.IOPortsNative, EntryPoint = "Termios_SendBreak", SetLastError = true)]
        internal static extern int TermiosSendBreak(SafeFileHandle fd, int duration);
    }
}
