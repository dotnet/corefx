// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.IO.Ports
{
    public enum SerialError
    {
        TXFull = Interop.Kernel32.CommErrors.CE_TXFULL,
        RXOver = Interop.Kernel32.CommErrors.CE_RXOVER,
        Overrun = Interop.Kernel32.CommErrors.CE_OVERRUN,
        RXParity = Interop.Kernel32.CommErrors.CE_PARITY,
        Frame = Interop.Kernel32.CommErrors.CE_FRAME,
    }
}
