// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Threading;
using Xunit;

namespace System.IO.Tests
{
    public class Directory_Create_Tests : FileSystemWatcherTest
    {
        [Theory]
        [InlineData(WatcherChangeTypes.Changed, false)]
        [InlineData(WatcherChangeTypes.Created, true)]
        [InlineData(WatcherChangeTypes.Deleted, false)]
        [InlineData(WatcherChangeTypes.Renamed, false)]
        public void FileSystemWatcher_Directory_Create(WatcherChangeTypes eventType, bool raisesEvent)
        {
            using (var testDirectory = new TempDirectory(GetTestFilePath()))
            using (var watcher = new FileSystemWatcher(testDirectory.Path))
            {
                string dirName = Path.Combine(testDirectory.Path, "dir");
                watcher.Filter = Path.GetFileName(dirName);

                Action action = () => Directory.CreateDirectory(dirName);
                Action cleanup = () => Directory.Delete(dirName);

                // Test that the event is observed or not observed
                if (raisesEvent)
                    ExpectEvent(watcher, eventType, action, cleanup);
                else
                    ExpectNoEvent(watcher, eventType, action, cleanup);
            }
        }
        
        [Theory]
        [InlineData(WatcherChangeTypes.Changed, false)]
        [InlineData(WatcherChangeTypes.Created, true)]
        [InlineData(WatcherChangeTypes.Deleted, false)]
        [InlineData(WatcherChangeTypes.Renamed, false)]
        public void FileSystemWatcher_Directory_Create_InNestedDirectory(WatcherChangeTypes eventType, bool raisesEvent)
        {
            using (var dir = new TempDirectory(GetTestFilePath()))
            using (var watcher = new FileSystemWatcher(dir.Path, "*"))
            using (var firstDir = new TempDirectory(Path.Combine(dir.Path, "dir1")))
            using (var nestedDir = new TempDirectory(Path.Combine(firstDir.Path, "nested")))
            {
                watcher.IncludeSubdirectories = true;

                string dirName = Path.Combine(nestedDir.Path, "dir");
                Action action = () => Directory.CreateDirectory(dirName);
                Action cleanup = () => Directory.Delete(dirName);

                // Test that the event is observed or not observed
                if (raisesEvent)
                    ExpectEvent(watcher, eventType, action, cleanup);
                else
                    ExpectNoEvent(watcher, eventType, action, cleanup);
            }
        }

        [Theory]
        [InlineData(WatcherChangeTypes.Changed, false)]
        [InlineData(WatcherChangeTypes.Created, true)]
        [InlineData(WatcherChangeTypes.Deleted, false)]
        [InlineData(WatcherChangeTypes.Renamed, false)]
        public void FileSystemWatcher_Directory_Create_DeepDirectoryStructure(WatcherChangeTypes eventType, bool raisesEvent)
        {
            // List of created directories
            List<TempDirectory> lst = new List<TempDirectory>();

            using (var testDirectory = new TempDirectory(GetTestFilePath()))
            using (var dir = new TempDirectory(Path.Combine(testDirectory.Path, "dir")))
            using (var watcher = new FileSystemWatcher(Path.GetFullPath(dir.Path), "*.*"))
            {
                AutoResetEvent eventOccurred = WatchForEvents(watcher, WatcherChangeTypes.Created);

                // put everything in our own directory to avoid collisions
                watcher.IncludeSubdirectories = true;

                // Priming directory
                lst.Add(new TempDirectory(Path.Combine(dir.Path, "dir")));

                // Create a deep directory structure and expect things to work
                for (int i = 1; i < 20; i++)
                {
                    // Test that the creation triggers an event correctly
                    string dirPath = Path.Combine(lst[i - 1].Path, String.Format("dir{0}", i));
                    Action dirAction = () => Directory.CreateDirectory(dirPath);
                    Action dirCleanup = () => Directory.Delete(dirPath);

                    // Test that the event is observed or not observed
                    if (raisesEvent)
                        ExpectEvent(watcher, eventType, dirAction, dirCleanup);
                    else
                        ExpectNoEvent(watcher, eventType, dirAction, dirCleanup);

                    // Create the directory so subdirectories may be created from it.
                    lst.Add(new TempDirectory(dirPath));
                }
            }
        }

        [ConditionalTheory(nameof(CanCreateSymbolicLinks))]
        //[ActiveIssue(3215, PlatformID.OSX)]
        [InlineData(WatcherChangeTypes.Changed, false)]
        [InlineData(WatcherChangeTypes.Created, true)]
        [InlineData(WatcherChangeTypes.Deleted, false)]
        [InlineData(WatcherChangeTypes.Renamed, false)]
        public void FileSystemWatcher_Directory_Create_WatcherDoesntFollowSymLinkToFolder(WatcherChangeTypes eventType, bool raisesEvent)
        {
            using (var testDirectory = new TempDirectory(GetTestFilePath()))
            using (var dir = new TempDirectory(Path.Combine(testDirectory.Path, "dir")))
            using (var temp = new TempDirectory(GetTestFilePath()))
            using (var watcher = new FileSystemWatcher(Path.GetFullPath(dir.Path), "*.*"))
            {
                // Make the symlink in our path (to the temp folder) and make sure an event is raised
                string symLinkPath = Path.Combine(dir.Path, Path.GetFileName(temp.Path));
                Action action = () => Assert.True(CreateSymLink(temp.Path, symLinkPath, true));
                Action cleanup = () => Directory.Delete(symLinkPath);

                // Test that the event is observed or not observed
                if (raisesEvent)
                    ExpectEvent(watcher, eventType, action, cleanup);
                else
                    ExpectNoEvent(watcher, eventType, action, cleanup);
            }
        }
    }
}