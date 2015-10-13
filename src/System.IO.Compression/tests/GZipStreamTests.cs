// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace System.IO.Compression.Tests
{
    public class ZLibGZipStreamTests : GZipStreamTests, IDisposable
    {
        public ZLibGZipStreamTests() { Common.SetDeflaterMode("zlib"); }
        public void Dispose() { Common.SetDeflaterMode("unknown"); }
    }

    public class ManagedGZipStreamTests : GZipStreamTests, IDisposable
    {
        public ManagedGZipStreamTests() { Common.SetDeflaterMode("managed"); }
        public void Dispose() { Common.SetDeflaterMode("unknown"); }
    }

    public abstract class GZipStreamTests
    {
        static String gzTestFile(String fileName) { return Path.Combine("GZTestData", fileName); }

        [Fact]
        public void BaseStream1()
        {
            var writeStream = new MemoryStream();
            var zip = new GZipStream(writeStream, CompressionMode.Compress);

            Assert.Same(zip.BaseStream, writeStream);
            writeStream.Dispose();
        }

        [Fact]
        public void BaseStream2()
        {
            var ms = new MemoryStream();
            var zip = new GZipStream(ms, CompressionMode.Decompress);

            Assert.Same(zip.BaseStream, ms);
            ms.Dispose();
        }

        [Fact]
        public async Task ModifyBaseStream()
        {
            var ms = await LocalMemoryStream.readAppFileAsync(gzTestFile("GZTestDocument.txt.gz"));

            var zip = new GZipStream(ms, CompressionMode.Decompress);
            int size = 1024;
            Byte[] bytes = new Byte[size];
            zip.BaseStream.Read(bytes, 0, size); // This will throw if the underlying stream is not writeable as expected
        }

        [Fact]
        public void DecompressCanRead()
        {
            var ms = new MemoryStream();
            var zip = new GZipStream(ms, CompressionMode.Decompress);

            Assert.True(zip.CanRead, "GZipStream not CanRead in Decompress");

            zip.Dispose();
            Assert.False(zip.CanRead, "GZipStream CanRead after dispose in Decompress");
        }

        [Fact]
        public void CompressCanWrite()
        {
            var ms = new MemoryStream();
            var zip = new GZipStream(ms, CompressionMode.Compress);
            Assert.True(zip.CanWrite, "GZipStream not CanWrite with CompressionMode.Compress");

            zip.Dispose();
            Assert.False(zip.CanWrite, "GZipStream CanWrite after dispose");
        }

        [Fact]
        public void CanDisposeBaseStream()
        {
            var ms = new MemoryStream();
            var zip = new GZipStream(ms, CompressionMode.Compress);
            ms.Dispose(); // This would throw if this was invalid
        }

        [Fact]
        public void CanDisposeGZipStream()
        {
            var ms = new MemoryStream();
            var zip = new GZipStream(ms, CompressionMode.Compress);
            zip.Dispose();

            Assert.Null(zip.BaseStream);

            zip.Dispose(); // Should be a no-op
        }

        [Fact]
        public async Task CanReadBaseStreamAfterDispose()
        {
            var ms = await LocalMemoryStream.readAppFileAsync(gzTestFile("GZTestDocument.txt.gz"));

            var zip = new GZipStream(ms, CompressionMode.Decompress, true);
            var baseStream = zip.BaseStream;
            zip.Dispose();

            int size = 1024;
            Byte[] bytes = new Byte[size];
            baseStream.Read(bytes, 0, size); // This will throw if the underlying stream is not writeable as expected
        }

        [Fact]
        public async Task DecompressWorks()
        {
            var compareStream = await LocalMemoryStream.readAppFileAsync(gzTestFile("GZTestDocument.txt"));
            var gzStream = await LocalMemoryStream.readAppFileAsync(gzTestFile("GZTestDocument.txt.gz"));

            await DecompressAsync(compareStream, gzStream);
        }

        [Fact]
        public async Task DecompressWorksWithDoc()
        {
            var compareStream = await LocalMemoryStream.readAppFileAsync(gzTestFile("GZTestDocument.doc"));
            var gzStream = await LocalMemoryStream.readAppFileAsync(gzTestFile("GZTestDocument.doc.gz"));

            await DecompressAsync(compareStream, gzStream);
        }

        [Fact]
        public async Task DecompressWorksWithDocx()
        {
            var compareStream = await LocalMemoryStream.readAppFileAsync(gzTestFile("GZTestDocument.docx"));
            var gzStream = await LocalMemoryStream.readAppFileAsync(gzTestFile("GZTestDocument.docx.gz"));

            await DecompressAsync(compareStream, gzStream);
        }

        [Fact]
        public async Task DecompressWorksWithPdf()
        {
            var compareStream = await LocalMemoryStream.readAppFileAsync(gzTestFile("GZTestDocument.pdf"));
            var gzStream = await LocalMemoryStream.readAppFileAsync(gzTestFile("GZTestDocument.pdf.gz"));

            await DecompressAsync(compareStream, gzStream);
        }

        [Fact]
        public async Task DecompressWorksWithReset()
        {            
            var gzStreamFirst = await LocalMemoryStream.readAppFileAsync(gzTestFile("GZTestDocument.pdf.gz"));
            using (var stream = new GZipStream(gzStreamFirst, CompressionMode.Decompress))
            {
                var compareStreamFirst = await LocalMemoryStream.readAppFileAsync(gzTestFile("GZTestDocument.pdf"));
                await DecompressAsync(compareStreamFirst, stream);

                var gzStreamSecond = await LocalMemoryStream.readAppFileAsync(gzTestFile("GZTestDocument.docx.gz"));
                stream.Reset(gzStreamSecond);

                var compareStreamSecond = await LocalMemoryStream.readAppFileAsync(gzTestFile("GZTestDocument.docx"));
                await DecompressAsync(compareStreamSecond, stream);
            }               
        }

        // Making this async since regular read/write are tested below
        private static async Task DecompressAsync(MemoryStream compareStream, GZipStream zip)
        {         
            var gzipOutputStream = new MemoryStream();

            int _bufferSize = 1024;
            var bytes = new Byte[_bufferSize];
            bool finished = false;
            int retCount;
            while (!finished)
            {
                retCount = await zip.ReadAsync(bytes, 0, _bufferSize);

                if (retCount != 0)
                    await gzipOutputStream.WriteAsync(bytes, 0, retCount);
                else
                    finished = true;
            }

            gzipOutputStream.Position = 0;
            compareStream.Position = 0;

            byte[] compareArray = compareStream.ToArray();
            byte[] writtenArray = gzipOutputStream.ToArray();

            Assert.Equal(compareArray.Length, writtenArray.Length);
            for (int i = 0; i < compareArray.Length; i++)
            {
                Assert.Equal(compareArray[i], writtenArray[i]);
            }
        }

        // Making this async since regular read/write are tested below
        private static async Task DecompressAsync(MemoryStream compareStream, MemoryStream gzStream)
        {
            var zip = new GZipStream(gzStream, CompressionMode.Decompress);
            await DecompressAsync(compareStream, zip);
        }

        [Fact]
        public void NullBaseStreamThrows()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                var deflate = new GZipStream(null, CompressionMode.Decompress);
            });

            Assert.Throws<ArgumentNullException>(() =>
            {
                var deflate = new GZipStream(null, CompressionMode.Compress);
            });
        }

        [Fact]
        public void DisposedBaseStreamThrows()
        {
            var ms = new MemoryStream();
            ms.Dispose();
            Assert.Throws<ArgumentException>(() =>
            {
                var deflate = new GZipStream(ms, CompressionMode.Decompress);
            });

            Assert.Throws<ArgumentException>(() =>
            {
                var deflate = new GZipStream(ms, CompressionMode.Compress);
            });
        }

        [Fact]
        public void ReadOnlyStreamThrowsOnCompress()
        {
            var ms = new LocalMemoryStream();
            ms.SetCanWrite(false);

            Assert.Throws<ArgumentException>(() =>
            {
                var gzip = new GZipStream(ms, CompressionMode.Compress);
            });
        }

        [Fact]
        public void WriteOnlyStreamThrowsOnDecompress()
        {
            var ms = new LocalMemoryStream();
            ms.SetCanRead(false);

            Assert.Throws<ArgumentException>(() =>
            {
                var gzip = new GZipStream(ms, CompressionMode.Decompress);
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
            //Create the GZipStream
            int _bufferSize = 1024;
            var bytes = new Byte[_bufferSize];
            var baseStream = new MemoryStream(bytes, true);
            GZipStream ds;

            if (leaveOpen == null)
            {
                ds = new GZipStream(baseStream, level);
            }
            else
            {
                ds = new GZipStream(baseStream, level, leaveOpen ?? false);
            }

            //Write some data and Close the stream
            String strData = "Test Data";
            var encoding = Encoding.UTF8;
            Byte[] data = encoding.GetBytes(strData);
            ds.Write(data, 0, data.Length);
            ds.Flush();
            ds.Dispose();

            if (leaveOpen != true)
            {
                //Check that Close has really closed the underlying stream
                Assert.Throws<ObjectDisposedException>(() => { baseStream.Write(bytes, 0, bytes.Length); });
            }

            //Read the data
            Byte[] data2 = new Byte[_bufferSize];
            baseStream = new MemoryStream(bytes, false);
            ds = new GZipStream(baseStream, CompressionMode.Decompress);
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
        public async Task Flush()
        {
            var ms = new MemoryStream();
            var ds = new GZipStream(ms, CompressionMode.Compress);
            ds.Flush();
            await ds.FlushAsync();

            // Just ensuring Flush doesn't throw
        }

        [Fact]
        public void FlushFailsAfterDispose()
        {
            var ms = new MemoryStream();
            var ds = new GZipStream(ms, CompressionMode.Compress);
            ds.Dispose();
            Assert.Throws<ObjectDisposedException>(() => { ds.Flush(); });
        }

        [Fact]
        public async Task FlushAsyncFailsAfterDispose()
        {
            var ms = new MemoryStream();
            var ds = new GZipStream(ms, CompressionMode.Compress);
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
            var zip = new GZipStream(ms, CompressionMode.Decompress);

            Assert.False(zip.CanSeek);

            Assert.Throws<NotSupportedException>(delegate { long value = zip.Length; });
            Assert.Throws<NotSupportedException>(delegate { long value = zip.Position; });
            Assert.Throws<NotSupportedException>(delegate { zip.Position = 100L; });
            Assert.Throws<NotSupportedException>(delegate { zip.SetLength(100L); });
            Assert.Throws<NotSupportedException>(delegate { zip.Seek(100L, SeekOrigin.Begin); });

            zip.Dispose();
            Assert.False(zip.CanSeek);
        }

        [Fact]
        public void TestSeekMethodsCompress()
        {
            var ms = new MemoryStream();
            var zip = new GZipStream(ms, CompressionMode.Compress);

            Assert.False(zip.CanSeek);

            Assert.Throws<NotSupportedException>(delegate { long value = zip.Length; });
            Assert.Throws<NotSupportedException>(delegate { long value = zip.Position; });
            Assert.Throws<NotSupportedException>(delegate { zip.Position = 100L; });
            Assert.Throws<NotSupportedException>(delegate { zip.SetLength(100L); });
            Assert.Throws<NotSupportedException>(delegate { zip.Seek(100L, SeekOrigin.Begin); });

            zip.Dispose();
            Assert.False(zip.CanSeek);
        }

        public static IEnumerable<object[]> Configurations
        {
            get
            {
                foreach (bool useAsync in new[] { true, false }) // whether to use Read/Write or ReadAsync/WriteAsync
                {
                    foreach (var level in new[] { CompressionLevel.Fastest, CompressionLevel.Optimal, CompressionLevel.NoCompression }) // compression level
                    {
                        yield return new object[] { useAsync, 1, 5, level }; // smallest possible writes
                        yield return new object[] { useAsync, 1023, 1023 * 10, level }; // overflowing internal buffer
                        yield return new object[] { useAsync, 1024 * 1024, 1024 * 1024, level }; // large single write
                    }
                }
            }
        }

        [Theory]
        [MemberData("Configurations")]
        public async Task SameCompressOutputWithReset(bool useAsync, int chunkSize, int totalSize, CompressionLevel level)
        {
            var rnd = new Random(42);
            byte[] data = new byte[totalSize];
            rnd.NextBytes(data);

            var expected = new MemoryStream();            
            var compressor = new GZipStream(expected, level, true);

            for (int i = 0; i < data.Length; i += chunkSize)
            {
                switch (useAsync)
                {
                    case true: await compressor.WriteAsync(data, i, chunkSize); break;
                    case false: compressor.Write(data, i, chunkSize); break;
                }
            }
            compressor.Close();

            var duplicated = new MemoryStream();
            compressor.Reset(duplicated);

            for (int i = 0; i < data.Length; i += chunkSize)
            {
                switch (useAsync)
                {
                    case true: await compressor.WriteAsync(data, i, chunkSize); break;
                    case false: compressor.Write(data, i, chunkSize); break;
                }
            }

            compressor.Close();

            Assert.Equal<byte>(expected.ToArray(), duplicated.ToArray());
        }

        //[Theory]
        //[MemberData("Configurations")]
        //public async Task RoundtripCompressDecompressWithReset(bool useAsync, int chunkSize, int totalSize, CompressionLevel level)
        //{
        //    var rnd = new Random(42);

        //    byte[][] data = new byte[5][]; // We have enough with 5 iterations to tryout Reset.            
        //    for (int iteration = 0; iteration < data.Length; iteration++)
        //    {
        //        data[iteration] = new byte[totalSize];
        //        rnd.NextBytes(data[iteration]);
        //    }

        //    var compressed = new MemoryStream();
        //    var compressor = new GZipStream(compressed, level, true);
        //    var decompressor = new GZipStream(compressed, CompressionMode.Decompress, true);

        //    for ( int iteration = 0; iteration < data.Length; iteration++ )
        //    {
        //        compressed = new MemoryStream();               
        //        compressor.Reset(compressed);
        //        decompressor.Reset(compressed);

        //        Assert.True(compressor.IsOpen);
        //        Assert.True(decompressor.IsOpen);

        //        for (int i = 0; i < data[iteration].Length; i += chunkSize)
        //        {
        //            switch (useAsync)
        //            {
        //                case true: await compressor.WriteAsync(data[iteration], i, chunkSize); break;
        //                case false: compressor.Write(data[iteration], i, chunkSize); break;
        //            }
        //        }

        //        compressor.Close();
        //        Assert.False(compressor.IsOpen);

        //        compressed.Position = 0;

        //        var decompressed = new MemoryStream();
        //        switch (useAsync)
        //        {
        //            case true: await decompressor.CopyToAsync(decompressed, chunkSize, CancellationToken.None); break;
        //            case false: decompressor.CopyTo(decompressed, chunkSize); break;
        //        }

        //        decompressor.Close();
        //        Assert.False(decompressor.IsOpen);

        //        Assert.Equal<byte>(data[iteration], decompressed.ToArray());
        //    }

        //    compressor.Dispose();
        //    decompressor.Dispose();
        //}    
    }
}
