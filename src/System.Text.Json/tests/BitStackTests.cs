// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Text.Json;
using Xunit;

namespace System.Text.JsonTests
{
    public static partial class BitStackTests
    {
        private static Random s_random = new Random(42);

        [Fact]
        public static void DefaultBitStack()
        {
            BitStack bitStack = default;
            Assert.Equal((ulong)0, bitStack._allocationFreeContainer);
        }

        [Theory]
        [InlineData(32)]
        [InlineData(64)]
        [InlineData(256)]
        public static void BitStackPushPop(int bitLength)
        {
            BitStack bitStack = default;
            Assert.Equal((ulong)0, bitStack._allocationFreeContainer);

            var values = new bool[bitLength];
            for (int i = 0; i < bitLength; i++)
            {
                values[i] = s_random.NextDouble() >= 0.5;
            }

            for (int i = 0; i < bitLength; i++)
            {
                if (values[i])
                {
                    bitStack.PushTrueAt(i);
                }
                else
                {
                    bitStack.PushFalseAt(i);
                }
            }

            for (int i = bitLength - 1; i > 0; i--)
            {
                // We need the value at the top *after* popping off the last one.
                Assert.Equal(values[i - 1], bitStack.PopAt(i));
            }
        }

        [Theory]
        [InlineData(3_200_000)]
        [InlineData(int.MaxValue / 32 + 1)]    // 67_108_864
        public static void BitStackPushPopLarge(int bitLength)
        {
            BitStack bitStack = default;
            Assert.Equal((ulong)0, bitStack._allocationFreeContainer);

            var values = new bool[bitLength];
            for (int i = 0; i < bitLength; i++)
            {
                values[i] = s_random.NextDouble() >= 0.5;
            }

            const int IterationCapacity = 1_600_000;

            // Only set and compare the first and last few (otherwise, the test takes too long)
            for (int i = 0; i <= IterationCapacity; i++)
            {
                if (values[i])
                {
                    bitStack.PushTrueAt(i);
                }
                else
                {
                    bitStack.PushFalseAt(i);
                }
            }
            for (int i = bitLength - IterationCapacity; i < bitLength; i++)
            {
                if (values[i])
                {
                    bitStack.PushTrueAt(i);
                }
                else
                {
                    bitStack.PushFalseAt(i);
                }
            }

            for (int i = bitLength - 1; i > bitLength - IterationCapacity; i--)
            {
                Assert.Equal(values[i - 1], bitStack.PopAt(i));
            }
            for (int i = IterationCapacity - 1; i > 0; i--)
            {
                Assert.Equal(values[i - 1], bitStack.PopAt(i));
            }
            
        }
    }
}
