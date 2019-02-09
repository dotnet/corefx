// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Test.Cryptography;

using Xunit;

namespace Tests.System
{
    public static class BitOpsTests
    {
        #region TrailingZeroCount

        [Theory]
        [InlineData(0u, 32)]
        [InlineData(0b1u, 0)]
        [InlineData(0b10u, 1)]
        [InlineData(0b100u, 2)]
        [InlineData(0b1000u, 3)]
        [InlineData(0b10000u, 4)]
        [InlineData(0b100000u, 5)]
        [InlineData(0b1000000u, 6)]
        [InlineData((uint)byte.MaxValue << 24, 32 - 8)]
        [InlineData((uint)byte.MaxValue << 24 - 2, 32 - 8 - 2)]
        [InlineData((uint)ushort.MaxValue << 16, 32 - 16)]
        [InlineData((uint)ushort.MaxValue << 16 + 3, 32 - 16 + 3)]
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
        [InlineData(byte.MaxValue << 24, 32 - 8)]
        [InlineData(byte.MaxValue << 24 - 2, 32 - 8 - 2)]
        [InlineData(ushort.MaxValue << 16, 32 - 16)]
        [InlineData(ushort.MaxValue << 16 + 3, 32 - 16 + 3)]
        [InlineData(int.MaxValue << 5, 5)]
        [InlineData(3 << 27, 27)]
        [InlineData(int.MaxValue, 0)]
        public static void BitOps_TrailingZeroCount_int(int n, int expected)
        {
            int actual = BitOps.TrailingZeroCount(n);
            Assert.Equal(expected, actual);
        }

        #endregion
    }
}
