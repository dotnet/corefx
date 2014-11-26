using System;
using System.IO;
using OnYourWayHome.ApplicationModel.Eventing;
using OnYourWayHome.ServiceBus.Messaging;

namespace OnYourWayHome.ServiceBus.Serialization.Parts
{
    // An IAzureEventSerializer that serializes and deserializes an event to a BrokeredMessage using the 
    // DataContractSerializer
    //
    // This class wraps a serialized version of an IEvent in an object wrapper, EventMessage that contains 
    // the fully qualified type of the IEvent. This enables to reconstitute into the appropriate type when 
    // pull it down it from cloud. We needed to do this instead of some of the built-in ways for the 
    // following reason:
    //
    // - KnownTypeAttribute requires you to know the types unfront.
    // - NetDataContractSerializer (which serializes fully-qualified type names) is only available on the 
    //   .NET Framework.
    // - DataContractSerializer's DataContractResolver (which enables us to get called back for each type) 
    //   is only available on .NET Framework.
    public partial class DataContractAzureEventSerializer : IAzureEventSerializer
    {
        private readonly IAzureServiceBusConfiguration _configuration;

        public DataContractAzureEventSerializer(IAzureServiceBusConfiguration configuration)
        {
            Requires.NotNull(configuration, "configuration");

            _configuration = configuration;
        }

        public IEvent Deserialize(BrokeredMessage message)
        {
            Requires.NotNull(message, "message");

            return DeserializeFromStream(message.BodyStream);
        }

        public BrokeredMessage Serialize(IEvent e)
        {
            Requires.NotNull(e, "event");

            Stream stream = SerializeAsStream(e);

            BrokeredMessage message = new BrokeredMessage(stream);

            // Always add the device id so that we can filter out our own device's messages
            message.SetMessageProperty("X-MS-PROP-" + WellKnownProperty.DeviceId, _configuration.DeviceId);

            return message;
        }

        private Stream SerializeAsStream(IEvent e)
        {
            EventMessage message = new EventMessage();
            message.AssemblyQualifiedTypeName = e.GetType().AssemblyQualifiedName;
            message.Data = DataContractServices.SerializeAsBytes(e);

            return DataContractServices.SerializeAsStream(message);
        }

        private IEvent DeserializeFromStream(Stream stream)
        {
            EventMessage message = DataContractServices.DeserializeFromStream<EventMessage>(stream);
            Type type = Type.GetType(message.AssemblyQualifiedTypeName);

            return (IEvent)DataContractServices.DeserializeFromBytes(type, message.Data);
        }
    }
}
