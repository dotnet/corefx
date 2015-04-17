// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xunit;

namespace EnumerableTests
{
    public class DirectoryInfo_EnumerableTests : IClassFixture<TestFileSystemEntries>
    {
        private static TestFileSystemEntries s_fixture;

        public DirectoryInfo_EnumerableTests(TestFileSystemEntries fixture)
        {
            s_fixture = fixture;
        }

        public static IEnumerable<object[]> GetDirectoriesInvariants
        {
            get
            {
                return new[]
                {
                    new object[] { "*", SearchOption.AllDirectories, TestFileSystemEntries.ExpectedDirs_Deep_FSI },
                    new object[] { "*", SearchOption.TopDirectoryOnly, TestFileSystemEntries.ExpectedDirs_Shallow_FSI },
                    new object[] { ".", SearchOption.TopDirectoryOnly, TestFileSystemEntries.ExpectedDirs_Shallow_FSI },
                    new object[] { "", SearchOption.AllDirectories, new HashSet<FSIEntry>() },
                    new object[] { Path.Combine("lev1_a", "*"), SearchOption.TopDirectoryOnly, TestFileSystemEntries.ExpectedDirs_SubDir_FSI }
                };
            }
        }

        [Theory]
        [MemberData("GetDirectoriesInvariants")]
        public void DoDirectoryGetDirectoriesTest(string searchPattern, SearchOption searchOption, HashSet<FSIEntry> expected)
        {
            DirectoryInfo di = new DirectoryInfo(s_fixture.TestDirectoryPath);
            DirectoryInfo[] dis = di.GetDirectories(searchPattern, searchOption);
            foreach (FSIEntry entry in expected)
            {
                DirectoryInfo dirInfo = dis.FirstOrDefault(x => x.Name == entry.Name);
                Assert.True(dirInfo != null, string.Format("Didn't get expected directory: \"{0}\"", entry.FullName));
                
                string expectedFullName = Path.Combine(s_fixture.TestDirectoryPath, entry.FullName);
                Assert.Equal(expectedFullName, dirInfo.FullName);
                Assert.Equal(entry.ExpectedToString, dirInfo.ToString());
            }
        }

        public static IEnumerable<object[]> GetFilesInvariants
        {
            get
            {
                return new[]
                {
                    new object[] { "*", SearchOption.AllDirectories, TestFileSystemEntries.ExpectedFiles_Deep_FSI },
                    new object[] { "*", SearchOption.TopDirectoryOnly, TestFileSystemEntries.ExpectedFiles_Shallow_FSI },
                    new object[] { ".", SearchOption.TopDirectoryOnly, TestFileSystemEntries.ExpectedFiles_Shallow_FSI },
                    new object[] { "", SearchOption.AllDirectories, new HashSet<FSIEntry>() }
                };
            }
        }

        [Theory]
        [MemberData("GetFilesInvariants")]
        public void DoDirectoryGetFilesTest(string searchPattern, SearchOption searchOption, HashSet<FSIEntry> expected)
        {
            DirectoryInfo di = new DirectoryInfo(s_fixture.TestDirectoryPath);
            FileInfo[] fis = di.GetFiles(searchPattern, searchOption);
            foreach (FSIEntry entry in expected)
            {
                FileInfo dirInfo = fis.FirstOrDefault(x => x.Name == entry.Name);
                Assert.True(dirInfo != null, string.Format("Didn't get expected file: \"{0}\"", entry.FullName));

                string expectedFullName = Path.Combine(s_fixture.TestDirectoryPath, entry.FullName);
                Assert.Equal(expectedFullName, dirInfo.FullName);
                Assert.Equal(entry.ExpectedToString, dirInfo.ToString());
            }
        }

        public static IEnumerable<object[]> EnumerateDirectoriesInvariants
        {
            get
            {
                return new[]
                {
                    new object[] { null, "*", SearchOption.AllDirectories, TestFileSystemEntries.ExpectedDirs_Deep },
                    new object[] { null, "*", SearchOption.TopDirectoryOnly, TestFileSystemEntries.ExpectedDirs_Shallow },
                    new object[] { null, ".", SearchOption.TopDirectoryOnly, TestFileSystemEntries.ExpectedDirs_Shallow },
                    new object[] { null, "", SearchOption.AllDirectories, new HashSet<String>() },
                    new object[] { null, "lev2_*", SearchOption.AllDirectories, TestFileSystemEntries.ExpectedDirs_Lev2SearchPattern },
                    new object[] { null, "lev2_*", SearchOption.TopDirectoryOnly, new HashSet<String>() },
                    new object[] { null, "lev2_f", SearchOption.AllDirectories, TestFileSystemEntries.ExpectedDirs_ExactSearchPattern },
                    new object[] { null, "lev2_f", SearchOption.TopDirectoryOnly, new HashSet<String>() },
                    new object[] { null, Path.Combine("lev1_a", "*"), SearchOption.TopDirectoryOnly, TestFileSystemEntries.ExpectedDirs_SubDir },
                    new object[] { "lev1_c", ".", SearchOption.TopDirectoryOnly, new HashSet<String>() },
                };
            }
        }

        [Theory]
        [MemberData("EnumerateDirectoriesInvariants")]
        public void DoDirectoryInfoGetDirectoriesTest(string subPath, string searchPattern, SearchOption searchOption, HashSet<string> expected)
        {
            string path = subPath == null ? s_fixture.TestDirectoryPath : Path.Combine(s_fixture.TestDirectoryPath, subPath);
            DirectoryInfo di = new DirectoryInfo(path);
            IEnumerable<DirectoryInfo> dis = di.EnumerateDirectories(searchPattern, searchOption);
            HashSet<string> dirsHs = new HashSet<string>(dis.Select(x => x.FullName));
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
        public void DoDirectoryInfoGetFilesTest(string searchPattern, SearchOption searchOption, HashSet<string> expected)
        {
            DirectoryInfo di = new DirectoryInfo(s_fixture.TestDirectoryPath);
            IEnumerable<FileInfo> fis = di.EnumerateFiles(searchPattern, searchOption);
            HashSet<String> filesHs = new HashSet<string>(fis.Select(x => x.FullName));
            foreach (string file in expected)
            {
                string dirPath = Path.Combine(s_fixture.TestDirectoryPath, file);
                Assert.True(filesHs.Contains(dirPath), string.Format("Didn't get expected file: \"{0}\"", file));
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
        public void DoDirectoryInfoGetFileSystemInfosTest(string searchPattern, SearchOption searchOption, HashSet<string> expected)
        {
            DirectoryInfo di = new DirectoryInfo(s_fixture.TestDirectoryPath);
            IEnumerable<FileSystemInfo> fsEntries = di.EnumerateFileSystemInfos(searchPattern, searchOption);
            HashSet<String> fsEntriesHs = new HashSet<string>(fsEntries.Select(x => x.FullName));
            foreach (string file in expected)
            {
                string dirPath = Path.Combine(s_fixture.TestDirectoryPath, file);
                Assert.True(fsEntriesHs.Contains(dirPath), string.Format("Didn't get expected fs entry: \"{0}\"", file));
            }
        }

        [Fact]
        public void TestSillySearchString()
        {
            DirectoryInfo di = new DirectoryInfo(s_fixture.TestDirectoryPath);
            DirectoryInfo[] dis = di.GetDirectories("\n");
            Assert.Empty(dis);
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
        public void SearchPatternNegativeTests(string pattern, Type expectedExceptionType)
        {
            DirectoryInfo di = new DirectoryInfo(s_fixture.TestDirectoryPath);

            Assert.Throws(expectedExceptionType, () => di.GetDirectories(pattern));
            Assert.Throws(expectedExceptionType, () => di.GetDirectories(pattern, SearchOption.AllDirectories));

            Assert.Throws(expectedExceptionType, () => di.GetFiles(pattern));
            Assert.Throws(expectedExceptionType, () => di.GetFiles(pattern, SearchOption.AllDirectories));

            Assert.Throws(expectedExceptionType, () => di.GetFileSystemInfos(pattern));
            Assert.Throws(expectedExceptionType, () => di.GetFileSystemInfos(pattern, SearchOption.AllDirectories));

            Assert.Throws(expectedExceptionType, () => di.EnumerateDirectories(pattern));
            Assert.Throws(expectedExceptionType, () => di.EnumerateDirectories(pattern, SearchOption.AllDirectories));

            Assert.Throws(expectedExceptionType, () => di.EnumerateFiles(pattern));
            Assert.Throws(expectedExceptionType, () => di.EnumerateFiles(pattern, SearchOption.AllDirectories));

            Assert.Throws(expectedExceptionType, () => di.EnumerateFileSystemInfos(pattern));
            Assert.Throws(expectedExceptionType, () => di.EnumerateFileSystemInfos(pattern, SearchOption.AllDirectories));
        }

        [Fact]
        public void SearchOptionOutOfRangeTest()
        {
            DirectoryInfo di = new DirectoryInfo(s_fixture.TestDirectoryPath);
            const string pattern = "*";

            Assert.Throws<ArgumentOutOfRangeException>(() => di.GetFiles(pattern, (SearchOption)42));
            Assert.Throws<ArgumentOutOfRangeException>(() => di.GetDirectories(pattern, (SearchOption)42));
            Assert.Throws<ArgumentOutOfRangeException>(() => di.GetFileSystemInfos(pattern, (SearchOption)42));

            Assert.Throws<ArgumentOutOfRangeException>(() => di.EnumerateFiles(pattern, (SearchOption)42));
            Assert.Throws<ArgumentOutOfRangeException>(() => di.EnumerateDirectories(pattern, (SearchOption)42));
            Assert.Throws<ArgumentOutOfRangeException>(() => di.EnumerateFileSystemInfos(pattern, (SearchOption)42));
        }

        public static IEnumerable<object[]> WeirdPaths
        {
            get
            {
                string whitespace = new string(new[] { (char)0x9, (char)0xA });
                string longPath = Path.Combine(new String('a', IOInputs.MaxDirectory), "bbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbb");
                string invalidChars = "temp" + Path.DirectorySeparatorChar + "fsd\0sdsds";

                return new[]
                {
                    new object[] { null,         typeof(ArgumentNullException) }, // null path
                    new object[] { string.Empty, typeof(ArgumentException) }, // empty path
                    new object[] { whitespace,   typeof(ArgumentException) }, // whitespace-only path
                    new object[] { longPath,     typeof(PathTooLongException) }, // PathTooLong
                    new object[] { invalidChars, typeof(ArgumentException) }, // path invalid chars
                };
            }
        }

        [Theory]
        [MemberData("WeirdPaths")]
        public void TestWeirdPath(string path, Type expectedExceptionType)
        {
            Assert.Throws(expectedExceptionType, () => new DirectoryInfo(path));
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
                TestFilePath(pathNotExists, typeof(DirectoryNotFoundException));
            }
        }

        [Fact]
        public void TestFilePath()
        {
            TestFilePath(s_fixture.TestFilePath, typeof(IOException));
        }

        private static void TestFilePath(string path, Type expectedExceptionType)
        {
            const string pattern = "*";
            DirectoryInfo di = new DirectoryInfo(path);

            Assert.Throws(expectedExceptionType, () => di.GetDirectories());
            Assert.Throws(expectedExceptionType, () => di.GetDirectories(pattern));
            Assert.Throws(expectedExceptionType, () => di.GetDirectories(pattern, SearchOption.AllDirectories));

            Assert.Throws(expectedExceptionType, () => di.GetFiles());
            Assert.Throws(expectedExceptionType, () => di.GetFiles(pattern));
            Assert.Throws(expectedExceptionType, () => di.GetFiles(pattern, SearchOption.AllDirectories));

            Assert.Throws(expectedExceptionType, () => di.GetFileSystemInfos());
            Assert.Throws(expectedExceptionType, () => di.GetFileSystemInfos(pattern));
            Assert.Throws(expectedExceptionType, () => di.GetFileSystemInfos(pattern, SearchOption.AllDirectories));

            Assert.Throws(expectedExceptionType, () => di.EnumerateDirectories());
            Assert.Throws(expectedExceptionType, () => di.EnumerateDirectories(pattern));
            Assert.Throws(expectedExceptionType, () => di.EnumerateDirectories(pattern, SearchOption.AllDirectories));

            Assert.Throws(expectedExceptionType, () => di.EnumerateFiles());
            Assert.Throws(expectedExceptionType, () => di.EnumerateFiles(pattern));
            Assert.Throws(expectedExceptionType, () => di.EnumerateFiles(pattern, SearchOption.AllDirectories));

            Assert.Throws(expectedExceptionType, () => di.EnumerateFileSystemInfos());
            Assert.Throws(expectedExceptionType, () => di.EnumerateFileSystemInfos(pattern));
            Assert.Throws(expectedExceptionType, () => di.EnumerateFileSystemInfos(pattern, SearchOption.AllDirectories));
        }
    }
}
