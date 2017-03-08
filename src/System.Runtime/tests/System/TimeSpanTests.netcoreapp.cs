// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using Xunit;

namespace System.Tests
{
    public static partial class TimeSpanTests
    {
        private static IEnumerable<object[]> MultiplicationTestData()
        {
            yield return new object[] {new TimeSpan(2, 30, 0), 2.0, new TimeSpan(5, 0, 0)};
            yield return new object[] {new TimeSpan(14, 2, 30, 0), 192.0, TimeSpan.FromDays(2708)};
            yield return new object[] {TimeSpan.FromDays(366), Math.PI, new TimeSpan(993446995288779)};
            yield return new object[] {TimeSpan.FromDays(366), -Math.E, new TimeSpan(-859585952922633)};
            yield return new object[] {TimeSpan.FromDays(29.530587981), 13.0, TimeSpan.FromDays(383.897643819444)};
            yield return new object[] {TimeSpan.FromDays(-29.530587981), -12.0, TimeSpan.FromDays(354.367055833333)};
            yield return new object[] {TimeSpan.FromDays(-29.530587981), 0.0, TimeSpan.Zero};
            yield return new object[] {TimeSpan.MaxValue, 0.5, TimeSpan.FromTicks((long)(long.MaxValue * 0.5))};
        }

        [Theory, MemberData(nameof(MultiplicationTestData))]
        public static void Multiplication(TimeSpan timeSpan, double factor, TimeSpan expected)
        {
            Assert.Equal(expected, timeSpan * factor);
            Assert.Equal(expected, factor * timeSpan);
        }

        [Fact]
        public static void OverflowingMultiplication()
        {
            Assert.Throws<OverflowException>(() => TimeSpan.MaxValue * 1.000000001);
            Assert.Throws<OverflowException>(() => -1.000000001 * TimeSpan.MaxValue);
        }

        [Fact]
        public static void NaNMultiplication()
        {
            Assert.Throws<ArgumentException>("factor", () => TimeSpan.FromDays(1) * double.NaN);
            Assert.Throws<ArgumentException>("factor", () => double.NaN * TimeSpan.FromDays(1));
        }

        [Theory, MemberData(nameof(MultiplicationTestData))]
        public static void Division(TimeSpan timeSpan, double factor, TimeSpan expected)
        {
            Assert.Equal(factor, expected / timeSpan, 14);
            double divisor = 1.0 / factor;
            Assert.Equal(expected, timeSpan / divisor);
        }

        [Fact]
        public static void DivideByZero()
        {
            Assert.Throws<OverflowException>(() => TimeSpan.FromDays(1) / 0);
            Assert.Throws<OverflowException>(() => TimeSpan.FromDays(-1) / 0);
            Assert.Throws<OverflowException>(() => TimeSpan.Zero / 0);
            Assert.Equal(double.PositiveInfinity, TimeSpan.FromDays(1) / TimeSpan.Zero);
            Assert.Equal(double.NegativeInfinity, TimeSpan.FromDays(-1) / TimeSpan.Zero);
            Assert.True(double.IsNaN(TimeSpan.Zero / TimeSpan.Zero));
        }

        [Fact]
        public static void NaNDivision()
        {
            Assert.Throws<ArgumentException>("divisor", () => TimeSpan.FromDays(1) / double.NaN);
        }
    }
}