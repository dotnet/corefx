// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xunit;

namespace EnumerableTests
{
    public class Directory_EnumerableAPIs_TestWhileEnumerating : IDisposable
    {
        private readonly TestFileSystemEntries _fixture;

        public Directory_EnumerableAPIs_TestWhileEnumerating()
        {
            _fixture = new TestFileSystemEntries();
        }

        [Fact]
        [PlatformSpecific(PlatformID.Windows)]
        public void DoGetDirectories_Add()
        {
            IEnumerable<string> dirs = Directory.EnumerateDirectories(_fixture.TestDirectoryPath, "*", SearchOption.AllDirectories);
            HashSet<string> dirsHs = new HashSet<string>();
            int count = 0;

            foreach (string dir in dirs)
            {
                dirsHs.Add(dir);
                count++;
                if (count == 2)
                    _fixture.ChangeFSAdd();
            }

            foreach (string dir in TestFileSystemEntries.ExpectedDirs_Changed)
            {
                string dirPath = Path.Combine(_fixture.TestDirectoryPath, dir);
                Assert.True(dirsHs.Contains(dirPath), string.Format("Didn't get expected subdirectory: \"{0}\"", dir));
            }
        }

        [Fact]
        [PlatformSpecific(PlatformID.Windows)]
        public void DoGetDirectories_Delete()
        {
            IEnumerable<string> dirs = Directory.EnumerateDirectories(_fixture.TestDirectoryPath, "*", SearchOption.AllDirectories);
            HashSet<string> dirsHs = new HashSet<string>();
            int count = 0;

            foreach (string dir in dirs)
            {
                dirsHs.Add(dir);
                count++;
                if (count == 2)
                    _fixture.ChangeFSDelete();
            }

            foreach (string dir in TestFileSystemEntries.ExpectedDirs_Changed)
            {
                string dirPath = Path.Combine(_fixture.TestDirectoryPath, dir);
                Assert.True(dirsHs.Contains(dirPath), string.Format("Didn't get expected subdirectory: \"{0}\"", dir));
            }
        }

        [Fact]
        [PlatformSpecific(PlatformID.Windows)]
        public void DoGetFiles_Add()
        {
            IEnumerable<string> files = Directory.EnumerateFiles(_fixture.TestDirectoryPath, "*", SearchOption.AllDirectories);
            HashSet<string> filesHs = new HashSet<string>();
            int count = 0;

            foreach (string file in files)
            {
                filesHs.Add(file);
                count++;
                if (count == 2)
                    _fixture.ChangeFSAdd();
            }

            foreach (string file in TestFileSystemEntries.ExpectedFiles_Changed)
            {
                string dirPath = Path.Combine(_fixture.TestDirectoryPath, file);
                Assert.True(filesHs.Contains(dirPath), string.Format("Didn't get expected file: \"{0}\"", file));
            }
        }

        [Fact]
        [PlatformSpecific(PlatformID.Windows)]
        public void DoGetFiles_Delete()
        {
            IEnumerable<string> files = Directory.EnumerateFiles(_fixture.TestDirectoryPath, "*", SearchOption.AllDirectories);
            HashSet<string> filesHs = new HashSet<string>();
            int count = 0;

            foreach (string file in files)
            {
                filesHs.Add(file);
                count++;
                if (count == 2)
                    _fixture.ChangeFSDelete();
            }

            foreach (string file in TestFileSystemEntries.ExpectedFiles_Changed)
            {
                string dirPath = Path.Combine(_fixture.TestDirectoryPath, file);
                Assert.True(filesHs.Contains(dirPath), string.Format("Didn't get expected file: \"{0}\"", file));
            }
        }

        [Fact]
        [PlatformSpecific(PlatformID.Windows)]
        public void DoGetFileSystemInfos_Add()
        {
            IEnumerable<string> fsEntries = Directory.EnumerateFileSystemEntries(_fixture.TestDirectoryPath, "*", SearchOption.AllDirectories);
            HashSet<string> fsEntriesHs = new HashSet<string>();
            int count = 0;

            foreach (string entry in fsEntries)
            {
                fsEntriesHs.Add(entry);
                count++;
                if (count == 2)
                    _fixture.ChangeFSAdd();
            }

            foreach (string entry in TestFileSystemEntries.ExpectedDirs_Changed.Union(TestFileSystemEntries.ExpectedFiles_Changed))
            {
                string dirPath = Path.Combine(_fixture.TestDirectoryPath, entry);
                Assert.True(fsEntriesHs.Contains(dirPath), string.Format("Didn't get expected fs entry: \"{0}\"", entry));
            }
        }

        [Fact]
        [PlatformSpecific(PlatformID.Windows)]
        public void DoGetFileSystemInfos_Delete()
        {
            IEnumerable<String> fsEntries = Directory.EnumerateFileSystemEntries(_fixture.TestDirectoryPath, "*", SearchOption.AllDirectories);
            HashSet<String> fsEntriesHs = new HashSet<string>();
            int count = 0;

            foreach (String d in fsEntries)
            {
                fsEntriesHs.Add(d);
                count++;
                if (count == 2)
                    _fixture.ChangeFSDelete();
            }

            foreach (string entry in TestFileSystemEntries.ExpectedDirs_Changed.Union(TestFileSystemEntries.ExpectedFiles_Changed))
            {
                string dirPath = Path.Combine(_fixture.TestDirectoryPath, entry);
                Assert.True(fsEntriesHs.Contains(dirPath), string.Format("Didn't get expected fs entry: \"{0}\"", entry));
            }
        }

        public void Dispose()
        {
            _fixture.Dispose();
        }
    }
}
