// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Text.Json;
using Xunit;

namespace System.Text.JsonTests
{
    public static partial class CustomUncheckedBitArrayTests
    {
        private static Random s_random = new Random(42);

        [Theory]
        [InlineData(1, 1)]
        [InlineData(2, 1)]
        [InlineData(31, 1)]
        [InlineData(32, 1)]
        [InlineData(33, 2)]
        [InlineData(63, 2)]
        [InlineData(64, 2)]
        [InlineData(65, 3)]
        [InlineData(255, 8)]
        [InlineData(256, 8)]
        [InlineData(257, 9)]
        public static void BitArrayGetSet(int bitLength, int intLength)
        {
            BigArrayGetSetHelper(bitLength, intLength);
        }

        private static void BigArrayGetSetHelper(int bitLength, int intLength)
        {
            var bitArray = new CustomUncheckedBitArray(bitLength, intLength);
            Assert.Equal(bitLength, bitArray.Length);

            var values = new bool[bitArray.Length];
            for (int i = 0; i < values.Length; i++)
            {
                values[i] = s_random.NextDouble() >= 0.5;
            }

            for (int i = 0; i < bitArray.Length; i++)
            {
                bitArray[i] = values[i];
            }

            for (int i = 0; i < bitArray.Length; i++)
            {
                Assert.Equal(values[i], bitArray[i]);
            }
        }

        [Theory]
        [OuterLoop]
        [InlineData(3_200_000, 100_000)]
        [InlineData(int.MaxValue / 32 + 1, (int.MaxValue / 32 + 1) / 32)]    // 67_108_864, 2_097_152
        public static void BitArrayGetSetLarge(int bitLength, int intLength)
        {
            BigArrayGetSetHelper(bitLength, intLength);
        }

        [Theory]
        [InlineData(1, 1)]
        [InlineData(2, 1)]
        [InlineData(31, 1)]
        [InlineData(32, 1)]
        [InlineData(33, 2)]
        [InlineData(63, 2)]
        [InlineData(64, 2)]
        [InlineData(65, 3)]
        [InlineData(255, 8)]
        [InlineData(256, 8)]
        [InlineData(257, 9)]
        public static void BitArrayGrow(int bitLength, int intLength)
        {
            var bitArray = new CustomUncheckedBitArray(bitLength, intLength);
            Assert.Equal(bitLength, bitArray.Length);

            const int growBy = 128;

            var values = new bool[bitLength + growBy];
            for (int i = 0; i < values.Length; i++)
            {
                values[i] = s_random.NextDouble() >= 0.5;
            }

            for (int i = 0; i < bitLength + growBy; i++)
            {
                bitArray[i] = values[i];
                Assert.True(bitArray.Length >= i);
            }

            Assert.Equal(bitLength + growBy, bitArray.Length);
            for (int i = 0; i < bitArray.Length; i++)
            {
                Assert.True(values[i] == bitArray[i], $"expected: {values[i]}, actual: {bitArray[i]}, index: {i}, Length: {bitArray.Length}");
            }
        }
    }
}
