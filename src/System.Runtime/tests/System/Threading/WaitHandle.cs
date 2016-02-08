// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading;

using Xunit;

namespace System.Runtime.Tests
{
    public static class WaitHandleTests
    {
        [Fact]
        public static void TestWaitOne()
        {
            var waitHandle = new ManualResetEvent(true);

            Assert.True(waitHandle.WaitOne());
            Assert.True(waitHandle.WaitOne(1));
            Assert.True(waitHandle.WaitOne(TimeSpan.FromMilliseconds(1)));

            waitHandle.Reset();

            Assert.False(waitHandle.WaitOne(1));
            Assert.False(waitHandle.WaitOne(TimeSpan.FromMilliseconds(1)));
        }

        [Fact]
        public static void TestWaitAny()
        {
            var waitHandles = new ManualResetEvent[] 
            {
                new ManualResetEvent(false),
                new ManualResetEvent(false),
                new ManualResetEvent(true)
            };

            Assert.Equal(2, WaitHandle.WaitAny(waitHandles));
            Assert.Equal(2, WaitHandle.WaitAny(waitHandles, 1));
            Assert.Equal(2, WaitHandle.WaitAny(waitHandles, TimeSpan.FromMilliseconds(1)));

            waitHandles[2].Reset();

            Assert.Equal(WaitHandle.WaitTimeout, WaitHandle.WaitAny(waitHandles, 1));
            Assert.Equal(WaitHandle.WaitTimeout, WaitHandle.WaitAny(waitHandles, TimeSpan.FromMilliseconds(1)));
        }

        [Fact]
        public static void TestWaitAnySameHandles()
        {
            var waitHandles = new ManualResetEvent[2];
            waitHandles[0] = new ManualResetEvent(true);
            waitHandles[1] = waitHandles[0];

            Assert.Equal(0, WaitHandle.WaitAny(waitHandles));
        }

        [Fact]
        public static void TestWaitAll()
        {
            var waitHandles = new ManualResetEvent[] 
            {
                new ManualResetEvent(true),
                new ManualResetEvent(true),
                new ManualResetEvent(true)
            };

            Assert.True(WaitHandle.WaitAll(waitHandles));
            Assert.True(WaitHandle.WaitAll(waitHandles, 1));
            Assert.True(WaitHandle.WaitAll(waitHandles, TimeSpan.FromMilliseconds(1)));

            waitHandles[2].Reset();

            Assert.False(WaitHandle.WaitAll(waitHandles, 1));
            Assert.False(WaitHandle.WaitAll(waitHandles, TimeSpan.FromMilliseconds(1)));
        }

        [Fact]
        public static void TestWaitAllSameHandles()
        {
            var waitHandles = new ManualResetEvent[2];
            waitHandles[0] = new ManualResetEvent(true);
            waitHandles[1] = waitHandles[0];

            Assert.ThrowsAny<ArgumentException>(() => WaitHandle.WaitAll(waitHandles));
        }

        [Fact]
        [PlatformSpecific(PlatformID.Windows)] // Names aren't supported on Unix
        public static void TestWaitAllSameNames()
        {
            var waitHandles = new Mutex[2];
            waitHandles[0] = new Mutex(false, "test");
            waitHandles[1] = new Mutex(false, "test");

            Assert.Throws<ArgumentException>(null, () => WaitHandle.WaitAll(waitHandles));
        }

        [Fact]
        public static void TestWaitTimeout()
        {
            Assert.Equal(WaitHandle.WaitTimeout, WaitHandle.WaitAny(new WaitHandle[] { new ManualResetEvent(false) }, 0));
        }
    }
}
