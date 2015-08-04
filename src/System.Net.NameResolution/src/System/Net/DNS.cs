// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Text;
using System.Collections;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Globalization;
using System.Collections.Generic;

namespace System.Net
{
    /// <devdoc>
    ///    <para>Provides simple
    ///       domain name resolution functionality.</para>
    /// </devdoc>

    public static class Dns
    {
        //
        // used by GetHostName() to preallocate a buffer for the call to gethostname.
        //
        private const int HostNameBufferLength = 256;

        // Host names any longer than this automatically fail at the winsock level.
        // If the host name is 255 chars, the last char must be a dot.
        private const int MaxHostName = 255;

        /*++

        Routine Description:

            Takes a native pointer (expressed as an int) to a hostent structure,
            and converts the information in their to an IPHostEntry class. This
            involves walking through an array of native pointers, and a temporary
            ArrayList object is used in doing this.

        Arguments:

            nativePointer   - Native pointer to hostent structure.



        Return Value:

            An IPHostEntry structure.

        --*/
        private static IPHostEntry NativeToHostEntry(IntPtr nativePointer)
        {
            //
            // marshal pointer to struct
            //

            hostent Host = Marshal.PtrToStructure<hostent>(nativePointer);
            IPHostEntry HostEntry = new IPHostEntry();

            if (Host.h_name != IntPtr.Zero)
            {
                HostEntry.HostName = Marshal.PtrToStringAnsi(Host.h_name);
                GlobalLog.Print("HostEntry.HostName: " + HostEntry.HostName);
            }

            // decode h_addr_list to ArrayList of IP addresses.
            // The h_addr_list field is really a pointer to an array of pointers
            // to IP addresses. Loop through the array, and while the pointer
            // isn't NULL read the IP address, convert it to an IPAddress class,
            // and add it to the list.

            var TempIPAddressList = new List<IPAddress>();
            int IPAddressToAdd;
            string AliasToAdd;
            IntPtr currentArrayElement;

            //
            // get the first pointer in the array
            //
            currentArrayElement = Host.h_addr_list;
            nativePointer = Marshal.ReadIntPtr(currentArrayElement);

            while (nativePointer != IntPtr.Zero)
            {
                //
                // if it's not null it points to an IPAddress,
                // read it...
                //
                IPAddressToAdd = Marshal.ReadInt32(nativePointer);
#if BIGENDIAN
                // IP addresses from native code are always a byte array
                // converted to int.  We need to convert the address into
                // a uniform integer value.
                IPAddressToAdd = (int)(((uint)IPAddressToAdd << 24) |
                                        (((uint)IPAddressToAdd & 0x0000FF00) << 8) |
                                        (((uint)IPAddressToAdd >> 8) & 0x0000FF00) |
                                        ((uint)IPAddressToAdd >> 24));
#endif

                GlobalLog.Print("currentArrayElement: " + currentArrayElement.ToString() + " nativePointer: " + nativePointer.ToString() + " IPAddressToAdd:" + IPAddressToAdd.ToString());

                //
                // ...and add it to the list
                //
                TempIPAddressList.Add(new IPAddress(IPAddressToAdd));

                //
                // now get the next pointer in the array and start over
                //
                currentArrayElement = IntPtrHelper.Add(currentArrayElement, IntPtr.Size);
                nativePointer = Marshal.ReadIntPtr(currentArrayElement);
            }

            HostEntry.AddressList = new IPAddress[TempIPAddressList.Count];
            TempIPAddressList.CopyTo(HostEntry.AddressList, 0);

            //
            // Now do the same thing for the aliases.
            //

            var TempAliasList = new List<string>();

            currentArrayElement = Host.h_aliases;
            nativePointer = Marshal.ReadIntPtr(currentArrayElement);

            while (nativePointer != IntPtr.Zero)
            {
                GlobalLog.Print("currentArrayElement: " + ((long)currentArrayElement).ToString() + "nativePointer: " + ((long)nativePointer).ToString());

                //
                // if it's not null it points to an Alias,
                // read it...
                //
                AliasToAdd = Marshal.PtrToStringAnsi(nativePointer);

                //
                // ...and add it to the list
                //
                TempAliasList.Add(AliasToAdd);

                //
                // now get the next pointer in the array and start over
                //
                currentArrayElement = IntPtrHelper.Add(currentArrayElement, IntPtr.Size);
                nativePointer = Marshal.ReadIntPtr(currentArrayElement);
            }

            HostEntry.Aliases = new string[TempAliasList.Count];
            TempAliasList.CopyTo(HostEntry.Aliases, 0);

            return HostEntry;
        } // NativeToHostEntry

        internal static IPHostEntry InternalGetHostByName(string hostName, bool includeIPv6)
        {
            if (Logging.On) Logging.Enter(Logging.Sockets, "DNS", "GetHostByName", hostName);
            IPHostEntry ipHostEntry = null;

            GlobalLog.Print("Dns.GetHostByName: " + hostName);

            if (hostName.Length > MaxHostName // If 255 chars, the last one must be a dot.
                || hostName.Length == MaxHostName && hostName[MaxHostName - 1] != '.')
            {
                throw new ArgumentOutOfRangeException("hostName", SR.Format(SR.net_toolong,
                    "hostName", MaxHostName.ToString(NumberFormatInfo.CurrentInfo)));
            }

            //
            // IPv6 Changes: IPv6 requires the use of getaddrinfo() rather
            //               than the traditional IPv4 gethostbyaddr() / gethostbyname().
            //               getaddrinfo() is also protocol independant in that it will also
            //               resolve IPv4 names / addresses. As a result, it is the preferred
            //               resolution mechanism on platforms that support it (Windows 5.1+).
            //               If getaddrinfo() is unsupported, IPv6 resolution does not work.
            //
            // Consider    : If IPv6 is disabled, we could detect IPv6 addresses
            //               and throw an unsupported platform exception.
            //
            // Note        : Whilst getaddrinfo is available on WinXP+, we only
            //               use it if IPv6 is enabled (platform is part of that
            //               decision). This is done to minimize the number of
            //               possible tests that are needed.
            //
            if (Socket.OSSupportsIPv6|| includeIPv6)
            {
                //
                // IPv6 enabled: use getaddrinfo() to obtain DNS information.
                //
                ipHostEntry = Dns.GetAddrInfo(hostName);
            }
            else
            {
                //
                // IPv6 disabled: use gethostbyname() to obtain DNS information.
                //
                IntPtr nativePointer =
                    Interop.Winsock.gethostbyname(
                        hostName);

                if (nativePointer == IntPtr.Zero)
                {
                    // This is for compatiblity with NT4/Win2k
                    // Need to do this first since if we wait the last error code might be overwritten.
                    SocketException socketException = new SocketException();

                    //This block supresses "unknown error" on NT4 when input is
                    //arbitrary IP address. It simulates same result as on Win2K.
                    // For Everett compat, we allow this to parse and return IPv6 even when it's disabled.
                    IPAddress address;
                    if (IPAddress.TryParse(hostName, out address))
                    {
                        ipHostEntry = GetUnresolveAnswer(address);
                        if (Logging.On) Logging.Exit(Logging.Sockets, "DNS", "GetHostByName", ipHostEntry);
                        return ipHostEntry;
                    }

                    throw socketException;
                }
                ipHostEntry = NativeToHostEntry(nativePointer);
            }

            if (Logging.On) Logging.Exit(Logging.Sockets, "DNS", "GetHostByName", ipHostEntry);
            return ipHostEntry;
        } // GetHostByName

        // Does internal IPAddress reverse and then forward lookups (for Legacy and current public methods).
        // Legacy methods do not include IPv6, unless configed to by Socket.LegacySupportsIPv6 and Socket.OSSupportsIPv6.
        internal static IPHostEntry InternalGetHostByAddress(IPAddress address, bool includeIPv6)
        {
            GlobalLog.Print("Dns.InternalGetHostByAddress: " + address.ToString());
            //
            // IPv6 Changes: We need to use the new getnameinfo / getaddrinfo functions
            //               for resolution of IPv6 addresses.
            //

            SocketError errorCode = SocketError.Success;
            Exception exception = null;
            if (Socket.OSSupportsIPv6 || includeIPv6)
            {
                //
                // Try to get the data for the host from it's address
                //
                // We need to call getnameinfo first, because getaddrinfo w/ the ipaddress string
                // will only return that address and not the full list.

                // Do a reverse lookup to get the host name.
                string name = TryGetNameInfo(address, out errorCode);
                if (errorCode == SocketError.Success)
                {
                    // Do the forward lookup to get the IPs for that host name
                    IPHostEntry hostEntry;
                    errorCode = TryGetAddrInfo(name, out hostEntry);
                    if (errorCode == SocketError.Success)
                        return hostEntry;

                    // Log failure
                    if (Logging.On)
                        Logging.Exception(Logging.Sockets, "DNS",
            "InternalGetHostByAddress", new SocketException((int)errorCode));

                    // One of two things happened:
                    // 1. There was a ptr record in dns, but not a corollary A/AAA record.
                    // 2. The IP was a local (non-loopback) IP that resolved to a connection specific dns suffix.
                    //    - Workaround, Check "Use this connection's dns suffix in dns registration" on that network
                    //      adapter's advanced dns settings.

                    // Just return the resolved host name and no IPs.
                    return hostEntry;
                }
                exception = new SocketException((int)errorCode);
            }

            //
            // If IPv6 is not enabled (maybe config switch) but we've been
            // given an IPv6 address then we need to bail out now.
            //
            else
            {
                if (address.AddressFamily == AddressFamily.InterNetworkV6)
                {
                    //
                    // Protocol not supported
                    //
                    throw new SocketException((int)SocketError.ProtocolNotSupported);
                }
                //
                // Use gethostbyaddr() to try to resolve the IP address
                //
                // End IPv6 Changes
                //

                // TODO: Optimize this (or decide if this legacy code can be removed):
                byte [] addressBytes = address.GetAddressBytes();
                int addressAsInt = BitConverter.ToInt32(addressBytes, 0);

#if BIGENDIAN
                // TODO: above code needs testing for BIGENDIAN.

                addressAsInt = (int)(((uint)addressAsInt << 24) | (((uint)addressAsInt & 0x0000FF00) << 8) |
                    (((uint)addressAsInt >> 8) & 0x0000FF00) | ((uint)addressAsInt >> 24));
#endif

                IntPtr nativePointer =
                    Interop.Winsock.gethostbyaddr(
                        ref addressAsInt,
                        Marshal.SizeOf<int>(),
                        ProtocolFamily.InterNetwork);


                if (nativePointer != IntPtr.Zero)
                {
                    return NativeToHostEntry(nativePointer);
                }
                exception = new SocketException();
            }

            if (Logging.On) Logging.Exception(Logging.Sockets, "DNS", "InternalGetHostByAddress", exception);
            throw exception;
        } // InternalGetHostByAddress

        /*****************************************************************************
         Function :    gethostname

         Abstract:     Queries the hostname from DNS

         Input Parameters:

         Returns: String
        ******************************************************************************/

        /// <devdoc>
        ///    <para>Gets the host name of the local machine.</para>
        /// </devdoc>
        public static string GetHostName()
        {
            GlobalLog.Print("Dns.GetHostName");

            //
            // note that we could cache the result ourselves since you
            // wouldn't expect the hostname of the machine to change during
            // execution, but this might still happen and we would want to
            // react to that change.
            //

            Socket.InitializeSockets();
            StringBuilder sb = new StringBuilder(HostNameBufferLength);
            SocketError errorCode =
                Interop.Winsock.gethostname(
                    sb,
                    HostNameBufferLength);

            //
            // if the call failed throw a SocketException()
            //
            if (errorCode != SocketError.Success)
            {
                throw new SocketException();
            }
            return sb.ToString();
        }

        private static IPHostEntry GetUnresolveAnswer(IPAddress address)
        {
            IPHostEntry ipHostEntry = new IPHostEntry();
            ipHostEntry.HostName = address.ToString();
            ipHostEntry.Aliases = Array.Empty<string>();
            ipHostEntry.AddressList = new IPAddress[] { address };
            return ipHostEntry;
        }

        private class ResolveAsyncResult : ContextAwareResult
        {
            // Forward lookup
            internal ResolveAsyncResult(string hostName, object myObject, bool includeIPv6, object myState, AsyncCallback myCallBack) :
                base(myObject, myState, myCallBack)
            {
                this.hostName = hostName;
                this.includeIPv6 = includeIPv6;
            }

            // Reverse lookup
            internal ResolveAsyncResult(IPAddress address, object myObject, bool includeIPv6, object myState, AsyncCallback myCallBack) :
                base(myObject, myState, myCallBack)
            {
                this.includeIPv6 = includeIPv6;
                this.address = address;
            }

            internal readonly string hostName;
            internal bool includeIPv6;
            internal IPAddress address;
        }

        private static void ResolveCallback(object context)
        {
            ResolveAsyncResult result = (ResolveAsyncResult)context;
            IPHostEntry hostEntry;
            try
            {
                if (result.address != null)
                {
                    hostEntry = InternalGetHostByAddress(result.address, result.includeIPv6);
                }
                else
                {
                    hostEntry = InternalGetHostByName(result.hostName, result.includeIPv6);
                }
            }
            catch (Exception exception)
            {
                if (exception is OutOfMemoryException)
                    throw;

                result.InvokeCallback(exception);
                return;
            }

            result.InvokeCallback(hostEntry);
        }

        // Helpers for async GetHostByName, ResolveToAddresses, and Resolve - they're almost identical
        // If hostName is an IPString and justReturnParsedIP==true then no reverse lookup will be attempted, but the orriginal address is returned.
        private static IAsyncResult HostResolutionBeginHelper(string hostName, bool justReturnParsedIp, bool flowContext, bool includeIPv6, bool throwOnIPAny, AsyncCallback requestCallback, object state)
        {
            if (hostName == null)
            {
                throw new ArgumentNullException("hostName");
            }

            GlobalLog.Print("Dns.HostResolutionBeginHelper: " + hostName);

            // See if it's an IP Address.
            IPAddress address;
            ResolveAsyncResult asyncResult;
            if (IPAddress.TryParse(hostName, out address))
            {
                if (throwOnIPAny && (address.Equals(IPAddress.Any) || address.Equals(IPAddress.IPv6Any)))
                {
                    throw new ArgumentException(SR.net_invalid_ip_addr, "hostNameOrAddress");
                }

                asyncResult = new ResolveAsyncResult(address, null, includeIPv6, state, requestCallback);

                if (justReturnParsedIp)
                {
                    IPHostEntry hostEntry = GetUnresolveAnswer(address);
                    asyncResult.StartPostingAsyncOp(false);
                    asyncResult.InvokeCallback(hostEntry);
                    asyncResult.FinishPostingAsyncOp();
                    return asyncResult;
                }
            }
            else
            {
                asyncResult = new ResolveAsyncResult(hostName, null, includeIPv6, state, requestCallback);
            }

            // Set up the context, possibly flow.
            if (flowContext)
            {
                asyncResult.StartPostingAsyncOp(false);
            }

            // Start the resolve.
            Task.Factory.StartNew(ResolveCallback, asyncResult);

            // Finish the flowing, maybe it completed?  This does nothing if we didn't initiate the flowing above.
            asyncResult.FinishPostingAsyncOp();
            return asyncResult;
        }

        private static IAsyncResult HostResolutionBeginHelper(IPAddress address, bool flowContext, bool includeIPv6, AsyncCallback requestCallback, object state)
        {
            if (address == null)
            {
                throw new ArgumentNullException("address");
            }

            if (address.Equals(IPAddress.Any) || address.Equals(IPAddress.IPv6Any))
            {
                throw new ArgumentException(SR.net_invalid_ip_addr, "address");
            }

            GlobalLog.Print("Dns.HostResolutionBeginHelper: " + address);

            // Set up the context, possibly flow.
            ResolveAsyncResult asyncResult = new ResolveAsyncResult(address, null, includeIPv6, state, requestCallback);
            if (flowContext)
            {
                asyncResult.StartPostingAsyncOp(false);
            }

            // Start the resolve.
            Task.Factory.StartNew(ResolveCallback, asyncResult);

            // Finish the flowing, maybe it completed?  This does nothing if we didn't initiate the flowing above.
            asyncResult.FinishPostingAsyncOp();
            return asyncResult;
        }

        private static IPHostEntry HostResolutionEndHelper(IAsyncResult asyncResult)
        {
            //
            // parameter validation
            //
            if (asyncResult == null)
            {
                throw new ArgumentNullException("asyncResult");
            }
            ResolveAsyncResult castedResult = asyncResult as ResolveAsyncResult;
            if (castedResult == null)
            {
                throw new ArgumentException(SR.net_io_invalidasyncresult, "asyncResult");
            }
            if (castedResult.EndCalled)
            {
                throw new InvalidOperationException(SR.Format(SR.net_io_invalidendcall, "EndResolve"));
            }

            GlobalLog.Print("Dns.HostResolutionEndHelper");

            castedResult.InternalWaitForCompletion();
            castedResult.EndCalled = true;

            Exception exception = castedResult.Result as Exception;
            if (exception != null)
            {
                throw exception;
            }

            return (IPHostEntry)castedResult.Result;
        }

        // TODO: Used by Ping, Socket and TCPClient
        public static IPAddress[] GetHostAddresses(string hostNameOrAddress)
        {
            if (Logging.On) Logging.Enter(Logging.Sockets, "DNS", "GetHostAddresses", hostNameOrAddress);

            if (hostNameOrAddress == null)
            {
                throw new ArgumentNullException("hostNameOrAddress");
            }

            // See if it's an IP Address.
            IPAddress address;
            IPAddress[] addresses;
            if (IPAddress.TryParse(hostNameOrAddress, out address))
            {
                if (address.Equals(IPAddress.Any) || address.Equals(IPAddress.IPv6Any))
                {
                    throw new ArgumentException(SR.net_invalid_ip_addr, "hostNameOrAddress");
                }
                addresses = new IPAddress[] { address };
            }
            else
            {
                // InternalGetHostByName works with IP addresses (and avoids a reverse-lookup), but we need
                // explicit handling in order to do the ArgumentException and guarantee the behavior.
                addresses = InternalGetHostByName(hostNameOrAddress, true).AddressList;
            }

            if (Logging.On) Logging.Exit(Logging.Sockets, "DNS", "GetHostAddresses", addresses);
            return addresses;
        }

        public static IAsyncResult BeginGetHostEntry(string hostNameOrAddress, AsyncCallback requestCallback, object stateObject)
        {
            if (Logging.On) Logging.Enter(Logging.Sockets, "DNS", "BeginGetHostEntry", hostNameOrAddress);

            IAsyncResult asyncResult = HostResolutionBeginHelper(hostNameOrAddress, false, true, true, true, requestCallback, stateObject);

            if (Logging.On) Logging.Exit(Logging.Sockets, "DNS", "BeginGetHostEntry", asyncResult);
            return asyncResult;
        } // BeginResolve

        public static IAsyncResult BeginGetHostEntry(IPAddress address, AsyncCallback requestCallback, object stateObject)
        {
            if (Logging.On) Logging.Enter(Logging.Sockets, "DNS", "BeginGetHostEntry", address);

            IAsyncResult asyncResult = HostResolutionBeginHelper(address, true, true, requestCallback, stateObject);

            if (Logging.On) Logging.Exit(Logging.Sockets, "DNS", "BeginGetHostEntry", asyncResult);
            return asyncResult;
        } // BeginResolve

        public static IPHostEntry EndGetHostEntry(IAsyncResult asyncResult)
        {
            if (Logging.On) Logging.Enter(Logging.Sockets, "DNS", "EndGetHostEntry", asyncResult);

            IPHostEntry ipHostEntry = HostResolutionEndHelper(asyncResult);

            if (Logging.On) Logging.Exit(Logging.Sockets, "DNS", "EndGetHostEntry", ipHostEntry);
            return ipHostEntry;
        } // EndResolve()

        public static IAsyncResult BeginGetHostAddresses(string hostNameOrAddress, AsyncCallback requestCallback, object state)
        {
            if (Logging.On) Logging.Enter(Logging.Sockets, "DNS", "BeginGetHostAddresses", hostNameOrAddress);

            IAsyncResult asyncResult = HostResolutionBeginHelper(hostNameOrAddress, true, true, true, true, requestCallback, state);

            if (Logging.On) Logging.Exit(Logging.Sockets, "DNS", "BeginGetHostAddresses", asyncResult);
            return asyncResult;
        } // BeginResolve

        public static IPAddress[] EndGetHostAddresses(IAsyncResult asyncResult)
        {
            if (Logging.On) Logging.Enter(Logging.Sockets, "DNS", "EndGetHostAddresses", asyncResult);

            IPHostEntry ipHostEntry = HostResolutionEndHelper(asyncResult);

            if (Logging.On) Logging.Exit(Logging.Sockets, "DNS", "EndGetHostAddresses", ipHostEntry);
            return ipHostEntry.AddressList;
        } // EndResolveToAddresses

        // TODO: Used by Socket.BeginConnect
        internal static IAsyncResult UnsafeBeginGetHostAddresses(string hostName, AsyncCallback requestCallback, object state)
        {
            if (Logging.On) Logging.Enter(Logging.Sockets, "DNS", "UnsafeBeginGetHostAddresses", hostName);

            IAsyncResult asyncResult = HostResolutionBeginHelper(hostName, true, false, true, true, requestCallback, state);

            if (Logging.On) Logging.Exit(Logging.Sockets, "DNS", "UnsafeBeginGetHostAddresses", asyncResult);
            return asyncResult;
        } // UnsafeBeginResolveToAddresses

        //************* Task-based async public methods *************************
        public static Task<IPAddress[]> GetHostAddressesAsync(string hostNameOrAddress)
        {
            return Task<IPAddress[]>.Factory.FromAsync(BeginGetHostAddresses, EndGetHostAddresses, hostNameOrAddress, null);
        }

        public static Task<IPHostEntry> GetHostEntryAsync(IPAddress address)
        {
            return Task<IPHostEntry>.Factory.FromAsync(BeginGetHostEntry, EndGetHostEntry, address, null);
        }

        public static Task<IPHostEntry> GetHostEntryAsync(string hostNameOrAddress)
        {
            return Task<IPHostEntry>.Factory.FromAsync(BeginGetHostEntry, EndGetHostEntry, hostNameOrAddress, null);
        }

        private unsafe static IPHostEntry GetAddrInfo(string name)
        {
            IPHostEntry hostEntry;
            SocketError errorCode = TryGetAddrInfo(name, out hostEntry);
            if (errorCode != SocketError.Success)
            {
                throw new SocketException((int)errorCode);
            }
            return hostEntry;
        }

        //
        // IPv6 Changes: Add getaddrinfo and getnameinfo methods.
        //
        private unsafe static SocketError TryGetAddrInfo(string name, out IPHostEntry hostinfo)
        {
            // gets the resolved name
            return TryGetAddrInfo(name, AddressInfoHints.AI_CANONNAME, out hostinfo);
        }

        private unsafe static SocketError TryGetAddrInfo(string name, AddressInfoHints flags, out IPHostEntry hostinfo)
        {
            //
            // Use SocketException here to show operation not supported
            // if, by some nefarious means, this method is called on an
            // unsupported platform.
            //
            SafeFreeAddrInfo root = null;
            var addresses = new List<IPAddress>();
            string canonicalname = null;

            AddressInfo hints = new AddressInfo();
            hints.ai_flags = flags;
            hints.ai_family = AddressFamily.Unspecified;   // gets all address families
            //
            // Use try / finally so we always get a shot at freeaddrinfo
            //
            try
            {
                SocketError errorCode = (SocketError)SafeFreeAddrInfo.GetAddrInfo(name, null, ref hints, out root);
                if (errorCode != SocketError.Success)
                { // Should not throw, return mostly blank hostentry
                    hostinfo = new IPHostEntry();
                    hostinfo.HostName = name;
                    hostinfo.Aliases = Array.Empty<string>();
                    hostinfo.AddressList = Array.Empty<IPAddress>();
                    return errorCode;
                }

                AddressInfo* pAddressInfo = (AddressInfo*)root.DangerousGetHandle();
                //
                // Process the results
                //
                while (pAddressInfo != null)
                {
                    SocketAddress sockaddr;
                    //
                    // Retrieve the canonical name for the host - only appears in the first AddressInfo
                    // entry in the returned array.
                    //
                    if (canonicalname == null && pAddressInfo->ai_canonname != null)
                    {
                        canonicalname = Marshal.PtrToStringUni((IntPtr)pAddressInfo->ai_canonname);
                    }
                    //
                    // Only process IPv4 or IPv6 Addresses. Note that it's unlikely that we'll
                    // ever get any other address families, but better to be safe than sorry.
                    // We also filter based on whether IPv6 is supported on the current
                    // platform / machine.
                    //
                    if ((pAddressInfo->ai_family == AddressFamily.InterNetwork) || // Never filter v4
                        (pAddressInfo->ai_family == AddressFamily.InterNetworkV6 && Socket.OSSupportsIPv6))

                    {
                        sockaddr = new SocketAddress(pAddressInfo->ai_family, pAddressInfo->ai_addrlen);
                        //
                        // Push address data into the socket address buffer
                        //
                        for (int d = 0; d < pAddressInfo->ai_addrlen; d++)
                        {
                            sockaddr[d] = *(pAddressInfo->ai_addr + d);
                        }
                        //
                        // NOTE: We need an IPAddress now, the only way to create it from a
                        //       SocketAddress is via IPEndPoint. This ought to be simpler.
                        //
                        if (pAddressInfo->ai_family == AddressFamily.InterNetwork)
                        {
                            addresses.Add(((IPEndPoint)IPEndPointStatics.Any.Create(sockaddr)).Address);
                        }
                        else
                        {
                            addresses.Add(((IPEndPoint)IPEndPointStatics.IPv6Any.Create(sockaddr)).Address);
                        }
                    }
                    //
                    // Next addressinfo entry
                    //
                    pAddressInfo = pAddressInfo->ai_next;
                }
            }
            finally
            {
                if (root != null)
                {
                    root.Dispose();
                }
            }

            //
            // Finally, put together the IPHostEntry
            //
            hostinfo = new IPHostEntry();

            hostinfo.HostName = canonicalname != null ? canonicalname : name;
            hostinfo.Aliases = Array.Empty<string>();
            hostinfo.AddressList = new IPAddress[addresses.Count];
            addresses.CopyTo(hostinfo.AddressList);

            return SocketError.Success;
        }

        internal static string TryGetNameInfo(IPAddress addr, out SocketError errorCode)
        {
            //
            // Use SocketException here to show operation not supported
            // if, by some nefarious means, this method is called on an
            // unsupported platform.
            //
            SocketAddress address = (new IPEndPoint(addr, 0)).Serialize();
            StringBuilder hostname = new StringBuilder(1025); // NI_MAXHOST

            int flags = (int)NameInfoFlags.NI_NAMEREQD;

            Socket.InitializeSockets();

            // TODO: Remove the copying step to improve performance. This requires a change in the contracts.
            byte[] addressBuffer = new byte[address.Size];
            for (int i = 0; i < address.Size; i++)
            {
                addressBuffer[i] = address[i];
            }
            
            errorCode =
                Interop.Winsock.GetNameInfoW(
                    addressBuffer,
                    address.Size,
                    hostname,
                    hostname.Capacity,
                    null, // We don't want a service name
                    0, // so no need for buffer or length
                    flags);

            if (errorCode != SocketError.Success)
            {
                return null;
            }

            return hostname.ToString();
        }
    }
}
