// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics.Eventing.Reader;
using System.Threading;
using Xunit;

namespace System.Diagnostics.Tests
{
    public class EventLogWatcherTests
    {
        static AutoResetEvent signal;
        private const string message = "EventRecordWrittenTestMessage";
        private int eventCounter;

        public void RaisingEvent(string log, string methodName, bool waitOnEvent = true)
        {
            signal = new AutoResetEvent(false);
            eventCounter = 0;
            string source = "Source_" + methodName;

            try
            {
                EventLog.CreateEventSource(source, log);
                var query = new EventLogQuery(log, PathType.LogName);
                using (EventLog eventLog = new EventLog())
                using (EventLogWatcher eventLogWatcher = new EventLogWatcher(query))
                {
                    eventLog.Source = source;
                    eventLogWatcher.EventRecordWritten += (s, e) =>
                    {
                        eventCounter += 1;
                        signal.Set();
                    };
                    Helpers.RetryOnWin7(() => eventLogWatcher.Enabled = waitOnEvent);
                    Helpers.RetryOnWin7(() => eventLog.WriteEntry(message, EventLogEntryType.Information));
                    if (waitOnEvent)
                    {
                        Assert.True(signal.WaitOne(6000));
                    }
                }
            }
            finally
            {
                EventLog.DeleteEventSource(source);
                Helpers.RetryOnWin7(() => EventLog.Delete(log));
            }
        }

        [ConditionalFact(typeof(Helpers), nameof(Helpers.IsElevatedAndSupportsEventLogs))]
        public void RecordWrittenEventRaised()
        {
            RaisingEvent("EnableEvent", nameof(RecordWrittenEventRaised));
            Assert.NotEqual(0, eventCounter);
        }

        [ConditionalFact(typeof(Helpers), nameof(Helpers.IsElevatedAndSupportsEventLogs))]
        public void RecordWrittenEventRaiseDisable()
        {
            RaisingEvent("DisableEvent", nameof(RecordWrittenEventRaiseDisable), waitOnEvent: false);
            Assert.Equal(0, eventCounter);
        }
    }
}
