// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Xunit;

namespace System.IO.FileSystem.Tests
{
    public class Directory_GetFiles_str_str_so : Directory_GetFileSystemEntries_str_str_so
    {
        #region Utilities

        protected override bool TestFiles { get { return true; } }
        protected override bool TestDirectories { get { return false; } }

        public override string[] GetEntries(string path)
        {
            return Directory.GetFiles(path, "*", SearchOption.TopDirectoryOnly);
        }

        public override string[] GetEntries(string path, string searchPattern)
        {
            return Directory.GetFiles(path, searchPattern, SearchOption.TopDirectoryOnly);
        }

        public override string[] GetEntries(string path, string searchPattern, SearchOption option)
        {
            return Directory.GetFiles(path, searchPattern, option);
        }

        #endregion
    }
}