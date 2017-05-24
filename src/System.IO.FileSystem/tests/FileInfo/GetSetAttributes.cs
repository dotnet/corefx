// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.IO.Tests
{
    public class FileInfo_GetSetAttributes : FileSystemTest
    {
        #region Utilities

        protected virtual FileAttributes Get(string path)
        {
            var info = new FileInfo(path);
            return info.Attributes;
        }

        protected virtual void Set(string path, FileAttributes attributes)
        {
            var info = new FileInfo(path);
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

        [Fact]
        public void NonExistentFile()
        {
            Assert.Throws<FileNotFoundException>(() => Set(GetTestFilePath(), FileAttributes.Normal));
        }

        [Fact]
        public void IsReadOnly_Set_And_Get()
        {
            var test = new FileInfo(GetTestFilePath());
            test.Create().Dispose();
            // Set to True
            test.IsReadOnly = true;
            test.Refresh();
            Assert.Equal(true, test.IsReadOnly);

            // Set To False
            test.IsReadOnly = false;
            test.Refresh();
            Assert.Equal(false, test.IsReadOnly);
        }

        [Theory]
        [InlineData(FileAttributes.ReadOnly)]
        [InlineData(FileAttributes.Normal)]
        [PlatformSpecific(TestPlatforms.AnyUnix)]  // Unix valid file attributes
        public void UnixAttributeSetting(FileAttributes attr)
        {
            var test = new FileInfo(GetTestFilePath());
            test.Create().Dispose();
            Set(test.FullName, attr);
            test.Refresh();
            Assert.Equal(attr, Get(test.FullName));
            Set(test.FullName, 0);
        }

        [Theory]
        [InlineData(FileAttributes.ReadOnly)]
        [InlineData(FileAttributes.Hidden)]
        [InlineData(FileAttributes.System)]
        [InlineData(FileAttributes.Archive)]
        [InlineData(FileAttributes.Normal)]
        [InlineData(FileAttributes.Temporary)]
        [InlineData(FileAttributes.ReadOnly | FileAttributes.Hidden)]
        [PlatformSpecific(TestPlatforms.Windows)]  // Windows valid file attributes
        public void WindowsAttributeSetting(FileAttributes attr)
        {
            var test = new FileInfo(GetTestFilePath());
            test.Create().Dispose();
            Set(test.FullName, attr);
            test.Refresh();
            Assert.Equal(attr, Get(test.FullName));
            Set(test.FullName, 0);
        }

        [Theory]
        [InlineData(FileAttributes.Temporary)]
        [InlineData(FileAttributes.Encrypted)]
        [InlineData(FileAttributes.SparseFile)]
        [InlineData(FileAttributes.ReparsePoint)]
        [InlineData(FileAttributes.Compressed)]
        [PlatformSpecific(TestPlatforms.AnyUnix)]  // Unix invalid file attributes
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
        [PlatformSpecific(TestPlatforms.Windows)] // Windows invalid file attributes
        public void WindowsInvalidAttributes(FileAttributes attr)
        {
            var path = GetTestFilePath();
            File.Create(path).Dispose();
            Set(path, attr);
            Assert.Equal(FileAttributes.Normal, Get(path));
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
    }
}
