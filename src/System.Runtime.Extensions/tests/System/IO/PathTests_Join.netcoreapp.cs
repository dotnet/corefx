﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.IO.Tests
{
    public class PathTests_Join : PathTestsBase
    {
        public static TheoryData<string, string, string> TestData_JoinTwoPaths = new TheoryData<string, string, string>
        {
            { "", "", "" },
            { Sep, "", Sep },
            { AltSep, "", AltSep },
            { "", Sep, Sep },
            { "", AltSep, AltSep },
            { Sep, Sep, $"{Sep}{Sep}" },
            { AltSep, AltSep, $"{AltSep}{AltSep}" },
            { "a", "", "a" },
            { "", "a", "a" },
            { "a", "a", $"a{Sep}a" },
            { $"a{Sep}", "a", $"a{Sep}a" },
            { "a", $"{Sep}a", $"a{Sep}a" },
            { $"a{Sep}", $"{Sep}a", $"a{Sep}{Sep}a" },
            { "a", $"a{Sep}", $"a{Sep}a{Sep}" },
            { $"a{AltSep}", "a", $"a{AltSep}a" },
            { "a", $"{AltSep}a", $"a{AltSep}a" },
            { $"a{Sep}", $"{AltSep}a", $"a{Sep}{AltSep}a" },
            { $"a{AltSep}", $"{AltSep}a", $"a{AltSep}{AltSep}a" },
            { "a", $"a{AltSep}", $"a{Sep}a{AltSep}" },
        };

        [Theory, MemberData(nameof(TestData_JoinTwoPaths))]
        public void JoinTwoPaths(string path1, string path2, string expected)
        {
            Assert.Equal(expected, Path.Join(path1, path2));
        }

        [Theory, MemberData(nameof(TestData_JoinTwoPaths))]
        public void TryJoinTwoPaths(string path1, string path2, string expected)
        {
            char[] output = new char[expected.Length];

            Assert.True(Path.TryJoin(path1, path2, output, out int written));
            Assert.Equal(expected.Length, written);
            Assert.Equal(expected, new string(output));

            if (expected.Length > 0)
            {
                Assert.False(Path.TryJoin(path1, path2, Span<char>.Empty, out written));
                Assert.Equal(0, written);

                output = new char[expected.Length - 1];
                Assert.False(Path.TryJoin(path1, path2, output, out written));
                Assert.Equal(0, written);
                Assert.Equal(output, new char[output.Length]);
            }
        }

        public static TheoryData<string, string, string, string> TestData_JoinThreePaths = new TheoryData<string, string, string, string>
        {
            { "", "", "", "" },
            { Sep, Sep, Sep, $"{Sep}{Sep}{Sep}" },
            { AltSep, AltSep, AltSep, $"{AltSep}{AltSep}{AltSep}" },
            { "a", "", "", "a" },
            { "", "a", "", "a" },
            { "", "", "a", "a" },
            { "a", "", "a", $"a{Sep}a" },
            { "a", "a", "", $"a{Sep}a" },
            { "", "a", "a", $"a{Sep}a" },
            { "a", "a", "a", $"a{Sep}a{Sep}a" },
            { "a", Sep, "a", $"a{Sep}a" },
            { $"a{Sep}", "", "a", $"a{Sep}a" },
            { $"a{Sep}", "a", "", $"a{Sep}a" },
            { "", $"a{Sep}", "a", $"a{Sep}a" },
            { "a", "", $"{Sep}a", $"a{Sep}a" },
            { $"a{AltSep}", "", "a", $"a{AltSep}a" },
            { $"a{AltSep}", "a", "", $"a{AltSep}a" },
            { "", $"a{AltSep}", "a", $"a{AltSep}a" },
            { "a", "", $"{AltSep}a", $"a{AltSep}a" },
        };

        [Theory, MemberData(nameof(TestData_JoinThreePaths))]
        public void JoinThreePaths(string path1, string path2, string path3, string expected)
        {
            Assert.Equal(expected, Path.Join(path1, path2, path3));
        }

        [Theory, MemberData(nameof(TestData_JoinThreePaths))]
        public void TryJoinThreePaths(string path1, string path2, string path3, string expected)
        {
            char[] output = new char[expected.Length];

            Assert.True(Path.TryJoin(path1, path2, path3, output, out int written));
            Assert.Equal(expected.Length, written);
            Assert.Equal(expected, new string(output));

            if (expected.Length > 0)
            {
                Assert.False(Path.TryJoin(path1, path2, path3, Span<char>.Empty, out written));
                Assert.Equal(0, written);

                output = new char[expected.Length - 1];
                Assert.False(Path.TryJoin(path1, path2, path3, output, out written));
                Assert.Equal(0, written);
                Assert.Equal(output, new char[output.Length]);
            }
        }
    }
}
