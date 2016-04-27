// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Runtime.InteropServices;
using Xunit;

namespace System.IO.Tests
{
    public class NotifyFilterTests : FileSystemWatcherTest
    {
        [DllImport(
        "api-ms-win-security-provider-l1-1-0.dll",
        EntryPoint = "SetNamedSecurityInfoW",
        CallingConvention = CallingConvention.Winapi,
        SetLastError = true,
        ExactSpelling = true,
        CharSet = CharSet.Unicode)]
        private static extern uint SetSecurityInfoByHandle(
        string name,
        uint objectType,
        uint securityInformation,
        IntPtr owner,
        IntPtr group,
        IntPtr dacl,
        IntPtr sacl);

        private const uint ERROR_SUCCESS = 0;
        private const uint DACL_SECURITY_INFORMATION = 0x00000004;
        private const uint SE_FILE_OBJECT = 0x1;

        public static IEnumerable<object[]> FilterTypes()
        {
            foreach (NotifyFilters filter in Enum.GetValues(typeof(NotifyFilters)))
                yield return new object[] { filter };
        }

        [Theory]
        [MemberData(nameof(FilterTypes))]
        //[ActiveIssue(2011, PlatformID.OSX)]
        public void FileSystemWatcher_NotifyFilter_Attributes(NotifyFilters filter)
        {
            using (var testDirectory = new TempDirectory(GetTestFilePath()))
            using (var file = new TempFile(Path.Combine(testDirectory.Path, "file")))
            using (var watcher = new FileSystemWatcher(testDirectory.Path, Path.GetFileName(file.Path)))
            {
                watcher.NotifyFilter = filter;
                var attributes = File.GetAttributes(file.Path);

                Action action = () => File.SetAttributes(file.Path, attributes | FileAttributes.ReadOnly);
                Action cleanup = () => File.SetAttributes(file.Path, attributes);

                if (filter == NotifyFilters.Attributes)
                    ExpectEvent(watcher, WatcherChangeTypes.Changed, action, cleanup);
                else
                    ExpectNoEvent(watcher, WatcherChangeTypes.Changed, action, cleanup);
            }
        }

        [Theory]
        [MemberData(nameof(FilterTypes))]
        //[ActiveIssue(2011, PlatformID.OSX)]
        public void FileSystemWatcher_NotifyFilter_CreationTime(NotifyFilters filter)
        {
            using (var testDirectory = new TempDirectory(GetTestFilePath()))
            using (var file = new TempFile(Path.Combine(testDirectory.Path, "file")))
            using (var watcher = new FileSystemWatcher(testDirectory.Path, Path.GetFileName(file.Path)))
            {
                watcher.NotifyFilter = filter;
                Action action = () => File.SetCreationTime(file.Path, DateTime.Now + TimeSpan.FromSeconds(10));

                if (filter == NotifyFilters.CreationTime)
                    ExpectEvent(watcher, WatcherChangeTypes.Changed, action);
                else if (filter == NotifyFilters.LastAccess && !RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                    ExpectEvent(watcher, WatcherChangeTypes.Changed, action); // Unix FS sets LastAccess on SetCreationTime.
                else
                    ExpectNoEvent(watcher, WatcherChangeTypes.Changed, action);
            }
        }

        [Theory]
        [MemberData(nameof(FilterTypes))]
        public void FileSystemWatcher_NotifyFilter_DirectoryName(NotifyFilters filter)
        {
            using (var testDirectory = new TempDirectory(GetTestFilePath()))
            using (var dir = new TempDirectory(Path.Combine(testDirectory.Path, "dir")))
            using (var watcher = new FileSystemWatcher(testDirectory.Path, Path.GetFileName(dir.Path)))
            {
                string sourcePath = dir.Path;
                string targetPath = Path.Combine(testDirectory.Path, "targetDir");
                watcher.NotifyFilter = filter;

                Action action = () => Directory.Move(sourcePath, targetPath);
                Action cleanup = () => Directory.Move(targetPath, sourcePath);

                if (filter == NotifyFilters.DirectoryName)
                    ExpectEvent(watcher, WatcherChangeTypes.Renamed, action, cleanup);
                else
                    ExpectNoEvent(watcher, WatcherChangeTypes.Renamed, action, cleanup);
            }
        }

        [Theory]
        [MemberData(nameof(FilterTypes))]
        //[ActiveIssue(2011, PlatformID.OSX)]
        public void FileSystemWatcher_NotifyFilter_LastAccessTime(NotifyFilters filter)
        {
            using (var testDirectory = new TempDirectory(GetTestFilePath()))
            using (var file = new TempFile(Path.Combine(testDirectory.Path, "file")))
            using (var watcher = new FileSystemWatcher(testDirectory.Path, Path.GetFileName(file.Path)))
            {
                watcher.NotifyFilter = filter;
                Action action = () => File.SetLastAccessTime(file.Path, DateTime.Now + TimeSpan.FromSeconds(10));

                if (filter == NotifyFilters.LastAccess)
                    ExpectEvent(watcher, WatcherChangeTypes.Changed, action);
                else
                    ExpectNoEvent(watcher, WatcherChangeTypes.Changed, action);
            }
        }

        [Theory]
        [MemberData(nameof(FilterTypes))]
        //[ActiveIssue(2011, PlatformID.OSX)]
        public void FileSystemWatcher_NotifyFilter_LastWriteTime(NotifyFilters filter)
        {
            using (var testDirectory = new TempDirectory(GetTestFilePath()))
            using (var file = new TempFile(Path.Combine(testDirectory.Path, "file")))
            using (var watcher = new FileSystemWatcher(testDirectory.Path, Path.GetFileName(file.Path)))
            {
                watcher.NotifyFilter = filter;
                Action action = () => File.SetLastWriteTime(file.Path, DateTime.Now + TimeSpan.FromSeconds(10));

                if (filter == NotifyFilters.LastWrite)
                    ExpectEvent(watcher, WatcherChangeTypes.Changed, action);
                else
                    ExpectNoEvent(watcher, WatcherChangeTypes.Changed, action);
            }
        }

        [Theory]
        [MemberData(nameof(FilterTypes))]
        //[ActiveIssue(2011, PlatformID.OSX)]
        public void FileSystemWatcher_NotifyFilter_Size(NotifyFilters filter)
        {
            using (var testDirectory = new TempDirectory(GetTestFilePath()))
            using (var file = new TempFile(Path.Combine(testDirectory.Path, "file")))
            using (var watcher = new FileSystemWatcher(testDirectory.Path, Path.GetFileName(file.Path)))
            {
                watcher.NotifyFilter = filter;
                Action action = () => File.WriteAllText(file.Path, "longText!");
                Action cleanup = () => File.WriteAllText(file.Path, "short");

                if (filter == NotifyFilters.Size || filter == NotifyFilters.LastWrite)
                    ExpectEvent(watcher, WatcherChangeTypes.Changed, action, cleanup);
                else
                    ExpectNoEvent(watcher, WatcherChangeTypes.Changed, action, cleanup);
            }
        }

        [Theory]
        [MemberData(nameof(FilterTypes))]
        [PlatformSpecific(PlatformID.Windows)]
        public void FileSystemWatcher_NotifyFilter_Security(NotifyFilters filter)
        {
            using (var testDirectory = new TempDirectory(GetTestFilePath()))
            using (var file = new TempFile(Path.Combine(testDirectory.Path, "file")))
            using (var watcher = new FileSystemWatcher(testDirectory.Path, Path.GetFileName(file.Path)))
            {
                watcher.NotifyFilter = filter;
                Action action = () =>
                {
                    // ACL support is not yet available, so pinvoke directly.
                    uint result = SetSecurityInfoByHandle(file.Path,
                        SE_FILE_OBJECT,
                        DACL_SECURITY_INFORMATION, // Only setting the DACL
                        owner: IntPtr.Zero,
                        group: IntPtr.Zero,
                        dacl: IntPtr.Zero, // full access to everyone
                        sacl: IntPtr.Zero);
                    Assert.Equal(ERROR_SUCCESS, result);
                };
                Action cleanup = () =>
                {
                    // Recreate the file.
                    File.Delete(file.Path);
                    File.WriteAllText(file.Path, "text");
                };

                if (filter == NotifyFilters.Security)
                    ExpectEvent(watcher, WatcherChangeTypes.Changed, action, cleanup);
                else
                    ExpectNoEvent(watcher, WatcherChangeTypes.Changed, action, cleanup);
            }
        }
    }
}