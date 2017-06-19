// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.Tests
{
    public class BooleanTests
    {
        [Fact]
        public void TrueString_Get_ReturnsTrue()
        {
            Assert.Equal("True", bool.TrueString);
        }

        [Fact]
        public void FalseString_Get_ReturnsFalse()
        {
            Assert.Equal("False", bool.FalseString);
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
        public void Parse_ValidValue_ReturnsExpected(string value, bool expected)
        {
            Assert.True(bool.TryParse(value, out bool result));
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
        public void Parse_InvalidValue_ThrowsException(string value, Type exceptionType)
        {
            Assert.Throws(exceptionType, () => bool.Parse(value));

            Assert.False(bool.TryParse(value, out bool result));
            Assert.False(result);
        }

        [Theory]
        [InlineData(true, "True")]
        [InlineData(false, "False")]
        public void ToString_Invoke_ReturnsExpected(bool value, string expected)
        {
            Assert.Equal(expected, value.ToString());
        }

        [Theory]
        [InlineData(true, true, 0)]
        [InlineData(true, false, 1)]
        [InlineData(true, null, 1)]
        [InlineData(false, false, 0)]
        [InlineData(false, true, -1)]
        [InlineData(false, null, 1)]
        public void CompareTo_Other_ReturnsExpected(bool b, object obj, int expected)
        {
            if (obj is bool boolValue)
            {
                Assert.Equal(expected, Math.Sign(b.CompareTo(boolValue)));
            }

            Assert.Equal(expected, Math.Sign(b.CompareTo(obj)));
        }

        [Theory]
        [InlineData(true, 1)]
        [InlineData(true, "true")]
        [InlineData(false, 0)]
        [InlineData(false, "false")]
        private void CompareTo_ObjectNotBool_ThrowsArgumentException(bool b, object obj)
        {
            AssertExtensions.Throws<ArgumentException>(null, () => b.CompareTo(obj));
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
        public void Equals_Other_ReturnsExpected(bool b1, object obj, bool expected)
        {
            if (obj is bool boolValue)
            {
                Assert.Equal(expected, b1.Equals(boolValue));
                Assert.Equal(expected, b1.GetHashCode().Equals(obj.GetHashCode()));
            }

            Assert.Equal(expected, b1.Equals(obj));
        }

        [Theory]
        [InlineData(true, 1)]
        [InlineData(false, 0)]
        public void GetHashCode_Invoke_ReturnsExpected(bool value, int expected)
        {
            Assert.Equal(expected, value.GetHashCode());
        }

        [Fact]
        public void GetTypeCode_Invoke_ReturnsBoolean()
        {
            Assert.Equal(TypeCode.Boolean, true.GetTypeCode());
        }
    }
}
