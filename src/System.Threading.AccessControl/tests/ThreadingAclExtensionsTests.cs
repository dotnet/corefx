// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Principal;
using Xunit;

namespace System.Threading.Tests
{
    public static class ThreadingAclExtensionsTests
    {
        #region Test methods

        #region Existence tests

        [Fact]
        [PlatformSpecific(TestPlatforms.Windows)]  // APIs not supported on Unix
        public static void ExistenceTest_Windows()
        {
            var e = new ManualResetEvent(true);
            var s = new Semaphore(1, 1);
            var m = new Mutex();

            Assert.NotNull(e.GetAccessControl());
            Assert.Throws<ArgumentNullException>(() => e.SetAccessControl(null));
            e.SetAccessControl(new EventWaitHandleSecurity());

            Assert.NotNull(s.GetAccessControl());
            Assert.Throws<ArgumentNullException>(() => s.SetAccessControl(null));
            s.SetAccessControl(new SemaphoreSecurity());

            Assert.NotNull(m.GetAccessControl());
            Assert.Throws<ArgumentNullException>(() => m.SetAccessControl(null));
            m.SetAccessControl(new MutexSecurity());
        }

        [Fact]
        [PlatformSpecific(TestPlatforms.AnyUnix)]  // APIs not supported on Unix
        public static void ExistenceTest_Unix()
        {
            var e = new ManualResetEvent(true);
            var s = new Semaphore(1, 1);
            var m = new Mutex();

            Assert.Throws<PlatformNotSupportedException>(() => e.GetAccessControl());
            Assert.Throws<PlatformNotSupportedException>(() => e.SetAccessControl(new EventWaitHandleSecurity()));
            Assert.Throws<PlatformNotSupportedException>(() => s.GetAccessControl());
            Assert.Throws<PlatformNotSupportedException>(() => s.SetAccessControl(new SemaphoreSecurity()));
            Assert.Throws<PlatformNotSupportedException>(() => m.GetAccessControl());
            Assert.Throws<PlatformNotSupportedException>(() => m.SetAccessControl(new MutexSecurity()));
        }

        #endregion

        #region Mutex

        [Fact]
        [PlatformSpecific(TestPlatforms.Windows)]
        public static void Mutex_Create_NullSecurity()
        {
            using Mutex _ = CreateAndVerifyMutex(initiallyOwned: true, GetRandomName(), expectedSecurity: null, expectedCreatedNew: true);
        }

        [Fact]
        [PlatformSpecific(TestPlatforms.Windows)]
        public static void Mutex_Create_NullName()
        {
            using Mutex _ = CreateAndVerifyMutex(initiallyOwned: true, name: null, GetBasicMutexSecurity(), expectedCreatedNew: true);
        }

        [Fact]
        [PlatformSpecific(TestPlatforms.Windows)]
        public static void Mutex_Create_NullNameMultipleNew()
        {
            string name = string.Empty;
            var security = GetBasicMutexSecurity();

            using Mutex mutex1 = CreateAndVerifyMutex(initiallyOwned: true, name, security, expectedCreatedNew: true);
            using Mutex mutex2 = CreateAndVerifyMutex(initiallyOwned: true, name, security, expectedCreatedNew: true);
        }

        [Fact]
        [PlatformSpecific(TestPlatforms.Windows)]
        public static void Mutex_Create_EmptyName()
        {
            using Mutex _ = CreateAndVerifyMutex(initiallyOwned: true, string.Empty, GetBasicMutexSecurity(), expectedCreatedNew: true);
        }

        [Fact]
        [PlatformSpecific(TestPlatforms.Windows)]
        public static void Mutex_Create_EmptyNameMultipleNew()
        {
            string name = string.Empty;
            var security = GetBasicMutexSecurity();

            using Mutex mutex1 = CreateAndVerifyMutex(initiallyOwned: true, name, security, expectedCreatedNew: true);
            using Mutex mutex2 = CreateAndVerifyMutex(initiallyOwned: true, name, security, expectedCreatedNew: true);
        }

        [Fact]
        [PlatformSpecific(TestPlatforms.Windows)]
        public static void Mutex_Create_CreateNewExisting()
        {
            string name = GetRandomName();
            var security = GetBasicMutexSecurity();

            using Mutex mutexNew      = CreateAndVerifyMutex(initiallyOwned: true, name, security, expectedCreatedNew: true);
            using Mutex mutexExisting = CreateAndVerifyMutex(initiallyOwned: true, name, security, expectedCreatedNew: false);
        }

        [Fact]
        [PlatformSpecific(TestPlatforms.Windows)]
        public static void Mutex_Create_BeyondMaxPathLength()
        {
            string name = new string('x', Interop.Kernel32.MAX_PATH + 100);

            if (PlatformDetection.IsFullFramework)
            {
                Assert.Throws<ArgumentException>(() =>
                {
                    using Mutex _ = CreateAndVerifyMutex(initiallyOwned: true, name, GetBasicMutexSecurity(), expectedCreatedNew: true);
                });
            }
            else
            {
                using Mutex mutex = CreateAndVerifyMutex(initiallyOwned: true, name, GetBasicMutexSecurity(), expectedCreatedNew: true);
                using Mutex openedByName = Mutex.OpenExisting(name);
                Assert.NotNull(openedByName);
            }
        }

        [Fact]
        [PlatformSpecific(TestPlatforms.Windows)]
        public static void Mutex_Create_BasicSecurity()
        {
            var security = GetBasicMutexSecurity();
            using Mutex _ = CreateAndVerifyMutex(initiallyOwned: true, GetRandomName(), security, expectedCreatedNew: true);
        }

        [Theory]
        [PlatformSpecific(TestPlatforms.Windows)]
        [InlineData(true,  MutexRights.FullControl, AccessControlType.Allow)]
        [InlineData(true,  MutexRights.FullControl, AccessControlType.Deny)]
        [InlineData(true,  MutexRights.Synchronize, AccessControlType.Allow)]
        [InlineData(true,  MutexRights.Synchronize, AccessControlType.Deny)]
        [InlineData(true,  MutexRights.Modify, AccessControlType.Allow)]
        [InlineData(true,  MutexRights.Modify, AccessControlType.Deny)]
        [InlineData(true,  MutexRights.Modify | MutexRights.Synchronize, AccessControlType.Allow)]
        [InlineData(true,  MutexRights.Modify | MutexRights.Synchronize, AccessControlType.Deny)]
        [InlineData(false, MutexRights.FullControl, AccessControlType.Allow)]
        [InlineData(false, MutexRights.FullControl, AccessControlType.Deny)]
        [InlineData(false, MutexRights.Synchronize, AccessControlType.Allow)]
        [InlineData(false, MutexRights.Synchronize, AccessControlType.Deny)]
        [InlineData(false, MutexRights.Modify, AccessControlType.Allow)]
        [InlineData(false, MutexRights.Modify, AccessControlType.Deny)]
        public static void Mutex_Create_SpecificParameters(bool initiallyOwned, MutexRights rights, AccessControlType accessControl)
        {
            var security = GetMutexSecurity(WellKnownSidType.BuiltinUsersSid, rights, accessControl);
            CreateAndVerifyMutex(initiallyOwned, GetRandomName(), security, expectedCreatedNew: true);

        }

        #endregion

        #endregion

        #region Helper methods

        private static string GetRandomName()
        {
            return Guid.NewGuid().ToString("N");
        }

        private static MutexSecurity GetBasicMutexSecurity()
        {
            return GetMutexSecurity(
                WellKnownSidType.BuiltinUsersSid,
                MutexRights.FullControl,
                AccessControlType.Allow);
        }

        private static MutexSecurity GetMutexSecurity(WellKnownSidType sid, MutexRights rights, AccessControlType accessControl)
        {
            var security = new MutexSecurity();
            SecurityIdentifier identity = new SecurityIdentifier(sid, null);
            var accessRule = new MutexAccessRule(identity, rights, accessControl);
            security.AddAccessRule(accessRule);
            return security;
        }

        private static Mutex CreateAndVerifyMutex(bool initiallyOwned, string name, MutexSecurity expectedSecurity, bool expectedCreatedNew)
        {
            Mutex mutex = MutexAcl.Create(initiallyOwned, name, out bool createdNew, expectedSecurity);
            Assert.NotNull(mutex);
            Assert.Equal(createdNew, expectedCreatedNew);

            if (expectedSecurity != null)
            {
                MutexSecurity actualSecurity = mutex.GetAccessControl();
                VerifyMutexSecurity(expectedSecurity, actualSecurity);
            }

            return mutex;
        }

        private static void VerifyMutexSecurity(MutexSecurity expectedSecurity, MutexSecurity actualSecurity)
        {
            Assert.Equal(typeof(MutexRights), expectedSecurity.AccessRightType);
            Assert.Equal(typeof(MutexRights), actualSecurity.AccessRightType);

            List<MutexAccessRule> expectedAccessRules = expectedSecurity.GetAccessRules(includeExplicit: true, includeInherited: false, typeof(SecurityIdentifier))
                .Cast<MutexAccessRule>().ToList();

            List<MutexAccessRule> actualAccessRules = actualSecurity.GetAccessRules(includeExplicit: true, includeInherited: false, typeof(SecurityIdentifier))
                .Cast<MutexAccessRule>().ToList();

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

        private static bool AreAccessRulesEqual(MutexAccessRule expectedRule, MutexAccessRule actualRule)
        {
            return
                expectedRule.AccessControlType == actualRule.AccessControlType &&
                expectedRule.MutexRights       == actualRule.MutexRights &&
                expectedRule.InheritanceFlags  == actualRule.InheritanceFlags &&
                expectedRule.PropagationFlags  == actualRule.PropagationFlags;
        }


        #endregion
    }
}
