// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.IO.Tests
{
    public class Directory_Changed_Tests : FileSystemWatcherTest
    {
        [Theory]
        [InlineData(WatcherChangeTypes.Changed, true)]
        [InlineData(WatcherChangeTypes.Created, false)]
        [InlineData(WatcherChangeTypes.Deleted, false)]
        [InlineData(WatcherChangeTypes.Renamed, false)]
        //[ActiveIssue(2011, PlatformID.OSX)]
        public void FileSystemWatcher_Directory_Changed_LastWrite(WatcherChangeTypes eventType, bool raisesEvent)
        {
            using (var testDirectory = new TempDirectory(GetTestFilePath()))
            using (var dir = new TempDirectory(Path.Combine(testDirectory.Path, "dir")))
            using (var watcher = new FileSystemWatcher(testDirectory.Path, Path.GetFileName(dir.Path)))
            {
                Action action = () => Directory.SetLastWriteTime(dir.Path, DateTime.Now + TimeSpan.FromSeconds(10));

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
        //[ActiveIssue(2279)]
        public void FileSystemWatcher_Directory_Changed_WatchedFolder(WatcherChangeTypes eventType, bool raisesEvent)
        {
            using (var testDirectory = new TempDirectory(GetTestFilePath()))
            using (var dir = new TempDirectory(Path.Combine(testDirectory.Path, "dir")))
            using (var watcher = new FileSystemWatcher(dir.Path, "*"))
            {
                Action action = () => Directory.SetLastWriteTime(dir.Path, DateTime.Now + TimeSpan.FromSeconds(10));

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
        public void FileSystemWatcher_Directory_Changed_Nested(WatcherChangeTypes eventType, bool raisesEvent, bool includeSubdirectories)
        {
            using (var dir = new TempDirectory(GetTestFilePath()))
            using (var watcher = new FileSystemWatcher(dir.Path, "*"))
            using (var firstDir = new TempDirectory(Path.Combine(dir.Path, "dir1")))
            using (var nestedDir = new TempDirectory(Path.Combine(firstDir.Path, "nested")))
            {
                watcher.IncludeSubdirectories = includeSubdirectories;
                watcher.NotifyFilter = NotifyFilters.DirectoryName | NotifyFilters.Attributes;

                var attributes = File.GetAttributes(nestedDir.Path);
                Action action = () => File.SetAttributes(nestedDir.Path, attributes | FileAttributes.ReadOnly);
                Action cleanup = () => File.SetAttributes(nestedDir.Path, attributes);

                if (raisesEvent)
                    ExpectEvent(watcher, eventType, action, cleanup);
                else
                    ExpectNoEvent(watcher, eventType, action, cleanup);
            }
        }

        [ConditionalTheory(nameof(CanCreateSymbolicLinks))]
        [InlineData(WatcherChangeTypes.Changed, false)]
        [InlineData(WatcherChangeTypes.Created, false)]
        [InlineData(WatcherChangeTypes.Deleted, false)]
        [InlineData(WatcherChangeTypes.Renamed, false)]
        public void FileSystemWatcher_Directory_Changed_SymLink(WatcherChangeTypes eventType, bool raisesEvent)
        {
            using (var testDirectory = new TempDirectory(GetTestFilePath()))
            using (var dir = new TempDirectory(Path.Combine(testDirectory.Path, "dir")))
            using (var tempDir = new TempDirectory(Path.Combine(testDirectory.Path, "tempDir")))
            using (var watcher = new FileSystemWatcher(dir.Path, "*"))
            using (var file = new TempFile(Path.Combine(tempDir.Path, "test")))
            {
                // Setup the watcher
                watcher.NotifyFilter = NotifyFilters.FileName | NotifyFilters.Size;
                watcher.IncludeSubdirectories = true;
                Assert.True(CreateSymLink(tempDir.Path, Path.Combine(dir.Path, "link"), true));

                Action action = () => File.WriteAllText(file.Path, "longtext");
                Action cleanup = () => File.WriteAllText(file.Path, "short");

                if (raisesEvent)
                    ExpectEvent(watcher, eventType, action, cleanup);
                else
                    ExpectNoEvent(watcher, eventType, action);
            }
        }
    }
}
