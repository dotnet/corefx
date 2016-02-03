// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.IO.Tests
{
    public class Directory_Move : FileSystemTest
    {
        #region Utilities

        public virtual void Move(string sourceDir, string destDir)
        {
            Directory.Move(sourceDir, destDir);
        }

        #endregion

        #region UniversalTests

        [Fact]
        public void NullPath()
        {
            Assert.Throws<ArgumentNullException>(() => Move(null, "."));
            Assert.Throws<ArgumentNullException>(() => Move(".", null));
        }

        [Fact]
        public void EmptyPath()
        {
            Assert.Throws<ArgumentException>(() => Move(string.Empty, "."));
            Assert.Throws<ArgumentException>(() => Move(".", string.Empty));
        }

        [Fact]
        public void NonExistentDirectory()
        {
            DirectoryInfo valid = Directory.CreateDirectory(GetTestFilePath());
            Assert.Throws<DirectoryNotFoundException>(() => Move(GetTestFilePath(), valid.FullName));
            Assert.Throws<DirectoryNotFoundException>(() => Move(valid.FullName, Path.Combine(TestDirectory, GetTestFileName(), GetTestFileName())));
        }

        [Fact]
        public void MoveOntoExistingDirectory()
        {
            DirectoryInfo testDir = Directory.CreateDirectory(GetTestFilePath());
            Assert.Throws<IOException>(() => Move(testDir.FullName, testDir.FullName));
        }

        [Fact]
        public void MoveIntoCurrentDirectory()
        {
            DirectoryInfo testDir = Directory.CreateDirectory(GetTestFilePath());
            Assert.Throws<IOException>(() => Move(testDir.FullName, Path.Combine(testDir.FullName, ".")));
        }

        [Fact]
        public void MoveOntoParentDirectory()
        {
            DirectoryInfo testDir = Directory.CreateDirectory(GetTestFilePath());
            Assert.Throws<IOException>(() => Move(testDir.FullName, Path.Combine(testDir.FullName, "..")));
        }

        [Fact]
        public void BasicMove()
        {
            string testDirSource = Path.Combine(TestDirectory, GetTestFileName());
            string testDirDest = Path.Combine(TestDirectory, GetTestFileName());

            Directory.CreateDirectory(testDirSource);
            Move(testDirSource, testDirDest);
            Assert.True(Directory.Exists(testDirDest));
        }

        [Fact]
        public void MultipleMoves()
        {
            string testDir = GetTestFilePath();
            string testDirSource = Path.Combine(testDir, GetTestFileName());
            string testDirDest1 = Path.Combine(testDir, GetTestFileName());
            string testDirDest2 = Path.Combine(testDir, GetTestFileName());

            Directory.CreateDirectory(testDirSource);
            Move(testDirSource, testDirDest1);
            Move(testDirDest1, testDirDest2);
            Assert.True(Directory.Exists(testDirDest2));
            Assert.False(Directory.Exists(testDirDest1));
            Assert.False(Directory.Exists(testDirSource));
        }

        [Fact]
        public void DirectoryNameWithSpaces()
        {
            string testDirSource = Path.Combine(TestDirectory, GetTestFileName());
            string testDirDest = Path.Combine(TestDirectory, "    e n   d");

            Directory.CreateDirectory(testDirSource);
            Move(testDirSource, testDirDest);
            Assert.True(Directory.Exists(testDirDest));
        }

        [Fact]
        public void TrailingDirectorySeparators()
        {
            string testDirSource = Path.Combine(TestDirectory, GetTestFileName());
            string testDirDest = Path.Combine(TestDirectory, GetTestFileName());

            Directory.CreateDirectory(testDirSource);
            Move(testDirSource + Path.DirectorySeparatorChar, testDirDest + Path.DirectorySeparatorChar);
            Assert.True(Directory.Exists(testDirDest));
        }

        [Fact]
        public void IncludeSubdirectories()
        {
            string testDirSource = Path.Combine(TestDirectory, GetTestFileName());
            string testDirSubDirectory = GetTestFileName();
            string testDirDest = Path.Combine(TestDirectory, GetTestFileName());

            Directory.CreateDirectory(testDirSource);
            Directory.CreateDirectory(Path.Combine(testDirSource, testDirSubDirectory));
            Move(testDirSource, testDirDest);

            Assert.True(Directory.Exists(testDirDest));
            Assert.False(Directory.Exists(testDirSource));
            Assert.True(Directory.Exists(Path.Combine(testDirDest, testDirSubDirectory)));
        }

        [Fact]
        public void Path_Longer_Than_MaxLongPath_Throws_Exception()
        {
            string testDir = GetTestFilePath();
            Directory.CreateDirectory(testDir);
            Assert.All((IOInputs.GetPathsLongerThanMaxLongPath(GetTestFilePath())), (path) =>
            {
                Assert.Throws<PathTooLongException>(() => Move(testDir, path));
                Assert.Throws<PathTooLongException>(() => Move(path, testDir));
            });
        }

        #endregion

        #region PlatformSpecific

        [Fact]
        [PlatformSpecific(PlatformID.Windows)]
        public void Path_With_Longer_Than_MaxDirectory_Succeeds()
        {
            string testDir = GetTestFilePath();
            Directory.CreateDirectory(testDir);
            Assert.True(Directory.Exists(testDir), "test directory should exist");
            Assert.All((IOInputs.GetPathsLongerThanMaxDirectory(GetTestFilePath())), (path) =>
            {
                string baseDestinationPath = Path.GetDirectoryName(path);
                if (!Directory.Exists(baseDestinationPath))
                {
                    Directory.CreateDirectory(baseDestinationPath);
                }
                Assert.True(Directory.Exists(baseDestinationPath), "base destination path should exist");

                Move(testDir, path);
                Assert.False(Directory.Exists(testDir), "source directory should exist");
                Assert.True(Directory.Exists(path), "destination directory should exist");
                Move(path, testDir);
                Assert.False(Directory.Exists(path), "source directory should exist");
                Assert.True(Directory.Exists(testDir), "destination directory should exist");
            });
        }

        [Fact]
        [PlatformSpecific(PlatformID.Windows)]
        public void WindowsWildCharacterPath()
        {
            Assert.Throws<ArgumentException>(() => Move("*", GetTestFilePath()));
            Assert.Throws<ArgumentException>(() => Move(TestDirectory, "*"));
            Assert.Throws<ArgumentException>(() => Move(TestDirectory, "Test*t"));
            Assert.Throws<ArgumentException>(() => Move(TestDirectory, "*Test"));
        }

        [Fact]
        [PlatformSpecific(PlatformID.AnyUnix)]
        public void UnixWildCharacterPath()
        {
            // Wildcards are allowed in paths for Unix move commands as literals as well as functional wildcards,
            // but to implement the latter in .NET would be confusing (e.g. having a DirectoryInfo represent multiple directories), 
            // so the implementation assumes the former.
            // Thus, any "*" characters will act the same as any other character when used in a file/directory name.
            string testDir = GetTestFilePath();
            string testDirSource = Path.Combine(testDir, "*");
            string testDirShouldntMove = Path.Combine(testDir, "*t");
            string testDirDest = Path.Combine(testDir, "*" + GetTestFileName());

            Directory.CreateDirectory(testDirSource);
            Directory.CreateDirectory(testDirShouldntMove);
            Move(testDirSource, testDirDest);
            Assert.True(Directory.Exists(testDirDest));
            Assert.False(Directory.Exists(testDirSource));
            Assert.True(Directory.Exists(testDirShouldntMove));

            Move(testDirDest, testDirSource);
            Assert.False(Directory.Exists(testDirDest));
            Assert.True(Directory.Exists(testDirSource));
            Assert.True(Directory.Exists(testDirShouldntMove));
        }

        [Fact]
        [PlatformSpecific(PlatformID.Windows)]
        public void WindowsWhitespacePath()
        {
            DirectoryInfo testDir = Directory.CreateDirectory(GetTestFilePath());
            Assert.Throws<ArgumentException>(() => Move(testDir.FullName, "         "));
            Assert.Throws<ArgumentException>(() => Move(testDir.FullName, "\n"));
            Assert.Throws<ArgumentException>(() => Move(testDir.FullName, ""));
            Assert.Throws<ArgumentException>(() => Move(testDir.FullName, ">"));
            Assert.Throws<ArgumentException>(() => Move(testDir.FullName, "<"));
            Assert.Throws<ArgumentException>(() => Move(testDir.FullName, "\0"));
            Assert.Throws<ArgumentException>(() => Move(testDir.FullName, "\t"));
        }

        [Fact]
        [PlatformSpecific(PlatformID.AnyUnix)]
        public void UnixWhitespacePath()
        {
            DirectoryInfo testDir = Directory.CreateDirectory(GetTestFilePath());
            string testDirToMove = Path.Combine(testDir.FullName, GetTestFileName());
            Directory.CreateDirectory(testDirToMove);
            Move(testDirToMove, Path.Combine(testDir.FullName, "         "));
            Move(Path.Combine(testDir.FullName, "         "), Path.Combine(testDir.FullName, "\n"));
            Move(Path.Combine(testDir.FullName, "\n"), Path.Combine(testDir.FullName, "\t"));
            Move(Path.Combine(testDir.FullName, "\t"), Path.Combine(testDir.FullName, ">"));
            Move(Path.Combine(testDir.FullName, ">"), Path.Combine(testDir.FullName, "< "));
            Assert.True(Directory.Exists(Path.Combine(testDir.FullName, "< ")));
        }

        [Fact]
        [PlatformSpecific(PlatformID.Windows)]
        public void WindowsExistingDirectory()
        {
            DirectoryInfo testDir = Directory.CreateDirectory(GetTestFilePath());
            string testDirSource = Path.Combine(testDir.FullName, GetTestFileName());
            string testDirDest = Path.Combine(testDir.FullName, GetTestFileName());

            Directory.CreateDirectory(testDirSource);
            Directory.CreateDirectory(testDirDest);
            Assert.Throws<IOException>(() => Move(testDirSource, testDirDest));
            Assert.True(Directory.Exists(testDirDest));
            Assert.True(Directory.Exists(testDirSource));
        }

        [Fact]
        [PlatformSpecific(PlatformID.Windows)]
        public void BetweenDriveLabels()
        {
            DirectoryInfo testDir = Directory.CreateDirectory(GetTestFilePath());
            string path = Path.GetFullPath(testDir.FullName);
            if (path.Substring(0, 3) == @"d:\" || path.Substring(0, 3) == @"D:\")
                Assert.Throws<IOException>(() => Move(path, "C:\\DoesntExist"));
            else
                Assert.Throws<IOException>(() => Move(path, "D:\\DoesntExist"));
        }

        [Fact]
        [PlatformSpecific(PlatformID.AnyUnix)]
        public void UnixExistingDirectory()
        {
            // Moving to an-empty directory is supported on Unix, but moving to a non-empty directory is not
            string testDirSource = GetTestFilePath();
            string testDirDestEmpty = GetTestFilePath();
            string testDirDestNonEmpty = GetTestFilePath();

            Directory.CreateDirectory(testDirSource);
            Directory.CreateDirectory(testDirDestEmpty);
            Directory.CreateDirectory(testDirDestNonEmpty);

            using (File.Create(Path.Combine(testDirDestNonEmpty, GetTestFileName())))
            {
                Assert.Throws<IOException>(() => Move(testDirSource, testDirDestNonEmpty));
                Assert.True(Directory.Exists(testDirDestNonEmpty));
                Assert.True(Directory.Exists(testDirSource));
            }

            Move(testDirSource, testDirDestEmpty);
            Assert.True(Directory.Exists(testDirDestEmpty));
            Assert.False(Directory.Exists(testDirSource));
        }

        #endregion
    }
}
