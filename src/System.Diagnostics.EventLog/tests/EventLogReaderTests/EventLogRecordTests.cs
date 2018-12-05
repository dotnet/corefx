// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics.Eventing.Reader;
using System.Security.Principal;
using System.Threading;
using Xunit;

namespace System.Diagnostics.Tests
{
    public class EventLogRecordTests
    {
        private static EventLogRecord _sampleRecord;

        public EventLogRecordTests()
        {
            var logName = "Application";
            var eventLog = new EventLogReader(logName);

            using (eventLog)
            {
                _sampleRecord = (EventLogRecord) eventLog.ReadEvent();
            }
        }

        [ConditionalFact(typeof(Helpers), nameof(Helpers.SupportsEventLogs))]
        public void Properties_Throws()
        {
            Assert.Throws<EventLogNotFoundException>(() => _sampleRecord.LevelDisplayName);
            Assert.Throws<EventLogNotFoundException>(() => _sampleRecord.KeywordsDisplayNames);
        }

        [ConditionalFact(typeof(Helpers), nameof(Helpers.SupportsEventLogs))]
        public void Properties_ReturnEmptyValues()
        {
            Assert.Null(_sampleRecord.Version);
            Assert.Null(_sampleRecord.Opcode);
            Assert.Null(_sampleRecord.ProviderId);
            Assert.Null(_sampleRecord.ProcessId);
            Assert.Null(_sampleRecord.ThreadId);
            SecurityIdentifier userId = _sampleRecord.UserId;
            Guid? activityId = _sampleRecord.ActivityId;
            Assert.False(activityId.HasValue);
            Guid? relatedActivityId = _sampleRecord.RelatedActivityId;
            Assert.False(relatedActivityId.HasValue);
        }

        [ConditionalFact(typeof(Helpers), nameof(Helpers.SupportsEventLogs))]
        public void Properties_ReturnNonEmptyValues()
        {
            Assert.NotEqual(0, _sampleRecord.Id);
            Assert.NotNull(_sampleRecord.Qualifiers);
            Assert.NotNull(_sampleRecord.Level);
            Assert.NotNull(_sampleRecord.Task);
            Assert.NotNull(_sampleRecord.Keywords);
            Assert.NotNull(_sampleRecord.RecordId);
            Assert.NotNull(_sampleRecord.ProviderName);
            Assert.NotNull(_sampleRecord.MachineName);
            SecurityIdentifier userId = _sampleRecord.UserId;
            Assert.NotNull(_sampleRecord.TimeCreated);
            Assert.NotNull(_sampleRecord.ContainerLog);
            Assert.NotNull(_sampleRecord.MatchedQueryIds);
            Assert.NotNull(_sampleRecord.Bookmark);
            Assert.NotNull(_sampleRecord.Properties);
        }

        [ConditionalFact(typeof(Helpers), nameof(Helpers.SupportsEventLogs))]
        public void Methods_ReturnThrows()
        {
            Assert.Throws<EventLogNotFoundException>(() => _sampleRecord.FormatDescription(new[] {"dummy"}));
        }

        [ConditionalFact(typeof(Helpers), nameof(Helpers.SupportsEventLogs))]
        public void Methods_ReturnEmptyValues()
        {
            Assert.Null(_sampleRecord.FormatDescription());
        }

        [ConditionalFact(typeof(Helpers), nameof(Helpers.SupportsEventLogs))]
        public void Methods_ReturnNonEmptyValues()
        {
            Assert.NotNull(_sampleRecord.GetPropertyValues(new EventLogPropertySelector(new []{"dummy"})));
            Assert.NotNull(_sampleRecord.ToXml());
        }
    }
}
