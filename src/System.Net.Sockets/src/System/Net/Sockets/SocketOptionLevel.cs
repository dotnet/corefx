// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace System.Net.Sockets
{
    //
    // Option flags per-socket.
    //

    /// <devdoc>
    ///    <para>
    ///       Defines socket option levels for the <see cref='System.Net.Sockets.Socket'/> class.
    ///    </para>
    /// </devdoc>
    //UEUE
    public enum SocketOptionLevel
    {
        /// <devdoc>
        ///    <para>
        ///       Indicates socket options apply to the socket itself.
        ///    </para>
        /// </devdoc>
        Socket = 0xffff,

        /// <devdoc>
        ///    <para>
        ///       Indicates socket options apply to IP sockets.
        ///    </para>
        /// </devdoc>
        IP = ProtocolType.IP,

        /// <devdoc>
        /// <para>
        /// Indicates socket options apply to IPv6 sockets.
        /// </para>
        /// </devdoc>
        IPv6 = ProtocolType.IPv6,

        /// <devdoc>
        ///    <para>
        ///       Indicates socket options apply to Tcp sockets.
        ///    </para>
        /// </devdoc>
        Tcp = ProtocolType.Tcp,

        /// <devdoc>
        /// <para>
        /// Indicates socket options apply to Udp sockets.
        /// </para>
        /// </devdoc>
        //UEUE
        Udp = ProtocolType.Udp,
    }; // enum SocketOptionLevel
} // namespace System.Net.Sockets
