// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading;

namespace System.IO.Compression {

    internal class DeflateStreamAsyncResult : IAsyncResult {
        public byte[] buffer;
        public int offset;
        public int count;
#pragma warning disable 0649
        public bool isWrite;
#pragma warning restore 0649

        private object m_AsyncObject;               // Caller's async object.
        private object m_AsyncState;                // Caller's state object.
        private AsyncCallback m_AsyncCallback;      // Caller's callback method.

        private object m_Result;                     // Final IO result to be returned byt the End*() method.
        internal bool m_CompletedSynchronously;      // true if the operation completed synchronously.
        private int m_InvokedCallback;               // 0 is callback is not called
        private int m_Completed;                     // 0 if not completed >0 otherwise.
        private object m_Event;                      // lazy allocated event to be returned in the IAsyncResult for the client to wait on

        public DeflateStreamAsyncResult(object asyncObject, object asyncState,
                                   AsyncCallback asyncCallback,
                                   byte[] buffer, int offset, int count)
        {

            this.buffer = buffer;
            this.offset = offset;
            this.count = count;
            m_CompletedSynchronously = true;
            m_AsyncObject = asyncObject;
            m_AsyncState = asyncState;
            m_AsyncCallback = asyncCallback;
        }

        // Interface method to return the caller's state object.
        public object AsyncState
        {
            get
            {
                return m_AsyncState;
            }
        }

        // Interface property to return a WaitHandle that can be waited on for I/O completion.
        // This property implements lazy event creation.
        // the event object is only created when this property is accessed,
        // since we're internally only using callbacks, as long as the user is using
        // callbacks as well we will not create an event at all.
        public WaitHandle AsyncWaitHandle
        {
            get
            {
                // save a copy of the completion status
                int savedCompleted = m_Completed;
                if (m_Event == null)
                {
                    // lazy allocation of the event:
                    // if this property is never accessed this object is never created
                    Interlocked.CompareExchange(ref m_Event, new ManualResetEvent(savedCompleted != 0), null);
                }

                ManualResetEvent castedEvent = (ManualResetEvent)m_Event;
                if (savedCompleted == 0 && m_Completed != 0)
                {
                    // if, while the event was created in the reset state,
                    // the IO operation completed, set the event here.
                    castedEvent.Set();
                }
                return castedEvent;
            }
        }

        // Interface property, returning synchronous completion status.
        public bool CompletedSynchronously
        {
            get
            {
                return m_CompletedSynchronously;
            }
        }

        // Interface property, returning completion status.
        public bool IsCompleted
        {
            get
            {
                return m_Completed != 0;
            }
        }

        // Internal property for setting the IO result.
        internal object Result
        {
            get
            {
                return m_Result;
            }
        }

        internal void Close()
        {
            if (m_Event != null)
            {
                // Uncomment when WaitHandler.Close is available
                //((ManualResetEvent)m_Event).Close();
            }
        }

        internal void InvokeCallback(bool completedSynchronously, object result)
        {
            Complete(completedSynchronously, result);
        }

        internal void InvokeCallback(object result)
        {
            Complete(result);
        }

        // Internal method for setting completion.
        // As a side effect, we'll signal the WaitHandle event and clean up.
        private void Complete(bool completedSynchronously, object result)
        {
            m_CompletedSynchronously = completedSynchronously;
            Complete(result);
        }

        private void Complete(object result)
        {
            m_Result = result;

            // Set IsCompleted and the event only after the usercallback method. 
            Interlocked.Increment(ref m_Completed);

            if (m_Event != null)
            {
                ((ManualResetEvent)m_Event).Set();
            }

            if (Interlocked.Increment(ref m_InvokedCallback) == 1)
            {
                if (m_AsyncCallback != null)
                {
                    m_AsyncCallback(this);
                }
            }
        }

    }

}

