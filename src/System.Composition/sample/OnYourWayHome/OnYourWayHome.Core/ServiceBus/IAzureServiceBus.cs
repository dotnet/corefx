using System;
using OnYourWayHome.ApplicationModel.Eventing;
using OnYourWayHome.ServiceBus.Messaging;

namespace OnYourWayHome.ServiceBus
{
    public interface IAzureServiceBus
    {
        void Send(BrokeredMessage message);

        event EventHandler<MessageReceivedEventArgs> MessageReceived;
    }
}
