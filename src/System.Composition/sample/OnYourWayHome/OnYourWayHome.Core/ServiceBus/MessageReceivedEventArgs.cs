using System;
using OnYourWayHome.ServiceBus;
using OnYourWayHome.ServiceBus.Messaging;

namespace OnYourWayHome.ApplicationModel.Eventing
{
    // Contains data for IAzureServiceBus.MessageReceived event
    public class MessageReceivedEventArgs : EventArgs
    {
        private readonly BrokeredMessage _message;

        public MessageReceivedEventArgs(BrokeredMessage message)
        {
            Requires.NotNull(message, "message");

            _message = message;
        }

        public BrokeredMessage Message
        {
            get { return _message; }
        }
    }
}
