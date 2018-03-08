// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.IO;

using Xunit;

namespace System.Net.Sockets.Tests
{
    public partial class SendPacketsAsync
    {
        #region Files

        [Theory]
        [InlineData(SocketImplementationType.APM)]
        [InlineData(SocketImplementationType.Async)]
        public void SendPacketsElement_Windows_FileLargeCount_Throws(SocketImplementationType type)
        {
            // Length is validated on Send
            Assert.Throws<OverflowException>(() => SendPackets(type,
                new SendPacketsElement(TestFileName, 5L, (long)uint.MaxValue + 10000), SocketError.InvalidArgument, 0));
        }

        #endregion Files

        #region FileStreams

        [Theory]
        [InlineData(SocketImplementationType.APM)]
        [InlineData(SocketImplementationType.Async)]
        public void SendPacketsElement_Windows_FileStreamLargeCount_Throws(SocketImplementationType type)
        {
            using (var stream = new FileStream(TestFileName, FileMode.Open, FileAccess.Read))
            {
                stream.Seek(s_testFileSize / 2, SeekOrigin.Begin);
                // Length is validated on Send
                Assert.Throws<OverflowException>(() => SendPackets(type,
                    new SendPacketsElement(stream, 5L, (long)uint.MaxValue + 10000), SocketError.InvalidArgument,
                    0));
            }
        }

        #endregion FileStreams
    }
}
