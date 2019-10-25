// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using Xunit;

namespace System.Globalization.Tests
{
    public class CompareInfoIsSuffixTests
    {
        private static CompareInfo s_invariantCompare = CultureInfo.InvariantCulture.CompareInfo;
        private static CompareInfo s_hungarianCompare = new CultureInfo("hu-HU").CompareInfo;
        private static CompareInfo s_turkishCompare = new CultureInfo("tr-TR").CompareInfo;
        private static CompareInfo s_frenchCompare = new CultureInfo("fr-FR").CompareInfo;
        private static CompareInfo s_slovakCompare = new CultureInfo("sk-SK").CompareInfo;

        public static IEnumerable<object[]> IsSuffix_TestData()
        {
            // Empty strings
            yield return new object[] { s_invariantCompare, "foo", "", CompareOptions.None, true };
            yield return new object[] { s_invariantCompare, "", "", CompareOptions.None, true };

            // Long strings
            yield return new object[] { s_invariantCompare, new string('a', 5555), "aaaaaaaaaaaaaaa", CompareOptions.None, true };
            yield return new object[] { s_invariantCompare, new string('a', 5555), new string('a', 5000), CompareOptions.None, true };
            yield return new object[] { s_invariantCompare, new string('a', 5555), new string('a', 5000) + "b", CompareOptions.None, false };

            // Hungarian
            yield return new object[] { s_hungarianCompare, "foobardzsdzs", "rddzs", CompareOptions.Ordinal, false };
            yield return new object[] { s_invariantCompare, "foobardzsdzs", "rddzs", CompareOptions.None, false };
            yield return new object[] { s_invariantCompare, "foobardzsdzs", "rddzs", CompareOptions.Ordinal, false };
            yield return new object[] { s_invariantCompare, "dz", "z", CompareOptions.None, true };
            yield return new object[] { s_hungarianCompare, "dz", "z", CompareOptions.None, false };
            yield return new object[] { s_hungarianCompare, "dz", "z", CompareOptions.Ordinal, true };

            // Slovak
            yield return new object[] { s_slovakCompare, "ch", "h", CompareOptions.None, false };
            yield return new object[] { s_slovakCompare, "velmi chora", "hora", CompareOptions.None, false };
            yield return new object[] { s_slovakCompare, "chh", "H", CompareOptions.IgnoreCase, true };

            // Turkish
            yield return new object[] { s_turkishCompare, "Hi", "I", CompareOptions.None, false };
            yield return new object[] { s_turkishCompare, "Hi", "I", CompareOptions.IgnoreCase, false };
            yield return new object[] { s_turkishCompare, "Hi", "\u0130", CompareOptions.None, false };
            yield return new object[] { s_turkishCompare, "Hi", "\u0130", CompareOptions.IgnoreCase, true };
            yield return new object[] { s_invariantCompare, "Hi", "I", CompareOptions.None, false };
            yield return new object[] { s_invariantCompare, "Hi", "I", CompareOptions.IgnoreCase, true };
            yield return new object[] { s_invariantCompare, "Hi", "\u0130", CompareOptions.None, false };
            yield return new object[] { s_invariantCompare, "Hi", "\u0130", CompareOptions.IgnoreCase, false };

            // Unicode
            yield return new object[] { s_invariantCompare, "Exhibit \u00C0", "A\u0300", CompareOptions.None, true };
            yield return new object[] { s_invariantCompare, "Exhibit \u00C0", "A\u0300", CompareOptions.Ordinal, false };
            yield return new object[] { s_invariantCompare, "Exhibit \u00C0", "a\u0300", CompareOptions.None, false };
            yield return new object[] { s_invariantCompare, "Exhibit \u00C0", "a\u0300", CompareOptions.IgnoreCase, true };
            yield return new object[] { s_invariantCompare, "Exhibit \u00C0", "a\u0300", CompareOptions.Ordinal, false };
            yield return new object[] { s_invariantCompare, "Exhibit \u00C0", "a\u0300", CompareOptions.OrdinalIgnoreCase, false };
            yield return new object[] { s_invariantCompare, "FooBar", "Foo\u0400Bar", CompareOptions.Ordinal, false };
            yield return new object[] { s_invariantCompare, "FooBA\u0300R", "FooB\u00C0R", CompareOptions.IgnoreNonSpace, true };
            yield return new object[] { s_invariantCompare, "o\u0308", "o", CompareOptions.None, false };
            yield return new object[] { s_invariantCompare, "o\u0308", "o", CompareOptions.Ordinal, false };
            yield return new object[] { s_invariantCompare, "o\u0308o", "o", CompareOptions.None, true };
            yield return new object[] { s_invariantCompare, "o\u0308o", "o", CompareOptions.Ordinal, true };

            // Surrogates
            yield return new object[] { s_invariantCompare, "\uD800\uDC00", "\uD800\uDC00", CompareOptions.None, true };
            yield return new object[] { s_invariantCompare, "\uD800\uDC00", "\uD800\uDC00", CompareOptions.IgnoreCase, true };
            yield return new object[] { s_invariantCompare, "\uD800\uDC00", "\uDC00", CompareOptions.Ordinal, true };
            yield return new object[] { s_invariantCompare, "\uD800\uDC00", "\uDC00", CompareOptions.OrdinalIgnoreCase, true };

            // Malformed Unicode - Invalid Surrogates (there is nothing special about them, they don't have a special treatment)
            yield return new object[] { s_invariantCompare, "\uD800\uD800", "\uD800", CompareOptions.None, true };
            yield return new object[] { s_invariantCompare, "\uD800\uD800", "\uD800\uD800", CompareOptions.None, true };

            // Ignore symbols
            yield return new object[] { s_invariantCompare, "More Test's", "Tests", CompareOptions.IgnoreSymbols, true };
            yield return new object[] { s_invariantCompare, "More Test's", "Tests", CompareOptions.None, false };

            // NULL character
            yield return new object[] { s_invariantCompare, "a\u0000b", "a\u0000b", CompareOptions.None, true };
            yield return new object[] { s_invariantCompare, "a\u0000b", "b\u0000b", CompareOptions.None, false };

            // Platform differences
            yield return new object[] { s_hungarianCompare, "foobardzsdzs", "rddzs", CompareOptions.None, PlatformDetection.IsWindows ? true : false };
            yield return new object[] { s_frenchCompare, "\u0153", "oe", CompareOptions.None, PlatformDetection.IsWindows ? true : false };
            yield return new object[] { s_invariantCompare, "\uD800\uDC00", "\uDC00", CompareOptions.None, PlatformDetection.IsWindows ? true : false };
            yield return new object[] { s_invariantCompare, "\uD800\uDC00", "\uDC00", CompareOptions.IgnoreCase, PlatformDetection.IsWindows ? true : false };
        }

        [Theory]
        [MemberData(nameof(IsSuffix_TestData))]
        public void IsSuffix(CompareInfo compareInfo, string source, string value, CompareOptions options, bool expected)
        {
            if (options == CompareOptions.None)
            {
                Assert.Equal(expected, compareInfo.IsSuffix(source, value));
            }
            Assert.Equal(expected, compareInfo.IsSuffix(source, value, options));

            if ((compareInfo == s_invariantCompare) && ((options == CompareOptions.None) || (options == CompareOptions.IgnoreCase)))
            {
                StringComparison stringComparison = (options == CompareOptions.IgnoreCase) ? StringComparison.InvariantCultureIgnoreCase : StringComparison.InvariantCulture;
                Assert.Equal(expected, source.EndsWith(value, stringComparison));
                Assert.Equal(expected, source.AsSpan().EndsWith(value.AsSpan(), stringComparison));
            }
        }

        [Fact]
        public void IsSuffix_UnassignedUnicode()
        {
            bool result = PlatformDetection.IsWindows ? true : false;

            IsSuffix(s_invariantCompare, "FooBar", "Foo\uFFFFBar", CompareOptions.None, result);
            IsSuffix(s_invariantCompare, "FooBar", "Foo\uFFFFBar", CompareOptions.IgnoreNonSpace, result);
        }

        [Fact]
        public void IsSuffix_Invalid()
        {
            // Source is null
            AssertExtensions.Throws<ArgumentNullException>("source", () => s_invariantCompare.IsSuffix(null, ""));
            AssertExtensions.Throws<ArgumentNullException>("source", () => s_invariantCompare.IsSuffix(null, "", CompareOptions.None));

            // Prefix is null
            AssertExtensions.Throws<ArgumentNullException>("suffix", () => s_invariantCompare.IsSuffix("", null));
            AssertExtensions.Throws<ArgumentNullException>("suffix", () => s_invariantCompare.IsSuffix("", null, CompareOptions.None));

            // Source and prefix are null
            AssertExtensions.Throws<ArgumentNullException>("source", () => s_invariantCompare.IsSuffix(null, null));
            AssertExtensions.Throws<ArgumentNullException>("source", () => s_invariantCompare.IsSuffix(null, null, CompareOptions.None));

            // Options are invalid
            AssertExtensions.Throws<ArgumentException>("options", () => s_invariantCompare.IsSuffix("Test's", "Tests", CompareOptions.StringSort));
            AssertExtensions.Throws<ArgumentException>("options", () => s_invariantCompare.IsSuffix("Test's", "Tests", CompareOptions.Ordinal | CompareOptions.IgnoreWidth));
            AssertExtensions.Throws<ArgumentException>("options", () => s_invariantCompare.IsSuffix("Test's", "Tests", CompareOptions.OrdinalIgnoreCase | CompareOptions.IgnoreWidth));
            AssertExtensions.Throws<ArgumentException>("options", () => s_invariantCompare.IsSuffix("Test's", "Tests", (CompareOptions)(-1)));
            AssertExtensions.Throws<ArgumentException>("options", () => s_invariantCompare.IsSuffix("Test's", "Tests", (CompareOptions)0x11111111));
        }
    }
}
