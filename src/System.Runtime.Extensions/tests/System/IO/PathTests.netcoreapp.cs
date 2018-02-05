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

        [Theory]
        [InlineData(@"dir/baz", @"dir")]
        [InlineData(@"dir//baz", @"dir")]
        [InlineData(@"dir\baz", @"")]
        [InlineData(@"dir/baz/bar", @"dir/baz")]
        [InlineData(@"../../files.txt", @"../..")]
        [InlineData(@"/", null)]
        [PlatformSpecific(TestPlatforms.AnyUnix)]  // Tests Unix-specific paths
        public static void GetDirectoryName_Unix(string path, string expected)
        {
            Assert.Equal(expected, Path.GetDirectoryName(path));
            Assert.Equal(expected ?? string.Empty, new string(Path.GetDirectoryName(path.AsReadOnlySpan())));
        }

        [PlatformSpecific(TestPlatforms.AnyUnix)]  // Checks Unix-specific special characters in directory path
        [Theory]
        [InlineData('\t', 1, "file")]
        [InlineData('\b', 2, "fi le")]
        [InlineData('\v', 3, "fi\nle")]
        [InlineData('\n', 4, "fi\rle")]
        public static void GetDirectoryName_ControlCharacters_Unix(char ch, int count, string file)
        {
            Assert.Equal(new string(ch, count), Path.GetDirectoryName(Path.Combine(new string(ch, count), file)));
            Assert.Equal(new string(ch, count), new string(Path.GetDirectoryName(Path.Combine(new string(ch, count), file).AsReadOnlySpan())));
        }

        [PlatformSpecific(TestPlatforms.AnyUnix)]  // Checks file extension behavior on Unix
        [Theory]
        [InlineData("file.e xe", ".e xe")]
        [InlineData("file. ", ". ")]
        [InlineData(" file. ", ". ")]
        [InlineData(" file.extension", ".extension")]
        [InlineData("file.exten\tsion", ".exten\tsion")]
        public static void GetExtension_Unix(string path, string expected)
        {
            Assert.Equal(expected, Path.GetExtension(path));
            Assert.Equal(!string.IsNullOrEmpty(expected), Path.HasExtension(path));

            Assert.Equal(expected, new string(Path.GetExtension(path.AsReadOnlySpan())));
            Assert.Equal(!string.IsNullOrEmpty(expected), Path.HasExtension(path.AsReadOnlySpan()));
        }

        [PlatformSpecific(TestPlatforms.AnyUnix)]  // Tests Unix-specific valid file names
        [Theory]
        [InlineData(" . ")]
        [InlineData(" .. ")]
        [InlineData("fi le")]
        public static void GetFileName_Unix(string file)
        {
            Assert.Equal(file, Path.GetFileName(file));
            Assert.Equal(file, new string(Path.GetFileName(file).AsReadOnlySpan()));
        }

        [Fact]
        public static void GetFileNameWithSpaces_Unix()
        {
            Assert.Equal("fi  le", Path.GetFileName(Path.Combine("b \r\n ar", "fi  le")));
            Assert.Equal("fi  le", new string(Path.GetFileName(Path.Combine("b \r\n ar", "fi  le").AsReadOnlySpan())));
        }

        [Fact]
        public static void GetDirectoryName_SpaceThrowOnWindows_Core()
        {
            string path = " ";
            Action action = () => Path.GetDirectoryName(path);
            if (PlatformDetection.IsWindows)
            {
                AssertExtensions.Throws<ArgumentException>("path", null, () => Path.GetDirectoryName(path));
                Assert.Equal(string.Empty, new string(Path.GetDirectoryName(path.AsReadOnlySpan())));
            }
            else
            {
                // This is a valid path on Unix
                action();
            }
        }
    }
}
