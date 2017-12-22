// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Net.Test.Common;
using System.Threading;
using System.Threading.Tasks;

using Xunit;
using Xunit.Abstractions;

namespace System.Net.Sockets.Tests
{
    [SkipOnTargetFramework(TargetFrameworkMonikers.NetFramework, "Requires fix shipping in .NET 4.7.2")]
    public class BeginConnectTest
    {
        [Fact]
        public void BeginConnectEndPoint_InitialAsyncOperation_Success()
        {
            DummySocketServer server = new DummySocketServer(new IPEndPoint(IPAddress.Loopback, 8000));
            Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IAsyncResult result = sock.BeginConnect(new DnsEndPoint("localhost", 8000), null, null);
            sock.EndConnect(result);
            sock.Close();
            server.Dispose();
        }
        
        [Fact]
        public void BeginConnectHostName_InitialAsyncOperation_Success()
        {
            DummySocketServer server = new DummySocketServer(new IPEndPoint(IPAddress.Loopback, 8000));
            Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IAsyncResult result = sock.BeginConnect("localhost", 8000, null, null);
            sock.EndConnect(result);
            sock.Close();
            server.Dispose();
        }
        
        [Fact]
        public void BeginConnectIPAddress_InitialAsyncOperation_Success()
        {
            DummySocketServer server = new DummySocketServer(new IPEndPoint(IPAddress.Loopback, 8000));
            Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IAsyncResult result = sock.BeginConnect(IPAddress.Loopback, 8000, null, null);
            sock.EndConnect(result);
            sock.Close();
            server.Dispose();
        }
        
        [Fact]
        public void BeginConnectIPAddressArray_InitialAsyncOperation_Success()
        {
            DummySocketServer server = new DummySocketServer(new IPEndPoint(IPAddress.Loopback, 8000));
            Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IAsyncResult result = sock.BeginConnect(new IPAddress[] { IPAddress.Loopback }, 8000, null, null);
            sock.EndConnect(result);
            sock.Close();
            server.Dispose();
        }
        
        [Fact]
        public void BeginConnectEndPoint_WithOngoingAsyncOperation_Throws()
        {
            DummySocketServer server = new DummySocketServer(new IPEndPoint(IPAddress.Loopback, 8000));
            Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IAsyncResult result = sock.BeginConnect(new DnsEndPoint("localhost", 8000), null, null);
            Assert.Throws<InvalidOperationException>(() => sock.BeginConnect(new DnsEndPoint("localhost", 8000), null, null));
            
            sock.EndConnect(result);
            sock.Close();
            server.Dispose();
        }
        
        [Fact]
        public void BeginConnectHostName_WithOngoingAsyncOperation_Throws()
        {
            DummySocketServer server = new DummySocketServer(new IPEndPoint(IPAddress.Loopback, 8000));
            Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IAsyncResult result = sock.BeginConnect(new DnsEndPoint("localhost", 8000), null, null);
            Assert.Throws<InvalidOperationException>(() => sock.BeginConnect("localhost", 8000, null, null));
            
            sock.EndConnect(result);
            sock.Close();
            server.Dispose();
        }
        
        [Fact]
        public void BeginConnectIPAddress_WithOngoingAsyncOperation_Throws()
        {
            DummySocketServer server = new DummySocketServer(new IPEndPoint(IPAddress.Loopback, 8000));
            Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IAsyncResult result = sock.BeginConnect(new DnsEndPoint("localhost", 8000), null, null);
            Assert.Throws<InvalidOperationException>(() => sock.BeginConnect(IPAddress.Loopback, 8000, null, null));
            
            sock.EndConnect(result);
            sock.Close();
            server.Dispose();
        }
        
        [Fact]
        public void BeginConnectIPAddressArray_WithOngoingAsyncOperation_Throws()
        {
            DummySocketServer server = new DummySocketServer(new IPEndPoint(IPAddress.Loopback, 8000));
            Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IAsyncResult result = sock.BeginConnect(new DnsEndPoint("localhost", 8000), null, null);
            Assert.Throws<InvalidOperationException>(() => sock.BeginConnect(new IPAddress[] { IPAddress.Loopback }, 8000, null, null));
            
            sock.EndConnect(result);
            sock.Close();
            server.Dispose();
        }
    }
}