// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using Xunit;

namespace System.Numerics.Tests
{
    public class BigIntegerAddTests
    {
        public static IEnumerable<object[]> UnaryPlus_TestData()
        {
            yield return new object[] { BigInteger.Parse("123123123123123123123123123123123123123123123123123123123123123123123123123123123123123123123123") };
            yield return new object[] { BigInteger.Parse("-123123123123123123123123123123123123123123123123123123123123123123123123123123123123123123123123") };
            yield return new object[] { new BigInteger(123) };
            yield return new object[] { new BigInteger(-123) };
            yield return new object[] { new BigInteger(0) };
            yield return new object[] { new BigInteger(-1) };
            yield return new object[] { new BigInteger(1) };
            yield return new object[] { new BigInteger(int.MinValue) };
            yield return new object[] { new BigInteger(int.MaxValue) };
            yield return new object[] { new BigInteger(long.MaxValue) };
            yield return new object[] { new BigInteger(long.MinValue) };
        }

        [Theory]
        [MemberData(nameof(UnaryPlus_TestData))]
        public static void UnaryPlus(BigInteger bigInteger)
        {
            Assert.Equal(bigInteger, +bigInteger);
        }

        public static IEnumerable<object[]> BinaryPlus_TestData()
        {
            yield return new object[] { new BigInteger(int.MinValue), new BigInteger(-1), new BigInteger((long)int.MinValue - 1) };
            yield return new object[] { new BigInteger(int.MinValue), new BigInteger(1), new BigInteger(int.MinValue + 1) };
            yield return new object[] { new BigInteger(int.MaxValue), new BigInteger(-1), new BigInteger(int.MaxValue - 1) };
            yield return new object[] { new BigInteger(int.MaxValue), new BigInteger(1), new BigInteger((long)int.MaxValue + 1) };

            yield return new object[] { new BigInteger(long.MinValue), new BigInteger(-1), new BigInteger(new byte[] { 255, 255, 255, 255, 255, 255, 255, 127, 255 }) };
            yield return new object[] { new BigInteger(long.MinValue), new BigInteger(1), new BigInteger(long.MinValue + 1) };
            yield return new object[] { new BigInteger(long.MaxValue), new BigInteger(-1), new BigInteger(long.MaxValue - 1) };
            yield return new object[] { new BigInteger(long.MaxValue), new BigInteger(1), new BigInteger((ulong)long.MaxValue + 1) };

            BigInteger largePositiveBigInt = BigInteger.Parse("123123123123123123123123123123123123123123123123123123123123123123123123123123123123123");
            BigInteger largeNegativeBigInt = BigInteger.Parse("-123123123123123123123123123123123123123123123123123123123123123123123123123123123123123");

            // Big + Big
            yield return new object[] { largePositiveBigInt, BigInteger.Parse("234234234234234234234234234234234234234234234234234234234234234234234234"), BigInteger.Parse("123123123123123357357357357357357357357357357357357357357357357357357357357357357357357") };
            yield return new object[] { largePositiveBigInt, BigInteger.Parse("-234234234234234234234234234234234234234234234234234234234234234234234234"), BigInteger.Parse("123123123123122888888888888888888888888888888888888888888888888888888888888888888888889") };

            // Big + Small
            yield return new object[] { largePositiveBigInt, new BigInteger(123), BigInteger.Parse("123123123123123123123123123123123123123123123123123123123123123123123123123123123123246") };
            yield return new object[] { largePositiveBigInt, new BigInteger(-123), BigInteger.Parse("123123123123123123123123123123123123123123123123123123123123123123123123123123123123000") };

            // Plus Zero
            yield return new object[] { largePositiveBigInt, BigInteger.Zero, largePositiveBigInt };
            yield return new object[] { largeNegativeBigInt, BigInteger.Zero, largeNegativeBigInt };
            yield return new object[] { new BigInteger(123), BigInteger.Zero, new BigInteger(123) };
            yield return new object[] { new BigInteger(-123), BigInteger.Zero, new BigInteger(-123) };
            yield return new object[] { BigInteger.Zero, BigInteger.Zero, BigInteger.Zero };

            // Boundaries
            yield return new object[] { new BigInteger(Math.Pow(2, 31) + Math.Pow(2, 30)), new BigInteger(Math.Pow(2, 31) + Math.Pow(2, 30)), new BigInteger(6442450944) };
            yield return new object[] { BigInteger.Zero, new BigInteger(Math.Pow(2, 32)), new BigInteger(Math.Pow(2, 32)) };
            yield return new object[] { new BigInteger(Math.Pow(2, 31)), new BigInteger(Math.Pow(2, 32) + Math.Pow(2, 31)), new BigInteger(8589934592) };
            yield return new object[] { new BigInteger(Math.Pow(2, 32)), new BigInteger(Math.Pow(2, 32)), new BigInteger(8589934592) };
            yield return new object[] { new BigInteger(Math.Pow(2, 32) + Math.Pow(2, 31)), new BigInteger(Math.Pow(2, 32) + Math.Pow(2, 31)), new BigInteger(12884901888) };
        }

        [Theory]
        [MemberData(nameof(BinaryPlus_TestData))]
        public static void BinaryPlus(BigInteger bigInteger1, BigInteger bigInteger2, BigInteger expected)
        {
            Assert.Equal(expected, bigInteger1 + bigInteger2);
            Assert.Equal(expected, bigInteger2 + bigInteger1);

            Assert.Equal(expected, BigInteger.Add(bigInteger1, bigInteger2));
            Assert.Equal(expected, BigInteger.Add(bigInteger2, bigInteger1));
        }
    }
}
