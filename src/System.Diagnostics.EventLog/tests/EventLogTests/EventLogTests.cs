using Xunit;

namespace System.Diagnostics.Tests
{
    public class EventLogTests : EventLogTestsBase
    {
        [ConditionalFact(typeof(PlatformDetection), nameof(PlatformDetection.IsNotWindowsNanoServer))]
        public void EventLogReIntializationException()
        {
            EventLog eventLog = new EventLog();
            eventLog.BeginInit();
            Assert.Throws<InvalidOperationException>(() => eventLog.BeginInit());
            eventLog.Close();
        }

        //[ConditionalFact(nameof(IsProcessElevated))]
        [ConditionalFact(typeof(PlatformDetection), nameof(PlatformDetection.IsNotWindowsNanoServer))]
        public void ClearLogTest()
        {
            if (!EventLog.SourceExists("MySource"))
            {
                EventLog.CreateEventSource("MySource", "MyNewLog");
            }

            EventLog myLog = new EventLog();
            myLog.Source = "MySource";
            myLog.WriteEntry("Writing to event log.");
            Assert.Equal(1, myLog.Entries.Count);
            myLog.Clear();
            Assert.Equal(0, myLog.Entries.Count);
        }

        [ConditionalFact(typeof(PlatformDetection), nameof(PlatformDetection.IsNotWindowsNanoServer))]
        public void ApplicationEventLog_Count()
        {
            EventLog ael = new EventLog("Application");
            Assert.InRange(ael.Entries.Count, 1, Int32.MaxValue);
        }

        //[ConditionalFact(nameof(IsProcessElevated))]
        [ConditionalFact(typeof(PlatformDetection), nameof(PlatformDetection.IsNotWindowsNanoServer))]
        public void DelteLogTest()
        {
            string source = Guid.NewGuid().ToString("N");
            string logName;
            if (!EventLog.SourceExists(source))
            {
                EventLog.CreateEventSource(source, "MyLog");
            }
            Assert.True(EventLog.Exists("MyLog"));

            if (EventLog.SourceExists(source))
            {
                logName = EventLog.LogNameFromSourceName(source, ".");
                EventLog.Delete(logName);
                Assert.False(EventLog.Exists(logName));
            }
        }
    }
}
