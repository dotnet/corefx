//---------------------------------------------------------------------------------
// Copyright (c), Microsoft Corporation
//---------------------------------------------------------------------------------

namespace OnYourWayHome.ServiceBus.Messaging
{
    using System;
    using OnYourWayHome.AccessControl;

    internal sealed class CompleteMessageAsyncResult : ServiceBusRequestAsyncResult<bool>
    {
        private readonly BrokeredMessage message;

        public CompleteMessageAsyncResult(BrokeredMessage message, TokenProvider tokenProvider)
            : base(tokenProvider)
        {
            if (message == null)
            {
                throw new ArgumentNullException("message");
            }

            if (message.LockLocation == null)
            {
                throw new InvalidOperationException("The message cannot be completed, because it does not appear to be locked.");
            }

            this.message = message;
        }

        public BrokeredMessage Message
        {
            get { return this.message; }
        }

        protected override Uri Uri
        {
            get { return this.message.LockLocation; }
        }

        protected override string Method
        {
            get { return "DELETE"; }
        }
    }
}
