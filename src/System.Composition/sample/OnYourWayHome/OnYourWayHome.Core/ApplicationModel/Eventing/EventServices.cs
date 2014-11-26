using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection;
using System.Text;

namespace OnYourWayHome.ApplicationModel.Eventing
{
    internal class EventServices
    {
        public static EventHandler MakeWeak(EventHandler handler, Action<EventHandler> remove)
        {
            var reference = new WeakReference(handler.Target);
            var method = handler.GetMethodInfo();

            EventHandler newHandler = null;
            newHandler = (sender, e) =>
            {
                var target = reference.Target;
                if (target != null)
                {
                    method.Invoke(target, new object[] { sender, e });
                }
                else
                {
                    // Collected, unhook us
                    remove(newHandler);
                }
            };

            return newHandler;
        }

        public static NotifyCollectionChangedEventHandler MakeWeak(NotifyCollectionChangedEventHandler handler, Action<NotifyCollectionChangedEventHandler> remove)
        {
            var reference = new WeakReference(handler.Target);
            var method = handler.GetMethodInfo();

            NotifyCollectionChangedEventHandler newHandler = null;
            newHandler = (sender, e) =>
            {
                var target = reference.Target;
                if (target != null)
                {
                    method.Invoke(target, new object[] { sender, e });
                }
                else
                {   
                    // Collected, unhook us
                    remove(newHandler);
                }
            };

            return newHandler;
        }
    }
}
