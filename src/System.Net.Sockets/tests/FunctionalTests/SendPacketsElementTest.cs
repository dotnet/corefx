// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.IO;
using Xunit;

namespace System.Net.Sockets.Tests
{
    public class SendPacketsElementTest
    {
        #region Buffer

        [Fact]
        public void NullBufferCtor_Throws()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                new SendPacketsElement((byte[])null);
            });
        }

        [Fact]
        public void NullBufferCtorWithOffset_Throws()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                new SendPacketsElement((byte[])null, 0, 0);
            });
        }

        [Fact]
        public void NullBufferCtorWithEndOfPacket_Throws()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                // Elements with null Buffers are ignored on Send
                new SendPacketsElement((byte[])null, 0, 0, true);
            });
        }

        [Fact]
        public void EmptyBufferCtor_Success()
        {
            // Elements with empty Buffers are ignored on Send
            SendPacketsElement element = new SendPacketsElement(new byte[0]);
            Assert.NotNull(element.Buffer);
            Assert.Equal(0, element.Buffer.Length);
            Assert.Equal(0, element.Offset);
            Assert.Equal(0, element.Count);
            Assert.Equal(0, element.LongOffset);
            Assert.False(element.EndOfPacket);
            Assert.Null(element.FilePath);
            Assert.Null(element.FileStream);
        }

        [Fact]
        public void BufferCtorNormal_Success()
        {
            SendPacketsElement element = new SendPacketsElement(new byte[10]);
            Assert.NotNull(element.Buffer);
            Assert.Equal(10, element.Buffer.Length);
            Assert.Equal(0, element.Offset);
            Assert.Equal(10, element.Count);
            Assert.Equal(0, element.LongOffset);
            Assert.False(element.EndOfPacket);
            Assert.Null(element.FilePath);
            Assert.Null(element.FileStream);
        }

        [Fact]
        public void BufferCtorNegOffset_ArgumentOutOfRangeException()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                new SendPacketsElement(new byte[10], -1, 11);
            });
        }

        [Fact]
        public void BufferCtorNegCount_ArgumentOutOfRangeException()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                new SendPacketsElement(new byte[10], 0, -1);
            });
        }

        [Fact]
        public void BufferCtorLargeOffset_ArgumentOutOfRangeException()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                new SendPacketsElement(new byte[10], 11, 1);
            });
        }

        [Fact]
        public void BufferCtorLargeCount_ArgumentOutOfRangeException()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                new SendPacketsElement(new byte[10], 5, 10);
            });
        }

        [Fact]
        public void BufferCtorEndOfBufferTrue_Success()
        {
            SendPacketsElement element = new SendPacketsElement(new byte[10], 2, 8, true);
            Assert.NotNull(element.Buffer);
            Assert.Equal(10, element.Buffer.Length);
            Assert.Equal(2, element.Offset);
            Assert.Equal(8, element.Count);
            Assert.Equal(2, element.LongOffset);
            Assert.True(element.EndOfPacket);
            Assert.Null(element.FilePath);
            Assert.Null(element.FileStream);
        }

        [Fact]
        public void BufferCtorEndOfBufferFalse_Success()
        {
            SendPacketsElement element = new SendPacketsElement(new byte[10], 6, 4, false);
            Assert.NotNull(element.Buffer);
            Assert.Equal(10, element.Buffer.Length);
            Assert.Equal(6, element.Offset);
            Assert.Equal(4, element.Count);
            Assert.Equal(6, element.LongOffset);
            Assert.False(element.EndOfPacket);
            Assert.Null(element.FilePath);
            Assert.Null(element.FileStream);
        }

        [Fact]
        public void BufferCtorZeroCount_Success()
        {
            // Elements with empty Buffers are ignored on Send
            SendPacketsElement element = new SendPacketsElement(new byte[0], 0, 0);
            Assert.NotNull(element.Buffer);
            Assert.Equal(0, element.Buffer.Length);
            Assert.Equal(0, element.Offset);
            Assert.Equal(0, element.Count);
            Assert.Equal(0, element.LongOffset);
            Assert.False(element.EndOfPacket);
            Assert.Null(element.FilePath);
            Assert.Null(element.FileStream);
        }

        #endregion Buffer

        #region File

        [Fact]
        public void FileCtorNull_Throws()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                new SendPacketsElement((string)null);
            });
            Assert.Throws<ArgumentNullException>(() =>
            {
                new SendPacketsElement((string)null, 0, 0);
            });
            Assert.Throws<ArgumentNullException>(() =>
            {
                new SendPacketsElement((string)null, 0, 0, true);
            });
            Assert.Throws<ArgumentNullException>(() =>
            {
                new SendPacketsElement((string)null, 0L, 0);
            });
            Assert.Throws<ArgumentNullException>(() =>
            {
                new SendPacketsElement((string)null, 0L, 0, true);
            });
        }

        [Fact]
        public void FileCtorEmpty_Success()
        {
            // An exception will happen on send if this file doesn't exist
            SendPacketsElement element = new SendPacketsElement(string.Empty);
            Assert.Null(element.FileStream);
            Assert.Null(element.Buffer);
            Assert.Equal(0, element.Offset);
            Assert.Equal(0, element.Count);
            Assert.Equal(0, element.LongOffset);
            Assert.False(element.EndOfPacket);
            Assert.Equal(string.Empty, element.FilePath);
        }

        [Fact]
        public void FileCtorWhiteSpace_Success()
        {
            // An exception will happen on send if this file doesn't exist
            SendPacketsElement element = new SendPacketsElement("   \t ");
            Assert.Null(element.FileStream);
            Assert.Null(element.Buffer);
            Assert.Equal(0, element.Offset);
            Assert.Equal(0, element.Count);
            Assert.Equal(0, element.LongOffset);
            Assert.False(element.EndOfPacket);
            Assert.Equal("   \t ", element.FilePath);
        }

        [Fact]
        public void FileCtorNormal_Success()
        {
            // An exception will happen on send if this file doesn't exist
            SendPacketsElement element = new SendPacketsElement("SomeFileName"); // Send whole file
            Assert.Null(element.FileStream);
            Assert.Null(element.Buffer);
            Assert.Equal(0, element.Offset);
            Assert.Equal(0, element.Count);
            Assert.Equal(0, element.LongOffset);
            Assert.False(element.EndOfPacket);
            Assert.Equal("SomeFileName", element.FilePath);
        }

        [Fact]
        public void FileCtorZeroCountLength_Success()
        {
            // An exception will happen on send if this file doesn't exist
            SendPacketsElement element = new SendPacketsElement("SomeFileName", 0, 0); // Send whole file
            Assert.Null(element.FileStream);
            Assert.Null(element.Buffer);
            Assert.Equal(0, element.Offset);
            Assert.Equal(0, element.Count);
            Assert.Equal(0, element.LongOffset);
            Assert.False(element.EndOfPacket);
            Assert.Equal("SomeFileName", element.FilePath);

            // An exception will happen on send if this file doesn't exist
            element = new SendPacketsElement("SomeFileName", 0L, 0); // Send whole file
            Assert.Null(element.FileStream);
            Assert.Null(element.Buffer);
            Assert.Equal(0, element.Offset);
            Assert.Equal(0, element.Count);
            Assert.Equal(0, element.LongOffset);
            Assert.False(element.EndOfPacket);
            Assert.Equal("SomeFileName", element.FilePath);
        }

        [Fact]
        public void FileCtorNegOffset_ArgumentOutOfRangeException()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                new SendPacketsElement("SomeFileName", -1, 11);
            });
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                new SendPacketsElement("SomeFileName", -1, 11, true);
            });
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                new SendPacketsElement("SomeFileName", -1L, 11);
            });
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                new SendPacketsElement("SomeFileName", -1L, 11, true);
            });
        }

        [Fact]
        public void FileCtorNegCount_ArgumentOutOfRangeException()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                new SendPacketsElement("SomeFileName", 0, -1);
            });
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                new SendPacketsElement("SomeFileName", 0, -1, true);
            });
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                new SendPacketsElement("SomeFileName", 0L, -1);
            });
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                new SendPacketsElement("SomeFileName", 0L, -1, true);
            });
        }

        // File lengths are validated on send

        [Fact]
        public void FileCtorEndOfBufferTrue_Success()
        {
            SendPacketsElement element = new SendPacketsElement("SomeFileName", 2, 8, true);
            Assert.Null(element.FileStream);
            Assert.Null(element.Buffer);
            Assert.Equal(2, element.Offset);
            Assert.Equal(8, element.Count);
            Assert.Equal(2, element.LongOffset);
            Assert.True(element.EndOfPacket);
            Assert.Equal("SomeFileName", element.FilePath);

            element = new SendPacketsElement("SomeFileName", 2L, 8, true);
            Assert.Null(element.FileStream);
            Assert.Null(element.Buffer);
            Assert.Equal(2, element.Offset);
            Assert.Equal(8, element.Count);
            Assert.Equal(2, element.LongOffset);
            Assert.True(element.EndOfPacket);
            Assert.Equal("SomeFileName", element.FilePath);

            element = new SendPacketsElement("SomeFileName", (long)int.MaxValue + 2, 8, true);
            Assert.Null(element.FileStream);
            Assert.Null(element.Buffer);
            Assert.Throws<OverflowException>(() =>
            {
                var ofset = element.Offset;
            });
            Assert.Equal(8, element.Count);
            Assert.Equal((long)int.MaxValue + 2, element.LongOffset);
            Assert.True(element.EndOfPacket);
            Assert.Equal("SomeFileName", element.FilePath);
        }

        [Fact]
        public void FileCtorEndOfBufferFalse_Success()
        {
            SendPacketsElement element = new SendPacketsElement("SomeFileName", 6, 4, false);
            Assert.Null(element.FileStream);
            Assert.Null(element.Buffer);
            Assert.Equal(6, element.Offset);
            Assert.Equal(4, element.Count);
            Assert.Equal(6, element.LongOffset);
            Assert.False(element.EndOfPacket);
            Assert.Equal("SomeFileName", element.FilePath);

            element = new SendPacketsElement("SomeFileName", 6L, 4, false);
            Assert.Null(element.FileStream);
            Assert.Null(element.Buffer);
            Assert.Equal(6, element.Offset);
            Assert.Equal(4, element.Count);
            Assert.Equal(6, element.LongOffset);
            Assert.False(element.EndOfPacket);
            Assert.Equal("SomeFileName", element.FilePath);

            element = new SendPacketsElement("SomeFileName", (long)int.MaxValue + 6, 4, false);
            Assert.Null(element.FileStream);
            Assert.Null(element.Buffer);
            Assert.Throws<OverflowException>(() =>
            {
                var ofset = element.Offset;
            });
            Assert.Equal(4, element.Count);
            Assert.Equal((long)int.MaxValue + 6, element.LongOffset);
            Assert.False(element.EndOfPacket);
            Assert.Equal("SomeFileName", element.FilePath);
        }

        #endregion File

        #region FileStream

        [Fact]
        public void FileStreamCtorNull_Throws()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                new SendPacketsElement((FileStream)null);
            });
            Assert.Throws<ArgumentNullException>(() =>
            {
                new SendPacketsElement((FileStream)null, 0, 0);
            });
            Assert.Throws<ArgumentNullException>(() =>
            {
                new SendPacketsElement((FileStream)null, 0, 0, true);
            });
        }

        [Fact]
        public void FileStreamCtorNormal_Success()
        {
            using(var stream = File.Create(Path.GetTempFileName(), 4096, FileOptions.DeleteOnClose))
            {
                SendPacketsElement element = new SendPacketsElement(stream);
                Assert.Null(element.FilePath);
                Assert.Equal(element.FileStream, stream);
                Assert.Null(element.Buffer);
                Assert.Equal(0, element.Offset);
                Assert.Equal(0, element.Count);
                Assert.Equal(0, element.LongOffset);
                Assert.False(element.EndOfPacket);
            }
        }

        [Fact]
        public void FileStreamCtorZeroCountLength_Success()
        {
            using(var stream = File.Create(Path.GetTempFileName(), 4096, FileOptions.DeleteOnClose))
            {
                SendPacketsElement element = new SendPacketsElement(stream, 0, 0); // Send whole file
                Assert.Null(element.FilePath);
                Assert.Equal(element.FileStream, stream);
                Assert.Null(element.Buffer);
                Assert.Equal(0, element.Offset);
                Assert.Equal(0, element.Count);
                Assert.Equal(0, element.LongOffset);
                Assert.False(element.EndOfPacket);

                element = new SendPacketsElement(stream, 0L, 0); // Send whole file
                Assert.Null(element.FilePath);
                Assert.Equal(element.FileStream, stream);
                Assert.Null(element.Buffer);
                Assert.Equal(0, element.Offset);
                Assert.Equal(0, element.Count);
                Assert.Equal(0, element.LongOffset);
                Assert.False(element.EndOfPacket);
            }
        }

        [Fact]
        public void FileStreamCtorNegOffset_ArgumentOutOfRangeException()
        {
            using(var stream = File.Create(Path.GetTempFileName(), 4096, FileOptions.DeleteOnClose))
            {
                Assert.Throws<ArgumentOutOfRangeException>(() =>
                {
                    new SendPacketsElement(stream, -1, 11);
                });
                Assert.Throws<ArgumentOutOfRangeException>(() =>
                {
                    new SendPacketsElement(stream, -1, 11, true);
                });
                Assert.Throws<ArgumentOutOfRangeException>(() =>
                {
                    new SendPacketsElement(stream, -1L, 11);
                });
                Assert.Throws<ArgumentOutOfRangeException>(() =>
                {
                    new SendPacketsElement(stream, -1L, 11, true);
                });
            }
        }

        [Fact]
        public void FileStreamCtorNegCount_ArgumentOutOfRangeException()
        {
            using(var stream = File.Create(Path.GetTempFileName(), 4096, FileOptions.DeleteOnClose))
            {
                Assert.Throws<ArgumentOutOfRangeException>(() =>
                {
                    new SendPacketsElement(stream, 0, -1);
                });
                Assert.Throws<ArgumentOutOfRangeException>(() =>
                {
                    new SendPacketsElement(stream, 0, -1, true);
                });
                Assert.Throws<ArgumentOutOfRangeException>(() =>
                {
                    new SendPacketsElement(stream, 0L, -1);
                });
                Assert.Throws<ArgumentOutOfRangeException>(() =>
                {
                    new SendPacketsElement(stream, 0L, -1, true);
                });
            }
        }

        // File lengths are validated on send

        [Fact]
        public void FileStreamCtorEndOfBufferTrue_Success()
        {
            using(var stream = File.Create(Path.GetTempFileName(), 4096, FileOptions.DeleteOnClose))
            {
                var element = new SendPacketsElement(stream, 2, 8, true);
                Assert.Null(element.FilePath);
                Assert.Equal(element.FileStream, stream);
                Assert.Null(element.Buffer);
                Assert.Equal(2, element.Offset);
                Assert.Equal(8, element.Count);
                Assert.Equal(2, element.LongOffset);
                Assert.True(element.EndOfPacket);

                element = new SendPacketsElement(stream, 2L, 8, true);
                Assert.Null(element.FilePath);
                Assert.Equal(element.FileStream, stream);
                Assert.Null(element.Buffer);
                Assert.Equal(2, element.Offset);
                Assert.Equal(8, element.Count);
                Assert.Equal(2, element.LongOffset);
                Assert.True(element.EndOfPacket);

                element = new SendPacketsElement(stream, (long)int.MaxValue + 2, 8, true);
                Assert.Null(element.FilePath);
                Assert.Equal(element.FileStream, stream);
                Assert.Null(element.Buffer);
                Assert.Throws<OverflowException>(() =>
                {
                    var ofset = element.Offset;
                });
                Assert.Equal(8, element.Count);
                Assert.Equal((long)int.MaxValue + 2, element.LongOffset);
                Assert.True(element.EndOfPacket);
            }
        }

        [Fact]
        public void FileStreamCtorEndOfBufferFalse_Success()
        {
            using(var stream = File.Create(Path.GetTempFileName(), 4096, FileOptions.DeleteOnClose))
            {
                SendPacketsElement element = new SendPacketsElement(stream, 6, 4, false);
                Assert.Null(element.FilePath);
                Assert.Equal(element.FileStream, stream);
                Assert.Null(element.Buffer);
                Assert.Equal(6, element.Offset);
                Assert.Equal(4, element.Count);
                Assert.Equal(6, element.LongOffset);
                Assert.False(element.EndOfPacket);

                element = new SendPacketsElement(stream, 6L, 4, false);
                Assert.Null(element.FilePath);
                Assert.Equal(element.FileStream, stream);
                Assert.Null(element.Buffer);
                Assert.Equal(6, element.Offset);
                Assert.Equal(4, element.Count);
                Assert.Equal(6, element.LongOffset);
                Assert.False(element.EndOfPacket);

                element = new SendPacketsElement(stream, (long)int.MaxValue + 6, 4, false);
                Assert.Null(element.FilePath);
                Assert.Equal(element.FileStream, stream);
                Assert.Null(element.Buffer);
                Assert.Throws<OverflowException>(() =>
                {
                    var ofset = element.Offset;
                });
                Assert.Equal(4, element.Count);
                Assert.Equal((long)int.MaxValue + 6, element.LongOffset);
                Assert.False(element.EndOfPacket);
            }
        }

        #endregion FileStream
    }
}
