// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace System.Net.Sockets
{
    /// <devdoc>
    ///    <para>
    ///       Provides constant values for socket messages.
    ///    </para>
    /// </devdoc>
    //UEUE

    [Flags]
    public enum SocketFlags
    {
        /// <devdoc>
        ///    <para>
        ///       Use no flags for this call.
        ///    </para>
        /// </devdoc>
        None = 0x0000,

        /// <devdoc>
        ///    <para>
        ///       Process out-of-band data.
        ///    </para>
        /// </devdoc>
        OutOfBand = 0x0001,

        /// <devdoc>
        ///    <para>
        ///       Peek at incoming message.
        ///    </para>
        /// </devdoc>
        Peek = 0x0002,

        /// <devdoc>
        ///    <para>
        ///       Send without using routing tables.
        ///    </para>
        /// </devdoc>
        DontRoute = 0x0004,

        // see: http://as400bks.rochester.ibm.com/pubs/html/as400/v4r5/ic2978/info/apis/recvms.htm
        MaxIOVectorLength = 0x0010,

        /// <devdoc>
        ///    <para>
        ///       Partial send or recv for message.
        ///    </para>
        /// </devdoc>

        Truncated = 0x0100,
        ControlDataTruncated = 0x0200,
        Broadcast = 0x0400,
        Multicast = 0x0800,


        Partial = 0x8000,
    }; // enum SocketFlags
       /*
       MSG_DONTROUTE
       Specifies that the data should not be subject to routing. A WinSock service 
       provider may choose to ignore this flag;.

       MSG_OOB
       Send out-of-band data (stream style socket such as SOCK_STREAM only).

       MSG_PARTIAL
       Specifies that lpBuffers only contains a partial message. Note 
       that the error code WSAEOPNOTSUPP will be returnedthis flag is ignored by 
       transports which do not support partial message transmissions.

       MSG_INTERRUPT // not supported (Win16)
       Specifies that the function is being called in interrupt context.
       The service provider must not make any Windows systems calls. Note that 
       this is applicable only to Win16 environments and only for protocols that 
       have the XP1_INTERRUPT bit set in the PROTOCOL_INFO struct.
       */


} // namespace System.Net.Sockets
