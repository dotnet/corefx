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
        // Dec: 34603010
        // Hex: 0x2100002
        // As predefined by the different wait handles:
        // - https://source.dot.net/#System.Private.CoreLib/shared/System/Threading/EventWaitHandle.Windows.cs
        // - https://source.dot.net/#System.Private.CoreLib/shared/System/Threading/Mutex.Windows.cs
        private const int BasicAccessRights = Interop.Kernel32.MAXIMUM_ALLOWED | Interop.Kernel32.SYNCHRONIZE | Interop.Kernel32.EVENT_MODIFY_STATE;

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
            AssertExtensions.Throws<ArgumentNullException>("eventSecurity", () =>
            {
                using Mutex eventHandle = MutexAcl.Create(initiallyOwned: true, "Test", out bool createdNew, mutexSecurity: null);
            });
        }

        [Theory]
        [PlatformSpecific(TestPlatforms.Windows)]
        [InlineData(null)]
        [InlineData("")]
        public static void Mutex_Create_InvalidName(string name)
        {
            AssertExtensions.Throws<ArgumentException>("name", () =>
            {
                using Mutex eventHandle = MutexAcl.Create(initiallyOwned: true, name, out bool createdNew, GetBasicMutexSecurity());
            });
        }

        [Fact]
        public static void Mutex_Create_BeyondMaxLengthName()
        {
            AssertExtensions.Throws<ArgumentException>("name", () =>
            {
                string name = new string('X', Interop.Kernel32.MAX_PATH + 1);
                using Mutex eventHandle = MutexAcl.Create(initiallyOwned: true, name, out bool createdNew, GetBasicMutexSecurity());
            });
        }

        [Fact]
        [PlatformSpecific(TestPlatforms.Windows)]
        public static void Mutex_Create_BasicSecurity()
        {
            var security = GetBasicMutexSecurity();
            CreateAndVerifyMutex(security);
        }

        [Theory]
        [PlatformSpecific(TestPlatforms.Windows)]
        [InlineData(true,  AccessControlType.Allow, BasicAccessRights)]
        [InlineData(false, AccessControlType.Allow, BasicAccessRights)]
        [InlineData(true,  AccessControlType.Deny,  BasicAccessRights)]
        [InlineData(false, AccessControlType.Deny,  BasicAccessRights)]

        public static void Mutex_Create_SpecificParameters(bool initiallyOwned, AccessControlType accessControl, int rights)
        {
            var security = GetMutexSecurity(WellKnownSidType.BuiltinUsersSid, (MutexRights)rights, accessControl);
            CreateAndVerifyMutex(initiallyOwned, security);
               
        }

        #endregion

        #endregion

        #region Helper methods

        private static string GetRandomNameMaxLength()
        {
            return Guid.NewGuid().ToString("N");
        }

        private static MutexSecurity GetBasicMutexSecurity()
        {
            return GetMutexSecurity(
                WellKnownSidType.BuiltinUsersSid,
                (MutexRights)BasicAccessRights,
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

        private static void CreateAndVerifyMutex(MutexSecurity security)
        {
            CreateAndVerifyMutex(initiallyOwned: true, security);
        }

        private static void CreateAndVerifyMutex(bool initiallyOwned, MutexSecurity expectedSecurity)
        {
            string name = GetRandomNameMaxLength();

            using Mutex eventHandle = MutexAcl.Create(initiallyOwned, name, out bool createdNew, expectedSecurity);

            Assert.NotNull(eventHandle);
            Assert.True(createdNew);

            MutexSecurity actualSecurity = eventHandle.GetAccessControl();

            VerifyMutexSecurity(expectedSecurity, actualSecurity);
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
                    int count = expectedAccessRules.Count(expectedAccessRule => AreMutexAccessRulesEqual(expectedAccessRule, actualAccessRule));
                    Assert.True(count > 0);
                });
            }
        }

        private static bool AreMutexAccessRulesEqual(MutexAccessRule expectedRule, MutexAccessRule actualRule)
        {
            return
                expectedRule.AccessControlType == actualRule.AccessControlType &&
                expectedRule.MutexRights == actualRule.MutexRights &&
                expectedRule.InheritanceFlags == actualRule.InheritanceFlags &&
                expectedRule.PropagationFlags == actualRule.PropagationFlags;
        }

        #endregion
    }
}
