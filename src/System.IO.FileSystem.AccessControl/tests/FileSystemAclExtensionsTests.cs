// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information

using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Principal;
using Xunit;

namespace System.IO
{
    public class FileSystemAclExtensionsTests
    {
        private const int DefaultBufferSize = 4096;


        #region Test methods

        #region GetAccessControl

        [Fact]
        public void GetAccessControl_DirectoryInfo_InvalidArguments()
        {
            Assert.Throws<NullReferenceException>(() => FileSystemAclExtensions.GetAccessControl((DirectoryInfo)null));
        }

        [Fact]
        public void GetAccessControl_DirectoryInfo_ReturnsValidObject()
        {
            using (var directory = new TempDirectory())
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(directory.Path);

                DirectorySecurity directorySecurity = directoryInfo.GetAccessControl();

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
            using (var directory = new TempDirectory())
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(directory.Path);
                AccessControlSections accessControlSections = new AccessControlSections();

                DirectorySecurity directorySecurity = directoryInfo.GetAccessControl(accessControlSections);

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
            using (var directory = new TempDirectory())
            using (var file = new TempFile(Path.Combine(directory.Path, "file.txt")))
            {
                FileInfo fileInfo = new FileInfo(file.Path);

                FileSecurity fileSecurity = fileInfo.GetAccessControl();

                Assert.NotNull(fileSecurity);
                Assert.Equal(typeof(FileSystemRights), fileSecurity.AccessRightType);
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
            using (var directory = new TempDirectory())
            using (var file = new TempFile(Path.Combine(directory.Path, "file.txt")))
            {
                FileInfo fileInfo = new FileInfo(file.Path);
                AccessControlSections accessControlSections = new AccessControlSections();

                FileSecurity fileSecurity = fileInfo.GetAccessControl(accessControlSections);

                Assert.NotNull(fileSecurity);
                Assert.Equal(typeof(FileSystemRights), fileSecurity.AccessRightType);
            }
        }

        [Fact]
        public void GetAccessControl_Filestream_InvalidArguments()
        {
            Assert.Throws<NullReferenceException>(() => FileSystemAclExtensions.GetAccessControl((FileStream)null));
        }

        [Fact]
        public void GetAccessControl_Filestream_ReturnValidObject()
        {
            using (var directory = new TempDirectory())
            using (var file = new TempFile(Path.Combine(directory.Path, "file.txt")))
            using (FileStream fileStream = File.Open(file.Path, System.IO.FileMode.Append, System.IO.FileAccess.Write, System.IO.FileShare.None))
            {
                FileSecurity fileSecurity = FileSystemAclExtensions.GetAccessControl(fileStream);
                Assert.NotNull(fileSecurity);
                Assert.Equal(typeof(FileSystemRights), fileSecurity.AccessRightType);
            }
        }

        #endregion

        #region SetAccessControl

        [Fact]
        public void SetAccessControl_DirectoryInfo_DirectorySecurity_InvalidArguments()
        {
            using (var directory = new TempDirectory())
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(directory.Path);
                AssertExtensions.Throws<ArgumentNullException>("directorySecurity", () => directoryInfo.SetAccessControl((DirectorySecurity)null));
            }
        }

        [Fact]
        public void SetAccessControl_DirectoryInfo_DirectorySecurity_Success()
        {
            using (var directory = new TempDirectory())
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(directory.Path);
                DirectorySecurity directorySecurity = new DirectorySecurity();

                directoryInfo.SetAccessControl(directorySecurity);
            }
        }

        [Fact]
        public void SetAccessControl_FileInfo_FileSecurity_InvalidArguments()
        {
            using (var directory = new TempDirectory())
            using (var file = new TempFile(Path.Combine(directory.Path, "file.txt")))
            {
                FileInfo fileInfo = new FileInfo(file.Path);
                AssertExtensions.Throws<ArgumentNullException>("fileSecurity", () => fileInfo.SetAccessControl((FileSecurity)null));
            }
        }

        [Fact]
        public void SetAccessControl_FileInfo_FileSecurity_Success()
        {
            using (var directory = new TempDirectory())
            using (var file = new TempFile(Path.Combine(directory.Path, "file.txt")))
            {
                FileInfo fileInfo = new FileInfo(file.Path);
                FileSecurity fileSecurity = new FileSecurity();

                fileInfo.SetAccessControl(fileSecurity);
            }
        }

        [Fact]
        public void SetAccessControl_FileStream_FileSecurity_InvalidArguments()
        {
            Assert.Throws<NullReferenceException>(() => FileSystemAclExtensions.SetAccessControl((FileStream)null, (FileSecurity)null));
        }

        [Fact]
        public void SetAccessControl_FileStream_FileSecurity_InvalidFileSecurityObject()
        {
            using (var directory = new TempDirectory())
            using (var file = new TempFile(Path.Combine(directory.Path, "file.txt")))
            using (FileStream fileStream = File.Open(file.Path, System.IO.FileMode.Append, System.IO.FileAccess.Write, System.IO.FileShare.None))
            {
                AssertExtensions.Throws<ArgumentNullException>("fileSecurity", () => FileSystemAclExtensions.SetAccessControl(fileStream, (FileSecurity)null));
            }
        }

        [Fact]
        public void SetAccessControl_FileStream_FileSecurity_Success()
        {
            using (var directory = new TempDirectory())
            using (var file = new TempFile(Path.Combine(directory.Path, "file.txt")))
            using (FileStream fileStream = File.Open(file.Path, System.IO.FileMode.Append, System.IO.FileAccess.Write, System.IO.FileShare.None))
            {
                FileSecurity fileSecurity = new FileSecurity();
                FileSystemAclExtensions.SetAccessControl(fileStream, fileSecurity);
            }
        }

        #endregion

        #region DirectoryInfo Create

        [Fact]
        [SkipOnTargetFramework(TargetFrameworkMonikers.Uap)]
        public void DirectoryInfo_Create_NullDirectoryInfo()
        {
            DirectoryInfo info = null;
            DirectorySecurity security = new DirectorySecurity();

            Assert.Throws<ArgumentNullException>("directoryInfo", () =>
            {
                if (PlatformDetection.IsFullFramework)
                {
                    FileSystemAclExtensions.Create(info, security);
                }
                else
                {
                    info.Create(security);
                }
            });
        }

        [Fact]
        [SkipOnTargetFramework(TargetFrameworkMonikers.Uap)]
        public void DirectoryInfo_Create_NullDirectorySecurity()
        {
            DirectoryInfo info = new DirectoryInfo("path");

            Assert.Throws<ArgumentNullException>("directorySecurity", () =>
            {
                if (PlatformDetection.IsFullFramework)
                {
                    FileSystemAclExtensions.Create(info, null);
                }
                else
                {
                    info.Create(null);
                }
            });
        }

        [Fact]
        [SkipOnTargetFramework(TargetFrameworkMonikers.Uap)]
        public void DirectoryInfo_Create_NotFound()
        {
            using var directory = new TempDirectory();
            string path = Path.Combine(directory.Path, Guid.NewGuid().ToString(), "ParentDoesNotExist");
            DirectoryInfo info = new DirectoryInfo(path);
            DirectorySecurity security = new DirectorySecurity();

            Assert.Throws<UnauthorizedAccessException>(() =>
            {
                if (PlatformDetection.IsFullFramework)
                {
                    FileSystemAclExtensions.Create(info, security);
                }
                else
                {
                    info.Create(security);
                }
            });
        }

        [Fact]
        [SkipOnTargetFramework(TargetFrameworkMonikers.Uap)]
        public void DirectoryInfo_Create_DefaultDirectorySecurity()
        {
            DirectorySecurity security = new DirectorySecurity();
            VerifyDirectorySecurity(security);
        }

        [Theory]
        [SkipOnTargetFramework(TargetFrameworkMonikers.Uap)]
        [InlineData(WellKnownSidType.BuiltinUsersSid, FileSystemRights.ReadAndExecute, AccessControlType.Allow)]
        [InlineData(WellKnownSidType.BuiltinUsersSid, FileSystemRights.ReadAndExecute, AccessControlType.Deny)]
        [InlineData(WellKnownSidType.BuiltinUsersSid, FileSystemRights.WriteData, AccessControlType.Allow)]
        [InlineData(WellKnownSidType.BuiltinUsersSid, FileSystemRights.WriteData, AccessControlType.Deny)]
        [InlineData(WellKnownSidType.BuiltinUsersSid, FileSystemRights.FullControl, AccessControlType.Allow)]
        [InlineData(WellKnownSidType.BuiltinUsersSid, FileSystemRights.FullControl, AccessControlType.Deny)]
        public void DirectoryInfo_Create_DirectorySecurityWithSpecificAccessRule(
            WellKnownSidType sid,
            FileSystemRights rights,
            AccessControlType controlType)
        {
            DirectorySecurity security = GetDirectorySecurity(sid, rights, controlType);
            VerifyDirectorySecurity(security);
        }

        #endregion

        #region FileInfo Create

        [Fact]
        [SkipOnTargetFramework(TargetFrameworkMonikers.Uap)]
        public void FileInfo_Create_NullFileInfo()
        {
            FileInfo info = null;
            FileSecurity security = new FileSecurity();

            Assert.Throws<ArgumentNullException>("fileInfo", () =>
            {
                if (PlatformDetection.IsFullFramework)
                {
                    FileSystemAclExtensions.Create(info, FileMode.Create, FileSystemRights.WriteData, FileShare.Read, DefaultBufferSize, FileOptions.None, security);
                }
                else
                {
                    info.Create(FileMode.Create, FileSystemRights.WriteData, FileShare.Read, DefaultBufferSize, FileOptions.None, security);
                }
            });
        }

        [Fact]
        [SkipOnTargetFramework(TargetFrameworkMonikers.Uap)]
        public void FileInfo_Create_NullFileSecurity()
        {
            FileInfo info = new FileInfo("path");

            Assert.Throws<ArgumentNullException>("fileSecurity", () =>
            {
                if (PlatformDetection.IsFullFramework)
                {
                    FileSystemAclExtensions.Create(info, FileMode.Create, FileSystemRights.WriteData, FileShare.Read, DefaultBufferSize, FileOptions.None, null);
                }
                else
                {
                    info.Create(FileMode.Create, FileSystemRights.WriteData, FileShare.Read, DefaultBufferSize, FileOptions.None, null);
                }
            });
        }

        [Fact]
        [SkipOnTargetFramework(TargetFrameworkMonikers.Uap)]
        public void FileInfo_Create_NotFound()
        {
            using var directory = new TempDirectory();
            string path = Path.Combine(directory.Path, Guid.NewGuid().ToString(), "file.txt");
            FileInfo info = new FileInfo(path);
            FileSecurity security = new FileSecurity();

            Assert.Throws<DirectoryNotFoundException>(() =>
            {
                if (PlatformDetection.IsFullFramework)
                {
                    FileSystemAclExtensions.Create(info, FileMode.Create, FileSystemRights.WriteData, FileShare.Read, DefaultBufferSize, FileOptions.None, security);
                }
                else
                {
                    info.Create(FileMode.Create, FileSystemRights.WriteData, FileShare.Read, DefaultBufferSize, FileOptions.None, security);
                }
            });
        }

        [Theory]
        [SkipOnTargetFramework(TargetFrameworkMonikers.Uap)]
        [InlineData((FileMode)int.MinValue)]
        [InlineData((FileMode)0)]
        [InlineData((FileMode)int.MaxValue)]
        public void FileInfo_Create_FileSecurity_InvalidFileMode(FileMode invalidMode)
        {
            FileSecurity security = new FileSecurity();
            FileInfo info = new FileInfo("path");

            Assert.Throws<ArgumentOutOfRangeException>("mode", () =>
            {
                if (PlatformDetection.IsFullFramework)
                {
                    FileSystemAclExtensions.Create(info, invalidMode, FileSystemRights.WriteData, FileShare.Read, DefaultBufferSize, FileOptions.None, security); ;
                }
                else
                {
                    info.Create(invalidMode, FileSystemRights.WriteData, FileShare.Read, DefaultBufferSize, FileOptions.None, security);
                }
            });
        }

        [Theory]
        [SkipOnTargetFramework(TargetFrameworkMonikers.Uap)]
        [InlineData((FileShare)(-1))]
        [InlineData((FileShare)int.MaxValue)]
        public void FileInfo_Create_FileSecurity_InvalidFileShare(FileShare invalidFileShare)
        {
            FileSecurity security = new FileSecurity();
            FileInfo info = new FileInfo("path");

            Assert.Throws<ArgumentOutOfRangeException>("share", () =>
            {
                if (PlatformDetection.IsFullFramework)
                {
                    FileSystemAclExtensions.Create(info, FileMode.Create, FileSystemRights.WriteData, invalidFileShare, DefaultBufferSize, FileOptions.None, security);
                }
                else
                {
                    info.Create(FileMode.Create, FileSystemRights.WriteData, invalidFileShare, DefaultBufferSize, FileOptions.None, security);
                }
            });
        }

        [Theory]
        [SkipOnTargetFramework(TargetFrameworkMonikers.Uap)]
        [InlineData(int.MinValue)]
        [InlineData(0)]
        public void FileInfo_Create_FileSecurity_InvalidBufferSize(int invalidBufferSize)
        {
            FileSecurity security = new FileSecurity();
            FileInfo info = new FileInfo("path");

            Assert.Throws<ArgumentOutOfRangeException>("bufferSize", () =>
            {
                if (PlatformDetection.IsFullFramework)
                {
                    FileSystemAclExtensions.Create(info, FileMode.Create, FileSystemRights.WriteData, FileShare.Read, invalidBufferSize, FileOptions.None, security);
                }
                else
                {
                    info.Create(FileMode.Create, FileSystemRights.WriteData, FileShare.Read, invalidBufferSize, FileOptions.None, security);
                }
            });
        }

        [Theory]
        [SkipOnTargetFramework(TargetFrameworkMonikers.Uap)]
        [InlineData(FileMode.Truncate, FileSystemRights.Read)]
        [InlineData(FileMode.Truncate, FileSystemRights.ReadData)]
        [InlineData(FileMode.CreateNew, FileSystemRights.Read)]
        [InlineData(FileMode.CreateNew, FileSystemRights.ReadData)]
        [InlineData(FileMode.Create, FileSystemRights.Read)]
        [InlineData(FileMode.Create, FileSystemRights.ReadData)]
        [InlineData(FileMode.Append, FileSystemRights.Read)]
        [InlineData(FileMode.Append, FileSystemRights.ReadData)]
        public void FileInfo_Create_FileSecurity_ForbiddenCombo_FileModeFileSystemSecurity(FileMode mode, FileSystemRights rights)
        {
            FileSecurity security = new FileSecurity();
            FileInfo info = new FileInfo("path");

            Assert.Throws<ArgumentException>(() =>
            {
                if (PlatformDetection.IsFullFramework)
                {
                    FileSystemAclExtensions.Create(info, mode, rights, FileShare.Read, DefaultBufferSize, FileOptions.None, security);
                }
                else
                {
                    info.Create(mode, rights, FileShare.Read, DefaultBufferSize, FileOptions.None, security);
                }
            });
        }

        [Fact]
        [SkipOnTargetFramework(TargetFrameworkMonikers.Uap)]
        public void FileInfo_Create_DefaultFileSecurity()
        {
            FileSecurity security = new FileSecurity();
            VerifyFileSecurity(security);
        }

        [Theory]
        [SkipOnTargetFramework(TargetFrameworkMonikers.Uap)]
        [InlineData(WellKnownSidType.BuiltinUsersSid, FileSystemRights.ReadAndExecute, AccessControlType.Allow)]
        [InlineData(WellKnownSidType.BuiltinUsersSid, FileSystemRights.ReadAndExecute, AccessControlType.Deny)]
        [InlineData(WellKnownSidType.BuiltinUsersSid, FileSystemRights.WriteData, AccessControlType.Allow)]
        [InlineData(WellKnownSidType.BuiltinUsersSid, FileSystemRights.WriteData, AccessControlType.Deny)]
        [InlineData(WellKnownSidType.BuiltinUsersSid, FileSystemRights.FullControl, AccessControlType.Allow)]
        [InlineData(WellKnownSidType.BuiltinUsersSid, FileSystemRights.FullControl, AccessControlType.Deny)]
        public void FileInfo_Create_FileSecurity_SpecificAccessRule(WellKnownSidType sid, FileSystemRights rights, AccessControlType controlType)
        {
            FileSecurity security = GetFileSecurity(sid, rights, controlType);
            VerifyFileSecurity(security);
        }

        #endregion

        #endregion


        #region Helper methods

        private DirectorySecurity GetDirectorySecurity(WellKnownSidType sid, FileSystemRights rights, AccessControlType controlType)
        {
            DirectorySecurity security = new DirectorySecurity();

            SecurityIdentifier identity = new SecurityIdentifier(sid, null);
            FileSystemAccessRule accessRule = new FileSystemAccessRule(identity, rights, controlType);
            security.AddAccessRule(accessRule);

            return security;
        }

        private void VerifyDirectorySecurity(DirectorySecurity expectedSecurity)
        {
            using var directory = new TempDirectory();
            string path = Path.Combine(directory.Path, "directory");
            DirectoryInfo info = new DirectoryInfo(path);

            info.Create(expectedSecurity);

            Assert.True(Directory.Exists(path));

            DirectoryInfo actualInfo = new DirectoryInfo(info.FullName);

            DirectorySecurity actualSecurity = actualInfo.GetAccessControl();

            VerifyAccessSecurity(expectedSecurity, actualSecurity);
        }

        private FileSecurity GetFileSecurity(WellKnownSidType sid, FileSystemRights rights, AccessControlType controlType)
        {
            FileSecurity security = new FileSecurity();

            SecurityIdentifier identity = new SecurityIdentifier(sid, null);
            FileSystemAccessRule accessRule = new FileSystemAccessRule(identity, rights, controlType);
            security.AddAccessRule(accessRule);

            return security;
        }

        private void VerifyFileSecurity(FileSecurity expectedSecurity)
        {
            VerifyFileSecurity(FileMode.Create, FileSystemRights.WriteData, FileShare.Read, DefaultBufferSize, FileOptions.None, expectedSecurity);
        }

        private void VerifyFileSecurity(FileMode mode, FileSystemRights rights, FileShare share, int bufferSize, FileOptions options, FileSecurity expectedSecurity)
        {
            using var directory = new TempDirectory();

            string path = Path.Combine(directory.Path, "file.txt");
            FileInfo info = new FileInfo(path);

            info.Create(mode, rights, share, bufferSize, options, expectedSecurity);

            Assert.True(File.Exists(path));

            FileInfo actualInfo = new FileInfo(info.FullName);

            FileSecurity actualSecurity = actualInfo.GetAccessControl();

            VerifyAccessSecurity(expectedSecurity, actualSecurity);
        }

        private void VerifyAccessSecurity(CommonObjectSecurity expectedSecurity, CommonObjectSecurity actualSecurity)
        {
            Assert.Equal(typeof(FileSystemRights), expectedSecurity.AccessRightType);

            Assert.Equal(typeof(FileSystemRights), actualSecurity.AccessRightType);

            List<FileSystemAccessRule> expectedAccessRules = expectedSecurity.GetAccessRules(includeExplicit: true, includeInherited: false, typeof(SecurityIdentifier))
                .Cast<FileSystemAccessRule>().ToList();

            List<FileSystemAccessRule> actualAccessRules = actualSecurity.GetAccessRules(includeExplicit: true, includeInherited: false, typeof(SecurityIdentifier))
                .Cast<FileSystemAccessRule>().ToList();

            Assert.Equal(expectedAccessRules.Count, actualAccessRules.Count);
            if (expectedAccessRules.Count > 0)
            {
                Assert.All(expectedAccessRules, actualAccessRule =>
                {
                    int count = expectedAccessRules.Count(expectedAccessRule => AreAccessRulesEqual(expectedAccessRule, actualAccessRule));
                    Assert.True(count > 0);
                });
            }
        }

        private bool AreAccessRulesEqual(FileSystemAccessRule expectedRule, FileSystemAccessRule actualRule)
        {
            return
                expectedRule.AccessControlType == actualRule.AccessControlType &&
                expectedRule.FileSystemRights  == actualRule.FileSystemRights &&
                expectedRule.InheritanceFlags  == actualRule.InheritanceFlags &&
                expectedRule.PropagationFlags  == actualRule.PropagationFlags;
        }

        #endregion
    }
}
