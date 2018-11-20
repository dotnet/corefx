// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics.Eventing.Reader;
using Xunit;

namespace System.Diagnostics.Tests
{
    public class EventLogReaderTests
    {
        [ConditionalTheory(typeof(Helpers), nameof(Helpers.SupportsEventLogs))]
        [InlineData("Application", false)]
        [InlineData("Application", true)]
        [InlineData("Microsoft-Windows-PowerShell/Operational", false)]
        [InlineData("Microsoft-Windows-PowerShell/Operational", true)]
        public void EventLog_Record(string logName, bool useQuery)
        {
            var eventLog =
                useQuery
                ? new EventLogReader(
                     new EventLogQuery(logName, PathType.LogName) { ReverseDirection = true })
                : new EventLogReader(logName);

            using (eventLog)
            {
                var record = eventLog.ReadEvent();
                Assert.NotNull(record);
                Assert.Equal(logName, record.LogName);
            }
        }
    }
}

