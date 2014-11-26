using System;
using System.Reflection;
using OnYourWayHome.ServiceBus;

namespace OnYourWayHome.ApplicationModel.Eventing.Parts
{
    // Provides the default implementation of IEventAggregator
    public partial class EventAggregator : IEventAggregator
    {
        private readonly EventHandlerMap _handlers = new EventHandlerMap();

        public EventAggregator()
        {
        }

        public void Publish<TEvent>(TEvent e)
            where TEvent : IEvent
        {
            // First notify handlers that have subscribed to this specific event
            foreach (IEventHandler<TEvent> handler in _handlers.GetHandlers<TEvent>())
            {
                handler.Handle(e);
            }

            // Next notify handlers that have subscribed for all events
            foreach (IEventHandler<IEvent> handler in _handlers.GetHandlers<IEvent>())
            {
                handler.Handle(e);
            }
        }

        public void SubscribeAll(IEventHandler<IEvent> handler)
        {
            Requires.NotNull(handler, "handler");

            _handlers.Register(handler);
        }

        public void Subscribe<TEvent>(IEventHandler<TEvent> handler)
            where TEvent : IEvent
        {
            Requires.NotNull(handler, "handler");

            _handlers.Register(handler);
        }

        public void UnsubscribeAll(IEventHandler<IEvent> handler)
        {
            Requires.NotNull(handler, "handler");

            _handlers.Unregister(handler);
        }

        public void Unsubscribe<TEvent>(IEventHandler<TEvent> handler)
            where TEvent : IEvent
        {
            Requires.NotNull(handler, "handler");

            _handlers.Unregister(handler);
        }
    }
}
