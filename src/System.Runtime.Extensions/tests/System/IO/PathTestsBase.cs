// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using Xunit;

namespace System.IO.Tests
{
    public partial class PathTestsBase : RemoteExecutorTestBase
    {
        protected static string Sep = Path.DirectorySeparatorChar.ToString();
        protected static string AltSep = Path.AltDirectorySeparatorChar.ToString();

        public static TheoryData<string> TestData_EmbeddedNull => new TheoryData<string>
        {
            "a\0b"
        };

        public static TheoryData<string> TestData_EmptyString => new TheoryData<string>
        {
            ""
        };

        public static TheoryData<string> TestData_ControlChars => new TheoryData<string>
        {
            "\t",
            "\r\n",
            "\b",
            "\v",
            "\n"
        };

        public static TheoryData<string> TestData_NonDriveColonPaths => new TheoryData<string>
        {
            @"bad:path",
            @"C:\some\bad:path",
            @"http://www.microsoft.com",
            @"file://www.microsoft.com",
            @"bad::$DATA",
            @"C  :",
            @"C  :\somedir"
        };

        public static TheoryData<string> TestData_Spaces => new TheoryData<string>
        {
            " ",
            "   "
        };

        public static TheoryData<string> TestData_Periods => new TheoryData<string>
        {
            // One and two periods have special meaning (current and parent dir)
            "...",
            "...."
        };

        public static TheoryData<string> TestData_Wildcards => new TheoryData<string>
        {
            "*",
            "?"
        };

        public static TheoryData<string> TestData_ExtendedWildcards => new TheoryData<string>
        {
            // These are supported by Windows although .NET blocked them historically
            "\"",
            "<",
            ">"
        };

        public static TheoryData<string> TestData_UnicodeWhiteSpace => new TheoryData<string>
        {
            "\u00A0", // Non-breaking Space
            "\u2028", // Line separator
            "\u2029", // Paragraph separator
        };

        public static TheoryData<string> TestData_InvalidUnc => new TheoryData<string>
        {
            // .NET used to validate properly formed UNCs
            @"\\",
            @"\\LOCALHOST",
            @"\\LOCALHOST\",
            @"\\LOCALHOST\\",
            @"\\LOCALHOST\.."
        };

        public static TheoryData<string> TestData_InvalidDriveLetters => new TheoryData<string>
        {
            { @"@:\foo" },  // 064 = @     065 = A
            { @"[:\\" },    // 091 = [     090 = Z
            { @"`:\foo "},  // 096 = `     097 = a
            { @"{:\\" },    // 123 = {     122 = z
            { @"@:/foo" },
            { @"[://" },
            { @"`:/foo "},
            { @"{:/" },
            { @"]:" }
        };

        public static TheoryData<string> TestData_ValidDriveLetters => new TheoryData<string>
        {
            { @"A:\foo" },  // 064 = @     065 = A
            { @"Z:\\" },    // 091 = [     090 = Z
            { @"a:\foo "},  // 096 = `     097 = a
            { @"z:\\" },    // 123 = {     122 = z
            { @"B:/foo" },
            { @"D://" },
            { @"E:/foo "},
            { @"F:/" },
            { @"G:" }
        };

        public static TheoryData<string, string> TestData_GetDirectoryName => new TheoryData<string, string>
        {
            { ".", "" },
            { "..", "" },
            { "baz", "" },
            { Path.Combine("dir", "baz"), "dir" },
            { "dir.foo" + Path.AltDirectorySeparatorChar + "baz.txt", "dir.foo" },
            { Path.Combine("dir", "baz", "bar"), Path.Combine("dir", "baz") },
            { Path.Combine("..", "..", "files.txt"), Path.Combine("..", "..") },
            { Path.DirectorySeparatorChar + "foo", Path.DirectorySeparatorChar.ToString() },
            { Path.DirectorySeparatorChar.ToString(), null }
        };

        public static TheoryData<string, string> TestData_GetDirectoryName_Windows => new TheoryData<string, string>
        {
            { @"C:\", null },
            { @"C:/", null },
            { @"C:", null },
            { @"dir\\baz", "dir" },
            { @"dir//baz", "dir" },
            { @"C:\foo", @"C:\" },
            { @"C:foo", "C:" }
        };

        public static TheoryData<string, string> TestData_GetExtension => new TheoryData<string, string>
        {
            { @"file.exe", ".exe" },
            { @"file", "" },
            { @"file.", "" },
            { @"file.s", ".s" },
            { @"test/file", "" },
            { @"test/file.extension", ".extension" },
            { @"test\file", "" },
            { @"test\file.extension", ".extension" },
            { "file.e xe", ".e xe"},
            { "file. ", ". "},
            { " file. ", ". "},
            { " file.extension", ".extension"}
        };

        public static TheoryData<string, string> TestData_GetFileName => new TheoryData<string, string>
        {
            { ".", "." },
            { "..", ".." },
            { "file", "file" },
            { "file.", "file." },
            { "file.exe", "file.exe" },
            { " . ", " . " },
            { " .. ", " .. " },
            { "fi le", "fi le" },
            { Path.Combine("baz", "file.exe"), "file.exe" },
            { Path.Combine("baz", "file.exe") + Path.AltDirectorySeparatorChar, "" },
            { Path.Combine("bar", "baz", "file.exe"), "file.exe" },
            { Path.Combine("bar", "baz", "file.exe") + Path.DirectorySeparatorChar, "" }
        };

        public static TheoryData<string, string> TestData_GetFileNameWithoutExtension => new TheoryData<string, string>
        {
            { "", "" },
            { "file", "file" },
            { "file.exe", "file" },
            { Path.Combine("bar", "baz", "file.exe"), "file" },
            { Path.Combine("bar", "baz") + Path.DirectorySeparatorChar, "" }
        };

        public static TheoryData<string, string> TestData_GetPathRoot_Unc => new TheoryData<string, string>
        {
            { @"\\test\unc\path\to\something", @"\\test\unc" },
            { @"\\a\b\c\d\e", @"\\a\b" },
            { @"\\a\b\", @"\\a\b" },
            { @"\\a\b", @"\\a\b" },
            { @"\\test\unc", @"\\test\unc" },
        };

        // TODO: Include \\.\ as well
        public static TheoryData<string, string> TestData_GetPathRoot_DevicePaths => new TheoryData<string, string>
        {
            { @"\\?\UNC\test\unc\path\to\something", PathFeatures.IsUsingLegacyPathNormalization() ? @"\\?\UNC" : @"\\?\UNC\test\unc" },
            { @"\\?\UNC\test\unc", PathFeatures.IsUsingLegacyPathNormalization() ? @"\\?\UNC" : @"\\?\UNC\test\unc" },
            { @"\\?\UNC\a\b1", PathFeatures.IsUsingLegacyPathNormalization() ? @"\\?\UNC" : @"\\?\UNC\a\b1" },
            { @"\\?\UNC\a\b2\", PathFeatures.IsUsingLegacyPathNormalization() ? @"\\?\UNC" : @"\\?\UNC\a\b2" },
            { @"\\?\C:\foo\bar.txt", PathFeatures.IsUsingLegacyPathNormalization() ? @"\\?\C:" : @"\\?\C:\" }
        };

        public static TheoryData<string, string> TestData_GetPathRoot_Windows => new TheoryData<string, string>
        {
            { @"C:", @"C:" },
            { @"C:\", @"C:\" },
            { @"C:\\", @"C:\" },
            { @"C:\foo1", @"C:\" },
            { @"C:\\foo2", @"C:\" },
        };

        public static TheoryData<string, string, string> GetFullPath_Windows_NonFullyQualified => new TheoryData<string, string, string>
        {
            { @"C:\git\corefx", @"C:\git\corefx", @"C:\git\corefx" },
            { @"C:\git\corefx.\.\.\.\.\.", @"C:\git\corefx", @"C:\git\corefx" },
            { @"C:\git\corefx" + new string(Path.DirectorySeparatorChar, 3) + ".", @"C:\git\corefx", @"C:\git\corefx" },
            { @"C:\git\corefx\..\" + Path.GetFileName(@"C:\git\corefx") + @"\.\..\" + Path.GetFileName(@"C:\git\corefx"), @"C:\git\corefx", @"C:\git\corefx" },
            { @"C:\somedir\..", @"C:\git\corefx", @"C:\" },
            { @"C:\", @"C:\git\corefx", @"C:\" },
            { @"..\..\..\..", @"C:\git\corefx", @"C:\" },
            { @"C:\" + new string(Path.DirectorySeparatorChar, 3), @"C:\git\corefx", @"C:\" },
        };

        public static TheoryData<string, string, string> GetFullPath_CommonUnRootedWindowsData => new TheoryData<string, string, string>
        {
            { "", @"C:\git\corefx", @"C:\git\corefx" },
            { "..", @"C:\git\corefx", Path.GetDirectoryName(@"C:\git\corefx") },

            // Current drive rooted
            { @"\tmp\bar", @"C:\git\corefx", @"C:\tmp\bar" },
            { @"\.\bar", @"C:\git\corefx", @"C:\bar" },
            { @"\tmp\..", @"C:\git\corefx", @"C:\" }, 
            { @"\tmp\bar\..", @"C:\git\corefx", @"C:\tmp" },
            { @"\tmp\bar\..", @"C:\git\corefx", @"C:\tmp" },
            { @"\", @"C:\git\corefx", @"C:\" },

            // Specific drive rooted
            { @"C:tmp\foo\..", @"C:\git\corefx", @"C:\git\corefx\tmp" },
            { @"C:tmp\foo\.", @"C:\git\corefx", @"C:\git\corefx\tmp\foo" },
            { @"C:tmp\foo\..", @"C:\git\corefx", @"C:\git\corefx\tmp" },
            { @"C:tmp", @"C:\git\corefx", @"C:\git\corefx\tmp" },
            { @"C:", @"C:\git\corefx", @"C:\git\corefx" },
            { @"C", @"C:\git\corefx", @"C:\git\corefx\C" },

            { @"Z:tmp\foo\..", @"C:\git\corefx", @"Z:\tmp" },
            { @"Z:tmp\foo\.", @"C:\git\corefx", @"Z:\tmp\foo" },
            { @"Z:tmp\foo\..", @"C:\git\corefx", @"Z:\tmp" },
            { @"Z:tmp", @"C:\git\corefx", @"Z:\tmp" },
            { @"Z:", @"C:\git\corefx", @"Z:\" },
            { @"Z", @"C:\git\corefx", @"C:\git\corefx" + Path.DirectorySeparatorChar + @"Z" }
        };


        public static TheoryData<string, string, string> GetFullPath_Windows_UNC => new TheoryData<string, string, string>
        {
            { @"foo", @"", @"foo" },
            { @"foo", @"server1", @"server1\foo" },
            { @"\foo", @"server2", @"server2\foo" },
            { @"foo", @"server3\", @"server3\foo" },
            //{ @"foo", @"server3\\", @"server3\\foo" },
            { @"..\foo", @"server4", @"server4\..\foo" },
            { @".\foo", @"server5\share", @"server5\share\foo" },
            //{ @"..\foo", @"server6\share", @"server6\foo" },
            { @"\foo", @"a\b\\", @"a\b\foo" },            
            { @"foo", @"LOCALHOST\share8\test.txt.~SS", @"LOCALHOST\share8\test.txt.~SS\foo" },
            { @"foo", @"LOCALHOST\share9", @"LOCALHOST\share9\foo" },
            { @"foo", @"LOCALHOST\shareA\dir", @"LOCALHOST\shareA\dir\foo" },
            { @". \foo", @"LOCALHOST\shareB\", @"LOCALHOST\shareB\. \foo" },
            { @".. \foo", @"LOCALHOST\shareC\", @"LOCALHOST\shareC\.. \foo" },
            { @"    \foo", @"LOCALHOST\shareD\", @"LOCALHOST\shareD\    \foo" },

            { "foo", @"LOCALHOST\  shareE\", @"LOCALHOST\  shareE\foo" },
            { "foo", @"LOCALHOST\shareF\test.txt.~SS", @"LOCALHOST\shareF\test.txt.~SS\foo" },
            { "foo", @"LOCALHOST\shareG", @"LOCALHOST\shareG\foo" },
            { "foo", @"LOCALHOST\shareH\dir", @"LOCALHOST\shareH\dir\foo" },
            { "foo", @"LOCALHOST\shareK\", @"LOCALHOST\shareK\foo" },
            { "foo", @"LOCALHOST\  shareL\", @"LOCALHOST\  shareL\foo" },
        };

        public static TheoryData<string, string, string> GetFullPath_Windows_CommonDevicePaths => new TheoryData<string, string, string>
        {
            // Device paths           
            { "foo", @"C:\ ", @"C:\ \foo" },
            { @" \ \foo", @"C:\", @"C:\ \ \foo" },
            { @" .\foo", @"C:\", @"C:\ .\foo" },
            { @" ..\foo", @"C:\", @"C:\ ..\foo" },
            { @"...\foo", @"C:\", @"C:\...\foo" },

            { @"foo", @"C:\\", @"C:\foo" },
            { @"|\foo", @"C:\", @"C:\|\foo" },
            { @".\foo", @"C:\", @"C:\foo" },
            { @"..\foo", @"C:\", @"foo" },

            { @"\Foo1\.\foo", @"C:\", @"C:\Foo1\foo" },
            { @"\Foo2\..\foo", @"C:\", @"C:\foo" },

            { @"foo", @"GLOBALROOT\", @"GLOBALROOT\foo" },
            { @"foo", @"", @"foo" },
            { @".\foo", @"", @".\foo" },
            { @"..\foo", @"", @"..\foo" },
            { @"C:", @"", @"C:\"},
        };

        public static TheoryData<string, string, string> GetFullPath_Windows_FullyQualified_Diff => new TheoryData<string, string, string>
        {
            // Path argument is a device path.
            // Device Paths with \\?\ wont get normalized i.e. relative segments wont get removed.
            { @"\\?\C:\git\corefx.\.\.\.\.\.", @"\\?\C:\git\corefx", @"\\?\C:\git\corefx.\.\.\.\.\." },
            { @"\\?\C:\git\corefx\\\" + ".", @"\\?\C:\git\corefx", @"\\?\C:\git\corefx\\\." },
            { @"\\?\C:\git\corefx\..\" + Path.GetFileName(@"\\?\C:\git\corefx") + @"\.\..\" + Path.GetFileName(@"\\?\C:\git\corefx"), @"\\?\C:\git\corefx", @"\\?\C:\git\corefx\..\corefx\.\..\corefx" },
            { @"\\?\\somedir\..", @"\\?\C:\git\corefx", @"\\?\\somedir\.." },
            { @"\\?\", @"\\?\C:\git\corefx", @"\\?\" },
            { @"\\?\..\..\..\..", @"\\?\C:\git\corefx", @"\\?\..\..\..\.." },
            { @"\\?\" + new string(Path.DirectorySeparatorChar, 3), @"\\?\C:\git\corefx", @"\\?\\\\" },

            { @"\\.\C:\git\corefx.\.\.\.\.\.", @"\\.\C:\git\corefx", @"\\.\C:\git\corefx" },
            { @"\\.\C:\git\corefx\\\" + ".", @"\\.\C:\git\corefx", @"\\.\C:\git\corefx" },
            { @"\\.\C:\git\corefx\..\" + Path.GetFileName(@"\\.\C:\git\corefx") + @"\.\..\" + Path.GetFileName(@"\\.\C:\git\corefx"), @"\\.\C:\git\corefx", @"\\.\C:\git\corefx" },
            { @"\\.\\somedir\..", @"\\.\C:\git\corefx", @"\\.\" },
            { @"\\.\", @"\\.\C:\git\corefx", @"\\.\" },
            { @"\\.\..\..\..\..", @"\\.\C:\git\corefx", @"\\.\" },
            { @"\\.\" + new string(Path.DirectorySeparatorChar, 3), @"\\.\C:\git\corefx", @"\\.\" },
        };

        public static TheoryData<string, string, string> GetFullPath_BasePath_BasicExpansions_TestData_Unix => new TheoryData<string, string, string>
        {
            { @"/home/git", @"/home/git", @"/home/git" },
            { "", @"/home/git", @"/home/git" },
            { "..", @"/home/git", Path.GetDirectoryName(@"/home/git") },
            { Path.Combine(@"/home/git", ".", ".", ".", ".", "."), @"/home/git", @"/home/git" },
            { @"/home/git" + new string(Path.DirectorySeparatorChar, 3) + ".", @"/home/git", @"/home/git" },
            { Path.Combine(@"/home/git", "..", Path.GetFileName(@"/home/git"), ".", "..", Path.GetFileName(@"/home/git")), @"/home/git", @"/home/git" },
            { Path.Combine(Path.GetPathRoot(@"/home/git"), "somedir", ".."), @"/home/git", Path.GetPathRoot(@"/home/git") },
            { Path.Combine(Path.GetPathRoot(@"/home/git"), "."), @"/home/git", Path.GetPathRoot(@"/home/git") },
            { Path.Combine(Path.GetPathRoot(@"/home/git"), "..", "..", "..", ".."), @"/home/git", Path.GetPathRoot(@"/home/git") },
            { Path.GetPathRoot(@"/home/git") + new string(Path.DirectorySeparatorChar, 3), @"/home/git", Path.GetPathRoot(@"/home/git") },
            { "tmp", @"/home/git", @"/home/git" + "/tmp" },
            { "tmp/bar/..", @"/home/git", @"/home/git" + "/tmp" },
            { "tmp/..", @"/home/git", @"/home/git" },
            { "tmp/./bar/../", @"/home/git", @"/home/git" + "/tmp/" },
            { "tmp/bar/../../", @"/home/git", @"/home/git" + "/" },
            { "tmp/bar/../next/../", @"/home/git", @"/home/git" + "/tmp/" },
            { "tmp/bar/next", @"/home/git", @"/home/git" + "/tmp/bar/next" },

            // Current drive rooted
            { @"/tmp/bar", @"/home/git", @"/tmp/bar" },
            { @"/bar", @"/home/git", @"/bar" },
            { @"/tmp/..", @"/home/git", @"/" },
            { @"/tmp/bar/..", @"/home/git", @"/tmp" },
            { @"/tmp/..", @"/home/git", @"/" },
            { @"/", @"/home/git", @"/" },
        };

        public static TheoryData<string, string, string> GetFullPathBasePath_ArgumentNullException => new TheoryData<string, string, string>
        {
            { @"", null, "basePath" },
            { @"tmp",null, "basePath" },
            { @"\home", null, "basePath"},
            { null, @"foo\bar", "path"},
            { null, @"foo\bar", "path"},
        };

        public static TheoryData<string, string, string> GetFullPathBasePath_ArgumentException => new TheoryData<string, string, string>
        {
            { @"", @"foo\bar", "basePath"},
            { @"tmp", @"foo\bar", "basePath"},
            { @"\home", @"foo\bar", "basePath"},
            { "/gi\0t", @"C:\foo\bar", null }
        };

        protected static void GetTempPath_SetEnvVar(string envVar, string expected, string newTempPath)
        {
            string original = Path.GetTempPath();
            Assert.NotNull(original);
            try
            {
                Environment.SetEnvironmentVariable(envVar, newTempPath);
                Assert.Equal(
                    Path.GetFullPath(expected),
                    Path.GetFullPath(Path.GetTempPath()));
            }
            finally
            {
                Environment.SetEnvironmentVariable(envVar, original);
                Assert.Equal(original, Path.GetTempPath());
            }
        }
    }
}
