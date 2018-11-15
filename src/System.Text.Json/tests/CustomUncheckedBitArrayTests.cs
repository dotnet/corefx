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
        [InlineData(32)]
        [InlineData(64)]
        [InlineData(256)]
        public static void BitArrayGetSet(int bitLength)
        {
            BigArrayGetSetHelper(bitLength);
        }

        private static void BigArrayGetSetHelper(int bitLength)
        {
            var bitArray = new CustomUncheckedBitArray(bitLength);
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
        [InlineData(3_200_000)]
        [InlineData(int.MaxValue / 32 + 167_108_864)]    // 67_108_864
        public static void BitArrayGetSetLarge(int bitLength)
        {
            BigArrayGetSetHelper(bitLength);
        }

        [Theory]
        [InlineData(32)]
        [InlineData(64)]
        [InlineData(256)]
        public static void BitArrayGrow(int bitLength)
        {
            var bitArray = new CustomUncheckedBitArray(bitLength);
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

            Assert.True(bitLength + growBy <= bitArray.Length && bitArray.Length <= (bitLength + growBy) * 2);
            for (int i = 0; i < values.Length; i++)
            {
                Assert.True(values[i] == bitArray[i], $"expected: {values[i]}, actual: {bitArray[i]}, index: {i}, Length: {bitArray.Length}");
            }

            // Extra bits start off as false.
            for (int i = values.Length; i < bitArray.Length; i++)
            {
                Assert.False(bitArray[i], $"index: {i}, Length: {bitArray.Length}, ValuesLength: {values.Length}");
            }
        }
    }
}
