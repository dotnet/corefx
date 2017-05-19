// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;
using Xunit;

namespace System.IO.Tests
{
    public class File_GetSetAttributes : FileSystemTest
    {
        #region Utilities

        protected virtual FileAttributes Get(string path)
        {
            return File.GetAttributes(path);
        }

        protected virtual void Set(string path, FileAttributes attributes)
        {
            File.SetAttributes(path, attributes);
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

        [Theory]
        [InlineData(FileAttributes.ReadOnly)]
        [InlineData(FileAttributes.Normal)]
        [PlatformSpecific(TestPlatforms.AnyUnix)]  // Valid Unix file attribute
        public void UnixAttributeSetting(FileAttributes attr)
        {
            var path = GetTestFilePath();
            File.Create(path).Dispose();
            Set(path, attr);
            Assert.Equal(attr, Get(path));
            Set(path, 0);
        }

        [Theory]
        [InlineData(FileAttributes.ReadOnly)]
        [InlineData(FileAttributes.Hidden)]
        [InlineData(FileAttributes.System)]
        [InlineData(FileAttributes.Archive)]
        [InlineData(FileAttributes.Normal)]
        [InlineData(FileAttributes.Temporary)]
        [InlineData(FileAttributes.ReadOnly | FileAttributes.Hidden)]
        [PlatformSpecific(TestPlatforms.Windows)]  // Valid Windows file attribute
        public void WindowsAttributeSetting(FileAttributes attr)
        {
            var path = GetTestFilePath();
            File.Create(path).Dispose();
            Set(path, attr);
            Assert.Equal(attr, Get(path));
            Set(path, 0);
        }

        [Theory]
        [InlineData(FileAttributes.SparseFile)]
        [InlineData(FileAttributes.Temporary)]
        [InlineData(FileAttributes.ReparsePoint)]
        [InlineData(FileAttributes.Compressed)]
        [InlineData(FileAttributes.Encrypted)]
        [PlatformSpecific(TestPlatforms.AnyUnix)]  // Invalid Unix file attributes
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
        [PlatformSpecific(TestPlatforms.Windows)]  // Invalid Windows file attributes 
        public void WindowsInvalidAttributes(FileAttributes attr)
        {
            var path = GetTestFilePath();
            File.Create(path).Dispose();
            Set(path, attr);
            Assert.Equal(FileAttributes.Normal, Get(path));
        }

        [ConditionalFact(nameof(CanCreateSymbolicLinks))]
        public void SymLinksAreReparsePoints()
        {
            var path = GetTestFilePath();
            var linkPath = GetTestFilePath();

            File.Create(path).Dispose();
            Assert.True(MountHelper.CreateSymbolicLink(linkPath, path, isDirectory: false));

            Assert.NotEqual(FileAttributes.ReparsePoint, FileAttributes.ReparsePoint & Get(path));
            Assert.Equal(FileAttributes.ReparsePoint, FileAttributes.ReparsePoint & Get(linkPath));
        }

        [ConditionalFact(nameof(CanCreateSymbolicLinks))]
        public void SymLinksReflectSymLinkAttributes()
        {
            var path = GetTestFilePath();
            var linkPath = GetTestFilePath();

            File.Create(path).Dispose();
            Assert.True(MountHelper.CreateSymbolicLink(linkPath, path, isDirectory: false));

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

        [Theory, MemberData(nameof(TrailingCharacters))]
        public void GetAttributes_MissingFile(char trailingChar)
        {
            Assert.Throws<FileNotFoundException>(() => Get(GetTestFilePath() + trailingChar));
        }

        [Theory, MemberData(nameof(TrailingCharacters))]
        public void GetAttributes_MissingDirectory(char trailingChar)
        {
            Assert.Throws<DirectoryNotFoundException>(() => Get(Path.Combine(GetTestFilePath(), "dir" + trailingChar)));
        }

        [Theory, MemberData(nameof(TrailingCharacters))]
        public void SetAttributes_MissingFile(char trailingChar)
        {
            Assert.Throws<FileNotFoundException>(() => Set(GetTestFilePath() + trailingChar, FileAttributes.ReadOnly));
        }

        [Theory, MemberData(nameof(TrailingCharacters))]
        public void SetAttributes_MissingDirectory(char trailingChar)
        {
            Assert.Throws<DirectoryNotFoundException>(() => Set(Path.Combine(GetTestFilePath(), "dir" + trailingChar), FileAttributes.ReadOnly));
        }
    }
}
