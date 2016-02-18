// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.Tests.Common;

using Xunit;

namespace System.Runtime.Tests
{
    public static class BooleanTests
    {
        [Fact]
        public static void TestFalseString()
        {
            Assert.Equal("False", bool.FalseString);
        }

        [Fact]
        public static void TestTrueString()
        {
            Assert.Equal("True", bool.TrueString);
        }

        [Theory]
        [InlineData(true, true, 0)]
        [InlineData(true, false, 1)]
        [InlineData(true, null, 1)]
        [InlineData(false, false, 0)]
        [InlineData(false, true, -1)]
        [InlineData(false, null, 1)]
        public static void TestCompareTo(bool b1, object obj, int expected)
        {
            if (obj is bool)
            {
                Assert.Equal(expected, CompareHelper.NormalizeCompare(b1.CompareTo((bool)obj)));
            }
            
            IComparable comparable = b1;
            Assert.Equal(expected, CompareHelper.NormalizeCompare(comparable.CompareTo(obj)));
        }

        [Theory]
        [InlineData(true, 1)]
        [InlineData(true, "true")]
        [InlineData(false, 0)]
        [InlineData(false, "false")]
        private static void TestCompareTo_Invalid(IComparable b, object obj)
        {
            Assert.Throws<ArgumentException>(null, () => b.CompareTo(obj));
        }

        [Theory]
        [InlineData(true, true, true)]
        [InlineData(true, false, false)]
        [InlineData(true, "1", false)]
        [InlineData(true, "True", false)]
        [InlineData(true, null, false)]
        [InlineData(false, false, true)]
        [InlineData(false, true, false)]
        [InlineData(false, "0", false)]
        [InlineData(false, "False", false)]
        [InlineData(false, null, false)]
        public static void TestEquals(bool b1, object obj, bool expected)
        {
            if (obj is bool)
            {
                bool b2 = (bool)obj;
                Assert.Equal(expected, b1.Equals(b2));
                Assert.Equal(expected, b1.GetHashCode().Equals(b2.GetHashCode()));
            }
            Assert.Equal(expected, b1.Equals(obj));
        }

        [Fact]
        public static void TestGetHashCode()
        {
            Assert.Equal(1, true.GetHashCode());
            Assert.Equal(0, false.GetHashCode());
        }
        
        [Theory]
        [InlineData("True", true)]
        [InlineData("true", true)]
        [InlineData("TRUE", true)]
        [InlineData("tRuE", true)]
        [InlineData("  True  ", true)]
        [InlineData("True\0", true)]
        [InlineData(" \0 \0  True   \0 ", true)]
        [InlineData("False", false)]
        [InlineData("false", false)]
        [InlineData("FALSE", false)]
        [InlineData("fAlSe", false)]
        [InlineData("False  ", false)]
        [InlineData("False\0", false)]
        [InlineData("  False \0\0\0  ", false)]
        public static void TestParse(string value, bool expected)
        {
            bool result;
            Assert.True(bool.TryParse(value, out result));
            Assert.Equal(expected, result);

            Assert.Equal(expected, bool.Parse(value));
        }

        [Theory]
        [InlineData(null, typeof(ArgumentNullException))]
        [InlineData("", typeof(FormatException))]
        [InlineData(" ", typeof(FormatException))]
        [InlineData("Garbage", typeof(FormatException))]
        [InlineData("True\0Garbage", typeof(FormatException))]
        [InlineData("True\0True", typeof(FormatException))]
        [InlineData("True True", typeof(FormatException))]
        [InlineData("True False", typeof(FormatException))]
        [InlineData("False True", typeof(FormatException))]
        [InlineData("Fa lse", typeof(FormatException))]
        [InlineData("T", typeof(FormatException))]
        [InlineData("0", typeof(FormatException))]
        [InlineData("1", typeof(FormatException))]
        public static void TestParse_Invalid(string value, Type exceptionType)
        {
            Assert.Throws(exceptionType, () => bool.Parse(value));

            bool result;
            Assert.False(bool.TryParse(value, out result));
        }

        [Fact]
        public static void TestToString()
        {
            Assert.Equal(bool.TrueString, true.ToString());
            Assert.Equal(bool.FalseString, false.ToString());
        }
    }
}
