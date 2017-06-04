// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
using System.DirectoryServices;
using System.Security.Principal;
using Xunit;

namespace System.Security.AccessControl
{
    public class DirectoryObjectSecurityTests
    {
        [Fact]
        public void ObjectInitialization_DefaultConstructor_Success()
        {
            var activeDirectorySecurity = new ActiveDirectorySecurity();
            Assert.NotNull(activeDirectorySecurity);
        }

        [Fact]
        public void ObjectInitialization_CommonSecurityDescriptor_Success()
        {
            
            CommonSecurityDescriptor descriptor = new CommonSecurityDescriptor(false, false, string.Empty);
            var customObjectSecurity = new CustomDirectoryObjectSecurity(descriptor);
            Assert.NotNull(customObjectSecurity);
        }

        [Fact]
        public void ObjectInitialization_InvalidArgument()
        {
            Assert.Throws<ArgumentNullException>(() => new CustomDirectoryObjectSecurity(null));
        }

        [Fact]
        public void GetAccessRules_InvalidTargetType()
        {
            var activeDirectorySecurity = new ActiveDirectorySecurity();
            Assert.Throws<ArgumentException>(() => 
               activeDirectorySecurity
               .GetAccessRules(false, false, typeof(System.Security.Principal.GenericPrincipal)));
        }

        [Fact]
        public void GetAccessRules_TargetType_SecurityIdentifier_ReturnsValidObject()
        {
            CommonSecurityDescriptor descriptor = new CommonSecurityDescriptor(false, false, string.Empty);
            var customObjectSecurity = new CustomDirectoryObjectSecurity(descriptor);

            AuthorizationRuleCollection ruleCollection =
                customObjectSecurity
               .GetAccessRules(false, false, typeof(System.Security.Principal.SecurityIdentifier));

            Assert.NotNull(ruleCollection);
        }

        [Fact]
        public void GetAccessRules_TargetType_NTAccount_ReturnsValidObject()
        {
            CommonSecurityDescriptor descriptor = new CommonSecurityDescriptor(false, false, string.Empty);
            var customObjectSecurity = new CustomDirectoryObjectSecurity(descriptor);

            AuthorizationRuleCollection ruleCollection =
                customObjectSecurity
               .GetAccessRules(false, false, typeof(System.Security.Principal.NTAccount));

            Assert.NotNull(ruleCollection);
        }

        [Fact]
        public void GetAuditRules_ReturnsValidObject()
        {
            CommonSecurityDescriptor descriptor = new CommonSecurityDescriptor(false, false, string.Empty);
            var customObjectSecurity = new CustomDirectoryObjectSecurity(descriptor);

            AuthorizationRuleCollection ruleCollection =
                customObjectSecurity
               .GetAuditRules(true, false, typeof(System.Security.Principal.NTAccount));

            Assert.NotNull(ruleCollection);
        }

        [Fact]
        public void RemoveAuditRuleAll_InvalidArguments()
        {
            var customObjectSecurity = new CustomDirectoryObjectSecurity();
            Assert.Throws<ArgumentNullException>(() => customObjectSecurity.RemoveAuditRuleAll(null));
        }

        [Fact]
        public void RemoveAuditRuleSpecific_InvalidArguments()
        {
            var customObjectSecurity = new CustomDirectoryObjectSecurity();
            Assert.Throws<ArgumentNullException>(() => customObjectSecurity.RemoveAuditRuleSpecific(null));
        }

        [Fact]
        public void RemoveAuditRule_InvalidArguments()
        {
            var customObjectSecurity = new CustomDirectoryObjectSecurity();
            Assert.Throws<ArgumentNullException>(() => customObjectSecurity.RemoveAuditRule(null));
        }

        [Fact]
        public void SetAuditRule_InvalidArguments()
        {
            var customObjectSecurity = new CustomDirectoryObjectSecurity();
            Assert.Throws<ArgumentNullException>(() => customObjectSecurity.SetAuditRule(null));
        }
        [Fact]
        public void AdduditRule_InvalidArguments()
        {
            var customObjectSecurity = new CustomDirectoryObjectSecurity();
            Assert.Throws<ArgumentNullException>(() => customObjectSecurity.AddAuditRule(null));
        }

        [Fact]
        public void RemoveAccessRuleSpecific_InvalidArguments()
        {
            var customObjectSecurity = new CustomDirectoryObjectSecurity();
            Assert.Throws<ArgumentNullException>(() => customObjectSecurity.RemoveAccessRuleSpecific(null));
        }

        [Fact]
        public void RemoveAccessRuleAll_InvalidArguments()
        {
            var customObjectSecurity = new CustomDirectoryObjectSecurity();
            Assert.Throws<ArgumentNullException>(() => customObjectSecurity.RemoveAccessRuleAll(null));
        }

        [Fact]
        public void RemoveAccessRule_InvalidArguments()
        {
            var customObjectSecurity = new CustomDirectoryObjectSecurity();
            Assert.Throws<ArgumentNullException>(() => customObjectSecurity.RemoveAccessRule(null));
        }

        [Fact]
        public void ResetAccessRule_InvalidArguments()
        {
            var customObjectSecurity = new CustomDirectoryObjectSecurity();
            Assert.Throws<ArgumentNullException>(() => customObjectSecurity.ResetAccessRule(null));
        }

        [Fact]
        public void SetAccessRule_InvalidArguments()
        {
            var customObjectSecurity = new CustomDirectoryObjectSecurity();
            Assert.Throws<ArgumentNullException>(() => customObjectSecurity.SetAccessRule(null));
        }

        [Fact]
        public void AddAccessRule_InvalidArguments()
        {
            var customObjectSecurity = new CustomDirectoryObjectSecurity();
            Assert.Throws<ArgumentNullException>(() => customObjectSecurity.AddAccessRule(null));
        }

        class CustomDirectoryObjectSecurity : DirectoryObjectSecurity
        {
            public CustomDirectoryObjectSecurity()
            {

            }
            public CustomDirectoryObjectSecurity(CommonSecurityDescriptor securityDescriptor) : base(securityDescriptor)
            {

            }

            public override Type AccessRightType => throw new NotImplementedException();

            public override Type AccessRuleType => throw new NotImplementedException();

            public override Type AuditRuleType => throw new NotImplementedException();

            public override AccessRule AccessRuleFactory(IdentityReference identityReference, int accessMask, bool isInherited, InheritanceFlags inheritanceFlags, PropagationFlags propagationFlags, AccessControlType type)
            {
                throw new NotImplementedException();
            }

            public override AuditRule AuditRuleFactory(IdentityReference identityReference, int accessMask, bool isInherited, InheritanceFlags inheritanceFlags, PropagationFlags propagationFlags, AuditFlags flags)
            {
                throw new NotImplementedException();
            }
            public new void RemoveAuditRuleAll(ObjectAuditRule rule)
            {
                base.RemoveAuditRuleAll(rule);
            }
            public new void RemoveAuditRuleSpecific(ObjectAuditRule rule)
            {
                base.RemoveAuditRuleSpecific(rule);
            }
            public new  bool RemoveAuditRule(ObjectAuditRule rule)
            {
                return base.RemoveAuditRule(rule);
            }
            public new void SetAuditRule(ObjectAuditRule rule)
            {
                 base.SetAuditRule(rule);
            }
            public new void AddAuditRule(ObjectAuditRule rule)
            {
                base.AddAuditRule(rule);
            }
            public new  void RemoveAccessRuleSpecific(ObjectAccessRule rule)
            {
                base.RemoveAccessRuleSpecific(rule);
            }
            public new void RemoveAccessRuleAll(ObjectAccessRule rule)
            {
                base.RemoveAccessRuleAll(rule);
            }
            public new  bool RemoveAccessRule(ObjectAccessRule rule)
            {
                return base.RemoveAccessRule(rule);
            }
            public new void ResetAccessRule(ObjectAccessRule rule)
            {
                base.ResetAccessRule(rule);
            }
            public new void SetAccessRule(ObjectAccessRule rule)
            {
                base.SetAccessRule(rule);
            }
            public new  void AddAccessRule(ObjectAccessRule rule)
            {
                base.AddAccessRule(rule);
            }
        }

    }
   
}
