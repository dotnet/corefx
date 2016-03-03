// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Globalization;
using System.IO;
using System.Text.Unicode;
using Xunit;

namespace System.Text.Encodings.Web.Tests
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
            var filter = new TextEncoderSettings();
            filter.AllowCharacters('a', 'b');
            filter.AllowCharacters('\0', '&', '\uFFFF', 'd');
            HtmlEncoder encoder = new HtmlEncoder(filter);
            
            Assert.Equal("a", encoder.HtmlEncode("a"));
            Assert.Equal("b", encoder.HtmlEncode("b"));
            Assert.Equal("&#x63;", encoder.HtmlEncode("c"));
            Assert.Equal("d", encoder.HtmlEncode("d"));
            Assert.Equal("&#x0;", encoder.HtmlEncode("\0")); // we still always encode control chars
            Assert.Equal("&amp;", encoder.HtmlEncode("&")); // we still always encode HTML-special chars
            Assert.Equal("&#xFFFF;", encoder.HtmlEncode("\uFFFF")); // we still always encode non-chars and other forbidden chars
        }

        [Fact]
        public void Ctor_WithUnicodeRanges()
        {
            HtmlEncoder encoder = new HtmlEncoder(UnicodeRanges.Latin1Supplement, UnicodeRanges.MiscellaneousSymbols);

            Assert.Equal("&#x61;", encoder.HtmlEncode("a"));
            Assert.Equal("\u00E9", encoder.HtmlEncode("\u00E9" /* LATIN SMALL LETTER E WITH ACUTE */));
            Assert.Equal("\u2601", encoder.HtmlEncode("\u2601" /* CLOUD */));
        }

        [Fact]
        public void Ctor_WithNoParameters_DefaultsToBasicLatin()
        {
            HtmlEncoder encoder = new HtmlEncoder();

            Assert.Equal("a", encoder.HtmlEncode("a"));
            Assert.Equal("&#xE9;", encoder.HtmlEncode("\u00E9" /* LATIN SMALL LETTER E WITH ACUTE */));
            Assert.Equal("&#x2601;", encoder.HtmlEncode("\u2601" /* CLOUD */));
        }

        [Fact]
        public void Default_EquivalentToBasicLatin()
        {
            HtmlEncoder controlEncoder = new HtmlEncoder(UnicodeRanges.BasicLatin);
            HtmlEncoder testEncoder = HtmlEncoder.Default;

            for (int i = 0; i <= Char.MaxValue; i++)
            {
                if (!IsSurrogateCodePoint(i))
                {
                    string input = new String((char)i, 1);
                    Assert.Equal(controlEncoder.HtmlEncode(input), testEncoder.HtmlEncode(input));
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
            HtmlEncoder encoder = new HtmlEncoder(UnicodeRanges.All);

            Assert.Equal(input, encoder.HtmlEncode(input));
        }

        [Fact]
        public void HtmlEncode_AllRangesAllowed_StillEncodesForbiddenChars_Extended()
        {
            HtmlEncoder encoder = new HtmlEncoder(UnicodeRanges.All);

            // BMP chars
            for (int i = 0; i <= char.MaxValue; i++)
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
                            expected = String.Format(CultureInfo.InvariantCulture, "&#x{0:X};", i);
                        }
                        else
                        {
                            expected = input; // no encoding
                        }
                    }
                }
                
                Assert.Equal(expected, encoder.HtmlEncode(input));
            }

            // Astral chars
            for (int i = 0x10000; i <= 0x10FFFF; i++)
            {
                string input = Char.ConvertFromUtf32(i);
                string expected = String.Format(CultureInfo.InvariantCulture, "&#x{0:X};", i);
                string retVal = encoder.HtmlEncode(input);
                Assert.Equal(expected, retVal);
            }
        }

        [Fact]
        public void HtmlEncode_BadSurrogates_ReturnsUnicodeReplacementChar()
        {
            HtmlEncoder encoder = new HtmlEncoder(UnicodeRanges.All); // allow all codepoints

            // "a<unpaired leading>b<unpaired trailing>c<trailing before leading>d<unpaired trailing><valid>e<high at end of string>"
            const string input = "a\uD800b\uDFFFc\uDFFF\uD800d\uDFFF\uD800\uDFFFe\uD800";
            const string expected = "a\uFFFDb\uFFFDc\uFFFD\uFFFDd\uFFFD&#x103FF;e\uFFFD";
            
            Assert.Equal(expected, encoder.HtmlEncode(input));
        }

        [Fact]
        public void HtmlEncode_EmptyStringInput_ReturnsEmptyString()
        {
            HtmlEncoder encoder = new HtmlEncoder();
            Assert.Equal("", encoder.HtmlEncode(""));
        }

        [Fact]
        public void HtmlEncode_InputDoesNotRequireEncoding_ReturnsOriginalStringInstance()
        {
            HtmlEncoder encoder = new HtmlEncoder();
            string input = "Hello, there!";
            Assert.Same(input, encoder.HtmlEncode(input));
        }

        [Fact]
        public void HtmlEncode_NullInput_Throws()
        {
            HtmlEncoder encoder = new HtmlEncoder();
            Assert.Throws<ArgumentNullException>(() => encoder.HtmlEncode(null));
        }

        [Fact]
        public void HtmlEncode_WithCharsRequiringEncodingAtBeginning()
        {
            Assert.Equal("&amp;Hello, there!", new HtmlEncoder().HtmlEncode("&Hello, there!"));
        }

        [Fact]
        public void HtmlEncode_WithCharsRequiringEncodingAtEnd()
        {
            Assert.Equal("Hello, there!&amp;", new HtmlEncoder().HtmlEncode("Hello, there!&"));
        }

        [Fact]
        public void HtmlEncode_WithCharsRequiringEncodingInMiddle()
        {
            Assert.Equal("Hello, &amp;there!", new HtmlEncoder().HtmlEncode("Hello, &there!"));
        }

        [Fact]
        public void HtmlEncode_WithCharsRequiringEncodingInterspersed()
        {
            Assert.Equal("Hello, &lt;there&gt;!", new HtmlEncoder().HtmlEncode("Hello, <there>!"));
        }

        [Fact]
        public void HtmlEncode_CharArray()
        {
            HtmlEncoder encoder = new HtmlEncoder();
            var output = new StringWriter();
            
            encoder.HtmlEncode("Hello+world!".ToCharArray(), 3, 5, output);
            Assert.Equal("lo&#x2B;wo", output.ToString());
        }

        [Fact]
        public void HtmlEncode_StringSubstring()
        {
            HtmlEncoder encoder = new HtmlEncoder();
            var output = new StringWriter();

            encoder.HtmlEncode("Hello+world!", 3, 5, output);
            Assert.Equal("lo&#x2B;wo", output.ToString());
        }

        private static bool IsSurrogateCodePoint(int codePoint)
        {
            return (0xD800 <= codePoint && codePoint <= 0xDFFF);
        }
    }
}
