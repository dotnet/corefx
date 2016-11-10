// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.IO.Tests
{
    public class FileInfo_Replace : FileSystemTest
    {
        public FileInfo Replace(string source, string dest, string destBackup)
        {
            return new FileInfo(source).Replace(dest, destBackup);
        }

        [Fact]
        public void Null_FileName()
        {
            string testFile = GetTestFilePath();
            Assert.Throws<ArgumentNullException>("destinationFileName", () => Replace(testFile, null, ""));
        }

        [Fact]
        public void FileInfo_IsSame_NoBackup()
        {
            string srcFile = GetTestFilePath();
            File.Create(srcFile).Dispose();
            string destFile = GetTestFilePath();
            FileInfo destInfo = new FileInfo(destFile);
            FileInfo newInfo = Replace(srcFile, destFile, null);

            Assert.False(File.Exists(srcFile));
            Assert.NotNull(newInfo);
            Assert.Equal(destInfo, newInfo);
        }

        [Fact]
        public void FileInfo_IsSame_Backup()
        {
            string srcFile = GetTestFilePath();
            File.Create(srcFile).Dispose();
            string destFile = GetTestFilePath();
            string destBackupPath = GetTestFilePath();
            FileInfo destInfo = new FileInfo(destFile);
            FileInfo newInfo = Replace(srcFile, destFile, destBackupPath);
            var destContents = File.ReadAllBytes(destFile);

            Assert.False(File.Exists(srcFile));
            Assert.NotNull(newInfo);
            Assert.Equal(destInfo, newInfo);
            Assert.Equal(destContents, File.ReadAllBytes(destBackupPath));
        }
    }
}