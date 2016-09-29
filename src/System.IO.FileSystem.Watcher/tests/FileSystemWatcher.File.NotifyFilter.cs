// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace System.IO.Tests
{
    public class File_NotifyFilter_Tests : FileSystemWatcherTest
    {
        [DllImport("api-ms-win-security-provider-l1-1-0.dll", EntryPoint = "SetNamedSecurityInfoW",
            CallingConvention = CallingConvention.Winapi, SetLastError = true, ExactSpelling = true, CharSet = CharSet.Unicode)]
        private static extern uint SetSecurityInfoByHandle(string name, uint objectType, uint securityInformation,
            IntPtr owner, IntPtr group, IntPtr dacl, IntPtr sacl);

        private const uint ERROR_SUCCESS = 0;
        private const uint DACL_SECURITY_INFORMATION = 0x00000004;
        private const uint SE_FILE_OBJECT = 0x1;

        [Theory]
        [MemberData(nameof(FilterTypes))]
        public void FileSystemWatcher_File_NotifyFilter_Attributes(NotifyFilters filter)
        {
            using (var testDirectory = new TempDirectory(GetTestFilePath()))
            using (var file = new TempFile(Path.Combine(testDirectory.Path, "file")))
            using (var watcher = new FileSystemWatcher(testDirectory.Path, Path.GetFileName(file.Path)))
            {
                watcher.NotifyFilter = filter;
                var attributes = File.GetAttributes(file.Path);

                Action action = () => File.SetAttributes(file.Path, attributes | FileAttributes.ReadOnly);
                Action cleanup = () => File.SetAttributes(file.Path, attributes);

                WatcherChangeTypes expected = 0;
                if (filter == NotifyFilters.Attributes)
                    expected |= WatcherChangeTypes.Changed;
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) && ((filter & LinuxFiltersForAttribute) > 0))
                    expected |= WatcherChangeTypes.Changed;
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX) && ((filter & OSXFiltersForModify) > 0))
                    expected |= WatcherChangeTypes.Changed;
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX) && ((filter & NotifyFilters.Security) > 0))
                    expected |= WatcherChangeTypes.Changed; // Attribute change on OSX is a ChangeOwner operation which passes the Security NotifyFilter.
                
                ExpectEvent(watcher, expected, action, cleanup, file.Path);
            }
        }

        [Theory]
        [MemberData(nameof(FilterTypes))]
        public void FileSystemWatcher_File_NotifyFilter_CreationTime(NotifyFilters filter)
        {
            using (var testDirectory = new TempDirectory(GetTestFilePath()))
            using (var file = new TempFile(Path.Combine(testDirectory.Path, "file")))
            using (var watcher = new FileSystemWatcher(testDirectory.Path, Path.GetFileName(file.Path)))
            {
                watcher.NotifyFilter = filter;
                Action action = () => File.SetCreationTime(file.Path, DateTime.Now + TimeSpan.FromSeconds(10));

                WatcherChangeTypes expected = 0;
                if (filter == NotifyFilters.CreationTime)
                    expected |= WatcherChangeTypes.Changed;
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) && ((filter & LinuxFiltersForAttribute) > 0))
                    expected |= WatcherChangeTypes.Changed;
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX) && ((filter & OSXFiltersForModify) > 0))
                    expected |= WatcherChangeTypes.Changed;
                
                ExpectEvent(watcher, expected, action, expectedPath: file.Path);
            }
        }

        [Theory]
        [MemberData(nameof(FilterTypes))]
        public void FileSystemWatcher_File_NotifyFilter_DirectoryName(NotifyFilters filter)
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

                WatcherChangeTypes expected = 0;

                if (filter == NotifyFilters.DirectoryName)
                    expected |= WatcherChangeTypes.Renamed;
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) && (filter == NotifyFilters.FileName))
                    expected |= WatcherChangeTypes.Renamed;

                ExpectEvent(watcher, expected, action, cleanup, targetPath);
            }
        }

        [Theory]
        [MemberData(nameof(FilterTypes))]
        public void FileSystemWatcher_File_NotifyFilter_LastAccessTime(NotifyFilters filter)
        {
            using (var testDirectory = new TempDirectory(GetTestFilePath()))
            using (var file = new TempFile(Path.Combine(testDirectory.Path, "file")))
            using (var watcher = new FileSystemWatcher(testDirectory.Path, Path.GetFileName(file.Path)))
            {
                watcher.NotifyFilter = filter;
                Action action = () => File.SetLastAccessTime(file.Path, DateTime.Now + TimeSpan.FromSeconds(10));

                WatcherChangeTypes expected = 0;
                if (filter == NotifyFilters.LastAccess)
                    expected |= WatcherChangeTypes.Changed;
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) && ((filter & LinuxFiltersForAttribute) > 0))
                    expected |= WatcherChangeTypes.Changed;
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX) && ((filter & OSXFiltersForModify) > 0))
                    expected |= WatcherChangeTypes.Changed;
                ExpectEvent(watcher, expected, action, expectedPath: file.Path);
            }
        }

        [Theory]
        [MemberData(nameof(FilterTypes))]
        public void FileSystemWatcher_File_NotifyFilter_LastWriteTime(NotifyFilters filter)
        {
            using (var testDirectory = new TempDirectory(GetTestFilePath()))
            using (var file = new TempFile(Path.Combine(testDirectory.Path, "file")))
            using (var watcher = new FileSystemWatcher(testDirectory.Path, Path.GetFileName(file.Path)))
            {
                watcher.NotifyFilter = filter;
                Action action = () => File.SetLastWriteTime(file.Path, DateTime.Now + TimeSpan.FromSeconds(10));

                WatcherChangeTypes expected = 0;
                if (filter == NotifyFilters.LastWrite)
                    expected |= WatcherChangeTypes.Changed;
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) && ((filter & LinuxFiltersForAttribute) > 0))
                    expected |= WatcherChangeTypes.Changed;
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX) && ((filter & OSXFiltersForModify) > 0))
                    expected |= WatcherChangeTypes.Changed;
                ExpectEvent(watcher, expected, action, expectedPath: file.Path);
            }
        }

        [Theory]
        [MemberData(nameof(FilterTypes))]
        public void FileSystemWatcher_File_NotifyFilter_Size(NotifyFilters filter)
        {
            using (var testDirectory = new TempDirectory(GetTestFilePath()))
            using (var file = new TempFile(Path.Combine(testDirectory.Path, "file")))
            using (var watcher = new FileSystemWatcher(testDirectory.Path, Path.GetFileName(file.Path)))
            {
                watcher.NotifyFilter = filter;
                Action action = () => File.AppendAllText(file.Path, "longText!");
                Action cleanup = () => File.AppendAllText(file.Path, "short");

                WatcherChangeTypes expected = 0;
                if (filter == NotifyFilters.Size || filter == NotifyFilters.LastWrite)
                    expected |= WatcherChangeTypes.Changed;
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) && ((filter & LinuxFiltersForModify) > 0))
                    expected |= WatcherChangeTypes.Changed;
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX) && ((filter & OSXFiltersForModify) > 0))
                    expected |= WatcherChangeTypes.Changed;
                else if (PlatformDetection.IsWindows7 && filter == NotifyFilters.Attributes) // win7 FSW Size change passes the Attribute filter
                    expected |= WatcherChangeTypes.Changed;
                ExpectEvent(watcher, expected, action, expectedPath: file.Path);
            }
        }

        [Theory]
        [MemberData(nameof(FilterTypes))]
        [PlatformSpecific(PlatformID.Windows)]
        public void FileSystemWatcher_File_NotifyFilter_Security(NotifyFilters filter)
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
                    File.AppendAllText(file.Path, "text");
                };

                WatcherChangeTypes expected = 0;
                if (filter == NotifyFilters.Security)
                    expected |= WatcherChangeTypes.Changed;
                else if (PlatformDetection.IsWindows7 && filter == NotifyFilters.Attributes) // win7 FSW Security change passes the Attribute filter
                    expected |= WatcherChangeTypes.Changed;
                ExpectEvent(watcher, expected, action, expectedPath: file.Path);
            }
        }

        /// <summary>
        /// Tests a changed event on a directory when filtering for LastWrite and FileName.
        /// </summary>
        [Fact]
        public void FileSystemWatcher_File_NotifyFilter_LastWriteAndFileName()
        {
            using (var testDirectory = new TempDirectory(GetTestFilePath()))
            using (var dir = new TempDirectory(Path.Combine(testDirectory.Path, "dir")))
            using (var watcher = new FileSystemWatcher(testDirectory.Path, Path.GetFileName(dir.Path)))
            {
                NotifyFilters filter = NotifyFilters.LastWrite | NotifyFilters.FileName;
                watcher.NotifyFilter = filter;

                Action action = () => Directory.SetLastWriteTime(dir.Path, DateTime.Now + TimeSpan.FromSeconds(10));

                ExpectEvent(watcher, WatcherChangeTypes.Changed, action, expectedPath: dir.Path);
            }
        }

        /// <summary>
        /// Tests the watcher behavior when two events - a Modification and a Creation - happen closely
        /// after each other.
        /// </summary>
        [Fact]
        public void FileSystemWatcher_File_NotifyFilter_ModifyAndCreate()
        {
            using (var testDirectory = new TempDirectory(GetTestFilePath()))
            using (var file = new TempFile(Path.Combine(testDirectory.Path, "file")))
            using (var watcher = new FileSystemWatcher(testDirectory.Path, "*"))
            {
                watcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName;
                string otherFile = Path.Combine(testDirectory.Path, "file2");

                Action action = () =>
                {
                    File.Create(otherFile).Dispose();
                    File.SetLastWriteTime(file.Path, DateTime.Now + TimeSpan.FromSeconds(10));
                };
                Action cleanup = () => File.Delete(otherFile);

                WatcherChangeTypes expected = 0;
                expected |= WatcherChangeTypes.Created | WatcherChangeTypes.Changed;
                ExpectEvent(watcher, expected, action, cleanup, new string[] { otherFile, file.Path });
            }
        }

        /// <summary>
        /// Tests the watcher behavior when two events - a Modification and a Deletion - happen closely
        /// after each other.
        /// </summary>
        [Fact]
        public void FileSystemWatcher_File_NotifyFilter_ModifyAndDelete()
        {
            using (var testDirectory = new TempDirectory(GetTestFilePath()))
            using (var file = new TempFile(Path.Combine(testDirectory.Path, "file")))
            using (var watcher = new FileSystemWatcher(testDirectory.Path, "*"))
            {
                watcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName;
                string otherFile = Path.Combine(testDirectory.Path, "file2");

                Action action = () =>
                {
                    File.Delete(otherFile);
                    File.SetLastWriteTime(file.Path, DateTime.Now + TimeSpan.FromSeconds(10));
                };
                Action cleanup = () =>
                {
                    File.Create(otherFile).Dispose();
                };
                cleanup();

                WatcherChangeTypes expected = 0;
                expected |= WatcherChangeTypes.Deleted | WatcherChangeTypes.Changed;
                ExpectEvent(watcher, expected, action, cleanup, new string[] { otherFile, file.Path });
            }
        }
    }
}