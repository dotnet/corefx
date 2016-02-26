// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using Xunit;

public class FileSystemWatcherLockTest
{
    [Fact]
    public static void FileSystemWatcher_WatchLocksWatchedDirectory()
    {
        using (var dir = Utility.CreateTestDirectory())
        using (var fsw = new FileSystemWatcher())
        {
            fsw.Path = Path.GetFullPath(dir.Path);
            fsw.EnableRaisingEvents = true;

            Microsoft.Win32.SafeHandles.SafeFileHandle handle = Interop.Sys.Open(fsw.Path, Interop.Sys.OpenFlags.O_RDONLY, 0);
            Assert.False(handle.IsInvalid); // We should always be able to open a handle
            Assert.True(Interop.Sys.FLock(handle, Interop.Sys.LockOperations.LOCK_EX | Interop.Sys.LockOperations.LOCK_NB) < 0);

            Interop.ErrorInfo err = Interop.Sys.GetLastErrorInfo();
            Assert.True(err.Error == Interop.Error.EWOULDBLOCK); // validate it is a sharing violation
        }
    }
}
