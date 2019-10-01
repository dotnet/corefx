// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Internals;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace System.Net
{
    internal static partial class NameResolutionPal
    {
        public const bool SupportsGetAddrInfoAsync = false;

        public static void EnsureSocketsAreInitialized() { } // No-op for Unix

        internal static Task GetAddrInfoAsync(string hostName, bool justAddresses) =>
            throw new NotSupportedException();

        private static SocketError GetSocketErrorForNativeError(int error)
        {
            switch (error)
            {
                case 0:
                    return SocketError.Success;
                case (int)Interop.Sys.GetAddrInfoErrorFlags.EAI_AGAIN:
                    return SocketError.TryAgain;
                case (int)Interop.Sys.GetAddrInfoErrorFlags.EAI_BADFLAGS:
                case (int)Interop.Sys.GetAddrInfoErrorFlags.EAI_BADARG:
                    return SocketError.InvalidArgument;
                case (int)Interop.Sys.GetAddrInfoErrorFlags.EAI_FAIL:
                    return SocketError.NoRecovery;
                case (int)Interop.Sys.GetAddrInfoErrorFlags.EAI_FAMILY:
                    return SocketError.AddressFamilyNotSupported;
                case (int)Interop.Sys.GetAddrInfoErrorFlags.EAI_NONAME:
                    return SocketError.HostNotFound;
                default:
                    Debug.Fail("Unexpected error: " + error.ToString());
                    return SocketError.SocketError;
            }
        }

        private static unsafe void ParseHostEntryAndInterfaces(Interop.Sys.HostEntry hostEntry, Interop.Sys.HostInterfaces hostInterfaces, bool justAddresses, out string hostName, out string[] aliases, out IPAddress[] addresses)
        {
            try
            {
                hostName = !justAddresses && hostEntry.CanonicalName != null ?
                    Marshal.PtrToStringAnsi((IntPtr)hostEntry.CanonicalName) :
                    null;


                var hostInterfacesCount = hostInterfaces == null ? 0 : hostInterfaces.IPAddressCount;

                IPAddress[] localAddresses;
                if (hostEntry.IPAddressCount == 0 && hostInterfacesCount == 0)
                {
                    localAddresses = Array.Empty<IPAddress>();
                }
                else
                {
                    // getaddrinfo returns multiple entries per address, for each socket type (datagram, stream, etc.).
                    // Our callers expect just one entry for each address.  So we need to deduplicate the results.
                    // It's important to keep the addresses in order, since they are returned in the order in which
                    // connections should be attempted.
                    //
                    // We assume that the list returned by getaddrinfo is relatively short; after all, the intent is that
                    // the caller may need to attempt to contact every address in the list before giving up on a connection
                    // attempt.  So an O(N^2) algorithm should be fine here.  Keep in mind that any "better" algorithm
                    // is likely to involve extra allocations, hashing, etc., and so will probably be more expensive than
                    // this one in the typical (short list) case.

                    var nativeAddresses = new Interop.Sys.IPAddress[hostEntry.IPAddressCount + hostInterfacesCount];
                    int nativeAddressCount = 0;

                    Interop.Sys.addrinfo* addrInfoHandle = hostEntry.AddressListHandle;
                    for (int i = 0; i < hostEntry.IPAddressCount; i++)
                    {
                        Interop.Sys.IPAddress nativeIPAddress = default;
                        int err = Interop.Sys.GetNextIPAddress(&hostEntry, &addrInfoHandle, &nativeIPAddress);
                        Debug.Assert(err == 0);

                        if (Array.IndexOf(nativeAddresses, nativeIPAddress, 0, nativeAddressCount) == -1)
                        {
                            nativeAddresses[nativeAddressCount++] = nativeIPAddress;
                        }
                    }

                    if (hostInterfaces != null)
                    {
                        Interop.Sys.ifaddrs* ifAddrsHandle = hostInterfaces.AddressListHandle;
                        for (int i = 0; i < hostInterfaces.IPAddressCount; i++)
                        {
                            Interop.Sys.IPAddress nativeIPAddress = default;
                            int err = Interop.Sys.GetNextIPAddress(&hostInterfaces, &ifAddrsHandle, &nativeIPAddress);
                            Debug.Assert(err == 0);

                            if (Array.IndexOf(nativeAddresses, nativeIPAddress, 0, nativeAddressCount) == -1)
                            {
                                nativeAddresses[nativeAddressCount++] = nativeIPAddress;
                            }
                        }
                    }

                    localAddresses = new IPAddress[nativeAddressCount];
                    for (int i = 0; i < nativeAddressCount; i++)
                    {
                        localAddresses[i] = nativeAddresses[i].GetIPAddress();
                    }
                }

                string[] localAliases = Array.Empty<string>();
                if (!justAddresses && hostEntry.Aliases != null)
                {
                    int numAliases = 0;
                    while (hostEntry.Aliases[numAliases] != null)
                    {
                        numAliases++;
                    }

                    if (numAliases > 0)
                    {
                        localAliases = new string[numAliases];
                        for (int i = 0; i < localAliases.Length; i++)
                        {
                            localAliases[i] = Marshal.PtrToStringAnsi((IntPtr)hostEntry.Aliases[i]);
                        }
                    }
                }

                aliases = localAliases;
                addresses = localAddresses;
            }
            finally
            {
                Interop.Sys.FreeHostEntry(&hostEntry);
                if (hostInterfaces != null)
                {
                    Interop.Sys.FreeHostInterfaces(&hostInterfaces);
                }
            }
        }

        public static unsafe SocketError TryGetAddrInfo(string name, bool justAddresses, out string hostName, out string[] aliases, out IPAddress[] addresses, out int nativeErrorCode)
        {
            string localHostName = Dns.GetHostName();
            if (name == "")
            {
                // To match documented behavior on Windows, if an empty string is passed in, use the local host's name.
                name = localHostName;
            }

            Interop.Sys.HostEntry entry;
            int result = Interop.Sys.GetHostEntryForName(name, &entry);
            if (result != 0)
            {
                nativeErrorCode = result;
                hostName = name;
                aliases = Array.Empty<string>();
                addresses = Array.Empty<IPAddress>();
                return GetSocketErrorForNativeError(result);
            }

            Interop.Sys.HostInterfaces interfaces = null;
            if (name == localHostName)
            {
                int result = Interop.Sys.GetHostInterfaces(&interfaces);
                if (result != 0)
                {
                    nativeErrorCode = result;
                    hostName = name;
                    aliases = Array.Empty<string>();
                    addresses = Array.Empty<IPAddress>();
                    return GetSocketErrorForNativeError(result);
                }
            }

            ParseHostEntryAndInterfaces(entry, justAddresses, out hostName, out aliases, ref addresses);
            nativeErrorCode = 0;
            return SocketError.Success;
        }

        public static unsafe string TryGetNameInfo(IPAddress addr, out SocketError socketError, out int nativeErrorCode)
        {
            byte* buffer = stackalloc byte[Interop.Sys.NI_MAXHOST + 1 /*for null*/];

            byte isIPv6;
            int rawAddressLength;
            if (addr.AddressFamily == AddressFamily.InterNetwork)
            {
                isIPv6 = 0;
                rawAddressLength = IPAddressParserStatics.IPv4AddressBytes;
            }
            else
            {
                isIPv6 = 1;
                rawAddressLength = IPAddressParserStatics.IPv6AddressBytes;
            }

            byte* rawAddress = stackalloc byte[rawAddressLength];
            addr.TryWriteBytes(new Span<byte>(rawAddress, rawAddressLength), out int bytesWritten);
            Debug.Assert(bytesWritten == rawAddressLength);

            int error = Interop.Sys.GetNameInfo(
                rawAddress,
                (uint)rawAddressLength,
                isIPv6,
                buffer,
                Interop.Sys.NI_MAXHOST,
                null,
                0,
                Interop.Sys.GetNameInfoFlags.NI_NAMEREQD);

            socketError = GetSocketErrorForNativeError(error);
            nativeErrorCode = error;
            return socketError == SocketError.Success ? Marshal.PtrToStringAnsi((IntPtr)buffer) : null;
        }

        public static string GetHostName() => Interop.Sys.GetHostName();
    }
}
