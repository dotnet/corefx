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

        [Fact]
        public static void DefaultBitArray()
        {
            CustomUncheckedBitArray bitArray = default;
            Assert.True(bitArray.IsDefault);
            Assert.Equal(-1, bitArray.MaxIndexableLength);
        }

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
            Assert.False(bitArray.IsDefault);
            Assert.Equal(bitLength - 1, bitArray.MaxIndexableLength);

            var values = new bool[bitArray.MaxIndexableLength + 1];
            for (int i = 0; i < values.Length; i++)
            {
                values[i] = s_random.NextDouble() >= 0.5;
            }

            for (int i = 0; i <= bitArray.MaxIndexableLength; i++)
            {
                bitArray[i] = values[i];
            }

            for (int i = 0; i <= bitArray.MaxIndexableLength; i++)
            {
                Assert.Equal(values[i], bitArray[i]);
            }
        }

        [Theory]
        [InlineData(3_200_000)]
        [InlineData(int.MaxValue / 32 + 1)]    // 67_108_864
        public static void BitArrayGetSetLarge(int bitLength)
        {
            var bitArray = new CustomUncheckedBitArray(bitLength);
            Assert.False(bitArray.IsDefault);
            Assert.Equal(bitLength - 1, bitArray.MaxIndexableLength);

            var values = new bool[bitArray.MaxIndexableLength + 1];
            for (int i = 0; i < values.Length; i++)
            {
                values[i] = s_random.NextDouble() >= 0.5;
            }

            const int IterationCapacity = 1_600_000;

            // Only set and compare the first and last few (otherwise, the test takes too long)
            for (int i = 0; i <= IterationCapacity; i++)
            {
                bitArray[i] = values[i];
            }
            for (int i = bitLength - IterationCapacity; i <= bitArray.MaxIndexableLength; i++)
            {
                bitArray[i] = values[i];
            }

            for (int i = 0; i <= IterationCapacity; i++)
            {
                Assert.Equal(values[i], bitArray[i]);
            }
            for (int i = bitLength - IterationCapacity; i <= bitArray.MaxIndexableLength; i++)
            {
                Assert.Equal(values[i], bitArray[i]);
            }
        }

        [Theory]
        [InlineData(32)]
        [InlineData(64)]
        [InlineData(128)]
        [InlineData(256)]
        public static void BitArrayGrow(int bitLength)
        {
            var bitArray = new CustomUncheckedBitArray(bitLength);
            Assert.Equal(bitLength - 1, bitArray.MaxIndexableLength);
            Assert.False(bitArray.IsDefault);

            const int growBy = 128;

            var values = new bool[bitLength + growBy];
            for (int i = 0; i < values.Length; i++)
            {
                values[i] = s_random.NextDouble() >= 0.5;
            }

            for (int i = 0; i < bitLength + growBy; i++)
            {
                bitArray[i] = values[i];
                Assert.True(bitArray.MaxIndexableLength >= i);
            }

            Assert.True(bitLength - 1 + growBy <= bitArray.MaxIndexableLength);

            int expectedLength = NextClosestPowerOf2(bitLength + growBy) - 1;

            Assert.True(expectedLength == bitArray.MaxIndexableLength, $"expected: {expectedLength}, actual: {bitArray.MaxIndexableLength}, bitLength: {bitLength}, growBy: {growBy}");
            for (int i = 0; i < values.Length; i++)
            {
                Assert.True(values[i] == bitArray[i], $"expected: {values[i]}, actual: {bitArray[i]}, index: {i}, Length: {bitArray.MaxIndexableLength}");
            }

            // Extra bits start off as false.
            for (int i = values.Length; i < bitArray.MaxIndexableLength; i++)
            {
                Assert.False(bitArray[i], $"index: {i}, Length: {bitArray.MaxIndexableLength}, ValuesLength: {values.Length}");
            }
        }

        [Fact]
        public static void BitArrayGrowByN()
        {
            const int bitLength = 32;
            
            for (int growBy = 0; growBy < 100; growBy++)
            {
                var bitArray = new CustomUncheckedBitArray(bitLength);
                Assert.Equal(bitLength - 1, bitArray.MaxIndexableLength);
                Assert.False(bitArray.IsDefault);

                bool value = s_random.NextDouble() >= 0.5;

                int index = bitLength + growBy - 1;
                bitArray[index] = value;
                Assert.True(bitArray.MaxIndexableLength >= index);

                // Round down to closest multiple of 32 and then add 31.
                int expectedLength = ((index >> 5) << 5) + 31;

                Assert.True(expectedLength == bitArray.MaxIndexableLength, $"expected: {expectedLength}, actual: {bitArray.MaxIndexableLength}, bitLength: {bitLength}, growBy: {growBy}");
                Assert.True(value == bitArray[index], $"expected: {value}, actual: {bitArray[index]}, index: {index}, Length: {bitArray.MaxIndexableLength}");

                // Extra bits start off as false.
                for (int i = index + 1; i < bitArray.MaxIndexableLength; i++)
                {
                    Assert.False(bitArray[i], $"index: {i}, Length: {bitArray.MaxIndexableLength}");
                }
            }
        }

        private static int NextClosestPowerOf2(int n)
        {
            // Required to handle powers of 2.
            n--;

            // Set all the bits to the right of the leftmost set bit to 1.
            n |= n >> 1;
            n |= n >> 2;
            n |= n >> 4;
            n |= n >> 8;
            n |= n >> 16;

            n++;

            return n;
        }
    }
}
