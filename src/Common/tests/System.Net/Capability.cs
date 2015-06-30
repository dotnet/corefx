// Copyright (c) Microsoft. All rights reserved.

using CoreFXTestLibrary;
using System;
using System.Collections.Generic;
#if PROJECTN
using Windows.Networking;
using Windows.Networking.Sockets;
#else
using System.Net;
using System.Threading;
#endif

namespace NCLTest.Common
{
    using System.Net.Sockets;

    public class Capability
    {
        // Capabilities declarations
        [Flags]
        public enum Type
        {
            InternetAccess,
            CorpnetAccess
        }

        // Capability type to check associations
        private static Dictionary<Type, Func<bool>> checks = new Dictionary<Type, Func<bool>>
        {
            {Type.InternetAccess, () => {return Capability.CheckInternetAccess();}},
            {Type.CorpnetAccess, () => {return Capability.CheckCorpnetAccess();}}
        };

        public static bool Check(Capability.Type capabilities)
        {
            bool ret = true;

            foreach (Type flag in Enum.GetValues(typeof(Type)))
            {
                if (capabilities.HasFlag(flag))
                {
                    Func<bool> check = checks[flag];
                    bool checkRet = check();
                    ret &= checkRet;

                    if (!checkRet)
                    {
                        Logger.LogInformation("Required capability not found: " + flag.ToString());
                    }
                }
            }

            return ret;
        }

        public static void Assert(Capability.Type capabilities)
        {
            CoreFXTestLibrary.Assert.IsTrue(Check(capabilities), "Required capabilities not found");
        }
        
        private static bool CheckInternetAccess()
        {
            return PortPing("www.microsoft.com", 80);
        }

        private static bool CheckCorpnetAccess()
        {
            return PortPing("nclweb", 80);
        }
#if !PROJECTN
        private static void OnConnectCompleted(object sender, SocketAsyncEventArgs args)
        {
            EventWaitHandle handle = (EventWaitHandle)args.UserToken;
            handle.Set();
        }

        private static bool PortPing(string server, int port, AddressFamily addressType )
        {
            Socket tcpSocket;
            bool connectionSuccess;
            try
            {
                AutoResetEvent completed = new AutoResetEvent(false);
                using (tcpSocket = new Socket(addressType, SocketType.Stream, ProtocolType.Tcp))
                {
                    SocketAsyncEventArgs args = new SocketAsyncEventArgs();
                    args.RemoteEndPoint = new DnsEndPoint(server, port);
                    args.Completed += new EventHandler<SocketAsyncEventArgs>(OnConnectCompleted);
                    args.UserToken = completed;
                    tcpSocket.ConnectAsync(args);
                    if (!completed.WaitOne(5000))
                    {
                        connectionSuccess = false;
                        Logger.LogInformation("Tcp socket timed out while waiting for async connection");
                    }
                    else if (args.SocketError != SocketError.Success)
                    {
                        connectionSuccess = false;
                    }
                    else
                    {
                        connectionSuccess = true;
                    }
                }
            }
            catch (SocketException)
            {
                connectionSuccess = false;
            }
            return connectionSuccess;
        }
#endif

        private static bool PortPing(string server, int port)
        {
#if PROJECTN
            StreamSocket s = null;
            
            try
            {
                s = new StreamSocket();
                bool success = s.ConnectAsync(new HostName(server), port.ToString()).AsTask().Wait(new TimeSpan(0, 0, 2));
                
                return success;
            }
            catch (Exception)
            {
                return false;
            }
            finally
            {
                if (s != null)
                {
                    s.Dispose();
                }
            }
#else
            bool ipv4ConnectSuccess = PortPing(server, port, AddressFamily.InterNetwork);
            bool ipv6ConnectSuccess = PortPing(server, port, AddressFamily.InterNetworkV6);
            return (ipv4ConnectSuccess || ipv6ConnectSuccess);
#endif
        }
    }
}