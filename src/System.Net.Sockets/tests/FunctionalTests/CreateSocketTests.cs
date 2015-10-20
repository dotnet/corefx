// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Xunit;

namespace System.Net.Sockets.Tests
{
    public class CreateSocket
    {
        public static object[][] DualModeSuccessInputs = {
            new object[] { SocketType.Stream, ProtocolType.Tcp },
            new object[] { SocketType.Dgram, ProtocolType.Udp },
        };

        public static object[][] DualModeFailureInputs = {
            new object[] { SocketType.Dgram, ProtocolType.Tcp },

            new object[] { SocketType.Rdm, ProtocolType.Tcp },
            new object[] { SocketType.Seqpacket, ProtocolType.Tcp },
            new object[] { SocketType.Unknown, ProtocolType.Tcp },
            new object[] { SocketType.Rdm, ProtocolType.Udp },
            new object[] { SocketType.Seqpacket, ProtocolType.Udp },
            new object[] { SocketType.Stream, ProtocolType.Udp },
            new object[] { SocketType.Unknown, ProtocolType.Udp },
/*
    Disabling these test cases because it actually passes in some cases
    see https://github.com/dotnet/corefx/issues/3726
            new object[] { SocketType.Raw, ProtocolType.Tcp },
            new object[] { SocketType.Raw, ProtocolType.Udp },
*/
        };

        [Theory, MemberData("DualModeSuccessInputs")]
        public void DualMode_Success(SocketType socketType, ProtocolType protocolType)
        {
            using (new Socket(socketType, protocolType))
            {
            }
        }

        [Theory, MemberData("DualModeFailureInputs")]
        public void DualMode_Failure(SocketType socketType, ProtocolType protocolType)
        {
            Assert.Throws<SocketException>(() => new Socket(socketType, protocolType));
        }

        public static object[][] CtorSuccessInputs = {
            new object[] { AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp },
            new object[] { AddressFamily.InterNetworkV6, SocketType.Stream, ProtocolType.Tcp },
            new object[] { AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp },
            new object[] { AddressFamily.InterNetworkV6, SocketType.Dgram, ProtocolType.Udp },
        };

        [Theory, MemberData("CtorSuccessInputs")]
        public void Ctor_Success(AddressFamily addressFamily, SocketType socketType, ProtocolType protocolType)
        {
            using (new Socket(addressFamily, socketType, protocolType))
            {
            }
        }

        public static object[][] CtorFailureInputs = {
            new object[] { AddressFamily.Unknown, SocketType.Stream, ProtocolType.Tcp },
            new object[] { AddressFamily.Unknown, SocketType.Dgram, ProtocolType.Udp },
            new object[] { AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Tcp },
            new object[] { AddressFamily.InterNetwork, SocketType.Rdm, ProtocolType.Tcp },
            new object[] { AddressFamily.InterNetwork, SocketType.Seqpacket, ProtocolType.Tcp },
            new object[] { AddressFamily.InterNetwork, SocketType.Unknown, ProtocolType.Tcp },
            new object[] { AddressFamily.InterNetwork, SocketType.Rdm, ProtocolType.Udp },
            new object[] { AddressFamily.InterNetwork, SocketType.Seqpacket, ProtocolType.Udp },
            new object[] { AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Udp },
            new object[] { AddressFamily.InterNetwork, SocketType.Unknown, ProtocolType.Udp },
/*
    Disabling these test cases because it actually passes in some cases
    see https://github.com/dotnet/corefx/issues/3726
            new object[] { AddressFamily.InterNetwork, SocketType.Raw, ProtocolType.Tcp },
            new object[] { AddressFamily.InterNetwork, SocketType.Raw, ProtocolType.Udp },
*/
        };

        [Theory, MemberData("CtorFailureInputs")]
        public void Ctor_Failure(AddressFamily addressFamily, SocketType socketType, ProtocolType protocolType)
        {
            Assert.Throws<SocketException>(() => new Socket(addressFamily, socketType, protocolType));
        }
    }
}
