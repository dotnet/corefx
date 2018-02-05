// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Net.Sockets;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Win32.SafeHandles;
using ProtocolFamily = System.Net.Internals.ProtocolFamily;

namespace System.Net
{
    internal static class NameResolutionPal
    {
        //
        // used by GetHostName() to preallocate a buffer for the call to gethostname.
        //
        private const int HostNameBufferLength = 256;
        private static bool s_initialized;
        private static readonly object s_initializedLock = new object();

        private static readonly unsafe Interop.Winsock.GetAddrInfoExCompletionCallback s_getAddrInfoExCallback = GetAddressInfoExCallback;
        private static bool s_getAddrInfoExSupported;

        public static bool SupportsGetAddrInfoAsync
        {
            get
            {
                EnsureSocketsAreInitialized();
                return s_getAddrInfoExSupported;
            }
        }

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
                if (NetEventSource.IsEnabled) NetEventSource.Info(null, $"HostEntry.HostName: {HostEntry.HostName}");
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

                if (NetEventSource.IsEnabled) NetEventSource.Info(null, $"currentArrayElement:{currentArrayElement} nativePointer:{nativePointer} IPAddressToAdd:{IPAddressToAdd}");

                //
                // ...and add it to the list
                //
                TempIPAddressList.Add(new IPAddress((long)IPAddressToAdd & 0x0FFFFFFFF));

                //
                // now get the next pointer in the array and start over
                //
                currentArrayElement = IntPtrHelper.Add(currentArrayElement, IntPtr.Size);
                nativePointer = Marshal.ReadIntPtr(currentArrayElement);
            }

            HostEntry.AddressList = TempIPAddressList.ToArray();

            //
            // Now do the same thing for the aliases.
            //

            var TempAliasList = new List<string>();

            currentArrayElement = Host.h_aliases;
            nativePointer = Marshal.ReadIntPtr(currentArrayElement);

            while (nativePointer != IntPtr.Zero)
            {
                if (NetEventSource.IsEnabled) NetEventSource.Info(null, $"currentArrayElement:{currentArrayElement} nativePointer:{nativePointer}");

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

            HostEntry.Aliases = TempAliasList.ToArray();

            return HostEntry;
        } // NativeToHostEntry

        public static IPHostEntry GetHostByName(string hostName)
        {
            //
            // IPv6 disabled: use gethostbyname() to obtain DNS information.
            //
            IntPtr nativePointer =
                Interop.Winsock.gethostbyname(
                    hostName);

            if (nativePointer == IntPtr.Zero)
            {
                // Need to do this first since if we wait the last error code might be overwritten.
                SocketException socketException = new SocketException();

                IPAddress address;
                if (IPAddress.TryParse(hostName, out address))
                {
                    IPHostEntry ipHostEntry = NameResolutionUtilities.GetUnresolvedAnswer(address);
                    if (NetEventSource.IsEnabled) NetEventSource.Exit(null, ipHostEntry);
                    return ipHostEntry;
                }

                throw socketException;
            }

            return NativeToHostEntry(nativePointer);
        }

        public static IPHostEntry GetHostByAddr(IPAddress address)
        {
            // TODO #2891: Optimize this (or decide if this legacy code can be removed):
#pragma warning disable CS0618 // Address is marked obsolete
            int addressAsInt = unchecked((int)address.Address);
#pragma warning restore CS0618

#if BIGENDIAN
            // TODO #2891: above code needs testing for BIGENDIAN.

            addressAsInt = (int)(((uint)addressAsInt << 24) | (((uint)addressAsInt & 0x0000FF00) << 8) |
                (((uint)addressAsInt >> 8) & 0x0000FF00) | ((uint)addressAsInt >> 24));
#endif

            IntPtr nativePointer =
                Interop.Winsock.gethostbyaddr(
                    ref addressAsInt,
                    sizeof(int),
                    ProtocolFamily.InterNetwork);
            
            if (nativePointer != IntPtr.Zero)
            {
                return NativeToHostEntry(nativePointer);
            }

            throw new SocketException();
        }

        public static unsafe SocketError TryGetAddrInfo(string name, out IPHostEntry hostinfo, out int nativeErrorCode)
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
            hints.ai_flags = AddressInfoHints.AI_CANONNAME;
            hints.ai_family = AddressFamily.Unspecified;   // gets all address families

            nativeErrorCode = 0;

            //
            // Use try / finally so we always get a shot at freeaddrinfo
            //
            try
            {
                SocketError errorCode = (SocketError)SafeFreeAddrInfo.GetAddrInfo(name, null, ref hints, out root);
                if (errorCode != SocketError.Success)
                { // Should not throw, return mostly blank hostentry
                    hostinfo = NameResolutionUtilities.GetUnresolvedAnswer(name);
                    return errorCode;
                }

                AddressInfo* pAddressInfo = (AddressInfo*)root.DangerousGetHandle();
                //
                // Process the results
                //
                while (pAddressInfo != null)
                {
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
                    if (pAddressInfo->ai_family == AddressFamily.InterNetwork)
                    {
                        addresses.Add(CreateIPv4Address(pAddressInfo->ai_addr, pAddressInfo->ai_addrlen));
                    }
                    else if (pAddressInfo->ai_family == AddressFamily.InterNetworkV6 && SocketProtocolSupportPal.OSSupportsIPv6)
                    {
                        addresses.Add(CreateIPv6Address(pAddressInfo->ai_addr, pAddressInfo->ai_addrlen));
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
            hostinfo.AddressList = addresses.ToArray();

            return SocketError.Success;
        }

        public static string TryGetNameInfo(IPAddress addr, out SocketError errorCode, out int nativeErrorCode)
        {
            //
            // Use SocketException here to show operation not supported
            // if, by some nefarious means, this method is called on an
            // unsupported platform.
            //
            SocketAddress address = (new IPEndPoint(addr, 0)).Serialize();
            StringBuilder hostname = new StringBuilder(1025); // NI_MAXHOST

            int flags = (int)Interop.Winsock.NameInfoFlags.NI_NAMEREQD;

            nativeErrorCode = 0;

            // TODO #2891: Remove the copying step to improve performance. This requires a change in the contracts.
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

        public static string GetHostName()
        {
            //
            // note that we could cache the result ourselves since you
            // wouldn't expect the hostname of the machine to change during
            // execution, but this might still happen and we would want to
            // react to that change.
            //

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

        public static void EnsureSocketsAreInitialized()
        {
            if (!Volatile.Read(ref s_initialized))
            {
                lock (s_initializedLock)
                {
                    if (!s_initialized)
                    {
                        Interop.Winsock.WSAData wsaData = new Interop.Winsock.WSAData();

                        SocketError errorCode =
                            Interop.Winsock.WSAStartup(
                                (short)0x0202, // we need 2.2
                                out wsaData);

                        if (errorCode != SocketError.Success)
                        {
                            //
                            // failed to initialize, throw
                            //
                            // WSAStartup does not set LastWin32Error
                            throw new SocketException((int)errorCode);
                        }

                        s_getAddrInfoExSupported = TestGetAddrInfoEx();

                        Volatile.Write(ref s_initialized, true);
                    }
                }
            }
        }

        private static bool TestGetAddrInfoEx()
        {
            using (SafeLibraryHandle libHandle = Interop.Kernel32.LoadLibraryExW(Interop.Libraries.Ws2_32, IntPtr.Zero, 0))
            {
                return Interop.Kernel32.GetProcAddress(libHandle, nameof(Interop.Winsock.GetAddrInfoExCancel)) != IntPtr.Zero;
            }
        }

        public static unsafe void GetAddrInfoAsync(string name, DnsResolveAsyncResult asyncResult)
        {
            try
            { }
            finally
            {
                GetAddrInfoExContext* context = GetAddrInfoExContext.AllocateContext(name, asyncResult);

                AddressInfoEx hints = new AddressInfoEx();
                hints.ai_flags = AddressInfoHints.AI_CANONNAME;
                hints.ai_family = AddressFamily.Unspecified; // Gets all address families

                SocketError errorCode =
                    (SocketError)Interop.Winsock.GetAddrInfoExW(name, null, 0 /* NS_ALL*/, IntPtr.Zero, ref hints, out context->Result, IntPtr.Zero, ref context->Overlapped, s_getAddrInfoExCallback, out context->CancelHandle);

                if (errorCode != SocketError.IOPending)
                    ProcessResult(errorCode, context);
            }
        }

        private static unsafe void GetAddressInfoExCallback([In] int error, [In] int bytes, [In] NativeOverlapped* overlapped)
        {
            try
            { }
            finally
            {
                // Can be casted directly to QueryContext* because the overlapped is its first field
                GetAddrInfoExContext* context = (GetAddrInfoExContext*)overlapped;

                ProcessResult((SocketError)error, context);
            }
        }

        private static unsafe void ProcessResult(SocketError errorCode, GetAddrInfoExContext* context)
        {
            GetAddrInfoExState state = context->GetQueryState();

            if (state == null || !state.SetCallbackStartedOrCanceled())
                return;

            try
            {
                if (errorCode != SocketError.Success)
                {
                    state.CompleteAsyncResult(new SocketException((int)errorCode));
                    return;
                }

                AddressInfoEx* result = context->Result;
                string canonicalName = null;

                List<IPAddress> addresses = new List<IPAddress>();

                while (result != null)
                {
                    if (canonicalName == null && result->ai_canonname != IntPtr.Zero)
                        canonicalName = Marshal.PtrToStringUni(result->ai_canonname);

                    IPAddress ipAddress = null;

                    if (result->ai_family == AddressFamily.InterNetwork)
                    {
                        ipAddress = CreateIPv4Address(result->ai_addr, result->ai_addrlen);
                    }
                    else if (SocketProtocolSupportPal.OSSupportsIPv6 && result->ai_family == AddressFamily.InterNetworkV6)
                    {
                        ipAddress = CreateIPv6Address(result->ai_addr, result->ai_addrlen);
                    }

                    if (ipAddress != null)
                        addresses.Add(ipAddress);

                    result = result->ai_next;
                }

                if (canonicalName == null)
                    canonicalName = state.HostAddress;

                state.CompleteAsyncResult(new IPHostEntry
                {
                    HostName = canonicalName,
                    Aliases = Array.Empty<string>(),
                    AddressList = addresses.ToArray()
                });
            }
            finally
            {
                state.Dispose();
            }
        }

        private static unsafe IPAddress CreateIPv4Address(byte* socketAddress, int addressLength)
        {
            const int IPv4SocketAddressSize = 16;

            if (addressLength != IPv4SocketAddressSize)
                return null;

            long address = (long)(
                               (socketAddress[4] & 0x000000FF) |
                               (socketAddress[5] << 8 & 0x0000FF00) |
                               (socketAddress[6] << 16 & 0x00FF0000) |
                               (socketAddress[7] << 24)
                           ) & 0x00000000FFFFFFFF;

            return new IPAddress(address);
        }

        private static unsafe IPAddress CreateIPv6Address(byte* socketAddress, int addressLength)
        {
            const int IPv6SocketAddressSize = 28;
            const int IPv6AddressBytes = 16;

            if (addressLength != IPv6SocketAddressSize)
                return null;

            byte[] address = new byte[IPv6AddressBytes];
            for (int i = 0; i < address.Length; i++)
            {
                address[i] = socketAddress[i + 8];
            }

            long scope = (long)((socketAddress[27] << 24) +
                                (socketAddress[26] << 16) +
                                (socketAddress[25] << 8) +
                                (socketAddress[24]));

            return new IPAddress(address, scope);
        }

        #region GetAddrInfoAsync Helper Classes

        private sealed unsafe class GetAddrInfoExState : CriticalFinalizerObject, IDisposable
        {
            private IntPtr m_context;
            private int m_callbackStartedOrCanceled;
            private DnsResolveAsyncResult m_asyncResult;

            public string HostAddress { get; }

            public GetAddrInfoExState(string hostAddress, DnsResolveAsyncResult asyncResult)
            {
                HostAddress = hostAddress;
                m_asyncResult = asyncResult;
            }

            [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
            public void SetQueryContext(IntPtr context)
            {
                m_context = context;
            }

            [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
            public bool SetCallbackStartedOrCanceled()
            {
                return Interlocked.Exchange(ref m_callbackStartedOrCanceled, 1) == 0;
            }

            public void CompleteAsyncResult(object o)
            {
                Task.Run(() => m_asyncResult.InvokeCallback(o));
            }

            [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
            ~GetAddrInfoExState()
            {
                // The finalizer should only be called when the AppDomain is unloading while there was a pending operation.
                // Calling GetAddrInfoExCancel will invoke the callback inline with WSA_E_CANCELLED error code.

                // If the callback is already invoked, let the traditional Dispose clear the resources.
                // This should not be possible, because the finalizer would not have been called if the object was still alive.
                // This is an extra protection in case the object was somehow resurrected.
                if (!SetCallbackStartedOrCanceled())
                    return;

                ReleaseResources(cancelQuery: true);
            }

            [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
            public void Dispose()
            {
                GC.SuppressFinalize(this);
                ReleaseResources(cancelQuery: false);
            }

            [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
            private void ReleaseResources(bool cancelQuery)
            {
                var ptr = Interlocked.Exchange(ref m_context, IntPtr.Zero);

                if (ptr == IntPtr.Zero)
                    return;

                var context = (GetAddrInfoExContext*)ptr;

                if (cancelQuery)
                    context->CancelQuery();

                GetAddrInfoExContext.FreeContext(context);
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        private unsafe struct GetAddrInfoExContext
        {
            private static readonly int Size = Marshal.SizeOf<GetAddrInfoExContext>();

            public NativeOverlapped Overlapped;
            public AddressInfoEx* Result;
            public IntPtr QueryStateHandle;
            public IntPtr CancelHandle;

            public GetAddrInfoExContext(GetAddrInfoExState state)
            {
                Overlapped = new NativeOverlapped();
                Result = null;

                var handle = GCHandle.Alloc(state, GCHandleType.Normal);
                QueryStateHandle = GCHandle.ToIntPtr(handle);

                CancelHandle = IntPtr.Zero;
            }

            public GetAddrInfoExState GetQueryState()
            {
                var stateHandle = Interlocked.Exchange(ref QueryStateHandle, IntPtr.Zero);

                if (stateHandle == IntPtr.Zero)
                    return null;

                var handle = GCHandle.FromIntPtr(stateHandle);

                if (!handle.IsAllocated)
                    return null;

                var state = (GetAddrInfoExState)handle.Target;
                handle.Free();

                return state;
            }

            [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
            public void CancelQuery()
            {
                if (CancelHandle != IntPtr.Zero)
                {
                    Interop.Winsock.GetAddrInfoExCancel(ref CancelHandle);
                    CancelHandle = IntPtr.Zero;
                }
            }

            public static GetAddrInfoExContext* AllocateContext(string hostAddress, DnsResolveAsyncResult asyncResult)
            {
                GetAddrInfoExContext* context = (GetAddrInfoExContext*)Marshal.AllocHGlobal(Size);
                GetAddrInfoExState state = new GetAddrInfoExState(hostAddress, asyncResult);

                *context = new GetAddrInfoExContext(state);
                state.SetQueryContext((IntPtr)context);
                return context;
            }

            [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
            public static void FreeContext(GetAddrInfoExContext* context)
            {
                if (context->Result != null)
                {
                    Interop.Winsock.FreeAddrInfoEx(context->Result);
                    context->Result = null;
                }

                // There is no need to free the GCHandle held in 'QueryStateHandle'
                // The handle is freed by 'GetQueryState' unless the AppDomain is unloaded.

                Marshal.FreeHGlobal((IntPtr)context);
            }
        }

        #endregion
    }
}
