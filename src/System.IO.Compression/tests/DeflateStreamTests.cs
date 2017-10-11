// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace System.IO.Compression.Tests
{
    public class DeflateStreamTests
    {
        static string gzTestFile(string fileName) => Path.Combine("GZTestData", fileName);

        [Fact]
        public void BaseStream1()
        {
            var writeStream = new MemoryStream();
            var zip = new DeflateStream(writeStream, CompressionMode.Compress);

            Assert.Same(zip.BaseStream, writeStream);
            writeStream.Dispose();
        }

        [Fact]
        public void BaseStream2()
        {
            var ms = new MemoryStream();
            var zip = new DeflateStream(ms, CompressionMode.Decompress);

            Assert.Same(zip.BaseStream, ms);
            ms.Dispose();
        }

        [Fact]
        public async Task ModifyBaseStream()
        {
            var ms = await LocalMemoryStream.readAppFileAsync(gzTestFile("GZTestDocument.txt.gz"));
            var newMs = StripHeaderAndFooter.Strip(ms);

            var zip = new DeflateStream(newMs, CompressionMode.Decompress);
            int size = 1024;
            byte[] bytes = new byte[size];
            zip.BaseStream.Read(bytes, 0, size); // This will throw if the underlying stream is not writable as expected

            zip.BaseStream.Position = 0;
            await zip.BaseStream.ReadAsync(bytes, 0, size);
        }

        [Fact]
        public void DecompressCanRead()
        {
            var ms = new MemoryStream();
            var zip = new DeflateStream(ms, CompressionMode.Decompress);

            Assert.True(zip.CanRead);

            zip.Dispose();
            Assert.False(zip.CanRead);
        }

        [Fact]
        public void CompressCanWrite()
        {
            var ms = new MemoryStream();
            var zip = new DeflateStream(ms, CompressionMode.Compress);
            Assert.True(zip.CanWrite);

            zip.Dispose();
            Assert.False(zip.CanWrite);
        }

        [Fact]
        public void CanDisposeBaseStream()
        {
            var ms = new MemoryStream();
            var zip = new DeflateStream(ms, CompressionMode.Compress);
            ms.Dispose(); // This would throw if this was invalid
        }

        [Fact]
        public void CanDisposeDeflateStream()
        {
            var ms = new MemoryStream();
            var zip = new DeflateStream(ms, CompressionMode.Compress);
            zip.Dispose();

            // Base Stream should be null after dispose
            Assert.Null(zip.BaseStream);

            zip.Dispose(); // Should be a no-op
        }

        [Fact]
        public async Task CanReadBaseStreamAfterDispose()
        {
            var ms = await LocalMemoryStream.readAppFileAsync(gzTestFile("GZTestDocument.txt.gz"));
            var newMs = StripHeaderAndFooter.Strip(ms);

            var zip = new DeflateStream(newMs, CompressionMode.Decompress, leaveOpen: true);
            var baseStream = zip.BaseStream;
            zip.Dispose();

            int size = 1024;
            byte[] bytes = new byte[size];
            baseStream.Read(bytes, 0, size); // This will throw if the underlying stream is not writable as expected

            baseStream.Position = 0;
            await baseStream.ReadAsync(bytes, 0, size);
        }

        [Fact]
        public async Task DecompressFailsWithRealGzStream()
        {
            string[] files = { gzTestFile("GZTestDocument.doc.gz"), gzTestFile("GZTestDocument.txt.gz") };
            foreach (string fileName in files)
            {
                var baseStream = await LocalMemoryStream.readAppFileAsync(fileName);
                var zip = new DeflateStream(baseStream, CompressionMode.Decompress);
                int _bufferSize = 2048;
                var bytes = new byte[_bufferSize];
                Assert.Throws<InvalidDataException>(() => { zip.Read(bytes, 0, _bufferSize); });
                zip.Dispose();
            }
        }

        [Fact]
        public void DisposedBaseStreamThrows()
        {
            var ms = new MemoryStream();
            ms.Dispose();
            AssertExtensions.Throws<ArgumentException>("stream", () =>
            {
                var deflate = new DeflateStream(ms, CompressionMode.Decompress);
            });

            AssertExtensions.Throws<ArgumentException>("stream", () =>
            {
                var deflate = new DeflateStream(ms, CompressionMode.Compress);
            });
        }

        [Fact]
        public void ReadOnlyStreamThrowsOnCompress()
        {
            var ms = new LocalMemoryStream();
            ms.SetCanWrite(false);

            AssertExtensions.Throws<ArgumentException>("stream", () =>
            {
                var gzip = new DeflateStream(ms, CompressionMode.Compress);
            });
        }

        [Fact]
        public void WriteOnlyStreamThrowsOnDecompress()
        {
            var ms = new LocalMemoryStream();
            ms.SetCanRead(false);

            AssertExtensions.Throws<ArgumentException>("stream", () =>
            {
                var gzip = new DeflateStream(ms, CompressionMode.Decompress);
            });
        }

        [Fact]
        public void TestCtors()
        {
            CompressionLevel[] legalValues = new CompressionLevel[] { CompressionLevel.Optimal, CompressionLevel.Fastest, CompressionLevel.NoCompression };

            foreach (CompressionLevel level in legalValues)
            {
                bool[] boolValues = new bool[] { true, false };

                foreach (bool remainsOpen in boolValues)
                {
                    TestCtor(level, remainsOpen);
                }
            }
        }

        [Fact]
        public void TestLevelOptimial()
        {
            TestCtor(CompressionLevel.Optimal);
        }

        [Fact]
        public void TestLevelNoCompression()
        {
            TestCtor(CompressionLevel.NoCompression);
        }

        [Fact]
        public void TestLevelFastest()
        {
            TestCtor(CompressionLevel.Fastest);
        }

        private static void TestCtor(CompressionLevel level, bool? leaveOpen = null)
        {
            //Create the DeflateStream
            int _bufferSize = 1024;
            var bytes = new byte[_bufferSize];
            var baseStream = new MemoryStream(bytes, writable: true);
            DeflateStream ds;

            if (leaveOpen == null)
            {
                ds = new DeflateStream(baseStream, level);
            }
            else
            {
                ds = new DeflateStream(baseStream, level, leaveOpen ?? false);
            }

            //Write some data and Close the stream
            string strData = "Test Data";
            var encoding = Encoding.UTF8;
            byte[] data = encoding.GetBytes(strData);
            ds.Write(data, 0, data.Length);
            ds.Flush();
            ds.Dispose();

            if (leaveOpen != true)
            {
                //Check that Close has really closed the underlying stream
                Assert.Throws<ObjectDisposedException>(() => { baseStream.Write(bytes, 0, bytes.Length); });
            }

            //Read the data
            byte[] data2 = new byte[_bufferSize];
            baseStream = new MemoryStream(bytes, writable: false);
            ds = new DeflateStream(baseStream, CompressionMode.Decompress);
            int size = ds.Read(data2, 0, _bufferSize - 5);

            //Verify the data roundtripped
            for (int i = 0; i < size + 5; i++)
            {
                if (i < data.Length)
                {
                    Assert.Equal(data[i], data2[i]);
                }
                else
                {
                    Assert.Equal(data2[i], (byte)0);
                }
            }
        }

        [Fact]
        public void CtorArgumentValidation()
        {
            Assert.Throws<ArgumentNullException>(() => new DeflateStream(null, CompressionLevel.Fastest));
            Assert.Throws<ArgumentNullException>(() => new DeflateStream(null, CompressionMode.Decompress));
            Assert.Throws<ArgumentNullException>(() => new DeflateStream(null, CompressionMode.Compress));

            Assert.Throws<ArgumentNullException>(() => new DeflateStream(null, CompressionLevel.Fastest, true));
            Assert.Throws<ArgumentNullException>(() => new DeflateStream(null, CompressionMode.Decompress, false));
            Assert.Throws<ArgumentNullException>(() => new DeflateStream(null, CompressionMode.Compress, true));

            AssertExtensions.Throws<ArgumentException>("mode", () => new DeflateStream(new MemoryStream(), (CompressionMode)42));
            AssertExtensions.Throws<ArgumentException>("mode", () => new DeflateStream(new MemoryStream(), (CompressionMode)43, true));

            AssertExtensions.Throws<ArgumentException>("stream", () => new DeflateStream(new MemoryStream(new byte[1], writable: false), CompressionLevel.Optimal));
        }

        [Fact]
        public async Task Flush()
        {
            var ms = new MemoryStream();
            var ds = new DeflateStream(ms, CompressionMode.Compress);
            ds.Flush();
            await ds.FlushAsync();
        }

        [Fact]
        public void DoubleFlush()
        {
            var ms = new MemoryStream();
            var ds = new DeflateStream(ms, CompressionMode.Compress);
            ds.Flush();
            ds.Flush();
        }

        [Fact]
        public void DoubleDispose()
        {
            var ms = new MemoryStream();
            var ds = new DeflateStream(ms, CompressionMode.Compress);
            ds.Dispose();
            ds.Dispose();
        }

        [Fact]
        public void FlushThenDispose()
        {
            var ms = new MemoryStream();
            var ds = new DeflateStream(ms, CompressionMode.Compress);
            ds.Flush();
            ds.Dispose();
        }

        [Fact]
        public void FlushFailsAfterDispose()
        {
            var ms = new MemoryStream();
            var ds = new DeflateStream(ms, CompressionMode.Compress);
            ds.Dispose();
            Assert.Throws<ObjectDisposedException>(() => { ds.Flush(); });
        }

        [Fact]
        public async Task FlushAsyncFailsAfterDispose()
        {
            var ms = new MemoryStream();
            var ds = new DeflateStream(ms, CompressionMode.Compress);
            ds.Dispose();

            await Assert.ThrowsAsync<ObjectDisposedException>(async () =>
            {
                await ds.FlushAsync();
            });
        }

        [Fact]
        public void TestSeekMethodsDecompress()
        {
            var ms = new MemoryStream();
            var zip = new DeflateStream(ms, CompressionMode.Decompress);

            Assert.False(zip.CanSeek, "CanSeek should be false");

            Assert.Throws<NotSupportedException>(delegate { long value = zip.Length; });
            Assert.Throws<NotSupportedException>(delegate { long value = zip.Position; });
            Assert.Throws<NotSupportedException>(delegate { zip.Position = 100L; });
            Assert.Throws<NotSupportedException>(delegate { zip.SetLength(100L); });
            Assert.Throws<NotSupportedException>(delegate { zip.Seek(100L, SeekOrigin.Begin); });
        }

        [Fact]
        public void TestSeekMethodsCompress()
        {
            var ms = new MemoryStream();
            var zip = new DeflateStream(ms, CompressionMode.Compress);

            Assert.False(zip.CanSeek, "CanSeek should be false");

            Assert.Throws<NotSupportedException>(delegate { long value = zip.Length; });
            Assert.Throws<NotSupportedException>(delegate { long value = zip.Position; });
            Assert.Throws<NotSupportedException>(delegate { zip.Position = 100L; });
            Assert.Throws<NotSupportedException>(delegate { zip.SetLength(100L); });
            Assert.Throws<NotSupportedException>(delegate { zip.Seek(100L, SeekOrigin.Begin); });
        }

        [Fact]
        public void ReadWriteArgumentValidation()
        {
            using (var ds = new DeflateStream(new MemoryStream(), CompressionMode.Compress))
            {
                Assert.Throws<ArgumentNullException>(() => ds.Write(null, 0, 0));
                Assert.Throws<ArgumentOutOfRangeException>(() => ds.Write(new byte[1], -1, 0));
                Assert.Throws<ArgumentOutOfRangeException>(() => ds.Write(new byte[1], 0, -1));
                AssertExtensions.Throws<ArgumentException>(null, () => ds.Write(new byte[1], 0, 2));
                AssertExtensions.Throws<ArgumentException>(null, () => ds.Write(new byte[1], 1, 1));
                Assert.Throws<InvalidOperationException>(() => ds.Read(new byte[1], 0, 1));
                ds.Write(new byte[1], 0, 0);
            }
            using (var ds = new DeflateStream(new MemoryStream(), CompressionMode.Compress))
            {
                Assert.Throws<ArgumentNullException>(() => { ds.WriteAsync(null, 0, 0); });
                Assert.Throws<ArgumentOutOfRangeException>(() => { ds.WriteAsync(new byte[1], -1, 0); });
                Assert.Throws<ArgumentOutOfRangeException>(() => { ds.WriteAsync(new byte[1], 0, -1); });
                AssertExtensions.Throws<ArgumentException>(null, () => { ds.WriteAsync(new byte[1], 0, 2); });
                AssertExtensions.Throws<ArgumentException>(null, () => { ds.WriteAsync(new byte[1], 1, 1); });
                Assert.Throws<InvalidOperationException>(() => { ds.Read(new byte[1], 0, 1); });
            }

            using (var ds = new DeflateStream(new MemoryStream(), CompressionMode.Decompress))
            {
                Assert.Throws<ArgumentNullException>(() => ds.Read(null, 0, 0));
                Assert.Throws<ArgumentOutOfRangeException>(() => ds.Read(new byte[1], -1, 0));
                Assert.Throws<ArgumentOutOfRangeException>(() => ds.Read(new byte[1], 0, -1));
                AssertExtensions.Throws<ArgumentException>(null, () => ds.Read(new byte[1], 0, 2));
                AssertExtensions.Throws<ArgumentException>(null, () => ds.Read(new byte[1], 1, 1));
                Assert.Throws<InvalidOperationException>(() => ds.Write(new byte[1], 0, 1));

                var data = new byte[1] { 42 };
                Assert.Equal(0, ds.Read(data, 0, 0));
                Assert.Equal(42, data[0]);
            }
            using (var ds = new DeflateStream(new MemoryStream(), CompressionMode.Decompress))
            {
                Assert.Throws<ArgumentNullException>(() => { ds.ReadAsync(null, 0, 0); });
                Assert.Throws<ArgumentOutOfRangeException>(() => { ds.ReadAsync(new byte[1], -1, 0); });
                Assert.Throws<ArgumentOutOfRangeException>(() => { ds.ReadAsync(new byte[1], 0, -1); });
                AssertExtensions.Throws<ArgumentException>(null, () => { ds.ReadAsync(new byte[1], 0, 2); });
                AssertExtensions.Throws<ArgumentException>(null, () => { ds.ReadAsync(new byte[1], 1, 1); });
                Assert.Throws<InvalidOperationException>(() => { ds.Write(new byte[1], 0, 1); });
            }
        }

        [Fact]
        public void CopyToAsyncArgumentValidation()
        {
            using (DeflateStream ds = new DeflateStream(new MemoryStream(), CompressionMode.Decompress))
            {
                AssertExtensions.Throws<ArgumentNullException>("destination", () => { ds.CopyToAsync(null); });
                AssertExtensions.Throws<ArgumentOutOfRangeException>("bufferSize", () => { ds.CopyToAsync(new MemoryStream(), 0); });
                Assert.Throws<NotSupportedException>(() => { ds.CopyToAsync(new MemoryStream(new byte[1], writable: false)); });
                ds.Dispose();
                Assert.Throws<ObjectDisposedException>(() => { ds.CopyToAsync(new MemoryStream()); });
            }
            using (DeflateStream ds = new DeflateStream(new MemoryStream(), CompressionMode.Compress))
            {
                Assert.Throws<NotSupportedException>(() => { ds.CopyToAsync(new MemoryStream()); });
            }
        }

        [Fact]
        [SkipOnTargetFramework(TargetFrameworkMonikers.NetFramework, "Full Framework Flush is a no-op.")]
        public void Precancellation()
        {
            var ms = new MemoryStream();
            using (DeflateStream ds = new DeflateStream(ms, CompressionMode.Compress, leaveOpen: true))
            {
                Assert.True(ds.WriteAsync(new byte[1], 0, 1, new CancellationToken(true)).IsCanceled);
                Assert.True(ds.FlushAsync(new CancellationToken(true)).IsCanceled);
            }
            using (DeflateStream ds = new DeflateStream(ms, CompressionMode.Decompress, leaveOpen: true))
            {
                Assert.True(ds.ReadAsync(new byte[1], 0, 1, new CancellationToken(true)).IsCanceled);
            }
        }

        public enum ReadWriteMode
        {
            SyncArray,
            SyncSpan,
            AsyncArray,
            AsyncMemory
        }

        public static IEnumerable<object[]> RoundtripCompressDecompressOuterData
        {
            get
            {
                foreach (ReadWriteMode readWriteMode in new[] { ReadWriteMode.SyncArray, ReadWriteMode.SyncSpan, ReadWriteMode.AsyncArray, ReadWriteMode.AsyncMemory })
                {
                    foreach (bool useGzip in new[] { true, false }) // whether to add on gzip headers/footers
                    {
                        foreach (var level in new[] { CompressionLevel.Fastest, CompressionLevel.Optimal, CompressionLevel.NoCompression }) // compression level
                        {
                            yield return new object[] { readWriteMode, useGzip, 1, 5, level }; // smallest possible writes
                            yield return new object[] { readWriteMode, useGzip, 1023, 1023 * 10, level }; // overflowing internal buffer
                            yield return new object[] { readWriteMode, useGzip, 1024 * 1024, 1024 * 1024, level }; // large single write
                        }
                    }
                }
            }
        }

        [Fact]
        public async Task RoundtripCompressDecompress()
        {
            await RoundtripCompressDecompress(ReadWriteMode.SyncArray, useGzip: false, chunkSize: 1, totalSize: 10, level: CompressionLevel.Fastest);
            await RoundtripCompressDecompress(ReadWriteMode.AsyncArray, useGzip: true, chunkSize: 1024, totalSize: 8192, level: CompressionLevel.Optimal);
        }

        [OuterLoop]
        [Theory]
        [MemberData(nameof(RoundtripCompressDecompressOuterData))]
        public async Task RoundtripCompressDecompress(ReadWriteMode readWriteMode, bool useGzip, int chunkSize, int totalSize, CompressionLevel level)
        {
            byte[] data = new byte[totalSize];
            new Random(42).NextBytes(data);

            var compressed = new MemoryStream();
            using (var compressor = useGzip ? (Stream)new GZipStream(compressed, level, true) : new DeflateStream(compressed, level, true))
            {
                for (int i = 0; i < data.Length; i += chunkSize) // not using CopyTo{Async} due to optimizations in MemoryStream's implementation that avoid what we're trying to test
                {
                    switch (readWriteMode)
                    {
                        case ReadWriteMode.AsyncArray:
                            await compressor.WriteAsync(data, i, chunkSize);
                            break;
                        case ReadWriteMode.SyncArray:
                            compressor.Write(data, i, chunkSize);
                            break;
                        case ReadWriteMode.SyncSpan:
                            compressor.Write(new ReadOnlySpan<byte>(data, i, chunkSize));
                            break;
                        case ReadWriteMode.AsyncMemory:
                            await compressor.WriteAsync(new ReadOnlyMemory<byte>(data, i, chunkSize));
                            break;
                    }
                }
            }
            compressed.Position = 0;
            await ReadAndValidateCompressedData(readWriteMode, useGzip, chunkSize, compressed, data);
            compressed.Dispose();
        }

        [Fact]
        [SkipOnTargetFramework(TargetFrameworkMonikers.NetFramework, "Full Framework Flush is a no-op.")]
        public async Task RoundTripWithFlush()
        {
            await RoundTripWithFlush(ReadWriteMode.SyncArray, useGzip: false, chunkSize: 1, totalSize: 10, level: CompressionLevel.Fastest);
            await RoundTripWithFlush(ReadWriteMode.AsyncArray, useGzip: true, chunkSize: 1024, totalSize: 8192, level: CompressionLevel.Optimal);
        }

        [OuterLoop]
        [Theory]
        [MemberData(nameof(RoundtripCompressDecompressOuterData))]
        [SkipOnTargetFramework(TargetFrameworkMonikers.NetFramework, "Full Framework Flush is a no-op.")]
        public async Task RoundTripWithFlush(ReadWriteMode readWriteMode, bool useGzip, int chunkSize, int totalSize, CompressionLevel level)
        {
            byte[] data = new byte[totalSize];
            new Random(42).NextBytes(data);

            using (var compressed = new MemoryStream())
            using (var compressor = useGzip ? (Stream)new GZipStream(compressed, level, true) : new DeflateStream(compressed, level, true))
            {
                for (int i = 0; i < data.Length; i += chunkSize) // not using CopyTo{Async} due to optimizations in MemoryStream's implementation that avoid what we're trying to test
                {
                    switch (readWriteMode)
                    {
                        case ReadWriteMode.AsyncArray:
                            await compressor.WriteAsync(data, i, chunkSize);
                            break;
                        case ReadWriteMode.SyncArray:
                            compressor.Write(data, i, chunkSize);
                            break;
                        case ReadWriteMode.SyncSpan:
                            compressor.Write(new ReadOnlySpan<byte>(data, i, chunkSize));
                            break;
                        case ReadWriteMode.AsyncMemory:
                            await compressor.WriteAsync(new ReadOnlyMemory<byte>(data, i, chunkSize));
                            break;
                    }
                }
                switch (readWriteMode)
                {
                    case ReadWriteMode.AsyncArray:
                    case ReadWriteMode.AsyncMemory:
                        await compressor.FlushAsync();
                        break;
                    case ReadWriteMode.SyncSpan:
                    case ReadWriteMode.SyncArray:
                        compressor.Flush();
                        break;
                }
                compressed.Position = 0;
                await ReadAndValidateCompressedData(readWriteMode, useGzip, chunkSize, compressed, data);
            }
        }

        [Fact]
        [SkipOnTargetFramework(TargetFrameworkMonikers.NetFramework, "Full Framework Flush is a no-op.")]
        public async Task WriteAfterFlushing()
        {
            await WriteAfterFlushing(ReadWriteMode.SyncArray, useGzip: false, chunkSize: 1, totalSize: 10, level: CompressionLevel.Fastest);
            await WriteAfterFlushing(ReadWriteMode.AsyncArray, useGzip: true, chunkSize: 1024, totalSize: 8192, level: CompressionLevel.Optimal);
        }

        [OuterLoop]
        [Theory]
        [MemberData(nameof(RoundtripCompressDecompressOuterData))]
        [SkipOnTargetFramework(TargetFrameworkMonikers.NetFramework, "Full Framework Flush is a no-op.")]
        public async Task WriteAfterFlushing(ReadWriteMode readWriteMode, bool useGzip, int chunkSize, int totalSize, CompressionLevel level)
        {
            byte[] data = new byte[totalSize];
            List<byte> expected = new List<byte>();
            new Random(42).NextBytes(data);

            using (var compressed = new MemoryStream())
            using (var compressor = useGzip ? (Stream)new GZipStream(compressed, level, true) : new DeflateStream(compressed, level, true))
            {
                for (int i = 0; i < data.Length; i += chunkSize) // not using CopyTo{Async} due to optimizations in MemoryStream's implementation that avoid what we're trying to test
                {
                    switch (readWriteMode)
                    {
                        case ReadWriteMode.AsyncArray:
                            await compressor.WriteAsync(data, i, chunkSize);
                            break;
                        case ReadWriteMode.SyncArray:
                            compressor.Write(data, i, chunkSize);
                            break;
                        case ReadWriteMode.SyncSpan:
                            compressor.Write(new ReadOnlySpan<byte>(data, i, chunkSize));
                            break;
                        case ReadWriteMode.AsyncMemory:
                            await compressor.WriteAsync(new ReadOnlyMemory<byte>(data, i, chunkSize));
                            break;
                    }
                    for (int j = i; j < i + chunkSize; j++)
                        expected.Insert(j, data[j]);

                    switch (readWriteMode)
                    {
                        case ReadWriteMode.AsyncArray:
                        case ReadWriteMode.AsyncMemory:
                            await compressor.FlushAsync();
                            break;
                        case ReadWriteMode.SyncSpan:
                        case ReadWriteMode.SyncArray:
                            compressor.Flush();
                            break;
                    }

                    MemoryStream partiallyCompressed = new MemoryStream(compressed.ToArray());
                    partiallyCompressed.Position = 0;
                    await ReadAndValidateCompressedData(readWriteMode, useGzip, chunkSize, partiallyCompressed, expected.ToArray());
                }
            }
        }

        [Fact]
        [SkipOnTargetFramework(TargetFrameworkMonikers.NetFramework, "Full Framework Flush is a no-op.")]
        public async Task FlushBeforeFirstWrites()
        {
            await FlushBeforeFirstWrites(ReadWriteMode.SyncArray, useGzip: false, chunkSize: 1, totalSize: 10, level: CompressionLevel.Fastest);
            await FlushBeforeFirstWrites(ReadWriteMode.AsyncArray, useGzip: true, chunkSize: 1024, totalSize: 8192, level: CompressionLevel.Optimal);
        }

        [OuterLoop]
        [Theory]
        [MemberData(nameof(RoundtripCompressDecompressOuterData))]
        [SkipOnTargetFramework(TargetFrameworkMonikers.NetFramework, "Full Framework Flush is a no-op.")]
        public async Task FlushBeforeFirstWrites(ReadWriteMode readWriteMode, bool useGzip, int chunkSize, int totalSize, CompressionLevel level)
        {
            byte[] data = new byte[totalSize];
            new Random(42).NextBytes(data);

            using (var compressed = new MemoryStream())
            using (var compressor = useGzip ? (Stream)new GZipStream(compressed, level, true) : new DeflateStream(compressed, level, true))
            {
                switch (readWriteMode)
                {
                    case ReadWriteMode.AsyncArray:
                    case ReadWriteMode.AsyncMemory:
                        await compressor.FlushAsync();
                        break;
                    case ReadWriteMode.SyncSpan:
                    case ReadWriteMode.SyncArray:
                        compressor.Flush();
                        break;
                }

                for (int i = 0; i < data.Length; i += chunkSize) // not using CopyTo{Async} due to optimizations in MemoryStream's implementation that avoid what we're trying to test
                {
                    switch (readWriteMode)
                    {
                        case ReadWriteMode.AsyncArray:
                            await compressor.WriteAsync(data, i, chunkSize);
                            break;
                        case ReadWriteMode.SyncArray:
                            compressor.Write(data, i, chunkSize);
                            break;
                        case ReadWriteMode.SyncSpan:
                            compressor.Write(new ReadOnlySpan<byte>(data, i, chunkSize));
                            break;
                        case ReadWriteMode.AsyncMemory:
                            await compressor.WriteAsync(new ReadOnlyMemory<byte>(data, i, chunkSize));
                            break;
                    }
                }

                switch (readWriteMode)
                {
                    case ReadWriteMode.AsyncArray:
                    case ReadWriteMode.AsyncMemory:
                        await compressor.FlushAsync();
                        break;
                    case ReadWriteMode.SyncSpan:
                    case ReadWriteMode.SyncArray:
                        compressor.Flush();
                        break;
                }
                compressed.Position = 0;
                await ReadAndValidateCompressedData(readWriteMode, useGzip, chunkSize, compressed, data);
            }
        }

        /// <summary>
        /// Given a MemoryStream of compressed data and a byte array of desired output, decompresses
        /// the stream and validates that it is equal to the expected array.
        /// </summary>
        private async Task ReadAndValidateCompressedData(ReadWriteMode readWriteMode, bool useGzip, int chunkSize, MemoryStream compressed, byte[] expected)
        {
            using (MemoryStream decompressed = new MemoryStream())
            using (Stream decompressor = useGzip ? (Stream)new GZipStream(compressed, CompressionMode.Decompress, true) : new DeflateStream(compressed, CompressionMode.Decompress, true))
            {
                int bytesRead;
                var buffer = new byte[chunkSize];
                switch (readWriteMode)
                {
                    case ReadWriteMode.SyncSpan:
                        while ((bytesRead = decompressor.Read(new Span<byte>(buffer))) != 0)
                        {
                            decompressed.Write(buffer, 0, bytesRead);
                        }
                        break;
                    case ReadWriteMode.SyncArray:
                        while ((bytesRead = decompressor.Read(buffer, 0, buffer.Length)) != 0)
                        {
                            decompressed.Write(buffer, 0, bytesRead);
                        }
                        break;
                    case ReadWriteMode.AsyncArray:
                        while ((bytesRead = await decompressor.ReadAsync(buffer, 0, buffer.Length)) != 0)
                        {
                            decompressed.Write(buffer, 0, bytesRead);
                        }
                        break;
                    case ReadWriteMode.AsyncMemory:
                        while ((bytesRead = await decompressor.ReadAsync(new Memory<byte>(buffer))) != 0)
                        {
                            decompressed.Write(buffer, 0, bytesRead);
                        }
                        break;
                }
                Assert.Equal<byte>(expected, decompressed.ToArray());
            }
        }

        [Theory]
        [InlineData(ReadWriteMode.SyncArray, false)]
        [InlineData(ReadWriteMode.AsyncArray, false)]
        [InlineData(ReadWriteMode.SyncSpan, false)]
        [InlineData(ReadWriteMode.SyncSpan, true)]
        [InlineData(ReadWriteMode.AsyncMemory, false)]
        [InlineData(ReadWriteMode.AsyncMemory, true)]
        public async Task SequentialReadsOnMemoryStream_Return_SameBytes(ReadWriteMode readWriteMode, bool derived)
        {
            byte[] data = new byte[1024 * 10];
            new Random(42).NextBytes(data);

            var compressed = new MemoryStream();
            using (var compressor = derived ?
                new DerivedDeflateStream(compressed, CompressionMode.Compress, true) :
                new DeflateStream(compressed, CompressionMode.Compress, true))
            {
                for (int i = 0; i < data.Length; i += 1024)
                {
                    switch (readWriteMode)
                    {
                        case ReadWriteMode.SyncArray: compressor.Write(data, i, 1024); break;
                        case ReadWriteMode.AsyncArray: await compressor.WriteAsync(data, i, 1024); break;
                        case ReadWriteMode.SyncSpan: compressor.Write(new Span<byte>(data, i, 1024)); break;
                        case ReadWriteMode.AsyncMemory: await compressor.WriteAsync(new ReadOnlyMemory<byte>(data, i, 1024)); break;
                    }
                }

                Assert.Equal(
                    derived && (readWriteMode == ReadWriteMode.SyncArray || readWriteMode == ReadWriteMode.SyncSpan),
                    compressor is DerivedDeflateStream dds && dds.WriteArrayInvoked);
            }
            compressed.Position = 0;

            using (var decompressor = derived ?
                new DerivedDeflateStream(compressed, CompressionMode.Decompress, true) :
                new DeflateStream(compressed, CompressionMode.Decompress, true))
            {
                int i, j;
                byte[] array = new byte[100];
                byte[] array2 = new byte[100];

                // only read in the first 100 bytes
                switch (readWriteMode)
                {
                    case ReadWriteMode.SyncArray: decompressor.Read(array, 0, array.Length); break;
                    case ReadWriteMode.AsyncArray: await decompressor.ReadAsync(array, 0, array.Length); break;
                    case ReadWriteMode.SyncSpan: decompressor.Read(new Span<byte>(array)); break;
                    case ReadWriteMode.AsyncMemory: await decompressor.ReadAsync(new Memory<byte>(array)); break;
                }
                for (i = 0; i < array.Length; i++)
                {
                    Assert.Equal(data[i], array[i]);
                }

                // read in the next 100 bytes and make sure nothing is missing
                switch (readWriteMode)
                {
                    case ReadWriteMode.SyncArray: decompressor.Read(array2, 0, array2.Length); break;
                    case ReadWriteMode.AsyncArray: await decompressor.ReadAsync(array2, 0, array2.Length); break;
                    case ReadWriteMode.SyncSpan: decompressor.Read(new Span<byte>(array2)); break;
                    case ReadWriteMode.AsyncMemory: await decompressor.ReadAsync(new Memory<byte>(array2)); break;
                }
                for (j = 0; j < array2.Length; j++)
                {
                    Assert.Equal(data[j], array[j]);
                }

                Assert.Equal(
                    derived && (readWriteMode == ReadWriteMode.SyncArray || readWriteMode == ReadWriteMode.SyncSpan),
                    decompressor is DerivedDeflateStream dds && dds.ReadArrayInvoked);
            }
        }

        [Fact]
        public void Roundtrip_Write_ReadByte()
        {
            byte[] data = new byte[1024 * 10];
            new Random(42).NextBytes(data);

            var compressed = new MemoryStream();
            using (var compressor = new DeflateStream(compressed, CompressionMode.Compress, true))
            {
                compressor.Write(data, 0, data.Length);
            }
            compressed.Position = 0;

            using (var decompressor = new DeflateStream(compressed, CompressionMode.Decompress, true))
            {
                for (int i = 0; i < data.Length; i++)
                    Assert.Equal(data[i], decompressor.ReadByte());
            }
        }

        [Fact]
        [SkipOnTargetFramework(TargetFrameworkMonikers.NetFramework, "Full Framework Flush is a no-op.")]
        public async Task WrapNullReturningTasksStream()
        {
            using (var ds = new DeflateStream(new BadWrappedStream(BadWrappedStream.Mode.ReturnNullTasks), CompressionMode.Decompress))
                await Assert.ThrowsAsync<ArgumentNullException>(() => ds.ReadAsync(new byte[1024], 0, 1024));
        }

        [Fact]
        [SkipOnTargetFramework(TargetFrameworkMonikers.NetFramework, "Full Framework Flush is a no-op.")]
        public async Task WrapStreamReturningBadReadValues()
        {
            using (var ds = new DeflateStream(new BadWrappedStream(BadWrappedStream.Mode.ReturnTooLargeCounts), CompressionMode.Decompress))
                Assert.Throws<InvalidDataException>(() => ds.Read(new byte[1024], 0, 1024));
            using (var ds = new DeflateStream(new BadWrappedStream(BadWrappedStream.Mode.ReturnTooLargeCounts), CompressionMode.Decompress))
                await Assert.ThrowsAsync<InvalidDataException>(() => ds.ReadAsync(new byte[1024], 0, 1024));
            using (var ds = new DeflateStream(new BadWrappedStream(BadWrappedStream.Mode.ReturnTooLargeCounts), CompressionMode.Decompress))
                await Assert.ThrowsAsync<InvalidDataException>(async () => { await ds.ReadAsync(new Memory<byte>(new byte[1024])); });

            using (var ds = new DeflateStream(new BadWrappedStream(BadWrappedStream.Mode.ReturnTooSmallCounts), CompressionMode.Decompress))
                Assert.Equal(0, ds.Read(new byte[1024], 0, 1024));
            using (var ds = new DeflateStream(new BadWrappedStream(BadWrappedStream.Mode.ReturnTooSmallCounts), CompressionMode.Decompress))
                Assert.Equal(0, await ds.ReadAsync(new byte[1024], 0, 1024));
            using (var ds = new DeflateStream(new BadWrappedStream(BadWrappedStream.Mode.ReturnTooSmallCounts), CompressionMode.Decompress))
                Assert.Equal(0, await ds.ReadAsync(new Memory<byte>(new byte[1024])));
        }

        public static IEnumerable<object[]> CopyToAsync_Roundtrip_OutputMatchesInput_MemberData()
        {
            var rand = new Random();
            foreach (int dataSize in new[] { 1, 1024, 4095, 1024 * 1024 })
            {
                var data = new byte[dataSize];
                rand.NextBytes(data);

                var compressed = new MemoryStream();
                using (var ds = new DeflateStream(compressed, CompressionMode.Compress, leaveOpen: true))
                {
                    ds.Write(data, 0, data.Length);
                }
                byte[] compressedData = compressed.ToArray();

                foreach (int copyBufferSize in new[] { 1, 4096, 80 * 1024 })
                {
                    // Memory source
                    var m = new MemoryStream(compressedData, writable: false);
                    yield return new object[] { data, copyBufferSize, m };

                    // File sources, sync and async
                    foreach (bool useAsync in new[] { true, false })
                    {
                        string path = Path.GetTempFileName();
                        File.WriteAllBytes(path, compressedData);

                        FileOptions options = FileOptions.DeleteOnClose;
                        if (useAsync) options |= FileOptions.Asynchronous;
                        yield return new object[] { data, copyBufferSize, new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read, 0x1000, options) };
                    }
                }
            }
        }

        [Theory]
        [MemberData(nameof(CopyToAsync_Roundtrip_OutputMatchesInput_MemberData))]
        public async Task CopyToAsync_Roundtrip_OutputMatchesInput(byte[] expectedDecrypted, int copyBufferSize, Stream source)
        {
            var m = new MemoryStream();
            using (DeflateStream ds = new DeflateStream(source, CompressionMode.Decompress))
            {
                await ds.CopyToAsync(m);
            }
            Assert.Equal(expectedDecrypted, m.ToArray());
        }

        private sealed class DerivedDeflateStream : DeflateStream
        {
            public bool ReadArrayInvoked = false, WriteArrayInvoked = false;
            internal DerivedDeflateStream(Stream stream, CompressionMode mode) : base(stream, mode) { }
            internal DerivedDeflateStream(Stream stream, CompressionMode mode, bool leaveOpen) : base(stream, mode, leaveOpen) { }

            public override int Read(byte[] array, int offset, int count)
            {
                ReadArrayInvoked = true;
                return base.Read(array, offset, count);
            }

            public override void Write(byte[] array, int offset, int count)
            {
                WriteArrayInvoked = true;
                base.Write(array, offset, count);
            }
        }

        private sealed class BadWrappedStream : Stream
        {
            public enum Mode
            {
                Default,
                ReturnNullTasks,
                ReturnTooSmallCounts,
                ReturnTooLargeCounts,
            }

            private readonly Mode _mode;

            public BadWrappedStream(Mode mode) { _mode = mode; }

            public override int Read(byte[] buffer, int offset, int count)
            {
                switch (_mode)
                {
                    case Mode.ReturnTooSmallCounts: return -1;
                    case Mode.ReturnTooLargeCounts: return buffer.Length + 1;
                    default: return 0;
                }
            }

            public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
            {
                return _mode == Mode.ReturnNullTasks ?
                    null :
                    base.ReadAsync(buffer, offset, count, cancellationToken);
            }

            public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
            {
                return _mode == Mode.ReturnNullTasks ?
                   null :
                   base.WriteAsync(buffer, offset, count, cancellationToken);
            }

            public override void Write(byte[] buffer, int offset, int count) { }
            public override void Flush() { }
            public override bool CanRead { get { return true; } }
            public override bool CanSeek { get { return false; } }
            public override bool CanWrite { get { return true; } }
            public override long Length { get { throw new NotSupportedException(); } }
            public override long Position { get { throw new NotSupportedException(); } set { throw new NotSupportedException(); } }
            public override long Seek(long offset, SeekOrigin origin) { throw new NotSupportedException(); }
            public override void SetLength(long value) { throw new NotSupportedException(); }
        }
    }

    public partial class ManualSyncMemoryStream : MemoryStream
    {
        private bool isSync;
        public ManualResetEventSlim manualResetEvent = new ManualResetEventSlim(initialState: false);

        public bool ReadHit = false;  // For validation of the async methods we want to ensure they correctly delegate the async
        public bool WriteHit = false; // methods of the underlying stream. This bool acts as a toggle to check that they're being used.

        public static async Task<ManualSyncMemoryStream> GetStreamFromFileAsync(string testFile, bool sync = false, bool strip = false)
        {
            var baseStream = await StreamHelpers.CreateTempCopyStream(testFile);
            if (strip)
            {
                baseStream = StripHeaderAndFooter.Strip(baseStream);
            }

            var ms = new ManualSyncMemoryStream(sync);
            await baseStream.CopyToAsync(ms);

            ms.Position = 0;
            return ms;
        }

        public ManualSyncMemoryStream(bool sync = false) : base()
        {
            isSync = sync;
        }

        public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback callback, object state) => TaskToApm.Begin(ReadAsync(buffer, offset, count), callback, state);
        public override int EndRead(IAsyncResult asyncResult) => TaskToApm.End<int>(asyncResult);
        public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback callback, object state) => TaskToApm.Begin(WriteAsync(buffer, offset, count), callback, state);
        public override void EndWrite(IAsyncResult asyncResult) => TaskToApm.End(asyncResult);

        public override async Task<int> ReadAsync(byte[] array, int offset, int count, CancellationToken cancellationToken)
        {
            ReadHit = true;
            if (isSync)
            {
                manualResetEvent.Wait(cancellationToken);
            }
            else
            {
                await Task.Run(() => manualResetEvent.Wait(cancellationToken));
            }

            return await base.ReadAsync(array, offset, count, cancellationToken);
        }

        public override async Task WriteAsync(byte[] array, int offset, int count, CancellationToken cancellationToken)
        {
            WriteHit = true;
            if (isSync)
            {
                manualResetEvent.Wait(cancellationToken);
            }
            else
            {
                await Task.Run(() => manualResetEvent.Wait(cancellationToken));
            }

            await base.WriteAsync(array, offset, count, cancellationToken);
        }
    }
}
