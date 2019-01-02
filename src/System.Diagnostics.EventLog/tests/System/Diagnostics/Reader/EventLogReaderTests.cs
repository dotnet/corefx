// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics.Eventing.Reader;
using System.Security.Principal;
using System.Reflection;
using System.Threading;
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
                EventRecord record = eventLog.ReadEvent();
                Assert.NotNull(record);
                Assert.Equal(logName, record.LogName);
            }
        }

        [ConditionalTheory(typeof(Helpers), nameof(Helpers.SupportsEventLogs))]
        [InlineData(true)]
        [InlineData(false)]
        public void WrongPathType_ReverseDirection_Throws(bool useBookmark)
        {
            if (PlatformDetection.IsWindows7) // Null events in PowerShell log
                return;
            var query = new EventLogQuery(null, PathType.FilePath, "*[System[(Level=2)]]") { ReverseDirection = true };
            if (useBookmark)
            {
                Assert.Throws<EventLogException>(() => new EventLogReader(query, bookmark: null));
                Assert.Throws<EventLogException>(() => new EventLogReader(query, bookmark: GetBookmark("Application", PathType.LogName)));
            }
            else
            {
                Assert.Throws<EventLogException>(() => new EventLogReader(query));
            }
        }

        [ConditionalTheory(typeof(Helpers), nameof(Helpers.SupportsEventLogs))]
        [InlineData(true)]
        [InlineData(false)]
        public void WrongPathType_TolerateQueryErrors_Throws(bool useBookmark)
        {
            if (PlatformDetection.IsWindows7) // Null events in PowerShell log
                return;
            var query = new EventLogQuery(null, PathType.FilePath, "*[System[(Level=2)]]") { TolerateQueryErrors = true };
            if (useBookmark)
            {
                Assert.Throws<EventLogException>(() => new EventLogReader(query, bookmark: null));
                Assert.Throws<EventLogException>(() => new EventLogReader(query, bookmark: GetBookmark("Application", PathType.LogName)));
            }
            else
            {
                Assert.Throws<EventLogException>(() => new EventLogReader(query));
            }
        }

        [ConditionalFact(typeof(Helpers), nameof(Helpers.SupportsEventLogs))]
        public void EventLogRecord_MethodsAndProtperties()
        {
            if (PlatformDetection.IsWindows7) // Null events in PowerShell log
                return;
            var query = new EventLogQuery("Application", PathType.LogName, "*[System]") { ReverseDirection = true };
            var eventLog = new EventLogReader(query, GetBookmark("Application", PathType.LogName));
            using (eventLog)
            {
                EventLogRecord record = (EventLogRecord)eventLog.ReadEvent();
                Assert.NotNull(record);
                    
                Assert.Throws<EventLogNotFoundException>(() => record.LevelDisplayName);
                Assert.Throws<EventLogNotFoundException>(() => record.KeywordsDisplayNames);
                Assert.Throws<EventLogNotFoundException>(() => record.OpcodeDisplayName);
                Assert.Throws<EventLogNotFoundException>(() => record.TaskDisplayName);

                Assert.Null(record.Version);
                Assert.Null(record.Opcode);
                Assert.Null(record.ProviderId);
                Assert.Null(record.ProcessId);
                Assert.Null(record.ThreadId);
                SecurityIdentifier userId = record.UserId;
                Guid? activityId = record.ActivityId;
                Assert.False(activityId.HasValue);
                Guid? relatedActivityId = record.RelatedActivityId;
                Assert.False(relatedActivityId.HasValue);
                
                Assert.NotEqual(0, record.Id);
                Assert.NotNull(record.Qualifiers);
                Assert.NotNull(record.Level);
                Assert.NotNull(record.Task);
                Assert.NotNull(record.Keywords);
                Assert.NotNull(record.RecordId);
                Assert.NotNull(record.ProviderName);
                Assert.NotNull(record.MachineName);
                Assert.NotNull(record.TimeCreated);
                Assert.NotNull(record.ContainerLog);
                Assert.NotNull(record.MatchedQueryIds);
                Assert.NotNull(record.Bookmark);

                Assert.NotNull(record.Properties);
                foreach (EventProperty eventProperty in record.Properties)
                {
                    Assert.NotNull(eventProperty.Value);
                }

                Assert.Throws<EventLogNotFoundException>(() => record.FormatDescription(new[] {"dummy"}));
                Assert.Null(record.FormatDescription());
                Assert.Throws<EventLogNotFoundException>(() => ((EventRecord)record).FormatDescription(new[] {"dummy"}));
                Assert.Null(((EventRecord)record).FormatDescription(null));
                Assert.Null(((EventRecord)record).FormatDescription());

                Assert.Throws<ArgumentNullException>(() => record.GetPropertyValues(null));
                Assert.NotNull(record.GetPropertyValues(new EventLogPropertySelector(new [] {"dummy"})));
                Assert.NotNull(record.ToXml());

                record.Dispose();
                // eventLog.CancelReading();
            }
        }

        private static EventBookmark GetBookmark(string log, PathType pathType)
        {
            var elq = new EventLogQuery(log, pathType) { ReverseDirection = true };
            var reader = new EventLogReader(elq);
            var record = reader.ReadEvent();
            if (record != null)
                return record.Bookmark;
            return null;
        }

        [ConditionalFact(typeof(Helpers), nameof(Helpers.SupportsEventLogs))]
        public void Seek()
        {
            using (var eventLog = new EventLogReader("Application"))
            {
                EventRecord record = eventLog.ReadEvent();
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
                foreach (var logStatus in eventLog.LogStatus)
                {
                    Assert.Equal("Application", logStatus.LogName);
                    Assert.Equal(0, logStatus.StatusCode);
                }
            }
        }

        [ConditionalFact(typeof(Helpers), nameof(Helpers.SupportsEventLogs))]
        public void LogStatus_OtherCtor()
        {
            using (var eventLog = new EventLogReader("Application", PathType.LogName))
            {
                Assert.Equal(64, eventLog.BatchSize);
            }
        }

        [ConditionalFact(typeof(Helpers), nameof(Helpers.SupportsEventLogs))]
        public void Ctor_NullQuery_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => new EventLogReader(null, null));
        }
    }
}

