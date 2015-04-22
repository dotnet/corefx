// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;
using Xunit;

namespace EnumerableTests
{
    public class File_FileEnumerableTests : IClassFixture<TestFileSystemEntries>
    {
        private static TestFileSystemEntries s_fixture;

        public File_FileEnumerableTests(TestFileSystemEntries fixture)
        {
            s_fixture = fixture;
        }

        [Fact]
        public void TestFileReadAllLinesIterator()
        {
            IEnumerable<string> lines = File.ReadLines(s_fixture.TestFilePath);
            foreach (var line in lines)
            {
            }

            try
            {
                foreach (var line in lines)
                { 
                }
            }
            catch(ObjectDisposedException)
            {
                // Currently the test is not hooked to report in case of failures. It is best if we rethrow an exception until we rewrite the harness
                Console.WriteLine("We should not get an ObjectDisposedException but got one ; Dev10 864262 regressed");
                throw;
            }
            
        }

        [Theory]
        [InlineData(false, false)]
        [InlineData(true, false)]
        [InlineData(false, true)]
        [InlineData(true, true)]
        public void TestFileWriteAllLines(bool useEncodingOverload, bool append)
        {
            const string lineContent = "This is another line";
            const string firstLineContent = "This is the first line";
            string[] expectedContents = Enumerable.Repeat(lineContent, 10).ToArray();

            string file1 = Path.Combine(s_fixture.TestDirectoryPath, "file1.out");

            if (useEncodingOverload)
            {
                if (append)
                {
                    File.WriteAllLines(file1, new string[] { firstLineContent }, Encoding.UTF8);
                    File.AppendAllLines(file1, expectedContents, Encoding.UTF8);
                }
                else
                {
                    File.WriteAllLines(file1, expectedContents, Encoding.UTF8);
                }
            }
            else
            {
                if (append)
                {
                    File.WriteAllLines(file1, new string[] { firstLineContent });
                    File.AppendAllLines(file1, expectedContents);
                }
                else
                {
                    File.WriteAllLines(file1, expectedContents);
                }
            }

            try
            {
                IEnumerable<string> actualContents = null;
                if (useEncodingOverload)
                {
                    actualContents = File.ReadLines(file1, Encoding.UTF8);
                }
                else
                {
                    actualContents = File.ReadLines(file1);
                }

                if (append)
                {
                    Assert.Equal(firstLineContent, actualContents.First());
                    Assert.Equal(expectedContents, actualContents.Skip(1));
                }
                else
                {
                    Assert.Equal(expectedContents, actualContents);
                }
            }
            finally
            {
                File.Delete(file1);
            }
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void TestFileReadAllLinesFast(bool useEncodingOverload)
        {
            string file1 = Path.Combine(s_fixture.TestDirectoryPath, "file1.out");
            const string lineContent = "This is a line of test file content";
            string[] expectedContents = Enumerable.Repeat(lineContent, 10).ToArray();

            if (useEncodingOverload)
            {
                File.WriteAllLines(file1, expectedContents, Encoding.UTF8);
            }
            else
            {
                File.WriteAllLines(file1, expectedContents);
            }

            try
            {
                string[] actualContents = null;
                if (useEncodingOverload)
                {
                    actualContents = File.ReadLines(file1, Encoding.UTF8).ToArray();
                }
                else
                {
                    actualContents = File.ReadLines(file1).ToArray();
                }

                Assert.Equal(expectedContents, actualContents);
            }
            finally
            {
                File.Delete(file1);
            }

            // empty file
            file1 = Path.Combine(s_fixture.TestDirectoryPath, "empty.out");
            FileStream fs = File.Create(file1);
            fs.Dispose();

            try
            {
                if (useEncodingOverload)
                {
                    Assert.Empty(File.ReadLines(file1, Encoding.UTF8));
                }
                else
                {
                    Assert.Empty(File.ReadLines(file1));
                }
            }
            finally
            {
                File.Delete(file1);
            }
        }

        [Fact]
        [PlatformSpecific(PlatformID.Windows)]
        public void TestReadFileWithWeirdPath()
        {
            string notExistsFileName = null;
            string unusedDrive = TestFileSystemEntries.GetUnusedDrive();
            if (unusedDrive != null)
            {
                notExistsFileName = Path.Combine(unusedDrive, Path.Combine("temp", "notExists", "temp.txt")); // just skip otherwise
            }

            // Read exceptions
            if (notExistsFileName != null)
            {
                TestReadFileWithWeirdPath(notExistsFileName, typeof(DirectoryNotFoundException));
            }

            // Write exceptions
            if (notExistsFileName != null)
            {
                TestWriteFileExceptions(notExistsFileName, typeof(DirectoryNotFoundException));
            }
        }

        public static IEnumerable<object[]> WeirdPaths
        {
            get
            {
                string longPath = Path.Combine(new String('a', IOInputs.MaxDirectory), "bbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbb");

                return new[]
                {
                    new object[] { null,         typeof(ArgumentNullException) }, // null path
                    new object[] { string.Empty, typeof(ArgumentException) }, // empty path
                    new object[] { " ",          typeof(ArgumentException) }, // whitespace-only path
                    new object[] { longPath,     typeof(PathTooLongException) }, // PathTooLong
                };
            }
        }

        [Theory]
        [MemberData("WeirdPaths")]
        public void TestReadFileWithWeirdPath(string path, Type expectedExceptionType)
        {
            Assert.Throws(expectedExceptionType, () => File.ReadLines(path));
            Assert.Throws(expectedExceptionType, () => File.ReadLines(path, Encoding.UTF8));
        }

        [Theory]
        [MemberData("WeirdPaths")]
        public void TestWriteFileExceptions(string path, Type expectedExceptionType)
        {
            string[] contents = { "test line 1", "test line 2" };
            Encoding encoding = Encoding.UTF8;

            Assert.Throws(expectedExceptionType, () => File.WriteAllLines(path, contents));
            Assert.Throws(expectedExceptionType, () => File.WriteAllLines(path, contents, encoding));

            Assert.Throws(expectedExceptionType, () => File.AppendAllLines(path, contents));
            Assert.Throws(expectedExceptionType, () => File.AppendAllLines(path, contents, encoding));
        }

        [Fact]
        public void TestWithNullContentOrEncoding()
        {
            string[] contents = { "test line 1", "test line 2" };

            Assert.Throws<ArgumentNullException>(() => File.ReadLines(s_fixture.TestFilePath, encoding: null));

            Assert.Throws<ArgumentNullException>(() => File.WriteAllLines(s_fixture.TestFilePath, contents: null));
            Assert.Throws<ArgumentNullException>(() => File.WriteAllLines(s_fixture.TestFilePath, contents: null, encoding: Encoding.UTF8));
            Assert.Throws<ArgumentNullException>(() => File.WriteAllLines(s_fixture.TestFilePath, contents, encoding: null));

            Assert.Throws<ArgumentNullException>(() => File.AppendAllLines(s_fixture.TestFilePath, contents: null));
            Assert.Throws<ArgumentNullException>(() => File.AppendAllLines(s_fixture.TestFilePath, contents: null, encoding: Encoding.UTF8));
            Assert.Throws<ArgumentNullException>(() => File.AppendAllLines(s_fixture.TestFilePath, contents, encoding: null));
        }
    }
}
