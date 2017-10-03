using Xunit;

namespace System.Diagnostics.Tests
{
    public class EventLogTests : EventLogTestsBase
    {
        [ConditionalFact(typeof(PlatformDetection), nameof(PlatformDetection.IsNotWindowsNanoServer))]
        public void EventLogReinitializationException()
        {
            using (EventLog eventLog = new EventLog())
            {
                eventLog.BeginInit();
                Assert.Throws<InvalidOperationException>(() => eventLog.BeginInit());
                eventLog.Close();
            }
        }

        [ConditionalFact(typeof(PlatformDetection), nameof(PlatformDetection.IsNotWindowsNanoServer))]
        public void ClearLogTest()
        {
            if (!AdminHelpers.IsProcessElevated())
                return;

            string source = Guid.NewGuid().ToString("N");
            EventLog.CreateEventSource(source, "MyNewLog");
            using (EventLog myLog = new EventLog())
            {
                myLog.Source = source;
                myLog.WriteEntry("Writing to event log.");
                Assert.True(myLog.Entries.Count != 0);
                myLog.Clear();
                Assert.Equal(0, myLog.Entries.Count);
            }
        }

        [ConditionalFact(typeof(PlatformDetection), nameof(PlatformDetection.IsNotWindowsNanoServer))]
        public void ApplicationEventLog_Count()
        {
            EventLog ael = new EventLog("Application");
            Assert.InRange(ael.Entries.Count, 1, Int32.MaxValue);
        }

        [ConditionalFact(typeof(PlatformDetection), nameof(PlatformDetection.IsNotWindowsNanoServer))]
        public void DelteLogTest()
        {
            if (!AdminHelpers.IsProcessElevated())
                return;

            string source = Guid.NewGuid().ToString("N");
            string logName = Guid.NewGuid().ToString("N");
            EventLog.CreateEventSource(source, logName);
            Assert.True(EventLog.Exists(logName));
            EventLog.Delete(logName);
            Assert.False(EventLog.Exists(logName));
        }
    }
}
