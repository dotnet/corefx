// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace System.Net.Sockets
{
    /// <devdoc>
    ///    <para>
    ///       Specifies the iocontrol codes that the <see cref='System.Net.Sockets.Socket'/> class supports.
    ///    </para>
    /// </devdoc>

    public enum IOControlCode : long
    {
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        AsyncIO = 0x8004667D,
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        NonBlockingIO = 0x8004667E,  //fionbio
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        DataToRead = 0x4004667F,  //fionread
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        OobDataRead = 0x40047307,
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        AssociateHandle = 0x88000001,  //SIO_ASSOCIATE_HANDLE
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        EnableCircularQueuing = 0x28000002,
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        Flush = 0x28000004,
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        GetBroadcastAddress = 0x48000005,
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        GetExtensionFunctionPointer = 0xC8000006,
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        GetQos = 0xC8000007,
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        GetGroupQos = 0xC8000008,
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        MultipointLoopback = 0x88000009,
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        MulticastScope = 0x8800000A,
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        SetQos = 0x8800000B,
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        SetGroupQos = 0x8800000C,
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        TranslateHandle = 0xC800000D,
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        RoutingInterfaceQuery = 0xC8000014,
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        RoutingInterfaceChange = 0x88000015,
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        AddressListQuery = 0x48000016,
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        AddressListChange = 0x28000017,
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        QueryTargetPnpHandle = 0x48000018,
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        NamespaceChange = 0x88000019,  //??????
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        AddressListSort = 0xC8000019,
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        ReceiveAll = 0x98000001,
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        ReceiveAllMulticast = 0x98000002,
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        ReceiveAllIgmpMulticast = 0x98000003,
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        KeepAliveValues = 0x98000004,
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        AbsorbRouterAlert = 0x98000005,     //?????
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        UnicastInterface = 0x98000006,    //?????
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        LimitBroadcasts = 0x98000007,
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        BindToInterface = 0x98000008,
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        MulticastInterface = 0x98000009,
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        AddMulticastGroupOnInterface = 0x9800000A,
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        DeleteMulticastGroupFromInterface = 0x9800000B
    }
} // namespace System.Net.Sockets
