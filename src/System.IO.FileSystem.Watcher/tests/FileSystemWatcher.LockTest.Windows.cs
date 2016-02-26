// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using Xunit;

public class FileSystemWatcheLockTest
{
    [Fact]
    public static void FileSystemWatcher_WatchLocksWatchedDirectory()
    {
        using (var dir = Utility.CreateTestDirectory())
        using (var fsw = new FileSystemWatcher())
        {
            fsw.Path = Path.GetFullPath(dir.Path);
            fsw.EnableRaisingEvents = true;

            Interop.mincore.SECURITY_ATTRIBUTES attr = default(Interop.mincore.SECURITY_ATTRIBUTES);
            Microsoft.Win32.SafeHandles.SafeFileHandle handle = Interop.mincore.CreateFile(
                fsw.Path,
                (int)FileAccess.Read,
                FileShare.None,
                ref attr,
                FileMode.Open,
                Interop.mincore.FileOperations.FILE_FLAG_BACKUP_SEMANTICS,
                IntPtr.Zero);
            Assert.True(handle.IsInvalid);
            Assert.True(System.Runtime.InteropServices.Marshal.GetLastWin32Error() == 32); // validate it was a sharing violation
        }
    }
}
