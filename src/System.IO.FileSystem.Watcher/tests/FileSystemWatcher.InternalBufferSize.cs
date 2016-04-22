// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using System.Threading;
using Xunit;

namespace System.IO.Tests
{
    public class InternalBufferSizeTests : FileSystemWatcherTest
    {
        [Fact]
        [PlatformSpecific(PlatformID.Windows)]
        public void FileSystemWatcher_InternalBufferSize_Error()
        {
            using (var testDirectory = new TempDirectory(GetTestFilePath()))
            using (var file = new TempFile(Path.Combine(testDirectory.Path, GetTestFileName())))
            using (var watcher = new FileSystemWatcher(testDirectory.Path))
            {
                watcher.Filter = Path.GetFileName(file.Path);
                ManualResetEvent unblockHandler = new ManualResetEvent(false);
                watcher.Changed += (o, e) =>
                {
                    // block the handling thread
                    unblockHandler.WaitOne();
                };

                AutoResetEvent eventOccurred = new AutoResetEvent(false);
                watcher.Error += (o, e) =>
                {
                    eventOccurred.Set();
                };
                watcher.EnableRaisingEvents = true;

                // See note in FileSystemWatcher_Error_File
                int originalInternalBufferOperationCapacity = watcher.InternalBufferSize / (12 + 2 * (file.Path.Length + 1));
                for (int i = 1; i < originalInternalBufferOperationCapacity * 4; i++)
                {
                    File.SetLastWriteTime(file.Path, DateTime.Now + TimeSpan.FromSeconds(i));
                }

                unblockHandler.Set();
                ExpectEvent(eventOccurred, "error");
            }
        }

        [Fact]
        [PlatformSpecific(PlatformID.Windows)]
        public void FileSystemWatcher_InternalBufferSize_Set()
        {
            using (var testDirectory = new TempDirectory(GetTestFilePath()))
            using (var file = new TempFile(Path.Combine(testDirectory.Path, GetTestFileName())))
            using (var watcher = new FileSystemWatcher(testDirectory.Path))
            {
                watcher.Filter = Path.GetFileName(file.Path);
                ManualResetEvent unblockHandler = new ManualResetEvent(false);
                watcher.Changed += (o, e) =>
                {
                    // block the handling thread
                    unblockHandler.WaitOne();
                };

                AutoResetEvent eventOccurred = new AutoResetEvent(false);
                watcher.Error += (o, e) =>
                {
                    eventOccurred.Set();
                };
                watcher.EnableRaisingEvents = true;

                // See note in FileSystemWatcher_Error_File
                int originalInternalBufferOperationCapacity = watcher.InternalBufferSize / (16 + 2 * (file.Path.Length + 1));
                watcher.InternalBufferSize = watcher.InternalBufferSize * 5;

                // Send the same number of events.
                for (int i = 1; i < originalInternalBufferOperationCapacity * 4; i++)
                {
                    File.SetLastWriteTime(file.Path, DateTime.Now + TimeSpan.FromSeconds(i));
                }
                unblockHandler.Set();
                // This time we should not see an error
                ExpectNoEvent(eventOccurred, "error");
            }
        }
    }
}