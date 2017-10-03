using Xunit;

namespace System.Diagnostics.Tests
{
    public class EventLogSourceCreationTests : EventLogTestsBase
    {
        [ConditionalFact(typeof(PlatformDetection), nameof(PlatformDetection.IsNotWindowsNanoServer))]
        public void CheckSourceExistenceAndDeletion()
        {
            if (!AdminHelpers.IsProcessElevated())
                return;

            string source = Guid.NewGuid().ToString("N");
            try
            {
                EventLog.CreateEventSource(source, "MyNewLog");
                Assert.True(EventLog.SourceExists(source));
            }
            finally
            {
                EventLog.DeleteEventSource(source);
            }
            Assert.False(EventLog.SourceExists(source));
        }

        [ConditionalFact(typeof(PlatformDetection), nameof(PlatformDetection.IsNotWindowsNanoServer))]
        public void CheckSourceExistsArgumentNull()
        {
            if (!AdminHelpers.IsProcessElevated())
                return;

            Assert.False(EventLog.SourceExists(null));
        }

        [ConditionalFact(typeof(PlatformDetection), nameof(PlatformDetection.IsNotWindowsNanoServer))]
        public void DeleteUnregisteredSource()
        {
            if (!AdminHelpers.IsProcessElevated())
                return;

            Assert.Throws<System.ArgumentException>(() => EventLog.DeleteEventSource(Guid.NewGuid().ToString("N")));
        }
    }
}
