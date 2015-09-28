﻿// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.IO;
using System.Runtime.InteropServices;
using Xunit;

namespace System.IO.Tests
{
    public partial class FileStream_ctor_str_fm_fa_fs
    {
        [Fact]
        public void FileShareDeleteNew()
        {
            string fileName = GetTestFilePath();
            using (FileStream fs = CreateFileStream(fileName, FileMode.Create, FileAccess.ReadWrite, FileShare.Delete))
            {
                Assert.True(File.Exists(fileName));
                File.Delete(fileName);
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) // file sharing restriction limitations on Unix
                {
                    Assert.True(File.Exists(fileName));
                }
            }

            Assert.False(File.Exists(fileName));
        }

        [Fact]
        public void FileShareDeleteNewRename()
        {
            string fileName = GetTestFilePath();
            string newFileName = GetTestFilePath();
            using (FileStream fs = CreateFileStream(fileName, FileMode.Create, FileAccess.ReadWrite, FileShare.Delete))
            {
                Assert.True(File.Exists(fileName));
                File.Move(fileName, newFileName);
                Assert.False(File.Exists(fileName));
                Assert.True(File.Exists(newFileName));
            }
        }

        [Fact]
        public void FileShareDeleteExisting()
        {
            // create the file
            string fileName = GetTestFilePath();
            using (CreateFileStream(fileName, FileMode.Create, FileAccess.ReadWrite, FileShare.None))
            { }

            Assert.True(File.Exists(fileName));

            // Open with delete sharing
            using (FileStream fs = CreateFileStream(fileName, FileMode.Open, FileAccess.ReadWrite, FileShare.Delete))
            {
                File.Delete(fileName);
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) // file sharing restriction limitations on Unix
                {
                    Assert.True(File.Exists(fileName));
                }
            }

            Assert.False(File.Exists(fileName));
        }

        [Fact]
        public void FileShareDeleteExistingRename()
        {
            // create the file
            string fileName = GetTestFilePath();
            using (CreateFileStream(fileName, FileMode.Create, FileAccess.ReadWrite, FileShare.None))
            { }

            Assert.True(File.Exists(fileName));

            string newFileName = GetTestFilePath();
            using (FileStream fs = CreateFileStream(fileName, FileMode.Create, FileAccess.ReadWrite, FileShare.Delete))
            {
                Assert.True(File.Exists(fileName));
                File.Move(fileName, newFileName);
                Assert.False(File.Exists(fileName));
                Assert.True(File.Exists(newFileName));
            }
        }
        [Fact]
        [PlatformSpecific(PlatformID.Windows)] // file sharing restriction limitations on Unix
        public void FileShareDeleteExistingMultipleClients()
        {
            // create the file
            string fileName = GetTestFilePath();
            using (FileStream fs = CreateFileStream(fileName, FileMode.Create, FileAccess.ReadWrite, FileShare.None))
            {
                fs.WriteByte(0);
            }

            Assert.True(File.Exists(fileName));

            using (FileStream fs1 = CreateFileStream(fileName, FileMode.Open, FileAccess.ReadWrite, FileShare.Delete | FileShare.ReadWrite))
            {
                using (FileStream fs2 = CreateFileStream(fileName, FileMode.Open, FileAccess.ReadWrite, FileShare.Delete | FileShare.ReadWrite))
                {
                    File.Delete(fileName);
                    Assert.Equal(0, fs2.ReadByte());
                    Assert.True(File.Exists(fileName));
                }

                Assert.Equal(0, fs1.ReadByte());
                fs1.WriteByte(0xFF);

                // Any attempt to reopen a file in pending-delete state will return Access-denied
                Assert.Throws<UnauthorizedAccessException>(() => CreateFileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Delete | FileShare.ReadWrite));

                Assert.True(File.Exists(fileName));
            }

            Assert.False(File.Exists(fileName));
        }

        [Fact]
        [PlatformSpecific(PlatformID.Windows)] // file sharing restriction limitations on Unix
        public void FileShareWithoutDeleteThrows()
        {
            string fileName = GetTestFilePath();
            string newFileName = GetTestFilePath();

            // Create without delete sharing
            using (FileStream fs = CreateFileStream(fileName, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite))
            {
                FSAssert.ThrowsSharingViolation(() => File.Delete(fileName));
                FSAssert.ThrowsSharingViolation(() => File.Move(fileName, newFileName));
            }

            Assert.True(File.Exists(fileName));
        }
    }
}
