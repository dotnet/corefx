// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.IO.Tests
{
    public class File_Changed_Tests : FileSystemWatcherTest
    {
        [Theory]
        [InlineData(WatcherChangeTypes.Changed, true)]
        [InlineData(WatcherChangeTypes.Created, false)]
        [InlineData(WatcherChangeTypes.Deleted, false)]
        [InlineData(WatcherChangeTypes.Renamed, false)]
        //[ActiveIssue(2011, PlatformID.OSX)]
        public void FileSystemWatcher_File_Changed_LastWrite(WatcherChangeTypes eventType, bool raisesEvent)
        {
            using (var testDirectory = new TempDirectory(GetTestFilePath()))
            using (var file = new TempFile(Path.Combine(testDirectory.Path, "file")))
            using (var watcher = new FileSystemWatcher(testDirectory.Path, Path.GetFileName(file.Path)))
            {
                Action action = () => Directory.SetLastWriteTime(file.Path, DateTime.Now + TimeSpan.FromSeconds(10));

                if (raisesEvent)
                    ExpectEvent(watcher, eventType, action);
                else
                    ExpectNoEvent(watcher, eventType, action);
            }
        }

        [Theory]
        [InlineData(WatcherChangeTypes.Changed, true, true)]
        [InlineData(WatcherChangeTypes.Created, false, true)]
        [InlineData(WatcherChangeTypes.Deleted, false, true)]
        [InlineData(WatcherChangeTypes.Renamed, false, true)]
        [InlineData(WatcherChangeTypes.Changed, false, false)]
        [InlineData(WatcherChangeTypes.Created, false, false)]
        [InlineData(WatcherChangeTypes.Deleted, false, false)]
        [InlineData(WatcherChangeTypes.Renamed, false, false)]
        public void FileSystemWatcher_File_Changed_Nested(WatcherChangeTypes eventType, bool raisesEvent, bool includeSubdirectories)
        {
            using (var dir = new TempDirectory(GetTestFilePath()))
            using (var watcher = new FileSystemWatcher(dir.Path, "*"))
            using (var firstDir = new TempDirectory(Path.Combine(dir.Path, "dir1")))
            using (var nestedDir = new TempDirectory(Path.Combine(firstDir.Path, "nested")))
            using (var nestedFile = new TempFile(Path.Combine(nestedDir.Path, "nestedFile")))
            {
                watcher.IncludeSubdirectories = includeSubdirectories;
                watcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.FileName;

                Action action = () => Directory.SetLastAccessTime(nestedFile.Path, DateTime.Now.Subtract(TimeSpan.FromDays(2)));

                if (raisesEvent)
                    ExpectEvent(watcher, eventType, action);
                else
                    ExpectNoEvent(watcher, eventType, action);
            }
        }

        [Theory]
        [InlineData(WatcherChangeTypes.Changed, true)]
        [InlineData(WatcherChangeTypes.Created, false)]
        [InlineData(WatcherChangeTypes.Deleted, false)]
        [InlineData(WatcherChangeTypes.Renamed, false)]
        //[ActiveIssue(2279)]
        public void FileSystemWatcher_File_Changed_DataModification(WatcherChangeTypes eventType, bool raisesEvent)
        {
            using (var testDirectory = new TempDirectory(GetTestFilePath()))
            using (var dir = new TempDirectory(Path.Combine(testDirectory.Path, "dir")))
            using (var watcher = new FileSystemWatcher(Path.GetFullPath(dir.Path), "*"))
            using (var file = File.Create(Path.Combine(dir.Path, "testfile.txt")))
            {
                watcher.NotifyFilter = NotifyFilters.FileName | NotifyFilters.Size;

                Action action = () =>
                {
                    // Change the nested file and verify we get the changed event
                    byte[] bt = new byte[4096];
                    file.Write(bt, 0, bt.Length);
                    file.Flush();
                };

                if (raisesEvent)
                    ExpectEvent(watcher, eventType, action);
                else
                    ExpectNoEvent(watcher, eventType, action);
            }
        }

        [Theory]
        [InlineData(WatcherChangeTypes.Changed, false)]
        [InlineData(WatcherChangeTypes.Created, false)]
        [InlineData(WatcherChangeTypes.Deleted, false)]
        [InlineData(WatcherChangeTypes.Renamed, false)]
        public void FileSystemWatcher_File_Changed_SymLink(WatcherChangeTypes eventType, bool raisesEvent)
        {
            using (var testDirectory = new TempDirectory(GetTestFilePath()))
            using (var dir = new TempDirectory(Path.Combine(testDirectory.Path, "dir")))
            using (var watcher = new FileSystemWatcher(dir.Path, "*"))
            using (var file = new TempFile(GetTestFilePath()))
            {
                watcher.NotifyFilter = NotifyFilters.FileName | NotifyFilters.Size;
                Assert.True(CreateSymLink(file.Path, Path.Combine(dir.Path, "link"), false));

                Action action = () => File.WriteAllText(file.Path, "longtext");
                Action cleanup = () => File.WriteAllText(file.Path, "short");

                if (raisesEvent)
                    ExpectEvent(watcher, eventType, action, cleanup);
                else
                    ExpectNoEvent(watcher, eventType, action, cleanup);
            }
        }
    }
}
