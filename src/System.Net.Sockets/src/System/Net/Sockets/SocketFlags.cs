// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace System.Net.Sockets
{
    // Provides constant values for socket messages.

    [Flags]
    public enum SocketFlags
    {
        // Use no flags for this call.
        None = 0x0000,

        // Process out-of-band data.
        OutOfBand = 0x0001,

        // Peek at incoming message.
        Peek = 0x0002,

        // Send without using routing tables.
        DontRoute = 0x0004,

        // This flag is ignored, but exists for compatibility.
        MaxIOVectorLength = 0x0010,
        
        // Partial send or recv for message.
        Truncated = 0x0100,
        ControlDataTruncated = 0x0200,
        Broadcast = 0x0400,
        Multicast = 0x0800,

        Partial = 0x8000,
    }
}
