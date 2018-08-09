// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics.Eventing.Reader;
using Xunit;

namespace System.Diagnostics.Tests
{
    public class EventLogReaderTests
    {
        [ConditionalFact(typeof(Helpers), nameof(Helpers.SupportsEventLogs))]
        public void ApplicationEventLog_Record()
        {
            using (var eventLog = new EventLogReader("Application"))
            {
                var record = eventLog.ReadEvent();
                Assert.NotNull(record);
                Assert.Equal("Application", record.LogName);
            }
        }

        [ConditionalFact(typeof(Helpers), nameof(Helpers.SupportsEventLogs))]
        public void ApplicationEventLogQuery_Record()
        {
            var query = new EventLogQuery("Application", PathType.LogName) { ReverseDirection = true };
            using (var eventLog = new EventLogReader(query))
            {
                var record = eventLog.ReadEvent();
                Assert.NotNull(record);
                Assert.Equal("Application", record.LogName);
            }
        }

        [ConditionalFact(typeof(Helpers), nameof(Helpers.SupportsEventLogs))]
        public void NestedEventLog_Record()
        {
            using (var eventLog = new EventLogReader("Microsoft-Windows-PowerShell/Operational"))
            {
                var record = eventLog.ReadEvent();
                Assert.NotNull(record);
                Assert.Equal("Microsoft-Windows-PowerShell/Operational", record.LogName);
            }
        }

        [ConditionalFact(typeof(Helpers), nameof(Helpers.SupportsEventLogs))]
        public void NestedEventLogQuery_Record()
        {
            var query = new EventLogQuery("Microsoft-Windows-PowerShell/Operational", PathType.LogName) { ReverseDirection = true };
            using (var eventLog = new EventLogReader(query))
            {
                var record = eventLog.ReadEvent();
                Assert.NotNull(record);
                Assert.Equal("Microsoft-Windows-PowerShell/Operational", record.LogName);
            }
        }
    }
}

