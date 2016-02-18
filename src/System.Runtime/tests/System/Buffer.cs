// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;

using Xunit;

namespace System.Runtime.Tests
{
    public static class BufferTests
    {
        public static IEnumerable<object[]> BlockCopyTestData()
        {
            yield return new object[] { new byte[] { 0x1a, 0x2b, 0x3c, 0x4d }, 0, new byte[] { 0x6f, 0x6f, 0x6f, 0x6f, 0x6f, 0x6f }, 1, 4, new byte[] { 0x6f, 0x1a, 0x2b, 0x3c, 0x4d, 0x6f } };

            var dst = new byte[] { 0x1a, 0x2b, 0x3c, 0x4d, 0x5e };
            yield return new object[] { dst, 1, dst, 2, 2, new byte[] { 0x1a, 0x2b, 0x2b, 0x3c, 0x5e } };
        }

        [Theory, MemberData("BlockCopyTestData")]
        public static void TestBlockCopy(Array src, int srcOffset, Array dst, int dstOffset, int count, Array expected)
        {
            Buffer.BlockCopy(src, srcOffset, dst, dstOffset, count);
            Assert.Equal(expected, dst);
        }

        [Fact]
        public static void TestBlockCopy_Invalid()
        {
            Assert.Throws<ArgumentNullException>("src", () => Buffer.BlockCopy(null, 0, new int[3], 0, 0)); // Src is null
            Assert.Throws<ArgumentNullException>("dst", () => Buffer.BlockCopy(new string[3], 0, null, 0, 0)); // Dst is null
            
            Assert.Throws<ArgumentOutOfRangeException>("srcOffset", () => Buffer.BlockCopy(new byte[3], -1, new byte[3], 0, 0)); // SrcOffset < 0
            Assert.Throws<ArgumentOutOfRangeException>("dstOffset", () => Buffer.BlockCopy(new byte[3], 0, new byte[3], -1, 0)); // DstOffset < 0
            Assert.Throws<ArgumentOutOfRangeException>("count", () => Buffer.BlockCopy(new byte[3], 0, new byte[3], 0, -1)); // Count < 0

            Assert.Throws<ArgumentException>("src", () => Buffer.BlockCopy(new string[3], 0, new byte[3], 0, 0)); // Src is not a byte array
            Assert.Throws<ArgumentException>("dest", () => Buffer.BlockCopy(new byte[3], 0, new string[3], 0, 0)); // Dst is not a byte array

            Assert.Throws<ArgumentException>("", () => Buffer.BlockCopy(new byte[3], 3, new byte[3], 0, 1)); // SrcOffset + count >= src.length
            Assert.Throws<ArgumentException>("", () => Buffer.BlockCopy(new byte[3], 4, new byte[3], 0, 0)); // SrcOffset >= src.Length

            Assert.Throws<ArgumentException>("", () => Buffer.BlockCopy(new byte[3], 0, new byte[3], 3, 1)); // DstOffset + count >= dst.Length
            Assert.Throws<ArgumentException>("", () => Buffer.BlockCopy(new byte[3], 0, new byte[3], 4, 0)); // DstOffset >= dst.Length
        }

        public static unsafe IEnumerable<object[]> ByteLengthTestData()
        {
            return new object[][]
            {
                new object[] { typeof(byte), sizeof(byte) },
                new object[] { typeof(sbyte), sizeof(sbyte) },
                new object[] { typeof(short), sizeof(short) },
                new object[] { typeof(ushort), sizeof(ushort) },
                new object[] { typeof(int), sizeof(int) },
                new object[] { typeof(uint), sizeof(uint) },
                new object[] { typeof(long), sizeof(long) },
                new object[] { typeof(ulong), sizeof(ulong) },
                new object[] { typeof(IntPtr), sizeof(IntPtr) },
                new object[] { typeof(UIntPtr), sizeof(UIntPtr) },
                new object[] { typeof(double), sizeof(double) },
                new object[] { typeof(float), sizeof(float) },
                new object[] { typeof(bool), sizeof(bool) },
                new object[] { typeof(char), sizeof(char) }
            };
        }

        [Theory, MemberData("ByteLengthTestData")]
        public static void TestByteLength(Type type, int size)
        {
            const int Length = 25;
            Array array = Array.CreateInstance(type, Length);
            Assert.Equal(Length * size, Buffer.ByteLength(array));
        }

        [Fact]
        public static void TestByteLength_Invalid()
        {
            Assert.Throws<ArgumentNullException>("array", () => Buffer.ByteLength(null)); // Array is null

            Assert.Throws<ArgumentException>("array", () => Buffer.ByteLength(Array.CreateInstance(typeof(DateTime), 25))); // Array is not a primitive
            Assert.Throws<ArgumentException>("array", () => Buffer.ByteLength(Array.CreateInstance(typeof(decimal), 25))); // Array is not a primitive
            Assert.Throws<ArgumentException>("array", () => Buffer.ByteLength(Array.CreateInstance(typeof(string), 25))); // Array is not a primitive
        }

        [Theory]
        [InlineData(new uint[] { 0x01234567, 0x89abcdef }, 0, 0x67)]
        [InlineData(new uint[] { 0x01234567, 0x89abcdef }, 7, 0x89)]
        public static void TestGetByte(Array array, int index, int expected)
        {
            Assert.Equal(expected, Buffer.GetByte(array, index));
        }

        [Fact]
        public static void TestGetByte_Invalid()
        {
            var array = new uint[] { 0x01234567, 0x89abcdef };

            Assert.Throws<ArgumentNullException>("array", () => Buffer.GetByte(null, 0)); // Array is null
            Assert.Throws<ArgumentException>("array", () => Buffer.GetByte(new object[10], 0)); // Array is not a primitive array

            Assert.Throws<ArgumentOutOfRangeException>("index", () => Buffer.GetByte(array, -1)); // Index < 0
            Assert.Throws<ArgumentOutOfRangeException>("index", () => Buffer.GetByte(array, 8)); // Index >= array.Length
        }

        [Theory]
        [InlineData(0, 0)]
        [InlineData(0, 5000)]
        [InlineData(5000, 6000)]
        public static unsafe void TestMemoryCopy_Long(int sourceIndexOffset, int destinationIndexOffset)
        {
            var sourceArray = new int[25000];
            for (int i = 0; i < sourceArray.Length; i++)
            {
                sourceArray[i] = i;
            }

            var destinationArray = new int[30000];
            fixed (int* sourceBase = sourceArray, destinationBase = destinationArray)
            {
                Buffer.MemoryCopy(sourceBase + sourceIndexOffset, destinationBase + destinationIndexOffset, 30000 * 4, (25000 - sourceIndexOffset) * 4);
            }

            for (int i = sourceIndexOffset; i < sourceArray.Length; i++)
            {
                Assert.Equal(sourceArray[i], destinationArray[i + destinationIndexOffset - sourceIndexOffset]);
            }
        }

        [Fact]
        public static unsafe void TestMemoryCopy_OverlappingBuffers()
        {
            var array = new int[200];
            for (int g = 0; g < array.Length; g++)
            {
                array[g] = g;
            }

            fixed (int* arrayBase = array)
            {
                Buffer.MemoryCopy(arrayBase, arrayBase + 50, 200 * 4, 100 * 4);
            }

            for (int g = 0; g < 100; g++)
            {
                Assert.Equal(g, array[g + 50]);
            }
        }

        [Fact]
        public static unsafe void TestMemoryCopy_OverlappingBuffersSmallCopy()
        {
            var array = new int[200];
            for (int g = 0; g < array.Length; g++)
            {
                array[g] = g;
            }

            fixed (int* arrayBase = array)
            {
                Buffer.MemoryCopy(arrayBase, arrayBase + 5, 200 * 4, 15 * 4);
            }

            for (int g = 0; g < 15; g++)
            {
                Assert.Equal(g, array[g + 5]);
            }
        }

        [Fact]
        public static unsafe void TestMemoryCopy_Invalid()
        {
            var sourceArray = new int[5000];
            var destinationArray = new int[1000];

            Assert.Throws<ArgumentOutOfRangeException>("sourceBytesToCopy", () =>
            {
                fixed (int* sourceBase = sourceArray, destinationBase = destinationArray)
                {
                    Buffer.MemoryCopy(sourceBase, destinationBase, 5000 * 4, 20000 * 4); // Source bytes to copy > destination size in bytes
                }
            });

            Assert.Throws<ArgumentOutOfRangeException>("sourceBytesToCopy", () =>
            {
                fixed (int* sourceBase = sourceArray, destinationBase = destinationArray)
                {
                    Buffer.MemoryCopy(sourceBase, destinationBase, (ulong)5000 * 4, (ulong)20000 * 4); // Source bytes to copy > destination size in bytes
                }
            });
        }

        [Theory]
        [InlineData(new uint[] { 0x01234567, 0x89abcdef }, 0, 0x42, new uint[] { 0x01234542, 0x89abcdef })]
        [InlineData(new uint[] { 0x01234542, 0x89abcdef }, 7, 0xa2, new uint[] { 0x01234542, 0xa2abcdef })]
        public static void TestSetByte(Array array, int index, byte value, Array expected)
        {
            Buffer.SetByte(array, index, value);
            Assert.Equal(expected, array);
        }

        [Fact]
        public static void TestSetByte_Invalid()
        {
            var array = new uint[] { 0x01234567, 0x89abcdef };

            Assert.Throws<ArgumentNullException>("array", () => Buffer.SetByte(null, 0, 0xff)); // Array is null
            Assert.Throws<ArgumentException>("array", () => Buffer.SetByte(new object[10], 0, 0xff)); // Array is not a primitive array

            Assert.Throws<ArgumentOutOfRangeException>("index", () => Buffer.SetByte(array, -1, 0xff)); // Index < 0
            Assert.Throws<ArgumentOutOfRangeException>("index", () => Buffer.SetByte(array, 8, 0xff)); // Index > array.Length
        }
    }
}
