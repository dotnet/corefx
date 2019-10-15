// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;


using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading;

namespace System.IO.Tests
{
    public class Directory_Move_Tests : FileSystemWatcherTest
    {
        [Fact]
        [PlatformSpecific(TestPlatforms.Windows)]  // Expected WatcherChangeTypes are different based on OS
        public void Directory_Move_To_Same_Directory()
        {
            DirectoryMove_SameDirectory(WatcherChangeTypes.Renamed);
        }

        [Fact]
        [PlatformSpecific(TestPlatforms.OSX)]
        public void Directory_Move_From_Watched_To_Unwatched()
        {
            DirectoryMove_FromWatchedToUnwatched(WatcherChangeTypes.Deleted);
        }

        [Fact]
        [PlatformSpecific(TestPlatforms.OSX)]
        public void Directory_Move_From_Watched_To_UnwatchedMac()
        {
            DirectoryMove_FromWatchedToUnwatchedMac(1);
        }

        [Fact]
        [PlatformSpecific(TestPlatforms.OSX)]
        public void Directory_Move_From_Watched_To_UnwatchedMac2()
        {
            DirectoryMove_FromWatchedToUnwatchedMac(2);
        }

        [Fact]
        [PlatformSpecific(TestPlatforms.Windows)]  // Expected WatcherChangeTypes are different based on OS
        public void Windows_Directory_Move_To_Different_Watched_Directory()
        {
            DirectoryMove_DifferentWatchedDirectory(WatcherChangeTypes.Changed);
        }

        [Fact]
        [PlatformSpecific(TestPlatforms.AnyUnix)]  // Expected WatcherChangeTypes are different based on OS
        public void Unix_Directory_Move_To_Different_Watched_Directory()
        {
            DirectoryMove_DifferentWatchedDirectory(0);
        }

        [Fact]
        public void Directory_Move_From_Unwatched_To_Watched()
        {
            DirectoryMove_FromUnwatchedToWatched(WatcherChangeTypes.Created);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void Directory_Move_In_Nested_Directory(bool includeSubdirectories)
        {
            DirectoryMove_NestedDirectory(includeSubdirectories ? WatcherChangeTypes.Renamed : 0, includeSubdirectories);
        }

        [Fact]
        public void Directory_Move_With_Set_NotifyFilter()
        {
            DirectoryMove_WithNotifyFilter(WatcherChangeTypes.Renamed);
        }

        #region Test Helpers

        private void DirectoryMove_SameDirectory(WatcherChangeTypes eventType)
        {
            using (var testDirectory = new TempDirectory(GetTestFilePath()))
            using (var dir = new TempDirectory(Path.Combine(testDirectory.Path, "dir")))
            using (var watcher = new FileSystemWatcher(testDirectory.Path, "*"))
            {
                string sourcePath = dir.Path;
                string targetPath = Path.Combine(testDirectory.Path, "target");

                Action action = () => Directory.Move(sourcePath, targetPath);
                Action cleanup = () => Directory.Move(targetPath, sourcePath);

                ExpectEvent(watcher, eventType, action, cleanup, targetPath);
            }
        }

        private void DirectoryMove_DifferentWatchedDirectory(WatcherChangeTypes eventType)
        {
            using (var testDirectory = new TempDirectory(GetTestFilePath()))
            using (var sourceDir = new TempDirectory(Path.Combine(testDirectory.Path, "source")))
            using (var adjacentDir = new TempDirectory(Path.Combine(testDirectory.Path, "adj")))
            using (var dir = new TempDirectory(Path.Combine(sourceDir.Path, "dir")))
            using (var watcher = new FileSystemWatcher(testDirectory.Path, "*"))
            {
                string sourcePath = dir.Path;
                string targetPath = Path.Combine(adjacentDir.Path, "target");

                // Move the dir to a different directory under the Watcher
                Action action = () => Directory.Move(sourcePath, targetPath);
                Action cleanup = () => Directory.Move(targetPath, sourcePath);

                ExpectEvent(watcher, eventType, action, cleanup, new string[] { sourceDir.Path, adjacentDir.Path });
            }
        }

        private class FiredEvent
        {
            public FiredEvent(WatcherChangeTypes EventType, string Dir1, string Dir2 = "")
            {
                this.EventType = EventType;
                this.Dir1 = Dir1;
                this.Dir2 = Dir2;
            }
            public WatcherChangeTypes EventType { get; }
            public string Dir1 { get; }
            public string Dir2 { get; }

            public override bool Equals(Object obj)
            {
                FiredEvent other = obj as FiredEvent;
                if (other == null)
                {
                    return false;
                }
                return this.EventType == other.EventType &&
                    this.Dir1 == other.Dir1 &&
                    this.Dir2 == other.Dir2;
            }

            public override int GetHashCode()
            {
                return this.EventType.GetHashCode() ^ this.Dir1.GetHashCode() ^  this.Dir2.GetHashCode();
            }

            public override string ToString() {
                return $"{EventType} {Dir1} {Dir2}";
            }

        }

        private List<FiredEvent> ExpectEvents(FileSystemWatcher watcher, int expectedEvents, Action action, Action cleanup)
        {
            var eventsOccured =  new AutoResetEvent(false);
            var eventsOrrures = 0;

            var events = new List<FiredEvent>();

            ErrorEventArgs error = null;

            FileSystemEventHandler fileWatcherEvent = (_, e) => addEvent(e.ChangeType, e.FullPath);
            RenamedEventHandler renameWatcherEvent = (_, e) => addEvent(e.ChangeType, e.FullPath, e.OldFullPath);
            ErrorEventHandler errorHandler = (_, e) => error = e;

            watcher.Changed += fileWatcherEvent;
            watcher.Created += fileWatcherEvent;
            watcher.Deleted += fileWatcherEvent;
            watcher.Renamed += renameWatcherEvent;
            watcher.Error += errorHandler;

            var raisingEvent = watcher.EnableRaisingEvents;
            watcher.EnableRaisingEvents = true;

            try
            {
                action();
                eventsOccured.WaitOne(new TimeSpan(0, 0, 5));
            }
            finally
            {
                watcher.Changed -= fileWatcherEvent;
                watcher.Created -= fileWatcherEvent;
                watcher.Deleted -= fileWatcherEvent;
                watcher.Renamed -= renameWatcherEvent;
                watcher.Error -= errorHandler;
                watcher.EnableRaisingEvents = raisingEvent;
            }

            cleanup();


            if(error != null && error.GetException() != null)
            {
                Assert.False(true, $"Filewatcher error event triggered: {error.GetException().Message}");
            }
            Assert.True(eventsOrrures == expectedEvents, $"Expected events ({expectedEvents}) count doesn't match triggered events count ({eventsOrrures}): {String.Join(", ", events)}");

            return events;

            void addEvent(WatcherChangeTypes eventType, string dir1, string dir2 = ""){
                events.Add(new FiredEvent(eventType, dir1, dir2));
                if (Interlocked.Increment(ref eventsOrrures) == expectedEvents) {
                    eventsOccured.Set();
                }
            }
        }

        private void DirectoryMove_FromWatchedToUnwatchedMac(int filesCount)
        {
            Assert.True(filesCount > 0);

            using (var watchedTestDirectory = new TempDirectory(GetTestFilePath()))
            using (var unwatchedTestDirectory = new TempDirectory(GetTestFilePath()))
            {
                var dirs = Enumerable.Range(0,filesCount)
                                .Select(i => new TempDirectory(Path.Combine(watchedTestDirectory.Path, $"dir{i}")))
                                .ToArray();
                try{
                // using (var dir = new TempDirectory(Path.Combine(watchedTestDirectory.Path, "dir")))
                    using (var watcher = new FileSystemWatcher(watchedTestDirectory.Path, "*"))
                    {
                        Action action = () => Array.ForEach(dirs, dir => Directory.Move(Dump(dir.Path), Dump(Path.Combine(unwatchedTestDirectory.Path, Path.GetFileName(dir.Path)))));
                        Action cleanup = () => Array.ForEach(dirs, dir => Directory.Move(Path.Combine(unwatchedTestDirectory.Path, Path.GetFileName(dir.Path)), dir.Path));

                        var events = ExpectEvents(watcher, filesCount * 2, action, cleanup);

                        Assert.Equal(events, new FiredEvent[]{ new FiredEvent(WatcherChangeTypes.Created, dirs[0].Path), new FiredEvent(WatcherChangeTypes.Deleted, dirs[0].Path) });

                    }
                }
                finally{
                    Array.ForEach(dirs, dir=>dir.Dispose());
                }
            }
        }

        private T Dump<T>(T obj){
            Console.WriteLine(obj);
            return obj;
        }

        private void DirectoryMove_FromWatchedToUnwatched(WatcherChangeTypes eventType)
        {
            using (var watchedTestDirectory = new TempDirectory(GetTestFilePath()))
            using (var unwatchedTestDirectory = new TempDirectory(GetTestFilePath()))
            using (var dir = new TempDirectory(Path.Combine(watchedTestDirectory.Path, "dir")))
            using (var watcher = new FileSystemWatcher(watchedTestDirectory.Path, "*"))
            {
                string sourcePath = dir.Path; // watched
                string targetPath = Path.Combine(unwatchedTestDirectory.Path, "target");

                Action action = () => Directory.Move(sourcePath, targetPath);
                Action cleanup = () => Directory.Move(targetPath, sourcePath);

                ExpectEvent(watcher, eventType, action, cleanup, sourcePath);
            }
        }

/*
         private void DirectoryMove_DifferentWatchedDirectory(WatcherChangeTypes eventType, int numberOfDirectories)
        {
            if (numberOfDirectories < 1)
            {
                throw new ArgumentException("At least one directory should be tested.");
            }

            using (var testDirectory = new TempDirectory(GetTestFilePath()))
            using (var sourceDir = new TempDirectory(Path.Combine(testDirectory.Path, "source")))
            using (var adjacentDir = new TempDirectory(Path.Combine(testDirectory.Path, "adj")))
            using (var watcher = new FileSystemWatcher(testDirectory.Path, "*"))
            {
                var dirs = Enumerable.Range(1, numberOfDirectories)
                                     .Select(i => new TempDirectory(Path.Combine(sourceDir.Path, $"dir{i}")))
                                     .ToArray();

                try
                {
                    // Move the dir to a different directory under the Watcher
                   Console.WriteLine(watcher.Path);

                   var events = ExpectEvents(watcher, 2, () => { Console.WriteLine($"Action"); Array.ForEach(dirs, dir => Directory.Move(dir.Path,Path.Combine(adjacentDir.Path, Path.GetFileName(dir.Path)))); } );

                   Console.WriteLine(events.Count);

                   Array.ForEach(dirs, dir => Directory.Move(Path.Combine(adjacentDir.Path, Path.GetFileName(dir.Path)), dir.Path));

                    // for (var dir in dirs)
                    // {
                    //     Action action = () => Array.ForEach(dirs, dir => Directory.Move(Dump(dir.Path), Dump(Path.Combine(adjacentDir.Path, Path.GetFileName(dir.Path)))));
                    //     Action cleanup = () => Array.ForEach(dirs, dir => Directory.Move(Path.Combine(adjacentDir.Path, Path.GetFileName(dir.Path)), dir.Path));

                    //     ExpectEvent(watcher, eventType, action, cleanup, new string[] { sourceDir.Path, adjacentDir.Path });
                    // }
                }
                finally
                {
                    Array.ForEach(dirs, dir => dir.Dispose());
                }
            }
        }
 */

        private void DirectoryMove_FromUnwatchedToWatched(WatcherChangeTypes eventType)
        {
            using (var watchedTestDirectory = new TempDirectory(GetTestFilePath()))
            using (var unwatchedTestDirectory = new TempDirectory(GetTestFilePath()))
            using (var dir = new TempDirectory(Path.Combine(unwatchedTestDirectory.Path, "dir")))
            using (var watcher = new FileSystemWatcher(watchedTestDirectory.Path, "*"))
            {
                string sourcePath = dir.Path; // unwatched
                string targetPath = Path.Combine(watchedTestDirectory.Path, "target");

                Action action = () => Directory.Move(sourcePath, targetPath);
                Action cleanup = () => Directory.Move(targetPath, sourcePath);

                ExpectEvent(watcher, eventType, action, cleanup, targetPath);
            }
        }

        private void DirectoryMove_NestedDirectory(WatcherChangeTypes eventType, bool includeSubdirectories)
        {
            using (var dir = new TempDirectory(GetTestFilePath()))
            using (var firstDir = new TempDirectory(Path.Combine(dir.Path, "first")))
            using (var secondDir = new TempDirectory(Path.Combine(firstDir.Path, "second")))
            using (var nestedDir = new TempDirectory(Path.Combine(secondDir.Path, "nested")))
            using (var watcher = new FileSystemWatcher(dir.Path, "*"))
            {
                watcher.IncludeSubdirectories = includeSubdirectories;
                watcher.NotifyFilter = NotifyFilters.DirectoryName;

                string sourcePath = nestedDir.Path;
                string targetPath = nestedDir.Path + "_2";

                // Move the dir to a different directory within the same nested directory
                Action action = () => Directory.Move(sourcePath, targetPath);
                Action cleanup = () => Directory.Move(targetPath, sourcePath);

                ExpectEvent(watcher, eventType, action, cleanup, targetPath);
            }
        }

        private void DirectoryMove_WithNotifyFilter(WatcherChangeTypes eventType)
        {
            using (var testDirectory = new TempDirectory(GetTestFilePath()))
            using (var dir = new TempDirectory(Path.Combine(testDirectory.Path, "dir")))
            using (var watcher = new FileSystemWatcher(testDirectory.Path, "*"))
            {
                watcher.NotifyFilter = NotifyFilters.DirectoryName;

                string sourcePath = dir.Path;
                string targetPath = dir.Path + "_2";

                Action action = () => Directory.Move(sourcePath, targetPath);
                Action cleanup = () => Directory.Move(targetPath, sourcePath);

                ExpectEvent(watcher, eventType, action, cleanup, targetPath);
            }
        }

        #endregion
    }
}