// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.IO.Ports
{
    public enum Parity
    {
        None = Interop.Kernel32.DCBParity.NOPARITY,
        Odd = Interop.Kernel32.DCBParity.ODDPARITY,
        Even = Interop.Kernel32.DCBParity.EVENPARITY,
        Mark = Interop.Kernel32.DCBParity.MARKPARITY,
        Space = Interop.Kernel32.DCBParity.SPACEPARITY
    };
}

