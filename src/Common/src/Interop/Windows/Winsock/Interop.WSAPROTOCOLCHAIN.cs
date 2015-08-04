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
        internal struct WSAPROTOCOLCHAIN
        {
            internal int ChainLen;                                 /* the length of the chain,     */
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 7)]
            internal uint[] ChainEntries;       /* a list of dwCatalogEntryIds */
        }
    }
}
