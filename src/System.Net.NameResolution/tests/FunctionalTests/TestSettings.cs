// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace System.Net.NameResolution.Tests
{
    internal static class TestSettings
    {
        public const string LocalHost = "localhost";

        public static Task<IPAddress> GetLocalIPAddress()
        {
            return ResolveHost(TestSettings.LocalHost, TestSettings.AddressFamily);
        }

        public static AddressFamily AddressFamily
        {
            get
            {
                // *nix machines are not always configured to resolve localhost to an IPv6 address.
                return RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ?
                    AddressFamily.InterNetworkV6 :
                    AddressFamily.InterNetwork;
            }
        }

        private static async Task<IPAddress> ResolveHost(string host, AddressFamily family)
        {
            var hostEntry = await Dns.GetHostEntryAsync(host);

            foreach (var address in hostEntry.AddressList)
            {
                if (address.AddressFamily == family)
                {
                    return address;
                }
            }
            return null;
        }
    }
}

