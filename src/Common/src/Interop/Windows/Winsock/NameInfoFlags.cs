// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Net.Sockets;
using System.Runtime.InteropServices;

namespace System.Net.Sockets
{
    [Flags]
    internal enum NameInfoFlags
    {
        NI_NOFQDN = 0x01, /* Only return nodename portion for local hosts */
        NI_NUMERICHOST = 0x02, /* Return numeric form of the host's address */
        NI_NAMEREQD = 0x04, /* Error if the host's name not in DNS */
        NI_NUMERICSERV = 0x08, /* Return numeric form of the service (port #) */
        NI_DGRAM = 0x10, /* Service is a datagram service */
    }        
} // namespace System.Net
