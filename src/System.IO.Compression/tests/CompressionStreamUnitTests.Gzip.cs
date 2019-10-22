﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Buffers;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace System.IO.Compression
{
    public class GzipStreamUnitTests : CompressionStreamUnitTestBase
    {
        public override Stream CreateStream(Stream stream, CompressionMode mode) => new GZipStream(stream, mode);
        public override Stream CreateStream(Stream stream, CompressionMode mode, bool leaveOpen) => new GZipStream(stream, mode, leaveOpen);
        public override Stream CreateStream(Stream stream, CompressionLevel level) => new GZipStream(stream, level);
        public override Stream CreateStream(Stream stream, CompressionLevel level, bool leaveOpen) => new GZipStream(stream, level, leaveOpen);
        public override Stream BaseStream(Stream stream) => ((GZipStream)stream).BaseStream;
        protected override string CompressedTestFile(string uncompressedPath) => Path.Combine("GZipTestData", Path.GetFileName(uncompressedPath) + ".gz");

        [Fact]
        public void Precancellation()
        {
            var ms = new MemoryStream();
            using (Stream compressor = new GZipStream(ms, CompressionMode.Compress, leaveOpen: true))
            {
                Assert.True(compressor.WriteAsync(new byte[1], 0, 1, new CancellationToken(true)).IsCanceled);
                Assert.True(compressor.FlushAsync(new CancellationToken(true)).IsCanceled);
            }
            using (Stream decompressor = CreateStream(ms, CompressionMode.Decompress, leaveOpen: true))
            {
                Assert.True(decompressor.ReadAsync(new byte[1], 0, 1, new CancellationToken(true)).IsCanceled);
            }
        }

        [Fact]
        public void ConcatenatedGzipStreams()
        {
            using (MemoryStream concatStream = new MemoryStream())
            {
                using (var gz = new GZipStream(concatStream, CompressionLevel.NoCompression, true))
                using (var sw = new StreamWriter(gz))
                    sw.WriteLine("Stream 1");

                using (var gz = new GZipStream(concatStream, CompressionLevel.NoCompression, true))
                using (var sw = new StreamWriter(gz))
                    sw.WriteLine("Stream 2");

                new GZipStream(concatStream, CompressionLevel.NoCompression, true).Dispose();

                concatStream.Seek(0, SeekOrigin.Begin);
                using (var gz = new GZipStream(concatStream, CompressionMode.Decompress))
                using (var sr = new StreamReader(gz))
                {
                    Assert.Equal("Stream 1", sr.ReadLine());
                    Assert.Equal("Stream 2", sr.ReadLine());
                    Assert.Empty(sr.ReadToEnd());
                }
            }
        }

        /// <summary>
        /// A derived MemoryStream that avoids MemoryStream's fast path in CopyTo
        /// that bypasses buffering.
        /// </summary>
        private class DerivedMemoryStream : MemoryStream
        { }

        [Fact]
        public async Task ConcatenatedEmptyGzipStreams()
        {
            const int copyToBufferSizeRequested = 0x8000;

            // we'll request a specific size buffer, but we cannot guarantee that's the size
            // that will be used since CopyTo will rent from the array pool
            // take advantage of this knowledge to find out what size it will actually use
            var rentedBuffer = ArrayPool<byte>.Shared.Rent(copyToBufferSizeRequested);
            int actualBufferSize = rentedBuffer.Length;
            ArrayPool<byte>.Shared.Return(rentedBuffer);

            // use 3 buffers-full so that we can prime the stream with the first buffer-full,
            // test that CopyTo successfully flushes this at the beginning of the operation, 
            // then populates the second buffer-full and reads its entirety despite every
            // payload being 0 length before it reads the final buffer-full.
            int minCompressedSize = 3 * actualBufferSize;

            using (Stream compressedStream = new DerivedMemoryStream())
            {
                using (var gz = new GZipStream(compressedStream, CompressionLevel.NoCompression, leaveOpen: true))
                {
                    // write one byte in order to allow us to prime the inflater buffer
                    gz.WriteByte(3);
                }

                while (compressedStream.Length < minCompressedSize)
                {
                    using (var gz = new GZipStream(compressedStream, CompressionLevel.NoCompression, leaveOpen: true))
                    {
                        gz.Write(Array.Empty<byte>());
                    }
                }

                compressedStream.Seek(0, SeekOrigin.Begin);
                using (Stream gz = new GZipStream(compressedStream, CompressionMode.Decompress, leaveOpen: true))
                using (Stream decompressedData = new DerivedMemoryStream())
                {
                    // read one byte in order to fill the inflater bufffer before copy
                    Assert.Equal(3, gz.ReadByte());

                    gz.CopyTo(decompressedData, copyToBufferSizeRequested);
                    Assert.Equal(0, decompressedData.Length);
                }

                compressedStream.Seek(0, SeekOrigin.Begin);
                using (Stream gz = new GZipStream(compressedStream, CompressionMode.Decompress, leaveOpen: true))
                using (Stream decompressedData = new DerivedMemoryStream())
                {
                    // read one byte in order to fill the inflater bufffer before copy
                    Assert.Equal(3, gz.ReadByte());

                    await gz.CopyToAsync(decompressedData, copyToBufferSizeRequested);
                    Assert.Equal(0, decompressedData.Length);
                }
            }
        }

        [Theory]
        [InlineData(1000, TestScenario.Read, 1000, 1)]
        [InlineData(1000, TestScenario.ReadByte, 0, 1)]
        [InlineData(1000, TestScenario.ReadAsync, 1000, 1)]
        [InlineData(1000, TestScenario.Copy, 1000, 1)]
        [InlineData(1000, TestScenario.CopyAsync, 1000, 1)]
        [InlineData(10, TestScenario.Read, 1000, 2000)]
        [InlineData(10, TestScenario.ReadByte, 0, 2000)]
        [InlineData(10, TestScenario.ReadAsync, 1000, 2000)]
        [InlineData(10, TestScenario.Copy, 1000, 2000)]
        [InlineData(10, TestScenario.CopyAsync, 1000, 2000)]
        [InlineData(2, TestScenario.Copy, 1000, 0x2000-30)]
        [InlineData(2, TestScenario.CopyAsync, 1000, 0x2000 - 30)]
        [InlineData(1000, TestScenario.Read, 1, 1)]
        [InlineData(1000, TestScenario.ReadAsync, 1, 1)]
        [InlineData(1000, TestScenario.Read, 1001 * 24, 1)]
        [InlineData(1000, TestScenario.ReadAsync, 1001 * 24, 1)]
        [InlineData(1000, TestScenario.Copy, 1001 * 24, 1)]
        [InlineData(1000, TestScenario.CopyAsync, 1001 * 24, 1)]
        public async Task ManyConcatenatedGzipStreams(int streamCount, TestScenario scenario, int bufferSize, int bytesPerStream)
        {
            await TestConcatenatedGzipStreams(streamCount, scenario, bufferSize, bytesPerStream);
        }

        [Theory]
        [OuterLoop("Tests take a very long time to complete")]
        [InlineData(400000, TestScenario.Read, 1000, 1)]
        [InlineData(400000, TestScenario.ReadAsync, 1000, 1)]
        [InlineData(400000, TestScenario.Copy, 1000, 1)]
        [InlineData(400000, TestScenario.CopyAsync, 1000, 1)]
        [InlineData(1000, TestScenario.Read, 1000, 20000)]
        [InlineData(1000, TestScenario.ReadByte, 0, 20000)]
        [InlineData(1000, TestScenario.ReadAsync, 1000, 9000)]
        [InlineData(1000, TestScenario.Read, 1, 9000)]
        [InlineData(1000, TestScenario.ReadAsync, 1, 9000)]
        [InlineData(1000, TestScenario.Read, 1001 * 24, 9000)]
        [InlineData(1000, TestScenario.ReadAsync, 1001 * 24, 9000)]
        [InlineData(1000, TestScenario.Copy, 1001 * 24, 9000)]
        [InlineData(1000, TestScenario.CopyAsync, 1001 * 24, 9000)]
        public async Task ManyManyConcatenatedGzipStreams(int streamCount, TestScenario scenario, int bufferSize, int bytesPerStream)
        {
            await TestConcatenatedGzipStreams(streamCount, scenario, bufferSize, bytesPerStream);
        }

        public enum TestScenario
        {
            ReadByte,
            Read,
            ReadAsync,
            Copy,
            CopyAsync
        }

        private async Task TestConcatenatedGzipStreams(int streamCount, TestScenario scenario, int bufferSize, int bytesPerStream = 1)
        {
            bool isCopy = scenario == TestScenario.Copy || scenario == TestScenario.CopyAsync;

            using (MemoryStream correctDecompressedOutput = new MemoryStream())
            // For copy scenarios use a derived MemoryStream to avoid MemoryStream's Copy optimization 
            // that turns the Copy into a single Write passing the backing buffer
            using (MemoryStream compressedStream = isCopy ? new DerivedMemoryStream() : new MemoryStream())  
            using (MemoryStream decompressorOutput = new MemoryStream())
            {
                for (int i = 0; i < streamCount; i++)
                {
                    using (var gz = new GZipStream(compressedStream, CompressionLevel.NoCompression, true))
                    {
                        for (int j = 0; j < bytesPerStream; j++)
                        {
                            byte b = (byte)((i * j) % 256);
                            gz.WriteByte(b);
                            correctDecompressedOutput.WriteByte(b);
                        }
                    }
                }
                compressedStream.Seek(0, SeekOrigin.Begin);

                var decompressor = CreateStream(compressedStream, CompressionMode.Decompress);

                var bytes = new byte[bufferSize];
                bool finished = false;
                int retCount = 0, totalRead = 0;
                while (!finished)
                {
                    switch (scenario)
                    {
                        case TestScenario.ReadAsync:
                            try
                            {
                                retCount = await decompressor.ReadAsync(bytes, 0, bufferSize);
                                totalRead += retCount;
                                if (retCount != 0)
                                    await decompressorOutput.WriteAsync(bytes, 0, retCount);
                                else
                                    finished = true;
                            }
                            catch (Exception)
                            {
                                throw new Exception(retCount + " " + totalRead);
                            }
                            break;
                        case TestScenario.ReadByte:
                            int b = decompressor.ReadByte();

                            if (b != -1)
                                decompressorOutput.WriteByte((byte)b);
                            else
                                finished = true;

                            break;
                        case TestScenario.Read:
                            retCount = decompressor.Read(bytes, 0, bufferSize);

                            if (retCount != 0)
                                decompressorOutput.Write(bytes, 0, retCount);
                            else
                                finished = true;

                            break;
                        case TestScenario.Copy:
                            decompressor.CopyTo(decompressorOutput, bufferSize);
                            finished = true;
                            break;
                        case TestScenario.CopyAsync:
                            await decompressor.CopyToAsync(decompressorOutput, bufferSize);
                            finished = true;
                            break;
                    }
                }
                decompressor.Dispose();
                decompressorOutput.Position = 0;

                byte[] decompressorOutputBytes = decompressorOutput.ToArray();
                byte[] correctOutputBytes = correctDecompressedOutput.ToArray();

                Assert.Equal(correctOutputBytes.Length, decompressorOutputBytes.Length);
                for (int i = 0; i < correctOutputBytes.Length; i++)
                {
                    Assert.Equal(correctOutputBytes[i], decompressorOutputBytes[i]);
                }
            }
        }

        [Fact]
        public void DerivedStream_ReadWriteSpan_UsesReadWriteArray()
        {
            var ms = new MemoryStream();
            using (var compressor = new DerivedGZipStream(ms, CompressionMode.Compress, leaveOpen: true))
            {
                compressor.Write(new Span<byte>(new byte[1]));
                Assert.True(compressor.WriteArrayInvoked);
            }
            ms.Position = 0;
            using (var compressor = new DerivedGZipStream(ms, CompressionMode.Decompress, leaveOpen: true))
            {
                compressor.Read(new Span<byte>(new byte[1]));
                Assert.True(compressor.ReadArrayInvoked);
            }
            ms.Position = 0;
            using (var compressor = new DerivedGZipStream(ms, CompressionMode.Decompress, leaveOpen: true))
            {
                compressor.ReadAsync(new Memory<byte>(new byte[1])).AsTask().Wait();
                Assert.True(compressor.ReadArrayInvoked);
            }
            ms.Position = 0;
            using (var compressor = new DerivedGZipStream(ms, CompressionMode.Compress, leaveOpen: true))
            {
                compressor.WriteAsync(new ReadOnlyMemory<byte>(new byte[1])).AsTask().Wait();
                Assert.True(compressor.WriteArrayInvoked);
            }
        }

        [Theory]
        [InlineData(false, false)]
        [InlineData(false, true)]
        [InlineData(true, false)]
        [InlineData(true, false)]
        public async Task DisposeAsync_Flushes(bool derived, bool leaveOpen)
        {
            var ms = new MemoryStream();
            var gs = derived ?
                new DerivedGZipStream(ms, CompressionMode.Compress, leaveOpen) :
                new GZipStream(ms, CompressionMode.Compress, leaveOpen);
            gs.WriteByte(1);
            await gs.FlushAsync();

            long pos = ms.Position;
            gs.WriteByte(1);
            Assert.Equal(pos, ms.Position);

            await gs.DisposeAsync();
            Assert.InRange(ms.ToArray().Length, pos + 1, int.MaxValue);
            if (leaveOpen)
            {
                Assert.InRange(ms.Position, pos + 1, int.MaxValue);
            }
            else
            {
                Assert.Throws<ObjectDisposedException>(() => ms.Position);
            }
        }

        [Theory]
        [InlineData(false, false)]
        [InlineData(false, true)]
        [InlineData(true, false)]
        [InlineData(true, false)]
        public async Task DisposeAsync_MultipleCallsAllowed(bool derived, bool leaveOpen)
        {
            using (var gs = derived ?
                new DerivedGZipStream(new MemoryStream(), CompressionMode.Compress, leaveOpen) :
                new GZipStream(new MemoryStream(), CompressionMode.Compress, leaveOpen))
            {
                await gs.DisposeAsync();
                await gs.DisposeAsync();
            }
        }

        private sealed class DerivedGZipStream : GZipStream
        {
            public bool ReadArrayInvoked = false, WriteArrayInvoked = false;
            internal DerivedGZipStream(Stream stream, CompressionMode mode) : base(stream, mode) { }
            internal DerivedGZipStream(Stream stream, CompressionMode mode, bool leaveOpen) : base(stream, mode, leaveOpen) { }

            public override int Read(byte[] array, int offset, int count)
            {
                ReadArrayInvoked = true;
                return base.Read(array, offset, count);
            }

            public override Task<int> ReadAsync(byte[] array, int offset, int count, CancellationToken cancellationToken)
            {
                ReadArrayInvoked = true;
                return base.ReadAsync(array, offset, count, cancellationToken);
            }

            public override void Write(byte[] array, int offset, int count)
            {
                WriteArrayInvoked = true;
                base.Write(array, offset, count);
            }

            public override Task WriteAsync(byte[] array, int offset, int count, CancellationToken cancellationToken)
            {
                WriteArrayInvoked = true;
                return base.WriteAsync(array, offset, count, cancellationToken);
            }
        }
    }
}
