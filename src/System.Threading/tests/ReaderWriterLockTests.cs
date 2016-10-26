// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using Xunit;

namespace System.Threading.Tests
{
    public static class ReaderWriterLockTests
    {
        private const int UnexpectedTimeoutMilliseconds = ThreadTestHelpers.UnexpectedTimeoutMilliseconds;

        private const int TimeoutExceptionHResult = unchecked((int)0x800705B4); // ERROR_TIMEOUT
        private const int NotOwnerExceptionHResult = unchecked((int)0x80070120); // ERROR_NOT_OWNER
        private const int InvalidLockCookieExceptionHResult = unchecked((int)0x80070057); // E_INVALIDARG

        [Fact]
        public static void InvalidTimeoutTest()
        {
            var rwl = new ReaderWriterLock();
            Assert.Throws<ArgumentOutOfRangeException>(() => rwl.AcquireReaderLock(-2));
            Assert.Throws<ArgumentOutOfRangeException>(() => rwl.AcquireReaderLock(TimeSpan.FromMilliseconds(-2)));
            Assert.Throws<ArgumentOutOfRangeException>(
                () => rwl.AcquireReaderLock(TimeSpan.FromMilliseconds((uint)int.MaxValue + 1)));

            Assert.Throws<ArgumentOutOfRangeException>(() => rwl.AcquireWriterLock(-2));
            Assert.Throws<ArgumentOutOfRangeException>(() => rwl.AcquireWriterLock(TimeSpan.FromMilliseconds(-2)));
            Assert.Throws<ArgumentOutOfRangeException>(
                () => rwl.AcquireWriterLock(TimeSpan.FromMilliseconds((uint)int.MaxValue + 1)));

            Assert.Throws<ArgumentOutOfRangeException>(() => rwl.UpgradeToWriterLock(-2));
            Assert.Throws<ArgumentOutOfRangeException>(() => rwl.UpgradeToWriterLock(TimeSpan.FromMilliseconds(-2)));
            Assert.Throws<ArgumentOutOfRangeException>(
                () => rwl.UpgradeToWriterLock(TimeSpan.FromMilliseconds((uint)int.MaxValue + 1)));
        }

        [Fact]
        public static void NotOwnerTest()
        {
            var trwl = new TestReaderWriterLock();
            trwl.ReleaseReaderLock(NotOwnerExceptionHResult);
            trwl.ReleaseWriterLock(NotOwnerExceptionHResult);
            trwl.DowngradeFromWriterLock(new TestLockCookie(), NotOwnerExceptionHResult);

            {
                trwl.AcquireReaderLock();

                trwl.ReleaseWriterLock(NotOwnerExceptionHResult);

                TestLockCookie tlc = trwl.UpgradeToWriterLock();
                TestLockCookie tlc2 = tlc.Clone();
                trwl.DowngradeFromWriterLock(tlc); // tlc is invalid, tlc2 is valid
                trwl.DowngradeFromWriterLock(tlc2, NotOwnerExceptionHResult);

                trwl.ReleaseReaderLock();
            }

            trwl.Dispose();
        }

        [Fact]
        public static void ShouldNotBeOwnerForRestoreLockTest()
        {
            var trwl = new TestReaderWriterLock();

            trwl.AcquireReaderLock();
            TestLockCookie restoreReadLockTlc = trwl.ReleaseLock();
            trwl.AcquireWriterLock();
            TestLockCookie restoreWriteLockTlc = trwl.ReleaseLock();

            Action verifyCannotRestore =
                () =>
                {
                    Assert.Throws<SynchronizationLockException>(() => trwl.RestoreLock(restoreReadLockTlc));
                    Assert.Throws<SynchronizationLockException>(() => trwl.RestoreLock(restoreWriteLockTlc));
                };

            trwl.AcquireReaderLock();
            verifyCannotRestore();
            trwl.ReleaseReaderLock();

            trwl.AcquireWriterLock();
            verifyCannotRestore();
            trwl.ReleaseWriterLock();

            trwl.Dispose();
        }

        [Fact]
        public static void InvalidLockCookieTest()
        {
            // Invalid lock cookie created by using one up with Upgrade/Downgrade
            var trwl = new TestReaderWriterLock();
            TestLockCookie tlc = trwl.UpgradeToWriterLock();
            trwl.DowngradeFromWriterLock(tlc);
            trwl.AcquireWriterLock();
            trwl.DowngradeFromWriterLock(tlc, InvalidLockCookieExceptionHResult);
            trwl.ReleaseWriterLock();
            trwl.RestoreLock(tlc, InvalidLockCookieExceptionHResult);

            // Invalid lock cookie created by using one up with Release/Restore
            tlc = trwl.ReleaseLock();
            trwl.RestoreLock(tlc);
            trwl.AcquireWriterLock();
            trwl.DowngradeFromWriterLock(tlc, InvalidLockCookieExceptionHResult);
            trwl.ReleaseWriterLock();
            trwl.RestoreLock(tlc, InvalidLockCookieExceptionHResult);

            // Lock cookie owned by another thread
            ThreadTestHelpers.RunTestInBackgroundThread(() =>
            {
                TestLockCookie tlc2 = trwl.UpgradeToWriterLock();
                tlc = tlc2.Clone();
                trwl.DowngradeFromWriterLock(tlc2);
            });
            trwl.AcquireWriterLock();
            trwl.DowngradeFromWriterLock(tlc, InvalidLockCookieExceptionHResult);
            trwl.ReleaseWriterLock();
            trwl.RestoreLock(tlc, InvalidLockCookieExceptionHResult);

            trwl.Dispose();
        }

        [Fact]
        public static void BasicLockTest()
        {
            var trwl = new TestReaderWriterLock();
            TestLockCookie tlc;
            var threadReady = new AutoResetEvent(false);
            var continueThread = new AutoResetEvent(false);
            Action checkForThreadErrors, waitForThread;
            Thread t =
                ThreadTestHelpers.CreateGuardedThread(out checkForThreadErrors, out waitForThread, () =>
                {
                    TestLockCookie tlc2;
                    Action switchToMainThread =
                        () =>
                        {
                            threadReady.Set();
                            continueThread.WaitOne();
                        };

                    switchToMainThread();

                    // Multiple readers from multiple threads
                    {
                        trwl.AcquireReaderLock();
                        trwl.AcquireReaderLock();
                        switchToMainThread();
                        trwl.ReleaseReaderLock();
                        switchToMainThread();
                        trwl.ReleaseReaderLock();
                        switchToMainThread();

                        trwl.AcquireReaderLock();
                        trwl.ReleaseReaderLock();
                        switchToMainThread();
                    }

                    // What can be done when a read lock is held
                    {
                        trwl.AcquireReaderLock();
                        switchToMainThread();

                        // Any thread can take a read lock
                        trwl.AcquireReaderLock();
                        trwl.ReleaseReaderLock();
                        switchToMainThread();

                        // No thread can take a write lock
                        trwl.AcquireWriterLock(TimeoutExceptionHResult);
                        trwl.AcquireReaderLock();
                        trwl.UpgradeToWriterLock(TimeoutExceptionHResult);
                        trwl.ReleaseReaderLock();
                        switchToMainThread();

                        trwl.ReleaseReaderLock();
                        switchToMainThread();

                        // Owning thread cannot upgrade when there is a waiting writer
                        trwl.AcquireWriterLock();
                        trwl.ReleaseWriterLock();
                        switchToMainThread();

                        // Owning thread cannot upgrade if there are other readers or writers
                        trwl.AcquireReaderLock();
                        switchToMainThread();
                        trwl.ReleaseReaderLock();
                        trwl.AcquireWriterLock();
                        switchToMainThread();
                        trwl.ReleaseWriterLock();
                        switchToMainThread();
                    }

                    // What can be done when a write lock is held
                    {
                        // Write lock acquired through AcquireWriteLock is exclusive
                        trwl.AcquireWriterLock();
                        switchToMainThread();
                        trwl.ReleaseWriterLock();
                        switchToMainThread();

                        // Write lock acquired through upgrading is also exclusive
                        trwl.AcquireReaderLock();
                        tlc2 = trwl.UpgradeToWriterLock();
                        switchToMainThread();
                        trwl.DowngradeFromWriterLock(tlc2);
                        trwl.ReleaseReaderLock();
                        switchToMainThread();

                        // Write lock acquired through restore is also exclusive
                        trwl.AcquireWriterLock();
                        tlc = trwl.ReleaseLock();
                        trwl.RestoreLock(tlc);
                        switchToMainThread();
                        trwl.ReleaseWriterLock();
                        switchToMainThread();
                    }
                });
            t.IsBackground = true;
            t.Start();

            Action beginSwitchToBackgroundThread = () => continueThread.Set();
            Action endSwitchToBackgroundThread =
                () =>
                {
                    try
                    {
                        threadReady.CheckedWait();
                    }
                    finally
                    {
                        checkForThreadErrors();
                    }
                };
            Action switchToBackgroundThread =
                () =>
                {
                    beginSwitchToBackgroundThread();
                    endSwitchToBackgroundThread();
                };
            endSwitchToBackgroundThread();

            // Multiple readers from muliple threads
            {
                trwl.AcquireReaderLock();
                trwl.AcquireReaderLock();
                switchToBackgroundThread(); // AcquireReaderLock * 2
                trwl.ReleaseReaderLock();
                switchToBackgroundThread(); // ReleaseReaderLock

                // Release/restore the read lock while a read lock is held by another thread
                tlc = trwl.ReleaseLock();
                trwl.RestoreLock(tlc);

                switchToBackgroundThread(); // ReleaseReaderLock

                // Downgrade to read lock allows another thread to acquire read lock
                tlc = trwl.UpgradeToWriterLock();
                trwl.DowngradeFromWriterLock(tlc);
                switchToBackgroundThread(); // AcquireReaderLock, ReleaseReaderLock

                trwl.ReleaseReaderLock();
            }

            // What can be done when a read lock is held
            {
                switchToBackgroundThread(); // AcquireReaderLock
                {
                    // Any thread can take a read lock
                    trwl.AcquireReaderLock();
                    trwl.ReleaseReaderLock();
                    switchToBackgroundThread(); // same as above

                    // No thread can take a write lock
                    trwl.AcquireWriterLock(TimeoutExceptionHResult);
                    trwl.AcquireReaderLock();
                    trwl.UpgradeToWriterLock(TimeoutExceptionHResult);
                    switchToBackgroundThread(); // same as above
                    trwl.ReleaseReaderLock();

                    // Other threads cannot upgrade to a write lock, but the owning thread can
                    trwl.AcquireReaderLock();
                    trwl.UpgradeToWriterLock(TimeoutExceptionHResult);
                    trwl.ReleaseReaderLock();
                }
                switchToBackgroundThread(); // ReleaseReaderLock

                // Owning thread releases read lock when upgrading
                trwl.AcquireReaderLock();
                beginSwitchToBackgroundThread(); // AcquireWriterLock: background thread gets blocked
                trwl.UpgradeToWriterLock(); // unblocks background thread: ReleaseWriterLock
                trwl.ReleaseWriterLock();
                endSwitchToBackgroundThread();

                // Owning thread cannot upgrade if there are other readers or writers
                trwl.AcquireReaderLock();
                switchToBackgroundThread(); // AcquireReaderLock
                trwl.UpgradeToWriterLock(TimeoutExceptionHResult);
                trwl.ReleaseReaderLock();
                switchToBackgroundThread(); // ReleaseReaderLock, AcquireWriterLock
                trwl.UpgradeToWriterLock(TimeoutExceptionHResult);
                switchToBackgroundThread(); // ReleaseWriterLock
            }

            // What can be done when a write lock is held
            {
                trwl.AcquireWriterLock();
                TestLockCookie restoreToWriteLockTlc = trwl.ReleaseLock();
                Action verifyCannotAcquireLock =
                    () =>
                    {
                        trwl.AcquireReaderLock(TimeoutExceptionHResult);
                        trwl.AcquireWriterLock(TimeoutExceptionHResult);
                        trwl.UpgradeToWriterLock(TimeoutExceptionHResult);
                    };
                Action verifyCanAcquireLock =
                    () =>
                    {
                        trwl.AcquireReaderLock();
                        tlc = trwl.UpgradeToWriterLock();
                        trwl.DowngradeFromWriterLock(tlc);
                        trwl.ReleaseReaderLock();
                        trwl.AcquireWriterLock();
                        trwl.ReleaseWriterLock();
                        trwl.RestoreLock(restoreToWriteLockTlc.Clone());
                        trwl.ReleaseWriterLock();
                    };

                // Write lock acquired through AcquireWriteLock is exclusive
                switchToBackgroundThread(); // AcquireWriterLock
                verifyCannotAcquireLock();
                switchToBackgroundThread(); // ReleaseWriterLock
                verifyCanAcquireLock();

                // Write lock acquired through upgrading is also exclusive
                switchToBackgroundThread(); // AcquireReaderLock, UpgradeToWriterLock
                verifyCannotAcquireLock();
                switchToBackgroundThread(); // DowngradeFromWriterLock, ReleaseReaderLock
                verifyCanAcquireLock();

                // Write lock acquired through restore is also exclusive
                switchToBackgroundThread(); // AcquireWriterLock, ReleaseLock, RestoreLock
                verifyCannotAcquireLock();
                switchToBackgroundThread(); // ReleaseWriterLock
                verifyCanAcquireLock();
            }

            beginSwitchToBackgroundThread();
            waitForThread();
            trwl.Dispose();
        }

        [Fact]
        public static void SingleThreadLockOwnerMiscellaneousTest()
        {
            var trwl = new TestReaderWriterLock();
            TestLockCookie tlc, tlc2;

            // Read lock owner can upgrade to a write lock
            trwl.AcquireReaderLock();
            tlc = trwl.UpgradeToWriterLock();
            trwl.DowngradeFromWriterLock(tlc);
            trwl.ReleaseReaderLock();

            // Can take a recursive write lock in multiple ways
            trwl.AcquireWriterLock();
            trwl.AcquireWriterLock();
            trwl.ReleaseWriterLock();
            trwl.AcquireReaderLock();
            trwl.ReleaseWriterLock();
            trwl.UpgradeToWriterLock();
            trwl.ReleaseWriterLock();
            trwl.ReleaseWriterLock();

            // Typical upgrade with single read lock
            trwl.AcquireReaderLock();
            tlc = trwl.UpgradeToWriterLock();
            trwl.DowngradeFromWriterLock(tlc);
            trwl.ReleaseReaderLock();

            // Write lock can be taken with UpgradeToWriterLock when no read lock is held, and with that lock cookie,
            // DowngradeFromWriterLock does not acquire a read lock
            tlc = trwl.UpgradeToWriterLock();
            tlc2 = trwl.UpgradeToWriterLock();
            trwl.DowngradeFromWriterLock(tlc2);
            trwl.DowngradeFromWriterLock(tlc);

            // Upgrading from recursive read lock downgrades back to recursive read lock
            trwl.AcquireReaderLock();
            trwl.AcquireReaderLock();
            tlc = trwl.UpgradeToWriterLock();
            trwl.DowngradeFromWriterLock(tlc);
            trwl.ReleaseReaderLock();
            trwl.ReleaseReaderLock();

            // Can downgrade from any write lock level, and to any read lock level with lock cookie from ReleaseLock
            trwl.AcquireReaderLock();
            trwl.AcquireReaderLock();
            tlc = trwl.ReleaseLock();
            trwl.AcquireWriterLock();
            trwl.AcquireWriterLock();
            trwl.DowngradeFromWriterLock(tlc);
            trwl.ReleaseReaderLock();
            trwl.ReleaseReaderLock();

            // Typical release/restore
            {
                trwl.AcquireReaderLock();
                trwl.AcquireReaderLock();
                tlc = trwl.ReleaseLock();
                trwl.RestoreLock(tlc);
                trwl.ReleaseReaderLock();
                trwl.ReleaseReaderLock();

                trwl.AcquireWriterLock();
                trwl.AcquireWriterLock();
                tlc = trwl.ReleaseLock();
                trwl.RestoreLock(tlc);
                trwl.ReleaseWriterLock();
                trwl.ReleaseWriterLock();
            }

            // Can restore to any read lock level with lock cookie from UpgradeToWriterLock
            trwl.AcquireReaderLock();
            trwl.AcquireReaderLock();
            tlc = trwl.UpgradeToWriterLock();
            trwl.ReleaseWriterLock();
            trwl.RestoreLock(tlc);
            trwl.ReleaseReaderLock();
            trwl.ReleaseReaderLock();

            trwl.Dispose();
        }

        [Fact]
        [SkipOnTargetFramework(TargetFrameworkMonikers.NetFramework)]
        public static void DowngradeQuirks_ChangedInDotNetCore()
        {
            var trwl = new TestReaderWriterLock();
            TestLockCookie tlc;

            // Downgrade quirk when downgrading from a state where multiple recursive write locks are held, when the lock cookie
            // was obtained from a state where:
            //   - No lock was held
            //   - When any number of recursive write locks are held
            // The expectation in both cases is that a downgrade respects the lock cookie and restores the write lock recursion
            // level to the point indicated by the lock cookie.
            {
                // Lock cookie obtained when no lock is held
                tlc = trwl.UpgradeToWriterLock();
                trwl.AcquireWriterLock();
                trwl.DowngradeFromWriterLock(tlc);
                Assert.False(trwl.IsReaderLockHeld);
                Assert.False(trwl.IsWriterLockHeld);

                // Lock cookie obtained when write locks are held
                trwl.AcquireWriterLock();
                tlc = trwl.UpgradeToWriterLock();
                trwl.AcquireWriterLock();
                trwl.DowngradeFromWriterLock(tlc);
                Assert.False(trwl.IsReaderLockHeld);
                Assert.True(trwl.IsWriterLockHeld);
                trwl.ReleaseWriterLock();
                Assert.False(trwl.IsWriterLockHeld);
            }

            // Cannot downgrade to a recursive write lock level greater than or equal to the current
            trwl.AcquireWriterLock(); 
            trwl.AcquireWriterLock();
            tlc = trwl.UpgradeToWriterLock();
            trwl.ReleaseWriterLock();
            trwl.DowngradeFromWriterLock(tlc, InvalidLockCookieExceptionHResult);
            trwl.ReleaseWriterLock();
            trwl.DowngradeFromWriterLock(tlc, InvalidLockCookieExceptionHResult);
            trwl.ReleaseWriterLock();
            Assert.False(trwl.IsReaderLockHeld);
            Assert.False(trwl.IsWriterLockHeld);

            trwl.Dispose();
        }

        [Fact]
        public static void WaitingReadersTest()
        {
            var trwl = new TestReaderWriterLock();
            trwl.AcquireWriterLock(0);

            Action acquireReleaseReaderLock =
                () =>
                {
                    trwl.AcquireReaderLock();
                    trwl.ReleaseReaderLock();
                };
            Action waitForWaitingReader1, waitForWaitingReader2;
            Thread waitingReader1 = ThreadTestHelpers.CreateGuardedThread(out waitForWaitingReader1, acquireReleaseReaderLock);
            Thread waitingReader2 = ThreadTestHelpers.CreateGuardedThread(out waitForWaitingReader2, acquireReleaseReaderLock);
            waitingReader1.IsBackground = true;
            waitingReader2.IsBackground = true;
            waitingReader1.Start();
            waitingReader2.Start();
            ThreadTestHelpers.WaitForCondition(() => (waitingReader1.ThreadState & ThreadState.WaitSleepJoin) != 0);
            ThreadTestHelpers.WaitForCondition(() => (waitingReader2.ThreadState & ThreadState.WaitSleepJoin) != 0);

            // Releasing the write lock releases all waiting readers
            trwl.ReleaseWriterLock();
            waitForWaitingReader1();
            waitForWaitingReader2();

            trwl.Dispose();
        }

        [Fact]
        public static void WaitingWritersTest()
        {
            var trwl = new TestReaderWriterLock();
            trwl.AcquireReaderLock(0);

            Action acquireReleaseWriterLock =
                () =>
                {
                    trwl.AcquireWriterLock();
                    trwl.ReleaseWriterLock();
                };
            Action waitForWaitingWriter1, waitForWaitingWriter2;
            Thread waitingWriter1 = ThreadTestHelpers.CreateGuardedThread(out waitForWaitingWriter1, acquireReleaseWriterLock);
            Thread waitingWriter2 = ThreadTestHelpers.CreateGuardedThread(out waitForWaitingWriter2, acquireReleaseWriterLock);
            waitingWriter1.IsBackground = true;
            waitingWriter2.IsBackground = true;
            waitingWriter1.Start();
            waitingWriter2.Start();
            ThreadTestHelpers.WaitForCondition(() => (waitingWriter1.ThreadState & ThreadState.WaitSleepJoin) != 0);
            ThreadTestHelpers.WaitForCondition(() => (waitingWriter2.ThreadState & ThreadState.WaitSleepJoin) != 0);

            // Releasing the read lock releases a waiting writer, that writer releases its write lock, in turn releasing the
            // other writer
            trwl.ReleaseReaderLock();
            waitForWaitingWriter1();
            waitForWaitingWriter2();

            trwl.Dispose();
        }

        [Fact]
        public static void ReadersWaitingOnWaitingWriterTest()
        {
            var trwl = new TestReaderWriterLock();
            trwl.AcquireReaderLock();

            var waitingWriterEvent = new AutoResetEvent(false);
            Action waitForWaitingWriter;
            Thread waitingWriter =
                ThreadTestHelpers.CreateGuardedThread(out waitForWaitingWriter, () =>
                {
                    trwl.AcquireWriterLock();
                    waitingWriterEvent.Set();
                    waitingWriterEvent.CheckedWait();
                    trwl.ReleaseWriterLock();
                });
            waitingWriter.IsBackground = true;
            waitingWriter.Start();
            ThreadTestHelpers.WaitForCondition(() => (waitingWriter.ThreadState & ThreadState.WaitSleepJoin) != 0);

            Action acquireReleaseReaderLock =
                () =>
                {
                    trwl.AcquireReaderLock();
                    trwl.ReleaseReaderLock();
                };
            Action waitForWaitingReader1, waitForWaitingReader2;
            Thread waitingReader1 = ThreadTestHelpers.CreateGuardedThread(out waitForWaitingReader1, acquireReleaseReaderLock);
            Thread waitingReader2 = ThreadTestHelpers.CreateGuardedThread(out waitForWaitingReader2, acquireReleaseReaderLock);
            waitingReader1.IsBackground = true;
            waitingReader2.IsBackground = true;
            waitingReader1.Start();
            waitingReader2.Start();
            ThreadTestHelpers.WaitForCondition(() => (waitingReader1.ThreadState & ThreadState.WaitSleepJoin) != 0);
            ThreadTestHelpers.WaitForCondition(() => (waitingReader2.ThreadState & ThreadState.WaitSleepJoin) != 0);

            // Releasing the read lock releases the waiting writer
            trwl.ReleaseReaderLock();
            waitingWriterEvent.CheckedWait();
            Assert.NotEqual((ThreadState)0, waitingReader1.ThreadState & ThreadState.WaitSleepJoin);
            Assert.NotEqual((ThreadState)0, waitingReader2.ThreadState & ThreadState.WaitSleepJoin);

            // Releasing the now-writer's write lock releases all waiting readers
            waitingWriterEvent.Set();
            waitForWaitingWriter();
            waitForWaitingReader1();
            waitForWaitingReader2();

            trwl.Dispose();
        }

        [Fact]
        public static void ReadersWaitingOnWaitingUpgraderTest()
        {
            var trwl = new TestReaderWriterLock();
            trwl.AcquireReaderLock();

            var waitingUpgraderEvent = new AutoResetEvent(false);
            Action waitForWaitingUpgrader;
            Thread waitingUpgrader =
                ThreadTestHelpers.CreateGuardedThread(out waitForWaitingUpgrader, () =>
                {
                    trwl.AcquireReaderLock(0);
                    trwl.UpgradeToWriterLock();
                    waitingUpgraderEvent.Set();
                    waitingUpgraderEvent.CheckedWait();
                    trwl.ReleaseWriterLock();
                    Assert.False(trwl.IsReaderLockHeld);
                    Assert.False(trwl.IsWriterLockHeld);
                });
            waitingUpgrader.IsBackground = true;
            waitingUpgrader.Start();
            ThreadTestHelpers.WaitForCondition(() => (waitingUpgrader.ThreadState & ThreadState.WaitSleepJoin) != 0);

            Action acquireReleaseReaderLock =
                () =>
                {
                    trwl.AcquireReaderLock();
                    trwl.ReleaseReaderLock();
                };
            Action waitForWaitingReader1, waitForWaitingReader2;
            Thread waitingReader1 = ThreadTestHelpers.CreateGuardedThread(out waitForWaitingReader1, acquireReleaseReaderLock);
            Thread waitingReader2 = ThreadTestHelpers.CreateGuardedThread(out waitForWaitingReader2, acquireReleaseReaderLock);
            waitingReader1.IsBackground = true;
            waitingReader2.IsBackground = true;
            waitingReader1.Start();
            waitingReader2.Start();
            ThreadTestHelpers.WaitForCondition(() => (waitingReader1.ThreadState & ThreadState.WaitSleepJoin) != 0);
            ThreadTestHelpers.WaitForCondition(() => (waitingReader2.ThreadState & ThreadState.WaitSleepJoin) != 0);

            // Releasing the read lock releases the waiting upgrader
            trwl.ReleaseReaderLock();
            waitingUpgraderEvent.CheckedWait();
            Assert.NotEqual((ThreadState)0, waitingReader1.ThreadState & ThreadState.WaitSleepJoin);
            Assert.NotEqual((ThreadState)0, waitingReader2.ThreadState & ThreadState.WaitSleepJoin);

            // Releasing the now-writer's write lock releases all waiting readers
            waitingUpgraderEvent.Set();
            waitForWaitingUpgrader();
            waitForWaitingReader1();
            waitForWaitingReader2();

            trwl.Dispose();
        }

        [Fact]
        public static void WaitingUpgradersTest()
        {
            var trwl = new TestReaderWriterLock();
            trwl.AcquireReaderLock();

            var waitingUpgrader1AcquiredReadLock = new ManualResetEvent(false);
            Action waitForWaitingUpgrader1, waitForWaitingUpgrader2, waitForWaitingUpgrader3;
            Thread waitingUpgrader1 =
                ThreadTestHelpers.CreateGuardedThread(out waitForWaitingUpgrader1, () =>
                {
                    trwl.AcquireReaderLock();
                    waitingUpgrader1AcquiredReadLock.Set();
                    TestLockCookie tlc = trwl.UpgradeToWriterLock();
                    trwl.DowngradeFromWriterLock(tlc);
                    trwl.ReleaseReaderLock();
                });
            Action upgradeDowngradeLock =
                () =>
                {
                    TestLockCookie tlc = trwl.UpgradeToWriterLock();
                    trwl.DowngradeFromWriterLock(tlc);
                };
            Thread waitingUpgrader2 = ThreadTestHelpers.CreateGuardedThread(out waitForWaitingUpgrader2, upgradeDowngradeLock);
            Thread waitingUpgrader3 = ThreadTestHelpers.CreateGuardedThread(out waitForWaitingUpgrader3, upgradeDowngradeLock);
            waitingUpgrader1.IsBackground = true;
            waitingUpgrader2.IsBackground = true;
            waitingUpgrader1.Start();
            waitingUpgrader1AcquiredReadLock.CheckedWait();
            waitingUpgrader2.Start();
            waitingUpgrader3.Start();
            ThreadTestHelpers.WaitForCondition(() => (waitingUpgrader1.ThreadState & ThreadState.WaitSleepJoin) != 0);
            ThreadTestHelpers.WaitForCondition(() => (waitingUpgrader2.ThreadState & ThreadState.WaitSleepJoin) != 0);
            ThreadTestHelpers.WaitForCondition(() => (waitingUpgrader3.ThreadState & ThreadState.WaitSleepJoin) != 0);

            // Releasing the read lock releases a waiting upgrader, that writer downgrades its write lock, in turn releasing the
            // other upgrader, and so on
            trwl.ReleaseReaderLock();
            waitForWaitingUpgrader1();
            waitForWaitingUpgrader2();
            waitForWaitingUpgrader3();

            trwl.Dispose();
        }

        [Fact]
        public static void AtomicRecursiveReaderTest()
        {
            var trwl = new TestReaderWriterLock();
            trwl.AcquireReaderLock();

            Action waitForWaitingWriter;
            Thread waitingWriter =
                ThreadTestHelpers.CreateGuardedThread(out waitForWaitingWriter, () =>
                {
                    trwl.AcquireWriterLock();
                    trwl.ReleaseWriterLock();
                });
            waitingWriter.Start();
            ThreadTestHelpers.WaitForCondition(() => (waitingWriter.ThreadState & ThreadState.WaitSleepJoin) != 0);

            // Acquire a recursive read lock successfully while there is a waiting writer
            trwl.AcquireReaderLock(0);
            Assert.NotEqual((ThreadState)0, waitingWriter.ThreadState & ThreadState.WaitSleepJoin);

            // Releasing both read locks releases the waiting writer
            trwl.ReleaseLock();
            waitForWaitingWriter();

            trwl.Dispose();
        }

        [Fact]
        public static void AtomicDowngradeTest()
        {
            var trwl = new TestReaderWriterLock();
            trwl.AcquireReaderLock();
            TestLockCookie tlc = trwl.UpgradeToWriterLock();

            Action waitForWaitingWriter;
            Thread waitingWriter =
                ThreadTestHelpers.CreateGuardedThread(out waitForWaitingWriter, () =>
                {
                    trwl.AcquireWriterLock();
                    trwl.ReleaseWriterLock();
                });
            waitingWriter.Start();
            ThreadTestHelpers.WaitForCondition(() => (waitingWriter.ThreadState & ThreadState.WaitSleepJoin) != 0);

            // Downgrade to a read lock successfully while there is a waiting writer
            trwl.DowngradeFromWriterLock(tlc);
            Assert.NotEqual((ThreadState)0, waitingWriter.ThreadState & ThreadState.WaitSleepJoin);

            // Releasing the read lock releases the waiting writer
            trwl.ReleaseReaderLock();
            waitForWaitingWriter();

            trwl.Dispose();
        }

        private class TestReaderWriterLock : IDisposable
        {
            private const int InvalidThreadID = -1;

            [ThreadStatic]
            private static Dictionary<TestReaderWriterLock, int> t_readerLevels;

            private readonly ReaderWriterLock _rwl;
            private int _writerThreadID = InvalidThreadID;
            private int _writerLevel;
            private int _writerSeqNum;

            // When there are pending changes, the state of the ReaderWriterLock may be manipulated in parallel
            // nondeterministically, so only verify the state once all pending state changes have been made and are reflected by
            // the state of this instance.
            private int _pendingStateChanges;

            public TestReaderWriterLock()
            {
                _rwl = new ReaderWriterLock();
                VerifyState();
            }

            public void Dispose()
            {
                GC.SuppressFinalize(this);
                Assert.False(RemoveFromThreadReaderLevels());
                Assert.Equal(InvalidThreadID, _writerThreadID);
                Assert.Equal(0, _writerLevel);
            }

            private Dictionary<TestReaderWriterLock, int> EnsureThreadReaderLevels()
            {
                Dictionary<TestReaderWriterLock, int> threadReaderLevels = t_readerLevels;
                if (threadReaderLevels == null)
                {
                    t_readerLevels = threadReaderLevels = new Dictionary<TestReaderWriterLock, int>();
                }
                return threadReaderLevels;
            }

            private bool RemoveFromThreadReaderLevels()
            {
                Dictionary<TestReaderWriterLock, int> threadReaderLevels = t_readerLevels;
                if (threadReaderLevels == null)
                {
                    return false;
                }

                bool removed = threadReaderLevels.Remove(this);
                if (threadReaderLevels.Count == 0)
                {
                    t_readerLevels = null;
                }
                return removed;
            }

            private int ThreadReaderLevel
            {
                get
                {
                    Dictionary<TestReaderWriterLock, int> threadReaderLevels = t_readerLevels;
                    if (threadReaderLevels == null)
                    {
                        return 0;
                    }

                    int readerLevel;
                    if (!threadReaderLevels.TryGetValue(this, out readerLevel))
                    {
                        return 0;
                    }

                    Assert.NotEqual(0, readerLevel);
                    return readerLevel;
                }
                set
                {
                    if (value == 0)
                    {
                        RemoveFromThreadReaderLevels();
                    }
                    else
                    {
                        EnsureThreadReaderLevels()[this] = value;
                    }
                }
            }

            public bool IsReaderLockHeld
            {
                get
                {
                    bool rwlIsReaderLockHeld;
                    lock (_rwl)
                    {
                        Assert.Equal(0, _pendingStateChanges);
                        rwlIsReaderLockHeld = _rwl.IsReaderLockHeld;
                    }

                    bool isReaderLockHeld = ThreadReaderLevel != 0;
                    Assert.Equal(isReaderLockHeld, rwlIsReaderLockHeld);
                    return isReaderLockHeld;
                }
            }

            public bool IsWriterLockHeld
            {
                get
                {
                    bool isWriteLockHeld, rwlIsWriteLockHeld;
                    lock (_rwl)
                    {
                        Assert.Equal(0, _pendingStateChanges);
                        isWriteLockHeld = _writerThreadID == Environment.CurrentManagedThreadId;
                        Assert.True(!isWriteLockHeld || _writerLevel != 0);
                        rwlIsWriteLockHeld = _rwl.IsWriterLockHeld;
                    }

                    Assert.Equal(isWriteLockHeld, rwlIsWriteLockHeld);
                    return isWriteLockHeld;
                }
            }

            private void VerifyState()
            {
                Assert.False(ThreadReaderLevel != 0 && _writerLevel != 0);
                Assert.Equal(ThreadReaderLevel != 0, _rwl.IsReaderLockHeld);
                Assert.Equal(_writerThreadID == Environment.CurrentManagedThreadId, _rwl.IsWriterLockHeld);
                Assert.Equal(_writerSeqNum, _rwl.WriterSeqNum);
            }

            private int GetTimeoutMilliseconds(int expectedFailureHResult)
            {
                return expectedFailureHResult == 0 ? UnexpectedTimeoutMilliseconds : 0;
            }

            private ApplicationException TryCatch(Action action)
            {
                lock (_rwl)
                {
                    ++_pendingStateChanges;
                }

                bool success = false;
                try
                {
                    action();
                    success = true;
                }
                catch (ApplicationException ex)
                {
                    return ex;
                }
                finally
                {
                    lock (_rwl)
                    {
                        --_pendingStateChanges;

                        if (!success)
                        {
                            VerifyState();
                        }
                    }
                }
                return null;
            }

            public void AcquireReaderLock(int expectedFailureHResult = 0)
            {
                ApplicationException ex =
                    TryCatch(() => _rwl.AcquireReaderLock(GetTimeoutMilliseconds(expectedFailureHResult)));
                Assert.Equal(expectedFailureHResult, ex == null ? 0 : ex.HResult);

                lock (_rwl)
                {
                    if (ex == null)
                    {
                        if (_writerThreadID == Environment.CurrentManagedThreadId)
                        {
                            // Write lock is already held, acquire a write lock recursively instead
                            Assert.NotEqual(0, _writerLevel);
                            ++_writerLevel;
                        }
                        else
                        {
                            ++ThreadReaderLevel;
                        }
                    }

                    if (_pendingStateChanges == 0)
                    {
                        VerifyState();
                    }
                }
            }

            public void AcquireWriterLock(int expectedFailureHResult = 0)
            {
                ApplicationException ex =
                    TryCatch(() => _rwl.AcquireWriterLock(GetTimeoutMilliseconds(expectedFailureHResult)));
                Assert.Equal(expectedFailureHResult, ex == null ? 0 : ex.HResult);

                lock (_rwl)
                {
                    if (ex == null)
                    {
                        if (_writerLevel == 0)
                        {
                            _writerThreadID = Environment.CurrentManagedThreadId;
                            ++_writerSeqNum;
                        }
                        else
                        {
                            Assert.Equal(Environment.CurrentManagedThreadId, _writerThreadID);
                        }
                        ++_writerLevel;
                    }

                    if (_pendingStateChanges == 0)
                    {
                        VerifyState();
                    }
                }
            }

            public void ReleaseReaderLock(int expectedFailureHResult = 0)
            {
                ApplicationException ex = TryCatch(() => _rwl.ReleaseReaderLock());
                Assert.Equal(expectedFailureHResult, ex == null ? 0 : ex.HResult);

                lock (_rwl)
                {
                    if (ex == null)
                    {
                        Assert.NotEqual(0, ThreadReaderLevel);
                        --ThreadReaderLevel;
                    }

                    if (_pendingStateChanges == 0)
                    {
                        VerifyState();
                    }
                }
            }

            public void ReleaseWriterLock(int expectedFailureHResult = 0)
            {
                ApplicationException ex = TryCatch(() => _rwl.ReleaseWriterLock());
                Assert.Equal(expectedFailureHResult, ex == null ? 0 : ex.HResult);

                lock (_rwl)
                {
                    if (ex == null)
                    {
                        Assert.Equal(Environment.CurrentManagedThreadId, _writerThreadID);
                        Assert.NotEqual(0, _writerLevel);
                        --_writerLevel;
                        if (_writerLevel == 0)
                        {
                            _writerThreadID = InvalidThreadID;
                        }
                    }

                    if (_pendingStateChanges == 0)
                    {
                        VerifyState();
                    }
                }
            }

            public TestLockCookie UpgradeToWriterLock(int expectedFailureHResult = 0)
            {
                var tlc = new TestLockCookie();
                ApplicationException ex =
                    TryCatch(() => tlc._lockCookie = _rwl.UpgradeToWriterLock(GetTimeoutMilliseconds(expectedFailureHResult)));
                Assert.Equal(expectedFailureHResult, ex == null ? 0 : ex.HResult);

                lock (_rwl)
                {
                    if (ex == null)
                    {
                        tlc._readerLevel = ThreadReaderLevel;
                        tlc._writerLevel = _writerLevel;

                        ThreadReaderLevel = 0;
                        if (_writerLevel == 0)
                        {
                            _writerThreadID = Environment.CurrentManagedThreadId;
                            ++_writerSeqNum;
                        }
                        else
                        {
                            Assert.Equal(Environment.CurrentManagedThreadId, _writerThreadID);
                        }
                        ++_writerLevel;
                    }

                    if (_pendingStateChanges == 0)
                    {
                        VerifyState();
                    }
                }
                return ex == null ? tlc : null;
            }

            public void DowngradeFromWriterLock(TestLockCookie tlc, int expectedFailureHResult = 0)
            {
                Assert.NotNull(tlc);

                ApplicationException ex = TryCatch(() => _rwl.DowngradeFromWriterLock(ref tlc._lockCookie));
                Assert.Equal(expectedFailureHResult, ex == null ? 0 : ex.HResult);

                lock (_rwl)
                {
                    if (ex == null)
                    {
                        Assert.Equal(Environment.CurrentManagedThreadId, _writerThreadID);
                        Assert.NotEqual(0, _writerLevel);
                        if (tlc._readerLevel == 0)
                        {
                            Assert.True(_writerLevel > tlc._writerLevel);
                            _writerLevel = tlc._writerLevel;
                            if (_writerLevel == 0)
                            {
                                _writerThreadID = InvalidThreadID;
                            }
                        }
                        else
                        {
                            _writerLevel = 0;
                            _writerThreadID = InvalidThreadID;
                            Assert.True(ThreadReaderLevel == 0);
                            ThreadReaderLevel = tlc._readerLevel;
                        }
                    }

                    if (_pendingStateChanges == 0)
                    {
                        VerifyState();
                    }
                }
            }

            public TestLockCookie ReleaseLock()
            {
                var tlc = new TestLockCookie();
                ApplicationException ex = TryCatch(() => tlc._lockCookie = _rwl.ReleaseLock());
                Assert.Null(ex);

                lock (_rwl)
                {
                    tlc._readerLevel = ThreadReaderLevel;
                    tlc._writerLevel = _writerLevel;

                    ThreadReaderLevel = 0;
                    _writerLevel = 0;
                    _writerThreadID = InvalidThreadID;

                    if (_pendingStateChanges == 0)
                    {
                        VerifyState();
                    }
                }
                return tlc;
            }

            public void RestoreLock(TestLockCookie tlc, int expectedFailureHResult = 0)
            {
                Assert.NotNull(tlc);
                Assert.NotEqual(TimeoutExceptionHResult, expectedFailureHResult);

                ApplicationException ex = TryCatch(() => _rwl.RestoreLock(ref tlc._lockCookie));
                Assert.Equal(expectedFailureHResult, ex == null ? 0 : ex.HResult);

                lock (_rwl)
                {
                    if (ex == null)
                    {
                        Assert.Equal(0, ThreadReaderLevel);
                        Assert.Equal(InvalidThreadID, _writerThreadID);
                        Assert.Equal(0, _writerLevel);
                        ThreadReaderLevel = tlc._readerLevel;
                        _writerLevel = tlc._writerLevel;
                        if (_writerLevel != 0)
                        {
                            _writerThreadID = Environment.CurrentManagedThreadId;
                            ++_writerSeqNum;
                        }
                    }

                    if (_pendingStateChanges == 0)
                    {
                        VerifyState();
                    }
                }
            }
        }

        private class TestLockCookie
        {
            public LockCookie _lockCookie;
            public int _readerLevel;
            public int _writerLevel;

            public TestLockCookie Clone()
            {
                return (TestLockCookie)MemberwiseClone();
            }
        }
    }
}
