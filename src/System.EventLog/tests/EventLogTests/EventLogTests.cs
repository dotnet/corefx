using System.Diagnostics;
using Xunit;
using System;

namespace System.Diagnostics.Tests
{
    public class EventLogTests : EventLogTestsBase
    {
        [Fact]
        public void EventLogReIntializationException()
        {
            EventLog eventLog = new EventLog();
            eventLog.BeginInit();
            Assert.Throws<InvalidOperationException>(() => eventLog.BeginInit());
            eventLog.Close();
        }

        [ConditionalFact(nameof(IsProcessElevated))]
        public void CLearLogTest()
        {
            if (!EventLog.SourceExists("MySource"))
            {
                EventLog.CreateEventSource("MySource", "MyNewLog");
            }

            EventLog myLog = new EventLog();
            myLog.Source = "MySource";
            myLog.WriteEntry("Writing to event log.");
            myLog.Clear();
            Assert.Equal(0, myLog.Entries.Count);
        }
    }
}
