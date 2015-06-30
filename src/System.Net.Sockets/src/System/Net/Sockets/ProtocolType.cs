// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace System.Net.Sockets
{
    /// <devdoc>
    ///    <para>
    ///       Specifies the protocols that the <see cref='System.Net.Sockets.Socket'/> class supports.
    ///    </para>
    /// </devdoc>

    public enum ProtocolType
    {
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        IP = 0,    // dummy for IP

        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        IPv6HopByHopOptions = 0,
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        Icmp = 1,    // control message protocol
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        Igmp = 2,    // group management protocol
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        Ggp = 3,    // gateway^2 (deprecated)

        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        IPv4 = 4,
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        Tcp = 6,    // tcp
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        Pup = 12,   // pup
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        Udp = 17,   // user datagram protocol
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        Idp = 22,   // xns idp
        /// <devdoc>
        /// <para>[To be supplied.]</para>
        /// </devdoc>
        IPv6 = 41,   // IPv4
        /// <devdoc>
        /// <para>[To be supplied.]</para>
        /// </devdoc>
        IPv6RoutingHeader = 43,   // IPv6RoutingHeader
        /// <devdoc>
        /// <para>[To be supplied.]</para>
        /// </devdoc>
        IPv6FragmentHeader = 44,   // IPv6FragmentHeader
        /// <devdoc>
        /// <para>[To be supplied.]</para>
        /// </devdoc>
        IPSecEncapsulatingSecurityPayload = 50,   // IPSecEncapsulatingSecurityPayload
        /// <devdoc>
        /// <para>[To be supplied.]</para>
        /// </devdoc>
        IPSecAuthenticationHeader = 51,   // IPSecAuthenticationHeader
        /// <devdoc>
        /// <para>[To be supplied.]</para>
        /// </devdoc>
        IcmpV6 = 58,   // IcmpV6
        /// <devdoc>
        /// <para>[To be supplied.]</para>
        /// </devdoc>
        IPv6NoNextHeader = 59,   // IPv6NoNextHeader
        /// <devdoc>
        /// <para>[To be supplied.]</para>
        /// </devdoc>
        IPv6DestinationOptions = 60,   // IPv6DestinationOptions
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        ND = 77,   // UNOFFICIAL net disk proto
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        Raw = 255,  // raw IP packet

        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        Unspecified = 0,
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        Ipx = 1000,
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        Spx = 1256,
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        SpxII = 1257,

        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        Unknown = -1,   // unknown protocol type
                        /*
                        consider adding:

                        #define IPPROTO_RSVP                0x2e 
                        #define DNPROTO_NSP                 1               // DECnet NSP transport protocol
                        #define ISOPROTO_TP_CONS            25              // Transport over CONS
                        #define ISOPROTO_CLTP_CONS          tba             // Connectionless Transport over CONS
                        #define ISOPROTO_TP4_CLNS           29              // Transport class 4 over CLNS
                        #define ISOPROTO_CLTP_CLNS          30              // Connectionless Transport over CLNS
                        #define ISOPROTO_X25                32              // X.25
                        #define ISOPROTO_X25PVC             tba             // Permanent Virtual Circuit
                        #define ISOPROTO_X25SVC             ISOPROTO_X25    // Switched Virtual Circuit
                        #define ISOPROTO_TP                 ISOPROTO_TP4_CLNS
                        #define ISOPROTO_CLTP               ISOPROTO_CLTP_CLNS
                        #define ISOPROTO_TP0_TCP            tba             // Transport class 0 over TCP (RFC1006)
                        #define ATMPROTO_AALUSER            0x00            // User-defined AAL
                        #define ATMPROTO_AAL1               0x01            // AAL 1
                        #define ATMPROTO_AAL2               0x02            // AAL 2
                        #define ATMPROTO_AAL34              0x03            // AAL 3/4
                        #define ATMPROTO_AAL5               0x05            // AAL 5
                        */


    } // enum ProtocolType
} // namespace System.Net.Sockets
