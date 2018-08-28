// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Text;
using Xunit;

using static System.Tests.Utf8TestUtilities;

namespace System.Tests
{
    public static unsafe class Utf8StringTests
    {
        // whitespace chars
        private const string AllWhitespaceChars = "\u0009\u000A\u000B\u000C\u000D\u0020\u0085\u00A0\u1680\u2000\u2001\u2002\u2003\u2004\u2005\u2006\u2007\u2008\u2009\u200A\u2028\u2029\u202F\u205F\u3000";

        [Fact]
        public static void Empty_HasLengthZero()
        {
            Assert.Equal(0, Utf8String.Empty.Length);
            SpanAssert.Equal(ReadOnlySpan<byte>.Empty, Utf8String.Empty.AsSpan());
        }

        [Fact]
        public static void Empty_ReturnsSingleton()
        {
            Assert.Same(Utf8String.Empty, Utf8String.Empty);
        }

        [Fact]
        public static void GetPinnableReference_CalledMultipleTimes_ReturnsSameValue()
        {
            var utf8 = Utf8String.Literal("Hello!");

            fixed (byte* pA = utf8)
            fixed (byte* pB = utf8)
            {
                Assert.True(pA == pB);
            }
        }
        
        [Fact]
        public static void GetPinnableReference_Empty()
        {
            fixed (byte* pStr = Utf8String.Empty)
            {
                Assert.True(pStr != null);
                Assert.Equal((byte)0, *pStr); // should point to null terminator
            }
        }

        [Fact]
        public static void GetPinnableReference_NotEmpty()
        {
            fixed (byte* pStr = Utf8String.Literal("Hello!"))
            {
                Assert.True(pStr != null);

                Assert.Equal((byte)'H', pStr[0]);
                Assert.Equal((byte)'e', pStr[1]);
                Assert.Equal((byte)'l', pStr[2]);
                Assert.Equal((byte)'l', pStr[3]);
                Assert.Equal((byte)'o', pStr[4]);
                Assert.Equal((byte)'!', pStr[5]);
                Assert.Equal((byte)'\0', pStr[6]);
            }
        }

        [Theory]
        [InlineData("", true)]
        [InlineData("not empty", false)]
        public static void IsNullOrEmpty(string value, bool expectedIsNullOrEmpty)
        {
            Assert.Equal(expectedIsNullOrEmpty, Utf8String.IsNullOrEmpty(new Utf8String(value)));
        }

        [Fact]
        public static void IsNullOrEmpty_Null_ReturnsTrue()
        {
            Assert.True(Utf8String.IsNullOrEmpty(null));
        }

        [Theory]
        [InlineData("", true)]
        [InlineData("not whitespace", false)]
        [InlineData("   not whitespace   ", false)]
        [InlineData("    ", true)]
        [InlineData(AllWhitespaceChars, true)]
        public static void IsNullOrWhiteSpace_And_IsEmptyOrWhiteSpace(string value, bool expectedIsNullOrWhiteSpace)
        {
            var utf8 = new Utf8String(value);

            Assert.Equal(expectedIsNullOrWhiteSpace, Utf8String.IsNullOrWhiteSpace(utf8));
            Assert.Equal(expectedIsNullOrWhiteSpace, Utf8String.IsEmptyOrWhiteSpace(utf8.AsSpan()));
        }

        [Fact]
        public static void IsNullOrWhiteSpace_Null_ReturnsTrue()
        {
            Assert.True(Utf8String.IsNullOrWhiteSpace(null));
        }

        [Fact]
        public static void Literal_DifferentInputs_ReturnsDifferentInstances()
        {
            Assert.NotSame(Utf8String.Literal("first"), Utf8String.Literal("second"));
        }

        [Fact]
        public static void Literal_Empty_ReturnsEmpty()
        {
            Assert.Same(Utf8String.Literal(string.Empty), Utf8String.Empty);
        }

        [Fact]
        public static void Literal_SameInputs_ReturnsSameInstance()
        {
            Assert.Same(Utf8String.Literal("first"), Utf8String.Literal("first"));
        }

        [Theory]
        [InlineData(null, null, true)]
        [InlineData("", null, false)]
        [InlineData(null, "", false)]
        [InlineData("hello", null, false)]
        [InlineData(null, "hello", false)]
        [InlineData("hello", "hello", true)]
        [InlineData("hello", "Hello", false)]
        [InlineData("hello there", "hello", false)]
        public static void Equality_Ordinal(string aString, string bString, bool expected)
        {
            Utf8String a = U(aString);
            Utf8String b = U(bString);

            // Operators

            Assert.Equal(expected, a == b);
            Assert.NotEqual(expected, a != b);

            // Static methods

            Assert.Equal(expected, Utf8String.Equals(a, b));
            Assert.Equal(expected, Utf8String.Equals(a, b, StringComparison.Ordinal));

            // Instance methods

            if (a != null)
            {
                Assert.Equal(expected, a.Equals(b));
                Assert.Equal(expected, a.Equals((object)b));
            }
        }

        [Theory]
        [InlineData("Hello", 0, 5, "Hello")]
        [InlineData("Hello", 0, 3, "Hel")]
        [InlineData("Hello", 2, 3, "llo")]
        [InlineData("Hello", 5, 0, "")]
        [InlineData("", 0, 0, "")]
        public static void Substring_Ascii(string sAsString, int startIndex, int length, string expectedAsString)
        {
            void Substring_AsciiCore(Utf8String s, Utf8String expected)
            {
                if (startIndex + length == s.Length)
                {
                    Assert.Equal(expected, s.Substring(startIndex));
                    Assert.Equal(expected, new Utf8String(s.AsSpan(startIndex)));

                    if (length == 0)
                    {
                        Assert.Same(Utf8String.Empty, s.Substring(startIndex));
                    }
                }
                Assert.Equal(expected, s.Substring(startIndex, length));

                Assert.Equal(expected, new Utf8String(s.AsSpan(startIndex, length)));

                if (length == s.Length)
                {
                    Assert.Same(s, s.Substring(startIndex));
                    Assert.Same(s, s.Substring(startIndex, length));
                }
                else if (length == 0)
                {
                    Assert.Same(Utf8String.Empty, s.Substring(startIndex, length));
                }
            };

            Substring_AsciiCore(new Utf8String(sAsString), new Utf8String(expectedAsString));
        }

        [Fact]
        public static void Substring_Invalid()
        {
            // Start index < 0
            AssertExtensions.Throws<ArgumentOutOfRangeException>("startIndex", () => Utf8String.Literal("foo").Substring(-1));
            AssertExtensions.Throws<ArgumentOutOfRangeException>("startIndex", () => Utf8String.Literal("foo").Substring(-1, 0));

            // Start index > string.Length
            AssertExtensions.Throws<ArgumentOutOfRangeException>("startIndex", () => Utf8String.Literal("foo").Substring(4));
            AssertExtensions.Throws<ArgumentOutOfRangeException>("startIndex", () => Utf8String.Literal("foo").Substring(4, 0));

            // Length < 0 or length > string.Length
            AssertExtensions.Throws<ArgumentOutOfRangeException>("length", () => Utf8String.Literal("foo").Substring(0, -1));
            AssertExtensions.Throws<ArgumentOutOfRangeException>("length", () => Utf8String.Literal("foo").Substring(0, 4));

            // Start index + length > string.Length
            AssertExtensions.Throws<ArgumentOutOfRangeException>("length", () => Utf8String.Literal("foo").Substring(3, 2));
            AssertExtensions.Throws<ArgumentOutOfRangeException>("length", () => Utf8String.Literal("foo").Substring(2, 2));
        }

        [Theory]
        [MemberData(nameof(TrimData))]
        public static void Trim(Utf8String input, Utf8String leftTrimmed, Utf8String rightTrimmed, Utf8String bothTrimmed)
        {
            Assert.Equal(leftTrimmed, input.TrimStart());
            Assert.Equal(rightTrimmed, input.TrimEnd());
            Assert.Equal(bothTrimmed, input.Trim());

            // If trimming didn't make any changes to the content of the string, it should return the original instance
            // instead of a clone of the original instance.

            if (input == leftTrimmed)
            {
                Assert.Same(input, leftTrimmed);
            }

            if (input == rightTrimmed)
            {
                Assert.Same(input, rightTrimmed);
            }

            if (input == bothTrimmed)
            {
                Assert.Same(input, bothTrimmed);
            }
        }

        public static IEnumerable<object[]> TrimData()
        {
            // data that doesn't need to be trimmed
            yield return new object[]
            {
                Utf8String.Literal("Hello!"), Utf8String.Literal("Hello!"), Utf8String.Literal("Hello!"), Utf8String.Literal("Hello!")
            };

            // data that's only whitespace chars
            yield return new object[]
            {
                Utf8String.Literal(AllWhitespaceChars), Utf8String.Empty, Utf8String.Empty, Utf8String.Empty
            };

            // whitespace chars on either side with non-whitespace in the middle
            yield return new object[]
            {
                Utf8String.Literal(AllWhitespaceChars + "Hello there!" + AllWhitespaceChars),
                Utf8String.Literal("Hello there!" + AllWhitespaceChars),
                Utf8String.Literal(AllWhitespaceChars + "Hello there!"),
                Utf8String.Literal("Hello there!")
            };

            /*
             * variants of non-whitespace multi-byte sequences that share the first byte with whitespace chars
             */

            var similarSequences = new[]
            {
                new { whitespaceChar = "\u0085", nearbyNonWhitespaceChar = "\u0086" },
                new { whitespaceChar = "\u1680", nearbyNonWhitespaceChar = "\u1681" },
                new { whitespaceChar = "\u200A", nearbyNonWhitespaceChar = "\u200B" },
                new { whitespaceChar = "\u2029", nearbyNonWhitespaceChar = "\u202A" },
                new { whitespaceChar = "\u205F", nearbyNonWhitespaceChar = "\u205E" },
                new { whitespaceChar = "\u3000", nearbyNonWhitespaceChar = "\u3001" },
            };

            foreach (var sequence in similarSequences)
            {
                yield return new object[]
                {
                    new Utf8String(sequence.whitespaceChar + sequence.nearbyNonWhitespaceChar + sequence.whitespaceChar + "Hello there!" + sequence.whitespaceChar + sequence.nearbyNonWhitespaceChar + sequence.whitespaceChar),
                    new Utf8String(sequence.nearbyNonWhitespaceChar + sequence.whitespaceChar + "Hello there!" + sequence.whitespaceChar + sequence.nearbyNonWhitespaceChar + sequence.whitespaceChar),
                    new Utf8String(sequence.whitespaceChar + sequence.nearbyNonWhitespaceChar + sequence.whitespaceChar + "Hello there!" + sequence.whitespaceChar + sequence.nearbyNonWhitespaceChar),
                    new Utf8String(sequence.nearbyNonWhitespaceChar + sequence.whitespaceChar + "Hello there!" + sequence.whitespaceChar + sequence.nearbyNonWhitespaceChar),
                };
            }

            /*
             * after this point contains invalid UTF-8 sequences, these shouldn't match whitespace
             */

            var utf8_C2 = Utf8String.Create(new byte[] { 0xC2 }, InvalidSequenceBehavior.LeaveUnchanged);
            yield return new object[]
            {
                utf8_C2,
                utf8_C2,
                utf8_C2,
                utf8_C2
            };

            var utf8_C2_x_C2 = Utf8String.Create(new byte[] { 0xC2, (byte)'x', 0xC2 }, InvalidSequenceBehavior.LeaveUnchanged);
            yield return new object[]
            {
                utf8_C2_x_C2,
                utf8_C2_x_C2,
                utf8_C2_x_C2,
                utf8_C2_x_C2
            };

            var utf8_E19A_x_E19A9A = Utf8String.Create(new byte[] { 0xE1, 0x9A, (byte)'x', 0xE1, 0x9A, 0x9A }, InvalidSequenceBehavior.LeaveUnchanged);
            yield return new object[]
            {
                utf8_E19A_x_E19A9A,
                utf8_E19A_x_E19A9A,
                utf8_E19A_x_E19A9A,
                utf8_E19A_x_E19A9A
            };

            var utf8_E19AE19A = Utf8String.Create(new byte[] { 0xE1, 0x9A, 0xE1, 0x9A }, InvalidSequenceBehavior.LeaveUnchanged);
            yield return new object[]
            {
                utf8_E19AE19A,
                utf8_E19AE19A,
                utf8_E19AE19A,
                utf8_E19AE19A
            };
        }
    }
}
