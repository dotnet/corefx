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
        public void ReadEvent(string logName, bool useQuery)
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

        [ConditionalTheory(typeof(Helpers), nameof(Helpers.SupportsEventLogs))]
        [InlineData("Microsoft-Windows-PowerShell/Operational", false)]
        [InlineData("Microsoft-Windows-PowerShell/Operational", true)]
        public void ReadEventPsh(string logName, bool useQuery)
        {
            if (PlatformDetection.IsWindows7) // Null events in PowerShell log
                return;

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

        [ConditionalFact(typeof(Helpers), nameof(Helpers.SupportsEventLogs))]
        public void Seek()
        {
            using (var eventLog = new EventLogReader("Application"))
            {
                var record = eventLog.ReadEvent();
                eventLog.Seek(record.Bookmark);
                eventLog.Seek(IO.SeekOrigin.Begin, 0);
                eventLog.Seek(IO.SeekOrigin.Current, 0);
                eventLog.Seek(IO.SeekOrigin.End, 0);
            }
        }


        [ConditionalFact(typeof(Helpers), nameof(Helpers.SupportsEventLogs))]
        public void CancelReading()
        {
            using (var eventLog = new EventLogReader("Application"))
            {
                eventLog.CancelReading();
            }
        }

        [ConditionalFact(typeof(Helpers), nameof(Helpers.SupportsEventLogs))]
        public void LogStatus()
        {
            using (var eventLog = new EventLogReader("Application"))
            {
                Assert.NotEmpty(eventLog.LogStatus);
            }
        }
    }
}

