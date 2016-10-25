// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.IO.Tests
{
    public class File_Replace_str_str_str : FileSystemTest
    {
        #region Utilities

        public virtual void Replace(string source, string dest, string destBackup)
        {
            File.Replace(source, dest, destBackup);
        }

        public virtual void Replace(string source, string dest, string destBackup, bool ignoreMetadataErrors)
        {
            File.Replace(source, dest, destBackup, ignoreMetadataErrors);
        }

        #endregion

        #region UniversalTests

        [Fact]
        public void NullFileName()
        {
            Assert.Throws<ArgumentNullException>("sourceFileName", () => File.Replace(null, "", ""));
            Assert.Throws<ArgumentNullException>("destinationFileName", () => File.Replace("", null, ""));
        }

        [Fact]
        public void NoBackup_FileCopiedAndDeleted()
        {
            string srcPath = GetTestFilePath();
            string destPath = GetTestFilePath();

            byte[] srcContents = new byte[] { 1, 2, 3, 4, 5 };
            File.WriteAllBytes(srcPath, srcContents);

            byte[] destContents = new byte[] { 6, 7, 8, 9, 10 };
            File.WriteAllBytes(destPath, destContents);

            Replace(srcPath, destPath, null);

            Assert.False(File.Exists(srcPath));
            Assert.Equal(srcContents, File.ReadAllBytes(destPath));
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void Backup_FileCopiedAndDeleted_DestCopied(bool ignoreMetadataErrors)
        {
            string srcPath = GetTestFilePath();
            string destPath = GetTestFilePath();
            string destBackupPath = GetTestFilePath();

            byte[] srcContents = new byte[] { 1, 2, 3, 4, 5 };
            File.WriteAllBytes(srcPath, srcContents);

            byte[] destContents = new byte[] { 6, 7, 8, 9, 10 };
            File.WriteAllBytes(destPath, destContents);

            Replace(srcPath, destPath, destBackupPath, ignoreMetadataErrors);

            Assert.False(File.Exists(srcPath));
            Assert.Equal(srcContents, File.ReadAllBytes(destPath));
            Assert.Equal(destContents, File.ReadAllBytes(destBackupPath));
        }

        [Fact]
        public void NonExistentSourcePath()
        {
            Assert.Throws<FileNotFoundException>(() => Replace(GetTestFilePath(), GetTestFilePath(), null));
        }

        [Fact]
        public void InvalidFileNames()
        {
            string testFile = GetTestFilePath();
            string testFile2 = GetTestFilePath();
            File.Create(testFile).Dispose();

            Assert.Throws<ArgumentException>(() => Replace(testFile, "\0", null));
            Assert.Throws<ArgumentException>(() => Replace(testFile, "*\0*", null));

            Assert.Throws<ArgumentException>(() => Replace("*\0*", testFile, null));
            Assert.Throws<ArgumentException>(() => Replace("\0", testFile, null));

            Assert.Throws<ArgumentException>(() => Replace(testFile, testFile2, "\0"));
            Assert.Throws<ArgumentException>(() => Replace(testFile, testFile2, "*\0*"));
        }

        #endregion
    }

    public class File_Replace_str_str_str_b : File_Replace_str_str_str
    {
        #region Utilities

        public override void Replace(string source, string dest, string destBackup)
        {
            File.Replace(source, dest, destBackup, false);
        }
        
        #endregion
    }
}
