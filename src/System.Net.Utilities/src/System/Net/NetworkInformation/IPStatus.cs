// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace System.Net.NetworkInformation
{
    //these are used for the status fields in icmpsendecho and its variants
    //the problem is that some of these are icmp errors that we care about and would
    //be returned in the reply structure.  Others are something we should throw on.

    internal enum IcmpV4Type
    {
        //can map these
        ICMP4_ECHO_REPLY = 0, // Echo Reply.
        ICMP4_DST_UNREACH = 3, // Destination Unreachable.
        ICMP4_SOURCE_QUENCH = 4, // Source Quench.
        ICMP4_TIME_EXCEEDED = 11, // Time Exceeded.
        ICMP4_PARAM_PROB = 12, // Parameter Problem.

        //unmappable
        ICMP4_REDIRECT = 5, // Redirect.
        ICMP4_ECHO_REQUEST = 8, // Echo Request.
        ICMP4_ROUTER_ADVERT = 9, // Router Advertisement.
        ICMP4_ROUTER_SOLICIT = 10, // Router Solicitation.
        ICMP4_TIMESTAMP_REQUEST = 13, // Timestamp Request.
        ICMP4_TIMESTAMP_REPLY = 14, // Timestamp Reply.
        ICMP4_MASK_REQUEST = 17, // Address Mask Request.
        ICMP4_MASK_REPLY = 18, // Address Mask Reply.
    }

    internal enum IcmpV4Code
    {
        ICMP4_UNREACH_NET = 0,
        ICMP4_UNREACH_HOST = 1,
        ICMP4_UNREACH_PROTOCOL = 2,
        ICMP4_UNREACH_PORT = 3,
        ICMP4_UNREACH_FRAG_NEEDED = 4,
        ICMP4_UNREACH_SOURCEROUTE_FAILED = 5,
        ICMP4_UNREACH_NET_UNKNOWN = 6,
        ICMP4_UNREACH_HOST_UNKNOWN = 7,
        ICMP4_UNREACH_ISOLATED = 8,
        ICMP4_UNREACH_NET_ADMIN = 9,
        ICMP4_UNREACH_HOST_ADMIN = 10,
        ICMP4_UNREACH_NET_TOS = 11,
        ICMP4_UNREACH_HOST_TOS = 12,
        ICMP4_UNREACH_ADMIN = 13,
    }


    public enum IPStatus
    {
        Success = 0,
        //BufferTooSmall = 11000 + 1,

        DestinationNetworkUnreachable = 11000 + 2,
        DestinationHostUnreachable = 11000 + 3,
        DestinationProtocolUnreachable = 11000 + 4,
        DestinationPortUnreachable = 11000 + 5,
        DestinationProhibited = 11000 + 4,

        NoResources = 11000 + 6,
        BadOption = 11000 + 7,
        HardwareError = 11000 + 8,
        PacketTooBig = 11000 + 9,
        TimedOut = 11000 + 10,
        //  BadRequest = 11000 + 11,
        BadRoute = 11000 + 12,

        TtlExpired = 11000 + 13,
        TtlReassemblyTimeExceeded = 11000 + 14,

        ParameterProblem = 11000 + 15,
        SourceQuench = 11000 + 16,
        //OptionTooBig = 11000 + 17,
        BadDestination = 11000 + 18,

        DestinationUnreachable = 11000 + 40,
        TimeExceeded = 11000 + 41,
        BadHeader = 11000 + 42,
        UnrecognizedNextHeader = 11000 + 43,
        IcmpError = 11000 + 44,
        DestinationScopeMismatch = 11000 + 45,
        Unknown = -1,
    }
}
