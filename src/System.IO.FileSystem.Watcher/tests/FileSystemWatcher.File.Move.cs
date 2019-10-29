// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Xunit;

namespace System.IO.Tests
{
    public class File_Move_Tests : FileSystemWatcherTest
    {
        [Fact]
        [PlatformSpecific(TestPlatforms.Windows)]  // Expected WatcherChangeTypes are different based on OS
        public void Windows_File_Move_To_Same_Directory()
        {
            FileMove_SameDirectory(WatcherChangeTypes.Renamed);
        }

        [Fact]
        [PlatformSpecific(TestPlatforms.AnyUnix)]  // Expected WatcherChangeTypes are different based on OS
        public void Unix_File_Move_To_Same_Directory()
        {
            FileMove_SameDirectory(WatcherChangeTypes.Renamed);
        }

        [Fact]
        public void File_Move_From_Watched_To_Unwatched()
        {
            FileMove_FromWatchedToUnwatched(WatcherChangeTypes.Deleted);
        }

        [Theory]
        [PlatformSpecific(TestPlatforms.OSX)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        public void File_Move_Multiple_From_Watched_To_Unwatched_Mac(int filesCount)
        {
            // On Mac, the FSStream aggregate old events caused by the test setup.
            // There is no option how to get rid of it but skip it.
            FileMove_Multiple_FromWatchedToUnwatched(filesCount, skipOldEvents: true);
        }

        [Theory]
        [PlatformSpecific(~TestPlatforms.OSX)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        public void File_Move_From_Watched_To_Unwatched(int filesCount)
        {
            FileMove_Multiple_FromWatchedToUnwatched(filesCount, skipOldEvents: false);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        public void File_Move_Multiple_From_Unwatched_To_WatchedMac(int filesCount)
        {
            FileMove_Multiple_FromUnwatchedToWatched(filesCount);
        }

        [Fact]
        [PlatformSpecific(TestPlatforms.Windows)]  // Expected WatcherChangeTypes are different based on OS
        public void Windows_File_Move_To_Different_Watched_Directory()
        {
            FileMove_DifferentWatchedDirectory(WatcherChangeTypes.Changed);
        }

        [Fact]
        [PlatformSpecific(TestPlatforms.OSX)]  // Expected WatcherChangeTypes are different based on OS
        public void OSX_File_Move_To_Different_Watched_Directory()
        {
            FileMove_DifferentWatchedDirectory(0);
        }

        [Fact]
        [PlatformSpecific(TestPlatforms.Linux)]  // Expected WatcherChangeTypes are different based on OS
        public void Linux_File_Move_To_Different_Watched_Directory()
        {
            FileMove_DifferentWatchedDirectory(0);
        }

        [Fact]
        public void File_Move_From_Unwatched_To_Watched()
        {
            // TODO remove OS version check after https://github.com/dotnet/corefx/issues/40034 fixed
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.OSX) || Environment.OSVersion.Version.Major < 19)
            {
                FileMove_FromUnwatchedToWatched(WatcherChangeTypes.Created);
            }
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        [PlatformSpecific(TestPlatforms.Windows)]  // Expected WatcherChangeTypes are different based on OS
        public void Windows_File_Move_In_Nested_Directory(bool includeSubdirectories)
        {
            FileMove_NestedDirectory(includeSubdirectories ? WatcherChangeTypes.Renamed : 0, includeSubdirectories);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        [PlatformSpecific(TestPlatforms.AnyUnix)]  // Expected WatcherChangeTypes are different based on OS
        public void Unix_File_Move_In_Nested_Directory(bool includeSubdirectories)
        {
            FileMove_NestedDirectory(includeSubdirectories ? WatcherChangeTypes.Renamed : 0, includeSubdirectories);
        }

        [Fact]
        [PlatformSpecific(TestPlatforms.Windows)]  // Expected WatcherChangeTypes are different based on OS
        public void Windows_File_Move_With_Set_NotifyFilter()
        {
            FileMove_WithNotifyFilter(WatcherChangeTypes.Renamed);
        }

        [Fact]
        [PlatformSpecific(TestPlatforms.AnyUnix)]  // Expected WatcherChangeTypes are different based on OS
        public void Unix_File_Move_With_Set_NotifyFilter()
        {
            FileMove_WithNotifyFilter(WatcherChangeTypes.Renamed);
        }

        #region Test Helpers

        private void FileMove_SameDirectory(WatcherChangeTypes eventType)
        {
            using (var testDirectory = new TempDirectory(GetTestFilePath()))
            using (var dir = new TempDirectory(Path.Combine(testDirectory.Path, "dir")))
            using (var testFile = new TempFile(Path.Combine(dir.Path, "file")))
            using (var watcher = new FileSystemWatcher(dir.Path, "*"))
            {
                string sourcePath = testFile.Path;
                string targetPath = testFile.Path + "_" + eventType.ToString();

                // Move the testFile to a different name in the same directory
                Action action = () => File.Move(sourcePath, targetPath);
                Action cleanup = () => File.Move(targetPath, sourcePath);

                if ((eventType & WatcherChangeTypes.Deleted) > 0)
                    ExpectEvent(watcher, eventType, action, cleanup, new string[] { sourcePath, targetPath });
                else
                    ExpectEvent(watcher, eventType, action, cleanup, targetPath);
            }
        }

        private void FileMove_DifferentWatchedDirectory(WatcherChangeTypes eventType)
        {
            using (var testDirectory = new TempDirectory(GetTestFilePath()))
            using (var dir = new TempDirectory(Path.Combine(testDirectory.Path, "dir")))
            using (var dir_adjacent = new TempDirectory(Path.Combine(testDirectory.Path, "dir_adj")))
            using (var testFile = new TempFile(Path.Combine(dir.Path, "file")))
            using (var watcher = new FileSystemWatcher(testDirectory.Path, "*"))
            {
                string sourcePath = testFile.Path;
                string targetPath = Path.Combine(dir_adjacent.Path, Path.GetFileName(testFile.Path) + "_" + eventType.ToString());

                // Move the testFile to a different directory under the Watcher
                Action action = () => File.Move(sourcePath, targetPath);
                Action cleanup = () => File.Move(targetPath, sourcePath);

                ExpectEvent(watcher, eventType, action, cleanup, new string[] { dir.Path, dir_adjacent.Path });
            }
        }

        private void FileMove_FromWatchedToUnwatched(WatcherChangeTypes eventType)
        {
            using (var testDirectory = new TempDirectory(GetTestFilePath()))
            using (var dir_watched = new TempDirectory(Path.Combine(testDirectory.Path, "dir_watched")))
            using (var dir_unwatched = new TempDirectory(Path.Combine(testDirectory.Path, "dir_unwatched")))
            using (var testFile = new TempFile(Path.Combine(dir_watched.Path, "file")))
            using (var watcher = new FileSystemWatcher(dir_watched.Path, "*"))
            {
                string sourcePath = testFile.Path; // watched
                string targetPath = Path.Combine(dir_unwatched.Path, "file");

                Action action = () => File.Move(sourcePath, targetPath);
                Action cleanup = () => File.Move(targetPath, sourcePath);

                ExpectEvent(watcher, eventType, action, cleanup, sourcePath);
            }
        }

        private void FileMove_Multiple_FromWatchedToUnwatched(int filesCount, bool skipOldEvents)
        {
            Assert.InRange(filesCount, 0, int.MaxValue);

            using var testDirectory = new TempDirectory(GetTestFilePath());
            using var watchedTestDirectory = new TempDirectory(Path.Combine(testDirectory.Path, "dir_watched"));
            using var unwatchedTestDirectory = new TempDirectory(Path.Combine(testDirectory.Path, "dir_unwatched"));

            var files = Enumerable.Range(0, filesCount)
                            .Select(i => new
                            {
                                FileInWatchedDir = Path.Combine(watchedTestDirectory.Path, $"file{i}"),
                                FileInUnwatchedDir = Path.Combine(unwatchedTestDirectory.Path, $"file{i}")
                            }).ToArray();

            Array.ForEach(files, (file) => File.Create(file.FileInWatchedDir).Dispose());

            using var watcher = new FileSystemWatcher(watchedTestDirectory.Path, "*");

            Action action = () => Array.ForEach(files, file => File.Move(file.FileInWatchedDir, file.FileInUnwatchedDir));

            // On macOS, for each file we receive two events as describe in comment below.
            int expectEvents = filesCount;
            if (skipOldEvents)
            {
                expectEvents = expectEvents * 3;
            }

            IEnumerable<FiredEvent> events = ExpectEvents(watcher, expectEvents, action);

            // Remove Created and Changed events as there is racecondition when create file and then observe parent folder. It receives Create and Changed event altought Watcher is not registered yet.
            if (skipOldEvents)
            {
                events = events.Where(x => (x.EventType & (WatcherChangeTypes.Created | WatcherChangeTypes.Changed)) == 0);
            }

            var expectedEvents = files.Select(file => new FiredEvent(WatcherChangeTypes.Deleted, file.FileInWatchedDir));

            Assert.Equal(expectedEvents, events);
        }

        private void FileMove_Multiple_FromUnwatchedToWatched(int filesCount)
        {
            Assert.InRange(filesCount, 0, int.MaxValue);

            using var testDirectory = new TempDirectory(GetTestFilePath());
            using var watchedTestDirectory = new TempDirectory(Path.Combine(testDirectory.Path, "dir_watched"));
            using var unwatchedTestDirectory = new TempDirectory(Path.Combine(testDirectory.Path, "dir_unwatched"));

            var files = Enumerable.Range(0, filesCount)
                            .Select(i => new
                            {
                                FileInWatchedDir = Path.Combine(watchedTestDirectory.Path, $"file{i}"),
                                FileInUnwatchedDir = Path.Combine(unwatchedTestDirectory.Path, $"file{i}")
                            }).ToArray();

            Array.ForEach(files, (file) => File.Create(file.FileInUnwatchedDir).Dispose());

            using var watcher = new FileSystemWatcher(watchedTestDirectory.Path, "*");

            Action action = () => Array.ForEach(files, file => File.Move(file.FileInUnwatchedDir, file.FileInWatchedDir));

            List<FiredEvent> events = ExpectEvents(watcher, filesCount, action);
            var expectedEvents = files.Select(file => new FiredEvent(WatcherChangeTypes.Created, file.FileInWatchedDir));

            Assert.Equal(expectedEvents, events);
        }

        private void FileMove_FromUnwatchedToWatched(WatcherChangeTypes eventType)
        {
            using (var testDirectory = new TempDirectory(GetTestFilePath()))
            using (var dir_watched = new TempDirectory(Path.Combine(testDirectory.Path, "dir_watched")))
            using (var dir_unwatched = new TempDirectory(Path.Combine(testDirectory.Path, "dir_unwatched")))
            using (var testFile = new TempFile(Path.Combine(dir_unwatched.Path, "file")))
            using (var watcher = new FileSystemWatcher(dir_watched.Path, "*"))
            {
                string sourcePath = testFile.Path; // unwatched
                string targetPath = Path.Combine(dir_watched.Path, "file");

                Action action = () => File.Move(sourcePath, targetPath);
                Action cleanup = () => File.Move(targetPath, sourcePath);

                ExpectEvent(watcher, eventType, action, cleanup, targetPath);
            }
        }

        private void FileMove_NestedDirectory(WatcherChangeTypes eventType, bool includeSubdirectories)
        {
            using (var dir = new TempDirectory(GetTestFilePath()))
            using (var firstDir = new TempDirectory(Path.Combine(dir.Path, "dir1")))
            using (var nestedDir = new TempDirectory(Path.Combine(firstDir.Path, "nested")))
            using (var nestedFile = new TempFile(Path.Combine(nestedDir.Path, "nestedFile" + eventType.ToString())))
            using (var watcher = new FileSystemWatcher(dir.Path, "*"))
            {
                watcher.NotifyFilter = NotifyFilters.FileName;
                watcher.IncludeSubdirectories = includeSubdirectories;

                string sourcePath = nestedFile.Path;
                string targetPath = nestedFile.Path + "_2";

                // Move the testFile to a different name within the same nested directory
                Action action = () => File.Move(sourcePath, targetPath);
                Action cleanup = () => File.Move(targetPath, sourcePath);

                if ((eventType & WatcherChangeTypes.Deleted) > 0)
                    ExpectEvent(watcher, eventType, action, cleanup, new string[] { targetPath, sourcePath });
                else
                    ExpectEvent(watcher, eventType, action, cleanup, targetPath);
            }
        }

        private void FileMove_WithNotifyFilter(WatcherChangeTypes eventType)
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

                if ((eventType & WatcherChangeTypes.Deleted) > 0)
                    ExpectEvent(watcher, eventType, action, cleanup, sourcePath);
                else
                    ExpectEvent(watcher, eventType, action, cleanup, targetPath);
            }
        }

        #endregion
    }
}
