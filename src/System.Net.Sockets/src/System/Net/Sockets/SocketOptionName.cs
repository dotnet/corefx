// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace System.Net.Sockets
{
    //
    // Option names per-socket.
    //

    /// <devdoc>
    ///    <para>
    ///       Defines socket option names for the <see cref='System.Net.Sockets.Socket'/> class.
    ///    </para>
    /// </devdoc>
    //UEUE
    public enum SocketOptionName
    {
        //
        // good for SocketOptionLevel.Socket
        //

        /// <devdoc>
        ///    <para>Record debugging information.</para>
        /// </devdoc>
        Debug = 0x0001,           // turn on debugging info recording
        /// <devdoc>
        ///    <para>Socket is listening.</para>
        /// </devdoc>
        AcceptConnection = 0x0002,           // socket has had listen()
        /// <devdoc>
        ///    <para>
        ///       Allows the socket to be bound to an address that is already in use.
        ///    </para>
        /// </devdoc>
        ReuseAddress = 0x0004,           // allow local address reuse
        /// <devdoc>
        ///    <para>
        ///       Send keep-alives.
        ///    </para>
        /// </devdoc>
        KeepAlive = 0x0008,           // keep connections alive
        /// <devdoc>
        ///    <para>
        ///       Do not route, send directly to interface addresses.
        ///    </para>
        /// </devdoc>
        DontRoute = 0x0010,           // just use interface addresses
        /// <devdoc>
        ///    <para>
        ///       Permit sending broadcast messages on the socket.
        ///    </para>
        /// </devdoc>
        Broadcast = 0x0020,           // permit sending of broadcast msgs
        /// <devdoc>
        ///    <para>
        ///       Bypass hardware when possible.
        ///    </para>
        /// </devdoc>
        UseLoopback = 0x0040,           // bypass hardware when possible
        /// <devdoc>
        ///    <para>
        ///       Linger on close if unsent data is present.
        ///    </para>
        /// </devdoc>
        Linger = 0x0080,           // linger on close if data present
        /// <devdoc>
        ///    <para>
        ///       Receives out-of-band data in the normal data stream.
        ///    </para>
        /// </devdoc>
        OutOfBandInline = 0x0100,           // leave received OOB data in line
        /// <devdoc>
        ///    <para>
        ///       Close socket gracefully without lingering.
        ///    </para>
        /// </devdoc>
        DontLinger = ~Linger,
        /// <devdoc>
        ///    <para>
        ///       Enables a socket to be bound for exclusive access.
        ///    </para>
        /// </devdoc>
        ExclusiveAddressUse = ~ReuseAddress,    // disallow local address reuse
        /// <devdoc>
        ///    <para>
        ///       Specifies the total per-socket buffer space reserved for sends. This is
        ///       unrelated to the maximum message size or the size of a TCP window.
        ///    </para>
        /// </devdoc>
        SendBuffer = 0x1001,           // send buffer size
        /// <devdoc>
        ///    <para>
        ///       Send low water mark.
        ///    </para>
        /// </devdoc>
        ReceiveBuffer = 0x1002,           // receive buffer size
        /// <devdoc>
        ///    <para>
        ///       Specifies the total per-socket buffer space reserved for receives. This is unrelated to the maximum message size or the size of a TCP window.
        ///    </para>
        /// </devdoc>
        SendLowWater = 0x1003,           // send low-water mark
        /// <devdoc>
        ///    <para>
        ///       Receive low water mark.
        ///    </para>
        /// </devdoc>
        ReceiveLowWater = 0x1004,           // receive low-water mark
        /// <devdoc>
        ///    <para>
        ///       Send timeout.
        ///    </para>
        /// </devdoc>
        SendTimeout = 0x1005,           // send timeout
        /// <devdoc>
        ///    <para>
        ///       Receive timeout.
        ///    </para>
        /// </devdoc>
        ReceiveTimeout = 0x1006,           // receive timeout
        /// <devdoc>
        ///    <para>
        ///       Get error status and clear.
        ///    </para>
        /// </devdoc>
        Error = 0x1007,          // get error status and clear
        /// <devdoc>
        ///    <para>
        ///       Get socket type.
        ///    </para>
        /// </devdoc>
        Type = 0x1008,           // get socket type
        /// <devdoc>
        ///    <para>
        ///       Maximum queue length that can be specified by <see cref='System.Net.Sockets.Socket.Listen'/>.
        ///    </para>
        /// </devdoc>
        MaxConnections = 0x7fffffff,       // Maximum queue length specifiable by listen.


        //
        // the following values are taken from ws2tcpip.h,
        // note that these are understood only by ws2_32.dll and are not backwards compatible
        // with the values found in winsock.h which are understood by wsock32.dll.
        //

        //
        // good for SocketOptionLevel.IP
        //

        /// <devdoc>
        ///    <para>
        ///       IP options.
        ///    </para>
        /// </devdoc>
        IPOptions = 1,
        /// <devdoc>
        ///    <para>
        ///       Header is included with data.
        ///    </para>
        /// </devdoc>
        HeaderIncluded = 2,
        /// <devdoc>
        ///    <para>
        ///       IP type of service and preced.
        ///    </para>
        /// </devdoc>
        TypeOfService = 3,
        /// <devdoc>
        ///    <para>
        ///       IP time to live.
        ///    </para>
        /// </devdoc>
        IpTimeToLive = 4,
        /// <devdoc>
        ///    <para>
        ///       IP multicast interface.
        ///       - Additional comments by mbolien:
        ///         multicast interface  You provide it with an SOCKADDR_IN, and that tells the
        ///         system that it should receive multicast messages on that interface (if you
        ///         have more than one interface).  Binding the socket is not sufficient, since
        ///         if the Ethernet hardware isnt set up to grab the multicast packets, it wont
        ///         do good to bind the socket.  Kinda like raw sockets.  Unless you
        ///         put the Ethernet card in promiscuous mode, youll only get stuff sent to and
        ///         from your machine.
        ///    </para>
        /// </devdoc>
        MulticastInterface = 9,
        /// <devdoc>
        ///    <para>
        ///       IP multicast time to live.
        ///    </para>
        /// </devdoc>
        MulticastTimeToLive = 10,
        /// <devdoc>
        ///    <para>
        ///       IP Multicast loopback.
        ///    </para>
        /// </devdoc>
        MulticastLoopback = 11,
        /// <devdoc>
        ///    <para>
        ///       Add an IP group membership.
        ///    </para>
        /// </devdoc>
        AddMembership = 12,
        /// <devdoc>
        ///    <para>
        ///       Drop an IP group membership.
        ///    </para>
        /// </devdoc>
        DropMembership = 13,
        /// <devdoc>
        ///    <para>
        ///       Don't fragment IP datagrams.
        ///    </para>
        /// </devdoc>
        DontFragment = 14,
        /// <devdoc>
        ///    <para>
        ///       Join IP group/source.
        ///    </para>
        /// </devdoc>
        AddSourceMembership = 15,
        /// <devdoc>
        ///    <para>
        ///       Leave IP group/source.
        ///    </para>
        /// </devdoc>
        DropSourceMembership = 16,
        /// <devdoc>
        ///    <para>
        ///       Block IP group/source.
        ///    </para>
        /// </devdoc>
        BlockSource = 17,
        /// <devdoc>
        ///    <para>
        ///       Unblock IP group/source.
        ///    </para>
        /// </devdoc>
        UnblockSource = 18,
        /// <devdoc>
        ///    <para>
        ///       Receive packet information for ipv4.
        ///    </para>
        /// </devdoc>
        PacketInformation = 19,


        //
        //good for ipv6
        //

        HopLimit = 21,            //IPV6_HOPLIMIT

        IPProtectionLevel = 23,     //IP_PROTECTION_LEVEL

        IPv6Only = 27,              //IPV6_V6ONLY

        //
        // good for SocketOptionLevel.Tcp
        //

        /// <devdoc>
        ///    <para>
        ///       Disables the Nagle algorithm for send coalescing.
        ///    </para>
        /// </devdoc>
        NoDelay = 1,
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        BsdUrgent = 2,
        Expedited = 2,


        //
        // good for SocketOptionLevel.Udp
        //

        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        NoChecksum = 1,
        /// <devdoc>
        ///    <para>
        ///       Udp-Lite checksum coverage.
        ///    </para>
        /// </devdoc>
        ChecksumCoverage = 20,

        UpdateAcceptContext = 0x700B,

        UpdateConnectContext = 0x7010,
    }; // enum SocketOptionName
} // namespace System.Net.Sockets
