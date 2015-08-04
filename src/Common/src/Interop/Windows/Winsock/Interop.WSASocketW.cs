// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Runtime.InteropServices;
using System.Net.Sockets;

internal static partial class Interop
{
    // TODO: Verify if these can be merged in Interop.WinsockAsync.cs
    internal static partial class Winsock
    {
        // TODO: The MCG compiler or Test infrastructure is currently broken and cannot properly interpret CharSet.Unicode.
        //       The code has been changed from WSASocket to WSASocketW to avoid an EXE loader issue.
        [DllImport(Interop.Libraries.Ws2_32, CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern SafeCloseSocket.InnerSafeCloseSocket WSASocketW(
                                                [In] AddressFamily addressFamily,
                                                [In] SocketType socketType,
                                                [In] ProtocolType protocolType,
                                                [In] IntPtr protocolInfo, // will be WSAProtcolInfo protocolInfo once we include QOS APIs
                                                [In] uint group,
                                                [In] SocketConstructorFlags flags
                                                );

        [DllImport(Interop.Libraries.Ws2_32, CharSet = CharSet.Unicode, SetLastError = true)]
        internal unsafe static extern SafeCloseSocket.InnerSafeCloseSocket WSASocketW(
                                    [In] AddressFamily addressFamily,
                                    [In] SocketType socketType,
                                    [In] ProtocolType protocolType,
                                    [In] byte* pinnedBuffer, // will be WSAProtcolInfo protocolInfo once we include QOS APIs
                                    [In] uint group,
                                    [In] SocketConstructorFlags flags
                                    );
    }
}
