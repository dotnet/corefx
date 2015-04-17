// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xunit;

namespace EnumerableTests
{
    public class DirectoryInfo_EnumerableAPIs_TestWhileEnumerating : IDisposable
    {
        private readonly TestFileSystemEntries _fixture;

        public DirectoryInfo_EnumerableAPIs_TestWhileEnumerating()
        {
            _fixture = new TestFileSystemEntries();
        }

        [Fact]
        [PlatformSpecific(PlatformID.Windows)]
        public void DoGetDirectories_Add()
        {
            DirectoryInfo di = new DirectoryInfo(_fixture.TestDirectoryPath);
            IEnumerable<DirectoryInfo> dis = di.EnumerateDirectories("*", SearchOption.AllDirectories);
            HashSet<string> dirsHs = new HashSet<string>();
            int count = 0;

            foreach (DirectoryInfo d in dis)
            {
                dirsHs.Add(d.FullName);
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
            DirectoryInfo di = new DirectoryInfo(_fixture.TestDirectoryPath);
            IEnumerable<DirectoryInfo> dis = di.EnumerateDirectories("*", SearchOption.AllDirectories);
            HashSet<string> dirsHs = new HashSet<string>();
            int count = 0;

            foreach (DirectoryInfo d in dis)
            {
                dirsHs.Add(d.FullName);
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
            DirectoryInfo di = new DirectoryInfo(_fixture.TestDirectoryPath);
            IEnumerable<FileInfo> fis = di.EnumerateFiles("*", SearchOption.AllDirectories);
            HashSet<string> filesHs = new HashSet<string>();
            int count = 0;

            foreach (FileInfo d in fis)
            {
                filesHs.Add(d.FullName);
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
            DirectoryInfo di = new DirectoryInfo(_fixture.TestDirectoryPath);
            IEnumerable<FileInfo> fis = di.EnumerateFiles("*", SearchOption.AllDirectories);
            HashSet<string> filesHs = new HashSet<string>();
            int count = 0;

            foreach (FileInfo d in fis)
            {
                filesHs.Add(d.FullName);
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
            DirectoryInfo di = new DirectoryInfo(_fixture.TestDirectoryPath);
            IEnumerable<FileSystemInfo> fsEntries = di.EnumerateFileSystemInfos("*", SearchOption.AllDirectories);
            HashSet<string> fsEntriesHs = new HashSet<string>();
            int count = 0;

            foreach (FileSystemInfo d in fsEntries)
            {
                fsEntriesHs.Add(d.FullName);
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
            DirectoryInfo di = new DirectoryInfo(_fixture.TestDirectoryPath);
            IEnumerable<FileSystemInfo> fsEntries = di.EnumerateFileSystemInfos("*", SearchOption.AllDirectories);
            HashSet<string> fsEntriesHs = new HashSet<string>();
            int count = 0;

            foreach (FileSystemInfo d in fsEntries)
            {
                fsEntriesHs.Add(d.FullName);
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
