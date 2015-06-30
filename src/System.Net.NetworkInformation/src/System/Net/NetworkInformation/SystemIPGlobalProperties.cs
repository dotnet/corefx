
using System.Net;
using System.Net.Sockets;
using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Collections.Generic;
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace System.Net.NetworkInformation
{
    internal class SystemIPGlobalProperties : IPGlobalProperties
    {
        internal SystemIPGlobalProperties()
        {
        }

        internal FIXED_INFO FixedInfo
        {
            get
            {
                return HostInformation.GetFixedInfo();
            }
        }

        /// <summary>Specifies the host name for the local computer.</summary>
        public override string HostName
        {
            get
            {
                return FixedInfo.hostName;
            }
        }
        /// <summary>Specifies the domain in which the local computer is registered.</summary>
        public override string DomainName
        {
            get
            {
                return FixedInfo.domainName;
            }
        }
        /// <summary>
        /// The type of node.
        /// </summary>
        /// <remarks>
        /// The exact mechanism by which NetBIOS names are resolved to IP addresses
        /// depends on the node's configured NetBIOS Node Type. Broadcast - uses broadcast
        /// NetBIOS Name Queries for name registration and resolution.
        /// PeerToPeer - uses a NetBIOS name server (NBNS), such as Windows Internet
        /// Name Service (WINS), to resolve NetBIOS names.
        /// Mixed - uses Broadcast then PeerToPeer.
        /// Hybrid - uses PeerToPeer then Broadcast.
        /// </remarks>
        public override NetBiosNodeType NodeType
        {
            get
            {
                return (NetBiosNodeType)FixedInfo.nodeType;
            }
        }
        /// <summary>Specifies the DHCP scope name.</summary>
        public override string DhcpScopeName
        {
            get
            {
                return FixedInfo.scopeId;
            }
        }
        /// <summary>Specifies whether the local computer is acting as an WINS proxy.</summary>
        public override bool IsWinsProxy
        {
            get
            {
                return (FixedInfo.enableProxy);
            }
        }


        public override TcpConnectionInformation[] GetActiveTcpConnections()
        {
            List<TcpConnectionInformation> list = new List<TcpConnectionInformation>();
            List<SystemTcpConnectionInformation> connections = GetAllTcpConnections();
            foreach (TcpConnectionInformation connection in connections)
            {
                if (connection.State != TcpState.Listen)
                {
                    list.Add(connection);
                }
            }
            return list.ToArray();
        }


        public override IPEndPoint[] GetActiveTcpListeners()
        {
            List<IPEndPoint> list = new List<IPEndPoint>();
            List<SystemTcpConnectionInformation> connections = GetAllTcpConnections();
            foreach (TcpConnectionInformation connection in connections)
            {
                if (connection.State == TcpState.Listen)
                {
                    list.Add(connection.LocalEndPoint);
                }
            }
            return list.ToArray();
        }



        /// <summary>
        /// Gets the active tcp connections. Uses the native GetTcpTable api.</summary>
        private List<SystemTcpConnectionInformation> GetAllTcpConnections()
        {
            uint size = 0;
            uint result = 0;
            SafeLocalFree buffer = null;
            List<SystemTcpConnectionInformation> tcpConnections = new List<SystemTcpConnectionInformation>();

            // Check if it supports IPv4 for IPv6 only modes.
            if (Socket.OSSupportsIPv4)
            {
                // Get the size of buffer needed
                result = UnsafeNetInfoNativeMethods.GetTcpTable(SafeLocalFree.Zero, ref size, true);

                while (result == IpHelperErrors.ErrorInsufficientBuffer)
                {
                    try
                    {
                        //allocate the buffer and get the tcptable
                        buffer = SafeLocalFree.LocalAlloc((int)size);
                        result = UnsafeNetInfoNativeMethods.GetTcpTable(buffer, ref size, true);

                        if (result == IpHelperErrors.Success)
                        {
                            //the table info just gives us the number of rows.
                            IntPtr newPtr = buffer.DangerousGetHandle();
                            MibTcpTable tcpTableInfo = Marshal.PtrToStructure<MibTcpTable>(newPtr);

                            if (tcpTableInfo.numberOfEntries > 0)
                            {
                                //we need to skip over the tableinfo to get the inline rows
                                newPtr = (IntPtr)((long)newPtr + Marshal.SizeOf(tcpTableInfo.numberOfEntries));

                                for (int i = 0; i < tcpTableInfo.numberOfEntries; i++)
                                {
                                    MibTcpRow tcpRow = Marshal.PtrToStructure<MibTcpRow>(newPtr);
                                    tcpConnections.Add(new SystemTcpConnectionInformation(tcpRow));

                                    //we increment the pointer to the next row
                                    newPtr = (IntPtr)((long)newPtr + Marshal.SizeOf(tcpRow));
                                }
                            }
                        }
                    }
                    finally
                    {
                        if (buffer != null)
                            buffer.Dispose();
                    }
                }

                // if we don't have any ipv4 interfaces detected, just continue
                if (result != IpHelperErrors.Success && result != IpHelperErrors.ErrorNoData)
                {
                    throw new NetworkInformationException((int)result);
                }
            }

            if (Socket.OSSupportsIPv6)
            {
                // IPv6 tcp connections
                // Get the size of buffer needed
                size = 0;
                result = UnsafeNetInfoNativeMethods.GetExtendedTcpTable(SafeLocalFree.Zero, ref size, true,
                                                                        (uint)AddressFamily.InterNetworkV6,
                                                                        TcpTableClass.TcpTableOwnerPidAll, 0);

                while (result == IpHelperErrors.ErrorInsufficientBuffer)
                {
                    try
                    {
                        // Allocate the buffer and get the tcptable
                        buffer = SafeLocalFree.LocalAlloc((int)size);
                        result = UnsafeNetInfoNativeMethods.GetExtendedTcpTable(buffer, ref size, true,
                                                                                (uint)AddressFamily.InterNetworkV6,
                                                                                TcpTableClass.TcpTableOwnerPidAll, 0);
                        if (result == IpHelperErrors.Success)
                        {
                            // The table info just gives us the number of rows.
                            IntPtr newPtr = buffer.DangerousGetHandle();

                            MibTcp6TableOwnerPid tcpTable6OwnerPid = Marshal.PtrToStructure<MibTcp6TableOwnerPid>(newPtr);

                            if (tcpTable6OwnerPid.numberOfEntries > 0)
                            {
                                // We need to skip over the tableinfo to get the inline rows
                                newPtr = (IntPtr)((long)newPtr + Marshal.SizeOf(tcpTable6OwnerPid.numberOfEntries));

                                for (int i = 0; i < tcpTable6OwnerPid.numberOfEntries; i++)
                                {
                                    MibTcp6RowOwnerPid tcp6RowOwnerPid = Marshal.PtrToStructure<MibTcp6RowOwnerPid>(newPtr);
                                    tcpConnections.Add(new SystemTcpConnectionInformation(tcp6RowOwnerPid));

                                    // We increment the pointer to the next row
                                    newPtr = (IntPtr)((long)newPtr + Marshal.SizeOf(tcp6RowOwnerPid));
                                }
                            }
                        }
                    }
                    finally
                    {
                        if (buffer != null)
                            buffer.Dispose();
                    }
                }

                // If we don't have any ipv6 interfaces detected, just continue
                if (result != IpHelperErrors.Success && result != IpHelperErrors.ErrorNoData)
                {
                    throw new NetworkInformationException((int)result);
                }
            }

            return tcpConnections;
        }




        /// <summary>Gets the active udp listeners. Uses the native GetUdpTable api.</summary>
        public override IPEndPoint[] GetActiveUdpListeners()
        {
            uint size = 0;
            uint result = 0;
            SafeLocalFree buffer = null;
            List<IPEndPoint> udpListeners = new List<IPEndPoint>();

            // Check if it support IPv4 for IPv6 only modes.
            if (Socket.OSSupportsIPv4)
            {
                // Get the size of buffer needed
                result = UnsafeNetInfoNativeMethods.GetUdpTable(SafeLocalFree.Zero, ref size, true);
                while (result == IpHelperErrors.ErrorInsufficientBuffer)
                {
                    try
                    {
                        //allocate the buffer and get the udptable
                        buffer = SafeLocalFree.LocalAlloc((int)size);
                        result = UnsafeNetInfoNativeMethods.GetUdpTable(buffer, ref size, true);

                        if (result == IpHelperErrors.Success)
                        {
                            //the table info just gives us the number of rows.
                            IntPtr newPtr = buffer.DangerousGetHandle();
                            MibUdpTable udpTableInfo = Marshal.PtrToStructure<MibUdpTable>(newPtr);

                            if (udpTableInfo.numberOfEntries > 0)
                            {
                                //we need to skip over the tableinfo to get the inline rows
                                newPtr = (IntPtr)((long)newPtr + Marshal.SizeOf(udpTableInfo.numberOfEntries));
                                for (int i = 0; i < udpTableInfo.numberOfEntries; i++)
                                {
                                    MibUdpRow udpRow = Marshal.PtrToStructure<MibUdpRow>(newPtr);
                                    int localPort = udpRow.localPort1 << 8 | udpRow.localPort2;

                                    udpListeners.Add(new IPEndPoint(udpRow.localAddr, (int)localPort));

                                    //we increment the pointer to the next row
                                    newPtr = (IntPtr)((long)newPtr + Marshal.SizeOf(udpRow));
                                }
                            }
                        }
                    }
                    finally
                    {
                        if (buffer != null)
                            buffer.Dispose();
                    }
                }
                // if we don't have any ipv4 interfaces detected, just continue
                if (result != IpHelperErrors.Success && result != IpHelperErrors.ErrorNoData)
                {
                    throw new NetworkInformationException((int)result);
                }
            }

            if (Socket.OSSupportsIPv6)
            {
                // Get the size of buffer needed
                size = 0;
                result = UnsafeNetInfoNativeMethods.GetExtendedUdpTable(SafeLocalFree.Zero, ref size, true,
                                                                        (uint)AddressFamily.InterNetworkV6,
                                                                        UdpTableClass.UdpTableOwnerPid, 0);
                while (result == IpHelperErrors.ErrorInsufficientBuffer)
                {
                    try
                    {
                        // Allocate the buffer and get the udptable
                        buffer = SafeLocalFree.LocalAlloc((int)size);
                        result = UnsafeNetInfoNativeMethods.GetExtendedUdpTable(buffer, ref size, true,
                                                                                (uint)AddressFamily.InterNetworkV6,
                                                                                UdpTableClass.UdpTableOwnerPid, 0);

                        if (result == IpHelperErrors.Success)
                        {
                            // The table info just gives us the number of rows.
                            IntPtr newPtr = buffer.DangerousGetHandle();
                            MibUdp6TableOwnerPid udp6TableOwnerPid = Marshal.PtrToStructure<MibUdp6TableOwnerPid>(newPtr);

                            if (udp6TableOwnerPid.numberOfEntries > 0)
                            {
                                // We need to skip over the tableinfo to get the inline rows
                                newPtr = (IntPtr)((long)newPtr + Marshal.SizeOf(udp6TableOwnerPid.numberOfEntries));
                                for (int i = 0; i < udp6TableOwnerPid.numberOfEntries; i++)
                                {
                                    MibUdp6RowOwnerPid udp6RowOwnerPid = Marshal.PtrToStructure<MibUdp6RowOwnerPid>(newPtr);
                                    int localPort = udp6RowOwnerPid.localPort1 << 8 | udp6RowOwnerPid.localPort2;

                                    udpListeners.Add(new IPEndPoint(new IPAddress(udp6RowOwnerPid.localAddr,
                                        udp6RowOwnerPid.localScopeId), localPort));

                                    // We increment the pointer to the next row
                                    newPtr = (IntPtr)((long)newPtr + Marshal.SizeOf(udp6RowOwnerPid));
                                }
                            }
                        }
                    }
                    finally
                    {
                        if (buffer != null)
                            buffer.Dispose();
                    }
                }
                // If we don't have any ipv6 interfaces detected, just continue
                if (result != IpHelperErrors.Success && result != IpHelperErrors.ErrorNoData)
                {
                    throw new NetworkInformationException((int)result);
                }
            }

            return udpListeners.ToArray();
        }

        public override IPGlobalStatistics GetIPv4GlobalStatistics()
        {
            return new SystemIPGlobalStatistics(AddressFamily.InterNetwork);
        }
        public override IPGlobalStatistics GetIPv6GlobalStatistics()
        {
            return new SystemIPGlobalStatistics(AddressFamily.InterNetworkV6);
        }

        public override TcpStatistics GetTcpIPv4Statistics()
        {
            return new SystemTcpStatistics(AddressFamily.InterNetwork);
        }
        public override TcpStatistics GetTcpIPv6Statistics()
        {
            return new SystemTcpStatistics(AddressFamily.InterNetworkV6);
        }

        public override UdpStatistics GetUdpIPv4Statistics()
        {
            return new SystemUdpStatistics(AddressFamily.InterNetwork);
        }
        public override UdpStatistics GetUdpIPv6Statistics()
        {
            return new SystemUdpStatistics(AddressFamily.InterNetworkV6);
        }

        public override IcmpV4Statistics GetIcmpV4Statistics()
        {
            return new SystemIcmpV4Statistics();
        }

        public override IcmpV6Statistics GetIcmpV6Statistics()
        {
            return new SystemIcmpV6Statistics();
        }

        public override UnicastIPAddressInformationCollection GetUnicastAddresses()
        {
            // Wait for the Address Table to stabilize
            using (ManualResetEvent stable = new ManualResetEvent(false))
            {
                if (!TeredoHelper.UnsafeNotifyStableUnicastIpAddressTable(StableUnicastAddressTableCallback, stable))
                {
                    stable.WaitOne();
                }
            }

            return GetUnicastAddressTable();
        }

        public override IAsyncResult BeginGetUnicastAddresses(AsyncCallback callback, object state)
        {
            ContextAwareResult asyncResult = new ContextAwareResult(false, false, this, state, callback);
            asyncResult.StartPostingAsyncOp(false);
            if (TeredoHelper.UnsafeNotifyStableUnicastIpAddressTable(StableUnicastAddressTableCallback, asyncResult))
            {
                asyncResult.InvokeCallback();
            }
            asyncResult.FinishPostingAsyncOp();

            return asyncResult;
        }

        public override UnicastIPAddressInformationCollection EndGetUnicastAddresses(IAsyncResult asyncResult)
        {
            if (asyncResult == null)
            {
                throw new ArgumentNullException("asyncResult");
            }

            ContextAwareResult result = asyncResult as ContextAwareResult;
            if (result == null || result.AsyncObject == null || result.AsyncObject.GetType() != typeof(SystemIPGlobalProperties))
            {
                throw new ArgumentException(SR.net_io_invalidasyncresult);
            }

            if (result.EndCalled)
            {
                throw new InvalidOperationException(SR.Format(SR.net_io_invalidendcall, "EndGetStableUnicastAddresses"));
            }

            result.InternalWaitForCompletion();

            result.EndCalled = true;
            return GetUnicastAddressTable();
        }

        private static void StableUnicastAddressTableCallback(object param)
        {
            EventWaitHandle handle = param as EventWaitHandle;
            if (handle != null)
            {
                handle.Set();
            }
            else
            {
                LazyAsyncResult asyncResult = (LazyAsyncResult)param;
                asyncResult.InvokeCallback();
            }
        }

        private static UnicastIPAddressInformationCollection GetUnicastAddressTable()
        {
            UnicastIPAddressInformationCollection rval = new UnicastIPAddressInformationCollection();

            NetworkInterface[] interfaces = NetworkInterface.GetAllNetworkInterfaces();
            for (int i = 0; i < interfaces.Length; ++i)
            {
                UnicastIPAddressInformationCollection addresses = interfaces[i].GetIPProperties().UnicastAddresses;

                foreach (UnicastIPAddressInformation address in addresses)
                {
                    if (!rval.Contains(address))
                    {
                        rval.InternalAdd(address);
                    }
                }
            }

            return rval;
        }
    }   //ends networkinformation class
}

