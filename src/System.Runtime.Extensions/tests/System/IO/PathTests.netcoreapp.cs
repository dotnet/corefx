// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using Xunit;

namespace System.IO.Tests
{
    public static partial class PathTests
    {
        [Theory]
        [InlineData("\u00A0")] // Non-breaking Space
        [InlineData("\u2028")] // Line separator
        [InlineData("\u2029")] // Paragraph separator
        public static void GetDirectoryName_NonControl_Span(string path)
        {
            Assert.Equal(string.Empty, new string(Path.GetDirectoryName(path.AsReadOnlySpan())));
        }

        [Theory]
        [InlineData("\u00A0")] // Non-breaking Space
        [InlineData("\u2028")] // Line separator
        [InlineData("\u2029")] // Paragraph separator
        public static void GetDirectoryName_NonControlWithSeparator_Span(string path)
        {
            Assert.Equal(path, new string(Path.GetDirectoryName(Path.Combine(path, path).AsReadOnlySpan())));
        }

        [Theory]
        [InlineData(".", "")]
        [InlineData("..", "")]
        [InlineData("baz", "")]
        public static void GetDirectoryName_Span(string path, string expected)
        {
            Assert.Equal(expected, new string(Path.GetDirectoryName(path.AsReadOnlySpan())));
        }

        [Theory]
        [InlineData(@"dir/baz", "dir")]
        [InlineData(@"dir\baz", "dir")]
        [InlineData(@"dir\baz\bar", @"dir\baz")]
        [InlineData(@"dir\baz", "dir")]
        [InlineData(@" dir\\baz", @" dir\")]
        [InlineData(@" C:\dir\baz", @" C:\dir")]
        [InlineData(@"..\..\files.txt", @"..\..")]
        [InlineData(@"C:\", null)]
        [InlineData(@"C:", null)]
        [PlatformSpecific(TestPlatforms.Windows)]  // Tests Windows-specific paths
        public static void GetDirectoryName_Windows_Span(string path, string expected)
        {
            Assert.Equal(expected ?? string.Empty, new string(Path.GetDirectoryName(path.AsReadOnlySpan())));
        }

        [Fact]
        public static void GetDirectoryName_CurrentDirectory_Span()
        {
            string curDir = Directory.GetCurrentDirectory();
            Assert.Equal(curDir, new string(Path.GetDirectoryName(Path.Combine(curDir, "baz").AsReadOnlySpan())));
            Assert.Equal(string.Empty, new string(Path.GetDirectoryName(Path.GetPathRoot(curDir).AsReadOnlySpan())));
        }

        [Theory]
        [InlineData("file.exe", ".exe")]
        [InlineData("file", "")]
        [InlineData("file.", "")]
        [InlineData("file.s", ".s")]
        [InlineData("test/file", "")]
        [InlineData("test/file.extension", ".extension")]
        public static void GetExtension_Span(string path, string expected)
        {
            path = path.Replace('/', Path.DirectorySeparatorChar);

            Assert.Equal(expected, new string(Path.GetExtension(path.AsReadOnlySpan())));
            Assert.Equal(!string.IsNullOrEmpty(expected), Path.HasExtension(path.AsReadOnlySpan()));
        }

        [Theory]
        [MemberData(nameof(GetFileName_TestData))]
        public static void GetFileName_Span(string path, string expected)
        {
            if (path != null)
                Assert.Equal(expected, new string(Path.GetFileName(path.AsReadOnlySpan())));
        }

        [Theory]
        [MemberData(nameof(GetFileNameWithoutExtension_TestData))]
        public static void GetFileNameWithoutExtension_Span(string path, string expected)
        {
            if(path != null)
                Assert.Equal(expected, new string(Path.GetFileNameWithoutExtension(path.AsReadOnlySpan())));
        }

        [Fact]
        public static void GetPathRoot_Empty_Span()
        {
            Assert.Equal(string.Empty, new string(Path.GetPathRoot(ReadOnlySpan<char>.Empty)));
        }

        [PlatformSpecific(TestPlatforms.Windows)]  // Tests UNC
        [Theory]
        [InlineData(@"\\test\unc\path\to\something", @"\\test\unc")]
        [InlineData(@"\\a\b\c\d\e", @"\\a\b")]
        [InlineData(@"\\a\b\", @"\\a\b")]
        [InlineData(@"\\a\b", @"\\a\b")]
        [InlineData(@"\\test\unc", @"\\test\unc")]
        public static void GetPathRoot_Windows_UncAndExtended_Span(string value, string expected)
        {
            Assert.True(Path.IsPathRooted(value.AsReadOnlySpan()));
            Assert.Equal(expected, new string(Path.GetPathRoot(value.AsReadOnlySpan())));
        }

        [PlatformSpecific(TestPlatforms.Windows)]  // Tests UNC
        [Theory]
        [InlineData(@"\\?\UNC\test\unc", @"\\?\UNC", @"\\?\UNC\test\unc\path\to\something")]
        [InlineData(@"\\?\UNC\test\unc", @"\\?\UNC", @"\\?\UNC\test\unc")]
        [InlineData(@"\\?\UNC\a\b1", @"\\?\UNC", @"\\?\UNC\a\b1")]
        [InlineData(@"\\?\UNC\a\b2", @"\\?\UNC", @"\\?\UNC\a\b2\")]
        [InlineData(@"\\?\C:\", @"\\?\C:", @"\\?\C:\foo\bar.txt")]
        public static void GetPathRoot_Windows_UncAndExtended_WithLegacySupport_Span(string normalExpected, string legacyExpected, string value)
        {
            Assert.True(Path.IsPathRooted(value.AsReadOnlySpan()));

            string expected = PathFeatures.IsUsingLegacyPathNormalization() ? legacyExpected : normalExpected;
            Assert.Equal(expected, new string(Path.GetPathRoot(value.AsReadOnlySpan())));
        }

        [PlatformSpecific(TestPlatforms.Windows)]  // Tests Windows-specific path convention
        [Theory]
        [InlineData(@"C:", @"C:")]
        [InlineData(@"C:\", @"C:\")]
        [InlineData(@"C:\\", @"C:\")]
        [InlineData(@"C://", @"C:/")]
        [InlineData(@"C:\foo1", @"C:\")]
        [InlineData(@"C:\\foo2", @"C:\")]
        [InlineData(@"C://foo3", @"C:/")]
        public static void GetPathRoot_Windows_Span(string value, string expected)
        {
            Assert.True(Path.IsPathRooted(value.AsReadOnlySpan()));
            Assert.Equal(expected, new string(Path.GetPathRoot(value.AsReadOnlySpan())));
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        public static void IsPathRooted_Span(string path)
        {
            Assert.False(Path.IsPathRooted(path.AsReadOnlySpan()));
        }

        // Testing invalid drive letters !(a-zA-Z)
        [PlatformSpecific(TestPlatforms.Windows)]
        [Theory]
        [InlineData(@"@:\foo")]    // 064 = @     065 = A
        [InlineData(@"[:\\")]       // 091 = [     090 = Z
        [InlineData(@"`:\foo")]    // 096 = `     097 = a
        [InlineData(@"{:\\")]       // 123 = {     122 = z
        [SkipOnTargetFramework(TargetFrameworkMonikers.NetFramework, "Bug fixed on Core where it would return true if the first char is not a drive letter followed by a VolumeSeparatorChar coreclr/10297")]
        public static void IsPathRooted_Windows_Invalid_Span(string value)
        {
            Assert.False(Path.IsPathRooted(value.AsReadOnlySpan()));
        }

        [Fact]
        public static void GetInvalidPathChars_Span()
        {
            Assert.All(Path.GetInvalidPathChars(), c =>
            {
                string bad = c.ToString();
                Assert.Equal(string.Empty, new string(Path.GetDirectoryName(bad.AsReadOnlySpan())));
                Assert.Equal(string.Empty, new string(Path.GetExtension(bad.AsReadOnlySpan())));
                Assert.Equal(bad, new string(Path.GetFileName(bad.AsReadOnlySpan())));
                Assert.Equal(bad, new string(Path.GetFileNameWithoutExtension(bad.AsReadOnlySpan())));
                Assert.Equal(string.Empty, new string(Path.GetPathRoot(bad.AsReadOnlySpan())));
                Assert.False(Path.IsPathRooted(bad.AsReadOnlySpan()));
            });
        }
    }
}
