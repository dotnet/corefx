// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using Xunit;

public class FileSystemWatcherTests
{
    private static void ValidateDefaults(FileSystemWatcher watcher, string path, string filter)
    {
        Assert.Equal(false, watcher.EnableRaisingEvents);
        Assert.Equal(filter, watcher.Filter);
        Assert.Equal(false, watcher.IncludeSubdirectories);
        Assert.Equal(8192, watcher.InternalBufferSize);
        Assert.Equal(NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName, watcher.NotifyFilter);
        Assert.Equal(path, watcher.Path);
    }

    [Fact]
    public static void FileSystemWatcher_ctor()
    {
        string path = String.Empty;
        string pattern = "*.*";
        using (FileSystemWatcher watcher = new FileSystemWatcher())
            ValidateDefaults(watcher, path, pattern);
    }

    [Fact]
    public static void FileSystemWatcher_ctor_path()
    {
        string path = @".";
        string pattern = "*.*";
        using (FileSystemWatcher watcher = new FileSystemWatcher(path))
            ValidateDefaults(watcher, path, pattern);
    }

    [Fact]
    public static void FileSystemWatcher_ctor_path_pattern()
    {
        string path = @".";
        string pattern = "honey.jar";
        using (FileSystemWatcher watcher = new FileSystemWatcher(path, pattern))
            ValidateDefaults(watcher, path, pattern);
    }

    [Fact]
    public static void FileSystemWatcher_ctor_InvalidStrings()
    {
        // Null filter
        Assert.Throws<ArgumentNullException>("filter", () => new FileSystemWatcher(".", null));

        // Null path
        Assert.Throws<ArgumentNullException>("path", () => new FileSystemWatcher(null));
        Assert.Throws<ArgumentNullException>("path", () => new FileSystemWatcher(null, "*"));

        // Empty path
        Assert.Throws<ArgumentException>("path", () => new FileSystemWatcher(string.Empty));
        Assert.Throws<ArgumentException>("path", () => new FileSystemWatcher(string.Empty, "*"));

        // Invalid directory
        Assert.Throws<ArgumentException>("path", () => new FileSystemWatcher(Guid.NewGuid().ToString()));
        Assert.Throws<ArgumentException>("path", () => new FileSystemWatcher(Guid.NewGuid().ToString(), "*"));
    }

    [Fact]
    public static void FileSystemWatcher_Changed()
    {
        using (FileSystemWatcher watcher = new FileSystemWatcher())
        {
            var handler = new FileSystemEventHandler((o, e) => { });

            // add / remove
            watcher.Changed += handler;
            watcher.Changed -= handler;

            // shouldn't throw
            watcher.Changed -= handler;
        }
    }

    [Fact]
    public static void FileSystemWatcher_Created()
    {
        using (FileSystemWatcher watcher = new FileSystemWatcher())
        {
            var handler = new FileSystemEventHandler((o, e) => { });

            // add / remove
            watcher.Created += handler;
            watcher.Created -= handler;

            // shouldn't throw
            watcher.Created -= handler;
        }
    }

    [Fact]
    public static void FileSystemWatcher_Deleted()
    {
        using (FileSystemWatcher watcher = new FileSystemWatcher())
        {
            var handler = new FileSystemEventHandler((o, e) => { });

            // add / remove
            watcher.Deleted += handler;
            watcher.Deleted -= handler;

            // shouldn't throw
            watcher.Deleted -= handler;
        }
    }

    [Fact]
    public static void FileSystemWatcher_Disposed()
    {
        FileSystemWatcher watcher = new FileSystemWatcher();
        watcher.Dispose();
        watcher.Dispose(); // shouldn't throw

        Assert.Throws<ObjectDisposedException>(() => watcher.EnableRaisingEvents = true);
    }

    [Fact]
    public static void FileSystemWatcher_EnableRaisingEvents()
    {
        FileSystemWatcher watcher = new FileSystemWatcher(".");
        Assert.Equal(false, watcher.EnableRaisingEvents);

        watcher.EnableRaisingEvents = true;
        Assert.Equal(true, watcher.EnableRaisingEvents);

        watcher.EnableRaisingEvents = false;
        Assert.Equal(false, watcher.EnableRaisingEvents);
    }


    [Fact]
    public static void FileSystemWatcher_Error()
    {
        using (FileSystemWatcher watcher = new FileSystemWatcher())
        {
            var handler = new ErrorEventHandler((o, e) => { });

            // add / remove
            watcher.Error += handler;
            watcher.Error -= handler;

            // shouldn't throw
            watcher.Error -= handler;
        }
    }

    [Fact]
    public static void FileSystemWatcher_Filter()
    {
        FileSystemWatcher watcher = new FileSystemWatcher();

        Assert.Equal("*.*", watcher.Filter);

        // Null and empty should be mapped to "*.*"
        watcher.Filter = null;
        Assert.Equal("*.*", watcher.Filter);

        watcher.Filter = String.Empty;
        Assert.Equal("*.*", watcher.Filter);

        watcher.Filter = " ";
        Assert.Equal(" ", watcher.Filter);

        watcher.Filter = "\0";
        Assert.Equal("\0", watcher.Filter);

        watcher.Filter = "\n";
        Assert.Equal("\n", watcher.Filter);

        watcher.Filter = "abc.dll";
        Assert.Equal("abc.dll", watcher.Filter);

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) || // expect no change for OrdinalIgnoreCase-equal strings
            RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            // expect no change for OrdinalIgnoreCase-equal strings
            // it's unclear why desktop does this but preserve it for compat        
            watcher.Filter = "ABC.DLL";
            Assert.Equal("abc.dll", watcher.Filter);
        }

        // We can make this setting by first changing to another value then back.
        watcher.Filter = null;
        watcher.Filter = "ABC.DLL";
        Assert.Equal("ABC.DLL", watcher.Filter);
    }


    [Fact]
    public static void FileSystemWatcher_IncludeSubdirectories()
    {
        FileSystemWatcher watcher = new FileSystemWatcher();
        Assert.Equal(false, watcher.IncludeSubdirectories);

        watcher.IncludeSubdirectories = true;
        Assert.Equal(true, watcher.IncludeSubdirectories);

        watcher.IncludeSubdirectories = false;
        Assert.Equal(false, watcher.IncludeSubdirectories);
    }

    [Fact]
    public static void FileSystemWatcher_InternalBufferSize()
    {
        FileSystemWatcher watcher = new FileSystemWatcher();
        Assert.Equal(8192, watcher.InternalBufferSize);

        watcher.InternalBufferSize = 20000;
        Assert.Equal(20000, watcher.InternalBufferSize);

        watcher.InternalBufferSize = int.MaxValue;
        Assert.Equal(int.MaxValue, watcher.InternalBufferSize);

        // FSW enforces a minimum value of 4096
        watcher.InternalBufferSize = 0;
        Assert.Equal(4096, watcher.InternalBufferSize);

        watcher.InternalBufferSize = -1;
        Assert.Equal(4096, watcher.InternalBufferSize);

        watcher.InternalBufferSize = int.MinValue;
        Assert.Equal(4096, watcher.InternalBufferSize);

        watcher.InternalBufferSize = 4095;
        Assert.Equal(4096, watcher.InternalBufferSize);
    }

    [Fact]
    public static void FileSystemWatcher_NotifyFilter()
    {
        FileSystemWatcher watcher = new FileSystemWatcher();
        Assert.Equal(NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName, watcher.NotifyFilter);

        var notifyFilters = Enum.GetValues(typeof(NotifyFilters)).Cast<NotifyFilters>();
        foreach (NotifyFilters filterValue in notifyFilters)
        {
            watcher.NotifyFilter = filterValue;
            Assert.Equal(filterValue, watcher.NotifyFilter);
        }

        var allFilters = notifyFilters.Aggregate((mask, flag) => mask | flag);
        watcher.NotifyFilter = allFilters;
        Assert.Equal(allFilters, watcher.NotifyFilter);

        // This doesn't make sense, but it is permitted.
        watcher.NotifyFilter = 0;
        Assert.Equal((NotifyFilters)0, watcher.NotifyFilter);

        // These throw InvalidEnumException on desktop, but ArgumentException on K
        Assert.Throws<ArgumentException>(() => watcher.NotifyFilter = (NotifyFilters)(-1));
        Assert.Throws<ArgumentException>(() => watcher.NotifyFilter = (NotifyFilters)int.MinValue);
        Assert.Throws<ArgumentException>(() => watcher.NotifyFilter = (NotifyFilters)int.MaxValue);
        Assert.Throws<ArgumentException>(() => watcher.NotifyFilter = allFilters + 1);

        // Simulate a bit added to the flags
        Assert.Throws<ArgumentException>(() => watcher.NotifyFilter = allFilters | (NotifyFilters)((int)notifyFilters.Max() << 1));
    }

    [Fact]
    public static void FileSystemWatcher_OnChanged()
    {
        using (TestFileSystemWatcher watcher = new TestFileSystemWatcher())
        {
            bool eventOccurred = false;
            object obj = null;
            FileSystemEventArgs actualArgs = null, expectedArgs = new FileSystemEventArgs(WatcherChangeTypes.Changed, "directory", "file");

            watcher.Changed += (o, e) =>
            {
                eventOccurred = true;
                obj = o;
                actualArgs = e;
            };

            watcher.CallOnChanged(expectedArgs);
            Assert.True(eventOccurred, "Event should be invoked");
            Assert.Equal(watcher, obj);
            Assert.Equal(expectedArgs, actualArgs);
        }
    }

    [Fact]
    public static void FileSystemWatcher_OnCreated()
    {
        using (TestFileSystemWatcher watcher = new TestFileSystemWatcher())
        {
            bool eventOccurred = false;
            object obj = null;
            FileSystemEventArgs actualArgs = null, expectedArgs = new FileSystemEventArgs(WatcherChangeTypes.Created, "directory", "file");

            watcher.Created += (o, e) =>
            {
                eventOccurred = true;
                obj = o;
                actualArgs = e;
            };

            watcher.CallOnCreated(expectedArgs);
            Assert.True(eventOccurred, "Event should be invoked");
            Assert.Equal(watcher, obj);
            Assert.Equal(expectedArgs, actualArgs);
        }
    }

    [Fact]
    public static void FileSystemWatcher_OnDeleted()
    {
        using (TestFileSystemWatcher watcher = new TestFileSystemWatcher())
        {
            bool eventOccurred = false;
            object obj = null;
            FileSystemEventArgs actualArgs = null, expectedArgs = new FileSystemEventArgs(WatcherChangeTypes.Deleted, "directory", "file");

            watcher.Deleted += (o, e) =>
            {
                eventOccurred = true;
                obj = o;
                actualArgs = e;
            };

            watcher.CallOnDeleted(expectedArgs);
            Assert.True(eventOccurred, "Event should be invoked");
            Assert.Equal(watcher, obj);
            Assert.Equal(expectedArgs, actualArgs);
        }
    }

    [Fact]
    public static void FileSystemWatcher_OnError()
    {
        using (TestFileSystemWatcher watcher = new TestFileSystemWatcher())
        {
            bool eventOccurred = false;
            object obj = null;
            ErrorEventArgs actualArgs = null, expectedArgs = new ErrorEventArgs(new Exception());

            watcher.Error += (o, e) =>
            {
                eventOccurred = true;
                obj = o;
                actualArgs = e;
            };

            watcher.CallOnError(expectedArgs);
            Assert.True(eventOccurred, "Event should be invoked");
            Assert.Equal(watcher, obj);
            Assert.Equal(expectedArgs, actualArgs);
        }
    }

    [Fact]
    public static void FileSystemWatcher_OnRenamed()
    {
        using (TestFileSystemWatcher watcher = new TestFileSystemWatcher())
        {
            bool eventOccurred = false;
            object obj = null;
            RenamedEventArgs actualArgs = null, expectedArgs = new RenamedEventArgs(WatcherChangeTypes.Renamed, "directory", "file", "oldFile");

            watcher.Renamed += (o, e) =>
            {
                eventOccurred = true;
                obj = o;
                actualArgs = e;
            };

            watcher.CallOnRenamed(expectedArgs);
            Assert.True(eventOccurred, "Event should be invoked");
            Assert.Equal(watcher, obj);
            Assert.Equal(expectedArgs, actualArgs);
        }
    }

    [Fact]
    public static void FileSystemWatcher_Path()
    {
        FileSystemWatcher watcher = new FileSystemWatcher();
        Assert.Equal(String.Empty, watcher.Path);

        watcher.Path = null;
        Assert.Equal(String.Empty, watcher.Path);

        watcher.Path = ".";
        Assert.Equal(".", watcher.Path);

        watcher.Path = "..";
        Assert.Equal("..", watcher.Path);

        string currentDir = Path.GetFullPath(".").TrimEnd('.', Path.DirectorySeparatorChar);
        watcher.Path = currentDir;
        Assert.Equal(currentDir, watcher.Path);

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) || // expect no change for OrdinalIgnoreCase-equal strings
            RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            watcher.Path = currentDir.ToUpperInvariant();
            Assert.Equal(currentDir, watcher.Path);

            watcher.Path = currentDir.ToLowerInvariant();
            Assert.Equal(currentDir, watcher.Path);
        }

        // expect a change for same "full-path" but different string path, FSW does not normalize
        string currentDirRelative = currentDir +
            Path.DirectorySeparatorChar + "." +
            Path.DirectorySeparatorChar + "." +
            Path.DirectorySeparatorChar + "." +
            Path.DirectorySeparatorChar + ".";
        watcher.Path = currentDirRelative;
        Assert.Equal(currentDirRelative, watcher.Path);

        // FSW starts with String.Empty and will ignore setting this if it is already set,
        // but if you set it after some other valid string has been set it will throw.
        Assert.Throws<ArgumentException>(() => watcher.Path = String.Empty);
        // Non-existent path
        Assert.Throws<ArgumentException>(() => watcher.Path = Guid.NewGuid().ToString());
        // Web path
        Assert.Throws<ArgumentException>(() => watcher.Path = "http://localhost");
        // File protocol
        Assert.Throws<ArgumentException>(() => watcher.Path = "file:///" + currentDir.Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar));
    }

    [Fact]
    public static void FileSystemWatcher_Renamed()
    {
        using (FileSystemWatcher watcher = new FileSystemWatcher())
        {
            var handler = new RenamedEventHandler((o, e) => { });

            // add / remove
            watcher.Renamed += handler;
            watcher.Renamed -= handler;

            // shouldn't throw
            watcher.Renamed -= handler;
        }
    }

    [PlatformSpecific(PlatformID.Linux)]
    [Fact]
    public static void FileSystemWatcher_CreateManyConcurrentInstances()
    {
        int maxUserInstances = int.Parse(File.ReadAllText("/proc/sys/fs/inotify/max_user_instances"));
        var watchers = new List<FileSystemWatcher>();

        using (var dir = Utility.CreateTestDirectory())
        {
            try
            {
                Assert.Throws<IOException>(() =>
                {
                    // Create enough inotify instances to exceed the number of allowed watches
                    for (int i = 0; i <= maxUserInstances; i++)
                    {
                        watchers.Add(new FileSystemWatcher(dir.Path) { EnableRaisingEvents = true });
                    }
                });
            }
            finally
            {
                foreach (FileSystemWatcher watcher in watchers)
                {
                    watcher.Dispose();
                }
            }
        }
    }

    [PlatformSpecific(PlatformID.Linux)]
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public static void FileSystemWatcher_CreateManyConcurrentWatches(bool enableBeforeCreatingWatches)
    {
        int maxUserWatches = int.Parse(File.ReadAllText("/proc/sys/fs/inotify/max_user_watches"));

        using (var dir = Utility.CreateTestDirectory())
        using (var watcher = new FileSystemWatcher(dir.Path) { IncludeSubdirectories = true, NotifyFilter = NotifyFilters.FileName })
        {
            Exception exc = null;
            ManualResetEventSlim mres = new ManualResetEventSlim();
            watcher.Error += (s, e) =>
            {
                exc = e.GetException();
                mres.Set();
            };

            if (enableBeforeCreatingWatches)
                watcher.EnableRaisingEvents = true;

            // Create enough directories to exceed the number of allowed watches
            for (int i = 0; i <= maxUserWatches; i++)
            {
                Directory.CreateDirectory(Path.Combine(dir.Path, i.ToString()));
            }

            if (!enableBeforeCreatingWatches)
                watcher.EnableRaisingEvents = true;

            Assert.True(mres.Wait(Utility.WaitForExpectedEventTimeout));
            Assert.IsType<IOException>(exc);

            // Make sure existing watches still work even after we've had one or more failures
            AutoResetEvent are = Utility.WatchForEvents(watcher, WatcherChangeTypes.Created);
            Utility.CreateTestFile(Path.Combine(dir.Path, Path.GetRandomFileName())).Dispose();
            Utility.ExpectEvent(are, "file created");
        }
    }

    [Fact]
    public static void FileSystemWatcher_StopCalledOnBackgroundThreadDoesNotDeadlock()
    {
        // Check the case where Stop or Dispose (they do the same thing) is called from 
        // a FSW event callback and make sure we don't Thread.Join to deadlock
        using (var dir = Utility.CreateTestDirectory())
        {
            FileSystemWatcher watcher = new FileSystemWatcher();
            AutoResetEvent are = new AutoResetEvent(false);

            FileSystemEventHandler callback = (sender, arg) => {
                watcher.Dispose();
                are.Set();
            };

            // Attach the FSW to the existing structure
            watcher.Path = Path.GetFullPath(dir.Path);
            watcher.Filter = "*";
            watcher.NotifyFilter = NotifyFilters.FileName | NotifyFilters.Size;
            watcher.Changed += callback;

            using (var file = File.Create(Path.Combine(dir.Path, "testfile.txt")))
            {
                watcher.EnableRaisingEvents = true;

                // Change the nested file and verify we get the changed event
                byte[] bt = new byte[4096];
                file.Write(bt, 0, bt.Length);
                file.Flush();
            }

            are.WaitOne(Utility.WaitForExpectedEventTimeout);
        }
    }

    [Fact]
    public static void FileSystemWatcher_WatchingAliasedFolderResolvesToRealPathWhenWatching()
    {
        using (var dir = Utility.CreateTestDirectory(Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString())))
        using (var fsw = new FileSystemWatcher(dir.Path))
        {
            AutoResetEvent are = Utility.WatchForEvents(fsw, WatcherChangeTypes.Created);

            fsw.Filter = "*";
            fsw.EnableRaisingEvents = true;

            using (var temp = Utility.CreateTestDirectory(Path.Combine(dir.Path, "foo")))
            {
                Utility.ExpectEvent(are, "created");
            }
        }
    }
}
