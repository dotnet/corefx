// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Xunit;

namespace System.IO.FileSystem.Tests
{
    public class Directory_Exists : FileSystemTest
    {
        #region Utilities

        public bool Exists(string path)
        {
            return Directory.Exists(path);
        }

        #endregion
        
        #region UniversalTests

        [Fact]
        public void NullAsPath_ReturnsFalse()
        {
            Assert.False(Exists(null));
        }

        [Fact]
        public void EmptyAsPath_ReturnsFalse()
        {
            Assert.False(Exists(string.Empty));
        }

        [Fact]
        public void NonExistentValidPath_ReturnsFalse()
        {
            Assert.All((IOInputs.GetValidPathComponentNames()), (path) =>
            {
                Assert.False(Exists(path), path);
            });
        }

        [Fact]
        public void ValidPathExists_ReturnsTrue()
        {
            Assert.All((IOInputs.GetValidPathComponentNames()), (component) =>
            {
                string path = Path.Combine(TestDirectory, component);
                DirectoryInfo testDir = Directory.CreateDirectory(path);
                Assert.True(Exists(path));
            });
        }

        [Fact]
        public void PathWithInvalidCharactersAsPath_ReturnsFalse()
        {
            // Checks that errors aren't thrown when calling Exists() on paths with impossible to create characters
            Assert.All((IOInputs.GetPathsWithInvalidCharacters()), (component) =>
            {
                Assert.False(Exists(component));
            });
        }

        [Fact]
        public void PathAlreadyExistsAsFile()
        {
            string path = GetTestFilePath();
            File.Create(path).Dispose();

            Assert.False(Exists(IOServices.RemoveTrailingSlash(path)));
            Assert.False(Exists(IOServices.RemoveTrailingSlash(IOServices.RemoveTrailingSlash(path))));
            Assert.False(Exists(IOServices.RemoveTrailingSlash(IOServices.AddTrailingSlashIfNeeded(path))));
        }

        [Fact]
        public void PathAlreadyExistsAsDirectory()
        {
            string path = GetTestFilePath();
            DirectoryInfo testDir = Directory.CreateDirectory(path);

            Assert.True(Exists(IOServices.RemoveTrailingSlash(path)));
            Assert.True(Exists(IOServices.RemoveTrailingSlash(IOServices.RemoveTrailingSlash(path))));
            Assert.True(Exists(IOServices.RemoveTrailingSlash(IOServices.AddTrailingSlashIfNeeded(path))));
        }

        [Fact]
        public void DotAsPath_ReturnsTrue()
        {
            Assert.True(Exists(Path.Combine(TestDirectory, ".")));
        }

        [Fact]
        public void DirectoryGetCurrentDirectoryAsPath_ReturnsTrue()
        {
            Assert.True(Exists(Directory.GetCurrentDirectory()));
        }

        [Fact]
        public void DotDotAsPath_ReturnsTrue()
        {
            Assert.True(Exists(Path.Combine(TestDirectory, GetTestFileName(), "..")));
        }

        [Fact]
        public void DirectoryLongerThanMaxPathAsPath_DoesntThrow()
        {
            Assert.All((IOInputs.GetPathsLongerThanMaxPath()), (path) =>
            {
                Assert.False(Exists(path), path);
            });
        }

        #endregion

        #region PlatformSpecific

        [Fact]
        [PlatformSpecific(PlatformID.Windows)]
        public void DirectoryLongerThanMaxDirectoryAsPath_DoesntThrow()
        {
            Assert.All((IOInputs.GetPathsLongerThanMaxDirectory()), (path) =>
            {
                Assert.False(Exists(path));
            });
        }

        [Fact]
        [PlatformSpecific(PlatformID.Windows)] // Unix equivalent tested already in CreateDirectory
        public void WindowsNonSignificantWhiteSpaceAsPath_ReturnsFalse()
        {
            // Checks that errors aren't thrown when calling Exists() on impossible paths
            Assert.All((IOInputs.GetNonSignificantTrailingWhiteSpace()), (component) =>
            {
                Assert.False(Exists(component));
            });
        }

        [Fact]
        [PlatformSpecific(PlatformID.Windows | PlatformID.OSX)]
        public void DoesCaseInsensitiveInvariantComparions()
        {
            DirectoryInfo testDir = Directory.CreateDirectory(GetTestFilePath());
            Assert.True(Exists(testDir.FullName));
            Assert.True(Exists(testDir.FullName.ToUpperInvariant()));
            Assert.True(Exists(testDir.FullName.ToLowerInvariant()));
        }

        [Fact]
        [PlatformSpecific(PlatformID.Linux | PlatformID.FreeBSD)] 
        public void DoesCaseSensitiveComparions()
        {
            DirectoryInfo testDir = Directory.CreateDirectory(GetTestFilePath());
            Assert.True(Exists(testDir.FullName));
            Assert.False(Exists(testDir.FullName.ToUpperInvariant()));
            Assert.False(Exists(testDir.FullName.ToLowerInvariant()));
        }

        [Fact]
        [PlatformSpecific(PlatformID.Windows)] // In Windows, trailing whitespace in a path is trimmed
        public void TrimTrailingWhitespacePath()
        {
            DirectoryInfo testDir = Directory.CreateDirectory(GetTestFilePath());
            Assert.All((IOInputs.GetNonSignificantTrailingWhiteSpace()), (component) =>
            {
                Assert.True(Exists(testDir.FullName + component)); // string concat in case Path.Combine() trims whitespace before Exists gets to it
            });
        }

        [Fact]
        [PlatformSpecific(PlatformID.Windows)] // alternate data stream
        public void PathWithAlternateDataStreams_ReturnsFalse()
        {
            Assert.All((IOInputs.GetPathsWithAlternativeDataStreams()), (component) =>
            {
                Assert.False(Exists(component)); 
            });
        }

        [Fact]
        [OuterLoop]
        [PlatformSpecific(PlatformID.Windows)] // device names
        public void PathWithReservedDeviceNameAsPath_ReturnsFalse()
        {
            Assert.All((IOInputs.GetPathsWithReservedDeviceNames()), (component) =>
            {
                Assert.False(Exists(component));
            });
        }

        [Fact]
        [PlatformSpecific(PlatformID.Windows)] // UNC paths
        public void UncPathWithoutShareNameAsPath_ReturnsFalse()
        {
            Assert.All((IOInputs.GetUncPathsWithoutShareName()), (component) =>
            {
                Assert.False(Exists(component));
            });
        }

        [Fact]
        [PlatformSpecific(PlatformID.Windows)] // max directory length not fixed on Unix
        public void DirectoryEqualToMaxDirectory_ReturnsTrue()
        {
            // Creates directories up to the maximum directory length all at once
            DirectoryInfo testDir = Directory.CreateDirectory(GetTestFilePath());
            PathInfo path = IOServices.GetPath(testDir.FullName, IOInputs.MaxDirectory, maxComponent: 10);
            Directory.CreateDirectory(path.FullPath);
            Assert.True(Exists(path.FullPath));
        }

        [Fact]
        [PlatformSpecific(PlatformID.Windows)] // max directory length not fixed on Unix
        public void DirectoryWithComponentLongerThanMaxComponentAsPath_ReturnsFalse()
        {
            Assert.All((IOInputs.GetPathsWithComponentLongerThanMaxComponent()), (component) =>
            {
                Assert.False(Exists(component));
            });
        }

        [Fact]
        [ActiveIssue(1221)]
        [PlatformSpecific(PlatformID.Windows)] // drive labels
        public void NotReadyDriveAsPath_ReturnsFalse()
        {
            var drive = IOServices.GetNotReadyDrive();
            if (drive == null)
            {
                Console.WriteLine("Skipping test. Unable to find a not-ready drive, such as CD-Rom with no disc inserted.");
                return;
            }

            bool result = Exists(drive);

            Assert.False(result);
        }

        [Fact]
        [ActiveIssue(1221)]
        [PlatformSpecific(PlatformID.Windows)] // drive labels
        public void SubdirectoryOnNotReadyDriveAsPath_ReturnsFalse()
        {
            var drive = IOServices.GetNotReadyDrive();
            if (drive == null)
            {
                Console.WriteLine("Skipping test. Unable to find a not-ready drive, such as CD-Rom with no disc inserted.");
                return;
            }

            bool result = Exists(Path.Combine(drive, "Subdirectory"));

            Assert.False(result);
        }

        [Fact]
        [PlatformSpecific(PlatformID.Windows)] // drive labels
        public void NonExistentDriveAsPath_ReturnsFalse()
        {
            Assert.False(Exists(IOServices.GetNonExistentDrive()));
        }

        [Fact]
        [PlatformSpecific(PlatformID.Windows)] // drive labels
        public void SubdirectoryOnNonExistentDriveAsPath_ReturnsFalse()
        {
            Assert.False(Exists(Path.Combine(IOServices.GetNonExistentDrive(), "nonexistentsubdir")));
        }

        #endregion
    }
}