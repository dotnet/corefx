// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Runtime.InteropServices;
using Xunit;

namespace System.IO.Tests
{
    public sealed class Directory_SetCurrentDirectory : RemoteExecutorTestBase
    {
        [Fact]
        public void Null_Path_Throws_ArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => Directory.SetCurrentDirectory(null));
        }

        [Fact]
        public void Empty_Path_Throws_ArgumentException()
        {
            Assert.Throws<ArgumentException>(() => Directory.SetCurrentDirectory(string.Empty));
        }

        [Fact]
        public void SetToNonExistentDirectory_ThrowsDirectoryNotFoundException()
        {
            Assert.Throws<DirectoryNotFoundException>(() => Directory.SetCurrentDirectory(GetTestFilePath()));
        }

        [Fact]
        public void SetToValidOtherDirectory()
        {
            RemoteInvoke(() =>
            {
                Directory.SetCurrentDirectory(TestDirectory);
                // On OSX, the temp directory /tmp/ is a symlink to /private/tmp, so setting the current
                // directory to a symlinked path will result in GetCurrentDirectory returning the absolute
                // path that followed the symlink.
                if (!RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    Assert.Equal(TestDirectory, Directory.GetCurrentDirectory());
                }
                return SuccessExitCode;
            }).Dispose();
        }
    }
}
