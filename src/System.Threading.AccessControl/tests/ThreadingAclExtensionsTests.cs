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

        #region EventWaitHandle

        [Fact]
        [PlatformSpecific(TestPlatforms.Windows)]
        public static void EventWaitHandle_Create_NullSecurity()
        {
            AssertExtensions.Throws<ArgumentNullException>("eventSecurity", () =>
            {
                using EventWaitHandle eventHandle = EventWaitHandleAcl.Create(initialState: true, EventResetMode.AutoReset, "Test", out bool createdNew, eventSecurity: null);
            });
        }

        [Fact]
        public static void EventWaitHandle_Create_BeyondMaxLengthName()
        {
            AssertExtensions.Throws<ArgumentException>("name", () =>
            {
                string name = new string('X', Interop.Kernel32.MAX_PATH + 1);
                using EventWaitHandle eventHandle = EventWaitHandleAcl.Create(initialState: true, EventResetMode.AutoReset, name, out bool createdNew, GetBasicEventWaitHandleSecurity());
            });
        }

        [Theory]
        [PlatformSpecific(TestPlatforms.Windows)]
        [InlineData((EventResetMode)int.MinValue)]
        [InlineData((EventResetMode)(-1))]
        [InlineData((EventResetMode)2)]
        [InlineData((EventResetMode)int.MaxValue)]
        public static void EventWaitHandle_Create_InvalidMode(EventResetMode mode)
        {
            AssertExtensions.Throws<ArgumentOutOfRangeException>("mode", () =>
            {
                using EventWaitHandle eventHandle = EventWaitHandleAcl.Create(initialState: true, mode, "name", out bool createdNew, GetBasicEventWaitHandleSecurity());
            });
        }

        [Fact]
        [PlatformSpecific(TestPlatforms.Windows)]
        public static void EventWaitHandle_Create_BasicSecurity()
        {
            var security = GetBasicEventWaitHandleSecurity();
            CreateAndVerifyEventWaitHandle(security);
        }

        [Theory]
        [PlatformSpecific(TestPlatforms.Windows)]
        [InlineData(true,  EventResetMode.AutoReset,   EventWaitHandleRights.FullControl, AccessControlType.Allow)]
        [InlineData(true,  EventResetMode.AutoReset,   EventWaitHandleRights.FullControl, AccessControlType.Deny)]
        [InlineData(true,  EventResetMode.AutoReset,   EventWaitHandleRights.Synchronize, AccessControlType.Allow)]
        [InlineData(true,  EventResetMode.AutoReset,   EventWaitHandleRights.Synchronize, AccessControlType.Deny)]
        [InlineData(true,  EventResetMode.AutoReset,   EventWaitHandleRights.Modify,      AccessControlType.Allow)]
        [InlineData(true,  EventResetMode.AutoReset,   EventWaitHandleRights.Modify,      AccessControlType.Deny)]
        [InlineData(true,  EventResetMode.AutoReset,   EventWaitHandleRights.Modify | EventWaitHandleRights.Synchronize,      AccessControlType.Allow)]
        [InlineData(true,  EventResetMode.AutoReset,   EventWaitHandleRights.Modify | EventWaitHandleRights.Synchronize,      AccessControlType.Deny)]
        [InlineData(true,  EventResetMode.ManualReset, EventWaitHandleRights.FullControl, AccessControlType.Allow)]
        [InlineData(true,  EventResetMode.ManualReset, EventWaitHandleRights.FullControl, AccessControlType.Deny)]
        [InlineData(true,  EventResetMode.ManualReset, EventWaitHandleRights.Synchronize, AccessControlType.Allow)]
        [InlineData(true,  EventResetMode.ManualReset, EventWaitHandleRights.Synchronize, AccessControlType.Deny)]
        [InlineData(true,  EventResetMode.ManualReset, EventWaitHandleRights.Modify,      AccessControlType.Allow)]
        [InlineData(true,  EventResetMode.ManualReset, EventWaitHandleRights.Modify,      AccessControlType.Deny)]
        [InlineData(true, EventResetMode.ManualReset,  EventWaitHandleRights.Modify | EventWaitHandleRights.Synchronize, AccessControlType.Allow)]
        [InlineData(true, EventResetMode.ManualReset,  EventWaitHandleRights.Modify | EventWaitHandleRights.Synchronize, AccessControlType.Deny)]
        public static void EventWaitHandle_Create_SpecificParameters(bool initialState, EventResetMode mode, EventWaitHandleRights rights, AccessControlType accessControl)
        {
            var security = GetEventWaitHandleSecurity(WellKnownSidType.BuiltinUsersSid, rights, accessControl);
            CreateAndVerifyEventWaitHandle(initialState, mode, security);
               
        }

        #endregion

        #endregion

        #region Helper methods

        private static string GetRandomNameMaxLength()
        {
            return Guid.NewGuid().ToString("N");
        }

        private static EventWaitHandleSecurity GetBasicEventWaitHandleSecurity()
        {
            return GetEventWaitHandleSecurity(
                WellKnownSidType.BuiltinUsersSid,
                EventWaitHandleRights.FullControl,
                AccessControlType.Allow);
        }

        private static EventWaitHandleSecurity GetEventWaitHandleSecurity(WellKnownSidType sid, EventWaitHandleRights rights, AccessControlType accessControl)
        {
            var security = new EventWaitHandleSecurity();
            SecurityIdentifier identity = new SecurityIdentifier(sid, null);
            var accessRule = new EventWaitHandleAccessRule(identity, rights, accessControl);
            security.AddAccessRule(accessRule);
            return security;
        }

        private static void CreateAndVerifyEventWaitHandle(EventWaitHandleSecurity security)
        {

            CreateAndVerifyEventWaitHandle(initialState: true, EventResetMode.AutoReset, security);
        }

        private static void CreateAndVerifyEventWaitHandle(bool initialState, EventResetMode mode, EventWaitHandleSecurity expectedSecurity)
        {
            string name = GetRandomNameMaxLength();

            using EventWaitHandle eventHandle = EventWaitHandleAcl.Create(initialState, mode, name, out bool createdNew, expectedSecurity);

            Assert.NotNull(eventHandle);
            Assert.True(createdNew);

            EventWaitHandleSecurity actualSecurity = eventHandle.GetAccessControl();

            VerifyEventWaitHandleSecurity(expectedSecurity, actualSecurity);
        }

        private static void VerifyEventWaitHandleSecurity(EventWaitHandleSecurity expectedSecurity, EventWaitHandleSecurity actualSecurity)
        {
            Assert.Equal(typeof(EventWaitHandleRights), expectedSecurity.AccessRightType);
            Assert.Equal(typeof(EventWaitHandleRights), actualSecurity.AccessRightType);

            List<EventWaitHandleAccessRule> expectedAccessRules = expectedSecurity.GetAccessRules(includeExplicit: true, includeInherited: false, typeof(SecurityIdentifier))
                .Cast<EventWaitHandleAccessRule>().ToList();

            List<EventWaitHandleAccessRule> actualAccessRules = actualSecurity.GetAccessRules(includeExplicit: true, includeInherited: false, typeof(SecurityIdentifier))
                .Cast<EventWaitHandleAccessRule>().ToList();

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

        private static bool AreAccessRulesEqual(EventWaitHandleAccessRule expectedRule, EventWaitHandleAccessRule actualRule)
        {
            return
                expectedRule.AccessControlType == actualRule.AccessControlType &&
                expectedRule.EventWaitHandleRights == actualRule.EventWaitHandleRights &&
                expectedRule.InheritanceFlags == actualRule.InheritanceFlags &&
                expectedRule.PropagationFlags == actualRule.PropagationFlags;
        }

        #endregion
    }
}
