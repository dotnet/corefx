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
        public void GetAccessControl_DirectoryInfo()
        {
            DirectoryInfo directoryInfo = new DirectoryInfo("\\");

            DirectorySecurity directorySecurity = FileSystemAclExtensions.GetAccessControl(directoryInfo);

            Assert.NotNull(directorySecurity);
        }

        [Fact]
        public void GetAccessControl_DirectoryInfo_AccessControlSections_InvalidArguments()
        {
            Assert.Throws<NullReferenceException>(() => FileSystemAclExtensions.GetAccessControl((DirectoryInfo)null, new AccessControlSections()));
        }

        [Fact]
        public void GetAccessControl_DirectoryInfo_AccessControlSections()
        {
            DirectoryInfo directoryInfo = new DirectoryInfo("\\");
            AccessControlSections accessControlSections = new AccessControlSections();

            DirectorySecurity directorySecurity = FileSystemAclExtensions.GetAccessControl(directoryInfo, accessControlSections);

            Assert.NotNull(directorySecurity);
        }

        [Fact]
        public void GetAccessControl_FileInfo_InvalidArguments()
        {
            Assert.Throws<NullReferenceException>(() => FileSystemAclExtensions.GetAccessControl((FileInfo)null));
        }

        [Fact]
        public void GetAccessControl_FileInfo()
        {
            FileInfo fileInfo = new FileInfo("\\");

            FileSecurity fileSecurity = FileSystemAclExtensions.GetAccessControl(fileInfo);

            Assert.NotNull(fileSecurity);
        }

        [Fact]
        public void GetAccessControl_FileInfo_AccessControlSections_InvalidArguments()
        {
            Assert.Throws<NullReferenceException>(() => FileSystemAclExtensions.GetAccessControl((FileInfo)null, new AccessControlSections()));
        }

        [Fact]
        public void GetAccessControl_FileInfo_AccessControlSections()
        {
            FileInfo fileInfo = new FileInfo("\\");
            AccessControlSections accessControlSections = new AccessControlSections();

            FileSecurity fileSecurity = FileSystemAclExtensions.GetAccessControl(fileInfo, accessControlSections);

            Assert.NotNull(fileSecurity);
        }

        [Fact]
        public void GetAccessControl_Filestream_InvalidArguments()
        {
            Assert.Throws<NullReferenceException>(() => FileSystemAclExtensions.GetAccessControl((FileStream)null));
        }

        [Fact]
        public void SetAccessControl_DirectoryInfo_DirectorySecurity_InvalidArguments()
        {
            DirectoryInfo directoryInfo = new DirectoryInfo("\\");
            Assert.Throws<ArgumentNullException>("directorySecurity", () => FileSystemAclExtensions.SetAccessControl(directoryInfo, (DirectorySecurity)null));
        }

        [Fact]
        public void SetAccessControl_DirectoryInfo_DirectorySecurity()
        {
            DirectoryInfo directoryInfo = new DirectoryInfo("\\");
            DirectorySecurity directorySecurity = new DirectorySecurity();

            FileSystemAclExtensions.SetAccessControl(directoryInfo, directorySecurity);
        }

        [Fact]
        public void SetAccessControl_FileInfo_FileSecurity_InvalidArguments()
        {
            FileInfo fileInfo = new FileInfo("\\");
            Assert.Throws<ArgumentNullException>("fileSecurity", () => FileSystemAclExtensions.SetAccessControl(fileInfo, (FileSecurity)null));
        }

        [Fact]
        public void SetAccessControl_FileInfo_FileSecurity()
        {
            FileInfo fileInfo = new FileInfo("\\");
            FileSecurity fileSecurity = new FileSecurity();

            FileSystemAclExtensions.SetAccessControl(fileInfo, fileSecurity);
        }

        [Fact]
        public void SetAccessControl_FileStream_FileSecurity_InvalidArguments()
        {
            Assert.Throws<NullReferenceException>(() => FileSystemAclExtensions.SetAccessControl((FileStream)null, (FileSecurity)null));
        }
    }
}