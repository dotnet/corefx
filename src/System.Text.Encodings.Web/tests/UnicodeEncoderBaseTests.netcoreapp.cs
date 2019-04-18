// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using Xunit;

namespace Microsoft.Framework.WebEncoders
{
    public class UnicodeEncoderBaseTests
    {
        [Fact]
        public void Ctor_WithCustomFilters()
        {
            // Arrange
            var filter = new TextEncoderSettings();
            filter.AllowCharacters('a', 'b');
            filter.AllowCharacters('\0', '&', '\uFFFF', 'd');
            CustomTextEncoder encoder = new CustomTextEncoder(filter);

            // Act & assert
            Assert.Equal("a", encoder.Encode("a"));
            Assert.Equal("b", encoder.Encode("b"));
            Assert.Equal("[U+0063]", encoder.Encode("c"));
            Assert.Equal("d", encoder.Encode("d"));
            Assert.Equal("[U+0000]", encoder.Encode("\0")); // we still always encode control chars
            Assert.Equal("[U+0026]", encoder.Encode("&")); // we still always encode HTML-special chars
            Assert.Equal("[U+FFFF]", encoder.Encode("\uFFFF")); // we still always encode non-chars and other forbidden chars
        }

        [Fact]
        public void Ctor_WithUnicodeRanges()
        {
            // Arrange
            CustomTextEncoder encoder = new CustomTextEncoder(new TextEncoderSettings(UnicodeRanges.Latin1Supplement, UnicodeRanges.MiscellaneousSymbols));

            // Act & assert
            Assert.Equal("[U+0061]", encoder.Encode("a"));
            Assert.Equal("\u00E9", encoder.Encode("\u00E9" /* LATIN SMALL LETTER E WITH ACUTE */));
            Assert.Equal("\u2601", encoder.Encode("\u2601" /* CLOUD */));
        }

        [Fact]
        public void Encode_AllRangesAllowed_StillEncodesForbiddenChars_Simple()
        {
            // Arrange
            CustomTextEncoder encoder = new CustomTextEncoder(UnicodeRanges.All);
            const string input = "Hello <>&\'\"+ there!";
            const string expected = "Hello [U+003C][U+003E][U+0026][U+0027][U+0022][U+002B] there!";

            // Act & assert
            Assert.Equal(expected, encoder.Encode(input));
        }

        [Fact]
        public void Encode_AllRangesAllowed_StillEncodesForbiddenChars_Extended()
        {
            // Arrange
            CustomTextEncoder encoder = new CustomTextEncoder(UnicodeRanges.All);

            // Act & assert - BMP chars
            for (int i = 0; i <= 0xFFFF; i++)
            {
                string input = new string((char)i, 1);
                string expected;
                if (IsSurrogateCodePoint(i))
                {
                    expected = "\uFFFD"; // unpaired surrogate -> Unicode replacement char
                }
                else
                {
                    bool mustEncode = false;
                    switch (i)
                    {
                        case '<':
                        case '>':
                        case '&':
                        case '\"':
                        case '\'':
                        case '+':
                            mustEncode = true;
                            break;
                    }

                    if (i <= 0x001F || (0x007F <= i && i <= 0x9F))
                    {
                        mustEncode = true; // control char
                    }
                    else if (!UnicodeHelpers.IsCharacterDefined((char)i))
                    {
                        mustEncode = true; // undefined (or otherwise disallowed) char
                    }

                    if (mustEncode)
                    {
                        expected = string.Format(CultureInfo.InvariantCulture, "[U+{0:X4}]", i);
                    }
                    else
                    {
                        expected = input; // no encoding
                    }
                }

                string retVal = encoder.Encode(input);
                Assert.Equal(expected, retVal);
            }

            // Act & assert - astral chars
            for (int i = 0x10000; i <= 0x10FFFF; i++)
            {
                string input = char.ConvertFromUtf32(i);
                string expected = string.Format(CultureInfo.InvariantCulture, "[U+{0:X}]", i);
                string retVal = encoder.Encode(input);
                Assert.Equal(expected, retVal);
            }
        }

        [Fact]
        public void Encode_BadSurrogates_ReturnsUnicodeReplacementChar()
        {
            // Arrange
            CustomTextEncoder encoder = new CustomTextEncoder(UnicodeRanges.All); // allow all codepoints

            // "a<unpaired leading>b<unpaired trailing>c<trailing before leading>d<unpaired trailing><valid>e<high at end of string>"
            const string input = "a\uD800b\uDFFFc\uDFFF\uD800d\uDFFF\uD800\uDFFFe\uD800";
            const string expected = "a\uFFFDb\uFFFDc\uFFFD\uFFFDd\uFFFD[U+103FF]e\uFFFD";

            // Act
            string retVal = encoder.Encode(input);

            // Assert
            Assert.Equal(expected, retVal);
        }

        [Fact]
        public void Encode_EmptyStringInput_ReturnsEmptyString()
        {
            // Arrange
            CustomTextEncoder encoder = new CustomTextEncoder(UnicodeRanges.All);

            // Act & assert
            Assert.Equal("", encoder.Encode(""));
        }

        [Fact]
        public void Encode_InputDoesNotRequireEncoding_ReturnsOriginalStringInstance()
        {
            // Arrange
            CustomTextEncoder encoder = new CustomTextEncoder(UnicodeRanges.All);
            string input = "Hello, there!";

            // Act & assert
            Assert.Same(input, encoder.Encode(input));
        }

        [Fact]
        public void Encode_WithCharsRequiringEncodingAtBeginning()
        {
            Assert.Equal("[U+0026]Hello, there!", new CustomTextEncoder(UnicodeRanges.All).Encode("&Hello, there!"));
        }

        [Fact]
        public void Encode_WithCharsRequiringEncodingAtEnd()
        {
            Assert.Equal("Hello, there![U+0026]", new CustomTextEncoder(UnicodeRanges.All).Encode("Hello, there!&"));
        }

        [Fact]
        public void Encode_WithCharsRequiringEncodingInMiddle()
        {
            Assert.Equal("Hello, [U+0026]there!", new CustomTextEncoder(UnicodeRanges.All).Encode("Hello, &there!"));
        }

        [Fact]
        public void Encode_WithCharsRequiringEncodingInterspersed()
        {
            Assert.Equal("Hello, [U+003C]there[U+003E]!", new CustomTextEncoder(UnicodeRanges.All).Encode("Hello, <there>!"));
        }

        [Fact]
        public void Encode_CharArray_ParameterChecking_NegativeTestCases()
        {
            // Arrange
            CustomTextEncoder encoder = new CustomTextEncoder();
            StringWriter writer = new StringWriter();

            // Act & assert
            Assert.Throws<ArgumentNullException>(() => encoder.Encode(writer, (char[])null, 0, 0));
            Assert.Throws<ArgumentNullException>(() => encoder.Encode(null, "abc".ToCharArray(), 0, 3));
            Assert.Throws<ArgumentOutOfRangeException>(() => encoder.Encode(writer, "abc".ToCharArray(), -1, 2));
            Assert.Throws<ArgumentOutOfRangeException>(() => encoder.Encode(writer, "abc".ToCharArray(), 2, 2));
            Assert.Throws<ArgumentOutOfRangeException>(() => encoder.Encode(writer, "abc".ToCharArray(), 4, 0));
            Assert.Throws<ArgumentOutOfRangeException>(() => encoder.Encode(writer, "abc".ToCharArray(), 2, -1));
            Assert.Throws<ArgumentOutOfRangeException>(() => encoder.Encode(writer, "abc".ToCharArray(), 1, 3));
        }

        //[Fact]
        //public void Encode_CharArray_ZeroCount_DoesNotCallIntoTextWriter()
        //{
        //    // Arrange
        //    CustomUnicodeEncoderBase encoder = new CustomUnicodeEncoderBase();
        //    TextWriter output = new Mock<TextWriter>(MockBehavior.Strict).Object;

        //    // Act
        //    encoder.Encode("abc".ToCharArray(), 2, 0, output);

        //    // Assert
        //    // If we got this far (without TextWriter throwing), success!
        //}

        [Fact]
        public void Encode_CharArray_AllCharsValid()
        {
            // Arrange
            CustomTextEncoder encoder = new CustomTextEncoder(UnicodeRanges.All);
            StringWriter output = new StringWriter();

            // Act
            encoder.Encode(output, "abc&xyz".ToCharArray(), 4, 2);

            // Assert
            Assert.Equal("xy", output.ToString());
        }

        [Fact]
        public void Encode_CharArray_AllCharsInvalid()
        {
            // Arrange
            CustomTextEncoder encoder = new CustomTextEncoder();
            StringWriter output = new StringWriter();

            // Act
            encoder.Encode(output, "abc&xyz".ToCharArray(), 4, 2);

            // Assert
            Assert.Equal("[U+0078][U+0079]", output.ToString());
        }

        [Fact]
        public void Encode_CharArray_SomeCharsValid()
        {
            // Arrange
            CustomTextEncoder encoder = new CustomTextEncoder(UnicodeRanges.All);
            StringWriter output = new StringWriter();

            // Act
            encoder.Encode(output, "abc&xyz".ToCharArray(), 2, 3);

            // Assert
            Assert.Equal("c[U+0026]x", output.ToString());
        }

        [Fact]
        public void Encode_StringSubstring_ParameterChecking_NegativeTestCases()
        {
            // Arrange
            CustomTextEncoder encoder = new CustomTextEncoder();
            StringWriter writer = new StringWriter();

            // Act & assert
            Assert.Throws<ArgumentNullException>(() => encoder.Encode(writer, (string)null, 0, 0));
            Assert.Throws<ArgumentNullException>(() => encoder.Encode(null, "abc", 0, 3));
            Assert.Throws<ArgumentOutOfRangeException>(() => encoder.Encode(writer, "abc", -1, 2));
            Assert.Throws<ArgumentOutOfRangeException>(() => encoder.Encode(writer, "abc", 2, 2));
            Assert.Throws<ArgumentOutOfRangeException>(() => encoder.Encode(writer, "abc", 4, 0));
            Assert.Throws<ArgumentOutOfRangeException>(() => encoder.Encode(writer, "abc", 2, -1));
            Assert.Throws<ArgumentOutOfRangeException>(() => encoder.Encode(writer, "abc", 1, 3));
        }

        [Fact]
        public void Encode_StringSubstring_AllCharsValid()
        {
            // Arrange
            CustomTextEncoder encoder = new CustomTextEncoder(UnicodeRanges.All);
            StringWriter output = new StringWriter();

            // Act
            encoder.Encode(output, "abc&xyz", 4, 2);

            // Assert
            Assert.Equal("xy", output.ToString());
        }

        [Fact]
        public void Encode_StringSubstring_AllCharsInvalid()
        {
            // Arrange
            CustomTextEncoder encoder = new CustomTextEncoder();
            StringWriter output = new StringWriter();

            // Act
            encoder.Encode(output, "abc&xyz", 4, 2);

            // Assert
            Assert.Equal("[U+0078][U+0079]", output.ToString());
        }

        [Fact]
        public void Encode_StringSubstring_SomeCharsValid()
        {
            // Arrange
            CustomTextEncoder encoder = new CustomTextEncoder(UnicodeRanges.All);
            StringWriter output = new StringWriter();

            // Act
            encoder.Encode(output, "abc&xyz", 2, 3);

            // Assert
            Assert.Equal("c[U+0026]x", output.ToString());
        }

        [Fact]
        public void Encode_StringSubstring_EntireString_SomeCharsValid()
        {
            // Arrange
            CustomTextEncoder encoder = new CustomTextEncoder(UnicodeRanges.All);
            StringWriter output = new StringWriter();

            // Act
            const string input = "abc&xyz";
            encoder.Encode(output, input, 0, input.Length);

            // Assert
            Assert.Equal("abc[U+0026]xyz", output.ToString());
        }

        private static bool IsSurrogateCodePoint(int codePoint)
        {
            return (0xD800 <= codePoint && codePoint <= 0xDFFF);
        }

        private sealed class CustomTextEncoderSettings : TextEncoderSettings
        {
            private readonly int[] _allowedCodePoints;

            public CustomTextEncoderSettings(params int[] allowedCodePoints)
            {
                _allowedCodePoints = allowedCodePoints;
            }

            public override IEnumerable<int> GetAllowedCodePoints()
            {
                return _allowedCodePoints;
            }
        }
    }
}
