// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Security.AccessControl;
using Xunit;

namespace System.IO
{
    public class FileSystemAclExtensionsTests
    {
        [Fact]
        public void GetAccessControl_DirectoryInfo_InvalidArguments()
        {
            Assert.Throws<NullReferenceException>(() => FileSystemAclExtensions.GetAccessControl((DirectoryInfo)null));
        }

        [Fact]
        public void GetAccessControl_DirectoryInfo_AccessControlSections_InvalidArguments()
        {
            Assert.Throws<NullReferenceException>(() => FileSystemAclExtensions.GetAccessControl((DirectoryInfo)null, new AccessControlSections()));
        }

        [Fact]
        public void GetAccessControl_FileInfo_InvalidArguments()
        {
            Assert.Throws<NullReferenceException>(() => FileSystemAclExtensions.GetAccessControl((FileInfo)null));
        }

        [Fact]
        public void GetAccessControl_FileInfo_AccessControlSections_InvalidArguments()
        {
            Assert.Throws<NullReferenceException>(() => FileSystemAclExtensions.GetAccessControl((FileInfo)null, new AccessControlSections()));
        }

        [Fact]
        public void GetAccessControl_Filestream_InvalidArguments()
        {
            Assert.Throws<NullReferenceException>(() => FileSystemAclExtensions.GetAccessControl((FileStream)null));
        }

        [Fact]
        public void SetAccessControl_DirectoryInfo_DirectorySecurity_InvalidArguments()
        {
            Assert.Throws<ArgumentNullException>(() => FileSystemAclExtensions.SetAccessControl((DirectoryInfo)null, (DirectorySecurity)null));
        }

        [Fact]
        public void SetAccessControl_FileInfo_FileSecurity_InvalidArguments()
        {
            Assert.Throws<ArgumentNullException>(() => FileSystemAclExtensions.SetAccessControl((FileInfo)null, (FileSecurity)null));
        }

        [Fact]
        public void SetAccessControl_FileStream_FileSecurity_InvalidArguments()
        {
            Assert.Throws<NullReferenceException>(() => FileSystemAclExtensions.SetAccessControl((FileStream)null, (FileSecurity)null));
        }
    }
}