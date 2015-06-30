namespace NCLTest.Sockets
{
    using CoreFXTestLibrary;
    using System;
    using System.Net;
    using System.Net.Sockets;
    
    [TestClass]
    public class TimeoutTest
    {
        private const int TestPortBase = 8110;

        private const int ServerPort = TestPortBase;

        [TestMethod]
        public void GetAndSet_Success()
        {
            Socket socket = new Socket(AddressFamily.InterNetworkV6, SocketType.Stream, ProtocolType.Tcp);
            Assert.AreEqual(0, socket.ReceiveTimeout);
            socket.ReceiveTimeout = 100;
            Assert.AreEqual(100, socket.ReceiveTimeout);
        }

        [TestMethod]
        public void SocketSendTimeout_GetAndSet_Success()
        {
            Socket socket = new Socket(AddressFamily.InterNetworkV6, SocketType.Stream, ProtocolType.Tcp);
            Assert.AreEqual(0, socket.SendTimeout);
            socket.SendTimeout = 100;
            Assert.AreEqual(100, socket.SendTimeout);
        }

        [TestMethod]
        public void ReceiveTimesOut_Throws()
        {
            using (Socket localSocket = new Socket(AddressFamily.InterNetworkV6, SocketType.Stream, ProtocolType.Tcp))
            {
                using (Socket remoteSocket = new Socket(AddressFamily.InterNetworkV6, SocketType.Stream, ProtocolType.Tcp))
                {
                    localSocket.Bind(new IPEndPoint(IPAddress.IPv6Loopback, ServerPort));
                    localSocket.Listen(1);
                    IAsyncResult localAsync = localSocket.BeginAccept(null, null);

                    remoteSocket.Connect(IPAddress.IPv6Loopback, ServerPort);

                    Socket acceptedSocket = localSocket.EndAccept(localAsync);
                    acceptedSocket.ReceiveTimeout = 100;

                    try
                    {
                        int bytes = acceptedSocket.Receive(new byte[1]);
                        Assert.Fail("Timeout Expected");
                    }
                    catch (SocketException sockEx)
                    {
                        Assert.AreEqual(SocketError.TimedOut, sockEx.SocketErrorCode);
                        Assert.IsTrue(acceptedSocket.Connected);
                    }
                }                
            }
        }

        [TestMethod]
        public void SocketSendTimeout_Send_Success()
        {
            using (Socket localSocket = new Socket(AddressFamily.InterNetworkV6, SocketType.Stream, ProtocolType.Tcp))
            {
                using (Socket remoteSocket = new Socket(AddressFamily.InterNetworkV6, SocketType.Stream, ProtocolType.Tcp))
                {
                    localSocket.Bind(new IPEndPoint(IPAddress.IPv6Loopback, ServerPort));
                    localSocket.Listen(1);
                    IAsyncResult localAsync = localSocket.BeginAccept(null, null);

                    remoteSocket.Connect(IPAddress.IPv6Loopback, ServerPort);

                    Socket acceptedSocket = localSocket.EndAccept(localAsync);
                    acceptedSocket.SendTimeout = 100;

                    // Note that Send almost never times out because it only has to copy the data to the native buffer.
                    int bytes = acceptedSocket.Send(new byte[100]);
                    Assert.AreEqual(100, bytes);
                    Assert.IsTrue(acceptedSocket.Connected);
                }
            }
        }
    }
}
