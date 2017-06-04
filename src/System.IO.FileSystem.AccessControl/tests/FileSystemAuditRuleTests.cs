// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.Security.Principal;
using Xunit;

namespace System.Security.AccessControl
{
    public class FileSystemAuditRuleTests
    {
        [Fact]
        public void ObjectInitialization_IdentityReference_FileSystemRights_AuditFlags_Success()
        {
            IdentityReference identity = new SecurityIdentifier(WellKnownSidType.WorldSid, null);
            FileSystemAuditRule auditRule = new FileSystemAuditRule(identity, FileSystemRights.ReadData, AuditFlags.Failure);
            Assert.NotNull(auditRule);
        }

        [Fact]
        public void ObjectInitialization_Identity_FileSystemRights_AuditFlags_InheritanceFlag_PropagationFlag_Success()
        {
            IdentityReference identity = new SecurityIdentifier(WellKnownSidType.WorldSid, null);
            FileSystemAuditRule auditRule = new FileSystemAuditRule(@"MYDOMAIN\MyAccount", FileSystemRights.ReadData,
                InheritanceFlags.ObjectInherit,PropagationFlags.InheritOnly,  AuditFlags.Failure);
            Assert.NotNull(auditRule);
        }

        [Fact]
        public void ObjectInitialization_Identity_FileSystemRights_AuditFlags_Success()
        {
            IdentityReference identity = new SecurityIdentifier(WellKnownSidType.WorldSid, null);
            FileSystemAuditRule auditRule = new FileSystemAuditRule(@"MYDOMAIN\MyAccount", FileSystemRights.ReadData, AuditFlags.Failure);
            Assert.NotNull(auditRule);
        }

        [Fact]
        public void ObjectInitialization_InvalidFileSystemRights()
        {
            FileSystemRights fileSystemRights = (FileSystemRights)(-1);
            IdentityReference identity = new SecurityIdentifier(WellKnownSidType.WorldSid, null);
            Assert.Throws< ArgumentOutOfRangeException>(()=> new FileSystemAuditRule(@"MYDOMAIN\MyAccount", fileSystemRights, AuditFlags.Failure));
        }


        [Fact]
        public void FileSystemRights_ReturnValidObject()
        {
            IdentityReference identity = new SecurityIdentifier(WellKnownSidType.WorldSid, null);
            FileSystemAuditRule auditRule = new FileSystemAuditRule(@"MYDOMAIN\MyAccount", FileSystemRights.ReadData, AuditFlags.Failure);
            var fileSystemRights= auditRule.FileSystemRights;
            Assert.NotNull(fileSystemRights);
        }





    }
}
