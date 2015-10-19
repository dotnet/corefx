// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;

namespace System.Net.NetworkInformation
{
    // Linux implementation of NetworkChange
    public class NetworkChange
    {
        private static NetworkAddressChangedEventHandler s_addressChangedSubscribers;
        private static IntPtr s_socket;
        private static readonly object s_lockObj = new object();

        static public event NetworkAddressChangedEventHandler NetworkAddressChanged
        {
            add
            {
                lock (s_lockObj)
                {
                    if (s_socket == IntPtr.Zero)
                    {
                        CreateSocket();
                    }

                    s_addressChangedSubscribers += value;
                }
            }
            remove
            {
                lock (s_lockObj)
                {
                    if (s_addressChangedSubscribers == null)
                    {
                        Debug.Assert(s_socket == IntPtr.Zero);
                        return;
                    }

                    s_addressChangedSubscribers -= value;
                    if (s_addressChangedSubscribers == null)
                    {
                        CloseSocket();
                    }
                }
            }
        }

        private static void CreateSocket()
        {
            Debug.Assert(s_socket == IntPtr.Zero);
            s_socket = Interop.Sys.CreateNetworkChangeListenerSocket();
            if (s_socket == new IntPtr(-1))
            {
                throw new NetworkInformationException("Error creating the netlink socket.");
            }
            Task.Run(() => LoopReadSocket(s_socket));
        }

        private static void CloseSocket()
        {

        }

        private static void LoopReadSocket(IntPtr socket)
        {
            while (socket == s_socket)
            {
                Interop.Sys.NetworkChangeKind kind = Interop.Sys.ReadSingleEvent(s_socket);
                if (kind == Interop.Sys.NetworkChangeKind.None)
                {
                    continue;
                }
                else
                {
                    lock (s_lockObj)
                    {
                        if (socket == s_socket)
                        {
                            OnSocketEvent(kind);
                        }
                    }
                }
            }
        }

        private static void OnSocketEvent(Interop.Sys.NetworkChangeKind kind)
        {
            switch (kind)
            {
                case Interop.Sys.NetworkChangeKind.AddressAdded:
                case Interop.Sys.NetworkChangeKind.AddressRemoved:
                    if (s_addressChangedSubscribers != null)
                    {
                        s_addressChangedSubscribers(null, EventArgs.Empty);
                    }
                    break;
            }
        }
    }
}
