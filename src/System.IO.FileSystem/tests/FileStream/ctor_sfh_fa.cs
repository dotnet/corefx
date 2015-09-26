﻿// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.Win32.SafeHandles;
using System;
using System.IO;
using Xunit;

namespace System.IO.Tests
{
    public class FileStream_ctor_sfh_fa : FileSystemTest
    {
        protected virtual FileStream CreateFileStream(SafeFileHandle handle, FileAccess access)
        {
            return new FileStream(handle, access);
        }

        [Fact]
        public void InvalidHandleThrows()
        {
            Assert.Throws<ArgumentException>("handle", () => CreateFileStream(new SafeFileHandle(new IntPtr(-1), true), FileAccess.Read));
        }

        [Fact]
        public void InvalidAccessThrows()
        {
            using (FileStream fs = new FileStream(GetTestFilePath(), FileMode.Create))
            {
                Assert.Throws<ArgumentOutOfRangeException>("access", () => CreateFileStream(fs.SafeFileHandle, ~FileAccess.Read));
            }
        }

        [Fact]
        public void FileAccessRead()
        {
            string fileName = GetTestFilePath();

            using (FileStream fs = new FileStream(fileName, FileMode.Create))
            {
                fs.WriteByte(0);
            }

            using (FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read))
            {
                using (FileStream fsr = CreateFileStream(fs.SafeFileHandle, FileAccess.Read))
                {
                    Assert.True(fsr.CanRead);
                    Assert.Equal(0, fsr.ReadByte());
                    Assert.False(fsr.CanWrite);
                    Assert.Throws<NotSupportedException>(() => fsr.WriteByte(0));
                    Assert.True(fsr.CanSeek);
                }
            }
        }

        [Fact]
        public void FileAccessWrite()
        {
            using (FileStream fs = new FileStream(GetTestFilePath(), FileMode.Create, FileAccess.Write))
            {
                using (FileStream fsw = CreateFileStream(fs.SafeFileHandle, FileAccess.Write))
                {
                    Assert.False(fsw.CanRead);
                    Assert.Throws<NotSupportedException>(() => fsw.ReadByte());
                    Assert.True(fsw.CanWrite);
                    fsw.WriteByte(0); // should not throw
                    Assert.True(fsw.CanSeek);
                }
            }
        }


        [Fact]
        public void FileAccessReadWrite()
        {
            string fileName = GetTestFilePath();

            using (FileStream fs = new FileStream(fileName, FileMode.Create))
            {
                fs.WriteByte(0);
            }

            using (FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.ReadWrite))
            {
                using (FileStream fsrw = CreateFileStream(fs.SafeFileHandle, FileAccess.ReadWrite))
                {
                    Assert.True(fsrw.CanRead);
                    Assert.Equal(0, fsrw.ReadByte());
                    Assert.True(fsrw.CanWrite);
                    fsrw.WriteByte(0); // should not throw
                    Assert.True(fsrw.CanSeek);
                }
            }
        }

        [Fact]
        public void InconsistentFileAccessThrows()
        {
            string fileName = GetTestFilePath();

            using (FileStream fs = new FileStream(fileName, FileMode.Create))
            {
                fs.WriteByte(0);
            }

            using (FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read))
            {
                FileStream fsw = CreateFileStream(fs.SafeFileHandle, FileAccess.Write);
                Assert.False(fsw.CanRead);
                Assert.Throws<NotSupportedException>(() => fsw.ReadByte());
                Assert.True(fsw.CanWrite);
                // doesn't throw due to buffering.
                fsw.WriteByte(0);
                Assert.True(fsw.CanSeek);
                // throws due to FS trying to flush write buffer
                Assert.Throws<UnauthorizedAccessException>(() => fsw.Dispose());
            }

            using (FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Write))
            {
                using (FileStream fsr = CreateFileStream(fs.SafeFileHandle, FileAccess.Read))
                {
                    Assert.True(fsr.CanRead);
                    Assert.Throws<UnauthorizedAccessException>(() => fsr.ReadByte());
                    Assert.False(fsr.CanWrite);
                    Assert.Throws<NotSupportedException>(() => fsr.WriteByte(0));
                    Assert.True(fsr.CanSeek);
                }
            }
        }
    }
}
