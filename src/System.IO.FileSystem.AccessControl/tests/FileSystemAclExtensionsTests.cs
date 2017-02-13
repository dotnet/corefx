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
        public void GetAccessControl_DirectoryInfo_ReturnsValidObject()
        {
            string directory = Path.GetTempPath() + Guid.NewGuid();

            using (new TempDirectory(directory))
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(directory);

                DirectorySecurity directorySecurity = FileSystemAclExtensions.GetAccessControl(directoryInfo);

                Assert.NotNull(directorySecurity);
                Assert.Equal(typeof(FileSystemRights), directorySecurity.AccessRightType);
            }
        }

        [Fact]
        public void GetAccessControl_DirectoryInfo_AccessControlSections_InvalidArguments()
        {
            Assert.Throws<NullReferenceException>(() => FileSystemAclExtensions.GetAccessControl((DirectoryInfo)null, new AccessControlSections()));
        }

        [Fact]
        public void GetAccessControl_DirectoryInfo_AccessControlSections_ReturnsValidObject()
        {
            string directory = Path.GetTempPath() + Guid.NewGuid();

            using (new TempDirectory(directory))
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(directory);
                AccessControlSections accessControlSections = new AccessControlSections();

                DirectorySecurity directorySecurity = FileSystemAclExtensions.GetAccessControl(directoryInfo, accessControlSections);

                Assert.NotNull(directorySecurity);
                Assert.Equal(typeof(FileSystemRights), directorySecurity.AccessRightType);
            }
        }

        [Fact]
        public void GetAccessControl_FileInfo_InvalidArguments()
        {
            Assert.Throws<NullReferenceException>(() => FileSystemAclExtensions.GetAccessControl((FileInfo)null));
        }

        [Fact]
        public void GetAccessControl_FileInfo_ReturnsValidObject()
        {
            string directory = Path.GetTempPath() + Guid.NewGuid();
            string file = Path.Combine(directory, Guid.NewGuid() + ".txt");

            using (new TempDirectory(directory))
            {
                using (new TempFile(file))
                {
                    FileInfo fileInfo = new FileInfo(file);

                    FileSecurity fileSecurity = FileSystemAclExtensions.GetAccessControl(fileInfo);

                    Assert.NotNull(fileSecurity);
                    Assert.Equal(typeof(FileSystemRights), fileSecurity.AccessRightType);
                }
            }
        }

        [Fact]
        public void GetAccessControl_FileInfo_AccessControlSections_InvalidArguments()
        {
            Assert.Throws<NullReferenceException>(() => FileSystemAclExtensions.GetAccessControl((FileInfo)null, new AccessControlSections()));
        }

        [Fact]
        public void GetAccessControl_FileInfo_AccessControlSections_ReturnsValidObject()
        {
            string directory = Path.GetTempPath() + Guid.NewGuid();
            string file = Path.Combine(directory, Guid.NewGuid() + ".txt");

            using (new TempDirectory(directory))
            {
                using (new TempFile(file))
                {
                    FileInfo fileInfo = new FileInfo(file);
                    AccessControlSections accessControlSections = new AccessControlSections();

                    FileSecurity fileSecurity = FileSystemAclExtensions.GetAccessControl(fileInfo, accessControlSections);

                    Assert.NotNull(fileSecurity);
                    Assert.Equal(typeof(FileSystemRights), fileSecurity.AccessRightType);
                }
            }
        }

        [Fact]
        public void GetAccessControl_Filestream_InvalidArguments()
        {
            Assert.Throws<NullReferenceException>(() => FileSystemAclExtensions.GetAccessControl((FileStream)null));
        }

        [Fact]
        public void SetAccessControl_DirectoryInfo_DirectorySecurity_InvalidArguments()
        {
            string directory = Path.GetTempPath() + Guid.NewGuid();

            using (new TempDirectory(directory))
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(directory);
                Assert.Throws<ArgumentNullException>("directorySecurity", () => FileSystemAclExtensions.SetAccessControl(directoryInfo, (DirectorySecurity) null));
            }
        }

        [Fact]
        public void SetAccessControl_DirectoryInfo_DirectorySecurity_Success()
        {
            string directory = Path.GetTempPath() + Guid.NewGuid();

            using (new TempDirectory(directory))
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(directory);
                DirectorySecurity directorySecurity = new DirectorySecurity();

                FileSystemAclExtensions.SetAccessControl(directoryInfo, directorySecurity);
            }
        }

        [Fact]
        public void SetAccessControl_FileInfo_FileSecurity_InvalidArguments()
        {
            string directory = Path.GetTempPath() + Guid.NewGuid();
            string file = Path.Combine(directory, Guid.NewGuid() + ".txt");

            using (new TempDirectory(directory))
            {
                using (new TempFile(file))
                {
                    FileInfo fileInfo = new FileInfo(file);
                    Assert.Throws<ArgumentNullException>("fileSecurity", () => FileSystemAclExtensions.SetAccessControl(fileInfo, (FileSecurity) null));
                }
            }
        }

        [Fact]
        public void SetAccessControl_FileInfo_FileSecurity_Success()
        {
            string directory = Path.GetTempPath() + Guid.NewGuid();
            string file = Path.Combine(directory, Guid.NewGuid() + ".txt");

            using (new TempDirectory(directory))
            {
                using (new TempFile(file))
                {
                    FileInfo fileInfo = new FileInfo(file);
                    FileSecurity fileSecurity = new FileSecurity();

                    FileSystemAclExtensions.SetAccessControl(fileInfo, fileSecurity);
                }
            }
        }

        [Fact]
        public void SetAccessControl_FileStream_FileSecurity_InvalidArguments()
        {
            Assert.Throws<NullReferenceException>(() => FileSystemAclExtensions.SetAccessControl((FileStream)null, (FileSecurity)null));
        }
    }
}