// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Immutable;
using System.IO;
using Xunit;

namespace System.Reflection.Internal.Tests
{
    public class AbstractMemoryBlockTests
    {
        [Fact]
        public unsafe void ByteArray()
        {
            var array = ImmutableArray.Create(new byte[] { 1, 2, 3 });
            using (var provider = new ByteArrayMemoryProvider(array))
            {
                using (var block = provider.GetMemoryBlock())
                {
                    Assert.Equal(3, block.Size);
                    AssertEx.Equal(provider.Pointer, block.Pointer);
                    Assert.Equal(array, block.GetContent());
                }

                using (var block = provider.GetMemoryBlock(1, 2))
                {
                    AssertEx.Equal(provider.Pointer + 1, block.Pointer);
                    Assert.Equal(2, block.Size);
                    Assert.Equal(new byte[] { 2, 3 }, block.GetContent());
                }
            }
        }

        [Fact]
        public unsafe void External()
        {
            var array = new byte[] { 1, 2, 3 };
            fixed (byte* arrayPtr = array)
            {
                using (var provider = new ExternalMemoryBlockProvider(arrayPtr, array.Length))
                {
                    using (var block = provider.GetMemoryBlock())
                    {
                        Assert.Equal(3, block.Size);
                        AssertEx.Equal(provider.Pointer, block.Pointer);
                        Assert.Equal(array, block.GetContent());
                    }

                    using (var block = provider.GetMemoryBlock(1, 2))
                    {
                        AssertEx.Equal(provider.Pointer + 1, block.Pointer);
                        Assert.Equal(2, block.Size);
                        Assert.Equal(new byte[] { 2, 3 }, block.GetContent());
                    }
                }
            }
        }

        [Fact]
        public void Stream()
        {
            var array = new byte[] { 1, 2, 3 };
            using (var stream = new MemoryStream(array))
            {
                Assert.False(FileStreamReadLightUp.IsFileStream(stream));

                using (var provider = new StreamMemoryBlockProvider(stream, 0, array.Length, isFileStream: false, leaveOpen: true))
                {
                    using (var block = provider.GetMemoryBlock())
                    {
                        Assert.IsType<NativeHeapMemoryBlock>(block);
                        Assert.Equal(3, block.Size);
                        Assert.Equal(array, block.GetContent());
                    }

                    Assert.Equal(3, stream.Position);

                    using (var block = provider.GetMemoryBlock(1, 2))
                    {
                        Assert.IsType<NativeHeapMemoryBlock>(block);
                        Assert.Equal(2, block.Size);
                        Assert.Equal(new byte[] { 2, 3 }, block.GetContent());
                    }

                    Assert.Equal(3, stream.Position);
                }

                using (var provider = new StreamMemoryBlockProvider(stream, 0, array.Length, isFileStream: false, leaveOpen: false))
                {
                    using (var block = provider.GetMemoryBlock())
                    {
                        Assert.IsType<NativeHeapMemoryBlock>(block);
                        Assert.Equal(3, block.Size);
                        Assert.Equal(array, block.GetContent());
                    }

                    Assert.Equal(3, stream.Position);
                }

                Assert.Throws<ObjectDisposedException>(() => stream.Position);
            }
        }

        [Fact]
        public void FileStreamWin7()
        {
            try
            {
                FileStreamReadLightUp.readFileModernNotAvailable = true;
                FileStream();
            }
            finally
            {
                FileStreamReadLightUp.readFileModernNotAvailable = false;
            }
        }

        [Fact]
        public void FileStreamUnix()
        {
            try
            {
                FileStreamReadLightUp.readFileModernNotAvailable = true;
                FileStreamReadLightUp.readFileCompatNotAvailable = true;
                FileStream();
            }
            finally
            {
                FileStreamReadLightUp.readFileModernNotAvailable = false;
                FileStreamReadLightUp.readFileCompatNotAvailable = false;
            }
        }

        [Fact]
        public void FileStream()
        {
            string filePath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            try
            {
                var array = new byte[StreamMemoryBlockProvider.MemoryMapThreshold + 1];
                for (int i = 0; i < array.Length; i++)
                {
                    array[i] = 0x12;
                }

                File.WriteAllBytes(filePath, array);

                foreach (bool useAsync in new[] { true, false })
                {
                    using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, useAsync))
                    {
                        Assert.True(FileStreamReadLightUp.IsFileStream(stream));

                        using (var provider = new StreamMemoryBlockProvider(stream, imageStart: 0, imageSize: array.Length, isFileStream: true, leaveOpen: false))
                        {
                            // large:
                            using (var block = provider.GetMemoryBlock())
                            {
                                Assert.IsType<MemoryMappedFileBlock>(block);
                                Assert.Equal(array.Length, block.Size);
                                Assert.Equal(array, block.GetContent());
                            }

                            // we didn't use the stream for reading
                            Assert.Equal(0, stream.Position);

                            // small:
                            using (var block = provider.GetMemoryBlock(1, 2))
                            {
                                Assert.IsType<NativeHeapMemoryBlock>(block);
                                Assert.Equal(2, block.Size);
                                Assert.Equal(new byte[] { 0x12, 0x12 }, block.GetContent());
                            }

                            Assert.Equal(3, stream.Position);
                        }

                        Assert.Throws<ObjectDisposedException>(() => stream.Position);
                    }
                }
            }
            finally
            {
                File.Delete(filePath);
            }
        }
    }
}
