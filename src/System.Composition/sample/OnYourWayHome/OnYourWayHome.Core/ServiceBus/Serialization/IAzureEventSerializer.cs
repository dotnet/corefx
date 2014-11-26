using System;
using OnYourWayHome.ApplicationModel.Eventing;
using OnYourWayHome.ServiceBus.Serialization;
using OnYourWayHome.ServiceBus.Messaging;

namespace OnYourWayHome.ServiceBus.Serialization
{
    // Serializers an IEvent to and from a BrokeredMessage;
    public interface IAzureEventSerializer
    {
        BrokeredMessage Serialize(IEvent e);

        IEvent Deserialize(BrokeredMessage message);
    }
}
