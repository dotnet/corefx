// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Xunit;

namespace Microsoft.VisualBasic.Tests
{
    public class StringsTests
    {
        [Fact]
        public void Asc_Char_ReturnsChar()
        {
            Assert.Equal('3', Strings.Asc('3'));
        }

        [Theory]
        [InlineData("3", 51)]
        [InlineData("345", 51)]
        [InlineData("ABCD", 65)]
        public void Asc_String_ReturnsExpected(string String, int expected)
        {
            Assert.Equal(expected, Strings.Asc(String));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void Asc_NullOrEmpty_ThrowsArgumentException(string String)
        {
            AssertExtensions.Throws<ArgumentException>("String", () => Strings.Asc(String));
        }

        [Fact]
        public void AscW_Char_ReturnsChar()
        {
            Assert.Equal('3', Strings.AscW('3'));
        }

        [Theory]
        [InlineData("3", 51)]
        [InlineData("345", 51)]
        [InlineData("ABCD", 65)]
        public void AscW_String_ReturnsExpected(string String, int expected)
        {
            Assert.Equal(expected, Strings.AscW(String));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void AscW_NullOrEmpty_ThrowsArgumentException(string String)
        {
            AssertExtensions.Throws<ArgumentException>("String", () => Strings.AscW(String));
        }

        [Theory]
        [InlineData(97)]
        [InlineData(65)]
        [InlineData(0)]
        public void Chr_CharCodeInRange_ReturnsExpected(int charCode)
        {
            Assert.Equal(Convert.ToChar(charCode & 0XFFFF), Strings.Chr(charCode));
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(256)]
        public void Chr_CharCodeOutOfRange_ThrowsNotSupportedException(int charCode)
        {
            // Documentation claims that < 0 or > 255 gives an ArgumentException but it doesn't
            Assert.Throws<NotSupportedException>(() => Strings.Chr(charCode));
        }

        [Theory]
        [InlineData(-32769)]
        [InlineData(65536)]
        public void Chr_CharCodeOutOfRange_ThrowsArgumentException(int charCode)
        {
            AssertExtensions.Throws<ArgumentException>("CharCode", () => Strings.Chr(charCode));
        }

        [Theory]
        [InlineData(97)]
        [InlineData(65)]
        [InlineData(65535)]
        [InlineData(-32768)]
        public void ChrW_CharCodeInRange_ReturnsExpected(int charCode)
        {
            Assert.Equal(Convert.ToChar(charCode & 0XFFFF), Strings.ChrW(charCode));
        }

        [Theory]
        [InlineData(-32769)]
        [InlineData(65536)]
        public void ChrW_CharCodeOutOfRange_ThrowsArgumentException(int charCode)
        {
            AssertExtensions.Throws<ArgumentException>("CharCode", () => Strings.ChrW(charCode));
        }

        [Theory]
        [InlineData("a", -1)]
        public void Left_Invalid(string str, int length)
        {
            AssertExtensions.Throws<ArgumentException>("Length", () => Strings.Left(str, length));
        }

        [Theory]
        [InlineData("", 0, "")]
        [InlineData("", 1, "")]
        [InlineData("abc", 0, "")]
        [InlineData("abc", 2, "ab")]
        [InlineData("abc", int.MaxValue, "abc")]
        public void Left_Valid(string str, int length, string expected)
        {
            Assert.Equal(expected, Strings.Left(str, length));
        }

        [Theory]
        [InlineData(null, "")]
        [InlineData("", "")]
        [InlineData(" ", "")]
        [InlineData(" abc ", "abc ")]
        [InlineData("\u3000\nabc ", "\nabc ")]
        [InlineData("\nabc ", "\nabc ")]
        [InlineData("abc ", "abc ")]
        [InlineData("abc", "abc")]
        public void LTrim_Valid(string str, string expected)
        {
            // Trims only space and \u3000 specifically
            Assert.Equal(expected, Strings.LTrim(str));
        }
        [Theory]
        [InlineData(null, "")]
        [InlineData("", "")]
        [InlineData(" ", "")]
        [InlineData(" abc ", " abc")]
        [InlineData(" abc\n\u3000", " abc\n")]
        [InlineData(" abc\n", " abc\n")]
        [InlineData(" abc", " abc")]
        [InlineData("abc", "abc")]
        public void RTrim_Valid(string str, string expected)
        {
            // Trims only space and \u3000 specifically
            Assert.Equal(expected, Strings.RTrim(str));
        }
    }
}
