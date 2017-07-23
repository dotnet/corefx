// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;
using Xunit.NetCore.Extensions;

namespace System.IO.Tests
{
    public class Directory_Delete_str : FileSystemTest
    {
        #region Utilities

        public virtual void Delete(string path)
        {
            Directory.Delete(path);
        }

        #endregion

        #region UniversalTests

        [Fact]
        public void NullParameters()
        {
            Assert.Throws<ArgumentNullException>(() => Delete(null));
        }

        [Fact]
        public void InvalidParameters()
        {
            Assert.Throws<ArgumentException>(() => Delete(string.Empty));
        }

        [Fact]
        public void ShouldThrowIOExceptionIfContainedFileInUse()
        {
            DirectoryInfo testDir = Directory.CreateDirectory(GetTestFilePath());
            using (File.Create(Path.Combine(testDir.FullName, GetTestFileName())))
            {
                Assert.Throws<IOException>(() => Delete(testDir.FullName));
            }
            Assert.True(testDir.Exists);
        }

        [Fact]
        public void ShouldThrowIOExceptionForDirectoryWithFiles()
        {
            DirectoryInfo testDir = Directory.CreateDirectory(GetTestFilePath());
            File.Create(Path.Combine(testDir.FullName, GetTestFileName())).Dispose();
            Assert.Throws<IOException>(() => Delete(testDir.FullName));
            Assert.True(testDir.Exists);
        }

        [Fact]
        public void DirectoryWithSubdirectories()
        {
            DirectoryInfo testDir = Directory.CreateDirectory(GetTestFilePath());
            testDir.CreateSubdirectory(GetTestFileName());
            Assert.Throws<IOException>(() => Delete(testDir.FullName));
            Assert.True(testDir.Exists);
        }

        [Fact]
        [OuterLoop]
        public void DeleteRoot()
        {
            Assert.Throws<IOException>(() => Delete(Path.GetPathRoot(Directory.GetCurrentDirectory())));
        }

        [Fact]
        public void PositiveTest()
        {
            DirectoryInfo testDir = Directory.CreateDirectory(GetTestFilePath());
            Delete(testDir.FullName);
            Assert.False(testDir.Exists);
        }

        [Theory, MemberData(nameof(TrailingCharacters))]
        public void MissingFile_ThrowsDirectoryNotFound(char trailingChar)
        {
            string path = GetTestFilePath() + trailingChar;
            Assert.Throws<DirectoryNotFoundException>(() => Delete(path));
        }

        [Theory, MemberData(nameof(TrailingCharacters))]
        public void MissingDirectory_ThrowsDirectoryNotFound(char trailingChar)
        {
            string path = Path.Combine(GetTestFilePath(), "file" + trailingChar);
            Assert.Throws<DirectoryNotFoundException>(() => Delete(path));
        }

        [Fact]
        public void ShouldThrowIOExceptionDeletingCurrentDirectory()
        {
            Assert.Throws<IOException>(() => Delete(Directory.GetCurrentDirectory()));
        }

        [ConditionalFact(nameof(CanCreateSymbolicLinks))]
        public void DeletingSymLinkDoesntDeleteTarget()
        {
            var path = GetTestFilePath();
            var linkPath = GetTestFilePath();

            Directory.CreateDirectory(path);
            Assert.True(MountHelper.CreateSymbolicLink(linkPath, path, isDirectory: true));

            // Both the symlink and the target exist
            Assert.True(Directory.Exists(path), "path should exist");
            Assert.True(Directory.Exists(linkPath), "linkPath should exist");

            // Delete the symlink
            Directory.Delete(linkPath);

            // Target should still exist
            Assert.True(Directory.Exists(path), "path should still exist");
            Assert.False(Directory.Exists(linkPath), "linkPath should no longer exist");
        }

        [ConditionalFact(nameof(UsingNewNormalization))]
        public void ExtendedDirectoryWithSubdirectories()
        {
            DirectoryInfo testDir = Directory.CreateDirectory(IOInputs.ExtendedPrefix + GetTestFilePath());
            testDir.CreateSubdirectory(GetTestFileName());
            Assert.Throws<IOException>(() => Delete(testDir.FullName));
            Assert.True(testDir.Exists);
        }

        [ConditionalFact(nameof(LongPathsAreNotBlocked), nameof(UsingNewNormalization))]
        public void LongPathExtendedDirectory()
        {
            DirectoryInfo testDir = Directory.CreateDirectory(IOServices.GetPath(IOInputs.ExtendedPrefix + TestDirectory, characterCount: 500));
            Delete(testDir.FullName);
            Assert.False(testDir.Exists);
        }

        #endregion

        #region PlatformSpecific

        [Fact]
        [PlatformSpecific(TestPlatforms.Windows)]  // Deleting readonly directory throws IOException
        public void WindowsDeleteReadOnlyDirectory()
        {
            DirectoryInfo testDir = Directory.CreateDirectory(GetTestFilePath());
            testDir.Attributes = FileAttributes.ReadOnly;
            Assert.Throws<IOException>(() => Delete(testDir.FullName));
            Assert.True(testDir.Exists);
            testDir.Attributes = FileAttributes.Normal;
        }

        [ConditionalFact(nameof(UsingNewNormalization))]
        [PlatformSpecific(TestPlatforms.Windows)]  // Deleting extended readonly directory throws IOException
        public void WindowsDeleteExtendedReadOnlyDirectory()
        {
            DirectoryInfo testDir = Directory.CreateDirectory(IOInputs.ExtendedPrefix + GetTestFilePath());
            testDir.Attributes = FileAttributes.ReadOnly;
            Assert.Throws<IOException>(() => Delete(testDir.FullName));
            Assert.True(testDir.Exists);
            testDir.Attributes = FileAttributes.Normal;
        }

        [Fact]
        [PlatformSpecific(TestPlatforms.AnyUnix)]  // Deleting readOnly directory succeeds
        public void UnixDeleteReadOnlyDirectory()
        {
            DirectoryInfo testDir = Directory.CreateDirectory(GetTestFilePath());
            testDir.Attributes = FileAttributes.ReadOnly;
            Delete(testDir.FullName);
            Assert.False(testDir.Exists);
        }

        [Fact]
        [PlatformSpecific(TestPlatforms.Windows)]  // Deleting hidden directory succeeds
        public void WindowsShouldBeAbleToDeleteHiddenDirectory()
        {
            DirectoryInfo testDir = Directory.CreateDirectory(GetTestFilePath());
            testDir.Attributes = FileAttributes.Hidden;
            Delete(testDir.FullName);
            Assert.False(testDir.Exists);
        }

        [ConditionalFact(nameof(UsingNewNormalization))]
        [PlatformSpecific(TestPlatforms.Windows)]  // Deleting extended hidden directory succeeds
        public void WindowsShouldBeAbleToDeleteExtendedHiddenDirectory()
        {
            DirectoryInfo testDir = Directory.CreateDirectory(IOInputs.ExtendedPrefix + GetTestFilePath());
            testDir.Attributes = FileAttributes.Hidden;
            Delete(testDir.FullName);
            Assert.False(testDir.Exists);
        }

        [Fact]
        [PlatformSpecific(TestPlatforms.AnyUnix)]  // Deleting hidden directory succeeds
        public void UnixShouldBeAbleToDeleteHiddenDirectory()
        {
            string testDir = "." + GetTestFileName();
            Directory.CreateDirectory(Path.Combine(TestDirectory, testDir));
            Assert.True(0 != (new DirectoryInfo(Path.Combine(TestDirectory, testDir)).Attributes & FileAttributes.Hidden));
            Delete(Path.Combine(TestDirectory, testDir));
            Assert.False(Directory.Exists(testDir));
        }

        [Fact]
        [OuterLoop("Needs sudo access")]
        [PlatformSpecific(TestPlatforms.Linux)]
        [Trait(XunitConstants.Category, XunitConstants.RequiresElevation)]
        public void Unix_NotFoundDirectory_ReadOnlyVolume()
        {
            ReadOnly_FileSystemHelper(readOnlyDirectory =>
            {
                Assert.Throws<DirectoryNotFoundException>(() => Delete(Path.Combine(readOnlyDirectory, "DoesNotExist")));
            });
        }
        #endregion
    }

    public class Directory_Delete_str_bool : Directory_Delete_str
    {
        #region Utilities

        public override void Delete(string path)
        {
            Directory.Delete(path, false);
        }

        public virtual void Delete(string path, bool recursive)
        {
            Directory.Delete(path, recursive);
        }

        #endregion

        [Fact]
        public void RecursiveDelete()
        {
            DirectoryInfo testDir = Directory.CreateDirectory(GetTestFilePath());
            File.Create(Path.Combine(testDir.FullName, GetTestFileName())).Dispose();
            testDir.CreateSubdirectory(GetTestFileName());
            Delete(testDir.FullName, true);
            Assert.False(testDir.Exists);
        }

        [Fact]
        public void RecursiveDeleteWithTrailingSlash()
        {
            DirectoryInfo testDir = Directory.CreateDirectory(GetTestFilePath());
            Delete(testDir.FullName + Path.DirectorySeparatorChar, true);
            Assert.False(testDir.Exists);
        }

        [Fact]
        [PlatformSpecific(TestPlatforms.Windows)]  // Recursive delete throws IOException if directory contains in-use file
        public void RecursiveDelete_ShouldThrowIOExceptionIfContainedFileInUse()
        {
            DirectoryInfo testDir = Directory.CreateDirectory(GetTestFilePath());
            using (File.Create(Path.Combine(testDir.FullName, GetTestFileName())))
            {
                Assert.Throws<IOException>(() => Delete(testDir.FullName, true));
            }
            Assert.True(testDir.Exists);
        }
    }
}
