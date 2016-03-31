// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using Xunit;

namespace System.Globalization.Tests
{
    public class StringInfoParseCombiningCharacters
    {
        public static IEnumerable<object[]> ParseCombiningCharacters_TestData()
        {
            yield return new object[] { "\u4f00\u302a\ud800\udc00\u4f01", new int[] { 0, 2, 4 } };
            yield return new object[] { "abcdefgh", new int[] { 0, 1, 2, 3, 4, 5, 6, 7 } };
            yield return new object[] { "!@#$%^&", new int[] { 0, 1, 2, 3, 4, 5, 6 } };
            yield return new object[] { "!\u20D1bo\uFE22\u20D1\u20EB|", new int[] { 0, 2, 3, 7 } };
            yield return new object[] { "1\uDBFF\uDFFF@\uFE22\u20D1\u20EB9", new int[] { 0, 1, 3, 7 } };
            yield return new object[] { "   ", new int[] { 0, 1, 2 } };
            yield return new object[] { "", new int[0] };
        }

        [Theory]
        [MemberData(nameof(ParseCombiningCharacters_TestData))]
        public void ParseCombiningCharacters(string str, int[] expected)
        {
            Assert.Equal(expected, StringInfo.ParseCombiningCharacters(str));
        }

        [Fact]
        public void ParseCombiningCharacters_InvalidUnicodeChars()
        {
            ParseCombiningCharacters("\u0000\uFFFFa", new int[] { 0, 1, 2 }); // Control chars
            ParseCombiningCharacters("\uD800a", new int[] { 0, 1 }); // Unmatched high surrogate
            ParseCombiningCharacters("\uDC00a", new int[] { 0, 1 }); // Unmatched low surrogate
            ParseCombiningCharacters("\u00ADa", new int[] { 0, 1 }); // Format character

            ParseCombiningCharacters("\u0000\u0300\uFFFF\u0300", new int[] { 0, 1, 2, 3 }); // Control chars + combining char
            ParseCombiningCharacters("\uD800\u0300", new int[] { 0, 1 }); // Unmatched high surrogate + combining char
            ParseCombiningCharacters("\uDC00\u0300", new int[] { 0, 1 }); // Unmatched low surrogate + combing char
            ParseCombiningCharacters("\u00AD\u0300", new int[] { 0, 1 }); // Format character + combining char

            ParseCombiningCharacters("\u0300\u0300", new int[] { 0, 1 }); // Two combining chars
        }
        
        [Fact]
        public void ParseCombiningCharacters_Null_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>("str", () => StringInfo.ParseCombiningCharacters(null)); // Str is null
        }
    }
}
