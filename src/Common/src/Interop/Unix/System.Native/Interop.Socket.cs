// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Net;
using System.Net.Internals;
using System.Net.Sockets;
using System.Runtime.InteropServices;

internal static partial class Interop
{
    internal static partial class Sys
    {
        [DllImport(Libraries.SystemNative)]
        internal static extern unsafe Error Socket(AddressFamily addressFamily, SocketType socketType, ProtocolType protocolType, int* socket);
    }
}
