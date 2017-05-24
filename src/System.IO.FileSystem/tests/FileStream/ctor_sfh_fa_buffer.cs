// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Win32.SafeHandles;
using System;
using System.IO;
using Xunit;

namespace System.IO.Tests
{
    public class FileStream_ctor_sfh_fa_buffer : FileStream_ctor_sfh_fa
    {
        protected sealed override FileStream CreateFileStream(SafeFileHandle handle, FileAccess access)
        {
            return CreateFileStream(handle, access, 4096);
        }

        protected virtual FileStream CreateFileStream(SafeFileHandle handle, FileAccess access, int bufferSize)
        {
            return new FileStream(handle, access, bufferSize);
        }

        [Fact]
        public void InvalidBufferSizeThrows()
        {
            using (FileStream fs = new FileStream(GetTestFilePath(), FileMode.Create))
            {
                AssertExtensions.Throws<ArgumentOutOfRangeException>("bufferSize", () => CreateFileStream(fs.SafeFileHandle, FileAccess.Read, -1));
                AssertExtensions.Throws<ArgumentOutOfRangeException>("bufferSize", () => CreateFileStream(fs.SafeFileHandle, FileAccess.Read, 0));
            }
        }

        [Fact]
        public void ValidBufferSize()
        {
            using (FileStream fs = new FileStream(GetTestFilePath(), FileMode.Create))
            {
                using (FileStream fsw = CreateFileStream(fs.SafeFileHandle, FileAccess.Write, 64 * 1024))
                { }
            }
        }
    }
}
