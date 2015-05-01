// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.IO;
using System.Collections;
using System.Globalization;
using System.Text;
using System.Threading;
using System.Runtime.CompilerServices;
using Xunit;

namespace System.IO.FileSystem.Tests
{
    public class Directory_SetLastWriteTime_str_dt : FileSystemTest
    {
        private readonly string NonExistantFileName = "nonexistant_file_name";
        private readonly TimeSpan Accuracy = TimeSpan.FromSeconds(3);

        private void CheckPathArgumentException<T>(string path) where T : Exception
        {
            Assert.Throws<T>(() => Directory.SetLastWriteTime(path, default(DateTime)));
            Assert.Throws<T>(() => Directory.SetLastWriteTime(path, new DateTime(2000, 1, 1, 1, 1, 1, 1, DateTimeKind.Utc)));
            Assert.Throws<T>(() => Directory.SetLastWriteTime(path, new DateTime(1, 1, 1, 1, 1, 1, 1, DateTimeKind.Utc)));
        }

        [Fact]
        public void FileNotFound()
        {
            // Check FileNotFoundException is thrown for nonexistant path argument regardless of lastWriteTime argument
            CheckPathArgumentException<FileNotFoundException>(NonExistantFileName);
        }

        [Fact]
        public void PathArgumentWhitespace()
        {
            // Check ArgumentException is thrown for only whitespace in path argument regardless of lastWriteTime argument
            Action<string> test = (s) => CheckPathArgumentException<ArgumentException>(s);

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
            // Check ArgumentException is thrown for invalid characters in path argument regardless of lastWriteTime argument
            Action<string> test = (s) => CheckPathArgumentException<ArgumentException>(s);

            foreach (char c in Path.GetInvalidPathChars())
                test(c.ToString());
            test("?");
            test("*");
        }

        [Fact]
        public void PathArgumentNull()
        {
            // Check ArgumentNullException is thrown for null path argument regardless of lastWriteTime argument
            CheckPathArgumentException<ArgumentNullException>(null);
        }

        [Fact]
        public void PathTooLong()
        {
            // Check PathTooLongException is thrown for too long a path regardless of lastWriteTime argument
            StringBuilder sb = new StringBuilder(TestDirectory);
            while (sb.Length < IOInputs.MaxPath + 1)
            {
                sb.Append("a");
            }

            CheckPathArgumentException<PathTooLongException>(sb.ToString());
        }

        [Fact]
        public void DirectoryTooLong()
        {
            // Check PathTooLongException is thrown for too long a directory component of path regardless of lastWriteTime argument
            StringBuilder sb = new StringBuilder();
            while (sb.Length < IOInputs.MaxDirectory + 1)
            {
                sb.Append("a");
            }

            CheckPathArgumentException<PathTooLongException>(TestDirectory + Path.DirectorySeparatorChar + sb.ToString());
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

            CheckPathArgumentException<FileNotFoundException>(sb.ToString());
        }

        [Fact]
        public void DateTimeOutOfRange()
        {
            // Check ArgumentOutOfRangeException is thrown for a lastWriteTime argument out of range.
            Action<DateTime> test = (time) => TestOnValidFileAndDirectory((path) => Assert.Throws<ArgumentOutOfRangeException>(() => Directory.SetLastWriteTime(path, time)));
            
            test(new DateTime(1, 1, 1, 1, 1, 1, 1, DateTimeKind.Utc));
            test(new DateTime(1600, 1, 1, 1, 1, 1, 1, DateTimeKind.Utc));
        }

        [Fact]
        public void PositiveTests()
        {
            // Positive tests ensure that the last write time we read is within Accuracy of the last write time we set
            Action<DateTime> test = (time) => TestOnValidFileAndDirectory((path) =>
            {
                Directory.SetLastWriteTime(path, time);
                Assert.True(Accuracy > Directory.GetLastWriteTimeUtc(path).Subtract(time).Duration());
            });

            test(new DateTime(1601, 1, 1, 1, 1, 1, 1, DateTimeKind.Utc));
            test(new DateTime(2000, 1, 1, 1, 1, 1, 1, DateTimeKind.Utc));
            test(new DateTime(2020, 10, 20, 20, 30, 30, 500, DateTimeKind.Utc));
            test(new DateTime(2040, 12, 31, 23, 59, 59, 999, DateTimeKind.Utc));
            test(new DateTime(9999, 1, 1, 1, 1, 1, 1, DateTimeKind.Utc));
        }

        [Fact]
        public void RelativeTimePositiveTests()
        {
            // Positive tests ensure that the last write time we read is within Accuracy of the last write time we set
            Action<DateTime> test = (time) => TestOnValidFileAndDirectory((path) =>
            {
                Directory.SetLastWriteTime(path, time);
                Assert.True(Accuracy > Directory.GetLastWriteTime(path).Subtract(time).Duration());
            });

            test(DateTime.Today);
            test(DateTime.Now);
            test(DateTime.Now.AddDays(1));
            test(DateTime.Now.AddMonths(1));
            test(DateTime.Now.AddYears(1));
            test(DateTime.Now.AddYears(10));
            test(DateTime.Now.AddDays(-1));
            test(DateTime.Now.AddMonths(-1));
            test(DateTime.Now.AddYears(-1));
            test(DateTime.Now.AddYears(-10));
        }
    }
}