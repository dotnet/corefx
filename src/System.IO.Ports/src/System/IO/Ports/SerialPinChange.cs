// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.IO.Ports
{
    public enum SerialPinChange
    {
        CtsChanged = Interop.Kernel32.CommEvents.EV_CTS,
        DsrChanged = Interop.Kernel32.CommEvents.EV_DSR,
        CDChanged = Interop.Kernel32.CommEvents.EV_RLSD,
        Ring = Interop.Kernel32.CommEvents.EV_RING,
        Break = Interop.Kernel32.CommEvents.EV_BREAK
    }
}
