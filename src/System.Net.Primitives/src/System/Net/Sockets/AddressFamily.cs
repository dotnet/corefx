// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace System.Net.Sockets
{
    /// <devdoc>
    ///    <para>
    ///       Specifies the address families that an instance of the <see cref='System.Net.Sockets.Socket'/>
    ///       class can use.
    ///    </para>
    /// </devdoc>
    public enum AddressFamily
    {
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        Unknown = -1,   // Unknown
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        Unspecified = 0,    // unspecified
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        Unix = 1,    // local to host (pipes, portals)
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        InterNetwork = 2,    // internetwork: UDP, TCP, etc.
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        ImpLink = 3,    // arpanet imp addresses
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        Pup = 4,    // pup protocols: e.g. BSP
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        Chaos = 5,    // mit CHAOS protocols
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        NS = 6,    // XEROX NS protocols
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        Ipx = NS,   // IPX and SPX
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        Iso = 7,    // ISO protocols
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        Osi = Iso,  // OSI is ISO
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        Ecma = 8,    // european computer manufacturers
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        DataKit = 9,    // datakit protocols
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        Ccitt = 10,   // CCITT protocols, X.25 etc
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        Sna = 11,   // IBM SNA
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        DecNet = 12,   // DECnet
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        DataLink = 13,   // Direct data link interface
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        Lat = 14,   // LAT
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        HyperChannel = 15,   // NSC Hyperchannel
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        AppleTalk = 16,   // AppleTalk
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        NetBios = 17,   // NetBios-style addresses
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        VoiceView = 18,   // VoiceView
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        FireFox = 19,   // FireFox
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        Banyan = 21,   // Banyan
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        Atm = 22,   // Native ATM Services
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        InterNetworkV6 = 23,   // Internetwork Version 6
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        Cluster = 24,   // Microsoft Wolfpack
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        Ieee12844 = 25,   // IEEE 1284.4 WG AF
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        Irda = 26,   // IrDA
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        NetworkDesigners = 28,   // Network Designers OSI & gateway enabled protocols
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        Max = 29,   // Max
                    /*
                    #define AF_UNSPEC       0               // unspecified 

                    // Although  AF_UNSPEC  is  defined for backwards compatibility, using
                    // AF_UNSPEC for the "af" parameter when creating a socket is STRONGLY
                    // DISCOURAGED.    The  interpretation  of  the  "protocol"  parameter
                    // depends  on the actual address family chosen.  As environments grow
                    // to  include  more  and  more  address families that use overlapping
                    // protocol  values  there  is  more  and  more  chance of choosing an
                    // undesired address family when AF_UNSPEC is used.

                    #define AF_UNIX         1               // local to host (pipes, portals) 
                    #define AF_INET         2               // internetwork: UDP, TCP, etc. 
                    #define AF_IMPLINK      3               // arpanet imp addresses 
                    #define AF_PUP          4               // pup protocols: e.g. BSP 
                    #define AF_CHAOS        5               // mit CHAOS protocols 
                    #define AF_NS           6               // XEROX NS protocols 
                    #define AF_IPX          AF_NS           // IPX protocols: IPX, SPX, etc. 
                    #define AF_ISO          7               // ISO protocols 
                    #define AF_OSI          AF_ISO          // OSI is ISO 
                    #define AF_ECMA         8               // european computer manufacturers 
                    #define AF_DATAKIT      9               // datakit protocols 
                    #define AF_CCITT        10              // CCITT protocols, X.25 etc 
                    #define AF_SNA          11              // IBM SNA 
                    #define AF_DECnet       12              // DECnet 
                    #define AF_DLI          13              // Direct data link interface 
                    #define AF_LAT          14              // LAT 
                    #define AF_HYLINK       15              // NSC Hyperchannel 
                    #define AF_APPLETALK    16              // AppleTalk 
                    #define AF_NETBIOS      17              // NetBios-style addresses 
                    #define AF_VOICEVIEW    18              // VoiceView 
                    #define AF_FIREFOX      19              // Protocols from Firefox 
                    #define AF_UNKNOWN1     20              // Somebody is using this! 
                    #define AF_BAN          21              // Banyan 
                    #define AF_ATM          22              // Native ATM Services 
                    #define AF_INET6        23              // Internetwork Version 6 
                    #define AF_CLUSTER      24              // Microsoft Wolfpack 
                    #define AF_12844        25              // IEEE 1284.4 WG AF 
                    #define AF_IRDA         26              // IrDA 
                    #define AF_NETDES       28              // Network Designers OSI & gateway enabled protocols 
                    #define AF_MAX          29
                    */

    }; // enum AddressFamily
} // namespace System.Net.Sockets
