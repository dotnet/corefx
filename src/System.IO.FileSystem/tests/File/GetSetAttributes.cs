﻿// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

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
        [PlatformSpecific(PlatformID.AnyUnix)]
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
        [PlatformSpecific(PlatformID.Windows)]
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
        [PlatformSpecific(PlatformID.AnyUnix)]
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
        [PlatformSpecific(PlatformID.Windows)]
        public void WindowsInvalidAttributes(FileAttributes attr)
        {
            var path = GetTestFilePath();
            File.Create(path).Dispose();
            Set(path, attr);
            Assert.Equal(FileAttributes.Normal, Get(path));
        }

        // In some cases (such as when running without elevated privileges,
        // the symbolic link may fail to create. Only run this test if it creates
        // links successfully.
        [ConditionalFact("CanCreateSymbolicLinks")]
        public void SymLinksAreReparsePoints()
        {
            var path = GetTestFilePath();
            var linkPath = GetTestFilePath();
            File.Create(path).Dispose();
            Assert.True(MountHelper.CreateSymbolicLink(linkPath, path));
            Assert.Equal(FileAttributes.ReparsePoint, FileAttributes.ReparsePoint & Get(linkPath));
        }

        // In some cases (such as when running without elevated privileges,
        // the symbolic link may fail to create. Only run this test if it creates
        // links successfully.
        [ConditionalFact("CanCreateSymbolicLinks")]
        public void SymLinksDoNotReflectTargetAttributes()
        {
            var path = GetTestFilePath();
            var linkPath = GetTestFilePath();
            File.Create(path).Dispose();
            Assert.True(MountHelper.CreateSymbolicLink(linkPath, path));

            Set(path, FileAttributes.ReadOnly);

            Assert.Equal(FileAttributes.ReadOnly, Get(path));

            // Can't assume that ReparsePoint is the only attribute because Windows will add Archive automatically
            // Instead, just make sure that ReparsePoint is present and ReadOnly is not
            Assert.Equal(FileAttributes.ReparsePoint, FileAttributes.ReparsePoint & Get(linkPath));
            Assert.Equal((FileAttributes)0, FileAttributes.ReadOnly & Get(linkPath));
        }

        private static bool CanCreateSymbolicLinks
        {
            get
            {
                var path = Path.GetTempFileName();
                var linkPath = path + ".link";
                var ret = MountHelper.CreateSymbolicLink(linkPath, path);
                try { File.Delete(path); } catch { }
                try { File.Delete(linkPath); } catch { }
                return ret;
            }
        }
    }
}