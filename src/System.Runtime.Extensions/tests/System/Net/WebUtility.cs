// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Xunit;

namespace System.Net.Tests
{
    public class WebUtilityTests
    {
        // HtmlEncode + HtmlDecode

        public static IEnumerable<object[]> HtmlDecode_TestData()
        {
            yield return new object[] { "Hello! &apos;&quot;&lt;&amp;&gt;\u2665&hearts;\u00E7&#xe7;&#231;", "Hello! '\"<&>\u2665\u2665\u00E7\u00E7\u00E7" };
            yield return new object[] { "Hello, world! \"<>\u2665\u00E7", "Hello, world! \"<>\u2665\u00E7" }; // No special chars
            yield return new object[] { null, null };

            yield return new object[] { "&#144308;", char.ConvertFromUtf32(144308) };
        }

        [Theory]
        [MemberData(nameof(HtmlDecode_TestData))]
        public static void HtmlDecode(string value, string expected)
        {
            Assert.Equal(expected, WebUtility.HtmlDecode(value));
        }

        public static IEnumerable<object[]> HtmlEncode_TestData()
        {
            // Single quotes need to be encoded as &#39; rather than &apos; since &#39; is valid both for
            // HTML and XHTML, but &apos; is valid only for XHTML.
            // For more info: http://fishbowl.pastiche.org/2003/07/01/the_curse_of_apos/
            yield return new object[] { "'", "&#39;" };
            yield return new object[] { "Hello! '\"<&>\u2665\u00E7 World", "Hello! &#39;&quot;&lt;&amp;&gt;\u2665&#231; World" };
            yield return new object[] { null, null };
            yield return new object[] { "Hello, world!", "Hello, world!" }; // No special chars

            yield return new object[] { char.ConvertFromUtf32(144308), "&#144308;" }; // Default strict settings
        }

        [Theory]
        [MemberData(nameof(HtmlEncode_TestData))]
        public static void HtmlEncode(string value, string expected)
        {
            Assert.Equal(expected, WebUtility.HtmlEncode(value));
        }

        // Shared test data for UrlEncode + Decode and their ToBytes counterparts

        public static IEnumerable<Tuple<string, string>> UrlDecode_SharedTestData()
        {
            // Recent change brings function inline with RFC 3986 to return hex-encoded chars in uppercase
            yield return Tuple.Create("%2F%5C%22%09Hello!+%E2%99%A5%3F%2F%5C%22%09World!+%E2%99%A5%3F%E2%99%A5", "/\\\"\tHello! \u2665?/\\\"\tWorld! \u2665?\u2665");
            yield return Tuple.Create("Hello, world", "Hello, world"); // No special chars
            yield return Tuple.Create("%F0%90%8F%BF", "\uD800\uDFFF"); // Surrogate pair
        }

        public static IEnumerable<Tuple<string, string>> UrlEncode_SharedTestData()
        {
            // Recent change brings function inline with RFC 3986 to return hex-encoded chars in uppercase
            yield return Tuple.Create("/\\\"\tHello! \u2665?/\\\"\tWorld! \u2665?\u2665", "%2F%5C%22%09Hello!+%E2%99%A5%3F%2F%5C%22%09World!+%E2%99%A5%3F%E2%99%A5");
            yield return Tuple.Create("'", "%27");
            yield return Tuple.Create("\uD800\uDFFF", "%F0%90%8F%BF"); // Surrogate pairs should be encoded as 4 bytes together
            
            // TODO: Uncomment this block out when dotnet/corefx#7166 is fixed.

            /*
            
            // Tests for stray surrogate chars (all should be encoded as U+FFFD)
            // Relevant GitHub issue: dotnet/corefx#7036
            
            yield return Tuple.Create("\uD800", "%EF%BF%BD"); // High surrogate
            yield return Tuple.Create("\uDC00", "%EF%BF%BD"); // Low surrogate

            yield return Tuple.Create("\uDC00\uD800", "%EF%BF%BD%EF%BF%BD"); // Low + high
            yield return Tuple.Create("\uD900\uDA00", "%EF%BF%BD%EF%BF%BD"); // High + high
            yield return Tuple.Create("\uDE00\uDF00", "%EF%BF%BD%EF%BF%BD"); // Low + low

            yield return Tuple.Create("!\uDB00@", "!%EF%BF%BD%40"); // Non-surrogate + high + non-surrogate
            yield return Tuple.Create("#\uDD00$", "%23%EF%BF%BD%24"); // Non-surrogate + low + non-surrogate
            
            */
        }

        public static IEnumerable<object[]> UrlEncodeDecode_Roundtrip_SharedTestData()
        {
            yield return new object[] { "'" };
            yield return new object[] { "http://www.microsoft.com" };
            yield return new object[] { "/\\\"\tHello! \u2665?/\\\"\tWorld! \u2665?\u2665" };
            yield return new object[] { "\uD800\uDFFF" }; // Surrogate pairs

            yield return new object[] { CharRange('\uE000', '\uF8FF') }; // BMP private use chars
            yield return new object[] { CharRange('\uFDD0', '\uFDEF') }; // Low BMP non-chars
            // TODO: Uncomment when dotnet/corefx#7166 is fixed.
            // yield return new object[] { "\uFFFE\uFFFF" }; // High BMP non-chars

            yield return new object[] { CharRange('\0', '\u001F') }; // C0 controls
            yield return new object[] { CharRange('\u0080', '\u009F') }; // C1 controls

            yield return new object[] { CharRange('\u202A', '\u202E') }; // BIDI embedding and override
            yield return new object[] { CharRange('\u2066', '\u2069') }; // BIDI isolate

            yield return new object[] { "\uFEFF" }; // BOM

            // Astral plane private use chars
            yield return new object[] { CharRange(0xF0000, 0xFFFFD) };
            yield return new object[] { CharRange(0x100000, 0x10FFFD) };
            // Astral plane non-chars
            yield return new object[] { "\U0001FFFE" };
            yield return new object[] { "\U0001FFFF" };
            yield return new object[] { CharRange(0x2FFFE, 0x10FFFF) };
        }

        // UrlEncode + UrlDecode

        public static IEnumerable<object[]> UrlDecode_TestData()
        {
            foreach (var tuple in UrlDecode_SharedTestData())
                yield return new object[] { tuple.Item1, tuple.Item2 };
            yield return new object[] { null, null };
        }

        [Theory]
        [MemberData(nameof(UrlDecode_TestData))]
        public static void UrlDecode(string encodedValue, string expected)
        {
            Assert.Equal(expected, WebUtility.UrlDecode(encodedValue));
        }

        public static IEnumerable<object[]> UrlEncode_TestData()
        {
            foreach (var tuple in UrlEncode_SharedTestData())
                yield return new object[] { tuple.Item1, tuple.Item2 };
            yield return new object[] { null, null };
        }

        [Theory]
        [MemberData(nameof(UrlEncode_TestData))]
        public static void UrlEncode(string value, string expected)
        {
            Assert.Equal(expected, WebUtility.UrlEncode(value));
        }

        [Theory]
        [MemberData(nameof(UrlEncodeDecode_Roundtrip_SharedTestData))]
        public static void UrlEncodeDecode_Roundtrip(string value)
        {
            string encoded = WebUtility.UrlEncode(value);
            Assert.Equal(value, WebUtility.UrlDecode(encoded));
        }
        
        // UrlEncode + DecodeToBytes

        public static IEnumerable<object[]> UrlDecodeToBytes_TestData()
        {
            foreach (var tuple in UrlDecode_SharedTestData())
            {
                byte[] input = Encoding.UTF8.GetBytes(tuple.Item1);
                byte[] output = Encoding.UTF8.GetBytes(tuple.Item2);
                yield return new object[] { input, 0, input.Length, output };
            }
            yield return new object[] { null, 0, 0, null };
        }

        [Theory]
        [MemberData(nameof(UrlDecodeToBytes_TestData))]
        public static void UrlDecodeToBytes(byte[] value, int offset, int count, byte[] expected)
        {
            byte[] actual = WebUtility.UrlDecodeToBytes(value, offset, count);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public static void UrlDecodeToBytes_Invalid()
        {
            Assert.Throws<ArgumentNullException>("bytes", () => WebUtility.UrlDecodeToBytes(null, 0, 1)); // Bytes is null

            Assert.Throws<ArgumentOutOfRangeException>("offset", () => WebUtility.UrlDecodeToBytes(new byte[1], -1, 1)); // Offset < 0
            Assert.Throws<ArgumentOutOfRangeException>("offset", () => WebUtility.UrlDecodeToBytes(new byte[1], 2, 1)); // Offset > bytes.Length

            Assert.Throws<ArgumentOutOfRangeException>("count", () => WebUtility.UrlDecodeToBytes(new byte[1], 0, -1)); // Count < 0
            Assert.Throws<ArgumentOutOfRangeException>("count", () => WebUtility.UrlDecodeToBytes(new byte[1], 0, 3)); // Count > bytes.Length
        }

        [Theory]
        [InlineData("a", 0, 1)]
        [InlineData("a", 1, 0)]
        [InlineData("abc", 0, 3)]
        [InlineData("abc", 1, 2)]
        [InlineData("abc", 1, 1)]
        [InlineData("abcd", 1, 2)]
        [InlineData("abcd", 2, 2)]
        public static void UrlEncodeToBytes_NothingToExpand_OutputMatchesSubInput(string inputString, int offset, int count)
        {
            byte[] inputBytes = Encoding.UTF8.GetBytes(inputString);
            byte[] subInputBytes = new byte[count];
            Buffer.BlockCopy(inputBytes, offset, subInputBytes, 0, count);
            Assert.Equal(inputString.Length, inputBytes.Length);

            byte[] outputBytes = WebUtility.UrlEncodeToBytes(inputBytes, offset, count);

            Assert.NotSame(inputBytes, outputBytes);
            Assert.Equal(count, outputBytes.Length);
            Assert.Equal(subInputBytes, outputBytes);
        }
        
        public static IEnumerable<object[]> UrlEncodeToBytes_TestData()
        {
            foreach (var tuple in UrlEncode_SharedTestData())
            {
                byte[] input = Encoding.UTF8.GetBytes(tuple.Item1);
                byte[] output = Encoding.UTF8.GetBytes(tuple.Item2);
                yield return new object[] { input, 0, input.Length, output };
            }
            yield return new object[] { null, 0, 0, null };
        }

        [Theory]
        [MemberData(nameof(UrlEncodeToBytes_TestData))]
        public static void UrlEncodeToBytes(byte[] value, int offset, int count, byte[] expected)
        {
            byte[] actual = WebUtility.UrlEncodeToBytes(value, offset, count);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public static void UrlEncodeToBytes_Invalid()
        {
            Assert.Throws<ArgumentNullException>("bytes", () => WebUtility.UrlEncodeToBytes(null, 0, 1)); // Bytes is null

            Assert.Throws<ArgumentOutOfRangeException>("offset", () => WebUtility.UrlEncodeToBytes(new byte[1], -1, 1)); // Offset < 0
            Assert.Throws<ArgumentOutOfRangeException>("offset", () => WebUtility.UrlEncodeToBytes(new byte[1], 2, 1)); // Offset > bytes.Length

            Assert.Throws<ArgumentOutOfRangeException>("count", () => WebUtility.UrlEncodeToBytes(new byte[1], 0, -1)); // Count < 0
            Assert.Throws<ArgumentOutOfRangeException>("count", () => WebUtility.UrlEncodeToBytes(new byte[1], 0, 3)); // Count > bytes.Length
        }

        [Theory]
        [MemberData(nameof(UrlEncodeDecode_Roundtrip_SharedTestData))]
        public static void UrlEncodeDecodeToBytes_Roundtrip(string url)
        {
            byte[] input = Encoding.UTF8.GetBytes(url);
            byte[] encoded = WebUtility.UrlEncodeToBytes(input, 0, input.Length);
            Assert.Equal(input, WebUtility.UrlDecodeToBytes(encoded, 0, encoded.Length));
        }
        
        [Theory]
        [InlineData("FooBarQuux", 3, 7, "BarQuux")]
        public static void UrlEncodeToBytes_ExcludeIrrelevantData(string value, int offset, int count, string expected)
        {
            byte[] input = Encoding.UTF8.GetBytes(value);
            byte[] encoded = WebUtility.UrlEncodeToBytes(input, offset, count);
            string actual = Encoding.UTF8.GetString(encoded);
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData("FooBarQuux", 3, 7, "BarQuux")]
        public static void UrlDecodeToBytes_ExcludeIrrelevantData(string value, int offset, int count, string expected)
        {
            byte[] input = Encoding.UTF8.GetBytes(value);
            byte[] decoded = WebUtility.UrlDecodeToBytes(input, offset, count);
            string actual = Encoding.UTF8.GetString(decoded);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public static void UrlEncodeToBytes_NewArray()
        {
            // If no encoding is needed, the current implementation simply
            // returns the input array to a method which then clones it.

            // We have to make sure it always returns a new array, to
            // prevent problems where the input array is changed if
            // the output one is modified.

            byte[] input = Encoding.UTF8.GetBytes("Dont.Need.Encoding");
            byte[] output = WebUtility.UrlEncodeToBytes(input, 0, input.Length);
            Assert.NotSame(input, output);
        }

        [Fact]
        public static void UrlDecodeToBytes_NewArray()
        {
            byte[] input = Encoding.UTF8.GetBytes("Dont.Need.Decoding");
            byte[] output = WebUtility.UrlDecodeToBytes(input, 0, input.Length);
            Assert.NotSame(input, output);
        }

        public static string CharRange(int start, int end)
        {
            Debug.Assert(start <= end);

            int capacity = end - start + 1;
            var builder = new StringBuilder(capacity);
            for (int i = start; i <= end; i++)
            {
                // 0 -> \0, 65 -> A, 0x10FFFF -> \U0010FFFF
                builder.Append(char.ConvertFromUtf32(i));
            }
            return builder.ToString();
        }
    }
}
