// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Linq;
using System.Runtime.InteropServices;
using Xunit;

namespace System.IO.Tests
{
    public class Directory_GetDirectories_str : Directory_GetFileSystemEntries_str
    {
        protected override bool TestFiles { get { return false; } }
        protected override bool TestDirectories { get { return true; } }

        public override string[] GetEntries(string path)
        {
            return Directory.GetDirectories(path);
        }

        [ConditionalFact(nameof(CanCreateSymbolicLinks))]
        public void EnumerateWithSymLinkToDirectory()
        {
            using (var containingFolder = new TemporaryDirectory())
            {
                // Test a symlink to a directory that does and then doesn't exist
                using (var targetDir = new TemporaryDirectory())
                {
                    // Create a symlink to a folder that exists
                    string linkPath = Path.Combine(containingFolder.Path, Path.GetRandomFileName());
                    Assert.True(MountHelper.CreateSymbolicLink(linkPath, targetDir.Path, isDirectory: true));

                    Assert.True(Directory.Exists(linkPath));
                    Assert.Equal(1, GetEntries(containingFolder.Path).Count());
                }

                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    Assert.Equal(1, GetEntries(containingFolder.Path).Count());
                    Assert.Equal(0, Directory.GetFiles(containingFolder.Path).Count());
                }
                else
                {
                    // The target file is gone and the symlink still exists; since it can't be resolved,
                    // on Unix it's treated as a file rather than as a directory.
                    Assert.Equal(0, GetEntries(containingFolder.Path).Count());
                    Assert.Equal(1, Directory.GetFiles(containingFolder.Path).Count());
                }
            }
        }
    }

    public class Directory_GetDirectories_str_str : Directory_GetFileSystemEntries_str_str
    {
        protected override bool TestFiles { get { return false; } }
        protected override bool TestDirectories { get { return true; } }

        public override string[] GetEntries(string path)
        {
            return Directory.GetDirectories(path, "*");
        }

        public override string[] GetEntries(string path, string searchPattern)
        {
            return Directory.GetDirectories(path, searchPattern);
        }
    }

    public class Directory_GetDirectories_str_str_so : Directory_GetFileSystemEntries_str_str_so
    {
        protected override bool TestFiles { get { return false; } }
        protected override bool TestDirectories { get { return true; } }

        public override string[] GetEntries(string path)
        {
            return Directory.GetDirectories(path, "*", SearchOption.TopDirectoryOnly);
        }

        public override string[] GetEntries(string path, string searchPattern)
        {
            return Directory.GetDirectories(path, searchPattern, SearchOption.TopDirectoryOnly);
        }

        public override string[] GetEntries(string path, string searchPattern, SearchOption option)
        {
            return Directory.GetDirectories(path, searchPattern, option);
        }
    }
}
