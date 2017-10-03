using Xunit;

namespace System.Diagnostics.Tests
{
    public class EventLogWriteEntryEventTests
    {
        [ConditionalFact(typeof(PlatformDetection), nameof(PlatformDetection.IsNotWindowsNanoServer))]
        public void WriteEntry()
        {
            string source = Guid.NewGuid().ToString("N");
            string message = Guid.NewGuid().ToString("N");
            EventLog.CreateEventSource(source, "MyNewLog");
            using (EventLog myLog = new EventLog())
            {
                myLog.Source = source;
                myLog.WriteEntry(message);
                Assert.Contains(message, myLog.Entries[myLog.Entries.Count - 1].Message);
            }
        }

        [ConditionalFact(typeof(PlatformDetection), nameof(PlatformDetection.IsNotWindowsNanoServer))]
        public void WriteEntryWithType()
        {
            string source = Guid.NewGuid().ToString("N");
            string message = Guid.NewGuid().ToString("N");
            EventLog.CreateEventSource(source, "MyNewLog");
            using (EventLog myLog = new EventLog())
            {
                myLog.Source = source;
                myLog.WriteEntry(message, EventLogEntryType.Warning);
                Assert.Contains(message, myLog.Entries[myLog.Entries.Count - 1].Message);
                Assert.True(EventLogEntryType.Warning == myLog.Entries[myLog.Entries.Count - 1].EntryType);
            }
        }
    }
}
