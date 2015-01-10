using System;
using System.Collections.Generic;
using OnYourWayHome.ApplicationModel;
using OnYourWayHome.ApplicationModel.Composition;
using OnYourWayHome.ApplicationModel.Eventing;
using OnYourWayHome.ServiceBus.Messaging;
using OnYourWayHome.ServiceBus.Serialization;

namespace OnYourWayHome.ServiceBus.Eventing.Parts
{
    // An IEventHandler<TEvent> that is responsible for listening to events from
    // the aggregator and publishing them to the service bus and vice versa
    public class AzureServiceBusEventHandler : DisposableObject, IStartupService, IEventHandler<IEvent>
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly IAzureServiceBus _serviceBus;
        private readonly IAzureEventSerializer _eventSerializer;
        private readonly Dictionary<IEvent, object> _incomingEvents = new Dictionary<IEvent, object>();

        public AzureServiceBusEventHandler(IEventAggregator eventAggregator, IAzureServiceBus serviceBus, IAzureEventSerializer eventSerializer)
        {
            Requires.NotNull(eventAggregator, "eventAggregator");
            Requires.NotNull(serviceBus, "serviceBus");
            Requires.NotNull(eventSerializer, "eventSerializer");

            _eventAggregator = eventAggregator;
            _eventAggregator.SubscribeAll(this);
            _serviceBus = serviceBus;
            _serviceBus.MessageReceived += OnMessageReceived;
            _eventSerializer = eventSerializer;
        }

        public void Handle(IEvent e)
        {
            Requires.NotNull(e, "e");

            // This event came from the service bus, ignore it
            if (_incomingEvents.Remove(e))
                return;

            BrokeredMessage message = _eventSerializer.Serialize(e);

            _serviceBus.Send(message);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _eventAggregator.UnsubscribeAll(this);
                _serviceBus.MessageReceived -= OnMessageReceived;
            }
        }

        private void OnMessageReceived(object sender, MessageReceivedEventArgs args)
        {
            IEvent e = _eventSerializer.Deserialize(args.Message);

            // Add to the incoming events lookup so that we
            // don't send it to the bus multiple times
            _incomingEvents.Add(e, null);
            _eventAggregator.PublishAsGeneric(e);
        }
    }
}


