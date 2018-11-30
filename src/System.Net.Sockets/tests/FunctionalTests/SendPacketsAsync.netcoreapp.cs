// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.IO;
using System.Runtime.InteropServices;
using Xunit;

namespace System.Net.Sockets.Tests
{
    public partial class SendPacketsAsync
    {
        [Theory]
        [InlineData(SocketImplementationType.APM)]
        [InlineData(SocketImplementationType.Async)]
        public void SendPacketsElement_FileZeroCount_OffsetLong_Success(SocketImplementationType type)
        {
            SendPackets(type, new SendPacketsElement(TestFileName, 0L, 0), s_testFileSize);  // Whole File
        }

        [Theory]
        [InlineData(SocketImplementationType.APM)]
        [InlineData(SocketImplementationType.Async)]
        public void SendPacketsElement_FilePart_OffsetLong_Success(SocketImplementationType type)
        {
            SendPackets(type, new SendPacketsElement(TestFileName, 10L, 20), 20);
        }

        [Theory]
        [InlineData(SocketImplementationType.APM)]
        [InlineData(SocketImplementationType.Async)]
        public void SendPacketsElement_FileMultiPart_OffsetLong_Success(SocketImplementationType type)
        {
            var elements = new[]
            {
                new SendPacketsElement(TestFileName, 10L, 20),
                new SendPacketsElement(TestFileName, 30L, 10),
                new SendPacketsElement(TestFileName, 0L, 10),
            };
            SendPackets(type, elements, SocketError.Success, 40);
        }

        [Theory]
        [InlineData(SocketImplementationType.APM)]
        [InlineData(SocketImplementationType.Async)]
        public void SendPacketsElement_FileLargeOffset__OffsetLong_Throws(SocketImplementationType type)
        {
            // Length is validated on Send
            SendPackets(type, new SendPacketsElement(TestFileName, (long)uint.MaxValue + 11000, 1), SocketError.InvalidArgument, 0);
        }

        [Theory]
        [InlineData(SocketImplementationType.APM)]
        [InlineData(SocketImplementationType.Async)]
        public void SendPacketsElement_FileLargeCount__OffsetLong_Throws(SocketImplementationType type)
        {
            // Length is validated on Send
            SendPackets(type, new SendPacketsElement(TestFileName, 5L, 10000), SocketError.InvalidArgument, 0);
        }

        #region FileStreams

        [Theory]
        [InlineData(SocketImplementationType.APM)]
        [InlineData(SocketImplementationType.Async)]
        public void SendPacketsElement_FileStream_Success(SocketImplementationType type)
        {
            using (var stream = new FileStream(TestFileName, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, useAsync: true))
            {
                stream.Seek(s_testFileSize / 2, SeekOrigin.Begin);
                SendPackets(type, new SendPacketsElement(stream), s_testFileSize); // Whole File
                if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                    Assert.Equal(s_testFileSize / 2, stream.Position);

                SendPackets(type, new SendPacketsElement(stream), s_testFileSize); // Whole File
                if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                    Assert.Equal(s_testFileSize / 2, stream.Position);
            }
        }

        [Theory]
        [InlineData(SocketImplementationType.APM)]
        [InlineData(SocketImplementationType.Async)]
        public void SendPacketsElement_FileStreamZeroCount_Success(SocketImplementationType type)
        {
            using (var stream = new FileStream(TestFileName, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, useAsync: true))
            {
                stream.Seek(s_testFileSize / 2, SeekOrigin.Begin);
                SendPackets(type, new SendPacketsElement(stream, 0, 0), s_testFileSize); // Whole File
                if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                    Assert.Equal(s_testFileSize / 2, stream.Position);

                SendPackets(type, new SendPacketsElement(stream, 0, 0), s_testFileSize); // Whole File
                if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                    Assert.Equal(s_testFileSize / 2, stream.Position);
            }
        }

        [Theory]
        [InlineData(SocketImplementationType.APM)]
        [InlineData(SocketImplementationType.Async)]
        public void SendPacketsElement_FileStreamSizeCount_Success(SocketImplementationType type)
        {
            using (var stream = new FileStream(TestFileName, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, useAsync: true))
            {
                stream.Seek(s_testFileSize / 2, SeekOrigin.Begin);
                SendPackets(type, new SendPacketsElement(stream, 0, s_testFileSize), s_testFileSize); // Whole File
                if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                    Assert.Equal(s_testFileSize / 2, stream.Position);

                SendPackets(type, new SendPacketsElement(stream, 0, s_testFileSize), s_testFileSize); // Whole File
                if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                    Assert.Equal(s_testFileSize / 2, stream.Position);
            }
        }

        [Theory]
        [InlineData(SocketImplementationType.APM)]
        [InlineData(SocketImplementationType.Async)]
        public void SendPacketsElement_FileStreamPart_Success(SocketImplementationType type)
        {
            using (var stream = new FileStream(TestFileName, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, useAsync: true))
            {
                stream.Seek(s_testFileSize - 10, SeekOrigin.Begin);
                SendPackets(type, new SendPacketsElement(stream, 0, 20), 20);
                if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                    Assert.Equal(s_testFileSize - 10, stream.Position);

                SendPackets(type, new SendPacketsElement(stream, 10, 20), 20);
                if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                    Assert.Equal(s_testFileSize - 10, stream.Position);

                SendPackets(type, new SendPacketsElement(stream, s_testFileSize - 20, 20), 20);
                if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                    Assert.Equal(s_testFileSize - 10, stream.Position);
            }
        }

        [Theory]
        [InlineData(SocketImplementationType.APM)]
        [InlineData(SocketImplementationType.Async)]
        public void SendPacketsElement_FileStreamMultiPart_Success(SocketImplementationType type)
        {
            using (var stream = new FileStream(TestFileName, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, useAsync: true))
            {
                var elements = new[]
                {
                    new SendPacketsElement(stream, 0, 20),
                    new SendPacketsElement(stream, s_testFileSize - 10, 10),
                    new SendPacketsElement(stream, 0, 10),
                    new SendPacketsElement(stream, 10, 20),
                    new SendPacketsElement(stream, 30, 10),
                };
                stream.Seek(s_testFileSize - 10, SeekOrigin.Begin);
                SendPackets(type, elements, SocketError.Success, 70);
                if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                    Assert.Equal(s_testFileSize - 10, stream.Position);

                SendPackets(type, elements, SocketError.Success, 70);
                if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                    Assert.Equal(s_testFileSize - 10, stream.Position);
            }
        }

        [Theory]
        [InlineData(SocketImplementationType.APM)]
        [InlineData(SocketImplementationType.Async)]
        public void SendPacketsElement_FileStreamAsyncMultiPart_Success(SocketImplementationType type)
        {
            using (var stream = new FileStream(TestFileName, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, FileOptions.Asynchronous))
            {
                var elements = new[]
                {
                    new SendPacketsElement(stream, 0, 20),
                    new SendPacketsElement(stream, s_testFileSize - 10, 10),
                    new SendPacketsElement(stream, 0, 10),
                    new SendPacketsElement(stream, 10, 20),
                    new SendPacketsElement(stream, 30, 10),
                };
                stream.Seek(s_testFileSize - 10, SeekOrigin.Begin);
                SendPackets(type, elements, SocketError.Success, 70);
                Assert.Equal(s_testFileSize - 10, stream.Position);

                SendPackets(type, elements, SocketError.Success, 70);
                Assert.Equal(s_testFileSize - 10, stream.Position);
            }
        }

        [Theory]
        [InlineData(SocketImplementationType.APM)]
        [InlineData(SocketImplementationType.Async)]
        public void SendPacketsElement_FileStreamLargeOffset_Throws(SocketImplementationType type)
        {
            using (var stream = new FileStream(TestFileName, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, useAsync: true))
            {
                stream.Seek(s_testFileSize / 2, SeekOrigin.Begin);
                // Length is validated on Send
                SendPackets(type, new SendPacketsElement(stream, (long)uint.MaxValue + 11000, 1), SocketError.InvalidArgument, 0);
            }
        }

        [Theory]
        [InlineData(SocketImplementationType.APM)]
        [InlineData(SocketImplementationType.Async)]
        public void SendPacketsElement_FileStreamLargeCount_Throws(SocketImplementationType type)
        {
            using (var stream = new FileStream(TestFileName, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, useAsync: true))
            {
                stream.Seek(s_testFileSize / 2, SeekOrigin.Begin);
                // Length is validated on Send
                SendPackets(type, new SendPacketsElement(stream, 5, 10000),
                    SocketError.InvalidArgument, 0);
            }
        }

        #endregion FileStreams
    }
}
