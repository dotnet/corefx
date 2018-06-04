// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading;
using Xunit;

namespace System.Net.Sockets.Tests
{
    public class KeepAliveTest : IDisposable
    {
        private const bool enabled = true;
        private const int retryCount = 60;
        private const int time = 5;
        private const int interval = 2;

        private readonly Socket socket;
        
        public KeepAliveTest()
        {
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        [Fact]
        public void Socket_KeepAlive_Disabled_By_Default()
        {
            Assert.False(IsKeepAliveEnabled(socket), "Keep-alive was turned on by default!");
        }

        [Fact]
        public void Socket_KeepAlive_Enable_Success()
        {
            socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);
            Assert.True(IsKeepAliveEnabled(socket));
        }

        [Fact]
        [PlatformSpecific(TestPlatforms.AnyUnix)]
        public void Socket_KeepAliveState_Set_Success_AnyUnix()
        {
            socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, enabled);
            socket.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.TcpKeepAliveRetryCount, retryCount);
            socket.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.TcpKeepAliveTime, time);
            socket.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.TcpKeepAliveInterval, interval);
            
            Assert.Equal<bool>(enabled, IsKeepAliveEnabled(socket));
            Assert.Equal<int>(retryCount, GetTcpOption(socket, SocketOptionName.TcpKeepAliveRetryCount));
            Assert.Equal<int>(time, GetTcpOption(socket, SocketOptionName.TcpKeepAliveTime));
            Assert.Equal<int>(interval, GetTcpOption(socket, SocketOptionName.TcpKeepAliveInterval));
        }

        public void Dispose()
        {
            socket.Dispose();
        }

        private bool IsKeepAliveEnabled(Socket socket)
        {
            return (int)socket.GetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive) != 0;
        }

        private int GetTcpOption(Socket socket, SocketOptionName socketOptionName)
        {
            return (int)socket.GetSocketOption(SocketOptionLevel.Tcp, socketOptionName);
        }
    }
}