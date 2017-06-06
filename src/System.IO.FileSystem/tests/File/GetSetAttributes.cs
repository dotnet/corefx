// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.IO.Tests
{
    public class File_GetSetAttributes : BaseGetSetAttributes
    {
        protected override FileAttributes Get(string path) => File.GetAttributes(path);
        protected override void Set(string path, FileAttributes attributes) => File.SetAttributes(path, attributes);

        // Getting only throws for File, not FileInfo
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
    }
}
