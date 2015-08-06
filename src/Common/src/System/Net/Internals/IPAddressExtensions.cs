// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace System.Net.Sockets
{
    public static class IPAddressExtensions
    {
        // TODO: Extract the following into IPAddressExtensions and reconcile with IPAddress.cs in Primitives:
        public static IPAddress Snapshot(this IPAddress original)
        {
            switch (original.AddressFamily)
            {
                case AddressFamily.InterNetwork:
                return new IPAddress(original.GetAddressBytes());

                case AddressFamily.InterNetworkV6:
                return new IPAddress(original.GetAddressBytes(), (uint)original.ScopeId);
            }

            throw new InternalException();
        }

        // TODO: Reuse in DNS.cs:239.
        public static long GetAddress(this IPAddress thisObj)
        {
            byte[] addressBytes = thisObj.GetAddressBytes();
            return BitConverter.ToInt64(addressBytes, 0);
        }
    }
}
