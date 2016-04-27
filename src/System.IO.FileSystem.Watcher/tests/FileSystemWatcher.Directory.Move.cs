// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.IO.Tests
{
    public class Directory_Move_Tests : FileSystemWatcherTest
    {
        #region WindowsTests

        [Theory]
        [InlineData(WatcherChangeTypes.Changed, false)]
        [InlineData(WatcherChangeTypes.Created, false)]
        [InlineData(WatcherChangeTypes.Deleted, false)]
        [InlineData(WatcherChangeTypes.Renamed, true)]
        [PlatformSpecific(PlatformID.Windows)]
        public void Windows_Directory_Move_To_Same_Directory(WatcherChangeTypes eventType, bool raisesEvent)
        {
            DirectoryMove_SameDirectory(eventType, raisesEvent);
        }

        [Theory]
        [InlineData(WatcherChangeTypes.Changed, false)]
        [InlineData(WatcherChangeTypes.Created, false)]
        [InlineData(WatcherChangeTypes.Deleted, true)]
        [InlineData(WatcherChangeTypes.Renamed, false)]
        [PlatformSpecific(PlatformID.Windows)]
        public void Windows_Directory_Move_To_Different_Unwatched_Directory(WatcherChangeTypes eventType, bool raisesEvent)
        {
            DirectoryMove_DifferentUnwatchedDirectory(eventType, raisesEvent);
        }

        [Theory]
        [InlineData(WatcherChangeTypes.Changed, true)]
        [InlineData(WatcherChangeTypes.Created, false)]
        [InlineData(WatcherChangeTypes.Deleted, false)]
        [InlineData(WatcherChangeTypes.Renamed, false)]
        [PlatformSpecific(PlatformID.Windows)]
        public void Windows_Directory_Move_To_Different_Watched_Directory(WatcherChangeTypes eventType, bool raisesEvent)
        {
            DirectoryMove_DifferentWatchedDirectory(eventType, raisesEvent);
        }

        [Theory]
        [InlineData(WatcherChangeTypes.Changed, false)]
        [InlineData(WatcherChangeTypes.Created, false)]
        [InlineData(WatcherChangeTypes.Deleted, false)]
        [InlineData(WatcherChangeTypes.Renamed, true)]
        [PlatformSpecific(PlatformID.Windows)]
        public void Windows_Directory_Move_From_Unwatched_To_Watched(WatcherChangeTypes eventType, bool raisesEvent)
        {
            DirectoryMove_FromUnwatchedToWatched(eventType, raisesEvent);
        }

        [Theory]
        [InlineData(WatcherChangeTypes.Changed, true, true)]
        [InlineData(WatcherChangeTypes.Created, false, true)]
        [InlineData(WatcherChangeTypes.Deleted, false, true)]
        [InlineData(WatcherChangeTypes.Renamed, true, true)]
        [InlineData(WatcherChangeTypes.Changed, false, false)]
        [InlineData(WatcherChangeTypes.Created, false, false)]
        [InlineData(WatcherChangeTypes.Deleted, false, false)]
        [InlineData(WatcherChangeTypes.Renamed, false, false)]
        [PlatformSpecific(PlatformID.Windows)]
        public void Windows_Directory_Move_In_Nested_Directory(WatcherChangeTypes eventType, bool raisesEvent, bool includeSubdirectories)
        {
            DirectoryMove_NestedDirectory(eventType, raisesEvent, includeSubdirectories);
        }

        [Theory]
        [InlineData(WatcherChangeTypes.Changed, false)]
        [InlineData(WatcherChangeTypes.Created, false)]
        [InlineData(WatcherChangeTypes.Deleted, false)]
        [InlineData(WatcherChangeTypes.Renamed, true)]
        [PlatformSpecific(PlatformID.Windows)]
        public void Windows_Directory_Move_With_Set_NotifyFilter(WatcherChangeTypes eventType, bool raisesEvent)
        {
            DirectoryMove_WithNotifyFilter(eventType, raisesEvent);
        }

        #endregion

        #region UnixTests

        [Theory]
        [InlineData(WatcherChangeTypes.Changed, false)]
        [InlineData(WatcherChangeTypes.Created, false)]
        [InlineData(WatcherChangeTypes.Deleted, false)]
        [InlineData(WatcherChangeTypes.Renamed, true)]
        [PlatformSpecific(PlatformID.AnyUnix)]
        public void Unix_Directory_Move_To_Same_Directory(WatcherChangeTypes eventType, bool raisesEvent)
        {
            DirectoryMove_SameDirectory(eventType, raisesEvent);
        }

        [Theory]
        [InlineData(WatcherChangeTypes.Changed, false)]
        [InlineData(WatcherChangeTypes.Created, false)]
        [InlineData(WatcherChangeTypes.Deleted, true)]
        [InlineData(WatcherChangeTypes.Renamed, false)]
        [PlatformSpecific(PlatformID.AnyUnix)]
        public void Unix_Directory_Move_To_Different_Unwatched_Directory(WatcherChangeTypes eventType, bool raisesEvent)
        {
            DirectoryMove_DifferentUnwatchedDirectory(eventType, raisesEvent);
        }

        [Theory]
        [InlineData(WatcherChangeTypes.Changed, false)]
        [InlineData(WatcherChangeTypes.Created, false)]
        [InlineData(WatcherChangeTypes.Deleted, false)]
        [InlineData(WatcherChangeTypes.Renamed, false)]
        [PlatformSpecific(PlatformID.AnyUnix)]
        public void Unix_Directory_Move_To_Different_Watched_Directory(WatcherChangeTypes eventType, bool raisesEvent)
        {
            DirectoryMove_DifferentWatchedDirectory(eventType, raisesEvent);
        }

        [Theory]
        [InlineData(WatcherChangeTypes.Changed, false)]
        [InlineData(WatcherChangeTypes.Created, false)]
        [InlineData(WatcherChangeTypes.Deleted, false)]
        [InlineData(WatcherChangeTypes.Renamed, true)]
        [PlatformSpecific(PlatformID.AnyUnix)]
        public void Unix_Directory_Move_From_Unwatched_To_Watched(WatcherChangeTypes eventType, bool raisesEvent)
        {
            DirectoryMove_FromUnwatchedToWatched(eventType, raisesEvent);
        }

        [Theory]
        [InlineData(WatcherChangeTypes.Changed, false, true)]
        [InlineData(WatcherChangeTypes.Created, false, true)]
        [InlineData(WatcherChangeTypes.Deleted, false, true)]
        [InlineData(WatcherChangeTypes.Renamed, true, true)]
        [InlineData(WatcherChangeTypes.Changed, false, false)]
        [InlineData(WatcherChangeTypes.Created, false, false)]
        [InlineData(WatcherChangeTypes.Deleted, false, false)]
        [InlineData(WatcherChangeTypes.Renamed, false, false)]
        [PlatformSpecific(PlatformID.AnyUnix)]
        //[ActiveIssue(3215, PlatformID.OSX)] // failing for Changed, false
        public void Unix_Directory_Move_In_Nested_Directory(WatcherChangeTypes eventType, bool raisesEvent, bool includeSubdirectories)
        {
            DirectoryMove_NestedDirectory(eventType, raisesEvent, includeSubdirectories);
        }

        [Theory]
        [InlineData(WatcherChangeTypes.Changed, false)]
        [InlineData(WatcherChangeTypes.Created, false)]
        [InlineData(WatcherChangeTypes.Deleted, false)]
        [InlineData(WatcherChangeTypes.Renamed, true)]
        [PlatformSpecific(PlatformID.AnyUnix)]
        public void Unix_Directory_Move_With_Set_NotifyFilter(WatcherChangeTypes eventType, bool raisesEvent)
        {
            DirectoryMove_WithNotifyFilter(eventType, raisesEvent);
        }

        #endregion

        #region Test Helpers

        private void DirectoryMove_SameDirectory(WatcherChangeTypes eventType, bool raisesEvent)
        {
            using (var testDirectory = new TempDirectory(GetTestFilePath()))
            using (var watcher = new FileSystemWatcher(testDirectory.Path, "*.*"))
            using (var dir = new TempDirectory(Path.Combine(testDirectory.Path, "dir")))
            {
                string sourcePath = dir.Path;
                string targetPath = Path.Combine(testDirectory.Path, "target");

                Action action = () => Directory.Move(sourcePath, targetPath);
                Action cleanup = () => Directory.Move(targetPath, sourcePath);

                // Test that the event is observed or not observed
                if (raisesEvent)
                    ExpectEvent(watcher, eventType, action, cleanup);
                else
                    ExpectNoEvent(watcher, eventType, action, cleanup);
            }
        }

        private void DirectoryMove_DifferentWatchedDirectory(WatcherChangeTypes eventType, bool raisesEvent)
        {
            using (var testDirectory = new TempDirectory(GetTestFilePath()))
            using (var sourceDir = new TempDirectory(Path.Combine(testDirectory.Path, "source")))
            using (var adjacentDir = new TempDirectory(Path.Combine(testDirectory.Path, "adj")))
            using (var dir = new TempDirectory(Path.Combine(sourceDir.Path, "dir")))
            using (var watcher = new FileSystemWatcher(testDirectory.Path, "*.*"))
            {
                string sourcePath = dir.Path;
                string targetPath = Path.Combine(adjacentDir.Path, "target");

                // Move the dir to a different directory under the Watcher
                Action action = () => Directory.Move(sourcePath, targetPath);
                Action cleanup = () => Directory.Move(targetPath, sourcePath);

                // Test that the event is observed or not observed
                if (raisesEvent)
                    ExpectEvent(watcher, eventType, action, cleanup);
                else
                    ExpectNoEvent(watcher, eventType, action, cleanup);
            }
        }

        private void DirectoryMove_DifferentUnwatchedDirectory(WatcherChangeTypes eventType, bool raisesEvent)
        {
            using (var watchedTestDirectory = new TempDirectory(GetTestFilePath()))
            using (var unwatchedTestDirectory = new TempDirectory(GetTestFilePath()))
            using (var dir = new TempDirectory(Path.Combine(watchedTestDirectory.Path, "dir")))
            using (var watcher = new FileSystemWatcher(watchedTestDirectory.Path, "*.*"))
            {
                string sourcePath = dir.Path;
                string targetPath = Path.Combine(unwatchedTestDirectory.Path, "target");

                // Move the dir to a different directory not under the watcher
                Action action = () => Directory.Move(sourcePath, targetPath);
                Action cleanup = () => Directory.Move(targetPath, sourcePath);

                // Test that the event is observed or not observed
                if (raisesEvent)
                    ExpectEvent(watcher, eventType, action, cleanup);
                else
                    ExpectNoEvent(watcher, eventType, action, cleanup);
            }
        }

        private void DirectoryMove_FromUnwatchedToWatched(WatcherChangeTypes eventType, bool raisesEvent)
        {
            using (var watchedTestDirectory = new TempDirectory(GetTestFilePath()))
            using (var unwatchedTestDirectory = new TempDirectory(GetTestFilePath()))
            using (var dir = new TempDirectory(Path.Combine(watchedTestDirectory.Path, "dir")))
            using (var watcher = new FileSystemWatcher(watchedTestDirectory.Path, "*.*"))
            {
                string sourcePath = dir.Path;
                string targetPath = Path.Combine(watchedTestDirectory.Path, "target");

                // Move the dir to a different directory not under the watcher
                Action action = () => Directory.Move(sourcePath, targetPath);
                Action cleanup = () => Directory.Move(targetPath, sourcePath);

                // Test that the event is observed or not observed
                if (raisesEvent)
                    ExpectEvent(watcher, eventType, action, cleanup);
                else
                    ExpectNoEvent(watcher, eventType, action, cleanup);
            }
        }

        private void DirectoryMove_NestedDirectory(WatcherChangeTypes eventType, bool raisesEvent, bool includeSubdirectories)
        {
            using (var dir = new TempDirectory(GetTestFilePath()))
            using (var watcher = new FileSystemWatcher(dir.Path, "*"))
            using (var firstDir = new TempDirectory(Path.Combine(dir.Path, "first")))
            using (var secondDir = new TempDirectory(Path.Combine(firstDir.Path, "second")))
            using (var nestedDir = new TempDirectory(Path.Combine(secondDir.Path, "nested")))
            {
                watcher.IncludeSubdirectories = includeSubdirectories;

                string sourcePath = nestedDir.Path;
                string targetPath = nestedDir.Path + "_2";

                // Move the dir to a different directory within the same nested directory
                Action action = () => Directory.Move(sourcePath, targetPath);
                Action cleanup = () => Directory.Move(targetPath, sourcePath);

                // Test that the event is observed or not observed
                if (raisesEvent)
                    ExpectEvent(watcher, eventType, action, cleanup);
                else
                    ExpectNoEvent(watcher, eventType, action, cleanup);
            }
        }

        private void DirectoryMove_WithNotifyFilter(WatcherChangeTypes eventType, bool raisesEvent)
        {
            using (var testDirectory = new TempDirectory(GetTestFilePath()))
            using (var watcher = new FileSystemWatcher(testDirectory.Path, "*.*"))
            using (var dir = new TempDirectory(Path.Combine(testDirectory.Path, "dir")))
            {
                watcher.NotifyFilter = NotifyFilters.DirectoryName;

                string sourcePath = dir.Path;
                string targetPath = dir.Path + "_2";

                Action action = () => Directory.Move(sourcePath, targetPath);
                Action cleanup = () => Directory.Move(targetPath, sourcePath);

                // Test that the event is observed or not observed
                if (raisesEvent)
                    ExpectEvent(watcher, eventType, action, cleanup);
                else
                    ExpectNoEvent(watcher, eventType, action, cleanup);
            }
        }

        #endregion
    }
}