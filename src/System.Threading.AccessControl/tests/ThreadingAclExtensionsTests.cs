// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Security.AccessControl;
using System.Transactions;
using Xunit;

namespace System.Threading.Tests
{
    public static class ThreadingAclExtensionsTests
    {
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
        [PlatformSpecific(TestPlatforms.Windows)]
        public static void EventWaitHandle_Create_InvalidName()
        {
            AssertExtensions.Throws<ArgumentException>("name", () =>
            {
                using EventWaitHandle eventHandle = EventWaitHandleAcl.Create(initialState: true, EventResetMode.AutoReset, null, out bool createdNew, new EventWaitHandleSecurity());
            });

            AssertExtensions.Throws<ArgumentException>("name", () =>
            {
                using EventWaitHandle eventHandle = EventWaitHandleAcl.Create(initialState: true, EventResetMode.AutoReset, string.Empty, out bool createdNew, new EventWaitHandleSecurity());
            });

            AssertExtensions.Throws<ArgumentException>("name", () =>
            {
                string name = new string('X', Interop.Kernel32.MAX_PATH + 1);
                using EventWaitHandle eventHandle = EventWaitHandleAcl.Create(initialState: true, EventResetMode.AutoReset, name, out bool createdNew, new EventWaitHandleSecurity());
            });
        }

        [Fact]
        [PlatformSpecific(TestPlatforms.Windows)]
        public static void EventWaitHandle_Create_InvalidMode()
        {
            AssertExtensions.Throws<ArgumentOutOfRangeException>("mode", () =>
            {
                using EventWaitHandle eventHandle = EventWaitHandleAcl.Create(initialState: true, (EventResetMode)(-1), "name", out bool createdNew, new EventWaitHandleSecurity());
            });

            AssertExtensions.Throws<ArgumentOutOfRangeException>("mode", () =>
            {
                using EventWaitHandle eventHandle = EventWaitHandleAcl.Create(initialState: true, (EventResetMode)2, "name", out bool createdNew, new EventWaitHandleSecurity());
            });
        }

        [Fact]
        [PlatformSpecific(TestPlatforms.Windows)]
        public static void EventWaitHandle_Create_DefaultSecurity()
        {
            using EventWaitHandle eventHandle = EventWaitHandleAcl.Create(initialState: true, EventResetMode.AutoReset, "WhyIsThisUnauthorized", out bool createdNew, new EventWaitHandleSecurity());
            Assert.NotNull(eventHandle);
        }

        #endregion
    }
}
