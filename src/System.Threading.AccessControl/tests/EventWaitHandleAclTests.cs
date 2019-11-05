// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace System.Threading.Tests
{
    public class EventWaitHandleAclTests : AclTests
    {
        [Fact]
        public void EventWaitHandle_Create_NullSecurity()
        {
            CreateAndVerifyEventWaitHandle(
                initialState: true,
                mode: EventResetMode.AutoReset,
                name: GetRandomName(),
                expectedSecurity: null,
                expectedCreatedNew: true).Dispose();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void EventWaitHandle_Create_NameMultipleNew(string name)
        {
            var security = GetBasicEventWaitHandleSecurity();

            using EventWaitHandle handle1 = CreateAndVerifyEventWaitHandle(
                initialState: true,
                mode: EventResetMode.AutoReset,
                name,
                security,
                expectedCreatedNew: true);

            using EventWaitHandle handle2 = CreateAndVerifyEventWaitHandle(
                initialState: true,
                mode: EventResetMode.AutoReset,
                name,
                security,
                expectedCreatedNew: true);
        }

        [Fact]
        public void EventWaitHandle_Create_CreateNewExisting()
        {
            string name = GetRandomName();
            var security = GetBasicEventWaitHandleSecurity();

            using EventWaitHandle handle1 = CreateAndVerifyEventWaitHandle(
                initialState: true,
                mode: EventResetMode.AutoReset,
                name,
                security,
                expectedCreatedNew: true);

            using EventWaitHandle handle2 = CreateAndVerifyEventWaitHandle(
                initialState: true,
                mode: EventResetMode.AutoReset,
                name,
                security,
                expectedCreatedNew: false);
        }

        [Fact]
        public void EventWaitHandle_Create_GlobalPrefixNameNotFound()
        {
            Assert.Throws<DirectoryNotFoundException>(() =>
            {
                string prefixedName = @"GLOBAL\" + GetRandomName();

                CreateAndVerifyEventWaitHandle(
                    initialState: true,
                    mode: EventResetMode.AutoReset,
                    prefixedName,
                    expectedSecurity: GetBasicEventWaitHandleSecurity(),
                    expectedCreatedNew: true).Dispose();
            });
        }

        // The documentation says MAX_PATH is the length limit for name, but it won't throw any errors:
        // https://docs.microsoft.com/en-us/windows/win32/api/synchapi/nf-synchapi-createeventexw
        // The .NET Core constructors for EventWaitHandle do not throw on name longer than MAX_LENGTH, so the extension method should match the behavior:
        // https://source.dot.net/#System.Private.CoreLib/shared/System/Threading/EventWaitHandle.Windows.cs,20
        // The .NET Framework constructor throws:
        // https://referencesource.microsoft.com/#mscorlib/system/threading/eventwaithandle.cs,59
        [Fact]
        public void EventWaitHandle_Create_BeyondMaxPathLength()
        {
            string name = new string('x', Interop.Kernel32.MAX_PATH + 100);
            var security = GetBasicEventWaitHandleSecurity();
            var mode = EventResetMode.AutoReset;

            if (PlatformDetection.IsFullFramework)
            {
                Assert.Throws<ArgumentException>(() =>
                {
                    CreateAndVerifyEventWaitHandle(
                        initialState: true,
                        mode,
                        name,
                        security,
                        expectedCreatedNew: true).Dispose();
                });
            }
            else
            {
                using EventWaitHandle created = CreateAndVerifyEventWaitHandle(
                        initialState: true,
                        mode,
                        name,
                        security,
                        expectedCreatedNew: true);

                using EventWaitHandle openedByName = EventWaitHandle.OpenExisting(name);
                Assert.NotNull(openedByName);
            }
        }

        [Theory]
        [InlineData((EventResetMode)int.MinValue)]
        [InlineData((EventResetMode)(-1))]
        [InlineData((EventResetMode)2)]
        [InlineData((EventResetMode)int.MaxValue)]
        public void EventWaitHandle_Create_InvalidMode(EventResetMode mode)
        {
            if (PlatformDetection.IsFullFramework)
            {
                Assert.Throws<ArgumentException>(() =>
                {
                    CreateAndVerifyEventWaitHandle(
                        initialState: true,
                        mode,
                        GetRandomName(),
                        GetBasicEventWaitHandleSecurity(),
                        expectedCreatedNew: true).Dispose();
                });
            }
            else
            {
                Assert.Throws<ArgumentOutOfRangeException>("mode", () =>
                {
                    CreateAndVerifyEventWaitHandle(
                        initialState: true,
                        mode,
                        GetRandomName(),
                        GetBasicEventWaitHandleSecurity(),
                        expectedCreatedNew: true).Dispose();
                });
            }
        }

        [Theory]
        [InlineData(true, EventResetMode.AutoReset,    EventWaitHandleRights.FullControl, AccessControlType.Allow)]
        [InlineData(true, EventResetMode.AutoReset,    EventWaitHandleRights.FullControl, AccessControlType.Deny)]
        [InlineData(true, EventResetMode.AutoReset,    EventWaitHandleRights.Synchronize, AccessControlType.Allow)]
        [InlineData(true, EventResetMode.AutoReset,    EventWaitHandleRights.Synchronize, AccessControlType.Deny)]
        [InlineData(true, EventResetMode.AutoReset,    EventWaitHandleRights.Modify,      AccessControlType.Allow)]
        [InlineData(true, EventResetMode.AutoReset,    EventWaitHandleRights.Modify,      AccessControlType.Deny)]
        [InlineData(true, EventResetMode.AutoReset,    EventWaitHandleRights.Modify | EventWaitHandleRights.Synchronize, AccessControlType.Allow)]
        [InlineData(true, EventResetMode.AutoReset,    EventWaitHandleRights.Modify | EventWaitHandleRights.Synchronize, AccessControlType.Deny)]
        [InlineData(true, EventResetMode.ManualReset,  EventWaitHandleRights.FullControl, AccessControlType.Allow)]
        [InlineData(true, EventResetMode.ManualReset,  EventWaitHandleRights.FullControl, AccessControlType.Deny)]
        [InlineData(true, EventResetMode.ManualReset,  EventWaitHandleRights.Synchronize, AccessControlType.Allow)]
        [InlineData(true, EventResetMode.ManualReset,  EventWaitHandleRights.Synchronize, AccessControlType.Deny)]
        [InlineData(true, EventResetMode.ManualReset,  EventWaitHandleRights.Modify,      AccessControlType.Allow)]
        [InlineData(true, EventResetMode.ManualReset,  EventWaitHandleRights.Modify,      AccessControlType.Deny)]
        [InlineData(true, EventResetMode.ManualReset,  EventWaitHandleRights.Modify | EventWaitHandleRights.Synchronize, AccessControlType.Allow)]
        [InlineData(true, EventResetMode.ManualReset,  EventWaitHandleRights.Modify | EventWaitHandleRights.Synchronize, AccessControlType.Deny)]
        [InlineData(false, EventResetMode.AutoReset,   EventWaitHandleRights.FullControl, AccessControlType.Allow)]
        [InlineData(false, EventResetMode.AutoReset,   EventWaitHandleRights.FullControl, AccessControlType.Deny)]
        [InlineData(false, EventResetMode.AutoReset,   EventWaitHandleRights.Synchronize, AccessControlType.Allow)]
        [InlineData(false, EventResetMode.AutoReset,   EventWaitHandleRights.Synchronize, AccessControlType.Deny)]
        [InlineData(false, EventResetMode.AutoReset,   EventWaitHandleRights.Modify,      AccessControlType.Allow)]
        [InlineData(false, EventResetMode.AutoReset,   EventWaitHandleRights.Modify,      AccessControlType.Deny)]
        [InlineData(false, EventResetMode.AutoReset,   EventWaitHandleRights.Modify | EventWaitHandleRights.Synchronize, AccessControlType.Allow)]
        [InlineData(false, EventResetMode.AutoReset,   EventWaitHandleRights.Modify | EventWaitHandleRights.Synchronize, AccessControlType.Deny)]
        [InlineData(false, EventResetMode.ManualReset, EventWaitHandleRights.FullControl, AccessControlType.Allow)]
        [InlineData(false, EventResetMode.ManualReset, EventWaitHandleRights.FullControl, AccessControlType.Deny)]
        [InlineData(false, EventResetMode.ManualReset, EventWaitHandleRights.Synchronize, AccessControlType.Allow)]
        [InlineData(false, EventResetMode.ManualReset, EventWaitHandleRights.Synchronize, AccessControlType.Deny)]
        [InlineData(false, EventResetMode.ManualReset, EventWaitHandleRights.Modify,      AccessControlType.Allow)]
        [InlineData(false, EventResetMode.ManualReset, EventWaitHandleRights.Modify,      AccessControlType.Deny)]
        [InlineData(false, EventResetMode.ManualReset, EventWaitHandleRights.Modify | EventWaitHandleRights.Synchronize, AccessControlType.Allow)]
        [InlineData(false, EventResetMode.ManualReset, EventWaitHandleRights.Modify | EventWaitHandleRights.Synchronize, AccessControlType.Deny)]
        public void EventWaitHandle_Create_SpecificParameters(bool initialState, EventResetMode mode, EventWaitHandleRights rights, AccessControlType accessControl)
        {
            var security = GetEventWaitHandleSecurity(WellKnownSidType.BuiltinUsersSid, rights, accessControl);
            CreateAndVerifyEventWaitHandle(
                initialState,
                mode,
                GetRandomName(),
                security,
                expectedCreatedNew: true).Dispose();

        }

        private EventWaitHandleSecurity GetBasicEventWaitHandleSecurity()
        {
            return GetEventWaitHandleSecurity(
                WellKnownSidType.BuiltinUsersSid,
                EventWaitHandleRights.FullControl,
                AccessControlType.Allow);
        }

        private EventWaitHandleSecurity GetEventWaitHandleSecurity(WellKnownSidType sid, EventWaitHandleRights rights, AccessControlType accessControl)
        {
            var security = new EventWaitHandleSecurity();
            SecurityIdentifier identity = new SecurityIdentifier(sid, null);
            var accessRule = new EventWaitHandleAccessRule(identity, rights, accessControl);
            security.AddAccessRule(accessRule);
            return security;
        }

        private EventWaitHandle CreateAndVerifyEventWaitHandle(bool initialState, EventResetMode mode, string name, EventWaitHandleSecurity expectedSecurity, bool expectedCreatedNew)
        {
            EventWaitHandle eventHandle = EventWaitHandleAcl.Create(initialState, mode, name, out bool createdNew, expectedSecurity);
            Assert.NotNull(eventHandle);
            Assert.Equal(expectedCreatedNew, createdNew);

            if (expectedSecurity != null)
            {
                EventWaitHandleSecurity actualSecurity = eventHandle.GetAccessControl();
                VerifyEventWaitHandleSecurity(expectedSecurity, actualSecurity);
            }

            return eventHandle;
        }

        private void VerifyEventWaitHandleSecurity(EventWaitHandleSecurity expectedSecurity, EventWaitHandleSecurity actualSecurity)
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

        private bool AreAccessRulesEqual(EventWaitHandleAccessRule expectedRule, EventWaitHandleAccessRule actualRule)
        {
            return
                expectedRule.AccessControlType     == actualRule.AccessControlType &&
                expectedRule.EventWaitHandleRights == actualRule.EventWaitHandleRights &&
                expectedRule.InheritanceFlags      == actualRule.InheritanceFlags &&
                expectedRule.PropagationFlags      == actualRule.PropagationFlags;
        }
    }
}
