using System.Diagnostics;
using Xunit;
using System;

namespace System.Diagnostics.Tests
{
    public class EventLogSourceCreationTests : EventLogTestsBase
    {
        [ConditionalFact(nameof(IsProcessElevated))]
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

        [ConditionalFact(nameof(IsProcessElevated))]
        public void CheckSourceExistsArgumentNull()
        {
            Assert.False(EventLog.SourceExists(null));
        }

        [ConditionalFact(nameof(IsProcessElevated))]
        public void DeleteUnregisteredSource()
        {
            Assert.Throws<System.ArgumentException>(() => EventLog.DeleteEventSource(Guid.NewGuid().ToString("N")));
        }
    }
}
