//---------------------------------------------------------------------------------
// Copyright (c), Microsoft Corporation
//---------------------------------------------------------------------------------

namespace OnYourWayHome.ServiceBus
{
    using System;
    using System.Threading;

    internal abstract class AsyncResult<T> : IAsyncResult
    {
        private readonly ManualResetEvent asyncWaitHandle = new ManualResetEvent(false);

        public T Result { get; protected set; }

        public Exception Exception { get; set; }

        public AsyncCallback Callback { get; protected set; }

        public object AsyncState { get; protected set; }

        public WaitHandle AsyncWaitHandle
        {
            get { return this.asyncWaitHandle; }
        }

        public bool CompletedSynchronously { get; private set; }

        public bool IsCompleted { get; private set; }

        public virtual void BeginInvoke(AsyncCallback callback, object state)
        {
            this.Callback = callback;
            this.AsyncState = state;
        }

        public virtual T EndInvoke()
        {
            if (!this.IsCompleted)
            {
                this.asyncWaitHandle.WaitOne();
            }

            this.asyncWaitHandle.Dispose();

            if (this.Exception != null)
            {
                throw this.Exception;
            }

            return this.Result;
        }

        protected virtual void SetCompleted(Exception exception, bool completedSyncronously)
        {
            this.Exception = exception;
            this.CompletedSynchronously = completedSyncronously;

            this.IsCompleted = true;
            this.asyncWaitHandle.Set();

            // If a callback method was set, call it
            if (this.Callback != null)
            {
                this.Callback(this);
            }
        }
    }
}
