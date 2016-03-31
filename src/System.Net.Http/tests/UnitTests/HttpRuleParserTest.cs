// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Text;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

using Xunit;

namespace System.Net.Http.Tests
{
    public class HttpRuleParserTest
    {
        private const string ValidTokenChars = "!#$%&'*+-.0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz^_`|~";

        [Fact]
        public void IsTokenChar_IterateArrayWithAllValidTokenChars_AllCharsConsideredValid()
        {
            for (int i = 0; i < ValidTokenChars.Length; i++)
            {
                // TODO: This test should be a [Theory]
                Assert.True(HttpRuleParser.IsTokenChar(ValidTokenChars[i]));
            }
        }

        [Fact]
        public void IsTokenChar_IterateArrayWithAllInvalidTokenChars_AllCharsConsideredInvalid()
        {
            // All octets not in 'validTokenChars' must be considered invalid characters.
            for (int i = 0; i < 256; i++)
            {
                if (ValidTokenChars.IndexOf((char)i) == -1)
                {
                    // TODO: This test should be a [Theory]
                    Assert.False(HttpRuleParser.IsTokenChar((char)i));
                }
            }
        }

        [Fact]
        public void GetTokenLength_SetOfValidTokens_AllConsideredValid()
        {
            AssertGetTokenLength("token", 0, 5);
            AssertGetTokenLength(" token ", 1, 5);
            AssertGetTokenLength(" token token", 1, 5);
            AssertGetTokenLength("x, y", 0, 1);
            AssertGetTokenLength("x,y", 0, 1);
            AssertGetTokenLength(":x:y", 1, 1);
            AssertGetTokenLength("(comment)token(comment)", 9, 5);
        }

        [Fact]
        public void GetTokenLength_SetOfInvalidTokens_TokenLengthIsZero()
        {
            AssertGetTokenLength(" token", 0, 0);
            AssertGetTokenLength("\token", 0, 0);
            AssertGetTokenLength("token token ", 5, 0);
            AssertGetTokenLength(" ", 0, 0);
        }

        [Fact]
        public void GetHostLength_SetOfValidHostStrings_MatchExpectation()
        {
            // Allow token or URI host:
            AssertGetHostLength("", 0, 0, true, null);
            AssertGetHostLength("  ", 2, 0, true, null);
            AssertGetHostLength("host", 0, 4, true, "host");
            AssertGetHostLength("host:80", 0, 7, true, "host:80");
            AssertGetHostLength("host:80 ", 0, 7, true, "host:80");
            AssertGetHostLength("host:80,nexthost", 0, 7, true, "host:80");
            AssertGetHostLength("host.com:80,nexthost", 0, 11, true, "host.com:80");
            AssertGetHostLength(".token ,nexthost", 0, 6, true, ".token");
            AssertGetHostLength(".token nexthost", 0, 6, true, ".token");
            AssertGetHostLength(".token", 0, 6, true, ".token");
            AssertGetHostLength("[::1]:80", 0, 8, true, "[::1]:80");
            AssertGetHostLength("[::1],host", 0, 5, true, "[::1]");
            AssertGetHostLength("192.168.0.1,", 0, 11, true, "192.168.0.1");
            AssertGetHostLength("192.168.0.1:8080 ", 0, 16, true, "192.168.0.1:8080");

            // Allow URI host only (no token):
            AssertGetHostLength("", 0, 0, false, null);
            AssertGetHostLength("  ", 2, 0, false, null);
            AssertGetHostLength("host", 0, 4, false, "host");
            AssertGetHostLength("host:80", 0, 7, false, "host:80");
            AssertGetHostLength("host:80 ", 0, 7, false, "host:80");
            AssertGetHostLength("host:80,nexthost", 0, 7, false, "host:80");
            AssertGetHostLength("host.com:80,nexthost", 0, 11, false, "host.com:80");
            AssertGetHostLength("[::1]:80", 0, 8, false, "[::1]:80");
            AssertGetHostLength("[::1],host", 0, 5, false, "[::1]");
            AssertGetHostLength("192.168.0.1,", 0, 11, false, "192.168.0.1");
            AssertGetHostLength("192.168.0.1:8080 ", 0, 16, false, "192.168.0.1:8080");
        }

        [Fact]
        public void GetHostLength_SetOfInvalidHostStrings_MatchExpectation()
        {
            // Allow token or URI host:
            AssertGetHostLength("host:80invalid", 0, 0, true, null);
            AssertGetHostLength("host:80:nexthost", 0, 0, true, null);
            AssertGetHostLength("  ", 0, 0, true, null);
            AssertGetHostLength("token@:80", 0, 0, true, null);
            AssertGetHostLength("token@host:80", 0, 0, true, null);
            AssertGetHostLength("token@host", 0, 0, true, null);
            AssertGetHostLength("token@", 0, 0, true, null);
            AssertGetHostLength("token<", 0, 0, true, null);
            AssertGetHostLength("192.168.0.1:8080!", 0, 0, true, null);
            AssertGetHostLength(".token/", 0, 0, true, null);
            AssertGetHostLength("host:80/", 0, 0, true, null);
            AssertGetHostLength("host:80/path", 0, 0, true, null);
            AssertGetHostLength("@host:80", 0, 0, true, null);
            AssertGetHostLength("u:p@host:80", 0, 0, true, null);

            // Allow URI host only (no token):
            AssertGetHostLength("host:80invalid", 0, 0, false, null);
            AssertGetHostLength("host:80:nexthost", 0, 0, false, null);
            AssertGetHostLength("  ", 0, 0, false, null);
            AssertGetHostLength("token@:80", 0, 0, false, null);
            AssertGetHostLength("token@host:80", 0, 0, false, null);
            AssertGetHostLength("token@host", 0, 0, false, null);
            AssertGetHostLength("token@", 0, 0, false, null);
            AssertGetHostLength("token<", 0, 0, false, null);
            AssertGetHostLength("192.168.0.1:8080!", 0, 0, false, null);
            AssertGetHostLength(".token/", 0, 0, false, null);
            AssertGetHostLength("host:80/", 0, 0, false, null);
            AssertGetHostLength("host:80/path", 0, 0, false, null);
            AssertGetHostLength("@host:80", 0, 0, false, null);
            AssertGetHostLength("u:p@host:80", 0, 0, false, null);
            AssertGetHostLength(".token", 0, 0, false, null);
            AssertGetHostLength("to~ken", 0, 0, false, null);
        }

        [Fact]
        public void GetQuotedPairLength_SetOfValidQuotedPairs_AllConsideredValid()
        {
            AssertGetQuotedPairLength("\\x", 0, 2, HttpParseResult.Parsed);
            AssertGetQuotedPairLength(" \\x ", 1, 2, HttpParseResult.Parsed);
            AssertGetQuotedPairLength("\\x ", 0, 2, HttpParseResult.Parsed);
            AssertGetQuotedPairLength("\\\t", 0, 2, HttpParseResult.Parsed);
        }

        [Fact]
        public void GetQuotedPairLength_SetOfInvalidQuotedPairs_AllConsideredInvalid()
        {
            // only ASCII chars allowed in quoted-pair
            AssertGetQuotedPairLength("\\ü", 0, 0, HttpParseResult.InvalidFormat);

            // a quoted-pair needs 1 char after '\'
            AssertGetQuotedPairLength("\\", 0, 0, HttpParseResult.InvalidFormat);
        }

        [Fact]
        public void GetQuotedPairLength_SetOfNonQuotedPairs_NothingParsed()
        {
            AssertGetQuotedPairLength("token\\x", 0, 0, HttpParseResult.NotParsed);
        }

        [Fact]
        public void GetQuotedStringLength_SetOfValidQuotedStrings_AllConsideredValid()
        {
            AssertGetQuotedStringLength("\"x\"", 0, 3, HttpParseResult.Parsed);
            AssertGetQuotedStringLength("token \"quoted string\" token", 6, 15, HttpParseResult.Parsed);
            AssertGetQuotedStringLength("\"\\x\"", 0, 4, HttpParseResult.Parsed); // "\x"
            AssertGetQuotedStringLength("\"\\\"\"", 0, 4, HttpParseResult.Parsed); // "\""
            AssertGetQuotedStringLength("\"before \\\" after\"", 0, 17, HttpParseResult.Parsed); // "before \" after"
            AssertGetQuotedStringLength("\"\\ü\"", 0, 4, HttpParseResult.Parsed); // "\ü"
            AssertGetQuotedStringLength("\"a\\ü\\\"b\"", 0, 8, HttpParseResult.Parsed); // "a\ü\"b"
            AssertGetQuotedStringLength("\"\\\"", 0, 3, HttpParseResult.Parsed); // "\"
            AssertGetQuotedStringLength("\"\\\"\"", 0, 4, HttpParseResult.Parsed); // "\""
            AssertGetQuotedStringLength(" \"\\\"", 1, 3, HttpParseResult.Parsed); // "\"
            AssertGetQuotedStringLength(" \"\\\"\"", 1, 4, HttpParseResult.Parsed); // "\""
            AssertGetQuotedStringLength("\"a \\\" b\"", 0, 8, HttpParseResult.Parsed); // "a \" b"
            AssertGetQuotedStringLength("\"s\\x\"", 0, 5, HttpParseResult.Parsed); // "s\x"
            AssertGetQuotedStringLength("\"\\xx\"", 0, 5, HttpParseResult.Parsed); // "\xx"
            AssertGetQuotedStringLength("\"(x)\"", 0, 5, HttpParseResult.Parsed); // "(x)"
            AssertGetQuotedStringLength(" \" (x) \" ", 1, 7, HttpParseResult.Parsed); // " (x) "
            AssertGetQuotedStringLength("\"text\r\n new line\"", 0, 17, HttpParseResult.Parsed); // "text<crlf> new line"
            AssertGetQuotedStringLength("\"a\\ü\\\"b\\\"c\\\"\\\"d\\\"\"", 0, 18, HttpParseResult.Parsed); // "a\ü\"b\"c\"\"d\""
            AssertGetQuotedStringLength("\"\\\" \"", 0, 5, HttpParseResult.Parsed); // "\" "
        }

        [Fact]
        public void GetQuotedStringLength_SetOfInvalidQuotedStrings_AllConsideredInvalid()
        {
            AssertGetQuotedStringLength("\"x", 0, 0, HttpParseResult.InvalidFormat); // "x
            AssertGetQuotedStringLength(" \"x ", 1, 0, HttpParseResult.InvalidFormat); // ' "x '
        }

        [Fact]
        public void GetQuotedStringLength_SetOfNonQuotedStrings_NothingParsed()
        {
            AssertGetQuotedStringLength("a\"x", 0, 0, HttpParseResult.NotParsed); // a"x"
            AssertGetQuotedStringLength("(\"x", 0, 0, HttpParseResult.NotParsed); // ("x"
            AssertGetQuotedStringLength("\\\"x", 0, 0, HttpParseResult.NotParsed); // \"x"
        }

        [Fact]
        public void GetCommentLength_SetOfValidComments_AllConsideredValid()
        {
            AssertGetCommentLength("()", 0, 2, HttpParseResult.Parsed);
            AssertGetCommentLength("(x)", 0, 3, HttpParseResult.Parsed);
            AssertGetCommentLength("token (comment) token", 6, 9, HttpParseResult.Parsed);
            AssertGetCommentLength("(\\x)", 0, 4, HttpParseResult.Parsed); // (\x)
            AssertGetCommentLength("(\\))", 0, 4, HttpParseResult.Parsed); // (\))
            AssertGetCommentLength("(\\()", 0, 4, HttpParseResult.Parsed); // (\()
            AssertGetCommentLength("(\\ü)", 0, 4, HttpParseResult.Parsed); // (\ü)
            AssertGetCommentLength("(\\)", 0, 3, HttpParseResult.Parsed); // (\)
            AssertGetCommentLength("(s\\x)", 0, 5, HttpParseResult.Parsed); // (s\x)
            AssertGetCommentLength("(\\xx)", 0, 5, HttpParseResult.Parsed); // (\xx)
            AssertGetCommentLength("(\"x\")", 0, 5, HttpParseResult.Parsed); // ("x")
            AssertGetCommentLength(" ( \"x\" ) ", 1, 7, HttpParseResult.Parsed); // ( "x" )
            AssertGetCommentLength("(text\r\n new line)", 0, 17, HttpParseResult.Parsed); // (text<crlf> new line)
            AssertGetCommentLength("(\\) )", 0, 5, HttpParseResult.Parsed); // (\))
            AssertGetCommentLength("(\\( )", 0, 5, HttpParseResult.Parsed); // (\()

            // Nested comments
            AssertGetCommentLength("((x))", 0, 5, HttpParseResult.Parsed);
            AssertGetCommentLength("( (x) )", 0, 7, HttpParseResult.Parsed);
            AssertGetCommentLength("( (\\(x) )", 0, 9, HttpParseResult.Parsed);
            AssertGetCommentLength("( (\\)x) )", 0, 9, HttpParseResult.Parsed);
            AssertGetCommentLength("(\\) (\\(x) )", 0, 11, HttpParseResult.Parsed);
            AssertGetCommentLength("((((((x))))))", 0, 13, HttpParseResult.Parsed);
            AssertGetCommentLength("((x) (x) ((x)x) ((((x)x)x)x(x(x))))", 0, 35, HttpParseResult.Parsed);
            AssertGetCommentLength("((x) (\\(x\\())", 0, 13, HttpParseResult.Parsed); // ((x) (\(x\()))
            AssertGetCommentLength("((\\)))", 0, 6, HttpParseResult.Parsed); // ((\))) -> quoted-pair )
            AssertGetCommentLength("((\\())", 0, 6, HttpParseResult.Parsed); // ((\()) -> quoted-pair (
            AssertGetCommentLength("((x)))", 0, 5, HttpParseResult.Parsed); // final ) ignored
        }

        [Fact]
        public void GetCommentLength_SetOfInvalidQuotedStrings_AllConsideredInvalid()
        {
            AssertGetCommentLength("(x", 0, 0, HttpParseResult.InvalidFormat);
            AssertGetCommentLength(" (x ", 1, 0, HttpParseResult.InvalidFormat);
            AssertGetCommentLength("((x) ", 0, 0, HttpParseResult.InvalidFormat);
            AssertGetCommentLength("((x ", 0, 0, HttpParseResult.InvalidFormat);
            AssertGetCommentLength("(x(x ", 0, 0, HttpParseResult.InvalidFormat);
            AssertGetCommentLength("(x(((((((((x ", 0, 0, HttpParseResult.InvalidFormat);

            // To prevent attacker from sending comments resulting in stack overflow exceptions, we limit the depth
            // of nested comments. I.e. the following comment is considered invalid since it is considered a 
            // "malicious" comment.
            AssertGetCommentLength("((((((((((x))))))))))", 0, 0, HttpParseResult.InvalidFormat);
        }

        [Fact]
        public void GetCommentLength_SetOfNonQuotedStrings_NothingParsed()
        {
            AssertGetCommentLength("a(x", 0, 0, HttpParseResult.NotParsed); // a"x"
            AssertGetCommentLength("\"(x", 0, 0, HttpParseResult.NotParsed); // ("x"
            AssertGetCommentLength("\\(x", 0, 0, HttpParseResult.NotParsed); // \"x"
        }

        [Fact]
        public void DateToString_UseRfcSampleTimestamp_FormattedAccordingToRfc1123()
        {
            // We don't need extensive tests, since we let DateTimeOffset do the formatting. This test is just
            // to validate that we use the correct parameters when calling into DateTimeOffset.ToString().
            DateTimeOffset dateTime = new DateTimeOffset(1994, 11, 6, 8, 49, 37, TimeSpan.Zero);
            Assert.Equal("Sun, 06 Nov 1994 08:49:37 GMT", HttpRuleParser.DateToString(dateTime));
        }

        [Fact]
        public void TryStringToDate_UseOfValidDateTimeStringsInDifferentFormats_ParsedCorrectly()
        {
            // We don't need extensive tests, since we let DateTimeOffset do the parsing. This test is just
            // to validate that we use the correct parameters when calling into DateTimeOffset.ToString().

            // RFC1123 date/time value
            DateTimeOffset expected = new DateTimeOffset(1994, 11, 6, 8, 49, 37, TimeSpan.Zero);
            DateTimeOffset result = DateTimeOffset.MinValue;
            Assert.True(HttpRuleParser.TryStringToDate("Sun, 06 Nov 1994 08:49:37 GMT", out result));
            Assert.Equal(expected, result);
            Assert.True(HttpRuleParser.TryStringToDate("Sun, 06 Nov 1994 08:49:37", out result));
            Assert.Equal(expected, result);
            Assert.True(HttpRuleParser.TryStringToDate("6 Nov 1994 8:49:37 GMT", out result));
            Assert.Equal(expected, result);
            Assert.True(HttpRuleParser.TryStringToDate("6 Nov 1994 8:49:37", out result));
            Assert.Equal(expected, result);
            Assert.True(HttpRuleParser.TryStringToDate("Sun, 06 Nov 94 08:49:37", out result));
            Assert.Equal(expected, result);
            Assert.True(HttpRuleParser.TryStringToDate("6 Nov 94 8:49:37", out result));
            Assert.Equal(expected, result);

            // RFC850 date/time value
            Assert.True(HttpRuleParser.TryStringToDate("Sunday, 06-Nov-94 08:49:37 GMT", out result));
            Assert.Equal(expected, result);
            Assert.True(HttpRuleParser.TryStringToDate("Sunday, 6-Nov-94 8:49:37", out result));
            Assert.Equal(expected, result);

            // ANSI C's asctime() format
            Assert.True(HttpRuleParser.TryStringToDate("Sun Nov  06 08:49:37 1994", out result));
            Assert.Equal(expected, result);
            Assert.True(HttpRuleParser.TryStringToDate("Sun Nov  6 8:49:37 1994", out result));
            Assert.Equal(expected, result);

            // RFC5322 date/time
            expected = new DateTimeOffset(1997, 11, 8, 9, 55, 6, new TimeSpan(-6, 0, 0));
            Assert.True(HttpRuleParser.TryStringToDate("Sat, 08 Nov 1997 09:55:06 -0600", out result));
            Assert.Equal(expected, result);
            expected = new DateTimeOffset(1997, 11, 8, 9, 55, 6, TimeSpan.Zero);
            Assert.True(HttpRuleParser.TryStringToDate("8 Nov 1997 9:55:6", out result));
            Assert.Equal(expected, result);
            expected = new DateTimeOffset(1997, 11, 8, 9, 55, 6, new TimeSpan(2, 0, 0));
            Assert.True(HttpRuleParser.TryStringToDate("Sat, 8 Nov 1997 9:55:6 +0200", out result));
            Assert.Equal(expected, result);
        }

        [Fact]
        public void TryStringToDate_UseInvalidDateTimeString_Throw()
        {
            DateTimeOffset result = DateTimeOffset.MinValue;
            Assert.False(HttpRuleParser.TryStringToDate("Sun, 06 Nov 1994 08:49:37 GMT invalid", out result));
            Assert.False(HttpRuleParser.TryStringToDate("Sun, 06 Nov 1994 08:49:37 GMT,", out result));
            Assert.False(HttpRuleParser.TryStringToDate(",Sun, 06 Nov 1994 08:49:37 GMT", out result));
        }

        [Fact]
        public void GetWhitespaceLength_SetOfValidWhitespaces_ParsedCorrectly()
        {
            Assert.Equal(1, HttpRuleParser.GetWhitespaceLength(" ", 0));
            Assert.Equal(0, HttpRuleParser.GetWhitespaceLength("a b", 0));
            Assert.Equal(1, HttpRuleParser.GetWhitespaceLength("a b", 1));
            Assert.Equal(1, HttpRuleParser.GetWhitespaceLength("a\tb", 1));
            Assert.Equal(1, HttpRuleParser.GetWhitespaceLength("a\t", 1));
            Assert.Equal(3, HttpRuleParser.GetWhitespaceLength("a\t  ", 1));
            Assert.Equal(2, HttpRuleParser.GetWhitespaceLength("\t b", 0));

            // Newlines
            Assert.Equal(3, HttpRuleParser.GetWhitespaceLength("a\r\n b", 1));
            Assert.Equal(3, HttpRuleParser.GetWhitespaceLength("\r\n ", 0));
            Assert.Equal(3, HttpRuleParser.GetWhitespaceLength("\r\n\t", 0));
            Assert.Equal(13, HttpRuleParser.GetWhitespaceLength("  \r\n\t\t  \r\n   ", 0));
            Assert.Equal(1, HttpRuleParser.GetWhitespaceLength(" \r\n", 0)); // first char considered valid whitespace
            Assert.Equal(1, HttpRuleParser.GetWhitespaceLength(" \r\n\r\n ", 0));
            Assert.Equal(3, HttpRuleParser.GetWhitespaceLength(" \r\n\r\n ", 3));
        }

        [Fact]
        public void GetWhitespaceLength_SetOfInvalidWhitespaces_ReturnsZero()
        {
            // Newlines: SP/HT required after #13#10
            Assert.Equal(0, HttpRuleParser.GetWhitespaceLength("\r\n", 0));
            Assert.Equal(0, HttpRuleParser.GetWhitespaceLength(" \r\n\r\n", 1));
            Assert.Equal(0, HttpRuleParser.GetWhitespaceLength("a\r\nb", 1));
        }

        [Fact]
        public void GetNumberLength_SetOfValidNumbers_ParsedCorrectly()
        {
            Assert.Equal(3, HttpRuleParser.GetNumberLength("123", 0, false));
            Assert.Equal(4, HttpRuleParser.GetNumberLength("123.", 0, true));
            Assert.Equal(7, HttpRuleParser.GetNumberLength("123.456", 0, true));
            Assert.Equal(1, HttpRuleParser.GetNumberLength("1a", 0, false));
            Assert.Equal(2, HttpRuleParser.GetNumberLength("1.a", 0, true));
            Assert.Equal(2, HttpRuleParser.GetNumberLength("1..", 0, true));
            Assert.Equal(3, HttpRuleParser.GetNumberLength("1.2.", 0, true));
            Assert.Equal(1, HttpRuleParser.GetNumberLength("1.2.", 0, false));
            Assert.Equal(5, HttpRuleParser.GetNumberLength("123456", 1, false));
            Assert.Equal(1, HttpRuleParser.GetNumberLength("1.5", 0, false)); // parse until '.'
            Assert.Equal(1, HttpRuleParser.GetNumberLength("1 2 3", 2, true));

            // GetNumberLength doesn't have any size restrictions. The caller needs to decide whether a value is
            // outside the valid range or not.
            Assert.Equal(30, HttpRuleParser.GetNumberLength("123456789012345678901234567890", 0, false));
            Assert.Equal(61, HttpRuleParser.GetNumberLength(
                "123456789012345678901234567890.123456789012345678901234567890", 0, true));
        }

        [Fact]
        public void GetNumberLength_SetOfInvalidNumbers_ReturnsZero()
        {
            Assert.Equal(0, HttpRuleParser.GetNumberLength(".456", 0, true));
            Assert.Equal(0, HttpRuleParser.GetNumberLength("-1", 0, true));
            Assert.Equal(0, HttpRuleParser.GetNumberLength("a", 0, true));
        }

        #region Helper methods

        private static void AssertGetTokenLength(string input, int startIndex, int expectedLength)
        {
            Assert.Equal(expectedLength, HttpRuleParser.GetTokenLength(input, startIndex));
        }

        private static void AssertGetQuotedPairLength(string input, int startIndex, int expectedLength,
            HttpParseResult expectedResult)
        {
            int length = 0;
            HttpParseResult result = HttpRuleParser.GetQuotedPairLength(input, startIndex, out length);

            Assert.Equal(expectedResult, result);
            Assert.Equal(expectedLength, length);
        }

        private static void AssertGetQuotedStringLength(string input, int startIndex, int expectedLength,
            HttpParseResult expectedResult)
        {
            int length = 0;
            HttpParseResult result = HttpRuleParser.GetQuotedStringLength(input, startIndex, out length);

            Assert.Equal(expectedResult, result);
            Assert.Equal(expectedLength, length);
        }

        private static void AssertGetCommentLength(string input, int startIndex, int expectedLength,
            HttpParseResult expectedResult)
        {
            int length = 0;
            HttpParseResult result = HttpRuleParser.GetCommentLength(input, startIndex, out length);

            Assert.Equal(expectedResult, result);
            Assert.Equal(expectedLength, length);
        }

        private static void AssertGetHostLength(string input, int startIndex, int expectedLength, bool allowToken,
            string expectedResult)
        {
            string result = null;
            Assert.Equal(expectedLength, HttpRuleParser.GetHostLength(input, startIndex, allowToken, out result));
            Assert.Equal(expectedResult, result);
        }
        #endregion
    }
}
