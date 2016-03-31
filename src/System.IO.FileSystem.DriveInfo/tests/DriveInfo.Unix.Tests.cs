// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Linq;
using Xunit;

namespace System.IO.FileSystem.DriveInfoTests
{
    public partial class DriveInfoUnixTests
    {
        [Fact]
        [PlatformSpecific(PlatformID.AnyUnix)]
        public void TestConstructor()
        {
            Assert.All(
                new[] { "", "\0", "\0/" },
                driveName => Assert.Throws<ArgumentException>("driveName", () => { new DriveInfo(driveName); }));

            Assert.Throws<ArgumentNullException>("driveName", () => { new DriveInfo(null); });

            Assert.Equal("/", new DriveInfo("/").Name);
        }

        [Fact]
        [PlatformSpecific(PlatformID.AnyUnix)]
        public void TestGetDrives()
        {
            var drives = DriveInfo.GetDrives();
            Assert.NotNull(drives);
            Assert.True(drives.Length > 0, "Expected at least one drive");
            Assert.All(drives, d => Assert.NotNull(d));
            Assert.Contains(drives, d => d.Name == "/");
            Assert.All(drives, d =>
            {
                // None of these should throw
                DriveType dt = d.DriveType;
                bool isReady = d.IsReady;
                DirectoryInfo di = d.RootDirectory;
            });
        }

        [Fact]
        [PlatformSpecific(PlatformID.AnyUnix)]
        public void PropertiesOfInvalidDrive()
        {
            string invalidDriveName = "NonExistentDriveName";
            var invalidDrive = new DriveInfo(invalidDriveName);

            Assert.Throws<DriveNotFoundException>(() =>invalidDrive.AvailableFreeSpace);
            Assert.Throws<DriveNotFoundException>(() => invalidDrive.DriveFormat);
            Assert.Equal(DriveType.NoRootDirectory, invalidDrive.DriveType);
            Assert.False(invalidDrive.IsReady);
            Assert.Equal(invalidDriveName, invalidDrive.Name);
            Assert.Equal(invalidDriveName, invalidDrive.ToString());
            Assert.Equal(invalidDriveName, invalidDrive.RootDirectory.Name);
            Assert.Throws<DriveNotFoundException>(() => invalidDrive.TotalFreeSpace);
            Assert.Throws<DriveNotFoundException>(() => invalidDrive.TotalSize);
            Assert.Equal(invalidDriveName, invalidDrive.VolumeLabel);   // VolumeLabel is equivalent to Name on Unix
        }

        [Fact]
        [PlatformSpecific(PlatformID.AnyUnix)]
        public void PropertiesOfValidDrive()
        {
            var root = new DriveInfo("/");
            Assert.True(root.AvailableFreeSpace > 0);
            var format = root.DriveFormat;
            Assert.Equal(DriveType.Fixed, root.DriveType);
            Assert.True(root.IsReady);
            Assert.Equal("/", root.Name);
            Assert.Equal("/", root.ToString());
            Assert.Equal("/", root.RootDirectory.FullName);
            Assert.True(root.TotalFreeSpace > 0);
            Assert.True(root.TotalSize > 0);
            Assert.Equal("/", root.VolumeLabel);
        }

        [Fact]
        [PlatformSpecific(PlatformID.AnyUnix)]
        public void SetVolumeLabel_Throws_PlatformNotSupportedException()
        {
            var root = new DriveInfo("/");
            Assert.Throws<PlatformNotSupportedException>(() => root.VolumeLabel = root.Name);
        }
    }
}
