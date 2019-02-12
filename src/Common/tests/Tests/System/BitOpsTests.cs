// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Xunit;

namespace Internal.Runtime.CompilerServices
{
    // Dummy namespace needed for compilation of BitOps
}

namespace Tests.System
{
    public static class BitOpsTests
    {
        [Theory]
        [InlineData(0u, 32)]
        [InlineData(0b1u, 0)]
        [InlineData(0b10u, 1)]
        [InlineData(0b100u, 2)]
        [InlineData(0b1000u, 3)]
        [InlineData(0b10000u, 4)]
        [InlineData(0b100000u, 5)]
        [InlineData(0b1000000u, 6)]
        [InlineData((uint)byte.MaxValue << 24, 24)]
        [InlineData((uint)byte.MaxValue << 22, 22)]
        [InlineData((uint)ushort.MaxValue << 16, 16)]
        [InlineData((uint)ushort.MaxValue << 19, 19)]
        [InlineData(uint.MaxValue << 5, 5)]
        [InlineData(3u << 27, 27)]
        [InlineData(uint.MaxValue, 0)]
        public static void BitOps_TrailingZeroCount_uint(uint n, int expected)
        {
            int actual = BitOps.TrailingZeroCount(n);
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(0, 32)]
        [InlineData(0b1, 0)]
        [InlineData(0b10, 1)]
        [InlineData(0b100, 2)]
        [InlineData(0b1000, 3)]
        [InlineData(0b10000, 4)]
        [InlineData(0b100000, 5)]
        [InlineData(0b1000000, 6)]
        [InlineData(byte.MaxValue << 24, 24)]
        [InlineData(byte.MaxValue << 22, 22)]
        [InlineData(ushort.MaxValue << 16, 16)]
        [InlineData(ushort.MaxValue << 19, 19)]
        [InlineData(int.MaxValue << 5, 5)]
        [InlineData(3 << 27, 27)]
        [InlineData(int.MaxValue, 0)]
        public static void BitOps_TrailingZeroCount_int(int n, int expected)
        {
            int actual = BitOps.TrailingZeroCount(n);
            Assert.Equal(expected, actual);
        }

        // Ignore: Work In Progress
        /*
        [Theory]
        [InlineData(0ul, 64)]
        [InlineData(0b1ul, 0)]
        [InlineData(0b10ul, 1)]
        [InlineData(0b100ul, 2)]
        [InlineData(0b1000ul, 3)]
        [InlineData(0b10000ul, 4)]
        [InlineData(0b100000ul, 5)]
        [InlineData(0b1000000ul, 6)]
        [InlineData((ulong)byte.MaxValue << 40, 40)]
        [InlineData((ulong)byte.MaxValue << 57, 57)]
        [InlineData((ulong)ushort.MaxValue << 31, 31)]
        [InlineData((ulong)ushort.MaxValue << 15, 15)]
        [InlineData(ulong.MaxValue << 5, 5)]
        [InlineData(3ul << 59, 59)]
        [InlineData(5ul << 63, 63)]
        [InlineData(ulong.MaxValue, 0)]
        public static void BitOps_TrailingZeroCount_ulong(ulong n, int expected)
        {
            int actual = BitOps.TrailingZeroCount(n);
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(0L, 64)]
        [InlineData(0b1L, 0)]
        [InlineData(0b10L, 1)]
        [InlineData(0b100L, 2)]
        [InlineData(0b1000L, 3)]
        [InlineData(0b10000L, 4)]
        [InlineData(0b100000L, 5)]
        [InlineData(0b1000000L, 6)]
        [InlineData((long)byte.MaxValue << 40, 40)]
        [InlineData((long)byte.MaxValue << 57, 57)]
        [InlineData((long)ushort.MaxValue << 31, 31)]
        [InlineData((long)ushort.MaxValue << 15, 15)]
        [InlineData(long.MaxValue << 5, 5)]
        [InlineData(3L << 59, 59)]
        [InlineData(5L << 63, 63)]
        [InlineData(long.MaxValue, 0)]
        public static void BitOps_TrailingZeroCount_long(long n, int expected)
        {
            int actual = BitOps.TrailingZeroCount(n);
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(0u, 32)]
        [InlineData(0b1u, 31)]
        [InlineData(0b10u, 30)]
        [InlineData(0b100u, 29)]
        [InlineData(0b1000u, 28)]
        [InlineData(0b10000u, 27)]
        [InlineData(0b100000u, 26)]
        [InlineData(0b1000000u, 25)]
        [InlineData(byte.MaxValue << 17, 32 - 8 - 17)]
        [InlineData(byte.MaxValue << 9, 32 - 8 - 9)]
        [InlineData(ushort.MaxValue << 11, 32 - 16 - 11)]
        [InlineData(ushort.MaxValue << 2, 32 - 16 - 2)]
        [InlineData(5 << 7, 32 - 3 - 7)]
        [InlineData(uint.MaxValue, 0)]
        public static void BitOps_LeadingZeroCount_uint(uint n, int expected)
        {
            int actual = BitOps.LeadingZeroCount(n);
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(0, 32)]
        [InlineData(0b1, 31)]
        [InlineData(0b10, 30)]
        [InlineData(0b100, 29)]
        [InlineData(0b1000, 28)]
        [InlineData(0b10000, 27)]
        [InlineData(0b100000, 26)]
        [InlineData(0b1000000, 25)]
        [InlineData(byte.MaxValue << 17, 32 - 8 - 17)]
        [InlineData(byte.MaxValue << 9, 32 - 8 - 9)]
        [InlineData(ushort.MaxValue << 11, 32 - 16 - 11)]
        [InlineData(ushort.MaxValue << 2, 32 - 16 - 2)]
        [InlineData(5 << 7, 32 - 3 - 7)]
        [InlineData(int.MinValue, 0)]
        [InlineData(int.MaxValue, 1)]
        public static void BitOps_LeadingZeroCount_int(int n, int expected)
        {
            int actual = BitOps.LeadingZeroCount(n);
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(0ul, 64)]
        [InlineData(0b1ul, 63)]
        [InlineData(0b10ul, 62)]
        [InlineData(0b100ul, 61)]
        [InlineData(0b1000ul, 60)]
        [InlineData(0b10000ul, 59)]
        [InlineData(0b100000ul, 58)]
        [InlineData(0b1000000ul, 57)]
        [InlineData((ulong)byte.MaxValue << 41, 64 - 8 - 41)]
        [InlineData((ulong)byte.MaxValue << 53, 64 - 8 - 53)]
        [InlineData((ulong)ushort.MaxValue << 31, 64 - 16 - 31)]
        [InlineData((ulong)ushort.MaxValue << 15, 64 - 16 - 15)]
        [InlineData(ulong.MaxValue >> 5, 5)]
        [InlineData(1ul << 63, 0)]
        [InlineData(1ul << 62, 1)]
        [InlineData(ulong.MaxValue, 0)]
        public static void BitOps_LeadingZeroCount_ulong(ulong n, int expected)
        {
            int actual = BitOps.LeadingZeroCount(n);
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(0L, 64)]
        [InlineData(0b1L, 63)]
        [InlineData(0b10L, 62)]
        [InlineData(0b100L, 61)]
        [InlineData(0b1000L, 60)]
        [InlineData(0b10000L, 59)]
        [InlineData(0b100000L, 58)]
        [InlineData(0b1000000L, 57)]
        [InlineData((long)byte.MaxValue << 41, 64 - 8 - 41)]
        [InlineData((long)byte.MaxValue << 53, 64 - 8 - 53)]
        [InlineData((long)ushort.MaxValue << 31, 64 - 16 - 31)]
        [InlineData((long)ushort.MaxValue << 15, 64 - 16 - 15)]
        [InlineData(1L << 62, 1)]
        [InlineData(long.MinValue, 0)]
        [InlineData(long.MaxValue, 1)]
        public static void BitOps_LeadingZeroCount_long(long n, int expected)
        {
            int actual = BitOps.LeadingZeroCount(n);
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(0, 0)]
        [InlineData(1, 0)]
        [InlineData(2, 1)]
        [InlineData(3, 2 - 1)]
        [InlineData(4, 2)]
        [InlineData(5, 3 - 1)]
        [InlineData(6, 3 - 1)]
        [InlineData(7, 3 - 1)]
        [InlineData(8, 3)]
        [InlineData(9, 4 - 1)]
        [InlineData(byte.MaxValue, 8 - 1)]
        [InlineData(ushort.MaxValue, 16 - 1)]
        [InlineData(uint.MaxValue, 32 - 1)]
        public static void BitOps_Log2_uint(uint n, int expected)
        {
            int actual = BitOps.Log2(n);
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(0, 0)]
        [InlineData(1, 0)]
        [InlineData(2, 1)]
        [InlineData(3, 2 - 1)]
        [InlineData(4, 2)]
        [InlineData(5, 3 - 1)]
        [InlineData(6, 3 - 1)]
        [InlineData(7, 3 - 1)]
        [InlineData(8, 3)]
        [InlineData(9, 4 - 1)]
        [InlineData(byte.MaxValue, 8 - 1)]
        [InlineData(ushort.MaxValue, 16 - 1)]
        [InlineData(uint.MaxValue, 32 - 1)]
        [InlineData(ulong.MaxValue, 64 - 1)]
        public static void BitOps_Log2_ulong(ulong n, int expected)
        {
            int actual = BitOps.Log2(n);
            Assert.Equal(expected, actual);
        }
        */
    }
}
