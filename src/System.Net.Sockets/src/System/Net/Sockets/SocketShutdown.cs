// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace System.Net.Sockets
{
    /// <devdoc>
    ///    <para>
    ///       Defines constants used by the <see cref='System.Net.Sockets.Socket.Shutdown'/> method.
    ///    </para>
    /// </devdoc>
    public enum SocketShutdown
    {
        /// <devdoc>
        ///    <para>
        ///       Shutdown sockets for receive.
        ///    </para>
        /// </devdoc>
        Receive = 0x00,
        /// <devdoc>
        ///    <para>
        ///       Shutdown socket for send.
        ///    </para>
        /// </devdoc>
        Send = 0x01,
        /// <devdoc>
        ///    <para>
        ///       Shutdown socket for both send and receive.
        ///    </para>
        /// </devdoc>
        Both = 0x02,
    }; // enum SocketShutdown
} // namespace System.Net.Sockets
