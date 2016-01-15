// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Net.Sockets;
using System.Runtime.InteropServices;

internal static partial class Interop
{
    internal static partial class Sys
    {
        [DllImport(Libraries.SystemNative, EntryPoint = "SystemNative_SetSockOpt")]
        internal static extern unsafe Error SetSockOpt(int socket, SocketOptionLevel optionLevel, SocketOptionName optionName, byte* optionValue, int optionLen);
    }
}
