// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xunit;

namespace EnumerableTests
{
    public class Directory_EnumerableTests : IClassFixture<TestFileSystemEntries>
    {
        private static TestFileSystemEntries s_fixture;

        public Directory_EnumerableTests(TestFileSystemEntries fixture)
        {
            s_fixture = fixture;
        }

        public static IEnumerable<object[]> EnumerateDirectoriesInvariants
        {
            get
            {
                return new[]
                {
                    new object[] { "*", SearchOption.AllDirectories, TestFileSystemEntries.ExpectedDirs_Deep },
                    new object[] { "*", SearchOption.TopDirectoryOnly, TestFileSystemEntries.ExpectedDirs_Shallow },
                    new object[] { ".", SearchOption.TopDirectoryOnly, TestFileSystemEntries.ExpectedDirs_Shallow },
                    new object[] { "", SearchOption.AllDirectories, new HashSet<String>() },
                    new object[] { "lev2_*", SearchOption.AllDirectories, TestFileSystemEntries.ExpectedDirs_Lev2SearchPattern },
                    new object[] { "lev2_*", SearchOption.TopDirectoryOnly, new HashSet<String>() },
                    new object[] { "lev2_f", SearchOption.AllDirectories, TestFileSystemEntries.ExpectedDirs_ExactSearchPattern },
                    new object[] { "lev2_f", SearchOption.TopDirectoryOnly, new HashSet<String>() },
                    new object[] { Path.Combine("lev1_a", "*"), SearchOption.TopDirectoryOnly, TestFileSystemEntries.ExpectedDirs_SubDir }
                };
            }
        }
            
        [Theory]
        [MemberData("EnumerateDirectoriesInvariants")]
        public void DoDirectoryGetDirectoriesTest(string searchPattern, SearchOption searchOption, HashSet<string> expected)
        {
            IEnumerable<string> dirs = Directory.EnumerateDirectories(s_fixture.TestDirectoryPath, searchPattern, searchOption);
            HashSet<String> dirsHs = new HashSet<string>(dirs);
            foreach (string dir in expected)
            {
                string dirPath = Path.Combine(s_fixture.TestDirectoryPath, dir);
                Assert.True(dirsHs.Contains(dirPath), string.Format("Didn't get expected subdirectory: \"{0}\"", dir));
            }
        }

        public static IEnumerable<object[]> EnumerateFilesInvariants
        {
            get
            {
                return new[]
                {
                    new object[] { "*", SearchOption.AllDirectories, TestFileSystemEntries.ExpectedFiles_Deep },
                    new object[] { "*", SearchOption.TopDirectoryOnly, TestFileSystemEntries.ExpectedFiles_Shallow },
                    new object[] { ".", SearchOption.TopDirectoryOnly, TestFileSystemEntries.ExpectedFiles_Shallow },
                    new object[] { "", SearchOption.AllDirectories, new HashSet<String>() },
                    new object[] { "lev2_f", SearchOption.AllDirectories, new HashSet<String>() },
                    new object[] { "lev2_f", SearchOption.TopDirectoryOnly, new HashSet<String>() }
                };
            }
        }

        [Theory]
        [MemberData("EnumerateFilesInvariants")]
        public void DoDirectoryGetFilesTest(string searchPattern, SearchOption searchOption, HashSet<string> expected)
        {
            IEnumerable<String> dirs = Directory.EnumerateFiles(s_fixture.TestDirectoryPath, searchPattern, searchOption);
            HashSet<String> dirsHs = new HashSet<string>(dirs);
            foreach (string file in expected)
            {
                string dirPath = Path.Combine(s_fixture.TestDirectoryPath, file);
                Assert.True(dirsHs.Contains(dirPath), string.Format("Didn't get expected subdirectory: \"{0}\"", file));
            }
        }

        public static IEnumerable<object[]> EnumerateFileSystemEntriesInvariants
        {
            get
            {
                return new[]
                {
                    new object[] { "*", SearchOption.AllDirectories, new HashSet<String>(TestFileSystemEntries.ExpectedFiles_Deep.Union(TestFileSystemEntries.ExpectedDirs_Deep)) },
                    new object[] { "*", SearchOption.TopDirectoryOnly, new HashSet<String>(TestFileSystemEntries.ExpectedFiles_Shallow.Union(TestFileSystemEntries.ExpectedDirs_Shallow)) },
                    new object[] { ".", SearchOption.TopDirectoryOnly, new HashSet<String>(TestFileSystemEntries.ExpectedFiles_Shallow.Union(TestFileSystemEntries.ExpectedDirs_Shallow)) },
                    new object[] { "", SearchOption.AllDirectories, new HashSet<String>() },
                    new object[] { "lev2_f", SearchOption.AllDirectories, TestFileSystemEntries.ExpectedDirs_ExactSearchPattern },
                    new object[] { "lev2_f", SearchOption.TopDirectoryOnly, new HashSet<String>() },
                    new object[] { "file1", SearchOption.AllDirectories, TestFileSystemEntries.ExpectedFiles_Shallow }
                };
            }
        }

        [Theory]
        [MemberData("EnumerateFileSystemEntriesInvariants")]
        public void DoDirectoryGetFileSystemEntriesTest(string searchPattern, SearchOption searchOption, HashSet<string> expected)
        {

            IEnumerable<String> dirs = Directory.EnumerateFileSystemEntries(s_fixture.TestDirectoryPath, searchPattern, searchOption);
            HashSet<String> dirsHs = new HashSet<string>(dirs);
            foreach (string file in expected)
            {
                string dirPath = Path.Combine(s_fixture.TestDirectoryPath, file);
                Assert.True(dirsHs.Contains(dirPath), string.Format("Didn't get expected filesystem entry: \"{0}\"", file));
            }
        }

        [Fact]
        public void SearchOptionOutOfRangeTest()
        {
            string path = s_fixture.TestDirectoryPath;
            const string pattern = "*";

            Assert.Throws<ArgumentOutOfRangeException>(() => Directory.GetDirectories(path, pattern, (SearchOption)42));
            Assert.Throws<ArgumentOutOfRangeException>(() => Directory.GetFiles(path, pattern, (SearchOption)42));
            Assert.Throws<ArgumentOutOfRangeException>(() => Directory.GetFileSystemEntries(path, pattern, (SearchOption)42));

            Assert.Throws<ArgumentOutOfRangeException>(() => Directory.EnumerateDirectories(path, pattern, (SearchOption)42));
            Assert.Throws<ArgumentOutOfRangeException>(() => Directory.EnumerateFiles(path, pattern, (SearchOption)42));
            Assert.Throws<ArgumentOutOfRangeException>(() => Directory.EnumerateFileSystemEntries(path, pattern, (SearchOption)42));
        }

        [Fact]
        [PlatformSpecific(PlatformID.Windows)]
        public void TestWeirdPaths()
        {
            // try to test a path that doesn't exist. Skip if can't find an unused drive
            string pathNotExists = null;
            string unusedDrive = TestFileSystemEntries.GetUnusedDrive();
            if (unusedDrive != null)
            {
                pathNotExists = Path.Combine(unusedDrive, "temp", "dir");
            }
            if (pathNotExists != null)
            {
                TestWeirdPaths(pathNotExists, typeof(DirectoryNotFoundException));
            }
        }

        public static IEnumerable<object[]> WeirdPaths
        {
            get
            {
                string whitespace = new string(new[] { (char)0x9, (char)0xA });
                string filePath = s_fixture.TestFilePath;
                string longPath = Path.Combine(new String('a', IOInputs.MaxDirectory), "bbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbb");
                string invalidChars = "temp" + Path.DirectorySeparatorChar + "fsd\0sdsds";

                return new[]
                {
                    new object[] { null,         typeof(ArgumentNullException) }, // null path
                    new object[] { string.Empty, typeof(ArgumentException) }, // empty path
                    new object[] { whitespace,   typeof(ArgumentException) }, // whitespace-only path
                    new object[] { filePath,     typeof(IOException) }, // file (not dir) name
                    new object[] { longPath,     typeof(PathTooLongException) }, // PathTooLong
                    new object[] { invalidChars, typeof(ArgumentException) }, // path invalid chars
                };
            }
        }

        [Theory]
        [MemberData("WeirdPaths")]
        public void TestWeirdPaths(string path, Type expectedExceptionType)
        {
            const string pattern = "*";
            const SearchOption option = SearchOption.AllDirectories;

            Assert.Throws(expectedExceptionType, () => Directory.GetDirectories(path));
            Assert.Throws(expectedExceptionType, () => Directory.GetDirectories(path, pattern));
            Assert.Throws(expectedExceptionType, () => Directory.GetDirectories(path, pattern, option));

            Assert.Throws(expectedExceptionType, () => Directory.GetFiles(path));
            Assert.Throws(expectedExceptionType, () => Directory.GetFiles(path, pattern));
            Assert.Throws(expectedExceptionType, () => Directory.GetFiles(path, pattern, option));

            Assert.Throws(expectedExceptionType, () => Directory.GetFileSystemEntries(path));
            Assert.Throws(expectedExceptionType, () => Directory.GetFileSystemEntries(path, pattern));
            Assert.Throws(expectedExceptionType, () => Directory.GetFileSystemEntries(path, pattern, option));

            Assert.Throws(expectedExceptionType, () => Directory.EnumerateDirectories(path));
            Assert.Throws(expectedExceptionType, () => Directory.EnumerateDirectories(path, pattern));
            Assert.Throws(expectedExceptionType, () => Directory.EnumerateDirectories(path, pattern, option));

            Assert.Throws(expectedExceptionType, () => Directory.EnumerateFiles(path));
            Assert.Throws(expectedExceptionType, () => Directory.EnumerateFiles(path, pattern));
            Assert.Throws(expectedExceptionType, () => Directory.EnumerateFiles(path, pattern, option));

            Assert.Throws(expectedExceptionType, () => Directory.EnumerateFileSystemEntries(path));
            Assert.Throws(expectedExceptionType, () => Directory.EnumerateFileSystemEntries(path, pattern));
            Assert.Throws(expectedExceptionType, () => Directory.EnumerateFileSystemEntries(path, pattern, option));
        }

        public static IEnumerable<object[]> SearchPatternTests
        {
            get
            {
                return new[]
                {
                    new object[] { null,                               typeof(ArgumentNullException) },
                    new object[] { "..",                               typeof(ArgumentException) },
                    new object[] { ".." + Path.DirectorySeparatorChar, typeof(ArgumentException) }
                };
            }
        }

        [Theory]
        [MemberData("SearchPatternTests")]
        public void SearchPatternNegativeTests(string searchPattern, Type expectedExceptionType)
        {
            string path = s_fixture.TestDirectoryPath;

            Assert.Throws(expectedExceptionType, () => Directory.GetDirectories(path, searchPattern));
            Assert.Throws(expectedExceptionType, () => Directory.GetDirectories(path, searchPattern, SearchOption.AllDirectories));

            Assert.Throws(expectedExceptionType, () => Directory.GetFiles(path, searchPattern));
            Assert.Throws(expectedExceptionType, () => Directory.GetFiles(path, searchPattern, SearchOption.AllDirectories));

            Assert.Throws(expectedExceptionType, () => Directory.GetFileSystemEntries(path, searchPattern));
            Assert.Throws(expectedExceptionType, () => Directory.GetFileSystemEntries(path, searchPattern, SearchOption.AllDirectories));

            Assert.Throws(expectedExceptionType, () => Directory.EnumerateDirectories(path, searchPattern));
            Assert.Throws(expectedExceptionType, () => Directory.EnumerateDirectories(path, searchPattern, SearchOption.AllDirectories));

            Assert.Throws(expectedExceptionType, () => Directory.EnumerateFiles(path, searchPattern));
            Assert.Throws(expectedExceptionType, () => Directory.EnumerateFiles(path, searchPattern, SearchOption.AllDirectories));

            Assert.Throws(expectedExceptionType, () => Directory.EnumerateFileSystemEntries(path, searchPattern));
            Assert.Throws(expectedExceptionType, () => Directory.EnumerateFileSystemEntries(path, searchPattern, SearchOption.AllDirectories));
        }
    }
}
