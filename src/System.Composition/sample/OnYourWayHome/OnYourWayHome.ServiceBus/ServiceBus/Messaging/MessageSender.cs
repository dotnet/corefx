//---------------------------------------------------------------------------------
// Copyright (c), Microsoft Corporation
//---------------------------------------------------------------------------------

namespace OnYourWayHome.ServiceBus.Messaging
{
    using System;
    using OnYourWayHome.AccessControl;

    public sealed class MessageSender
    {
        private readonly string path;
        private readonly TokenProvider tokenProvider;

        public MessageSender(string path, TokenProvider tokenProvider)
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
        }

        public string Path
        {
            get { return this.path; }
        }

        public TokenProvider TokenProvider
        {
            get { return this.tokenProvider; }
        }

        public IAsyncResult BeginSend(BrokeredMessage message, AsyncCallback callback, object state)
        {
            var asyncResult = new SendMessageAsyncResult(this.path, message, this.tokenProvider);
            asyncResult.BeginInvoke(callback, state);

            return asyncResult;
        }

        public void EndSend(IAsyncResult result)
        {
            var asyncResult = (SendMessageAsyncResult)result;
            asyncResult.EndInvoke();
        }
    }
}
