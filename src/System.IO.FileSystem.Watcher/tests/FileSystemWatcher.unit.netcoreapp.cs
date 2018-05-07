// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.ObjectModel;
using System.Threading;
using Xunit;

namespace System.IO.Tests
{
    public class FileSystemWatcherFilterListTests : FileSystemWatcherTest
    {
        [Fact]
        public void DefaultFiltersValue()
        {
            var watcher = new FileSystemWatcher();
            Assert.Equal(0, watcher.Filters.Count);
            Assert.Empty(watcher.Filters);
            Assert.NotNull(watcher.Filters);
            Assert.True(EqualItems(new Collection<string> { }, watcher.Filters));
        }

        [Fact]
        public void AddFilterToFilters()
        {
            var watcher = new FileSystemWatcher();
            watcher.Filters.Add("*.pdb");
            watcher.Filters.Add("*.dll");
            Assert.Equal(2, watcher.Filters.Count);
            Assert.True(EqualItems(new Collection<string> { "*.pdb", "*.dll" }, watcher.Filters));
        }

        [Fact]
        public void FiltersCaseSensitive()
        {
            var watcher = new FileSystemWatcher();
            watcher.Filters.Add("foo");
            Assert.Equal("foo", watcher.Filters[0]);
            watcher.Filters[0] = "Foo";
            Assert.Equal("Foo", watcher.Filters[0]);
        }

        [Fact]
        public void RemoveFilterFromFilters()
        {
            var watcher = new FileSystemWatcher();
            Setup(watcher);

            watcher.Filters.Remove("*.pdb");
            Assert.DoesNotContain(watcher.Filters, t => t == "*.pdb");
            Assert.True(EqualItems(new Collection<string> { "*.dll" }, watcher.Filters));

            // No Exception is thrown while removing an item which is not present in the list.
            watcher.Filters.Remove("*.pdb");
        }

        [Fact]
        public void AddNullOrEmptyStringToFilters()
        {
            var watcher = new FileSystemWatcher();
            Setup(watcher);

            watcher.Filters.Add(string.Empty);
            Assert.Equal(0, watcher.Filters.Count);

            Setup(watcher);
            Assert.Equal(2, watcher.Filters.Count);
            watcher.Filters.Add(null);
            Assert.Equal(0, watcher.Filters.Count);
            Assert.True(EqualItems(new Collection<string> { }, watcher.Filters));
        }

        [Fact]
        public void SetNullOrEmptyStringToFilters()
        {
            var watcher = new FileSystemWatcher();
            Setup(watcher);

            watcher.Filters[0] = string.Empty;
            Assert.Equal(0, watcher.Filters.Count);

            Setup(watcher);
            Assert.Equal(2, watcher.Filters.Count);
            watcher.Filters[0] = null;
            Assert.Equal(0, watcher.Filters.Count);
            Assert.True(EqualItems(new Collection<string> { }, watcher.Filters));
        }

        [Fact]
        public void ClearFilters()
        {
            var watcher = new FileSystemWatcher();
            Setup(watcher);

            watcher.Filters.Clear();
            Assert.Equal(0, watcher.Filters.Count);
            Assert.True(EqualItems(new Collection<string> { }, watcher.Filters));
        }

        [Fact]
        public void GetFilterAfterFiltersClear()
        {
            using (var testDirectory = new TempDirectory(GetTestFilePath()))
            {
                var watcher = new FileSystemWatcher(testDirectory.Path);
                Setup(watcher);

                watcher.Filters.Clear();
                Assert.Equal("*", watcher.Filter);
                Assert.True(EqualItems(new Collection<string> { }, watcher.Filters));
            }
        }

        [Fact]
        public void GetFiltersAfterFiltersClear()
        {
            using (var testDirectory = new TempDirectory(GetTestFilePath()))
            {
                var watcher = new FileSystemWatcher(testDirectory.Path);
                Setup(watcher);

                watcher.Filters.Clear();
                Assert.Throws<ArgumentOutOfRangeException>(() => watcher.Filters[0]);
                Assert.Equal(0, watcher.Filters.Count);
                Assert.Empty(watcher.Filters);
                Assert.NotNull(watcher.Filters);
            }
        }

        [Fact]
        public void InvalidOperationsOnFilters()
        {
            var watcher = new FileSystemWatcher();
            Setup(watcher);

            Assert.Throws<ArgumentOutOfRangeException>(() => watcher.Filters.Insert(4, "*"));            
            watcher.Filters.Clear();
            Assert.Throws<ArgumentOutOfRangeException>(() => watcher.Filters[0]);
        }

        [Fact]
        public void SetAndGetFilterProperty()
        {
            using (var testDirectory = new TempDirectory(GetTestFilePath()))
            {
                var watcher = new FileSystemWatcher(testDirectory.Path, "*.pdb");
                watcher.Filters.Add("foo");
                Assert.Equal(2, watcher.Filters.Count);
                Assert.True(EqualItems(new Collection<string> { "*.pdb", "foo" }, watcher.Filters));

                watcher.Filter = "*.doc";
                Assert.Equal(1, watcher.Filters.Count);
                Assert.Equal("*.doc", watcher.Filter);
                Assert.Equal("*.doc", watcher.Filters[0]);
                Assert.True(EqualItems(new Collection<string> { "*.doc" }, watcher.Filters));

                watcher.Filters.Clear();
                Assert.Equal("*", watcher.Filter);
            }
        }

        [Fact]
        public void SetAndGetFiltersProperty()
        {
            using (var testDirectory = new TempDirectory(GetTestFilePath()))
            {
                var watcher = new FileSystemWatcher(testDirectory.Path, "*.pdb");
                watcher.Filters.Add("foo");
                Assert.True(EqualItems(new Collection<string> { "*.pdb", "foo" }, watcher.Filters));
            }
        }

        [Fact]
        public void FileSystemWatcher_Directory_Create()
        {
            using (var testDirectory = new TempDirectory(GetTestFilePath()))
            using (var watcher = new FileSystemWatcher(testDirectory.Path, Path.GetFileName(Path.Combine(testDirectory.Path, "dirtest"))))
            {
                string dirName = Path.Combine(testDirectory.Path, "dirtest");
                string dirNameSecond = Path.Combine(testDirectory.Path, "dirfoo");
                string dirNameThird = Path.Combine(testDirectory.Path, "dirtfoo");
                string[] expectedPaths = new string[] { dirName, dirNameSecond,dirNameThird };

                watcher.Filters.Add(Path.GetFileName(dirNameSecond));

                Action cleanup = () => Directory.Delete(dirName);
                Action action = () => Directory.CreateDirectory(dirName);

                ExpectEvent(watcher, WatcherChangeTypes.Created, action, cleanup, expectedPaths);

                cleanup = () => Directory.Delete(dirNameSecond);
                action = () => Directory.CreateDirectory(dirNameSecond);

                ExpectEvent(watcher, WatcherChangeTypes.Created, action, cleanup, expectedPaths);

                cleanup = () => Directory.Delete(dirNameThird);
                action = () => Directory.CreateDirectory(dirNameThird);

                Assert.Throws<Xunit.Sdk.TrueException>(() => ExpectEvent(watcher, WatcherChangeTypes.Created, action, cleanup, expectedPaths));

            }
        }

        [Fact]
        public void FileSystemWatcher_Directory_Create_Empty_Ctor()
        {
            using (var testDirectory = new TempDirectory(GetTestFilePath()))
            using (var watcher = new FileSystemWatcher(testDirectory.Path, Path.GetFileName(Path.Combine(testDirectory.Path))))
            {
                string dirName = Path.Combine(testDirectory.Path, "dirtest");
                string dirNameSecond = Path.Combine(testDirectory.Path, "dirfoo");
                string dirNameThird = Path.Combine(testDirectory.Path, "dirtfoo");
                string[] expectedPaths = new string[] { dirName, dirNameSecond,dirNameThird };

                watcher.Filters.Add(Path.GetFileName(dirName));
                watcher.Filters.Add(Path.GetFileName(dirNameSecond));

                Action cleanup = () => Directory.Delete(dirName);
                Action action = () => Directory.CreateDirectory(dirName);

                ExpectEvent(watcher, WatcherChangeTypes.Created, action, cleanup, expectedPaths);

                cleanup = () => Directory.Delete(dirNameSecond);
                action = () => Directory.CreateDirectory(dirNameSecond);

                ExpectEvent(watcher, WatcherChangeTypes.Created, action, cleanup, expectedPaths);

                cleanup = () => Directory.Delete(dirNameThird);
                action = () => Directory.CreateDirectory(dirNameThird);

                Assert.Throws<Xunit.Sdk.TrueException>(() => ExpectEvent(watcher, WatcherChangeTypes.Created, action, cleanup, expectedPaths));

            }
        }

        [Fact]
        public void FileSystemWatcher_Directory_Delete()
        {
            using (var testDirectory = new TempDirectory(GetTestFilePath()))
            using (var watcher = new FileSystemWatcher(testDirectory.Path))
            {
                string dirName = Path.Combine(testDirectory.Path, "dirtest");
                string dirNameSecond = Path.Combine(testDirectory.Path, "dirfoo");
                string[] expectedPaths = new string[] { dirName, dirNameSecond };

                watcher.Filters.Add(Path.GetFileName(dirName));
                watcher.Filters.Add(Path.GetFileName(dirNameSecond));

                Action action = () => Directory.Delete(dirName);
                Action cleanup = () => Directory.CreateDirectory(dirName);
                cleanup();

                ExpectEvent(watcher, WatcherChangeTypes.Deleted, action, cleanup, expectedPaths);

                action = () => Directory.Delete(dirNameSecond);
                cleanup = () => Directory.CreateDirectory(dirNameSecond);
                cleanup();

                ExpectEvent(watcher, WatcherChangeTypes.Deleted, action, cleanup, expectedPaths);
            }
        }

        [Fact]
        public void FileSystemWatcher_File_Create()
        {
            using (var testDirectory = new TempDirectory(GetTestFilePath()))
            using (var watcher = new FileSystemWatcher(testDirectory.Path))
            {
                string fileName = Path.Combine(testDirectory.Path, "file");
                string secondFileName = Path.Combine(testDirectory.Path, "Secondfile");
                string[] expectedPaths = new string[] { fileName, secondFileName };

                watcher.Filters.Add(Path.GetFileName(fileName));
                watcher.Filters.Add(Path.GetFileName(secondFileName));

                Action action = () => File.Create(fileName).Dispose();
                Action cleanup = () => File.Delete(fileName);

                ExpectEvent(watcher, WatcherChangeTypes.Created, action, cleanup, expectedPaths);

                action = () => File.Create(secondFileName).Dispose();
                cleanup = () => File.Delete(secondFileName);
                cleanup();

                ExpectEvent(watcher, WatcherChangeTypes.Created, action, cleanup, expectedPaths);
            }
        }

        [Fact]
        public void FileSystemWatcher_File_Delete()
        {
            using (var testDirectory = new TempDirectory(GetTestFilePath()))
            using (var watcher = new FileSystemWatcher(testDirectory.Path))
            {
                string fileName = Path.Combine(testDirectory.Path, "file");
                string secondFileName = Path.Combine(testDirectory.Path, "Secondfile");
                string[] expectedPaths = new string[] { fileName, secondFileName };

                watcher.Filters.Add(Path.GetFileName(fileName));
                watcher.Filters.Add(Path.GetFileName(secondFileName));

                Action action = () => File.Delete(fileName);
                Action cleanup = () => File.Create(fileName).Dispose();
                cleanup();

                ExpectEvent(watcher, WatcherChangeTypes.Deleted, action, cleanup, expectedPaths);

                action = () => File.Delete(secondFileName);
                cleanup = () => File.Create(secondFileName).Dispose();
                cleanup();

                ExpectEvent(watcher, WatcherChangeTypes.Deleted, action, cleanup, expectedPaths);
            }
        }

        private bool EqualItems(Collection<string> firstCollection, Collection<string> secondCollection)
        {
            if (firstCollection.Count != secondCollection.Count)
                return false;

            for (int i = 0; i < firstCollection.Count; i++)
            {
                if (firstCollection[i] != secondCollection[i])
                    return false;
            }
            return true;
        }

        private void Setup(FileSystemWatcher watcher)
        {
            watcher.Filters.Add("*.pdb");
            watcher.Filters.Add("*.dll");
            Assert.Equal(2, watcher.Filters.Count);
            Assert.True(EqualItems(new Collection<string> { "*.pdb", "*.dll" }, watcher.Filters));
        }
    }
}
