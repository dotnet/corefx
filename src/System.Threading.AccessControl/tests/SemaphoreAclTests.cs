using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Principal;
using Xunit;

namespace System.Threading.Tests
{
    public class SemaphoreAclTests : AclTests
    {
        private int _defaultInitialCount = 0;
        private int _defaultMaximumCount = 1;

        [Fact]
        [PlatformSpecific(TestPlatforms.Windows)]
        public void Semaphore_Create_NullSecurity()
        {
            using Semaphore _ = CreateAndVerifySemaphore(
                _defaultInitialCount,
                _defaultMaximumCount,
                name: GetRandomName(),
                expectedSecurity: null,
                expectedCreatedNew: true);
        }

        [Theory]
        [InlineData(-1, 1)]
        [InlineData(0, 0)]
        [InlineData(1, 0)]
        [PlatformSpecific(TestPlatforms.Windows)]
        public void Semaphore_Create_InvalidCounts(int initialCount, int maximumCount)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                using Semaphore _ = CreateAndVerifySemaphore(
                    initialCount,
                    maximumCount,
                    name: GetRandomName(),
                    expectedSecurity: GetBasicSemaphoreSecurity(),
                    expectedCreatedNew: true);
            });
        }

        [Theory]
        [PlatformSpecific(TestPlatforms.Windows)]
        [InlineData(null)]
        [InlineData("")]
        public void Semaphore_Create_NameMultipleNew(string name)
        {
            var security = GetBasicSemaphoreSecurity();
            bool expectedCreatedNew = true;

            using Semaphore semaphore1 = CreateAndVerifySemaphore(
                _defaultInitialCount,
                _defaultMaximumCount,
                name,
                security,
                expectedCreatedNew);

            using Semaphore semaphore2 = CreateAndVerifySemaphore(
                _defaultInitialCount,
                _defaultMaximumCount,
                name,
                security,
                expectedCreatedNew);
        }

        [Fact]
        [PlatformSpecific(TestPlatforms.Windows)]
        public void Semaphore_Create_CreateNewExisting()
        {
            string name = GetRandomName();
            var security = GetBasicSemaphoreSecurity();

            using Semaphore SemaphoreNew = CreateAndVerifySemaphore(
                _defaultInitialCount,
                _defaultMaximumCount,
                name,
                security,
                expectedCreatedNew: true);

            using Semaphore SemaphoreExisting = CreateAndVerifySemaphore(
                _defaultInitialCount,
                _defaultMaximumCount,
                name,
                security,
                expectedCreatedNew: false);
        }

        [Fact]
        [PlatformSpecific(TestPlatforms.Windows)]
        public void Semaphore_Create_BeyondMaxPathLength()
        {
            string name = new string('x', Interop.Kernel32.MAX_PATH + 100);

            if (PlatformDetection.IsFullFramework)
            {
                Assert.Throws<ArgumentException>(() =>
                {
                    using Semaphore _ = CreateAndVerifySemaphore(
                        _defaultInitialCount,
                        _defaultMaximumCount,
                        name,
                        GetBasicSemaphoreSecurity(),
                        expectedCreatedNew: true);
                });
            }
            else
            {
                using Semaphore Semaphore = CreateAndVerifySemaphore(
                    _defaultInitialCount,
                    _defaultMaximumCount,
                    name,
                    GetBasicSemaphoreSecurity(),
                    expectedCreatedNew: true);

                using Semaphore openedByName = Semaphore.OpenExisting(name);
                Assert.NotNull(openedByName);
            }
        }

        [Theory]
        [PlatformSpecific(TestPlatforms.Windows)]
        [InlineData(SemaphoreRights.FullControl, AccessControlType.Allow)]
        [InlineData(SemaphoreRights.FullControl, AccessControlType.Deny)]
        [InlineData(SemaphoreRights.Synchronize, AccessControlType.Allow)]
        [InlineData(SemaphoreRights.Synchronize, AccessControlType.Deny)]
        [InlineData(SemaphoreRights.Modify, AccessControlType.Allow)]
        [InlineData(SemaphoreRights.Modify, AccessControlType.Deny)]
        [InlineData(SemaphoreRights.Modify | SemaphoreRights.Synchronize, AccessControlType.Allow)]
        [InlineData(SemaphoreRights.Modify | SemaphoreRights.Synchronize, AccessControlType.Deny)]
        public void Semaphore_Create_SpecificSecurity(SemaphoreRights rights, AccessControlType accessControl)
        {
            var security = GetSemaphoreSecurity(WellKnownSidType.BuiltinUsersSid, rights, accessControl);

            CreateAndVerifySemaphore(
                _defaultInitialCount,
                _defaultMaximumCount,
                GetRandomName(),
                security,
                expectedCreatedNew: true);

        }

        private SemaphoreSecurity GetBasicSemaphoreSecurity()
        {
            return GetSemaphoreSecurity(
                WellKnownSidType.BuiltinUsersSid,
                SemaphoreRights.FullControl,
                AccessControlType.Allow);
        }

        private SemaphoreSecurity GetSemaphoreSecurity(WellKnownSidType sid, SemaphoreRights rights, AccessControlType accessControl)
        {
            var security = new SemaphoreSecurity();
            SecurityIdentifier identity = new SecurityIdentifier(sid, null);
            var accessRule = new SemaphoreAccessRule(identity, rights, accessControl);
            security.AddAccessRule(accessRule);
            return security;
        }

        private Semaphore CreateAndVerifySemaphore(int initialCount, int maximumCount, string name, SemaphoreSecurity expectedSecurity, bool expectedCreatedNew)
        {
            Semaphore Semaphore = SemaphoreAcl.Create(initialCount, maximumCount, name, out bool createdNew, expectedSecurity);
            Assert.NotNull(Semaphore);
            Assert.Equal(createdNew, expectedCreatedNew);

            if (expectedSecurity != null)
            {
                SemaphoreSecurity actualSecurity = Semaphore.GetAccessControl();
                VerifySemaphoreSecurity(expectedSecurity, actualSecurity);
            }

            return Semaphore;
        }

        private void VerifySemaphoreSecurity(SemaphoreSecurity expectedSecurity, SemaphoreSecurity actualSecurity)
        {
            Assert.Equal(typeof(SemaphoreRights), expectedSecurity.AccessRightType);
            Assert.Equal(typeof(SemaphoreRights), actualSecurity.AccessRightType);

            List<SemaphoreAccessRule> expectedAccessRules = expectedSecurity.GetAccessRules(includeExplicit: true, includeInherited: false, typeof(SecurityIdentifier))
                .Cast<SemaphoreAccessRule>().ToList();

            List<SemaphoreAccessRule> actualAccessRules = actualSecurity.GetAccessRules(includeExplicit: true, includeInherited: false, typeof(SecurityIdentifier))
                .Cast<SemaphoreAccessRule>().ToList();

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

        private bool AreAccessRulesEqual(SemaphoreAccessRule expectedRule, SemaphoreAccessRule actualRule)
        {
            return
                expectedRule.AccessControlType == actualRule.AccessControlType &&
                expectedRule.SemaphoreRights   == actualRule.SemaphoreRights &&
                expectedRule.InheritanceFlags  == actualRule.InheritanceFlags &&
                expectedRule.PropagationFlags  == actualRule.PropagationFlags;
        }
    }
}
