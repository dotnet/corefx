using System;
using System.Reflection;

namespace OnYourWayHome.ApplicationModel.Eventing
{
    public static class EventAggregatorExtensions
    {
        public static void PublishAsGeneric(this IEventAggregator aggregator, IEvent e)
        {
            Requires.NotNull(aggregator, "aggregator");
            Requires.NotNull(e, "e");

            // We need to call the generic Publish<TEvent> but we can't statically because 
            // we don't what TEvent is upfront. Use Reflection to call it, satisfying TEvent
            // with the type of the event.
            MethodInfo method = typeof(IEventAggregator).GetRuntimeMethod("Publish", new[] { typeof(IEvent) });  // Get definition Publish<TEvent>
            method = method.MakeGenericMethod(e.GetType());                // Get instantiated Publish<MyEvent>

            method.Invoke(aggregator, new object[] { e });
        }
    }
}
