// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.IO.Tests
{
    // Tests that are valid for File, FileInfo, and DirectoryInfo
    public abstract class AllGetSetAttributes : BaseGetSetAttributes
    {
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


        [ConditionalFact(nameof(CanCreateSymbolicLinks))]
        public void SymLinksAreReparsePoints()
        {
            var path = CreateItem();
            var linkPath = GetTestFilePath();

            Assert.True(MountHelper.CreateSymbolicLink(linkPath, path, isDirectory: IsDirectory));

            Assert.NotEqual(FileAttributes.ReparsePoint, FileAttributes.ReparsePoint & Get(path));
            Assert.Equal(FileAttributes.ReparsePoint, FileAttributes.ReparsePoint & Get(linkPath));
        }

        [ConditionalFact(nameof(CanCreateSymbolicLinks))]
        public void SymLinksReflectSymLinkAttributes()
        {
            var path = CreateItem();
            var linkPath = GetTestFilePath();

            Assert.True(MountHelper.CreateSymbolicLink(linkPath, path, isDirectory: IsDirectory));

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
