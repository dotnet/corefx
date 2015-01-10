using System;

namespace OnYourWayHome.ApplicationModel.Eventing
{
    public interface IEventAggregator
    {
        void Publish<TEvent>(TEvent e)
            where TEvent : IEvent;

        void Subscribe<TEvent>(IEventHandler<TEvent> handler)
            where TEvent : IEvent;

        void SubscribeAll(IEventHandler<IEvent> handler);

        void Unsubscribe<TEvent>(IEventHandler<TEvent> handler)
            where TEvent : IEvent;

        void UnsubscribeAll(IEventHandler<IEvent> handler);
    }
}
