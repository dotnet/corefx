// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Threading;

namespace System.Net.Sockets
{
    //
    // IO-Control operations are not directly exposed.
    // blocking is controlled by "Blocking" property on socket (FIONBIO)
    // amount of data available is queried by "Available" property (FIONREAD)
    // The other flags are not exposed currently
    //
    internal static class IoctlSocketConstants
    {
        public const int FIONREAD = 0x4004667F;
        public const int FIONBIO = unchecked((int)0x8004667E);
        public const int FIOASYNC = unchecked((int)0x8004667D);
        public const int SIOGETEXTENSIONFUNCTIONPOINTER = unchecked((int)0xC8000006);
        //
        // not likely to block (sync IO ok)
        //
        // FIONBIO
        // FIONREAD
        // SIOCATMARK
        // SIO_RCVALL
        // SIO_RCVALL_MCAST
        // SIO_RCVALL_IGMPMCAST
        // SIO_KEEPALIVE_VALS
        // SIO_ASSOCIATE_HANDLE (opcode setting: I, T==1)
        // SIO_ENABLE_CIRCULAR_QUEUEING (opcode setting: V, T==1)
        // SIO_GET_BROADCAST_ADDRESS (opcode setting: O, T==1)
        // SIO_GET_EXTENSION_FUNCTION_POINTER (opcode setting: O, I, T==1)
        // SIO_MULTIPOINT_LOOPBACK (opcode setting: I, T==1)
        // SIO_MULTICAST_SCOPE (opcode setting: I, T==1)
        // SIO_TRANSLATE_HANDLE (opcode setting: I, O, T==1)
        // SIO_ROUTING_INTERFACE_QUERY (opcode setting: I, O, T==1)
        //
        // likely to block (reccommended for async IO)
        //
        // SIO_FIND_ROUTE (opcode setting: O, T==1)
        // SIO_FLUSH (opcode setting: V, T==1)
        // SIO_GET_QOS (opcode setting: O, T==1)
        // SIO_GET_GROUP_QOS (opcode setting: O, I, T==1)
        // SIO_SET_QOS (opcode setting: I, T==1)
        // SIO_SET_GROUP_QOS (opcode setting: I, T==1)
        // SIO_ROUTING_INTERFACE_CHANGE (opcode setting: I, T==1)
        // SIO_ADDRESS_LIST_CHANGE (opcode setting: T==1)
    }

    //
    // WinSock 2 extension -- bit values and indices for FD_XXX network events
    //
    [Flags]
    internal enum AsyncEventBits
    {
        FdNone = 0,
        FdRead = 1 << 0,
        FdWrite = 1 << 1,
        FdOob = 1 << 2,
        FdAccept = 1 << 3,
        FdConnect = 1 << 4,
        FdClose = 1 << 5,
        FdQos = 1 << 6,
        FdGroupQos = 1 << 7,
        FdRoutingInterfaceChange = 1 << 8,
        FdAddressListChange = 1 << 9,
        FdAllEvents = (1 << 10) - 1,
    }

    // Array position in NetworkEvents (WSAEnumNetworkEvents).
    internal enum AsyncEventBitsPos
    {
        FdReadBit = 0,
        FdWriteBit = 1,
        FdOobBit = 2,
        FdAcceptBit = 3,
        FdConnectBit = 4,
        FdCloseBit = 5,
        FdQosBit = 6,
        FdGroupQosBit = 7,
        FdRoutingInterfaceChangeBit = 8,
        FdAddressListChangeBit = 9,
        FdMaxEvents = 10,
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct NetworkEvents
    {
        //
        // Indicates which of the FD_XXX network events have occurred.
        //
        public AsyncEventBits Events;

        //
        // An array that contains any associated error codes, with an array index that corresponds to the position of event bits in lNetworkEvents. The identifiers FD_READ_BIT, FD_WRITE_BIT and other can be used to index the iErrorCode array.
        //
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = (int)AsyncEventBitsPos.FdMaxEvents)]
        public int[] ErrorCodes;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    internal struct hostent
    {
        public IntPtr h_name;
        public IntPtr h_aliases;
        public short h_addrtype;
        public short h_length;
        public IntPtr h_addr_list;
    }
} // namespace System.Net.Sockets
