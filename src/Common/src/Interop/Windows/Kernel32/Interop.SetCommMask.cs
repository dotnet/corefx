// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Win32.SafeHandles;
using System.Runtime.InteropServices;

internal partial class Interop
{
    internal partial class Kernel32
    {
        internal static class CommEvents
        {
            internal const int EV_RXCHAR = 0x01;
            internal const int EV_RXFLAG = 0x02;
            internal const int EV_CTS = 0x08;
            internal const int EV_DSR = 0x10;
            internal const int EV_RLSD = 0x20;
            internal const int EV_BREAK = 0x40;
            internal const int EV_ERR = 0x80;
            internal const int EV_RING = 0x100;
            internal const int ALL_EVENTS = 0x1fb;
        }

        [DllImport(Libraries.Kernel32, SetLastError=true, CharSet=CharSet.Auto)]
        internal static extern bool SetCommMask(
            SafeFileHandle hFile,
            int dwEvtMask
        );
    }
}