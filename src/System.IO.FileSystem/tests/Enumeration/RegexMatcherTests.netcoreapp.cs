// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.IO.Enumeration;
using Xunit;

namespace System.IO.Tests.Enumeration
{
    public class RegexMatcherTests
    {
        public static TheoryData<string, string, bool, bool> TestData_MatchesRegularExpression = new TheoryData<string, string, bool, bool>
        {
            { "", "", true, true },
            { "", "", false, true },

            // Check casing options are set properly
            { "A", "a", false, false },
            { "A", "(?i)a", false, true },
            { "A", "a", true, true },
            { "A", "(?-i)a", true, false },

            // Ensure that single line mode is our default
            { "a" + Environment.NewLine + "b", ".b", true, true },
            { "a" + Environment.NewLine + "b", ".b", false, true },
            { "a" + Environment.NewLine + "b", "(?-s).b", true, false },
            { "a" + Environment.NewLine + "b", "(?-s).b", false, false },

            // Try a named capture
            { "aa", @"(?<first>\w)\k<first>", true, true },
            { "ab", @"(?<first>\w)\k<first>", true, false },

            // Implicit capturing
            { "aa", @"(?-n)(\w)\1", true, true },
            { "ab", @"(?-n)(\w)\1", true, false }
        };

        [Theory, MemberData(nameof(TestData_MatchesRegularExpression))]
        public void MatchTests(string value, string expression, bool ignoreCase, bool expected)
        {
            Assert.Equal(expected, FileSystemName.MatchesRegularExpression(expression, value, ignoreCase));
        }

        [Fact]
        public void ImplicitCapturesDisabled()
        {
            Assert.Throws<ArgumentException>(null, () => FileSystemName.MatchesRegularExpression(@"(\w)\1", "aa"));
        }
    }
}
