// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
#if USE_MDT_EVENTSOURCE
using Microsoft.Diagnostics.Tracing;
#else
using System.Diagnostics.Tracing;
#endif
using Xunit;

namespace BasicEventSourceTests
{
    public class EventListenerListener : Listener
    {
        private readonly EventListener _listener;
        private Action<EventSource> _onEventSourceCreated;

        public event EventHandler<EventSourceCreatedEventArgs> EventSourceCreated
        {
            add
            {
                if (this._listener != null)
                    this._listener.EventSourceCreated += value;
            }
            remove
            {
                if (this._listener != null)
                    this._listener.EventSourceCreated -= value;
            }
        }

        public event EventHandler<EventWrittenEventArgs> EventWritten
        {
            add
            {
                if (this._listener != null)
                    this._listener.EventWritten += value;
            }
            remove
            {
                if (this._listener != null)
                    this._listener.EventWritten -= value;
            }
        }

        public EventListenerListener(bool useEventsToListen = false)
        {
            if (useEventsToListen)
            {
                _listener = new HelperEventListener(null);
                _listener.EventSourceCreated += (sender, eventSourceCreatedEventArgs)
                    => _onEventSourceCreated?.Invoke(eventSourceCreatedEventArgs.EventSource);
                _listener.EventWritten += mListenerEventWritten;
            }
            else
            {
                _listener = new HelperEventListener(this);
            }
        }

        public override void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            _disposed = true;
            EventTestHarness.LogWriteLine("Disposing Listener");
            _listener.Dispose();
        }

        private void DoCommand(EventSource source, EventCommand command, FilteringOptions options)
        {
            if (command == EventCommand.Enable)
                _listener.EnableEvents(source, options.Level, options.Keywords, options.Args);
            else if (command == EventCommand.Disable)
                _listener.DisableEvents(source);
            else
                throw new NotImplementedException();
        }

        public override void EventSourceCommand(string eventSourceName, EventCommand command, FilteringOptions options = null)
        {
            EventTestHarness.LogWriteLine("Sending command {0} to EventSource {1} Options {2}", eventSourceName, command, options);

            if (options == null)
                options = new FilteringOptions();

            foreach (EventSource source in EventSource.GetSources())
            {
                if (source.Name == eventSourceName)
                {
                    DoCommand(source, command, options);
                    return;
                }
            }

            _onEventSourceCreated += delegate (EventSource sourceBeingCreated)
            {
                if (eventSourceName != null && eventSourceName == sourceBeingCreated.Name)
                {
                    DoCommand(sourceBeingCreated, command, options);
                    eventSourceName = null;         // so we only do it once.  
                }
            };
        }

        private void mListenerEventWritten(object sender, EventWrittenEventArgs eventData)
        {
            OnEvent(new EventListenerEvent(eventData));
        }

        private class HelperEventListener : EventListener
        {
            private readonly EventListenerListener _forwardTo;

            public HelperEventListener(EventListenerListener forwardTo)
            {
                _forwardTo = forwardTo;
            }

            protected override void OnEventSourceCreated(EventSource eventSource)
            {
                base.OnEventSourceCreated(eventSource);

                _forwardTo?._onEventSourceCreated?.Invoke(eventSource);
            }

            protected override void OnEventWritten(EventWrittenEventArgs eventData)
            {
                // OnEventWritten is abstract in netfx <= 461
                base.OnEventWritten(eventData);
                _forwardTo?.OnEvent?.Invoke(new EventListenerEvent(eventData));
            }
        }

        /// <summary>
        /// EtwEvent implements the 'Event' abstraction for TraceListene events (it has a EventWrittenEventArgs in it) 
        /// </summary>
        internal class EventListenerEvent : Event
        {
            private readonly EventWrittenEventArgs _data;

            public override bool IsEventListener { get { return true; } }

            public override string ProviderName { get { return _data.EventSource.Name; } }

            public override string EventName { get { return _data.EventName; } }

            public override IList<string> PayloadNames { get { return _data.PayloadNames; } }

            public override int PayloadCount
            {
                get { return _data.Payload?.Count ?? 0; }
            }

            internal EventListenerEvent(EventWrittenEventArgs data)
            {
                _data = data;
            }

            public override object PayloadValue(int propertyIndex, string propertyName)
            {
                if (propertyName != null)
                    Assert.Equal(propertyName, _data.PayloadNames[propertyIndex]);

                return _data.Payload[propertyIndex];
            }
        }

        private bool _disposed;
    }
}
