// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Transactions;
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

        [Theory]
        [PlatformSpecific(TestPlatforms.Windows)]
        [InlineData(null)]
        [InlineData("")]
        public static void EventWaitHandle_Create_InvalidName(string name)
        {
            AssertExtensions.Throws<ArgumentException>("name", () =>
            {
                using EventWaitHandle eventHandle = EventWaitHandleAcl.Create(initialState: true, EventResetMode.AutoReset, name, out bool createdNew, GetBasicEventWaitHandleSecurity());
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
            VerifyEventWaitHandle(security);
        }

        [Theory]
        [PlatformSpecific(TestPlatforms.Windows)]
        [InlineData(true,  EventResetMode.AutoReset,   AccessControlType.Allow, Interop.Kernel32.MAXIMUM_ALLOWED | Interop.Kernel32.SYNCHRONIZE | Interop.Kernel32.EVENT_MODIFY_STATE)]
        [InlineData(false, EventResetMode.AutoReset,   AccessControlType.Allow, Interop.Kernel32.MAXIMUM_ALLOWED | Interop.Kernel32.SYNCHRONIZE | Interop.Kernel32.EVENT_MODIFY_STATE)]
        [InlineData(true,  EventResetMode.ManualReset, AccessControlType.Allow, Interop.Kernel32.MAXIMUM_ALLOWED | Interop.Kernel32.SYNCHRONIZE | Interop.Kernel32.EVENT_MODIFY_STATE)]
        [InlineData(false, EventResetMode.ManualReset, AccessControlType.Allow, Interop.Kernel32.MAXIMUM_ALLOWED | Interop.Kernel32.SYNCHRONIZE | Interop.Kernel32.EVENT_MODIFY_STATE)]
        [InlineData(true,  EventResetMode.AutoReset,   AccessControlType.Deny,  Interop.Kernel32.MAXIMUM_ALLOWED | Interop.Kernel32.SYNCHRONIZE | Interop.Kernel32.EVENT_MODIFY_STATE)]
        [InlineData(false, EventResetMode.AutoReset,   AccessControlType.Deny,  Interop.Kernel32.MAXIMUM_ALLOWED | Interop.Kernel32.SYNCHRONIZE | Interop.Kernel32.EVENT_MODIFY_STATE)]
        [InlineData(true,  EventResetMode.ManualReset, AccessControlType.Deny,  Interop.Kernel32.MAXIMUM_ALLOWED | Interop.Kernel32.SYNCHRONIZE | Interop.Kernel32.EVENT_MODIFY_STATE)]
        [InlineData(false, EventResetMode.ManualReset, AccessControlType.Deny,  Interop.Kernel32.MAXIMUM_ALLOWED | Interop.Kernel32.SYNCHRONIZE | Interop.Kernel32.EVENT_MODIFY_STATE)]
        public static void EventWaitHandle_Create_SpecificParameters(bool initialState, EventResetMode mode, AccessControlType accessControl, int rights)
        {
            var security = GetEventWaitHandleSecurity(WellKnownSidType.BuiltinUsersSid, (EventWaitHandleRights)rights, accessControl);
            if (accessControl == AccessControlType.Deny)
            {
                Assert.Throws<UnauthorizedAccessException>(() =>
                {
                    VerifyEventWaitHandle(initialState, mode, "MyName", security);
                });
            }
            else
            {
                VerifyEventWaitHandle(initialState, mode, "MyName", security);
            }
        }

        #endregion

        #endregion

        #region Helper methods

        private static EventWaitHandleSecurity GetBasicEventWaitHandleSecurity()
        {
            return GetEventWaitHandleSecurity(
                WellKnownSidType.BuiltinUsersSid,
                EventWaitHandleRights.Synchronize | EventWaitHandleRights.Modify | EventWaitHandleRights.FullControl,
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

        private static void VerifyEventWaitHandle(EventWaitHandleSecurity security)
        {
            VerifyEventWaitHandle(initialState: true, EventResetMode.AutoReset, "DefaultEventName", security);
        }

        private static void VerifyEventWaitHandle(bool initialState, EventResetMode mode, string name, EventWaitHandleSecurity expectedSecurity)
        {
            using EventWaitHandle eventHandle = EventWaitHandleAcl.Create(initialState, mode, name, out bool createdNew, expectedSecurity);

            Assert.NotNull(eventHandle);

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
