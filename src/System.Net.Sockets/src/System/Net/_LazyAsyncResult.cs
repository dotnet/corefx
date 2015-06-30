// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Diagnostics;
using System.Collections;
using System.Threading.Tasks;

namespace System.Net
{
    // LazyAsyncResult - Base class for all IAsyncResult classes
    // that want to take advantage of lazy allocated event handles
    internal class LazyAsyncResult : IAsyncResult
    {
        private const int c_HighBit = unchecked((int)0x80000000);
        private const int c_ForceAsyncCount = 50;

#if !NET_PERF
        // This is to avoid user mistakes when they queue another async op from a callback the completes sync.
        [ThreadStatic]
        private static ThreadContext t_ThreadContext;

        private static ThreadContext CurrentThreadContext
        {
            get
            {
                ThreadContext threadContext = t_ThreadContext;
                if (threadContext == null)
                {
                    threadContext = new ThreadContext();
                    t_ThreadContext = threadContext;
                }
                return threadContext;
            }
        }

        private class ThreadContext
        {
            internal int m_NestedIOCount;
        }
#endif

#if DEBUG
        internal object _DebugAsyncChain = null;           // Optionally used to track chains of async calls.
        private bool _ProtectState;                 // Used by ContextAwareResult to prevent some calls.
#endif

        //
        // class members
        //
        private object m_AsyncObject;               // Caller's async object.
        private object m_AsyncState;                // Caller's state object.
        private AsyncCallback m_AsyncCallback;      // Caller's callback method.
        private object m_Result;                    // Final IO result to be returned byt the End*() method.
        private int m_ErrorCode;                    // Win32 error code for Win32 IO async calls (that want to throw).
        private int m_IntCompleted;                 // Sign bit indicates synchronous completion if set.
                                                    // Remaining bits count the number of InvokeCallbak() calls.

        private bool m_EndCalled;                   // true if the user called the End*() method.
        private bool m_UserEvent;                   // true if the event has been (or is about to be) handed to the user

        private object m_Event;                     // lazy allocated event to be returned in the IAsyncResult for the client to wait on


        internal LazyAsyncResult(object myObject, object myState, AsyncCallback myCallBack)
        {
            m_AsyncObject = myObject;
            m_AsyncState = myState;
            m_AsyncCallback = myCallBack;
            m_Result = DBNull.Value;
            GlobalLog.Print("LazyAsyncResult#" + Logging.HashString(this) + "::.ctor()");
#if TRACK_LAR
            _MyIndex = Interlocked.Increment(ref _PendingIndex);
            _PendingResults.Add(_MyIndex, this);
#endif
        }

        // Allows creating a pre-completed result with less interlockeds.  Beware!  Constructor calls the callback.
        // if a derived class ever uses this and overloads Cleanup, this may need to change
        internal LazyAsyncResult(object myObject, object myState, AsyncCallback myCallBack, object result)
        {
            GlobalLog.Assert(result != DBNull.Value, "LazyAsyncResult#{0}::.ctor()|Result can't be set to DBNull - it's a special internal value.", Logging.HashString(this));
            m_AsyncObject = myObject;
            m_AsyncState = myState;
            m_AsyncCallback = myCallBack;
            m_Result = result;
            m_IntCompleted = 1;

            if (m_AsyncCallback != null)
            {
                GlobalLog.Print("LazyAsyncResult#" + Logging.HashString(this) + "::Complete() invoking callback");
                m_AsyncCallback(this);
            }
            else
            {
                GlobalLog.Print("LazyAsyncResult#" + Logging.HashString(this) + "::Complete() no callback to invoke");
            }

            GlobalLog.Print("LazyAsyncResult#" + Logging.HashString(this) + "::.ctor() (pre-completed)");
        }

        // Interface method to return the original async object:
        internal object AsyncObject
        {
            get
            {
                return m_AsyncObject;
            }
        }

        // Interface method to return the caller's state object.
        public object AsyncState
        {
            get
            {
                return m_AsyncState;
            }
        }

        protected AsyncCallback AsyncCallback
        {
            get
            {
                return m_AsyncCallback;
            }

            set
            {
                m_AsyncCallback = value;
            }
        }

        // Interface property to return a WaitHandle that can be waited on for I/O completion.
        // This property implements lazy event creation.
        // the event object is only created when this property is accessed,
        // since we're internally only using callbacks, as long as the user is using
        // callbacks as well we will not create an event at all.
        // If this is used, the event cannot be disposed because it is under the control of the
        // application.  Internal should use InternalWaitForCompletion instead - never AsyncWaitHandle.
        public WaitHandle AsyncWaitHandle
        {
            get
            {
                GlobalLog.Print("LazyAsyncResult#" + Logging.HashString(this) + "::get_AsyncWaitHandle()");

#if DEBUG
                // Can't be called when state is protected.
                if (_ProtectState)
                {
                    throw new InvalidOperationException("get_AsyncWaitHandle called in protected state");
                }
#endif

                ManualResetEvent asyncEvent;

                // Indicates that the user has seen the event; it can't be disposed.
                m_UserEvent = true;

                // The user has access to this object.  Lock-in CompletedSynchronously.
                if (m_IntCompleted == 0)
                {
                    Interlocked.CompareExchange(ref m_IntCompleted, c_HighBit, 0);
                }

                // Because InternalWaitForCompletion() tries to dispose this event, it's
                // possible for m_Event to become null immediately after being set, but only if
                // IsCompleted has become true.  Therefore it's possible for this property
                // to give different (set) events to different callers when IsCompleted is true.
                asyncEvent = (ManualResetEvent)m_Event;
                while (asyncEvent == null)
                {
                    LazilyCreateEvent(out asyncEvent);
                }

                GlobalLog.Print("LazyAsyncResult#" + Logging.HashString(this) + "::get_AsyncWaitHandle() m_Event:" + Logging.HashString(m_Event));
                return asyncEvent;
            }
        }

        // Returns true if this call created the event.
        // May return with a null handle.  That means it thought it got one, but it was disposed in the mean time.
        private bool LazilyCreateEvent(out ManualResetEvent waitHandle)
        {
            // lazy allocation of the event:
            // if this property is never accessed this object is never created
            waitHandle = new ManualResetEvent(false);
            try
            {
                if (Interlocked.CompareExchange(ref m_Event, waitHandle, null) == null)
                {
                    if (InternalPeekCompleted)
                    {
                        waitHandle.Set();
                    }
                    return true;
                }
                else
                {
                    waitHandle.Dispose();
                    waitHandle = (ManualResetEvent)m_Event;
                    // There's a chance here that m_Event became null.  But the only way is if another thread completed
                    // in InternalWaitForCompletion and disposed it.  If we're in InternalWaitForCompletion, we now know
                    // IsCompleted is set, so we can avoid the wait when waitHandle comes back null.  AsyncWaitHandle
                    // will try again in this case.
                    return false;
                }
            }
            catch
            {
                // This should be very rare, but doing this will reduce the chance of deadlock.
                m_Event = null;
                if (waitHandle != null)
                    waitHandle.Dispose();
                throw;
            }
        }

        // This allows ContextAwareResult to not let anyone trigger the CompletedSynchronously tripwire while the context is being captured.
        [Conditional("DEBUG")]
        protected void DebugProtectState(bool protect)
        {
#if DEBUG
            _ProtectState = protect;
#endif
        }

        // Interface property, returning synchronous completion status.
        public bool CompletedSynchronously
        {
            get
            {
                GlobalLog.Print("LazyAsyncResult#" + Logging.HashString(this) + "::get_CompletedSynchronously()");

#if DEBUG
                // Can't be called when state is protected.
                if (_ProtectState)
                {
                    throw new InvalidOperationException("get_CompletedSynchronously called in protected state");
                }
#endif

                // If this returns greater than zero, it means it was incremented by InvokeCallback before anyone ever saw it.
                int result = m_IntCompleted;
                if (result == 0)
                {
                    result = Interlocked.CompareExchange(ref m_IntCompleted, c_HighBit, 0);
                }
                GlobalLog.Print("LazyAsyncResult#" + Logging.HashString(this) + "::get_CompletedSynchronously() returns: " + ((result > 0) ? "true" : "false"));
                return result > 0;
            }
        }

        // Interface property, returning completion status.
        public bool IsCompleted
        {
            get
            {
                GlobalLog.Print("LazyAsyncResult#" + Logging.HashString(this) + "::get_IsCompleted()");

#if DEBUG
                // Can't be called when state is protected.
                if (_ProtectState)
                {
                    throw new InvalidOperationException("get_IsCompleted called in protected state");
                }
#endif

                // Look at just the low bits to see if it's been incremented.  If it hasn't, set the high bit
                // to show that it's been looked at.
                int result = m_IntCompleted;
                if (result == 0)
                {
                    result = Interlocked.CompareExchange(ref m_IntCompleted, c_HighBit, 0);
                }
                return (result & ~c_HighBit) != 0;
            }
        }

        // Use to see if something's completed without fixing CompletedSynchronously
        internal bool InternalPeekCompleted
        {
            get
            {
                return (m_IntCompleted & ~c_HighBit) != 0;
            }
        }

        // Internal property for setting the IO result.
        internal object Result
        {
            get
            {
                return m_Result == DBNull.Value ? null : m_Result;
            }
            set
            {
                // Ideally this should never be called, since setting
                // the result object really makes sense when the IO completes.
                //
                // But if the result was set here (as a preemptive error or for some other reason),
                // then the "result" parameter passed to InvokeCallback() will be ignored.
                //

                // It's an error to call after the result has been completed or with DBNull.
                GlobalLog.Assert(value != DBNull.Value, "LazyAsyncResult#{0}::set_Result()|Result can't be set to DBNull - it's a special internal value.", Logging.HashString(this));
                GlobalLog.Assert(!InternalPeekCompleted, "LazyAsyncResult#{0}::set_Result()|Called on completed result.", Logging.HashString(this));
                m_Result = value;
            }
        }

        internal bool EndCalled
        {
            get
            {
                return m_EndCalled;
            }
            set
            {
                m_EndCalled = value;
            }
        }

        // Internal property for setting the Win32 IO async error code.
        internal int ErrorCode
        {
            get
            {
                return m_ErrorCode;
            }
            set
            {
                m_ErrorCode = value;
            }
        }

        // A method for completing the IO with a result
        // and invoking the user's callback.
        // Used by derived classes to pass context into an overridden Complete().  Useful
        // for determining the 'winning' thread in case several may simultaneously call
        // the equivalent of InvokeCallback().
        protected void ProtectedInvokeCallback(object result, IntPtr userToken)
        {
            GlobalLog.Print("LazyAsyncResult#" + Logging.HashString(this) + "::ProtectedInvokeCallback() result = " +
                            (result is Exception ? ((Exception)result).Message : result == null ? "<null>" : result.ToString()) +
                            ", userToken:" + userToken.ToString());

            // Critical to disallow DBNull here - it could result in a stuck spinlock in WaitForCompletion.
            if (result == DBNull.Value)
            {
                throw new ArgumentNullException("result");
            }

#if DEBUG
            // Always safe to ask for the state now.
            _ProtectState = false;
#endif

            if ((m_IntCompleted & ~c_HighBit) == 0 && (Interlocked.Increment(ref m_IntCompleted) & ~c_HighBit) == 1)
            {
                // DBNull.Value is used to guarantee that the first caller wins,
                // even if the result was set to null.
                if (m_Result == DBNull.Value)
                    m_Result = result;

                // Does this need a memory barrier to be sure this thread gets the m_Event if it's set?  I don't think so
                // because the Interlockeds on m_IntCompleted/m_Event should serve as the barrier.
                ManualResetEvent asyncEvent = (ManualResetEvent)m_Event;
                if (asyncEvent != null)
                {
                    try
                    {
                        asyncEvent.Set();
                    }
                    catch (ObjectDisposedException)
                    {
                        // Simply ignore this exception - There is apparently a rare race condition
                        // where the event is disposed before the completion method is called.
                    }
                }

                Complete(userToken);
            }
        }

        // A method for completing the IO with a result
        // and invoking the user's callback.
        internal void InvokeCallback(object result)
        {
            ProtectedInvokeCallback(result, IntPtr.Zero);
        }

        // A method for completing the IO without a result
        // and invoking the user's callback.
        internal void InvokeCallback()
        {
            ProtectedInvokeCallback(null, IntPtr.Zero);
        }

        //
        //  MUST NOT BE CALLED DIRECTLY
        //  A protected method that does callback job and it is guaranteed to be called exactly once.
        //  A derived overriding method must call the base class somewhere or the completion is lost.
        //
        protected virtual void Complete(IntPtr userToken)
        {
#if !NET_PERF
            bool offloaded = false;
            ThreadContext threadContext = CurrentThreadContext;
            try
            {
                ++threadContext.m_NestedIOCount;
#else
            try
            {
#endif
                if (m_AsyncCallback != null)
                {
                    GlobalLog.Print("LazyAsyncResult#" + Logging.HashString(this) + "::Complete() invoking callback");

#if !NET_PERF
                    if (threadContext.m_NestedIOCount >= c_ForceAsyncCount)
                    {
                        GlobalLog.Print("LazyAsyncResult::Complete *** OFFLOADED the user callback ***");
                        Task.Factory.StartNew(WorkerThreadComplete, null);
                        offloaded = true;
                    }
                    else
#endif
                    {
                        m_AsyncCallback(this);
                    }
                }
                else
                {
                    GlobalLog.Print("LazyAsyncResult#" + Logging.HashString(this) + "::Complete() no callback to invoke");
                }
            }
            finally
            {
#if !NET_PERF
                --threadContext.m_NestedIOCount;

                // Never call this method unless interlocked m_IntCompleted check has succeeded (like in this case)
                if (!offloaded)
#endif
                {
                    Cleanup();
                }
            }
        }



#if !NET_PERF
        // Only called in the above method
        void WorkerThreadComplete(object state)
        {
            try
            {
                m_AsyncCallback(this);
            }
            finally
            {
                Cleanup();
            }
        }
#endif

        // Custom instance cleanup method.
        // Derived types override this method to release unmanaged resources associated with an IO request.
        protected virtual void Cleanup()
        {
#if TRACK_LAR
            _PendingResults.Remove(_MyIndex);
#endif
        }

        internal object InternalWaitForCompletion()
        {
            return WaitForCompletion(true);
        }

        /*
        internal object InternalWaitForCompletionNoSideEffects()
        {
            return WaitForCompletion(false);
        }
        */

        private object WaitForCompletion(bool snap)
        {
            ManualResetEvent waitHandle = null;
            bool createdByMe = false;
            bool complete = snap ? IsCompleted : InternalPeekCompleted;

            if (!complete)
            {
                // Not done yet, so wait:
                waitHandle = (ManualResetEvent)m_Event;
                if (waitHandle == null)
                {
                    createdByMe = LazilyCreateEvent(out waitHandle);
                }
            }

            if (waitHandle != null)
            {
                try
                {
                    GlobalLog.Print("LazyAsyncResult#" + Logging.HashString(this) + "::InternalWaitForCompletion() Waiting for completion m_Event#" + Logging.HashString(waitHandle));
                    waitHandle.WaitOne(Timeout.Infinite);
                }
                catch (ObjectDisposedException)
                {
                    // This can occur if this method is called from two different threads.
                    // This possibility is the trade-off for not locking.
                }
                finally
                {
                    // We also want to dispose the event although we can't unless we did wait on it here.
                    if (createdByMe && !m_UserEvent)
                    {
                        // Does m_UserEvent need to be volatile (or m_Event set via Interlocked) in order
                        // to avoid giving a user a disposed event?
                        ManualResetEvent oldEvent = (ManualResetEvent)m_Event;
                        m_Event = null;
                        if (!m_UserEvent)
                        {
                            oldEvent.Dispose();
                        }
                    }
                }
            }

            // A race condition exists because InvokeCallback sets m_IntCompleted before m_Result (so that m_Result
            // can benefit from the synchronization of m_IntCompleted).  That means you can get here before m_Result got
            // set (although rarely - once every eight hours of stress).  Handle that case with a spin-lock.

            SpinWait sw = new SpinWait();
            while (m_Result == DBNull.Value)
            {
                sw.SpinOnce();
            }

            GlobalLog.Print("LazyAsyncResult#" + Logging.HashString(this) + "::InternalWaitForCompletion() done: " +
                            (m_Result is Exception ? ((Exception)m_Result).Message : m_Result == null ? "<null>" : m_Result.ToString()));

            return m_Result;
        }

        // A general interface that is called to release unmanaged resources associated with the class.
        // It completes the result but doesn't do any of the notifications.
        internal void InternalCleanup()
        {
            if ((m_IntCompleted & ~c_HighBit) == 0 && (Interlocked.Increment(ref m_IntCompleted) & ~c_HighBit) == 1)
            {
                // Set no result so that just in case there are waiters, they don't hang in the spin lock.
                m_Result = null;
                Cleanup();
            }
        }
    }
}
