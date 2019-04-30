// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Threading.Tasks;
using Microsoft.DotNet.RemoteExecutor;
using Xunit;

namespace System.Net.Sockets.Tests
{
    public partial class CreateSocket
    {
        [Theory]
        [InlineData(AddressFamily.Netlink)]
        [InlineData(AddressFamily.Packet)]
        [InlineData(AddressFamily.ControllerAreaNetwork)]
        [PlatformSpecific(~TestPlatforms.Linux)]
        public void Ctor_Netcoreapp_Throws(AddressFamily addressFamily)
        {
            // All protocols are Linux specific and throw throw on other platforms
            Assert.Throws<SocketException>(() => new Socket(addressFamily, SocketType.Raw, 0));
        }

        [Theory]
        [InlineData(AddressFamily.Netlink)]
        [InlineData(AddressFamily.Packet)]
        [InlineData(AddressFamily.ControllerAreaNetwork)]
        [PlatformSpecific(TestPlatforms.Linux)]
        public void Ctor_Netcoreapp_Success(AddressFamily addressFamily)
        {
            try
            {
                Socket s = new Socket(addressFamily, SocketType.Raw, 0);
                s.Close();
            }
            catch (SocketException e)
            {
                // We may fail to create socket because of permission or loaded kernel modules.
                if (e.SocketErrorCode != SocketError.AccessDenied && e.SocketErrorCode != SocketError.ProtocolNotSupported)
                {
                    throw;
                }
            }
        }
    }
}
