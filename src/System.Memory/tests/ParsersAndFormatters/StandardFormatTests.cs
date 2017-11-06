// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Text;
using System.Linq;

using Xunit;

namespace System.Buffers.Text.Tests
{
    public static partial class StandardFormatTests
    {
        [Fact]
        public static void StandardFormatCtorNegative()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new StandardFormat((char)256));
            Assert.Throws<ArgumentOutOfRangeException>(() => new StandardFormat('D', StandardFormat.MaxPrecision + 1));
        }

        [Theory]
        [InlineData("G4", 'G', 4)]
        [InlineData("n0", 'n', 0)]
        [InlineData("d", 'd', StandardFormat.NoPrecision)]
        [InlineData("x99", 'x', StandardFormat.MaxPrecision)]
        [InlineData(null, default(char), default(byte))]
        [InlineData("", default(char), default(byte))]
        public static void StandardFormatParseString(string formatString, char expectedSymbol, byte expectedPrecision)
        {
            StandardFormat format = StandardFormat.Parse(formatString);
            Assert.Equal(expectedSymbol, format.Symbol);
            Assert.Equal(expectedPrecision, format.Precision);
        }

        [Theory]
        [InlineData("G4", 'G', 4)]
        [InlineData("n0", 'n', 0)]
        [InlineData("d", 'd', StandardFormat.NoPrecision)]
        [InlineData("x99", 'x', StandardFormat.MaxPrecision)]
        [InlineData("", default(char), default(byte))]
        public static void StandardFormatParseSpan(string formatString, char expectedSymbol, byte expectedPrecision)
        {
            ReadOnlySpan<char> span = formatString.AsReadOnlySpan();
            StandardFormat format = StandardFormat.Parse(span);
            Assert.Equal(expectedSymbol, format.Symbol);
            Assert.Equal(expectedPrecision, format.Precision);
        }

        [Theory]
        [InlineData("G$")]
        [InlineData("Ga")]
        [InlineData("G100")]
        public static void StandardFormatParseNegative(string badFormatString)
        {
            Assert.Throws<FormatException>(() => StandardFormat.Parse(badFormatString));
        }

        [Theory]
        [InlineData('a')]
        [InlineData('B')]
        public static void StandardFormatOpImplicitFromChar(char c)
        {
            StandardFormat format = c;
            Assert.Equal(c, format.Symbol);
            Assert.Equal(StandardFormat.NoPrecision, format.Precision);
        }

        [Theory]
        [MemberData(nameof(EqualityTestData))]
        public static void StandardFormatEquality(StandardFormat f1, StandardFormat f2, bool expectedToBeEqual)
        {
            {
                bool actual = f1.Equals(f2);
                Assert.Equal(expectedToBeEqual, actual);
            }
        }

        [Theory]
        [MemberData(nameof(EqualityTestData))]
        public static void StandardFormatBoxedEquality(StandardFormat f1, StandardFormat f2, bool expectedToBeEqual)
        {
            object boxedf2 = f2;
            bool actual = f1.Equals(boxedf2);
            Assert.Equal(expectedToBeEqual, actual);

        }

        [Theory]
        [MemberData(nameof(EqualityTestData))]
        public static void StandardFormatOpEquality(StandardFormat f1, StandardFormat f2, bool expectedToBeEqual)
        {
            {
                bool actual = f1 == f2;
                Assert.Equal(expectedToBeEqual, actual);
            }
        }

        [Theory]
        [MemberData(nameof(EqualityTestData))]
        public static void StandardFormatOpInequality(StandardFormat f1, StandardFormat f2, bool expectedToBeEqual)
        {
            {
                bool actual = f1 != f2;
                Assert.NotEqual(expectedToBeEqual, actual);
            }
        }

        [Theory]
        [MemberData(nameof(EqualityTestData))]
        public static void StandardFormatGetHashCode(StandardFormat f1, StandardFormat f2, bool expectedToBeEqual)
        {
            if (expectedToBeEqual)
            {
                int h1 = f1.GetHashCode();
                int h2 = f2.GetHashCode();
                Assert.Equal(h1, h2);
            }
        }

        [Theory]
        [MemberData(nameof(EqualityTestData))]
        public static void StandardFormatGetHashCodeIsContentBased(StandardFormat f1, StandardFormat f2, bool expectedToBeEqual)
        {
            object boxedf1 = f1;
            object aDifferentBoxedF1 = f1;
            int h1 = boxedf1.GetHashCode();
            int h2 = aDifferentBoxedF1.GetHashCode();
            Assert.Equal(h1, h2);
        }

        public static IEnumerable<object[]> EqualityTestData
        {
            get
            {
                yield return new object[] { new StandardFormat('A', 3), new StandardFormat('A', 3), true };
                yield return new object[] { new StandardFormat('a', 3), new StandardFormat('A', 3), false };
                yield return new object[] { new StandardFormat('A', 3), new StandardFormat('A', 4), false };
                yield return new object[] { new StandardFormat('A', 3), new StandardFormat('A', StandardFormat.NoPrecision), false };
            }
        }
    }
}

