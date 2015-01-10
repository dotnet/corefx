//---------------------------------------------------------------------------------
// Copyright (c), Microsoft Corporation
//---------------------------------------------------------------------------------

namespace OnYourWayHome.ServiceBus.Messaging
{
    using System;
    using OnYourWayHome.AccessControl;

    public sealed class MessageReceiver
    {
        private readonly string path;
        private readonly TokenProvider tokenProvider;

        public MessageReceiver(string path, TokenProvider tokenProvider)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentNullException("path");
            }

            if (tokenProvider == null)
            {
                throw new ArgumentNullException("tokenProvider");
            }

            this.path = path;
            this.tokenProvider = tokenProvider;

            this.Timeout = TimeSpan.FromSeconds(60);
            this.ReceiveMode = ReceiveMode.ReceiveAndDelete;
        }

        public string Path
        {
            get { return this.path; }
        }

        public TokenProvider TokenProvider
        {
            get { return this.tokenProvider; }
        }

        public TimeSpan Timeout { get; set; }

        public ReceiveMode ReceiveMode { get; set; }

        public IAsyncResult BeginReceive(AsyncCallback callback, object state)
        {
            return this.BeginReceive(this.Timeout, callback, state);
        }

        public IAsyncResult BeginReceive(TimeSpan timeout, AsyncCallback callback, object state)
        {
            return this.BeginReceive(timeout, this.ReceiveMode, callback, state);
        }

        public IAsyncResult BeginReceive(TimeSpan timeout, ReceiveMode receiveMode, AsyncCallback callback, object state)
        {
            ServiceBusRequestAsyncResult<BrokeredMessage> asyncResult;
            if (receiveMode == ReceiveMode.PeekLock)
            {
                asyncResult = new PeekLockMessageAsyncResult(this.path, timeout, this.tokenProvider);
            }
            else
            {
                asyncResult = new ReceiveAndDeleteMessageAsyncResult(this.path, timeout, this.tokenProvider);
            }

            asyncResult.BeginInvoke(callback, state);

            return asyncResult;
        }

        public BrokeredMessage EndReceive(IAsyncResult result)
        {
            var asyncResult = (ServiceBusRequestAsyncResult<BrokeredMessage>)result;
            return asyncResult.EndInvoke();
        }

        public IAsyncResult BeginAbandon(BrokeredMessage message, AsyncCallback callback, object state)
        {
            if (this.ReceiveMode == ReceiveMode.ReceiveAndDelete)
            {
                throw new InvalidOperationException("Only a Receiver in PeekLock mode can be used to Abandon messages.");
            }

            var asyncResult = new AbandonMessageAsyncResult(message, this.TokenProvider);
            asyncResult.BeginInvoke(callback, state);

            return asyncResult;
        }

        public void EndAbandon(IAsyncResult result)
        {
            var asyncResult = (AbandonMessageAsyncResult)result;
            asyncResult.EndInvoke();
        }

        public IAsyncResult BeginComplete(BrokeredMessage message, AsyncCallback callback, object state)
        {
            if (this.ReceiveMode == ReceiveMode.ReceiveAndDelete)
            {
                throw new InvalidOperationException("Only a Receiver in PeekLock mode can be used to Complete messages.");
            }

            var asyncResult = new CompleteMessageAsyncResult(message, this.TokenProvider);
            asyncResult.BeginInvoke(callback, state);

            return asyncResult;
        }

        public void EndComplete(IAsyncResult result)
        {
            var asyncResult = (CompleteMessageAsyncResult)result;
            asyncResult.EndInvoke();
        }
    }
}
