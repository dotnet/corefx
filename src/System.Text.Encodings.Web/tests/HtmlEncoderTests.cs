// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Globalization;
using System.IO;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using Xunit;

namespace Microsoft.Framework.WebEncoders
{
    public class HtmlEncoderTests
    {
        [Fact]
        public void TestSurrogate()
        {
            Assert.Equal("&#x1F4A9;", System.Text.Encodings.Web.HtmlEncoder.Default.Encode("\U0001f4a9"));
            
            using (var writer = new StringWriter())
            {
                System.Text.Encodings.Web.HtmlEncoder.Default.Encode(writer, "\U0001f4a9");
                Assert.Equal("&#x1F4A9;", writer.GetStringBuilder().ToString());
            }
        }
        
        [Fact]
        public void Ctor_WithTextEncoderSettings()
        {
            // Arrange
            var filter = new TextEncoderSettings();
            filter.AllowCharacters('a', 'b');
            filter.AllowCharacters('\0', '&', '\uFFFF', 'd');
            HtmlEncoder encoder = HtmlEncoder.Create(filter);

            // Act & assert
            Assert.Equal("a", encoder.Encode("a"));
            Assert.Equal("b", encoder.Encode("b"));
            Assert.Equal("&#x63;", encoder.Encode("c"));
            Assert.Equal("d", encoder.Encode("d"));
            Assert.Equal("&#x0;", encoder.Encode("\0")); // we still always encode control chars
            Assert.Equal("&amp;", encoder.Encode("&")); // we still always encode HTML-special chars
            Assert.Equal("&#xFFFF;", encoder.Encode("\uFFFF")); // we still always encode non-chars and other forbidden chars
        }

        [Fact]
        public void Ctor_WithUnicodeRanges()
        {
            // Arrange
            HtmlEncoder encoder = HtmlEncoder.Create(UnicodeRanges.Latin1Supplement, UnicodeRanges.MiscellaneousSymbols);

            // Act & assert
            Assert.Equal("&#x61;", encoder.Encode("a"));
            Assert.Equal("\u00E9", encoder.Encode("\u00E9" /* LATIN SMALL LETTER E WITH ACUTE */));
            Assert.Equal("\u2601", encoder.Encode("\u2601" /* CLOUD */));
        }

        [Fact]
        public void Default_EquivalentToBasicLatin_Implicit()
        {
            // Arrange
            HtmlEncoder encoder = HtmlEncoder.Default;

            // Act & assert
            Assert.Equal("a", encoder.Encode("a"));
            Assert.Equal("&#xE9;", encoder.Encode("\u00E9" /* LATIN SMALL LETTER E WITH ACUTE */));
            Assert.Equal("&#x2601;", encoder.Encode("\u2601" /* CLOUD */));
        }

        [Fact]
        public void Default_EquivalentToBasicLatin_Explicit()
        {
            // Arrange
            HtmlEncoder controlEncoder = HtmlEncoder.Create(UnicodeRanges.BasicLatin);
            HtmlEncoder testEncoder = HtmlEncoder.Default;

            // Act & assert
            for (int i = 0; i <= char.MaxValue; i++)
            {
                if (!IsSurrogateCodePoint(i))
                {
                    string input = new string((char)i, 1);
                    Assert.Equal(controlEncoder.Encode(input), testEncoder.Encode(input));
                }
            }
        }

        [Theory]
        [InlineData("<", "&lt;")]
        [InlineData(">", "&gt;")]
        [InlineData("&", "&amp;")]
        [InlineData("'", "&#x27;")]
        [InlineData("\"", "&quot;")]
        [InlineData("+", "&#x2B;")]
        public void HtmlEncode_AllRangesAllowed_StillEncodesForbiddenChars_Simple(string input, string expected)
        {
            // Arrange
            HtmlEncoder encoder = HtmlEncoder.Create(UnicodeRanges.All);

            // Act
            string retVal = encoder.Encode(input);

            // Assert
            Assert.Equal(expected, retVal);
        }

        [Fact]
        public void HtmlEncode_AllRangesAllowed_StillEncodesForbiddenChars_Extended()
        {
            // Arrange
            HtmlEncoder encoder = HtmlEncoder.Create(UnicodeRanges.All);

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
                    if (input == "<") { expected = "&lt;"; }
                    else if (input == ">") { expected = "&gt;"; }
                    else if (input == "&") { expected = "&amp;"; }
                    else if (input == "\"") { expected = "&quot;"; }
                    else
                    {
                        bool mustEncode = false;
                        if (i == '\'' || i == '+')
                        {
                            mustEncode = true; // apostrophe, plus
                        }
                        else if (i <= 0x001F || (0x007F <= i && i <= 0x9F))
                        {
                            mustEncode = true; // control char
                        }
                        else if (!UnicodeHelpers.IsCharacterDefined((char)i))
                        {
                            mustEncode = true; // undefined (or otherwise disallowed) char
                        }

                        if (mustEncode)
                        {
                            expected = string.Format(CultureInfo.InvariantCulture, "&#x{0:X};", i);
                        }
                        else
                        {
                            expected = input; // no encoding
                        }
                    }
                }

                string retVal = encoder.Encode(input);
                Assert.Equal(expected, retVal);
            }

            // Act & assert - astral chars
            for (int i = 0x10000; i <= 0x10FFFF; i++)
            {
                string input = char.ConvertFromUtf32(i);
                string expected = string.Format(CultureInfo.InvariantCulture, "&#x{0:X};", i);
                string retVal = encoder.Encode(input);
                Assert.Equal(expected, retVal);
            }
        }

        [Fact]
        public void HtmlEncode_BadSurrogates_ReturnsUnicodeReplacementChar()
        {
            // Arrange
            HtmlEncoder encoder = HtmlEncoder.Create(UnicodeRanges.All); // allow all codepoints

            // "a<unpaired leading>b<unpaired trailing>c<trailing before leading>d<unpaired trailing><valid>e<high at end of string>"
            const string input = "a\uD800b\uDFFFc\uDFFF\uD800d\uDFFF\uD800\uDFFFe\uD800";
            const string expected = "a\uFFFDb\uFFFDc\uFFFD\uFFFDd\uFFFD&#x103FF;e\uFFFD";

            // Act
            string retVal = encoder.Encode(input);

            // Assert
            Assert.Equal(expected, retVal);
        }

        [Fact]
        public void HtmlEncode_EmptyStringInput_ReturnsEmptyString()
        {
            // Arrange
            HtmlEncoder encoder = HtmlEncoder.Default;

            // Act & assert
            Assert.Equal("", encoder.Encode(""));
        }

        [Fact]
        public void HtmlEncode_InputDoesNotRequireEncoding_ReturnsOriginalStringInstance()
        {
            // Arrange
            HtmlEncoder encoder = HtmlEncoder.Default;
            string input = "Hello, there!";

            // Act & assert
            Assert.Same(input, encoder.Encode(input));
        }

        [Fact]
        public void HtmlEncode_NullInput_Throws()
        {
            // Arrange
            HtmlEncoder encoder = HtmlEncoder.Default;
            Assert.Throws<ArgumentNullException>(() => { encoder.Encode(null); });
        }

        [Fact]
        public void HtmlEncode_WithCharsRequiringEncodingAtBeginning()
        {
            Assert.Equal("&amp;Hello, there!", HtmlEncoder.Default.Encode("&Hello, there!"));
        }

        [Fact]
        public void HtmlEncode_WithCharsRequiringEncodingAtEnd()
        {
            Assert.Equal("Hello, there!&amp;", HtmlEncoder.Default.Encode("Hello, there!&"));
        }

        [Fact]
        public void HtmlEncode_WithCharsRequiringEncodingInMiddle()
        {
            Assert.Equal("Hello, &amp;there!", HtmlEncoder.Default.Encode("Hello, &there!"));
        }

        [Fact]
        public void HtmlEncode_WithCharsRequiringEncodingInterspersed()
        {
            Assert.Equal("Hello, &lt;there&gt;!", HtmlEncoder.Default.Encode("Hello, <there>!"));
        }

        [Fact]
        public void HtmlEncode_CharArray()
        {
            // Arrange
            HtmlEncoder encoder = HtmlEncoder.Default;
            var output = new StringWriter();

            // Act
            encoder.Encode(output, "Hello+world!".ToCharArray(), 3, 5);

            // Assert
            Assert.Equal("lo&#x2B;wo", output.ToString());
        }

        [Fact]
        public void HtmlEncode_StringSubstring()
        {
            // Arrange
            HtmlEncoder encoder = HtmlEncoder.Default;
            var output = new StringWriter();

            // Act
            encoder.Encode(output, "Hello+world!", 3, 5);

            // Assert
            Assert.Equal("lo&#x2B;wo", output.ToString());
        }

        [Fact]
        public void HtmlEncode_AstralWithTextWriter()
        {
            byte[] buffer = new byte[10];
            MemoryStream ms = new MemoryStream(buffer);
            using (StreamWriter sw = new StreamWriter(ms))
            {
                string input = "\U0010FFFF";
                System.Text.Encodings.Web.HtmlEncoder.Default.Encode(sw, input);
            }
            Assert.Equal("&#x10FFFF;", System.Text.Encoding.UTF8.GetString(buffer));
        }

        private static bool IsSurrogateCodePoint(int codePoint)
        {
            return (0xD800 <= codePoint && codePoint <= 0xDFFF);
        }
    }
}
