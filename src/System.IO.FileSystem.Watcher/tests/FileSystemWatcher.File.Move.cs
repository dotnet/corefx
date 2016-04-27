// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.IO.Tests
{
    public class File_Move_Tests : FileSystemWatcherTest
    {
        #region WindowsTests

        [Theory]
        [InlineData(WatcherChangeTypes.Changed, false)]
        [InlineData(WatcherChangeTypes.Created, false)]
        [InlineData(WatcherChangeTypes.Deleted, false)]
        [InlineData(WatcherChangeTypes.Renamed, true)]
        [PlatformSpecific(PlatformID.Windows)]
        public void Windows_File_Move_To_Same_Directory(WatcherChangeTypes eventType, bool raisesEvent)
        {
            FileMove_SameDirectory(eventType, raisesEvent);
        }

        [Theory]
        [InlineData(WatcherChangeTypes.Changed, false)]
        [InlineData(WatcherChangeTypes.Created, false)]
        [InlineData(WatcherChangeTypes.Deleted, true)]
        [InlineData(WatcherChangeTypes.Renamed, false)]
        [PlatformSpecific(PlatformID.Windows)]
        public void Windows_File_Move_To_Different_Unwatched_Directory(WatcherChangeTypes eventType, bool raisesEvent)
        {
            FileMove_DifferentUnwatchedDirectory(eventType, raisesEvent);
        }

        [Theory]
        [InlineData(WatcherChangeTypes.Changed, true)]
        [InlineData(WatcherChangeTypes.Created, false)]
        [InlineData(WatcherChangeTypes.Deleted, false)]
        [InlineData(WatcherChangeTypes.Renamed, false)]
        [PlatformSpecific(PlatformID.Windows)]
        public void Windows_File_Move_To_Different_Watched_Directory(WatcherChangeTypes eventType, bool raisesEvent)
        {
            FileMove_DifferentWatchedDirectory(eventType, raisesEvent);
        }

        [Theory]
        [InlineData(WatcherChangeTypes.Changed, true)]
        [InlineData(WatcherChangeTypes.Created, false)]
        [InlineData(WatcherChangeTypes.Deleted, false)]
        [InlineData(WatcherChangeTypes.Renamed, true)]
        [PlatformSpecific(PlatformID.Windows)]
        public void Windows_File_Move_In_Nested_Directory(WatcherChangeTypes eventType, bool raisesEvent)
        {
            FileMove_NestedDirectory(eventType, raisesEvent);
        }

        [Theory]
        [InlineData(WatcherChangeTypes.Changed, false)]
        [InlineData(WatcherChangeTypes.Created, false)]
        [InlineData(WatcherChangeTypes.Deleted, false)]
        [InlineData(WatcherChangeTypes.Renamed, true)]
        [PlatformSpecific(PlatformID.Windows)]
        public void Windows_File_Move_With_Set_NotifyFilter(WatcherChangeTypes eventType, bool raisesEvent)
        {
            FileMove_WithNotifyFilter(eventType, raisesEvent);
        }

        #endregion

        #region UnixTests

        [Theory]
        [InlineData(WatcherChangeTypes.Changed, false)]
        [InlineData(WatcherChangeTypes.Created, true)]
        [InlineData(WatcherChangeTypes.Deleted, true)]
        [InlineData(WatcherChangeTypes.Renamed, false)]
        [PlatformSpecific(PlatformID.AnyUnix)]
        public void Unix_File_Move_To_Same_Directory(WatcherChangeTypes eventType, bool raisesEvent)
        {
            FileMove_SameDirectory(eventType, raisesEvent);
        }

        [Theory]
        [InlineData(WatcherChangeTypes.Changed, false)]
        [InlineData(WatcherChangeTypes.Created, false)]
        [InlineData(WatcherChangeTypes.Deleted, true)]
        [InlineData(WatcherChangeTypes.Renamed, false)]
        [PlatformSpecific(PlatformID.AnyUnix)]
        public void Unix_File_Move_To_Different_Unwatched_Directory(WatcherChangeTypes eventType, bool raisesEvent)
        {
            FileMove_DifferentUnwatchedDirectory(eventType, raisesEvent);
        }

        [Theory]
        [InlineData(WatcherChangeTypes.Changed, false)]
        [InlineData(WatcherChangeTypes.Created, false)]
        [InlineData(WatcherChangeTypes.Deleted, false)]
        [InlineData(WatcherChangeTypes.Renamed, true)]
        [PlatformSpecific(PlatformID.AnyUnix)]
        public void Unix_File_Move_To_Different_Watched_Directory(WatcherChangeTypes eventType, bool raisesEvent)
        {
            FileMove_DifferentWatchedDirectory(eventType, raisesEvent);
        }

        [Theory]
        [InlineData(WatcherChangeTypes.Changed, false)]
        [InlineData(WatcherChangeTypes.Created, true)]
        [InlineData(WatcherChangeTypes.Deleted, true)]
        [InlineData(WatcherChangeTypes.Renamed, false)]
        [PlatformSpecific(PlatformID.AnyUnix)]
        //[ActiveIssue(3215, PlatformID.OSX)] // failing for Changed, false
        public void Unix_File_Move_In_Nested_Directory(WatcherChangeTypes eventType, bool raisesEvent)
        {
            FileMove_NestedDirectory(eventType, raisesEvent);
        }

        [Theory]
        [InlineData(WatcherChangeTypes.Changed, false)]
        [InlineData(WatcherChangeTypes.Created, false)]
        [InlineData(WatcherChangeTypes.Deleted, true)]
        [InlineData(WatcherChangeTypes.Renamed, false)]
        [PlatformSpecific(PlatformID.AnyUnix)]
        public void Unix_File_Move_With_Set_NotifyFilter(WatcherChangeTypes eventType, bool raisesEvent)
        {
            FileMove_WithNotifyFilter(eventType, raisesEvent);
        }

        #endregion

        #region Test Helpers

        /// <summary>
        /// Sets up watchers for the type given before performing a File.Move operation and checking for
        /// events. If raisesEvent is true, we make sure that the given event type is observed. If false,
        /// we ensure that it is not observed.
        /// 
        /// This test will move the source file to a destination file in the same directory i.e. rename it
        /// </summary>
        private void FileMove_SameDirectory(WatcherChangeTypes eventType, bool raisesEvent)
        {
            using (var testDirectory = new TempDirectory(GetTestFilePath()))
            using (var dir = new TempDirectory(Path.Combine(testDirectory.Path, "dir")))
            using (var watcher = new FileSystemWatcher(dir.Path, "*.*"))
            using (var testFile = new TempFile(Path.Combine(dir.Path, "file")))
            {
                string sourcePath = testFile.Path;
                string targetPath = testFile.Path + "_" + eventType.ToString();

                // Move the testFile to a different name in the same directory
                Action action = () => File.Move(sourcePath, targetPath);
                Action cleanup = () => File.Move(targetPath, sourcePath);

                // Test that the event is observed or not observed
                if (raisesEvent)
                    ExpectEvent(watcher, eventType, action, cleanup);
                else
                    ExpectNoEvent(watcher, eventType, action, cleanup);
            }
        }

        /// <summary>
        /// Sets up watchers for the type given before performing a File.Move operation and checking for
        /// events. If raisesEvent is true, we make sure that the given event type is observed. If false,
        /// we ensure that it is not observed.
        /// 
        /// This test checks for when the file being moved has a destination directory that is outside of
        /// the path of the FileSystemWatcher.
        /// </summary>
        private void FileMove_DifferentUnwatchedDirectory(WatcherChangeTypes eventType, bool raisesEvent)
        {
            using (var testDirectory = new TempDirectory(GetTestFilePath()))
            using (var dir = new TempDirectory(Path.Combine(testDirectory.Path, "dir")))
            using (var dir_unwatched = new TempDirectory(Path.Combine(testDirectory.Path, "dir_unwatched")))
            using (var watcher = new FileSystemWatcher(dir.Path, "*.*"))
            using (var testFile = new TempFile(Path.Combine(dir.Path, "file")))
            {
                string sourcePath = testFile.Path;
                string targetPath = Path.Combine(dir_unwatched.Path, Path.GetFileName(testFile.Path) + "_" + eventType.ToString());

                // Move the testFile to a different directory not under the Watcher
                Action action = () => File.Move(sourcePath, targetPath);
                Action cleanup = () => File.Move(targetPath, sourcePath);

                // Test that the event is observed or not observed
                if (raisesEvent)
                    ExpectEvent(watcher, eventType, action, cleanup);
                else
                    ExpectNoEvent(watcher, eventType, action, cleanup);
            }
        }

        /// <summary>
        /// Sets up watchers for the type given before performing a File.Move operation and checking for
        /// events. If raisesEvent is true, we make sure that the given event type is observed. If false,
        /// we ensure that it is not observed.
        /// 
        /// This test checks for when the file being moved has a destination directory that is inside the
        /// bounds of the watcher but parallel to its original directory.
        /// </summary>
        private void FileMove_DifferentWatchedDirectory(WatcherChangeTypes eventType, bool raisesEvent)
        {
            using (var testDirectory = new TempDirectory(GetTestFilePath()))
            using (var dir = new TempDirectory(Path.Combine(testDirectory.Path, "dir")))
            using (var dir_adjacent = new TempDirectory(Path.Combine(testDirectory.Path, "dir_adj")))
            using (var watcher = new FileSystemWatcher(testDirectory.Path, "*.*"))
            using (var testFile = new TempFile(Path.Combine(dir.Path, "file")))
            {
                string sourcePath = testFile.Path;
                string targetPath = Path.Combine(dir_adjacent.Path, Path.GetFileName(testFile.Path) + "_" + eventType.ToString());

                // Move the testFile to a different directory under the Watcher
                Action action = () => File.Move(sourcePath, targetPath);
                Action cleanup = () => File.Move(targetPath, sourcePath);

                // Test that the event is observed or not observed
                if (raisesEvent)
                    ExpectEvent(watcher, eventType, action, cleanup);
                else
                    ExpectNoEvent(watcher, eventType, action, cleanup);
            }
        }

        /// <summary>
        /// Sets up watchers for the type given before performing a File.Move operation and checking for
        /// events. If raisesEvent is true, we make sure that the given event type is observed. If false,
        /// we ensure that it is not observed.
        /// 
        /// This test will move the source file of a file within a nested directory
        /// </summary>
        private void FileMove_NestedDirectory(WatcherChangeTypes eventType, bool raisesEvent)
        {
            using (var dir = new TempDirectory(GetTestFilePath()))
            using (var watcher = new FileSystemWatcher(dir.Path, "*"))
            using (var firstDir = new TempDirectory(Path.Combine(dir.Path, "dir1")))
            using (var nestedDir = new TempDirectory(Path.Combine(firstDir.Path, "nested")))
            using (var nestedFile = new TempFile(Path.Combine(nestedDir.Path, "nestedFile" + eventType.ToString())))
            {
                watcher.IncludeSubdirectories = true;

                string sourceFileName = nestedFile.Path;
                string targetFileName = nestedFile.Path + "_2";

                // Move the testFile to a different name within the same nested directory
                Action action = () => File.Move(sourceFileName, targetFileName);
                Action cleanup = () => File.Move(targetFileName, sourceFileName);

                // Test that the event is observed or not observed
                if (raisesEvent)
                    ExpectEvent(watcher, eventType, action, cleanup);
                else
                    ExpectNoEvent(watcher, eventType, action, cleanup);
            }
        }

        /// <summary>
        /// Sets up watchers for the type given before performing a File.Move operation and checking for
        /// events. If raisesEvent is true, we make sure that the given event type is observed. If false,
        /// we ensure that it is not observed.
        /// 
        /// This test will use the NotifyFilter attribute of the FileSystemWatcher before the move and subsequent
        /// checks are made.
        /// </summary>
        private void FileMove_WithNotifyFilter(WatcherChangeTypes eventType, bool raisesEvent)
        {
            using (var testDirectory = new TempDirectory(GetTestFilePath()))
            using (var file = new TempFile(Path.Combine(testDirectory.Path, "file")))
            using (var watcher = new FileSystemWatcher(testDirectory.Path, Path.GetFileName(file.Path)))
            {
                watcher.NotifyFilter = NotifyFilters.FileName;
                string sourcePath = file.Path;
                string targetPath = Path.Combine(testDirectory.Path, "target");

                // Move the testFile to a different name under the same directory with active notifyfilters
                Action action = () => File.Move(sourcePath, targetPath);
                Action cleanup = () => File.Move(targetPath, sourcePath);

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