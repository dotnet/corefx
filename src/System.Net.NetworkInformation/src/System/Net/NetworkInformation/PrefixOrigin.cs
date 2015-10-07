// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace System.Net.NetworkInformation
{
    /// Specifies how an IP address network prefix was located.
    public enum PrefixOrigin
    {
        Other = 0,
        Manual,
        WellKnown,
        Dhcp,
        RouterAdvertisement,
    }
}
