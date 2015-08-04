// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace System.Net.Sockets
{
    internal class SocketExceptionShim
    {
        public static SocketException NewSocketException(int socketError, EndPoint endPoint)
        {
            // TODO: expose SocketException(int, EndPoint) to maintain exception Message compatibility.
            return new SocketException(socketError);
        }

        public static SocketException NewSocketException(EndPoint endPoint)
        {
            // TODO: expose SocketException(EndPoint) to maintain exception Message compatibility.
            return new SocketException();
        }
    }
}
