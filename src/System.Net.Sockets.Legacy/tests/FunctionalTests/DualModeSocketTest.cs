using System.Net.Test.Common;
using System.Threading;

using Xunit;
using Xunit.Abstractions;

namespace System.Net.Sockets.Tests
{
    public class DualMode
    {
        private const int TestPortBase = 8200;  // to 8300
        private readonly ITestOutputHelper _log;

        public DualMode(ITestOutputHelper output)
        {
            _log = TestLogging.GetInstance();
        }

        #region Constructor and Property

        [Fact]
        public void DualModeConstructor_DualModeConfgiured()
        {
            Socket socket = new Socket(SocketType.Stream, ProtocolType.Tcp);
            Assert.Equal(AddressFamily.InterNetworkV6, socket.AddressFamily);
            Assert.True(socket.DualMode);
        }

        [Fact]
        public void NormalConstructor_DualModeConfgiureable()
        {
            Socket socket = new Socket(AddressFamily.InterNetworkV6, SocketType.Stream, ProtocolType.Tcp);
            Assert.False(socket.DualMode);

            socket.DualMode = true;
            Assert.True(socket.DualMode);

            socket.DualMode = false;
            Assert.False(socket.DualMode);
        }

        [Fact]
        public void IPv4Constructor_DualModeThrows()
        {
            try
            {
                Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                Assert.False(socket.DualMode);
                socket.DualMode = true;

                Assert.Fail("Expected NotSupportedException");
            }
            catch (NotSupportedException)
            {
                // expected
                return;
            }
        }

        #endregion Constructor and Property

        #region Connect

        #region Connect Sync

        #region Connect to IPAddress

        [Fact] // Base case
        public void Socket_ConnectV4IPAddressToV4Host_Throws()
        {
            try 
            {
                Socket socket = new Socket(SocketType.Stream, ProtocolType.Tcp);
                socket.DualMode = false;
                socket.Connect(IPAddress.Loopback, TestPortBase);
                Assert.Fail("Expected NotSupportedException");
            }
            catch (NotSupportedException)
            {
                // expected
                return;
            }
        }

        [Fact] // Base Case
        public void ConnectV4MappedIPAddressToV4Host_Success()
        {
            DualModeConnect_IPAddressToHost_Helper(IPAddress.Loopback.MapToIPv6(), IPAddress.Loopback, false);
        }

        [Fact] // Base Case
        public void ConnectV4MappedIPAddressToDualHost_Success()
        {
            DualModeConnect_IPAddressToHost_Helper(IPAddress.Loopback.MapToIPv6(), IPAddress.IPv6Any, true);
        }

        [Fact]
        public void ConnectV4IPAddressToV4Host_Success()
        {
            DualModeConnect_IPAddressToHost_Helper(IPAddress.Loopback, IPAddress.Loopback, false);
        }

        [Fact]
        public void ConnectV6IPAddressToV6Host_Success()
        {
            DualModeConnect_IPAddressToHost_Helper(IPAddress.IPv6Loopback, IPAddress.IPv6Loopback, false);
        }

        [Fact]
        public void ConnectV4IPAddressToV6Host_Fails()
        {
            try
            {
                DualModeConnect_IPAddressToHost_Helper(IPAddress.Loopback, IPAddress.IPv6Loopback, false);
                Assert.Fail("Expected SocketException");
            }
            catch (SocketException)
            {
                // expected
                return;
            }
        }

        [Fact]
        public void ConnectV6IPAddressToV4Host_Fails()
        {
            try
            {
                DualModeConnect_IPAddressToHost_Helper(IPAddress.IPv6Loopback, IPAddress.Loopback, false);
                Assert.Fail("Expected SocketException");
            }
            catch (SocketException)
            {
                // expected
                return;
            }
        }

        [Fact]
        public void ConnectV4IPAddressToDualHost_Success()
        {
            DualModeConnect_IPAddressToHost_Helper(IPAddress.Loopback, IPAddress.IPv6Any, true);
        }

        [Fact]
        public void ConnectV6IPAddressToDualHost_Success()
        {
            DualModeConnect_IPAddressToHost_Helper(IPAddress.IPv6Loopback, IPAddress.IPv6Any, true);
        }

        private void DualModeConnect_IPAddressToHost_Helper(IPAddress connectTo, IPAddress listenOn, bool dualModeServer)
        {
            Socket socket = new Socket(SocketType.Stream, ProtocolType.Tcp);
            using (SocketServer server = new SocketServer(listenOn, dualModeServer, TestPortBase + 1))
            {
                socket.Connect(connectTo, TestPortBase + 1);
                Assert.True(socket.Connected);
            }
        }

        #endregion Connect to IPAddress
        
        #region Connect to IPEndPoint
        
        [Fact] // Base case
        // "The system detected an invalid pointer address in attempting to use a pointer argument in a call 127.0.0.1:8989"
        public void Socket_ConnectV4IPEndPointToV4Host_Throws()
        {
            try
            {
                Socket socket = new Socket(SocketType.Stream, ProtocolType.Tcp);
                socket.DualMode = false;
                socket.Connect(new IPEndPoint(IPAddress.Loopback, TestPortBase + 2));
                Assert.Fail("Expected SocketException");
            }
            catch (SocketException)
            {
                // expected
                return;
            }
        }

        [Fact] // Base case
        public void ConnectV4MappedIPEndPointToV4Host_Success()
        {
            DualModeConnect_IPEndPointToHost_Helper(IPAddress.Loopback.MapToIPv6(), IPAddress.Loopback, false);
        }

        [Fact] // Base case
        public void ConnectV4MappedIPEndPointToDualHost_Success()
        {
            DualModeConnect_IPEndPointToHost_Helper(IPAddress.Loopback.MapToIPv6(), IPAddress.IPv6Any, true);
        }

        [Fact]
        public void ConnectV4IPEndPointToV4Host_Success()
        {
            DualModeConnect_IPEndPointToHost_Helper(IPAddress.Loopback, IPAddress.Loopback, false);
        }

        [Fact]
        public void ConnectV6IPEndPointToV6Host_Success()
        {
            DualModeConnect_IPEndPointToHost_Helper(IPAddress.IPv6Loopback, IPAddress.IPv6Loopback, false);
        }

        [Fact]
        public void ConnectV4IPEndPointToV6Host_Fails()
        {
            try
            {
                DualModeConnect_IPEndPointToHost_Helper(IPAddress.Loopback, IPAddress.IPv6Loopback, false);
                Assert.Fail("Expected SocketException");
            }
            catch (SocketException)
            {
                // expected
                return;
            }
        }

        [Fact]
        public void ConnectV6IPEndPointToV4Host_Fails()
        {
            try
            {
                DualModeConnect_IPEndPointToHost_Helper(IPAddress.IPv6Loopback, IPAddress.Loopback, false);
                Assert.Fail("Expected SocketException");
            }
            catch (SocketException)
            {
                // expected
                return;
            }
        }

        [Fact]
        public void ConnectV4IPEndPointToDualHost_Success()
        {
            DualModeConnect_IPEndPointToHost_Helper(IPAddress.Loopback, IPAddress.IPv6Any, true);
        }

        [Fact]
        public void ConnectV6IPEndPointToDualHost_Success()
        {
            DualModeConnect_IPEndPointToHost_Helper(IPAddress.IPv6Loopback, IPAddress.IPv6Any, true);
        }

        private void DualModeConnect_IPEndPointToHost_Helper(IPAddress connectTo, IPAddress listenOn, bool dualModeServer)
        {
            Socket socket = new Socket(SocketType.Stream, ProtocolType.Tcp);
            using (SocketServer server = new SocketServer(listenOn, dualModeServer, TestPortBase + 3))
            {                
                socket.Connect(new IPEndPoint(connectTo, TestPortBase + 3));
                Assert.True(socket.Connected);
            }
        }

        #endregion Connect to IPEndPoint

        #region Connect to IPAddress[]

        [Fact] // Base Case
        // "None of the discovered or specified addresses match the socket address family."
        public void Socket_ConnectV4IPAddressListToV4Host_Throws()
        {
            try
            {
                Socket socket = new Socket(SocketType.Stream, ProtocolType.Tcp);
                socket.DualMode = false;
                using (SocketServer server = new SocketServer(IPAddress.Loopback, false, TestPortBase + 4))
                {
                    socket.Connect(new IPAddress[] { IPAddress.Loopback }, TestPortBase + 4);
                }
                Assert.Fail("Expected ArgumentException");
            }
            catch (ArgumentException)
            {
                // expected
                return;
            }
        }

        [Fact] // Base Case
        public void ConnectMappedV4IPAddressListToV4Host_Success()
        {
            DualModeConnect_IPAddressListToHost_Helper(
                new IPAddress[] { IPAddress.Loopback.MapToIPv6() },
                IPAddress.Loopback, false);
        }

        [Fact]
        public void ConnectMappedV4IPAddressListToAnyV6Host_Throws()
        {
            try
            {
                DualModeConnect_IPAddressListToHost_Helper(
                    new IPAddress[] { IPAddress.Loopback.MapToIPv6() },
                    IPAddress.IPv6Loopback, false);
                Assert.Fail("Expected SocketException");
            }
            catch (SocketException)
            {
                // expected
                return;
            }
        }

        [Fact]
        public void ConnectV4IPAddressListToV4Host_Success()
        {
            DualModeConnect_IPAddressListToHost_Helper(
                new IPAddress[] { IPAddress.Loopback },
                IPAddress.Loopback, false);
        }

        [Fact]
        public void ConnectV4IPAddressListToSpecificV6Host_Throws()
        {
            try
            {
                DualModeConnect_IPAddressListToHost_Helper(
                    new IPAddress[] { IPAddress.Loopback },
                    IPAddress.IPv6Loopback, false);
                Assert.Fail("Expected SocketException");
            }
            catch (SocketException)
            {
                // expected
                return;
            }
        }

        [Fact]
        public void ConnectV4IPAddressListToAnyV6Host_Throws()
        {
            try
            {
                DualModeConnect_IPAddressListToHost_Helper(
                    new IPAddress[] { IPAddress.Loopback },
                    IPAddress.IPv6Any, false);
                Assert.Fail("Expected SocketException");
            }
            catch (SocketException)
            {
                // expected
                return;
            }
        }

        [Fact]
        public void ConnectV4IPAddressListToSpecificV6DualHost_Throws()
        {
            try
            {
                DualModeConnect_IPAddressListToHost_Helper(
                    new IPAddress[] { IPAddress.Loopback },
                    IPAddress.IPv6Loopback, true);
                Assert.Fail("Expected SocketException");
            }
            catch (SocketException)
            {
                // expected
                return;
            }
        }

        [Fact]
        public void ConnectV4IPAddressListToAnyV6DualHost_Success()
        {
            DualModeConnect_IPAddressListToHost_Helper(
                new IPAddress[] { IPAddress.Loopback },
                IPAddress.IPv6Any, true);
        }
        
        [Fact]
        public void ConnectV4V6IPAddressListToV4Host_Success()
        {
            DualModeConnect_IPAddressListToHost_Helper(
                new IPAddress[] { IPAddress.Loopback, IPAddress.IPv6Loopback }, 
                IPAddress.Loopback, false);
        }

        [Fact]
        public void ConnectV6V4IPAddressListToV4Host_Success()
        {
            DualModeConnect_IPAddressListToHost_Helper(
                new IPAddress[] { IPAddress.IPv6Loopback, IPAddress.Loopback },
                IPAddress.Loopback, false);
        }

        [Fact]
        public void ConnectV4V6IPAddressListToV6Host_Success()
        {
            DualModeConnect_IPAddressListToHost_Helper(
                new IPAddress[] { IPAddress.Loopback, IPAddress.IPv6Loopback },
                IPAddress.IPv6Loopback, false);
        }

        [Fact]
        public void ConnectV6V4IPAddressListToV6Host_Success()
        {
            DualModeConnect_IPAddressListToHost_Helper(
                new IPAddress[] { IPAddress.IPv6Loopback, IPAddress.Loopback },
                IPAddress.IPv6Loopback, false);
        }

        [Fact]
        public void ConnectV4V6IPAddressListToDualHost_Success()
        {
            DualModeConnect_IPAddressListToHost_Helper(
                new IPAddress[] { IPAddress.Loopback, IPAddress.IPv6Loopback },
                IPAddress.IPv6Any, true);
        }

        [Fact]
        public void ConnectV6V4IPAddressListToDualHost_Success()
        {
            DualModeConnect_IPAddressListToHost_Helper(
                new IPAddress[] { IPAddress.IPv6Loopback, IPAddress.Loopback },
                IPAddress.IPv6Any, true);
        }

        private void DualModeConnect_IPAddressListToHost_Helper(IPAddress[] connectTo, IPAddress listenOn, bool dualModeServer)
        {
            Socket socket = new Socket(SocketType.Stream, ProtocolType.Tcp);
            using (SocketServer server = new SocketServer(listenOn, dualModeServer, TestPortBase + 5))
            {
                socket.Connect(connectTo, TestPortBase + 5);
                Assert.True(socket.Connected);
            }
        }

        #endregion Connect to IPAdddress[]

        #region Connect to host string
        
        [Fact]
        public void ConnectLoopbackToV4Host_Success()
        {
            DualModeConnect_LoopackDnsToHost_Helper(IPAddress.Loopback, false);
        }

        [Fact]
        public void ConnectLoopbackToV6Host_Success()
        {
            DualModeConnect_LoopackDnsToHost_Helper(IPAddress.IPv6Loopback, false);
        }

        [Fact]
        public void ConnectLoopbackToDualHost_Success()
        {
            DualModeConnect_LoopackDnsToHost_Helper(IPAddress.IPv6Any, true);
        }

        private void DualModeConnect_LoopackDnsToHost_Helper(IPAddress listenOn, bool dualModeServer)
        {
            Socket socket = new Socket(SocketType.Stream, ProtocolType.Tcp);
            using (SocketServer server = new SocketServer(listenOn, dualModeServer, TestPortBase + 6))
            {
                socket.Connect("loopback", TestPortBase + 6);
                Assert.True(socket.Connected);
            }
        }

        #endregion Connect to host string

        #region Connect to DnsEndPoint

        [Fact]
        public void DualModeSocket_DnsEndPointToV4Host_Success()
        {
            DualModeConnect_DnsEndPointToHost_Helper(IPAddress.Loopback, false, AddressFamily.Unspecified);
        }

        [Fact]
        public void DualModeSocket_DnsEndPointToV6Host_Success()
        {
            DualModeConnect_DnsEndPointToHost_Helper(IPAddress.IPv6Loopback, false, AddressFamily.Unspecified);
        }

        [Fact]
        public void DualModeSocket_DnsEndPointToDualHost_Success()
        {
            DualModeConnect_DnsEndPointToHost_Helper(IPAddress.IPv6Any, true, AddressFamily.Unspecified);
        }

        private void DualModeConnect_DnsEndPointToHost_Helper(IPAddress listenOn, bool dualModeServer, AddressFamily addressFamily)
        {
            Socket socket = new Socket(SocketType.Stream, ProtocolType.Tcp);
            using (SocketServer server = new SocketServer(listenOn, dualModeServer, TestPortBase + 49))
            {
                socket.Connect(new DnsEndPoint("loopback", TestPortBase + 49, addressFamily));
                Assert.True(socket.Connected);
            }
        }

        #endregion Connect to DnsEndPoint

        #endregion Connect Sync

        #region BeginConnect
        
        #region BeginConnect to IPAddress
        
        [Fact] // Base case
        public void Socket_BeginConnectV4IPAddressToV4Host_Throws()
        {
            try
            {
                Socket socket = new Socket(SocketType.Stream, ProtocolType.Tcp);
                socket.DualMode = false;
                socket.BeginConnect(IPAddress.Loopback, TestPortBase + 39, null, null);
                Assert.Fail("Expected NotSupportedException");
            }
            catch (NotSupportedException)
            {
                // expected
                return;
            }
        }

        [Fact]
        public void BeginConnectV4IPAddressToV4Host_Success()
        {
            DualModeBeginConnect_IPAddressToHost_Helper(IPAddress.Loopback, IPAddress.Loopback, false);
        }

        [Fact]
        public void BeginConnectV6IPAddressToV6Host_Success()
        {
            DualModeBeginConnect_IPAddressToHost_Helper(IPAddress.IPv6Loopback, IPAddress.IPv6Loopback, false);
        }

        [Fact]
        public void BeginConnectV4IPAddressToV6Host_Fails()
        {
            try
            {
                DualModeBeginConnect_IPAddressToHost_Helper(IPAddress.Loopback, IPAddress.IPv6Loopback, false);
                Assert.Fail("Expected SocketException");
            }
            catch (SocketException)
            {
                // expected
                return;
            }
        }

        [Fact]
        public void BeginConnectV6IPAddressToV4Host_Fails()
        {
            try
            {
                DualModeBeginConnect_IPAddressToHost_Helper(IPAddress.IPv6Loopback, IPAddress.Loopback, false);
                Assert.Fail("Expected SocketException");
            }
            catch (SocketException)
            {
                // expected
                return;
            }
        }

        [Fact]
        public void BeginConnectV4IPAddressToDualHost_Success()
        {
            DualModeBeginConnect_IPAddressToHost_Helper(IPAddress.Loopback, IPAddress.IPv6Any, true);
        }

        [Fact]
        public void BeginConnectV6IPAddressToDualHost_Success()
        {
            DualModeBeginConnect_IPAddressToHost_Helper(IPAddress.IPv6Loopback, IPAddress.IPv6Any, true);
        }

        private void DualModeBeginConnect_IPAddressToHost_Helper(IPAddress connectTo, IPAddress listenOn, bool dualModeServer)
        {
            Socket socket = new Socket(SocketType.Stream, ProtocolType.Tcp);
            using (SocketServer server = new SocketServer(listenOn, dualModeServer, TestPortBase + 7))
            {
                IAsyncResult async = socket.BeginConnect(connectTo, TestPortBase + 7, null, null);
                socket.EndConnect(async);
                Assert.True(socket.Connected);
            }
        }

        #endregion BeginConnect to IPAddress
        
        #region BeginConnect to IPEndPoint
        
        [Fact] // Base case
        // "The system detected an invalid pointer address in attempting to use a pointer argument in a call"
        public void Socket_BeginConnectV4IPEndPointToV4Host_Throws()
        {
            try
            {
                Socket socket = new Socket(SocketType.Stream, ProtocolType.Tcp);
                socket.DualMode = false;
                socket.BeginConnect(new IPEndPoint(IPAddress.Loopback, TestPortBase + 8), null, null);
                Assert.Fail("Expected SocketException");
            }
            catch (SocketException)
            {
                // expected
                return;
            }
        }

        [Fact]
        public void BeginConnectV4IPEndPointToV4Host_Success()
        {
            DualModeBeginConnect_IPEndPointToHost_Helper(IPAddress.Loopback, IPAddress.Loopback, false);
        }

        [Fact]
        public void BeginConnectV6IPEndPointToV6Host_Success()
        {
            DualModeBeginConnect_IPEndPointToHost_Helper(IPAddress.IPv6Loopback, IPAddress.IPv6Loopback, false);
        }

        [Fact]
        public void BeginConnectV4IPEndPointToV6Host_Fails()
        {
            try
            {
                DualModeBeginConnect_IPEndPointToHost_Helper(IPAddress.Loopback, IPAddress.IPv6Loopback, false);
                Assert.Fail("Expected SocketException");
            }
            catch (SocketException)
            {
                // expected
                return;
            }
        }

        [Fact]
        public void BeginConnectV6IPEndPointToV4Host_Fails()
        {
            try
            {
                DualModeBeginConnect_IPEndPointToHost_Helper(IPAddress.IPv6Loopback, IPAddress.Loopback, false);
                Assert.Fail("Expected SocketException");
            }
            catch (SocketException)
            {
                // expected
                return;
            }
        }

        [Fact]
        public void BeginConnectV4IPEndPointToDualHost_Success()
        {
            DualModeBeginConnect_IPEndPointToHost_Helper(IPAddress.Loopback, IPAddress.IPv6Any, true);
        }

        [Fact]
        public void BeginConnectV6IPEndPointToDualHost_Success()
        {
            DualModeBeginConnect_IPEndPointToHost_Helper(IPAddress.IPv6Loopback, IPAddress.IPv6Any, true);
        }

        private void DualModeBeginConnect_IPEndPointToHost_Helper(IPAddress connectTo, IPAddress listenOn, bool dualModeServer)
        {
            Socket socket = new Socket(SocketType.Stream, ProtocolType.Tcp);
            using (SocketServer server = new SocketServer(listenOn, dualModeServer, TestPortBase + 9))
            {
                IAsyncResult async = socket.BeginConnect(new IPEndPoint(connectTo, TestPortBase + 9), null, null);
                socket.EndConnect(async);
                Assert.True(socket.Connected);
            }
        }

        #endregion BeginConnect to IPEndPoint

        #region BeginConnect to IPAddress[]

        [Fact]
        public void BeginConnectV4V6IPAddressListToV4Host_Success()
        {
            DualModeBeginConnect_IPAddressListToHost_Helper(
                new IPAddress[] { IPAddress.Loopback, IPAddress.IPv6Loopback },
                IPAddress.Loopback, false);
        }

        [Fact]
        public void BeginConnectV6V4IPAddressListToV4Host_Success()
        {
            DualModeBeginConnect_IPAddressListToHost_Helper(
                new IPAddress[] { IPAddress.IPv6Loopback, IPAddress.Loopback },
                IPAddress.Loopback, false);
        }

        [Fact]
        public void BeginConnectV4V6IPAddressListToV6Host_Success()
        {
            DualModeBeginConnect_IPAddressListToHost_Helper(
                new IPAddress[] { IPAddress.Loopback, IPAddress.IPv6Loopback },
                IPAddress.IPv6Loopback, false);
        }

        [Fact]
        public void BeginConnectV6V4IPAddressListToV6Host_Success()
        {
            DualModeBeginConnect_IPAddressListToHost_Helper(
                new IPAddress[] { IPAddress.IPv6Loopback, IPAddress.Loopback },
                IPAddress.IPv6Loopback, false);
        }

        [Fact]
        public void BeginConnectV4V6IPAddressListToDualHost_Success()
        {
            DualModeBeginConnect_IPAddressListToHost_Helper(
                new IPAddress[] { IPAddress.Loopback, IPAddress.IPv6Loopback },
                IPAddress.IPv6Any, true);
        }

        [Fact]
        public void BeginConnectV6V4IPAddressListToDualHost_Success()
        {
            DualModeBeginConnect_IPAddressListToHost_Helper(
                new IPAddress[] { IPAddress.IPv6Loopback, IPAddress.Loopback },
                IPAddress.IPv6Any, true);
        }

        private void DualModeBeginConnect_IPAddressListToHost_Helper(IPAddress[] connectTo, IPAddress listenOn, bool dualModeServer)
        {
            Socket socket = new Socket(SocketType.Stream, ProtocolType.Tcp);
            using (SocketServer server = new SocketServer(listenOn, dualModeServer, TestPortBase + 10))
            {
                IAsyncResult async = socket.BeginConnect(connectTo, TestPortBase + 10, null, null);
                socket.EndConnect(async);
                Assert.True(socket.Connected);
            }
        }

        #endregion BeginConnect to IPAdddress[]

        #region BeginConnect to host string

        [Fact]
        public void BeginConnectLoopbackToV4Host_Success()
        {
            DualModeBeginConnect_LoopackDnsToHost_Helper(IPAddress.Loopback, false);
        }

        [Fact]
        public void BeginConnectLoopbackToV6Host_Success()
        {
            DualModeBeginConnect_LoopackDnsToHost_Helper(IPAddress.IPv6Loopback, false);
        }

        [Fact]
        public void BeginConnectLoopbackToDualHost_Success()
        {
            DualModeBeginConnect_LoopackDnsToHost_Helper(IPAddress.IPv6Any, true);
        }

        private void DualModeBeginConnect_LoopackDnsToHost_Helper(IPAddress listenOn, bool dualModeServer)
        {
            Socket socket = new Socket(SocketType.Stream, ProtocolType.Tcp);
            using (SocketServer server = new SocketServer(listenOn, dualModeServer, TestPortBase + 11))
            {
                IAsyncResult async = socket.BeginConnect("loopback", TestPortBase + 11, null, null);
                socket.EndConnect(async);
                Assert.True(socket.Connected);
            }
        }

        #endregion BeginConnect to host string

        #region BeginConnect to DnsEndPoint

        [Fact]
        public void DualModeSocket_BeginConnectDnsEndPointToV4Host_Success()
        {
            DualModeBeginConnect_DnsEndPointToHost_Helper(IPAddress.Loopback, false);
        }

        [Fact]
        public void DualModeSocket_BeginConnectDnsEndPointToV6Host_Success()
        {
            DualModeBeginConnect_DnsEndPointToHost_Helper(IPAddress.IPv6Loopback, false);
        }

        [Fact]
        public void DualModeSocket_BeginConnectDnsEndPointToDualHost_Success()
        {
            DualModeBeginConnect_DnsEndPointToHost_Helper(IPAddress.IPv6Any, true);
        }

        private void DualModeBeginConnect_DnsEndPointToHost_Helper(IPAddress listenOn, bool dualModeServer)
        {
            Socket socket = new Socket(SocketType.Stream, ProtocolType.Tcp);
            using (SocketServer server = new SocketServer(listenOn, dualModeServer, TestPortBase + 50))
            {
                IAsyncResult async = socket.BeginConnect(new DnsEndPoint("loopback", TestPortBase + 50), null, null);
                socket.EndConnect(async);
                Assert.True(socket.Connected);
            }
        }

        #endregion BeginConnect to DnsEndPoint

        #endregion BeginConnect

        #region ConnectAsync

        #region ConnectAsync to IPEndPoint
        
        [Fact] // Base case
        public void Socket_ConnectAsyncV4IPEndPointToV4Host_Throws()
        {
            try
            {
                Socket socket = new Socket(SocketType.Stream, ProtocolType.Tcp);
                socket.DualMode = false;

                SocketAsyncEventArgs args = new SocketAsyncEventArgs();
                args.RemoteEndPoint = new IPEndPoint(IPAddress.Loopback, TestPortBase + 12);
                socket.ConnectAsync(args);
                Assert.Fail("Expected NotSupportedException");
            }
            catch (NotSupportedException)
            {
                // expected
                return;
            }
        }

        [Fact]
        public void ConnectAsyncV4IPEndPointToV4Host_Success()
        {
            DualModeConnectAsync_IPEndPointToHost_Helper(IPAddress.Loopback, IPAddress.Loopback, false);
        }

        [Fact]
        public void ConnectAsyncV6IPEndPointToV6Host_Success()
        {
            DualModeConnectAsync_IPEndPointToHost_Helper(IPAddress.IPv6Loopback, IPAddress.IPv6Loopback, false);
        }

        [Fact]
        public void ConnectAsyncV4IPEndPointToV6Host_Fails()
        {
            try
            {
                DualModeConnectAsync_IPEndPointToHost_Helper(IPAddress.Loopback, IPAddress.IPv6Loopback, false);
                Assert.Fail("Expected SocketException");
            }
            catch (SocketException)
            {
                // expected
                return;
            }
        }

        [Fact]
        public void ConnectAsyncV6IPEndPointToV4Host_Fails()
        {
            try
            {
                DualModeConnectAsync_IPEndPointToHost_Helper(IPAddress.IPv6Loopback, IPAddress.Loopback, false);
                Assert.Fail("Expected SocketException");
            }
            catch (SocketException)
            {
                // expected
                return;
            }
        }

        [Fact]
        public void ConnectAsyncV4IPEndPointToDualHost_Success()
        {
            DualModeConnectAsync_IPEndPointToHost_Helper(IPAddress.Loopback, IPAddress.IPv6Any, true);
        }

        [Fact]
        public void ConnectAsyncV6IPEndPointToDualHost_Success()
        {
            DualModeConnectAsync_IPEndPointToHost_Helper(IPAddress.IPv6Loopback, IPAddress.IPv6Any, true);
        }

        private void DualModeConnectAsync_IPEndPointToHost_Helper(IPAddress connectTo, IPAddress listenOn, bool dualModeServer)
        {
            Socket socket = new Socket(SocketType.Stream, ProtocolType.Tcp);
            using (SocketServer server = new SocketServer(listenOn, dualModeServer, TestPortBase + 13))
            {
                ManualResetEvent waitHandle = new ManualResetEvent(false);
                SocketAsyncEventArgs args = new SocketAsyncEventArgs();
                args.Completed += new EventHandler<SocketAsyncEventArgs>(AsyncCompleted);
                args.RemoteEndPoint = new IPEndPoint(connectTo, TestPortBase + 13);
                args.UserToken = waitHandle;

                socket.ConnectAsync(args);

                Assert.True(waitHandle.WaitOne(5000), "Timed out while waiting for connection");
                if (args.SocketError != SocketError.Success)
                {
                    throw new SocketException((int)args.SocketError);
                }
                Assert.True(socket.Connected);
            }
        }

        #endregion ConnectAsync to IPAddress

        #region ConnectAsync to DnsEndPoint

        [Fact]
        public void DualModeSocket_ConnectAsyncDnsEndPointToV4Host_Success()
        {
            DualModeConnectAsync_DnsEndPointToHost_Helper(IPAddress.Loopback, false);
        }

        [Fact]
        public void DualModeSocket_ConnectAsyncDnsEndPointToV6Host_Success()
        {
            DualModeConnectAsync_DnsEndPointToHost_Helper(IPAddress.IPv6Loopback, false);
        }

        [Fact]
        public void DualModeSocket_ConnectAsyncDnsEndPointToDualHost_Success()
        {
            DualModeConnectAsync_DnsEndPointToHost_Helper(IPAddress.IPv6Any, true);
        }

        private void DualModeConnectAsync_DnsEndPointToHost_Helper(IPAddress listenOn, bool dualModeServer)
        {
            Socket socket = new Socket(SocketType.Stream, ProtocolType.Tcp);
            using (SocketServer server = new SocketServer(listenOn, dualModeServer, TestPortBase + 51))
            {
                ManualResetEvent waitHandle = new ManualResetEvent(false);
                SocketAsyncEventArgs args = new SocketAsyncEventArgs();
                args.Completed += new EventHandler<SocketAsyncEventArgs>(AsyncCompleted);
                args.RemoteEndPoint = new DnsEndPoint("loopback", TestPortBase + 51);
                args.UserToken = waitHandle;

                socket.ConnectAsync(args);

                waitHandle.WaitOne();
                if (args.SocketError != SocketError.Success)
                {
                    throw new SocketException((int)args.SocketError);
                }
                Assert.True(socket.Connected);
            }
        }

        #endregion ConnectAsync to DnsEndPoint

        #endregion ConnectAsync

        #endregion Connect

        #region Accept

        #region Bind

        [Fact] // Base case
        // "The system detected an invalid pointer address in attempting to use a pointer argument in a call"
        public void Socket_BindV4IPEndPoint_Throws()
        {
            try
            {
                using (Socket socket = new Socket(SocketType.Stream, ProtocolType.Tcp))
                {
                    socket.DualMode = false;
                    socket.Bind(new IPEndPoint(IPAddress.Loopback, TestPortBase + 14));
                }
                Assert.Fail("Expected SocketException");
            }
            catch (SocketException)
            {
                // expected
                return;
            }
        }

        [Fact] // Base Case; BSoD on Win7, Win8 with IPv4 uninstalled
        public void BindMappedV4IPEndPoint_Success()
        {
            using (Socket socket = new Socket(SocketType.Stream, ProtocolType.Tcp))
            {
                socket.Bind(new IPEndPoint(IPAddress.Loopback.MapToIPv6(), TestPortBase + 15));
            }
        }

        [Fact] // BSoD on Win7, Win8 with IPv4 uninstalled
        public void BindV4IPEndPoint_Success()
        {
            using (Socket socket = new Socket(SocketType.Stream, ProtocolType.Tcp))
            {
                socket.Bind(new IPEndPoint(IPAddress.Loopback, TestPortBase + 16));
            }
        }

        [Fact]
        public void BindV6IPEndPoint_Success()
        {
            using (Socket socket = new Socket(SocketType.Stream, ProtocolType.Tcp))
            {
                socket.Bind(new IPEndPoint(IPAddress.IPv6Loopback, TestPortBase + 17));
            }
        }

        [Fact]
        [ExpectedException(typeof(System.ArgumentException))]
        public void Socket_BindDnsEndPoint_Throws()
        {
            using (Socket socket = new Socket(SocketType.Stream, ProtocolType.Tcp))
            {
                socket.Bind(new DnsEndPoint("loopback", TestPortBase + 52));
            }
        }

        [Fact]
        // "An invalid argument was supplied"
        public void Socket_EnableDualModeAfterBind_Throws()
        {
            try
            {
                using (Socket serverSocket = new Socket(SocketType.Stream, ProtocolType.Tcp))
                {
                    serverSocket.DualMode = false;
                    serverSocket.Bind(new IPEndPoint(IPAddress.IPv6Any, TestPortBase + 18));
                    serverSocket.DualMode = true;
                }
                Assert.Fail("Expected SocketException");
            }
            catch (SocketException)
            {
                // expected
                return;
            }
        }

        #endregion Bind

        #region Accept Sync
        
        [Fact]
        public void AcceptV4BoundToSpecificV4_Success()
        {
            Accept_Helper(IPAddress.Loopback, IPAddress.Loopback);
        }

        [Fact]
        public void AcceptV4BoundToAnyV4_Success()
        {
            Accept_Helper(IPAddress.Any, IPAddress.Loopback);
        }

        [Fact]
        public void AcceptV6BoundToSpecificV6_Success()
        {
            Accept_Helper(IPAddress.IPv6Loopback, IPAddress.IPv6Loopback);
        }

        [Fact]
        public void AcceptV6BoundToAnyV6_Success()
        {
            Accept_Helper(IPAddress.IPv6Any, IPAddress.IPv6Loopback);
        }

        [Fact]
        public void AcceptV6BoundToSpecificV4_CantConnect()
        {
            try
            {
                Accept_Helper(IPAddress.Loopback, IPAddress.IPv6Loopback);
                Assert.Fail("Expected SocketException");
            }
            catch (SocketException)
            {
                // expected
                return;
            }
        }

        [Fact]
        public void AcceptV4BoundToSpecificV6_CantConnect()
        {
            try
            {
                Accept_Helper(IPAddress.IPv6Loopback, IPAddress.Loopback);
                Assert.Fail("Expected SocketException");
            }
            catch (SocketException)
            {
                // expected
                return;
            }
        }

        [Fact]
        public void AcceptV6BoundToAnyV4_CantConnect()
        {
            try
            {
                Accept_Helper(IPAddress.Any, IPAddress.IPv6Loopback);
                Assert.Fail("Expected SocketException");
            }
            catch (SocketException)
            {
                // expected
                return;
            }
        }

        [Fact]
        public void AcceptV4BoundToAnyV6_Success()
        {
            Accept_Helper(IPAddress.IPv6Any, IPAddress.Loopback);
        }
        
        private void Accept_Helper(IPAddress listenOn, IPAddress connectTo)
        {
            using (Socket serverSocket = new Socket(SocketType.Stream, ProtocolType.Tcp))
            {
                serverSocket.Bind(new IPEndPoint(listenOn, TestPortBase + 19));
                serverSocket.Listen(1);
                SocketClient client = new SocketClient(serverSocket, connectTo, TestPortBase + 19);
                Socket clientSocket = serverSocket.Accept();
                Assert.True(clientSocket.Connected);
                Assert.True(clientSocket.DualMode);
                Assert.Equal(AddressFamily.InterNetworkV6, clientSocket.AddressFamily);
            }
        }

        #endregion Accept Sync
        
        #region Accept Begin/End
        
        [Fact]
        public void BeginAcceptV4BoundToSpecificV4_Success()
        {
            DualModeConnect_BeginAccept_Helper(IPAddress.Loopback, IPAddress.Loopback);
        }

        [Fact]
        public void BeginAcceptV4BoundToAnyV4_Success()
        {
            DualModeConnect_BeginAccept_Helper(IPAddress.Any, IPAddress.Loopback);
        }

        [Fact]
        public void BeginAcceptV6BoundToSpecificV6_Success()
        {
            DualModeConnect_BeginAccept_Helper(IPAddress.IPv6Loopback, IPAddress.IPv6Loopback);
        }

        [Fact]
        public void BeginAcceptV6BoundToAnyV6_Success()
        {
            DualModeConnect_BeginAccept_Helper(IPAddress.IPv6Any, IPAddress.IPv6Loopback);
        }

        [Fact]
        public void BeginAcceptV6BoundToSpecificV4_CantConnect()
        {
            try
            {
                DualModeConnect_BeginAccept_Helper(IPAddress.Loopback, IPAddress.IPv6Loopback);
                Assert.Fail("Expected SocketException");
            }
            catch (SocketException)
            {
                // expected
                return;
            }
        }

        [Fact]
        public void BeginAcceptV4BoundToSpecificV6_CantConnect()
        {
            try
            {
                DualModeConnect_BeginAccept_Helper(IPAddress.IPv6Loopback, IPAddress.Loopback);
                Assert.Fail("Expected SocketException");
            }
            catch (SocketException)
            {
                // expected
                return;
            }
        }

        [Fact]
        public void BeginAcceptV6BoundToAnyV4_CantConnect()
        {
            try
            {
                DualModeConnect_BeginAccept_Helper(IPAddress.Any, IPAddress.IPv6Loopback);
                Assert.Fail("Expected SocketException");
            }
            catch (SocketException)
            {
                // expected
                return;
            }
        }

        [Fact]
        public void BeginAcceptV4BoundToAnyV6_Success()
        {
            DualModeConnect_BeginAccept_Helper(IPAddress.IPv6Any, IPAddress.Loopback);
        }

        private void DualModeConnect_BeginAccept_Helper(IPAddress listenOn, IPAddress connectTo)
        {
            using (Socket serverSocket = new Socket(SocketType.Stream, ProtocolType.Tcp))
            {
                serverSocket.Bind(new IPEndPoint(listenOn, TestPortBase + 20));
                serverSocket.Listen(1);
                IAsyncResult async = serverSocket.BeginAccept(null, null);
                SocketClient client = new SocketClient(serverSocket, connectTo, TestPortBase + 20);
                Socket clientSocket = serverSocket.EndAccept(async);
                Assert.True(clientSocket.Connected);
                Assert.True(clientSocket.DualMode);
                Assert.Equal(AddressFamily.InterNetworkV6, clientSocket.AddressFamily);
                Assert.Equal(connectTo.MapToIPv6(), ((IPEndPoint)clientSocket.LocalEndPoint).Address);
            }
        }

        #endregion Accept Begin/End

        #region Accept Async/Event
        
        [Fact]
        public void AcceptAsyncV4BoundToSpecificV4_Success()
        {
            DualModeConnect_AcceptAsync_Helper(IPAddress.Loopback, IPAddress.Loopback, TestPortBase + 41);
        }

        [Fact]
        public void AcceptAsyncV4BoundToAnyV4_Success()
        {
            DualModeConnect_AcceptAsync_Helper(IPAddress.Any, IPAddress.Loopback, TestPortBase + 42);
        }

        [Fact]
        public void AcceptAsyncV6BoundToSpecificV6_Success()
        {
            DualModeConnect_AcceptAsync_Helper(IPAddress.IPv6Loopback, IPAddress.IPv6Loopback, TestPortBase + 43);
        }

        [Fact]
        public void AcceptAsyncV6BoundToAnyV6_Success()
        {
            DualModeConnect_AcceptAsync_Helper(IPAddress.IPv6Any, IPAddress.IPv6Loopback, TestPortBase + 44);
        }

        [Fact]
        public void AcceptAsyncV6BoundToSpecificV4_CantConnect()
        {
            try
            {
                DualModeConnect_AcceptAsync_Helper(IPAddress.Loopback, IPAddress.IPv6Loopback, TestPortBase + 45);
                Assert.Fail("Expected SocketException");
            }
            catch (SocketException)
            {
                // expected
                return;
            }
        }

        [Fact]
        public void AcceptAsyncV4BoundToSpecificV6_CantConnect()
        {
            try
            {
                DualModeConnect_AcceptAsync_Helper(IPAddress.IPv6Loopback, IPAddress.Loopback, TestPortBase + 46);
                Assert.Fail("Expected SocketException");
            }
            catch (SocketException)
            {
                // expected
                return;
            }
        }

        [Fact]
        public void AcceptAsyncV6BoundToAnyV4_CantConnect()
        {
            try
            {
                DualModeConnect_AcceptAsync_Helper(IPAddress.Any, IPAddress.IPv6Loopback, TestPortBase + 47);
                Assert.Fail("Expected SocketException");
            }
            catch (SocketException)
            {
                // expected
                return;
            }
        }

        [Fact]
        public void AcceptAsyncV4BoundToAnyV6_Success()
        {
            DualModeConnect_AcceptAsync_Helper(IPAddress.IPv6Any, IPAddress.Loopback, TestPortBase + 48);
        }

        private void DualModeConnect_AcceptAsync_Helper(IPAddress listenOn, IPAddress connectTo, int port)
        {
            using (Socket serverSocket = new Socket(SocketType.Stream, ProtocolType.Tcp))
            {
                serverSocket.Bind(new IPEndPoint(listenOn, port));
                serverSocket.Listen(1);

                SocketAsyncEventArgs args = new SocketAsyncEventArgs();
                args.Completed += AsyncCompleted;
                ManualResetEvent waitHandle = new ManualResetEvent(false);
                args.UserToken = waitHandle;

                _log.WriteLine(args.GetHashCode() + " SocketAsyncEventArgs with manual event " + waitHandle.GetHashCode());

                serverSocket.AcceptAsync(args);
                SocketClient client = new SocketClient(serverSocket, connectTo, port);
                Assert.True(waitHandle.WaitOne(5000), "Timed out while waiting for connection");

                if (args.SocketError != SocketError.Success)
                {
                    throw new SocketException((int)args.SocketError);
                }

                Socket clientSocket = args.AcceptSocket;
                Assert.NotNull(clientSocket);
                Assert.True(clientSocket.Connected);
                Assert.True(clientSocket.DualMode);
                Assert.Equal(AddressFamily.InterNetworkV6, clientSocket.AddressFamily);
                Assert.Equal(connectTo.MapToIPv6(), ((IPEndPoint)clientSocket.LocalEndPoint).Address);
            }
        }

        #endregion Accept Async/Event

        #endregion Accept

        #region Connectionless
        
        [Fact]
        public void DualModeUdpConstructor_DualModeConfgiured()
        {
            Socket socket = new Socket(SocketType.Dgram, ProtocolType.Udp);
            Assert.Equal(AddressFamily.InterNetworkV6, socket.AddressFamily);
            Assert.True(socket.DualMode);
        }

        #region SendTo

        #region SendTo Sync IPEndPoint

        [Fact] // Base case
        // "The system detected an invalid pointer address in attempting to use a pointer argument in a call"
        public void Socket_SendToV4IPEndPointToV4Host_Throws()
        {
            try
            {
                Socket socket = new Socket(SocketType.Dgram, ProtocolType.Udp);
                socket.DualMode = false;
                socket.SendTo(new byte[1], new IPEndPoint(IPAddress.Loopback, TestPortBase + 40));
                Assert.Fail("Expected SocketException");
            }
            catch (SocketException)
            {
                // expected
                return;
            }
        }

        [Fact] // Base case
        [ExpectedException(typeof(System.ArgumentException))]
        // "The parameter remoteEP must not be of type DnsEndPoint."
        public void Socket_SendToDnsEndPoint_Throws()
        {
            Socket socket = new Socket(SocketType.Dgram, ProtocolType.Udp);
            socket.SendTo(new byte[1], new DnsEndPoint("localhost", TestPortBase + 59));
        }
        
        [Fact]
        public void SendToV4IPEndPointToV4Host_Success()
        {
            DualModeSendTo_IPEndPointToHost_Helper(IPAddress.Loopback, IPAddress.Loopback, false);
        }

        [Fact]
        public void SendToV6IPEndPointToV6Host_Success()
        {
            DualModeSendTo_IPEndPointToHost_Helper(IPAddress.IPv6Loopback, IPAddress.IPv6Loopback, false);
        }

        [Fact]
        public void SendToV4IPEndPointToV6Host_NotReceived()
        {
            try
            {
                DualModeSendTo_IPEndPointToHost_Helper(IPAddress.Loopback, IPAddress.IPv6Loopback, false);
                Assert.Fail("Expected TimeoutException");
            }
            catch (TimeoutException)
            {
                // expected
                return;
            }
        }

        [Fact]
        public void SendToV6IPEndPointToV4Host_NotReceived()
        {
            try
            {
                DualModeSendTo_IPEndPointToHost_Helper(IPAddress.IPv6Loopback, IPAddress.Loopback, false);
                Assert.Fail("Expected TimeoutException");
            }
            catch (TimeoutException)
            {
                // expected
                return;
            }
        }

        [Fact]
        public void SendToV4IPEndPointToDualHost_Success()
        {
            DualModeSendTo_IPEndPointToHost_Helper(IPAddress.Loopback, IPAddress.IPv6Any, true);
        }

        [Fact]
        public void SendToV6IPEndPointToDualHost_Success()
        {
            DualModeSendTo_IPEndPointToHost_Helper(IPAddress.IPv6Loopback, IPAddress.IPv6Any, true);
        }

        private void DualModeSendTo_IPEndPointToHost_Helper(IPAddress connectTo, IPAddress listenOn, bool dualModeServer)
        {
            Socket client = new Socket(SocketType.Dgram, ProtocolType.Udp);
            using (SocketUdpServer server = new SocketUdpServer(listenOn, dualModeServer, TestPortBase + 22))
            {
                int sent = client.SendTo(new byte[1], new IPEndPoint(connectTo, TestPortBase + 22));
                Assert.Equal(1, sent); 

                bool success = server.WaitHandle.WaitOne(500); // Make sure the bytes were received
                if (!success)
                {
                    throw new TimeoutException();
                }
            }
        }

        #endregion SendTo Sync

        #region SendTo Begin/End
        
        [Fact] // Base case
        // "The system detected an invalid pointer address in attempting to use a pointer argument in a call"
        public void Socket_BeginSendToV4IPEndPointToV4Host_Throws()
        {
            try
            {
                Socket socket = new Socket(SocketType.Dgram, ProtocolType.Udp);
                socket.DualMode = false;
                socket.BeginSendTo(new byte[1], 0, 1, SocketFlags.None, new IPEndPoint(IPAddress.Loopback, TestPortBase + 23), 
                    null, null);
                Assert.Fail("Expected SocketException");
            }
            catch (SocketException)
            {
                // expected
                return;
            }
        }

        [Fact] // Base case
        [ExpectedException(typeof(System.ArgumentException))]
        // "The parameter remoteEP must not be of type DnsEndPoint."
        public void Socket_BeginSendToDnsEndPoint_Throws()
        {
            Socket socket = new Socket(SocketType.Dgram, ProtocolType.Udp);
            socket.BeginSendTo(new byte[1], 0, 1, SocketFlags.None, new DnsEndPoint("localhost", TestPortBase + 61), null, null);
        }

        [Fact]
        public void BeginSendToV4IPEndPointToV4Host_Success()
        {
            DualModeBeginSendTo_EndPointToHost_Helper(IPAddress.Loopback, IPAddress.Loopback, false);
        }

        [Fact]
        public void BeginSendToV6IPEndPointToV6Host_Success()
        {
            DualModeBeginSendTo_EndPointToHost_Helper(IPAddress.IPv6Loopback, IPAddress.IPv6Loopback, false);
        }

        [Fact]
        public void BeginSendToV4IPEndPointToV6Host_NotReceived()
        {
            try
            {
                DualModeBeginSendTo_EndPointToHost_Helper(IPAddress.Loopback, IPAddress.IPv6Loopback, false);
                Assert.Fail("Expected TimeoutException");
            }
            catch (TimeoutException)
            {
                // expected
                return;
            }
        }

        [Fact]
        public void BeginSendToV6IPEndPointToV4Host_NotReceived()
        {
            try
            {
                DualModeBeginSendTo_EndPointToHost_Helper(IPAddress.IPv6Loopback, IPAddress.Loopback, false);
                Assert.Fail("Expected TimeoutException");
            }
            catch (TimeoutException)
            {
                // expected
                return;
            }
        }

        [Fact]
        public void BeginSendToV4IPEndPointToDualHost_Success()
        {
            DualModeBeginSendTo_EndPointToHost_Helper(IPAddress.Loopback, IPAddress.IPv6Any, true);
        }

        [Fact]
        public void BeginSendToV6IPEndPointToDualHost_Success()
        {
            DualModeBeginSendTo_EndPointToHost_Helper(IPAddress.IPv6Loopback, IPAddress.IPv6Any, true);
        }

        private void DualModeBeginSendTo_EndPointToHost_Helper(IPAddress connectTo, IPAddress listenOn, bool dualModeServer)
        {
            Socket client = new Socket(SocketType.Dgram, ProtocolType.Udp);
            using (SocketUdpServer server = new SocketUdpServer(listenOn, dualModeServer, TestPortBase + 24))
            {
                IAsyncResult async = client.BeginSendTo(new byte[1], 0, 1, SocketFlags.None,
                    new IPEndPoint(connectTo, TestPortBase + 24), null, null);
                int sent = client.EndSendTo(async);
                Assert.Equal(1, sent);
                bool success = server.WaitHandle.WaitOne(100); // Make sure the bytes were received
                if (!success)
                {
                    throw new TimeoutException();
                }
            }
        }

        #endregion SendTo Begin/End

        #region SendTo Async/Event

        [Fact] // Base case
        public void Socket_SendToAsyncV4IPEndPointToV4Host_Throws()
        {
            Socket socket = new Socket(SocketType.Dgram, ProtocolType.Udp);
            socket.DualMode = false;
            SocketAsyncEventArgs args = new SocketAsyncEventArgs();
            args.RemoteEndPoint = new IPEndPoint(IPAddress.Loopback, TestPortBase + 25);
            args.SetBuffer(new byte[1], 0, 1);
            bool async = socket.SendToAsync(args);
            Assert.False(async);
            Assert.Equal(SocketError.Fault, args.SocketError);
        }

        [Fact] // Base case
        [ExpectedException(typeof(System.ArgumentException))]
        // "The parameter remoteEP must not be of type DnsEndPoint."
        public void Socket_SendToAsyncDnsEndPoint_Throws()
        {
            Socket socket = new Socket(SocketType.Dgram, ProtocolType.Udp);
            SocketAsyncEventArgs args = new SocketAsyncEventArgs();
            args.RemoteEndPoint = new DnsEndPoint("localhost", TestPortBase + 53);
            args.SetBuffer(new byte[1], 0, 1);
            socket.SendToAsync(args);
        }

        [Fact]
        public void SendToAsyncV4IPEndPointToV4Host_Success()
        {
            DualModeSendToAsync_IPEndPointToHost_Helper(IPAddress.Loopback, IPAddress.Loopback, false);
        }

        [Fact]
        public void SendToAsyncV6IPEndPointToV6Host_Success()
        {
            DualModeSendToAsync_IPEndPointToHost_Helper(IPAddress.IPv6Loopback, IPAddress.IPv6Loopback, false);
        }

        [Fact]
        public void SendToAsyncV4IPEndPointToV6Host_NotReceived()
        {
            try
            {
                DualModeSendToAsync_IPEndPointToHost_Helper(IPAddress.Loopback, IPAddress.IPv6Loopback, false);
                Assert.Fail("Expected TimeoutException");
            }
            catch (TimeoutException)
            {
                // expected
                return;
            }
        }

        [Fact]
        public void SendToAsyncV6IPEndPointToV4Host_NotReceived()
        {
            try
            {
                DualModeSendToAsync_IPEndPointToHost_Helper(IPAddress.IPv6Loopback, IPAddress.Loopback, false);
                Assert.Fail("Expected TimeoutException");
            }
            catch (TimeoutException)
            {
                // expected
                return;
            }
        }

        [Fact]
        public void SendToAsyncV4IPEndPointToDualHost_Success()
        {
            DualModeSendToAsync_IPEndPointToHost_Helper(IPAddress.Loopback, IPAddress.IPv6Any, true);
        }

        [Fact]
        public void SendToAsyncV6IPEndPointToDualHost_Success()
        {
            DualModeSendToAsync_IPEndPointToHost_Helper(IPAddress.IPv6Loopback, IPAddress.IPv6Any, true);
        }

        private void DualModeSendToAsync_IPEndPointToHost_Helper(IPAddress connectTo, IPAddress listenOn, bool dualModeServer)
        {
            ManualResetEvent waitHandle = new ManualResetEvent(false);
            Socket client = new Socket(SocketType.Dgram, ProtocolType.Udp);
            using (SocketUdpServer server = new SocketUdpServer(listenOn, dualModeServer, TestPortBase + 26))
            {
                SocketAsyncEventArgs args = new SocketAsyncEventArgs();
                args.RemoteEndPoint = new IPEndPoint(connectTo, TestPortBase + 26);
                args.SetBuffer(new byte[1], 0, 1);
                args.UserToken = waitHandle;
                args.Completed += AsyncCompleted;

                bool async = client.SendToAsync(args);
                if (async)
                {
                    Assert.True(waitHandle.WaitOne(5000), "Timeout while waiting for connection");
                }
                Assert.Equal(1, args.BytesTransferred);
                if (args.SocketError != SocketError.Success)
                {
                    throw new SocketException((int)args.SocketError);
                }

                bool success = server.WaitHandle.WaitOne(100); // Make sure the bytes were received
                if (!success)
                {
                    throw new TimeoutException();
                }
            }
        }

        #endregion SendTo Async/Event

        #endregion SendTo

        #region ReceiveFrom

        #region ReceiveFrom Sync
        
        [Fact] // Base case
        // "The supplied EndPoint of AddressFamily InterNetwork is not valid for this Socket, use InterNetworkV6 instead."
        public void Socket_ReceiveFromV4IPEndPointFromV4Client_Throws()
        {
            try
            {
                Socket socket = new Socket(SocketType.Dgram, ProtocolType.Udp);
                socket.DualMode = false;
                EndPoint receivedFrom = new IPEndPoint(IPAddress.Loopback, TestPortBase + 27);
                int received = socket.ReceiveFrom(new byte[1], ref receivedFrom);
                Assert.Fail("Expected ArgumentException");
            }
            catch (ArgumentException)
            {
                // expected
                return;
            }
        }

        [Fact] // Base case
        [ExpectedException(typeof(System.ArgumentException))]
        // "The parameter remoteEP must not be of type DnsEndPoint."
        public void Socket_ReceiveFromDnsEndPoint_Throws()
        {
            using (Socket socket = new Socket(SocketType.Dgram, ProtocolType.Udp))
            {
                EndPoint receivedFrom = new DnsEndPoint("localhost", TestPortBase + 54, AddressFamily.InterNetworkV6);
                socket.Bind(new IPEndPoint(IPAddress.IPv6Loopback, TestPortBase + 54));
                int received = socket.ReceiveFrom(new byte[1], ref receivedFrom);
            }
        }

        [Fact]
        public void ReceiveFromV4BoundToSpecificV4_Success()
        {
            ReceiveFrom_Helper(IPAddress.Loopback, IPAddress.Loopback);
        }

        [Fact]
        public void ReceiveFromV4BoundToAnyV4_Success()
        {
            ReceiveFrom_Helper(IPAddress.Any, IPAddress.Loopback);
        }

        [Fact]
        public void ReceiveFromV6BoundToSpecificV6_Success()
        {
            ReceiveFrom_Helper(IPAddress.IPv6Loopback, IPAddress.IPv6Loopback);
        }

        [Fact]
        public void ReceiveFromV6BoundToAnyV6_Success()
        {
            ReceiveFrom_Helper(IPAddress.IPv6Any, IPAddress.IPv6Loopback);
        }

        [Fact]
        public void ReceiveFromV6BoundToSpecificV4_NotReceived()
        {
            try
            {
                ReceiveFrom_Helper(IPAddress.Loopback, IPAddress.IPv6Loopback);
                Assert.Fail("Expected SocketException");
            }
            catch (SocketException)
            {
                // expected
                return;
            }
        }

        [Fact]
        public void ReceiveFromV4BoundToSpecificV6_NotReceived()
        {
            try
            {
                ReceiveFrom_Helper(IPAddress.IPv6Loopback, IPAddress.Loopback);
                Assert.Fail("Expected SocketException");
            }
            catch (SocketException)
            {
                // expected
                return;
            }
        }

        [Fact]
        public void ReceiveFromV6BoundToAnyV4_NotReceived()
        {
            try
            {
                ReceiveFrom_Helper(IPAddress.Any, IPAddress.IPv6Loopback);
                Assert.Fail("Expected SocketException");
            }
            catch (SocketException)
            {
                // expected
                return;
            }
        }

        [Fact]
        public void ReceiveFromV4BoundToAnyV6_Success()
        {
            ReceiveFrom_Helper(IPAddress.IPv6Any, IPAddress.Loopback);
        }

        private void ReceiveFrom_Helper(IPAddress listenOn, IPAddress connectTo)
        {
            using (Socket serverSocket = new Socket(SocketType.Dgram, ProtocolType.Udp))
            {
                serverSocket.ReceiveTimeout = 500;
                serverSocket.Bind(new IPEndPoint(listenOn, TestPortBase + 28));
                SocketUdpClient client = new SocketUdpClient(serverSocket, connectTo, TestPortBase + 28);

                EndPoint receivedFrom = new IPEndPoint(connectTo, TestPortBase + 28);
                int received = serverSocket.ReceiveFrom(new byte[1], ref receivedFrom);

                Assert.Equal(1, received);
                Assert.Equal<Type>(receivedFrom.GetType(), typeof(IPEndPoint));
                IPEndPoint remoteEndPoint = receivedFrom as IPEndPoint;
                Assert.Equal(AddressFamily.InterNetworkV6, remoteEndPoint.AddressFamily);
                Assert.Equal(connectTo.MapToIPv6(), remoteEndPoint.Address);
            }
        }

        #endregion ReceiveFrom Sync

        #region ReceiveFrom Begin/End
        
        [Fact] // Base case
        // "The supplied EndPoint of AddressFamily InterNetwork is not valid for this Socket, use InterNetworkV6 instead."
        public void Socket_BeginReceiveFromV4IPEndPointFromV4Client_Throws()
        {
            try
            {
                Socket socket = new Socket(SocketType.Dgram, ProtocolType.Udp);
                socket.DualMode = false;
                EndPoint receivedFrom = new IPEndPoint(IPAddress.Loopback, TestPortBase + 29);
                socket.BeginReceiveFrom(new byte[1], 0, 1, SocketFlags.None, ref receivedFrom, null, null);
                Assert.Fail("Expected ArgumentException");
            }
            catch (ArgumentException)
            {
                // expected
                return;
            }
        }

        [Fact] // Base case
        [ExpectedException(typeof(System.ArgumentException))]
        // "The parameter remoteEP must not be of type DnsEndPoint."
        public void Socket_BeginReceiveFromDnsEndPoint_Throws()
        {
            using (Socket socket = new Socket(SocketType.Dgram, ProtocolType.Udp))
            {
                EndPoint receivedFrom = new DnsEndPoint("localhost", TestPortBase + 55, AddressFamily.InterNetworkV6);
                socket.Bind(new IPEndPoint(IPAddress.IPv6Loopback, TestPortBase + 55));
                socket.BeginReceiveFrom(new byte[1], 0, 1, SocketFlags.None, ref receivedFrom, null, null);
            }
        }

        [Fact]
        public void BeginReceiveFromV4BoundToSpecificV4_Success()
        {
            BeginReceiveFrom_Helper(IPAddress.Loopback, IPAddress.Loopback);
        }

        [Fact]
        public void BeginReceiveFromV4BoundToAnyV4_Success()
        {
            BeginReceiveFrom_Helper(IPAddress.Any, IPAddress.Loopback);
        }

        [Fact]
        public void BeginReceiveFromV6BoundToSpecificV6_Success()
        {
            BeginReceiveFrom_Helper(IPAddress.IPv6Loopback, IPAddress.IPv6Loopback);
        }

        [Fact]
        public void BeginReceiveFromV6BoundToAnyV6_Success()
        {
            BeginReceiveFrom_Helper(IPAddress.IPv6Any, IPAddress.IPv6Loopback);
        }

        [Fact]
        public void BeginReceiveFromV6BoundToSpecificV4_NotReceived()
        {
            try
            {
                BeginReceiveFrom_Helper(IPAddress.Loopback, IPAddress.IPv6Loopback);
                Assert.Fail("Expected TimeoutException");
            }
            catch (TimeoutException)
            {
                // expected
                return;
            }
        }

        [Fact]
        public void BeginReceiveFromV4BoundToSpecificV6_NotReceived()
        {
            try
            {
                BeginReceiveFrom_Helper(IPAddress.IPv6Loopback, IPAddress.Loopback);
                Assert.Fail("Expected TimeoutException");
            }
            catch (TimeoutException)
            {
                // expected
                return;
            }
        }

        [Fact]
        public void BeginReceiveFromV6BoundToAnyV4_NotReceived()
        {
            try
            {
                BeginReceiveFrom_Helper(IPAddress.Any, IPAddress.IPv6Loopback);
                Assert.Fail("Expected TimeoutException");
            }
            catch (TimeoutException)
            {
                // expected
                return;
            }
        }

        [Fact]
        public void BeginReceiveFromV4BoundToAnyV6_Success()
        {
            BeginReceiveFrom_Helper(IPAddress.IPv6Any, IPAddress.Loopback);
        }

        private void BeginReceiveFrom_Helper(IPAddress listenOn, IPAddress connectTo)
        {
            using (Socket serverSocket = new Socket(SocketType.Dgram, ProtocolType.Udp))
            {
                serverSocket.ReceiveTimeout = 500;
                serverSocket.Bind(new IPEndPoint(listenOn, TestPortBase + 30));
                EndPoint receivedFrom = new IPEndPoint(connectTo, TestPortBase + 30);
                IAsyncResult async = serverSocket.BeginReceiveFrom(new byte[1], 0, 1, SocketFlags.None, 
                    ref receivedFrom, null, null);

                IPEndPoint remoteEndPoint = receivedFrom as IPEndPoint;
                Assert.Equal(AddressFamily.InterNetworkV6, remoteEndPoint.AddressFamily);
                Assert.Equal(connectTo.MapToIPv6(), remoteEndPoint.Address);

                SocketUdpClient client = new SocketUdpClient(serverSocket, connectTo, TestPortBase + 30);
                bool success = async.AsyncWaitHandle.WaitOne(500);
                if (!success)
                {
                    throw new TimeoutException();
                }

                receivedFrom = new IPEndPoint(connectTo, TestPortBase + 30);
                int received = serverSocket.EndReceiveFrom(async, ref receivedFrom);

                Assert.Equal(1, received);
                Assert.Equal<Type>(receivedFrom.GetType(), typeof(IPEndPoint));
                remoteEndPoint = receivedFrom as IPEndPoint;
                Assert.Equal(AddressFamily.InterNetworkV6, remoteEndPoint.AddressFamily);
                Assert.Equal(connectTo.MapToIPv6(), remoteEndPoint.Address);
            }
        }

        #endregion ReceiveFrom Begin/End

        #region ReceiveFrom Async/Event

        [Fact] // Base case
        // "The supplied EndPoint of AddressFamily InterNetwork is not valid for this Socket, use InterNetworkV6 instead."
        public void Socket_ReceiveFromAsyncV4IPEndPointFromV4Client_Throws()
        {
            try
            {
                Socket socket = new Socket(SocketType.Dgram, ProtocolType.Udp);
                socket.DualMode = false;
                SocketAsyncEventArgs args = new SocketAsyncEventArgs();
                args.RemoteEndPoint = new IPEndPoint(IPAddress.Loopback, TestPortBase + 31);
                args.SetBuffer(new byte[1], 0, 1);
                socket.ReceiveFromAsync(args);
                Assert.Fail("Expected ArgumentException");
            }
            catch (ArgumentException)
            {
                // expected
                return;
            }
        }

        [Fact] // Base case
        [ExpectedException(typeof(System.ArgumentException))]
        // "The parameter remoteEP must not be of type DnsEndPoint."
        public void Socket_ReceiveFromAsyncDnsEndPoint_Throws()
        {
            using (Socket socket = new Socket(SocketType.Dgram, ProtocolType.Udp))
            {
                socket.Bind(new IPEndPoint(IPAddress.IPv6Loopback, TestPortBase + 56));
                SocketAsyncEventArgs args = new SocketAsyncEventArgs();
                args.RemoteEndPoint = new DnsEndPoint("localhost", TestPortBase + 56, AddressFamily.InterNetworkV6);
                args.SetBuffer(new byte[1], 0, 1);
                socket.ReceiveFromAsync(args);
            }
        }

        [Fact]
        public void ReceiveFromAsyncV4BoundToSpecificV4_Success()
        {
            ReceiveFromAsync_Helper(IPAddress.Loopback, IPAddress.Loopback);
        }

        [Fact]
        public void ReceiveFromAsyncV4BoundToAnyV4_Success()
        {
            ReceiveFromAsync_Helper(IPAddress.Any, IPAddress.Loopback);
        }

        [Fact]
        public void ReceiveFromAsyncV6BoundToSpecificV6_Success()
        {
            ReceiveFromAsync_Helper(IPAddress.IPv6Loopback, IPAddress.IPv6Loopback);
        }

        [Fact]
        public void ReceiveFromAsyncV6BoundToAnyV6_Success()
        {
            ReceiveFromAsync_Helper(IPAddress.IPv6Any, IPAddress.IPv6Loopback);
        }

        [Fact]
        public void ReceiveFromAsyncV6BoundToSpecificV4_NotReceived()
        {
            try
            {
                ReceiveFromAsync_Helper(IPAddress.Loopback, IPAddress.IPv6Loopback);
                Assert.Fail("Expected TimeoutException");
            }
            catch (TimeoutException)
            {
                // expected
                return;
            }
        }

        [Fact]
        public void ReceiveFromAsyncV4BoundToSpecificV6_NotReceived()
        {
            try
            {
                ReceiveFromAsync_Helper(IPAddress.IPv6Loopback, IPAddress.Loopback);
                Assert.Fail("Expected TimeoutException");
            }
            catch (TimeoutException)
            {
                // expected
                return;
            }
        }

        [Fact]
        public void ReceiveFromAsyncV6BoundToAnyV4_NotReceived()
        {
            try
            {
                ReceiveFromAsync_Helper(IPAddress.Any, IPAddress.IPv6Loopback);
                Assert.Fail("Expected TimeoutException");
            }
            catch (TimeoutException)
            {
                // expected
                return;
            }
        }

        [Fact]
        public void ReceiveFromAsyncV4BoundToAnyV6_Success()
        {
            ReceiveFromAsync_Helper(IPAddress.IPv6Any, IPAddress.Loopback);
        }

        private void ReceiveFromAsync_Helper(IPAddress listenOn, IPAddress connectTo)
        {
            using (Socket serverSocket = new Socket(SocketType.Dgram, ProtocolType.Udp))
            {
                serverSocket.Bind(new IPEndPoint(listenOn, TestPortBase + 32));

                SocketAsyncEventArgs args = new SocketAsyncEventArgs();
                args.RemoteEndPoint = new IPEndPoint(listenOn, TestPortBase + 32);
                args.SetBuffer(new byte[1], 0, 1);
                ManualResetEvent waitHandle = new ManualResetEvent(false);
                args.UserToken = waitHandle;
                args.Completed += AsyncCompleted;

                bool async = serverSocket.ReceiveFromAsync(args);
                SocketUdpClient client = new SocketUdpClient(serverSocket, connectTo, TestPortBase + 32);
                if (async && !waitHandle.WaitOne(200))
                {
                    throw new TimeoutException();
                }

                if (args.SocketError != SocketError.Success)
                {
                    throw new SocketException((int)args.SocketError);
                }

                Assert.Equal(1, args.BytesTransferred);
                Assert.Equal<Type>(args.RemoteEndPoint.GetType(), typeof(IPEndPoint));
                IPEndPoint remoteEndPoint = args.RemoteEndPoint as IPEndPoint;
                Assert.Equal(AddressFamily.InterNetworkV6, remoteEndPoint.AddressFamily);
                Assert.Equal(connectTo.MapToIPv6(), remoteEndPoint.Address);
            }
        }

        #endregion ReceiveFrom Async/Event
        
        #endregion ReceiveFrom

        #region ReceiveMessageFrom

        #region ReceiveMessageFrom Sync

        [Fact] // Base case
        // "The supplied EndPoint of AddressFamily InterNetwork is not valid for this Socket, use InterNetworkV6 instead."
        public void Socket_ReceiveMessageFromV4IPEndPointFromV4Client_Throws()
        {
            try
            {
                Socket socket = new Socket(SocketType.Dgram, ProtocolType.Udp);
                socket.DualMode = false;
                EndPoint receivedFrom = new IPEndPoint(IPAddress.Loopback, TestPortBase + 33);
                SocketFlags socketFlags = SocketFlags.None;
                IPPacketInformation ipPacketInformation;
                int received = socket.ReceiveMessageFrom(new byte[1], 0, 1, ref socketFlags, ref receivedFrom, 
                    out ipPacketInformation);
                Assert.Fail("Expected ArgumentException");
            }
            catch (ArgumentException)
            {
                // expected
                return;
            }
        }

        [Fact] // Base case
        [ExpectedException(typeof(System.ArgumentException))]
        // "The parameter remoteEP must not be of type DnsEndPoint."
        public void Socket_ReceiveMessageFromDnsEndPoint_Throws()
        {
            using (Socket socket = new Socket(SocketType.Dgram, ProtocolType.Udp))
            {
                EndPoint receivedFrom = new DnsEndPoint("localhost", TestPortBase + 60, AddressFamily.InterNetworkV6);
                socket.Bind(new IPEndPoint(IPAddress.IPv6Loopback, TestPortBase + 60));
                SocketFlags socketFlags = SocketFlags.None;
                IPPacketInformation ipPacketInformation;
                int received = socket.ReceiveMessageFrom(new byte[1], 0, 1, ref socketFlags, ref receivedFrom,
                    out ipPacketInformation);
            }
        }

        [Fact] // Base case
        public void ReceiveMessageFromV4BoundToSpecificMappedV4_Success()
        {
            ReceiveMessageFrom_Helper(IPAddress.Loopback.MapToIPv6(), IPAddress.Loopback);
        }

        [Fact] // Base case
        public void ReceiveMessageFromV4BoundToAnyMappedV4_Success()
        {
            ReceiveMessageFrom_Helper(IPAddress.Any.MapToIPv6(), IPAddress.Loopback);
        }

        [Fact]
        public void ReceiveMessageFromV4BoundToSpecificV4_Success()
        {
            ReceiveMessageFrom_Helper(IPAddress.Loopback, IPAddress.Loopback);
        }

        [Fact]
        public void ReceiveMessageFromV4BoundToAnyV4_Success()
        {
            ReceiveMessageFrom_Helper(IPAddress.Any, IPAddress.Loopback);
        }

        [Fact]
        public void ReceiveMessageFromV6BoundToSpecificV6_Success()
        {
            ReceiveMessageFrom_Helper(IPAddress.IPv6Loopback, IPAddress.IPv6Loopback);
        }

        [Fact]
        public void ReceiveMessageFromV6BoundToAnyV6_Success()
        {
            ReceiveMessageFrom_Helper(IPAddress.IPv6Any, IPAddress.IPv6Loopback);
        }

        [Fact]
        public void ReceiveMessageFromV6BoundToSpecificV4_NotReceived()
        {
            try
            {
                ReceiveMessageFrom_Helper(IPAddress.Loopback, IPAddress.IPv6Loopback);
                Assert.Fail("Expected SocketException");
            }
            catch (SocketException)
            {
                // expected
                return;
            }
        }

        [Fact]
        public void ReceiveMessageFromV4BoundToSpecificV6_NotReceived()
        {
            try
            {
                ReceiveMessageFrom_Helper(IPAddress.IPv6Loopback, IPAddress.Loopback);
                Assert.Fail("Expected SocketException");
            }
            catch (SocketException)
            {
                // expected
                return;
            }
        }

        [Fact]
        public void ReceiveMessageFromV6BoundToAnyV4_NotReceived()
        {
            try
            {
                ReceiveMessageFrom_Helper(IPAddress.Any, IPAddress.IPv6Loopback);
                Assert.Fail("Expected SocketException");
            }
            catch (SocketException)
            {
                // expected
                return;
            }
        }

        [Fact]
        public void ReceiveMessageFromV4BoundToAnyV6_Success()
        {
            ReceiveMessageFrom_Helper(IPAddress.IPv6Any, IPAddress.Loopback);
        }

        private void ReceiveMessageFrom_Helper(IPAddress listenOn, IPAddress connectTo)
        {
            using (Socket serverSocket = new Socket(SocketType.Dgram, ProtocolType.Udp))
            {
                serverSocket.ReceiveTimeout = 500;
                serverSocket.Bind(new IPEndPoint(listenOn, TestPortBase + 34));

                EndPoint receivedFrom = new IPEndPoint(connectTo, TestPortBase + 34);
                SocketFlags socketFlags = SocketFlags.None;
                IPPacketInformation ipPacketInformation;
                int received = 0;

                try
                {
                    // This is a false start.  See the comments below.
                    received = serverSocket.ReceiveMessageFrom(new byte[1], 0, 1, ref socketFlags, ref receivedFrom,
                        out ipPacketInformation);
                    Assert.Fail(); // Timeout Expected
                }
                catch (SocketException)
                {
                    // http://msdn.microsoft.com/en-us/library/system.net.sockets.socket.receivemessagefrom.aspx
                    // "...the returned IPPacketInformation object will only be valid for packets which arrive at the 
                    // local computer after the socket option has been set. If a socket is sent packets between when 
                    // it is bound to a local endpoint (explicitly by the Bind method or implicitly by one of the Connect, 
                    // ConnectAsync, SendTo, or SendToAsync methods) and its first call to the ReceiveMessageFrom method, 
                    // calls to ReceiveMessageFrom method will return invalid IPPacketInformation objects for these packets."
                }

                SocketUdpClient client = new SocketUdpClient(serverSocket, connectTo, TestPortBase + 34);

                receivedFrom = new IPEndPoint(connectTo, TestPortBase + 34);
                socketFlags = SocketFlags.None;
                received = serverSocket.ReceiveMessageFrom(new byte[1], 0, 1, ref socketFlags, ref receivedFrom,
                    out ipPacketInformation);

                Assert.Equal(1, received);
                Assert.Equal<Type>(receivedFrom.GetType(), typeof(IPEndPoint));
                IPEndPoint remoteEndPoint = receivedFrom as IPEndPoint;
                Assert.Equal(AddressFamily.InterNetworkV6, remoteEndPoint.AddressFamily);
                Assert.Equal(connectTo.MapToIPv6(), remoteEndPoint.Address);

                Assert.Equal(SocketFlags.None, socketFlags);
                Assert.NotNull(ipPacketInformation);

                Assert.Equal(connectTo, ipPacketInformation.Address);
                Assert.Equal(NetworkInterface.IPv6LoopbackInterfaceIndex, ipPacketInformation.Interface);
            }
        }

        #endregion ReceiveMessageFrom Sync

        #region ReceiveMessageFrom Begin/End

        [Fact] // Base case
        // "The supplied EndPoint of AddressFamily InterNetwork is not valid for this Socket, use InterNetworkV6 instead."
        public void Socket_BeginReceiveMessageFromV4IPEndPointFromV4Client_Throws()
        {
            try
            {
                Socket socket = new Socket(SocketType.Dgram, ProtocolType.Udp);
                socket.DualMode = false;
                EndPoint receivedFrom = new IPEndPoint(IPAddress.Loopback, TestPortBase + 35);
                SocketFlags socketFlags = SocketFlags.None;
                socket.BeginReceiveMessageFrom(new byte[1], 0, 1, socketFlags, ref receivedFrom, null, null);
                Assert.Fail("Expected ArgumentException");
            }
            catch (ArgumentException)
            {
                // expected
                return;
            }
        }

        [Fact] // Base case
        [ExpectedException(typeof(System.ArgumentException))]
        // "The parameter remoteEP must not be of type DnsEndPoint."
        public void Socket_BeginReceiveMessageFromDnsEndPoint_Throws()
        {
            using (Socket socket = new Socket(SocketType.Dgram, ProtocolType.Udp))
            {
                EndPoint receivedFrom = new DnsEndPoint("localhost", TestPortBase + 57, AddressFamily.InterNetworkV6);
                socket.Bind(new IPEndPoint(IPAddress.IPv6Loopback, TestPortBase + 57));
                SocketFlags socketFlags = SocketFlags.None;
                socket.BeginReceiveMessageFrom(new byte[1], 0, 1, socketFlags, ref receivedFrom, null, null);
            }
        }

        [Fact] // Base case
        public void BeginReceiveMessageFromV4BoundToSpecificMappedV4_Success()
        {
            BeginReceiveMessageFrom_Helper(IPAddress.Loopback.MapToIPv6(), IPAddress.Loopback);
        }

        [Fact] // Base case
        public void BeginReceiveMessageFromV4BoundToAnyMappedV4_Success()
        {
            BeginReceiveMessageFrom_Helper(IPAddress.Any.MapToIPv6(), IPAddress.Loopback);
        }

        [Fact]
        public void BeginReceiveMessageFromV4BoundToSpecificV4_Success()
        {
            BeginReceiveMessageFrom_Helper(IPAddress.Loopback, IPAddress.Loopback);
        }

        [Fact]
        public void BeginReceiveMessageFromV4BoundToAnyV4_Success()
        {
            BeginReceiveMessageFrom_Helper(IPAddress.Any, IPAddress.Loopback);
        }

        [Fact]
        public void BeginReceiveMessageFromV6BoundToSpecificV6_Success()
        {
            BeginReceiveMessageFrom_Helper(IPAddress.IPv6Loopback, IPAddress.IPv6Loopback);
        }

        [Fact]
        public void BeginReceiveMessageFromV6BoundToAnyV6_Success()
        {
            BeginReceiveMessageFrom_Helper(IPAddress.IPv6Any, IPAddress.IPv6Loopback);
        }

        [Fact]
        public void BeginReceiveMessageFromV6BoundToSpecificV4_NotReceived()
        {
            try
            {
                BeginReceiveMessageFrom_Helper(IPAddress.Loopback, IPAddress.IPv6Loopback);
                Assert.Fail("Expected TimeoutException");
            }
            catch (TimeoutException)
            {
                // expected
                return;
            }
        }

        [Fact]
        public void BeginReceiveMessageFromV4BoundToSpecificV6_NotReceived()
        {
            try
            {
                BeginReceiveMessageFrom_Helper(IPAddress.IPv6Loopback, IPAddress.Loopback);
                Assert.Fail("Expected TimeoutException");
            }
            catch (TimeoutException)
            {
                // expected
                return;
            }
        }

        [Fact]
        public void BeginReceiveMessageFromV6BoundToAnyV4_NotReceived()
        {
            try
            {
                BeginReceiveMessageFrom_Helper(IPAddress.Any, IPAddress.IPv6Loopback);
                Assert.Fail("Expected TimeoutException");
            }
            catch (TimeoutException)
            {
                // expected
                return;
            }
        }

        [Fact]
        public void BeginReceiveMessageFromV4BoundToAnyV6_Success()
        {
            BeginReceiveMessageFrom_Helper(IPAddress.IPv6Any, IPAddress.Loopback);
        }

        private void BeginReceiveMessageFrom_Helper(IPAddress listenOn, IPAddress connectTo)
        {
            using (Socket serverSocket = new Socket(SocketType.Dgram, ProtocolType.Udp))
            {
                serverSocket.Bind(new IPEndPoint(listenOn, TestPortBase + 36));
                EndPoint receivedFrom = new IPEndPoint(connectTo, TestPortBase + 36);
                SocketFlags socketFlags = SocketFlags.None;
                IPPacketInformation ipPacketInformation;
                IAsyncResult async = serverSocket.BeginReceiveMessageFrom(new byte[1], 0, 1, socketFlags,
                    ref receivedFrom, null, null);
                IPEndPoint remoteEndPoint = receivedFrom as IPEndPoint;
                Assert.Equal(AddressFamily.InterNetworkV6, remoteEndPoint.AddressFamily);
                Assert.Equal(connectTo.MapToIPv6(), remoteEndPoint.Address);

                SocketUdpClient client = new SocketUdpClient(serverSocket, connectTo, TestPortBase + 36);
                bool success = async.AsyncWaitHandle.WaitOne(500);
                if (!success)
                {
                    throw new TimeoutException();
                }
                receivedFrom = new IPEndPoint(connectTo, TestPortBase + 36);
                int received = serverSocket.EndReceiveMessageFrom(async, ref socketFlags, ref receivedFrom, out ipPacketInformation);

                Assert.Equal(1, received);
                Assert.Equal<Type>(receivedFrom.GetType(), typeof(IPEndPoint));
                remoteEndPoint = receivedFrom as IPEndPoint;
                Assert.Equal(AddressFamily.InterNetworkV6, remoteEndPoint.AddressFamily);
                Assert.Equal(connectTo.MapToIPv6(), remoteEndPoint.Address);

                Assert.Equal(SocketFlags.None, socketFlags);
                Assert.NotNull(ipPacketInformation);
                Assert.Equal(connectTo, ipPacketInformation.Address);
                
                // TODO: Move to NetworkInformation tests.
                //Assert.Equal(NetworkInterface.IPv6LoopbackInterfaceIndex, ipPacketInformation.Interface);
            }
        }

        #endregion ReceiveMessageFrom Begin/End
        
        #region ReceiveMessageFrom Async/Event

        [Fact] // Base case
        // "The supplied EndPoint of AddressFamily InterNetwork is not valid for this Socket, use InterNetworkV6 instead."
        public void Socket_ReceiveMessageFromAsyncV4IPEndPointFromV4Client_Throws()
        {
            try
            {
                Socket socket = new Socket(SocketType.Dgram, ProtocolType.Udp);
                socket.DualMode = false;
                SocketAsyncEventArgs args = new SocketAsyncEventArgs();
                args.RemoteEndPoint = new IPEndPoint(IPAddress.Loopback, TestPortBase + 37);
                args.SetBuffer(new byte[1], 0, 1);
                socket.ReceiveMessageFromAsync(args);
                Assert.Fail("Expected ArgumentException");
            }
            catch (ArgumentException)
            {
                // expected
                return;
            }
        }

        [Fact] // Base case
        [ExpectedException(typeof(System.ArgumentException))]
        // "The parameter remoteEP must not be of type DnsEndPoint."
        public void Socket_ReceiveMessageFromAsyncDnsEndPoint_Throws()
        {
            using (Socket socket = new Socket(SocketType.Dgram, ProtocolType.Udp))
            {
                socket.Bind(new IPEndPoint(IPAddress.IPv6Loopback, TestPortBase + 58));
                SocketAsyncEventArgs args = new SocketAsyncEventArgs();
                args.RemoteEndPoint = new DnsEndPoint("localhost", TestPortBase + 58, AddressFamily.InterNetworkV6);
                args.SetBuffer(new byte[1], 0, 1);
                socket.ReceiveMessageFromAsync(args);
            }
        }

        [Fact] // Base case
        public void ReceiveMessageFromAsyncV4BoundToSpecificMappedV4_Success()
        {
            ReceiveMessageFromAsync_Helper(IPAddress.Loopback.MapToIPv6(), IPAddress.Loopback);
        }

        [Fact] // Base case
        public void ReceiveMessageFromAsyncV4BoundToAnyMappedV4_Success()
        {
            ReceiveMessageFromAsync_Helper(IPAddress.Any.MapToIPv6(), IPAddress.Loopback);
        }

        [Fact]
        public void ReceiveMessageFromAsyncV4BoundToSpecificV4_Success()
        {
            ReceiveMessageFromAsync_Helper(IPAddress.Loopback, IPAddress.Loopback);
        }

        [Fact]
        public void ReceiveMessageFromAsyncV4BoundToAnyV4_Success()
        {
            ReceiveMessageFromAsync_Helper(IPAddress.Any, IPAddress.Loopback);
        }

        [Fact]
        public void ReceiveMessageFromAsyncV6BoundToSpecificV6_Success()
        {
            ReceiveMessageFromAsync_Helper(IPAddress.IPv6Loopback, IPAddress.IPv6Loopback);
        }

        [Fact]
        public void ReceiveMessageFromAsyncV6BoundToAnyV6_Success()
        {
            ReceiveMessageFromAsync_Helper(IPAddress.IPv6Any, IPAddress.IPv6Loopback);
        }

        [Fact]
        public void ReceiveMessageFromAsyncV6BoundToSpecificV4_NotReceived()
        {
            try
            {
                ReceiveMessageFromAsync_Helper(IPAddress.Loopback, IPAddress.IPv6Loopback);
                Assert.Fail("Expected TimeoutException");
            }
            catch (TimeoutException)
            {
                // expected
                return;
            }
        }

        [Fact]
        public void ReceiveMessageFromAsyncV4BoundToSpecificV6_NotReceived()
        {
            try
            {
                ReceiveMessageFromAsync_Helper(IPAddress.IPv6Loopback, IPAddress.Loopback);
                Assert.Fail("Expected TimeoutException");
            }
            catch (TimeoutException)
            {
                // expected
                return;
            }
        }

        [Fact]
        public void ReceiveMessageFromAsyncV6BoundToAnyV4_NotReceived()
        {
            try
            {
                ReceiveMessageFromAsync_Helper(IPAddress.Any, IPAddress.IPv6Loopback);
                Assert.Fail("Expected TimeoutException");
            }
            catch (TimeoutException)
            {
                // expected
                return;
            }
        }

        [Fact]
        public void ReceiveMessageFromAsyncV4BoundToAnyV6_Success()
        {
            ReceiveMessageFromAsync_Helper(IPAddress.IPv6Any, IPAddress.Loopback);
        }

        private void ReceiveMessageFromAsync_Helper(IPAddress listenOn, IPAddress connectTo)
        {
            using (Socket serverSocket = new Socket(SocketType.Dgram, ProtocolType.Udp))
            {
                serverSocket.ReceiveTimeout = 500;
                serverSocket.Bind(new IPEndPoint(listenOn, TestPortBase + 38));

                SocketAsyncEventArgs args = new SocketAsyncEventArgs();
                args.RemoteEndPoint = new IPEndPoint(connectTo, TestPortBase + 38);
                args.SetBuffer(new byte[1], 0, 1);
                args.Completed += AsyncCompleted;
                ManualResetEvent waitHandle = new ManualResetEvent(false);
                args.UserToken = waitHandle;
                bool async = serverSocket.ReceiveMessageFromAsync(args);
                Assert.True(async);

                SocketUdpClient client = new SocketUdpClient(serverSocket, connectTo, TestPortBase + 38);

                if (!waitHandle.WaitOne(500))
                {
                    throw new TimeoutException();
                }

                Assert.Equal(1, args.BytesTransferred);
                Assert.Equal<Type>(args.RemoteEndPoint.GetType(), typeof(IPEndPoint));
                IPEndPoint remoteEndPoint = args.RemoteEndPoint as IPEndPoint;
                Assert.Equal(AddressFamily.InterNetworkV6, remoteEndPoint.AddressFamily);
                Assert.Equal(connectTo.MapToIPv6(), remoteEndPoint.Address);

                Assert.Equal(SocketFlags.None, args.SocketFlags);
                Assert.NotNull(args.ReceiveMessageFromPacketInfo);
                Assert.Equal(connectTo, args.ReceiveMessageFromPacketInfo.Address);
                Assert.Equal(NetworkInterface.IPv6LoopbackInterfaceIndex, args.ReceiveMessageFromPacketInfo.Interface);
            }
        }

        #endregion ReceiveMessageFrom Async/Event

        #endregion ReceiveMessageFrom

        #endregion Connectionless

        #region GC Finalizer test
        // This test assumes sequential execution of tests and that it is going to be executed after other tests
        // that used Sockets. 
        [Fact]
        public void TestFinalizers()
        {
            // Making several passes through the FReachable list.
            for (int i = 0; i < 3; i++)
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
        }
        #endregion 

        #region Helpers

        private class SocketServer : IDisposable
        {
            private TcpListener server;

            public SocketServer(IPAddress address, bool dualMode, int port)
            {
                server = new TcpListener(address, port);
                if (dualMode) server.Server.DualMode = dualMode;
                server.Start();
                server.BeginAcceptSocket(Accepted, null);
            }

            private void Accepted(IAsyncResult ar)
            {
                try
                {
                    Socket socket = server.EndAcceptSocket(ar);
                    _log.WriteLine("Accpeted Socket: " + socket.RemoteEndPoint);
                }
                catch (SocketException) { }
                catch (ObjectDisposedException) { }
            }

            public void Dispose()
            {
                try
                {
                    server.Stop();
                }
                catch (Exception) { }
            }
        }

        private class SocketClient
        {
            private IPAddress connectTo;
            private Socket serverSocket;
            private int port;

            public SocketClient(Socket serverSocket, IPAddress connectTo, int port)
            {
                this.connectTo = connectTo;
                this.serverSocket = serverSocket;
                this.port = port;
                ThreadPool.QueueUserWorkItem(ConnectClient);
            }

            private void ConnectClient(object state)
            {
                try
                {
                    Socket socket = new Socket(connectTo.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                    socket.Connect(this.connectTo, this.port);
                }
                catch (SocketException) 
                {
                    serverSocket.Dispose(); // Cancels the test
                }
            }
        }

        private class SocketUdpServer : IDisposable
        {
            private Socket server;
            private ManualResetEvent waitHandle = new ManualResetEvent(false);

            public EventWaitHandle WaitHandle 
            { 
                get { return waitHandle; } 
            }
            
            public SocketUdpServer(IPAddress address, bool dualMode, int port)
            {
                server = new Socket(address.AddressFamily, SocketType.Dgram, ProtocolType.Udp);
                if (dualMode) server.DualMode = dualMode;
                server.Bind(new IPEndPoint(address, port));
                EndPoint remote = new IPEndPoint(
                    (address.AddressFamily == AddressFamily.InterNetwork ? IPAddress.Any : IPAddress.IPv6Any) , 0);
                server.BeginReceiveFrom(new byte[1], 0, 1, SocketFlags.None, ref remote, Received, null);
            }

            private void Received(IAsyncResult ar)
            {
                try
                {
                    EndPoint remote = new IPEndPoint(
                        (server.AddressFamily == AddressFamily.InterNetwork ? IPAddress.Any : IPAddress.IPv6Any), 0);
                    int byteCount = server.EndReceiveFrom(ar, ref remote);                 
                }
                catch (SocketException) { }
                catch (ObjectDisposedException) { }
                WaitHandle.Set();
            }

            public void Dispose()
            {
                try
                {
                    server.Dispose();
                }
                catch (Exception) { }
            }
        }

        private class SocketUdpClient
        {
            private int port;
            private IPAddress connectTo;
            private Socket serverSocket;

            public SocketUdpClient(Socket serverSocket, IPAddress connectTo, int port)
            {
                this.connectTo = connectTo;
                this.port = port;
                this.serverSocket = serverSocket;
                ThreadPool.QueueUserWorkItem(ClientSend);
            }

            private void ClientSend(object state)
            {
                try
                {
                    Socket socket = new Socket(connectTo.AddressFamily, SocketType.Dgram, ProtocolType.Udp);
                    socket.SendTo(new byte[1], new IPEndPoint(connectTo, port));
                }
                catch (SocketException) 
                {
                    serverSocket.Dispose(); // Cancels the test
                }
            }
        }

        private void AsyncCompleted(object sender, SocketAsyncEventArgs e)
        {
            ManualResetEvent waitHandle = (ManualResetEvent)e.UserToken;
            _log.WriteLine(
                "AsyncCompleted: " + e.GetHashCode() + " SocketAsyncEventArgs with manual event " +
                waitHandle.GetHashCode() + " error: " + e.SocketError);

            waitHandle.Set();
        }

        #endregion Helpers
    }
}
