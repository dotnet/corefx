using Xunit;

namespace System.Diagnostics.Tests
{
    public class EventLogSourceCreationTests : EventLogTestsBase
    {
        //[ConditionalFact(nameof(IsProcessElevated))]
        [ConditionalFact(typeof(PlatformDetection), nameof(PlatformDetection.IsNotWindowsNanoServer))]
        public void CheckSourceExistanceAndDeletion()
        {
            string source = Guid.NewGuid().ToString("N");
            if (!EventLog.SourceExists(source))
            {
                EventLog.CreateEventSource(source, "MyNewLog");
            }
            Assert.True(EventLog.SourceExists(source));
            EventLog.DeleteEventSource(source);
            Assert.False(EventLog.SourceExists(source));
        }

        //[ConditionalFact(nameof(IsProcessElevated))]
        [ConditionalFact(typeof(PlatformDetection), nameof(PlatformDetection.IsNotWindowsNanoServer))]
        public void CheckSourceExistsArgumentNull()
        {
            Assert.False(EventLog.SourceExists(null));
        }

        //[ConditionalFact(nameof(IsProcessElevated))]
        [ConditionalFact(typeof(PlatformDetection), nameof(PlatformDetection.IsNotWindowsNanoServer))]
        public void DeleteUnregisteredSource()
        {
            Assert.Throws<System.ArgumentException>(() => EventLog.DeleteEventSource(Guid.NewGuid().ToString("N")));
        }
    }
}
