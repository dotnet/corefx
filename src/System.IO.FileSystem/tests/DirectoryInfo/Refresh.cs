// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Xunit;

namespace System.IO.Tests
{
    public class DirectoryInfo_Refresh : FileSystemTest
    {
        #region UniversalTests

        [Fact]
        public void DeleteThenRefresh()
        {
            DirectoryInfo testDir = Directory.CreateDirectory(GetTestFilePath());
            testDir.Delete();
            testDir.Refresh();
            Assert.False(testDir.Exists);
        }

        [Fact]
        public void NameChange()
        {
            //NOTE: MoveTo adds a trailing slash to the path of the DirectoryInfo
            string source = GetTestFilePath();
            string dest = GetTestFilePath();
            DirectoryInfo testDir = Directory.CreateDirectory(source);
            testDir.MoveTo(dest);
            testDir.Refresh();
            Assert.Equal(testDir.FullName, dest + Path.DirectorySeparatorChar);
        }

        [Fact]
        public void AttributeChange()
        {
            DirectoryInfo testDir = Directory.CreateDirectory(GetTestFilePath());
            Assert.True((testDir.Attributes & FileAttributes.ReadOnly) != FileAttributes.ReadOnly);
            testDir.Attributes = FileAttributes.ReadOnly;
            testDir.Refresh();
            Assert.True((testDir.Attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly);
            testDir.Attributes = new FileAttributes();
            testDir.Refresh();
            Assert.True((testDir.Attributes & FileAttributes.ReadOnly) != FileAttributes.ReadOnly);
        }

        #endregion
    }
}
