// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Runtime.InteropServices;
using System.Net.Sockets;

internal static partial class Interop
{
    internal static partial class Winsock
    {
            [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
            internal struct WSAPROTOCOL_INFO
            {
                internal uint dwServiceFlags1;
                internal uint dwServiceFlags2;
                internal uint dwServiceFlags3;
                internal uint dwServiceFlags4;
                internal uint dwProviderFlags;
                private Guid _providerId;
                internal uint dwCatalogEntryId;
                private WSAPROTOCOLCHAIN _protocolChain;
                internal int iVersion;
                internal AddressFamily iAddressFamily;
                internal int iMaxSockAddr;
                internal int iMinSockAddr;
                internal int iSocketType;
                internal int iProtocol;
                internal int iProtocolMaxOffset;
                internal int iNetworkByteOrder;
                internal int iSecurityScheme;
                internal uint dwMessageSize;
                internal uint dwProviderReserved;
                [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
                internal string szProtocol;
            }
    }
}
