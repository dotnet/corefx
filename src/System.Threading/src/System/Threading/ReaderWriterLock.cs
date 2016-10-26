// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Runtime.ConstrainedExecution;

namespace System.Threading
{
    public sealed class ReaderWriterLock : CriticalFinalizerObject
    {
        private const int InvalidThreadID = -1;
        private const ushort MaxAcquireCount = ushort.MaxValue;
        private static readonly int DefaultSpinCount = Environment.ProcessorCount != 1 ? 500 : 0;

        private static long s_mostRecentLockID;

        private ManualResetEvent _readerEvent;
        private AutoResetEvent _writerEvent;
        private long _lockID;
        private volatile int _state;
        private int _writerID = InvalidThreadID;
        private int _writerSeqNum;
        private ushort _writerLevel;

#if RWLOCK_STATISTICS
        private int _readerEntryCount;
        private int _readerContentionCount;
        private int _writerEntryCount;
        private int _writerContentionCount;
        private int _eventsReleasedCount;
#endif // RWLOCK_STATISTICS

        public ReaderWriterLock()
        {
            _lockID = Interlocked.Increment(ref s_mostRecentLockID);
            GC.SuppressFinalize(this);
        }

        ~ReaderWriterLock()
        {
        }

        public bool IsReaderLockHeld
        {
            get
            {
                LockEntry lockEntry = LockEntry.GetCurrent(_lockID);
                if (lockEntry != null)
                {
                    return lockEntry._readerLevel != 0;
                }
                return false;
            }
        }

        public bool IsWriterLockHeld
        {
            get
            {
                return _writerID == GetCurrentThreadID();
            }
        }

        public int WriterSeqNum
        {
            get
            {
                return _writerSeqNum;
            }
        }

        public bool AnyWritersSince(int seqNum)
        {
            if (_writerID == GetCurrentThreadID())
            {
                ++seqNum;
            }
            return (uint)_writerSeqNum > (uint)seqNum;
        }

        public void AcquireReaderLock(int millisecondsTimeout)
        {
            if (millisecondsTimeout < -1)
            {
                throw GetInvalidTimeoutException(nameof(millisecondsTimeout));
            }

            LockEntry lockEntry = LockEntry.GetOrCreateCurrent(_lockID);

            // Check for the fast path
            if (Interlocked.CompareExchange(ref _state, LockStates.Reader, 0) == 0)
            {
                Debug.Assert(lockEntry._readerLevel == 0);
            }
            // Check for nested reader
            else if (lockEntry._readerLevel != 0)
            {
                Debug.Assert((_state & LockStates.ReadersMask) != 0);

                if (lockEntry._readerLevel == MaxAcquireCount)
                {
                    throw new OverflowException(SR.Overflow_UInt16);
                }
                ++lockEntry._readerLevel;
                return;
            }
            // Check if the thread already has writer lock
            else if (_writerID == GetCurrentThreadID())
            {
                AcquireWriterLock(millisecondsTimeout);
                Debug.Assert(lockEntry.IsEmpty);
                return;
            }
            else
            {
                int spinCount = 0;
                int currentState = _state;
                do
                {
                    int knownState = currentState;

                    // Reader need not wait if there are only readers and no writer
                    if (knownState < LockStates.ReadersMask ||
                        (
                            (knownState & LockStates.ReaderSignaled) != 0 &&
                            (knownState & LockStates.Writer) == 0 &&
                            (
                                // A waiting reader, after successfully completing the wait, expects that it can become a
                                // reader, so ensure that there is enough room for waiting readers and this potential reader.
                                (
                                    (knownState & LockStates.ReadersMask) +
                                    ((knownState & LockStates.WaitingReadersMask) >> LockStates.WaitingReadersShift)
                                ) <= LockStates.ReadersMask - 2
                            )
                        ))
                    {
                        // Add to readers
                        currentState = Interlocked.CompareExchange(ref _state, knownState + LockStates.Reader, knownState);
                        if (currentState == knownState)
                        {
                            // One more reader
                            break;
                        }
                        continue;
                    }

                    // Check for too many readers or waiting readers, or if signaling is in progress
                    if ((knownState & LockStates.ReadersMask) == LockStates.ReadersMask ||
                        (knownState & LockStates.WaitingReadersMask) == LockStates.WaitingReadersMask ||
                        (knownState & LockStates.CachingEvents) == LockStates.ReaderSignaled)
                    {
                        // Sleep for a while, then update to the latest state and try again
                        Helpers.Sleep(1000);
                        spinCount = 0;
                        currentState = _state;
                        continue;
                    }

                    ++spinCount;

                    // Check if events are being cached. The purpose of this check is that "caching" events could involve
                    // disposing one or both of {_readerEvent, _writerEvent}. This check prevents the waiting code below from
                    // trying to use these events during this dangerous time, and instead causes the loop to spin until the
                    // caching state is cleared and events can be recreated. See ReleaseEvents() and callers.
                    if ((knownState & LockStates.CachingEvents) == LockStates.CachingEvents)
                    {
                        if (spinCount > DefaultSpinCount)
                        {
                            Helpers.Sleep(1);
                            spinCount = 0;
                        }
                        currentState = _state;
                        continue;
                    }

                    // Check spin count
                    if (spinCount <= DefaultSpinCount)
                    {
                        currentState = _state;
                        continue;
                    }

                    // Add to waiting readers
                    currentState = Interlocked.CompareExchange(ref _state, knownState + LockStates.WaitingReader, knownState);
                    if (currentState != knownState)
                    {
                        continue;
                    }

#if RWLOCK_STATISTICS
                    Interlocked.Increment(ref _readerContentionCount);
#endif

                    int modifyState = -LockStates.WaitingReader;
                    ManualResetEvent readerEvent = null;
                    bool waitSucceeded = false;
                    try
                    {
                        readerEvent = GetOrCreateReaderEvent();
                        waitSucceeded = readerEvent.WaitOne(millisecondsTimeout);

                        // AcquireReaderLock cannot have reentry via pumping while waiting for readerEvent, so
                        // lockEntry's state should not change from underneath us
                        Debug.Assert(lockEntry.HasLockID(_lockID));

                        if (waitSucceeded)
                        {
                            // Become a reader
                            Debug.Assert((_state & LockStates.ReaderSignaled) != 0);
                            Debug.Assert((_state & LockStates.ReadersMask) < LockStates.ReadersMask);
                            modifyState += LockStates.Reader;
                        }
                    }
                    finally
                    {
                        // Make the state changes determined above
                        knownState = Interlocked.Add(ref _state, modifyState) - modifyState;

                        if (!waitSucceeded)
                        {
                            // Check for last signaled waiting reader
                            if ((knownState & LockStates.ReaderSignaled) != 0 &&
                                (knownState & LockStates.WaitingReadersMask) == LockStates.WaitingReader)
                            {
                                if (readerEvent == null)
                                {
                                    readerEvent = _readerEvent;
                                    Debug.Assert(readerEvent != null);
                                }

                                // Ensure the event is signalled before resetting it, since the ReaderSignaled state is set
                                // before the event is set.
                                readerEvent.WaitOne();
                                Debug.Assert((_state & LockStates.ReadersMask) < LockStates.ReadersMask);

                                // Reset the event and lower reader signaled flag
                                readerEvent.Reset();
                                Interlocked.Add(ref _state, LockStates.Reader - LockStates.ReaderSignaled);

                                // Honor the orginal status
                                ++lockEntry._readerLevel;
                                ReleaseReaderLock();
                            }

                            Debug.Assert(lockEntry.IsEmpty);
                        }
                    }

                    if (!waitSucceeded)
                    {
                        throw GetTimeoutException();
                    }

                    // Check for last signaled waiting reader
                    Debug.Assert((knownState & LockStates.ReaderSignaled) != 0);
                    Debug.Assert((knownState & LockStates.ReadersMask) < LockStates.ReadersMask);
                    if ((knownState & LockStates.WaitingReadersMask) == LockStates.WaitingReader)
                    {
                        // Reset the event and the reader signaled flag
                        readerEvent.Reset();
                        Interlocked.Add(ref _state, -LockStates.ReaderSignaled);
                    }

                    break;
                } while (YieldProcessor());
            }

            // Success
            Debug.Assert((_state & LockStates.Writer) == 0);
            Debug.Assert((_state & LockStates.ReadersMask) != 0);
            ++lockEntry._readerLevel;
#if RWLOCK_STATISTICS
            Interlocked.Increment(ref _readerEntryCount);
#endif
        }

        public void AcquireReaderLock(TimeSpan timeout) => AcquireReaderLock(ToTimeoutMilliseconds(timeout));

        public void AcquireWriterLock(int millisecondsTimeout)
        {
            if (millisecondsTimeout < -1)
            {
                throw GetInvalidTimeoutException(nameof(millisecondsTimeout));
            }

            int threadID = GetCurrentThreadID();

            // Check for the fast path
            if (Interlocked.CompareExchange(ref _state, LockStates.Writer, 0) == 0)
            {
                Debug.Assert((_state & LockStates.ReadersMask) == 0);
            }
            // Check if the thread already has writer lock
            else if (_writerID == threadID)
            {
                if (_writerLevel == MaxAcquireCount)
                {
                    throw new OverflowException(SR.Overflow_UInt16);
                }
                ++_writerLevel;
                return;
            }
            else
            {
                int spinCount = 0;
                int currentState = _state;
                do
                {
                    int knownState = currentState;

                    // Writer need not wait if there are no readers and writer
                    if (knownState == 0 || knownState == LockStates.CachingEvents)
                    {
                        // Can be a writer
                        currentState = Interlocked.CompareExchange(ref _state, knownState + LockStates.Writer, knownState);
                        if (currentState == knownState)
                        {
                            // Only writer
                            break;
                        }
                        continue;
                    }

                    // Check for too many waiting writers
                    if ((knownState & LockStates.WaitingWritersMask) == LockStates.WaitingWritersMask)
                    {
                        Helpers.Sleep(1000);
                        spinCount = 0;
                        currentState = _state;
                        continue;
                    }

                    ++spinCount;

                    // Check if events are being cached. The purpose of this check is that "caching" events could involve
                    // disposing one or both of {_readerEvent, _writerEvent}. This check prevents the waiting code below from
                    // trying to use these events during this dangerous time, and instead causes the loop to spin until the
                    // caching state is cleared and events can be recreated. See ReleaseEvents() and callers.
                    if ((knownState & LockStates.CachingEvents) == LockStates.CachingEvents)
                    {
                        if (spinCount > DefaultSpinCount)
                        {
                            Helpers.Sleep(1);
                            spinCount = 0;
                        }
                        currentState = _state;
                        continue;
                    }

                    // Check spin count
                    if (spinCount <= DefaultSpinCount)
                    {
                        currentState = _state;
                        continue;
                    }

                    // Add to waiting writers
                    currentState = Interlocked.CompareExchange(ref _state, knownState + LockStates.WaitingWriter, knownState);
                    if (currentState != knownState)
                    {
                        continue;
                    }

#if RWLOCK_STATISTICS
                    Interlocked.Increment(ref _writerContentionCount);
#endif

                    int modifyState = -LockStates.WaitingWriter;
                    AutoResetEvent writerEvent = null;
                    bool waitSucceeded = false;
                    try
                    {
                        writerEvent = GetOrCreateWriterEvent();
                        waitSucceeded = writerEvent.WaitOne(millisecondsTimeout);

                        if (waitSucceeded)
                        {
                            // Become a writer and remove the writer-signaled state
                            Debug.Assert((_state & LockStates.WriterSignaled) != 0);
                            modifyState += LockStates.Writer - LockStates.WriterSignaled;
                        }
                    }
                    finally
                    {
                        // Make the state changes determined above
                        knownState = Interlocked.Add(ref _state, modifyState) - modifyState;

                        if (!waitSucceeded &&
                            (knownState & LockStates.WriterSignaled) != 0 &&
                            (knownState & LockStates.WaitingWritersMask) == LockStates.WaitingWriter)
                        {
                            if (writerEvent == null)
                            {
                                writerEvent = _writerEvent;
                                Debug.Assert(writerEvent != null);
                            }

                            while (true)
                            {
                                knownState = _state;
                                if ((knownState & LockStates.WriterSignaled) == 0 ||
                                    (knownState & LockStates.WaitingWritersMask) != 0)
                                {
                                    break;
                                }

                                if (!writerEvent.WaitOne(10))
                                {
                                    continue;
                                }

                                modifyState = LockStates.Writer - LockStates.WriterSignaled;
                                knownState = Interlocked.Add(ref _state, modifyState) - modifyState;
                                Debug.Assert((knownState & LockStates.WriterSignaled) != 0);
                                Debug.Assert((knownState & LockStates.Writer) == 0);

                                // Honor the orginal status
                                _writerID = threadID;
                                Debug.Assert(_writerLevel == 0);
                                _writerLevel = 1;
                                ReleaseWriterLock();
                                break;
                            }
                        }
                    }

                    if (!waitSucceeded)
                    {
                        throw GetTimeoutException();
                    }
                    break;
                } while (YieldProcessor());
            }

            // Success
            Debug.Assert((_state & LockStates.Writer) != 0);
            Debug.Assert((_state & LockStates.ReadersMask) == 0);
            Debug.Assert(_writerID == InvalidThreadID);

            // Save threadid of the writer
            _writerID = threadID;
            _writerLevel = 1;
            ++_writerSeqNum;
#if RWLOCK_STATISTICS
            ++_writerEntryCount;
#endif
            return;
        }

        public void AcquireWriterLock(TimeSpan timeout) => AcquireWriterLock(ToTimeoutMilliseconds(timeout));

        public void ReleaseReaderLock()
        {
            // Check if the thread has writer lock
            if (_writerID == GetCurrentThreadID())
            {
                ReleaseWriterLock();
                return;
            }

            LockEntry lockEntry = LockEntry.GetCurrent(_lockID);
            if (lockEntry == null)
            {
                throw GetNotOwnerException();
            }

            Debug.Assert((_state & LockStates.Writer) == 0);
            Debug.Assert((_state & LockStates.ReadersMask) != 0);
            Debug.Assert(lockEntry._readerLevel != 0);

            --lockEntry._readerLevel;
            if (lockEntry._readerLevel != 0)
            {
                return;
            }

            // Not a reader any more
            bool isLastReader;
            bool cacheEvents;
            AutoResetEvent writerEvent = null;
            ManualResetEvent readerEvent = null;
            int currentState = _state;
            int knownState;
            do
            {
                isLastReader = false;
                cacheEvents = false;
                knownState = currentState;
                int modifyState = -LockStates.Reader;

                if ((knownState & (LockStates.ReadersMask | LockStates.ReaderSignaled)) == LockStates.Reader)
                {
                    isLastReader = true;
                    if ((knownState & LockStates.WaitingWritersMask) != 0)
                    {
                        try
                        {
                            writerEvent = GetOrCreateWriterEvent();
                        }
                        catch (SystemException)
                        {
                            Helpers.Sleep(100);
                            currentState = _state;
                            knownState = 0;
                            Debug.Assert(currentState != knownState);
                            continue;
                        }
                        modifyState += LockStates.WriterSignaled;
                    }
                    else if ((knownState & LockStates.WaitingReadersMask) != 0)
                    {
                        try
                        {
                            readerEvent = GetOrCreateReaderEvent();
                        }
                        catch (SystemException)
                        {
                            Helpers.Sleep(100);
                            currentState = _state;
                            knownState = 0;
                            Debug.Assert(currentState != knownState);
                            continue;
                        }
                        modifyState += LockStates.ReaderSignaled;
                    }
                    else if (knownState == LockStates.Reader && (_readerEvent != null || _writerEvent != null))
                    {
                        cacheEvents = true;
                        modifyState += LockStates.CachingEvents;
                    }
                }

                Debug.Assert((knownState & LockStates.Writer) == 0);
                Debug.Assert((knownState & LockStates.ReadersMask) != 0);
                currentState = Interlocked.CompareExchange(ref _state, knownState + modifyState, knownState);
            } while (currentState != knownState);

            // Check for last reader
            if (isLastReader)
            {
                // Check for waiting writers
                if ((knownState & LockStates.WaitingWritersMask) != 0)
                {
                    Debug.Assert((_state & LockStates.WriterSignaled) != 0);
                    Debug.Assert(writerEvent != null);
                    writerEvent.Set();
                }
                // Check for waiting readers
                else if ((knownState & LockStates.WaitingReadersMask) != 0)
                {
                    Debug.Assert((_state & LockStates.ReaderSignaled) != 0);
                    Debug.Assert(readerEvent != null);
                    readerEvent.Set();
                }
                // Check for the need to release events
                else if (cacheEvents)
                {
                    ReleaseEvents();
                }
            }

            Debug.Assert(lockEntry.IsEmpty);
        }

        public void ReleaseWriterLock()
        {
            if (_writerID != GetCurrentThreadID())
            {
                throw GetNotOwnerException();
            }

            Debug.Assert((_state & LockStates.ReadersMask) == 0);
            Debug.Assert((_state & LockStates.Writer) != 0);
            Debug.Assert(_writerLevel != 0);

            // Check for nested release
            --_writerLevel;
            if (_writerLevel != 0)
            {
                return;
            }

            // Not a writer any more
            _writerID = InvalidThreadID;
            bool cacheEvents;
            ManualResetEvent readerEvent = null;
            AutoResetEvent writerEvent = null;
            int currentState = _state;
            int knownState;
            do
            {
                cacheEvents = false;
                knownState = currentState;
                int modifyState = -LockStates.Writer;

                if ((knownState & LockStates.WaitingReadersMask) != 0)
                {
                    try
                    {
                        readerEvent = GetOrCreateReaderEvent();
                    }
                    catch (SystemException)
                    {
                        Helpers.Sleep(100);
                        currentState = _state;
                        knownState = 0;
                        Debug.Assert(currentState != knownState);
                        continue;
                    }
                    modifyState += LockStates.ReaderSignaled;
                }
                else if ((knownState & LockStates.WaitingWritersMask) != 0)
                {
                    try
                    {
                        writerEvent = GetOrCreateWriterEvent();
                    }
                    catch (SystemException)
                    {
                        Helpers.Sleep(100);
                        currentState = _state;
                        knownState = 0;
                        Debug.Assert(currentState != knownState);
                        continue;
                    }
                    modifyState += LockStates.WriterSignaled;
                }
                else if (knownState == LockStates.Writer && (_readerEvent != null || _writerEvent != null))
                {
                    cacheEvents = true;
                    modifyState += LockStates.CachingEvents;
                }

                Debug.Assert((knownState & LockStates.ReadersMask) == 0);
                Debug.Assert((knownState & LockStates.Writer) != 0);
                currentState = Interlocked.CompareExchange(ref _state, knownState + modifyState, knownState);
            } while (currentState != knownState);

            // Check for waiting readers
            if ((knownState & LockStates.WaitingReadersMask) != 0)
            {
                Debug.Assert((_state & LockStates.ReaderSignaled) != 0);
                Debug.Assert(readerEvent != null);
                readerEvent.Set();
            }
            // Check for waiting writers
            else if ((knownState & LockStates.WaitingWritersMask) != 0)
            {
                Debug.Assert((_state & LockStates.WriterSignaled) != 0);
                Debug.Assert(writerEvent != null);
                writerEvent.Set();
            }
            // Check for the need to release events
            else if (cacheEvents)
            {
                ReleaseEvents();
            }
        }

        public LockCookie UpgradeToWriterLock(int millisecondsTimeout)
        {
            if (millisecondsTimeout < -1)
            {
                throw GetInvalidTimeoutException(nameof(millisecondsTimeout));
            }

            var lockCookie = new LockCookie();
            int threadID = GetCurrentThreadID();
            lockCookie._threadID = threadID;

            // Check if the thread is already a writer
            if (_writerID == threadID)
            {
                // Update cookie state
                lockCookie._flags = LockCookieFlags.Upgrade | LockCookieFlags.OwnedWriter;
                lockCookie._writerLevel = _writerLevel;

                // Acquire the writer lock again
                AcquireWriterLock(millisecondsTimeout);
                return lockCookie;
            }

            LockEntry lockEntry = LockEntry.GetCurrent(_lockID);
            if (lockEntry == null)
            {
                lockCookie._flags = LockCookieFlags.Upgrade | LockCookieFlags.OwnedNone;
            }
            else
            {
                // Sanity check
                Debug.Assert((_state & LockStates.ReadersMask) != 0);
                Debug.Assert(lockEntry._readerLevel != 0);

                // Save lock state in the cookie
                lockCookie._flags = LockCookieFlags.Upgrade | LockCookieFlags.OwnedReader;
                lockCookie._readerLevel = lockEntry._readerLevel;

                // If there is only one reader, try to convert reader to a writer
                int knownState = Interlocked.CompareExchange(ref _state, LockStates.Writer, LockStates.Reader);
                if (knownState == LockStates.Reader)
                {
                    // Thread is no longer a reader
                    lockEntry._readerLevel = 0;
                    Debug.Assert(lockEntry.IsEmpty);

                    // Thread is a writer
                    _writerID = threadID;
                    _writerLevel = 1;
                    ++_writerSeqNum;

                    // No intevening writes
#if RWLOCK_STATISTICS
                    ++_writerEntryCount;
#endif
                    return lockCookie;
                }

                // Release the reader lock
                lockEntry._readerLevel = 1;
                ReleaseReaderLock();
            }

            // We are aware of the contention on the lock and the thread will most probably block to acquire writer lock
            bool acquired = false;
            try
            {
                AcquireWriterLock(millisecondsTimeout);
                acquired = true;
                return lockCookie;
            }
            finally
            {
                if (!acquired)
                {
                    // Invalidate cookie
                    LockCookieFlags flags = lockCookie._flags;
                    lockCookie._flags = LockCookieFlags.Invalid;

                    RecoverLock(ref lockCookie, flags & LockCookieFlags.OwnedReader);
                }
            }
        }

        public LockCookie UpgradeToWriterLock(TimeSpan timeout) => UpgradeToWriterLock(ToTimeoutMilliseconds(timeout));

        public void DowngradeFromWriterLock(ref LockCookie lockCookie)
        {
            int threadID = GetCurrentThreadID();
            if (_writerID != threadID)
            {
                throw GetNotOwnerException();
            }

            // Validate cookie
            LockCookieFlags flags = lockCookie._flags;
            ushort requestedWriterLevel = lockCookie._writerLevel;
            if ((flags & LockCookieFlags.Invalid) != 0 ||
                lockCookie._threadID != threadID ||
                // REVIEW: Addition to original code, see large REVIEW comment below
                (
                    // Cannot downgrade to a writer level that is greater than or equal to the current
                    (flags & (LockCookieFlags.OwnedWriter | LockCookieFlags.OwnedNone)) != 0 &&
                    _writerLevel <= requestedWriterLevel
                ))
            {
                throw GetInvalidLockCookieException();
            }

            // Check if the thread was a reader
            if ((flags & LockCookieFlags.OwnedReader) != 0)
            {
                // REVIEW: Original code:
                //   Debug.Assert(_writerLevel == 1);
                // I believe this assert is incorrect. Multiple recursive write locks could have been taken before downgrading,
                // and the code below the assert release all write locks as expected.
                //
                // Several of the new tests fail with the original assertion. Fixed the assert under this assumption.
                Debug.Assert(_writerLevel != 0);

                LockEntry lockEntry = LockEntry.GetOrCreateCurrent(_lockID);

                // Downgrade to a reader
                _writerID = InvalidThreadID;
                _writerLevel = 0;
                ManualResetEvent readerEvent = null;
                int currentState = _state;
                int knownState;
                do
                {
                    knownState = currentState;
                    int modifyState = LockStates.Reader - LockStates.Writer;
                    if ((knownState & LockStates.WaitingReadersMask) != 0)
                    {
                        try
                        {
                            readerEvent = GetOrCreateReaderEvent();
                        }
                        catch (SystemException)
                        {
                            Helpers.Sleep(100);
                            currentState = _state;
                            knownState = 0;
                            Debug.Assert(currentState != knownState);
                            continue;
                        }
                        modifyState += LockStates.ReaderSignaled;
                    }

                    Debug.Assert((knownState & LockStates.ReadersMask) == 0);
                    currentState = Interlocked.CompareExchange(ref _state, knownState + modifyState, knownState);
                } while (currentState != knownState);

                // Check for waiting readers
                if ((knownState & LockStates.WaitingReadersMask) != 0)
                {
                    Debug.Assert((_state & LockStates.ReaderSignaled) != 0);
                    Debug.Assert(readerEvent != null);
                    readerEvent.Set();
                }

                // Restore reader nesting level
                lockEntry._readerLevel = lockCookie._readerLevel;
#if RWLOCK_STATISTICS
                Interlocked.Increment(ref _readerEntryCount);
#endif
            }
            else if ((flags & (LockCookieFlags.OwnedWriter | LockCookieFlags.OwnedNone)) != 0)
            {
                // REVIEW: Original code:
                //     ReleaseWriterLock();
                //     Debug.Assert((flags & LockCookieFlags.OwnedWriter) != 0 || _writerID != threadID);
                //
                // I believe the assertion is correct, but the code is not, since the lock cookie is ignored on this path.
                // UpgradeToWriterLock allows upgrading from an unlocked state or when the write lock is already held, where it
                // just calls AcquireWriteLock. To compensate, I believe DowngradeFromWriterLock intends to just behave as
                // ReleaseWriterLock.
                //
                // However, the lock cookie could be several operations old. Consider:
                //   lockCookie = UpgradeToWriterLock()
                //   AcquireWriterLock()
                //   DowngradeFromWriterLock(ref lockCookie)
                //
                // Since the lock cookie indicates that no lock was held at the time of the upgrade, The ReleaseWriterLock in
                // the original code above does not result in releasing all writer locks as requested by the lock cookie and as
                // expected by the assertion. The code should respect the lock cookie (as it does in the case above where the
                // lock cookie indicates that a read lock was held), and restore the writer level appropriately.
                //
                // Similarly, when the lock cookie does indicate that a write lock was held, the downgrade does not restore the
                // write lock recursion level to that indicated by the lock cookie. Consider:
                //   AcquireWriterLock()
                //   lockCookie = UpgradeToWriterLock()
                //   AcquireWriterLock()
                //   DowngradeFromWriterLock(ref lockCookie) // does not restore recursion level of write lock!
                // I assume this quirk is unintended as well.
                //
                // Fixed the code under the assumptions above, and retained the original assert.
                Debug.Assert(_writerLevel != 0);
                Debug.Assert(_writerLevel > requestedWriterLevel);
                if (requestedWriterLevel != 0)
                {
                    _writerLevel = requestedWriterLevel;
                }
                else
                {
                    if (_writerLevel != 1)
                    {
                        _writerLevel = 1;
                    }
                    ReleaseWriterLock();
                }
                Debug.Assert((flags & LockCookieFlags.OwnedWriter) != 0 || _writerID != threadID);
            }

            // Update the validation fields of the cookie
            lockCookie._flags = LockCookieFlags.Invalid;
        }

        public LockCookie ReleaseLock()
        {
            var lockCookie = new LockCookie();
            int threadID = GetCurrentThreadID();
            lockCookie._threadID = threadID;

            if (_writerID == threadID)
            {
                // Save lock state in the cookie
                lockCookie._flags = LockCookieFlags.Release | LockCookieFlags.OwnedWriter;
                lockCookie._writerLevel = _writerLevel;

                // Release the writer lock
                _writerLevel = 1;
                ReleaseWriterLock();
                return lockCookie;
            }

            LockEntry lockEntry = LockEntry.GetCurrent(_lockID);
            if (lockEntry == null)
            {
                lockCookie._flags = LockCookieFlags.Release | LockCookieFlags.OwnedNone;
                return lockCookie;
            }

            Debug.Assert((_state & LockStates.ReadersMask) != 0);
            Debug.Assert(lockEntry._readerLevel != 0);

            // Save lock state in the cookie
            lockCookie._flags = LockCookieFlags.Release | LockCookieFlags.OwnedReader;
            lockCookie._readerLevel = lockEntry._readerLevel;

            // Release the reader lock
            lockEntry._readerLevel = 1;
            ReleaseReaderLock();
            return lockCookie;
        }

        public void RestoreLock(ref LockCookie lockCookie)
        {
            // Validate cookie
            int threadID = GetCurrentThreadID();
            if (lockCookie._threadID != threadID)
            {
                throw GetInvalidLockCookieException();
            }

            if (_writerID == threadID || LockEntry.GetCurrent(_lockID) != null)
            {
                throw new SynchronizationLockException(SR.ReaderWriterLock_RestoreLockWithOwnedLocks);
            }

            LockCookieFlags flags = lockCookie._flags;
            if ((flags & LockCookieFlags.Invalid) != 0)
            {
                throw GetInvalidLockCookieException();
            }

            do
            {
                if ((flags & LockCookieFlags.OwnedNone) != 0)
                {
                    break;
                }

                // Check for the no contention case
                if ((flags & LockCookieFlags.OwnedWriter) != 0)
                {
                    if (Interlocked.CompareExchange(ref _state, LockStates.Writer, 0) == 0)
                    {
                        // Restore writer nesting level
                        _writerID = threadID;
                        _writerLevel = lockCookie._writerLevel;
                        ++_writerSeqNum;
#if RWLOCK_STATISTICS
                        ++_writerEntryCount;
#endif
                        break;
                    }
                }
                else if ((flags & LockCookieFlags.OwnedReader) != 0)
                {
                    // This thread should not already be a reader else bad things can happen
                    LockEntry lockEntry = LockEntry.GetOrCreateCurrent(_lockID);
                    Debug.Assert(lockEntry.IsEmpty);

                    int knownState = _state;
                    if (knownState < LockStates.ReadersMask &&
                        Interlocked.CompareExchange(ref _state, knownState + LockStates.Reader, knownState) == knownState)
                    {
                        // Restore reader nesting level
                        lockEntry._readerLevel = lockCookie._readerLevel;
#if RWLOCK_STATISTICS
                        Interlocked.Increment(ref _readerEntryCount);
#endif
                        break;
                    }
                }

                // We are aware of the contention on the lock and the thread will most probably block to acquire the lock
                RecoverLock(ref lockCookie, flags);
            } while (false);

            lockCookie._flags = LockCookieFlags.Invalid;
        }

        /// <summary>
        /// Helper function that restores the lock to the original state indicated by parameters
        /// </summary>
        private void RecoverLock(ref LockCookie lockCookie, LockCookieFlags flags)
        {
            // Contrary to the legacy code, this method does not use a finite timeout for recovering the previous lock state, as
            // a timeout would leave the lock in an inconsistent state. That possibility is not documented, but as documented,
            // the caller of the public entry method should expect that it does not return until the state is consistent.

            // Check if the thread was a writer
            if ((flags & LockCookieFlags.OwnedWriter) != 0)
            {
                // Acquire writer lock
                AcquireWriterLock(Timeout.Infinite);
                _writerLevel = lockCookie._writerLevel;
            }
            // Check if the thread was a reader
            else if ((flags & LockCookieFlags.OwnedReader) != 0)
            {
                AcquireReaderLock(Timeout.Infinite);
                LockEntry lockEntry = LockEntry.GetCurrent(_lockID);
                Debug.Assert(lockEntry != null);
                lockEntry._readerLevel = lockCookie._readerLevel;
            }
        }

        private static int GetCurrentThreadID()
        {
            int threadID = Environment.CurrentManagedThreadId;
            Debug.Assert(threadID != InvalidThreadID);
            return threadID;
        }

        private static bool YieldProcessor()
        {
            // Indicate to the processor that we are spinning. The return value facilitates usage in do-while spin loops that
            // use 'continue' statements for readability, like:
            //   do { ... } while (YieldProcessor());
            Helpers.Spin(1);
            return true;
        }

        private ManualResetEvent GetOrCreateReaderEvent()
        {
            ManualResetEvent currentEvent = _readerEvent;
            if (currentEvent != null)
            {
                return currentEvent;
            }

            currentEvent = new ManualResetEvent(false);
            ManualResetEvent previousEvent = Interlocked.CompareExchange(ref _readerEvent, currentEvent, null);
            if (previousEvent == null)
            {
                return currentEvent;
            }

            currentEvent.Dispose();
            return previousEvent;
        }

        private AutoResetEvent GetOrCreateWriterEvent()
        {
            AutoResetEvent currentEvent = _writerEvent;
            if (currentEvent != null)
            {
                return currentEvent;
            }

            currentEvent = new AutoResetEvent(false);
            AutoResetEvent previousEvent = Interlocked.CompareExchange(ref _writerEvent, currentEvent, null);
            if (previousEvent == null)
            {
                return currentEvent;
            }

            currentEvent.Dispose();
            return previousEvent;
        }

        private void ReleaseEvents()
        {
            Debug.Assert((_state & LockStates.CachingEvents) == LockStates.CachingEvents);

            // Save events
            AutoResetEvent writerEvent = _writerEvent;
            _writerEvent = null;
            ManualResetEvent readerEvent = _readerEvent;
            _readerEvent = null;

            // Allow readers and writers to continue
            Interlocked.Add(ref _state, -LockStates.CachingEvents);

            // Cache events
            // TODO: (old) Disposing events for now. What is needed is an event cache to which the events are released.
            if (writerEvent != null)
            {
                writerEvent.Dispose();
            }
            if (readerEvent != null)
            {
                readerEvent.Dispose();
            }

#if RWLOCK_STATISTICS
            Interlocked.Increment(ref _eventsReleasedCount);
#endif
        }

        private static ArgumentOutOfRangeException GetInvalidTimeoutException(string parameterName)
        {
            return new ArgumentOutOfRangeException(parameterName, SR.ArgumentOutOfRange_TimeoutMilliseconds);
        }

        private static int ToTimeoutMilliseconds(TimeSpan timeout)
        {
            var timeoutMilliseconds = (long)timeout.TotalMilliseconds;
            if (timeoutMilliseconds < -1 || timeoutMilliseconds > int.MaxValue)
            {
                throw GetInvalidTimeoutException(nameof(timeout));
            }
            return (int)timeoutMilliseconds;
        }

        private class ReaderWriterLockApplicationException : ApplicationException
        {
            public ReaderWriterLockApplicationException(int errorHResult, string message)
                : base(SR.Format(message, SR.Format(SR.ExceptionFromHResult, errorHResult)))
            {
                HResult = errorHResult;
            }
        }

        private static ApplicationException GetTimeoutException()
        {
            return new ReaderWriterLockApplicationException(HResults.ERROR_TIMEOUT, SR.ReaderWriterLock_Timeout);
        }

        private static ApplicationException GetNotOwnerException()
        {
            return new ReaderWriterLockApplicationException(HResults.ERROR_NOT_OWNER, SR.ReaderWriterLock_NotOwner);
        }

        private static ApplicationException GetInvalidLockCookieException()
        {
            return new ReaderWriterLockApplicationException(HResults.E_INVALIDARG, SR.ReaderWriterLock_InvalidLockCookie);
        }

        // This would normally be a [Flags] enum, but due to the limited types on which methods of Interlocked operate, and to
        // avoid the required explicit casts between the enum and its underlying type, the values below are typed directly as
        // the underlying type.
        private static class LockStates
        {
            // Reader increment
            public const int Reader = 0x1;
            // Max number of readers
            public const int ReadersMask = 0x3ff;
            // Reader event is or is about to be signaled
            public const int ReaderSignaled = 0x400;
            // Writer event is or is about to be signaled
            public const int WriterSignaled = 0x800;
            public const int Writer = 0x1000;
            // Waiting reader increment
            public const int WaitingReader = 0x2000;
            // Max number of waiting readers (maximum count must be less than or equal to the maximum count of readers)
            public const int WaitingReadersMask = 0x7FE000;
            public const int WaitingReadersShift = 13;
            // Waiting writer increment
            public const int WaitingWriter = 0x800000;
            // Max number of waiting writers
            public const int WaitingWritersMask = unchecked((int)0xFF800000);
            // Events are being cached (for all intents and purposes, "cached" means "disposed of"). New acquire requests cannot
            // become waiters during this time since they need the events for waiting. Once events are disposed of and the
            // state is changed, new to-be-waiters can recreate the events they need.
            public const int CachingEvents = ReaderSignaled | WriterSignaled;
        }

        // REVIEW: The original code maintained lists of thread-local lock entries on the CLR's thread objects, and manually
        // released lock entries, which involved walking through all threads. While this is possible with ThreadLocal<T>, I
        // preferred to use a similar design to that from ReaderWriterLockSlim, and allow reusing empty entries without removing
        // entries, since it is unlikely that the list length for any thread would get unreasonably long. I have preserved the
        // logic from the original code to move recently used entries to the front of the list in the GetOrCreate path.
        /// <summary>
        /// Stores thread-local lock info and manages the association of this info with each <see cref="ReaderWriterLock"/>
        /// owned by a thread.
        /// </summary>
        private class LockEntry
        {
            [ThreadStatic]
            private static LockEntry t_lockEntryHead;

            private long _lockID;
            private LockEntry _next;
            public ushort _readerLevel;

            private LockEntry(long lockID)
            {
                _lockID = lockID;
            }

            public bool HasLockID(long lockID)
            {
                return _lockID == lockID;
            }

            public bool IsEmpty
            {
                get
                {
                    return _readerLevel == 0;
                }
            }

            private static void VerifyNoNonemptyEntryInListAfter(long lockID, LockEntry afterEntry)
            {
                Debug.Assert(lockID != 0);
                Debug.Assert(afterEntry != null);

#if DEBUG
                for (LockEntry currentEntry = afterEntry._next; currentEntry != null; currentEntry = currentEntry._next)
                {
                    Debug.Assert(currentEntry._lockID != lockID || currentEntry.IsEmpty);
                }
#endif
            }

            public static LockEntry GetCurrent(long lockID)
            {
                Debug.Assert(lockID != 0);

                LockEntry headEntry = t_lockEntryHead;
                for (LockEntry currentEntry = headEntry; currentEntry != null; currentEntry = currentEntry._next)
                {
                    if (currentEntry._lockID == lockID)
                    {
                        VerifyNoNonemptyEntryInListAfter(lockID, currentEntry);

                        // The lock ID assignment is only relevant when the entry is not empty, since the lock ID is not reset
                        // when its state becomes empty. Empty entries can be poached by other ReaderWriterLocks.
                        return currentEntry.IsEmpty ? null : currentEntry;
                    }
                }
                return null;
            }

            public static LockEntry GetOrCreateCurrent(long lockID)
            {
                Debug.Assert(lockID != 0);

                LockEntry headEntry = t_lockEntryHead;
                if (headEntry != null)
                {
                    if (headEntry._lockID == lockID)
                    {
                        VerifyNoNonemptyEntryInListAfter(lockID, headEntry);
                        return headEntry;
                    }

                    if (headEntry.IsEmpty)
                    {
                        VerifyNoNonemptyEntryInListAfter(lockID, headEntry);
                        headEntry._lockID = lockID;
                        return headEntry;
                    }
                }

                return GetOrCreateCurrentSlow(lockID, headEntry);
            }

            private static LockEntry GetOrCreateCurrentSlow(long lockID, LockEntry headEntry)
            {
                Debug.Assert(lockID != 0);
                Debug.Assert(headEntry == t_lockEntryHead);
                Debug.Assert(headEntry == null || headEntry._lockID != lockID);

                LockEntry entry = null;
                LockEntry emptyEntryPrevious = null;
                LockEntry emptyEntry = null;

                if (headEntry != null)
                {
                    if (headEntry.IsEmpty)
                    {
                        emptyEntry = headEntry;
                    }

                    for (LockEntry previousEntry = headEntry, currentEntry = headEntry._next;
                        currentEntry != null;
                        previousEntry = currentEntry, currentEntry = currentEntry._next)
                    {
                        if (currentEntry._lockID == lockID)
                        {
                            VerifyNoNonemptyEntryInListAfter(lockID, currentEntry);

                            // Unlink the entry, preparing to move it to the head of the list
                            previousEntry._next = currentEntry._next;
                            entry = currentEntry;
                            break;
                        }

                        if (emptyEntry == null && currentEntry.IsEmpty)
                        {
                            // Record the first empty entry in case there is no existing entry
                            emptyEntryPrevious = previousEntry;
                            emptyEntry = currentEntry;
                        }
                    }
                }

                if (entry == null)
                {
                    if (emptyEntry != null)
                    {
                        // Claim the first empty entry that was found
                        emptyEntry._lockID = lockID;

                        // Unlink the empty entry, preparing to move it to the head of the list
                        if (emptyEntryPrevious == null)
                        {
                            Debug.Assert(emptyEntry == headEntry);
                            return emptyEntry;
                        }
                        emptyEntryPrevious._next = emptyEntry._next;
                        entry = emptyEntry;
                    }
                    else
                    {
                        entry = new LockEntry(lockID);
                    }
                }

                // Insert the entry at the head of the list
                Debug.Assert(entry._lockID == lockID);
                entry._next = headEntry;
                t_lockEntryHead = entry;
                return entry;
            }
        }
    }

    [Flags]
    internal enum LockCookieFlags
    {
        // The performed operation that may need to be reverted
        Upgrade = 0x2000,
        Release = 0x4000,

        // Lock state before the performed operation, to which the lock state may need to be reverted
        OwnedNone = 0x10000,
        OwnedWriter = 0x20000,
        OwnedReader = 0x40000,

        Invalid = ~(Upgrade | Release | OwnedNone | OwnedWriter | OwnedReader)
    }
}
