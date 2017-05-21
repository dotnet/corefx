// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.IO.Tests
{
    public class DirectoryInfo_GetSetAttributes : FileSystemTest
    {
        #region Utilities

        protected virtual FileAttributes Get(string path)
        {
            var info = new DirectoryInfo(path);
            return info.Attributes;
        }

        protected virtual void Set(string path, FileAttributes attributes)
        {
            var info = new DirectoryInfo(path);
            info.Attributes = attributes;
        }

        #endregion

        [Fact]
        public void NullParameters()
        {
            Assert.Throws<ArgumentNullException>(() => Get(null));
            Assert.Throws<ArgumentNullException>(() => Set(null, FileAttributes.Normal));
        }

        [Fact]
        public void InvalidParameters()
        {
            Assert.Throws<ArgumentException>(() => Get(string.Empty));
            Assert.Throws<ArgumentException>(() => Set(string.Empty, FileAttributes.Normal));
        }

        // In NetFX we ignore "not found" errors, which leaves the attributes
        // state as invalid (0xFFFFFFFF), which makes all flags true.

        [Theory, MemberData(nameof(TrailingCharacters))]
        public void GetAttributes_MissingFile(char trailingChar)
        {
            Assert.Equal((FileAttributes)(-1), Get(GetTestFilePath() + trailingChar));
        }

        [Theory, MemberData(nameof(TrailingCharacters))]
        public void GetAttributes_MissingDirectory(char trailingChar)
        {
            Assert.Equal((FileAttributes)(-1), Get(Path.Combine(GetTestFilePath(), "file" + trailingChar)));
        }

        [Theory, MemberData(nameof(TrailingCharacters))]
        public void SetAttributes_MissingFile(char trailingChar)
        {
            Assert.Throws<FileNotFoundException>(() => Set(GetTestFilePath() + trailingChar, FileAttributes.ReadOnly));
        }

        [Theory, MemberData(nameof(TrailingCharacters))]
        public void SetAttributes_MissingDirectory(char trailingChar)
        {
            Assert.Throws<DirectoryNotFoundException>(() => Set(Path.Combine(GetTestFilePath(), "file" + trailingChar), FileAttributes.ReadOnly));
        }

        [Theory]
        [InlineData(FileAttributes.ReadOnly)]
        [PlatformSpecific(TestPlatforms.AnyUnix)]  // Tests Unix file attributes
        public void UnixAttributeSetting(FileAttributes attr)
        {
            var test = new DirectoryInfo(GetTestFilePath());
            test.Create();
            Set(test.FullName, attr);
            Assert.Equal(attr | FileAttributes.Directory, Get(test.FullName));
            Set(test.FullName, 0);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        [PlatformSpecific(TestPlatforms.AnyUnix)]
        public void UnixDirectoryBeginningWithPeriodIsHidden(bool endsWithSlash)
        {
            string testDir = "." + GetTestFileName();
            Directory.CreateDirectory(Path.Combine(TestDirectory, testDir));
            Assert.True(0 != (new DirectoryInfo(Path.Combine(TestDirectory, testDir) + (endsWithSlash ? "/" : "")).Attributes & FileAttributes.Hidden));
        }

        [Theory]
        [InlineData(FileAttributes.ReadOnly)]
        [InlineData(FileAttributes.Hidden)]
        [InlineData(FileAttributes.System)]
        [InlineData(FileAttributes.Archive)]
        [InlineData(FileAttributes.ReadOnly | FileAttributes.Hidden)]
        [PlatformSpecific(TestPlatforms.Windows)]  // Tests Windows file attributes
        public void WindowsAttributeSetting(FileAttributes attr)
        {
            var test = new DirectoryInfo(GetTestFilePath());
            test.Create();
            Set(test.FullName, attr);
            Assert.Equal(attr | FileAttributes.Directory, Get(test.FullName));
            Set(test.FullName, 0);
        }

        [Theory]
        [InlineData(FileAttributes.Normal)]
        [InlineData(FileAttributes.Temporary)]
        [InlineData(FileAttributes.Encrypted)]
        [InlineData(FileAttributes.SparseFile)]
        [InlineData(FileAttributes.ReparsePoint)]
        [InlineData(FileAttributes.Compressed)]
        [PlatformSpecific(TestPlatforms.AnyUnix)]  // Unix-invalid file attributes that don't get set
        public void UnixInvalidAttributes(FileAttributes attr)
        {
            var path = GetTestFilePath();
            File.Create(path).Dispose();
            Set(path, attr);
            Assert.Equal(FileAttributes.Normal, Get(path));
        }

        [Theory]
        [InlineData(FileAttributes.Normal)]
        [InlineData(FileAttributes.Encrypted)]
        [InlineData(FileAttributes.SparseFile)]
        [InlineData(FileAttributes.ReparsePoint)]
        [InlineData(FileAttributes.Compressed)]
        [PlatformSpecific(TestPlatforms.Windows)]  // Windows-invalid file attributes that don't get set
        public void WindowsInvalidAttributes(FileAttributes attr)
        {
            var path = GetTestFilePath();
            File.Create(path).Dispose();
            Set(path, attr);
            Assert.Equal(FileAttributes.Normal, Get(path));
        }

        [Theory]
        [InlineData(~FileAttributes.ReadOnly)]
        [InlineData(-1)]
        [PlatformSpecific(TestPlatforms.AnyUnix)]  // Unix-invalid file attributes that throw
        public void UnixInvalidAttributes_ThrowArgumentException(FileAttributes attr)
        {
            var test = new DirectoryInfo(GetTestFilePath());
            test.Create();
            Assert.Throws<ArgumentException>(() => Set(test.FullName, attr));
        }

        [Theory]
        [InlineData(FileAttributes.Temporary)]
        [InlineData(~FileAttributes.ReadOnly)]
        [InlineData(-1)]
        [PlatformSpecific(TestPlatforms.Windows)]  // Windows-invalid file attributes that throw
        public void WindowsInvalidAttributes_ThrowArgumentException(FileAttributes attr)
        {
            var test = new DirectoryInfo(GetTestFilePath());
            test.Create();
            Assert.Throws<ArgumentException>(() => Set(test.FullName, attr));
        }

        [ConditionalFact(nameof(CanCreateSymbolicLinks))]
        public void SymLinksAreReparsePoints()
        {
            var path = GetTestFilePath();
            var linkPath = GetTestFilePath();

            Directory.CreateDirectory(path);
            Assert.True(MountHelper.CreateSymbolicLink(linkPath, path, isDirectory: true));

            Assert.NotEqual(FileAttributes.ReparsePoint, FileAttributes.ReparsePoint & Get(path));
            Assert.Equal(FileAttributes.ReparsePoint, FileAttributes.ReparsePoint & Get(linkPath));
        }

        [ConditionalFact(nameof(CanCreateSymbolicLinks))]
        public void SymLinksReflectSymLinkAttributes()
        {
            var path = GetTestFilePath();
            var linkPath = GetTestFilePath();

            Directory.CreateDirectory(path);
            Assert.True(MountHelper.CreateSymbolicLink(linkPath, path, isDirectory: true));

            Set(path, FileAttributes.ReadOnly);
            try
            {
                Assert.Equal(FileAttributes.ReadOnly, FileAttributes.ReadOnly & Get(path));
                Assert.NotEqual(FileAttributes.ReadOnly, FileAttributes.ReadOnly & Get(linkPath));
            }
            finally
            {
                Set(path, Get(path) & ~FileAttributes.ReadOnly);
            }
        }
    }
}
