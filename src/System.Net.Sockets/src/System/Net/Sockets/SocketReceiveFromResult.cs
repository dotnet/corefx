// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Net.Sockets
{
    public struct SocketReceiveFromResult
    {
        public int ReceivedBytes;
        public EndPoint RemoteEndPoint;
    }

    // Review note: RemoteEndPoint definition includes the Address and Port.
    // PacketInformation includes Address and Interface (physical interface number).
    // The redundancy could be removed by replacing RemoteEndPoint with Port.

    // Alternative:
    //    public struct SocketReceiveFromResult
    //    {
    //        public int ReceivedBytes;
    //        public IPAddress Address;
    //        public int Port;
    //    }
}
