// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace System.Net.Sockets
{
    internal static class DnsAPMExtensions
    {
        public static IAsyncResult BeginGetHostAddresses(string hostNameOrAddress, AsyncCallback requestCallback, object state)
        {
            throw new NotImplementedException();
        }

        public static IPAddress[] EndGetHostAddresses(IAsyncResult asyncResult)
        {
            throw new NotImplementedException();
        }
    }
}
