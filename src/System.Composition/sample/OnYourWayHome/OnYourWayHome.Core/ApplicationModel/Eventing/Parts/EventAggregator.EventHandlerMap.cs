using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace OnYourWayHome.ApplicationModel.Eventing.Parts
{
    partial class EventAggregator
    {
        private class EventHandlerMap
        {
            private readonly Dictionary<Type, List<object>> _handlerMap = new Dictionary<Type, List<object>>();

            public EventHandlerMap()
            {
            }

            public void Register<TEvent>(IEventHandler<TEvent> handler)
                where TEvent : IEvent
            {
                var handlers = GetOrCreateHandlers<TEvent>();
                handlers.Add(handler);
            }

            public void Unregister<TEvent>(IEventHandler<TEvent> handler)
                where TEvent : IEvent
            {
                var handlers = GetOrCreateHandlers<TEvent>();
                handlers.Remove(handler);
            }

            public IEnumerable<IEventHandler<TEvent>> GetHandlers<TEvent>()
                where TEvent : IEvent
            {
                IList<object> handlers = GetOrCreateHandlers<TEvent>();

                return handlers.Cast<IEventHandler<TEvent>>();
            }

            private IList<object> GetOrCreateHandlers<TEvent>()
                where TEvent : IEvent
            {
                // Note that our handlers typed as List<object> because there
                // is no common base or interface that all handlers share
                List<object> handlers;
                if (!_handlerMap.TryGetValue(typeof(TEvent), out handlers))
                {
                    handlers = new List<object>();
                    _handlerMap.Add(typeof(TEvent), handlers);
                }

                return handlers;
            }
        }
    }
}
