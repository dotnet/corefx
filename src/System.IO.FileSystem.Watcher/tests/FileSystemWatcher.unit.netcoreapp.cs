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
            Assert.Equal(new string[] { }, watcher.Filters);
        }

        [Fact]
        public void AddFilterToFilters()
        {
            var watcher = new FileSystemWatcher();
            watcher.Filters.Add("*.pdb");
            watcher.Filters.Add("*.dll");
            Assert.Equal(2, watcher.Filters.Count);
            Assert.Equal(new string[] { "*.pdb", "*.dll" }, watcher.Filters);
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
            watcher.Filters.Add("*.pdb");
            watcher.Filters.Add("*.dll");

            watcher.Filters.Remove("*.pdb");
            Assert.DoesNotContain(watcher.Filters, t => t == "*.pdb");
            Assert.Equal(new string[] { "*.dll" }, watcher.Filters);

            // No Exception is thrown while removing an item which is not present in the list.
            watcher.Filters.Remove("*.pdb");
        }

        [Fact]
        public void AddEmptyStringToFilters()
        {
            var watcher = new FileSystemWatcher();
            watcher.Filters.Add("*.pdb");
            watcher.Filters.Add("*.dll");

            watcher.Filters.Add(string.Empty);
            Assert.Equal(3, watcher.Filters.Count);
            Assert.Equal(new string[] { "*.pdb", "*.dll", "*" }, watcher.Filters);
        }

        [Fact]
        public void AddNullToFilters()
        {
            var watcher = new FileSystemWatcher();

            watcher.Filters.Add("*.pdb");
            watcher.Filters.Add("*.dll");

            watcher.Filters.Add(null);
            Assert.Equal(3, watcher.Filters.Count);
            Assert.Equal(new string[] { "*.pdb", "*.dll", "*" }, watcher.Filters);
        }

        [Fact]
        public void SetEmptyStringToFilters()
        {
            var watcher = new FileSystemWatcher();
            watcher.Filters.Add("*.pdb");
            watcher.Filters.Add("*.dll");

            watcher.Filters[0] = string.Empty;
            Assert.Equal(2, watcher.Filters.Count);
            Assert.Equal("*", watcher.Filters[0]);
            Assert.Equal(new string[] { "*", "*.dll"}, watcher.Filters);
        }

        [Fact]
        public void RemoveEmptyStringToFilters()
        {
            var watcher = new FileSystemWatcher();
            watcher.Filters.Add("*.pdb");
            watcher.Filters.Add("*.dll");
            watcher.Filters.Add(string.Empty);

            Assert.Equal(3, watcher.Filters.Count);
            watcher.Filters.Remove(string.Empty);
            Assert.Equal(3, watcher.Filters.Count);
            Assert.Equal(new string[] { "*.pdb", "*.dll", "*" }, watcher.Filters);
        }

        [Fact]
        public void RemoveAtFilters()
        {
            var watcher = new FileSystemWatcher();
            watcher.Filters.Add("*.pdb");
            watcher.Filters.Add("*.dll");
            
            watcher.Filters.RemoveAt(0);
            Assert.Equal(1, watcher.Filters.Count);
            Assert.Equal("*.dll", watcher.Filter);
            Assert.Equal(new string[] {"*.dll" }, watcher.Filters);
        }

        [Fact]
        public void RemoveAtEmptyFilters()
        {
            var watcher = new FileSystemWatcher();
            watcher.Filters.Add("*.pdb");

            watcher.Filters.RemoveAt(0);
            Assert.Equal(0, watcher.Filters.Count);
            Assert.Equal("*", watcher.Filter);
            Assert.Equal(new string[] { }, watcher.Filters);
        }

        [Fact]
        public void SetNullToFilters()
        {
            var watcher = new FileSystemWatcher();
            watcher.Filters.Add("*.pdb");
            watcher.Filters.Add("*.dll");

            watcher.Filters[0] = null;
            Assert.Equal(2, watcher.Filters.Count);
            Assert.Equal("*", watcher.Filters[0]);
            Assert.Equal(new string[] { "*", "*.dll" }, watcher.Filters);
        }

        [Fact]
        public void ContainsEmptyStringFilters()
        {
            var watcher = new FileSystemWatcher();
            watcher.Filters.Add("*.pdb");
            watcher.Filters.Add("*.dll");
            watcher.Filters.Add(string.Empty);

            Assert.False(watcher.Filters.Contains(string.Empty));
            Assert.True(watcher.Filters.Contains("*"));
        }

        [Fact]
        public void ContainsNullFilters()
        {
            var watcher = new FileSystemWatcher();
            watcher.Filters.Add("*.pdb");
            watcher.Filters.Add("*.dll");
            watcher.Filters.Add(null);

            Assert.False(watcher.Filters.Contains(null));
            Assert.True(watcher.Filters.Contains("*"));
        }

        [Fact]
        public void ContainsFilters()
        {
            var watcher = new FileSystemWatcher();
            watcher.Filters.Add("*.pdb");
            watcher.Filters.Add("*.dll");
            
            Assert.True(watcher.Filters.Contains("*.pdb"));
        }

        [Fact]
        public void InsertEmptyStringFilters()
        {
            var watcher = new FileSystemWatcher();
            watcher.Filters.Add("*.pdb");
            watcher.Filters.Add("*.dll");
            watcher.Filters.Insert(1, string.Empty);

            Assert.Equal("*", watcher.Filters[1]);
            Assert.Equal(3, watcher.Filters.Count);
            Assert.Equal(new string[] { "*.pdb", "*", "*.dll" }, watcher.Filters);
        }

        [Fact]
        public void InsertNullFilters()
        {
            var watcher = new FileSystemWatcher();
            watcher.Filters.Add("*.pdb");
            watcher.Filters.Add("*.dll");
            watcher.Filters.Insert(1, null);

            Assert.Equal("*", watcher.Filters[1]);
            Assert.Equal(3, watcher.Filters.Count);
            Assert.Equal(new string[] { "*.pdb", "*", "*.dll" }, watcher.Filters);
        }

        [Fact]
        public void InsertFilters()
        {
            var watcher = new FileSystemWatcher();
            watcher.Filters.Add("*.pdb");
            watcher.Filters.Add("*.dll");
            watcher.Filters.Insert(1, "foo");

            Assert.Equal("foo", watcher.Filters[1]);
            Assert.Equal(3, watcher.Filters.Count);
            Assert.Equal(new string[] { "*.pdb", "foo", "*.dll" }, watcher.Filters);
        }

        [Fact]
        public void InsertAtZero()
        {
            var watcher = new FileSystemWatcher();
            watcher.Filters.Add("*.pdb");
            watcher.Filters.Add("*.dll");
            watcher.Filters.Insert(0, "foo");

            Assert.Equal("foo", watcher.Filters[0]);
            Assert.Equal("foo", watcher.Filter);
            Assert.Equal(3, watcher.Filters.Count);
            Assert.Equal(new string[] { "foo", "*.pdb", "*.dll" }, watcher.Filters);
        }
        [Fact]
        public void IndexOfEmptyStringFilters()
        {
            var watcher = new FileSystemWatcher();
            watcher.Filters.Add("*.pdb");
            watcher.Filters.Add("*.dll");
            watcher.Filters.Add(string.Empty);

            Assert.Equal(-1, watcher.Filters.IndexOf(string.Empty));
        }

        [Fact]
        public void IndexOfNullFilters()
        {
            var watcher = new FileSystemWatcher();
            watcher.Filters.Add("*.pdb");
            watcher.Filters.Add("*.dll");
            watcher.Filters.Add(null);

            Assert.Equal(-1, watcher.Filters.IndexOf(null));
        }

        [Fact]
        public void IndexOfFilters()
        {
            var watcher = new FileSystemWatcher();
            watcher.Filters.Add("*.pdb");
            watcher.Filters.Add("*.dll");
            
            Assert.Equal(-1, watcher.Filters.IndexOf("foo"));
            Assert.Equal(0, watcher.Filters.IndexOf("*.pdb"));
        }

        [Fact]
        public void GetTypeFilters()
        {
            var watcher = new FileSystemWatcher();
            Assert.IsAssignableFrom<Collection<string>>(watcher.Filters);
        }

        [Fact]
        public void ClearFilters()
        {
            var watcher = new FileSystemWatcher();
            watcher.Filters.Add("*.pdb");
            watcher.Filters.Add("*.dll");

            watcher.Filters.Clear();
            Assert.Equal(0, watcher.Filters.Count);
            Assert.Equal(new string[] { }, watcher.Filters) ;
        }

        [Fact]
        public void GetFilterAfterFiltersClear()
        {
            using (var testDirectory = new TempDirectory(GetTestFilePath()))
            {
                var watcher = new FileSystemWatcher(testDirectory.Path);
                watcher.Filters.Add("*.pdb");
                watcher.Filters.Add("*.dll");

                watcher.Filters.Clear();
                Assert.Equal("*", watcher.Filter);
                Assert.Equal(new string[] { }, watcher.Filters);
            }
        }

        [Fact]
        public void GetFiltersAfterFiltersClear()
        {
            using (var testDirectory = new TempDirectory(GetTestFilePath()))
            {
                var watcher = new FileSystemWatcher(testDirectory.Path);
                watcher.Filters.Add("*.pdb");
                watcher.Filters.Add("*.dll");

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
            watcher.Filters.Add("*.pdb");
            watcher.Filters.Add("*.dll");

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
                Assert.Equal(new string[] { "*.pdb", "foo" }, watcher.Filters);

                watcher.Filter = "*.doc";
                Assert.Equal(1, watcher.Filters.Count);
                Assert.Equal("*.doc", watcher.Filter);
                Assert.Equal("*.doc", watcher.Filters[0]);
                Assert.Equal(new string[] { "*.doc" }, watcher.Filters);

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
                Assert.Equal(new string[] { "*.pdb", "foo" }, watcher.Filters);
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

                Action action = () => Directory.CreateDirectory(dirName);
                Action cleanup = () => Directory.Delete(dirName);

                ExpectEvent(watcher, WatcherChangeTypes.Created, action, cleanup, expectedPaths);

                action = () => Directory.CreateDirectory(dirNameSecond);
                cleanup = () => Directory.Delete(dirNameSecond);

                ExpectEvent(watcher, WatcherChangeTypes.Created, action, cleanup, expectedPaths);

                action = () => Directory.CreateDirectory(dirNameThird);
                cleanup = () => Directory.Delete(dirNameThird);

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

                Action action = () => Directory.CreateDirectory(dirName);
                Action cleanup = () => Directory.Delete(dirName);

                ExpectEvent(watcher, WatcherChangeTypes.Created, action, cleanup, expectedPaths);

                action = () => Directory.CreateDirectory(dirNameSecond);
                cleanup = () => Directory.Delete(dirNameSecond);

                ExpectEvent(watcher, WatcherChangeTypes.Created, action, cleanup, expectedPaths);

                action = () => Directory.CreateDirectory(dirNameThird);
                cleanup = () => Directory.Delete(dirNameThird);

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
    }
}
