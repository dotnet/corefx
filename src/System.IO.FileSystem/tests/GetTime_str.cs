// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.IO;
using System.Collections;
using System.Globalization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using Xunit;

namespace System.IO.FileSystem.Tests
{
    public abstract class GetTime_str : FileSystemTest
    {
        private readonly string NonExistantFileName = "nonexistant_file_name";
        private readonly TimeSpan CreateCheckGap = TimeSpan.FromMinutes(8);

        protected abstract DateTime m_Get(string path);

        [Fact]
        public void FileNotFound()
        {
            // If the directory described in the path parameter does not exist,
            // this method returns 12:00 midnight, January 1, 1601 A.D. (C.E.)
            // Coordinated Universal Time (UTC), adjusted to local time.
            DateTime time = m_Get(NonExistantFileName);

            Assert.Equal(time, new DateTime(1601, 1, 1, 0, 0, 0, DateTimeKind.Utc).ToLocalTime());
        }

        [Fact]
        public void PathArgumentWhitespace()
        {
            // Check ArgumentException is thrown for only whitespace in path
            Action<string> test = (s) => Assert.Throws<ArgumentException>(() => m_Get(s));

            test("");
            test("    ");
            test("  \t\t  ");
            test("  \t\t\n\n  ");
            test("\t\t");
            test("\n\n");
            test("\x9");
            test("\xA");
            test("\xB");
            test("\xC");
            test("\xD");
            test("\x20");
            test("\x85");
            test("\xA0");
        }

        [Fact]
        public void PathInvalidCharacters()
        {
            // Check ArgumentException is thrown for invalid characters in path
            Action<string> test = (s) => Assert.Throws<ArgumentException>(() => m_Get(s));

            foreach (char c in Path.GetInvalidPathChars())
                test(c.ToString());
            test("?");
            test("*");
        }

        [Fact]
        public void PathArgumentNull()
        {
            // Check ArgumentNullException is thrown for null path argument
            Assert.Throws<ArgumentNullException>(() => m_Get(null));
        }

        [Fact]
        public void PathTooLong()
        {
            // Check PathTooLongException is thrown for too long a path
            StringBuilder sb = new StringBuilder(TestDirectory);
            while (sb.Length < IOInputs.MaxPath + 1)
            {
                sb.Append("a");
            }

            Assert.Throws<PathTooLongException>(() => m_Get(sb.ToString()));
        }

        [Fact]
        public void DirectoryTooLong()
        {
            // Check PathTooLongException is thrown for too long a directory component of path
            StringBuilder sb = new StringBuilder();
            while (sb.Length < IOInputs.MaxDirectory + 1)
            {
                sb.Append("a");
            }

            Assert.Throws<PathTooLongException>(() => m_Get(TestDirectory + Path.DirectorySeparatorChar + sb.ToString()));
        }

        [Fact]
        public void PathJustShortEnough()
        {
            // Check PathTooLongException is not thrown for a path that is just short enough
            StringBuilder sb = new StringBuilder(TestDirectory + Path.DirectorySeparatorChar);
            sb.Append(NonExistantFileName);
            while (sb.Length < IOInputs.MaxPath)
            {
                sb.Append("a");
            }

            m_Get(sb.ToString());
        }

        [Fact]
        public virtual void PositiveTests()
        {
            // Create a new file / directory
            // Verify all of the file times are within CreateCheckGap of DateTime.Now
            TestOnValidFileAndDirectory((path) => 
            {
                DateTime time = m_Get(path);
                Assert.True(CreateCheckGap > time.Subtract(DateTime.Now).Duration());
            });
        }
    }

}
