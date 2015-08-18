using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

using Xunit;

namespace NCLTest.Sockets
{
    public class UDPClientTest
    {
        private const int TestPortBase = 8400;
        private ManualResetEvent _waitHandle = new ManualResetEvent(false);

        [Fact]
        public void BeginSend_NegativeBytes_Throws()
        {
            UdpClient udpClient = new UdpClient();
            byte[] sendBytes = new byte[1];
            IPEndPoint remoteServer = new IPEndPoint(IPAddress.Loopback, TestPortBase);

            Assert.Throws<ArgumentOutOfRangeException>("bytes", () =>
            {
                udpClient.BeginSend(sendBytes, -1, remoteServer, new AsyncCallback(AsyncCompleted), udpClient);
            });
        }

        [Fact]
        public void BeginSend_BytesMoreThanArrayLength_Throws()
        {
            UdpClient udpClient = new UdpClient();
            byte[] sendBytes = new byte[1];
            IPEndPoint remoteServer = new IPEndPoint(IPAddress.Loopback, TestPortBase + 1);

            Assert.Throws<ArgumentOutOfRangeException>("bytes", () =>
            {
                udpClient.BeginSend(sendBytes, sendBytes.Length + 1, remoteServer, new AsyncCallback(AsyncCompleted), udpClient);
            });
        }

        [Fact]
        public void BeginSend_AsyncOperationCompletes_Success()
        {
            UdpClient udpClient = new UdpClient();
            byte[] sendBytes = new byte[1];
            IPEndPoint remoteServer = new IPEndPoint(IPAddress.Loopback, TestPortBase + 2);
            _waitHandle.Reset();
            udpClient.BeginSend(sendBytes, sendBytes.Length, remoteServer, new AsyncCallback(AsyncCompleted), udpClient);

            Assert.True(_waitHandle.WaitOne(5000), "Timed out while waiting for connection");
        }

        private void AsyncCompleted(IAsyncResult ar)
        {
            UdpClient udpService = (UdpClient)ar.AsyncState;
            udpService.EndSend(ar);
            _waitHandle.Set();
        }
    }
}
