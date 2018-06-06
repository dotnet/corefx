// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;
using Xunit;

namespace System.Net.Sockets.Tests
{
    public class KeepAliveTest
    {
        private bool RunningOnWindowsBelow10v1703
        {
            get
            {
                if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                    return false;
                Version version = Environment.OSVersion.Version;
                return version.Major < 10 || version.Major == 10 && version.Build < 15063;
            }
        }

        private bool RunningOnWindowsBelow10v1709
        {
            get
            {
                if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                    return false;
                Version version = Environment.OSVersion.Version;
                return version.Major < 10 || version.Major == 10 && version.Build < 16299;
            }
        }

        [Fact]
        public void Socket_KeepAlive_Disabled_By_Default()
        {
            using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
            {
                Assert.Equal<int>(0, (int)socket.GetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive));
            }
        }

        [Fact]
        public void Socket_KeepAlive_Enable()
        {
            using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
            {
                socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);
                Assert.NotEqual<int>(0, (int)socket.GetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive));
            }
        }

        [Fact]
        public void Socket_KeepAliveState_Set_RetryCount()
        {
            // TcpKeepAliveRetryCount can be managed via *SocketOption starting from Windows 10 version 1703
            if (RunningOnWindowsBelow10v1703)
                return;

            const int retryCount = 60;
            using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
            {
                socket.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.TcpKeepAliveRetryCount, retryCount);
                Assert.Equal<int>(retryCount, (int)socket.GetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.TcpKeepAliveRetryCount));
            }            
        }

        [Fact]
        public void Socket_KeepAliveState_Set_Time()
        {
            // TcpKeepAliveTime can be managed via *SocketOption starting from Windows 10 version 1709
            if (RunningOnWindowsBelow10v1709)
                return;

            const int time = 5;
            using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
            {
                socket.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.TcpKeepAliveTime, time);
                Assert.Equal<int>(time, (int)socket.GetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.TcpKeepAliveTime));
            }            
        }

        [Fact]
        public void Socket_KeepAliveState_Set_Interval()
        {
            // TcpKeepAliveInterval can be managed via *SocketOption starting from Windows 10 version 1709
            if (RunningOnWindowsBelow10v1709)
                return;

            const int interval = 2;
            using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
            {
                socket.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.TcpKeepAliveInterval, interval);
                Assert.Equal<int>(interval, (int)socket.GetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.TcpKeepAliveInterval));
            }            
        }
    }
}