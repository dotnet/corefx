// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics.Eventing.Reader;
using System.Threading;
using Xunit;

namespace System.Diagnostics.Tests
{
    public class EventLogRecordTests
    {
        private static EventLogRecord _sampleRecord;

        static EventLogRecordTests()
        {
            var logName = "Application";
            var eventLog = new EventLogReader(logName);

            using (eventLog)
            {
                _sampleRecord = (EventLogRecord) eventLog.ReadEvent();
            }
        }

        [ConditionalFact(typeof(Helpers), nameof(Helpers.SupportsEventLogs))]
        public void Properties_ReturnNonEmptyValues()
        {
            Assert.NotEqual(0, _sampleRecord.Id);
            Assert.NotNull(_sampleRecord.Version);
            Assert.NotNull(_sampleRecord.Qualifiers);
            Assert.NotNull(_sampleRecord.Level);
            Assert.NotNull(_sampleRecord.Task);
            Assert.NotNull(_sampleRecord.Opcode);
            Assert.NotNull(_sampleRecord.Keywords);
            Assert.NotNull(_sampleRecord.RecordId);
            Assert.NotNull(_sampleRecord.ProviderName);
            Assert.NotNull(_sampleRecord.ProviderId);
            Assert.NotNull(_sampleRecord.ProcessId);
            Assert.NotNull(_sampleRecord.ThreadId);
            Assert.NotNull(_sampleRecord.MachineName);
            var userId = _sampleRecord.UserId;
            Assert.NotNull(_sampleRecord.TimeCreated);
            var activityId = _sampleRecord.ActivityId;
            var relatedActivityId = _sampleRecord.RelatedActivityId;
            Assert.NotNull(_sampleRecord.ContainerLog);
            Assert.NotNull(_sampleRecord.MatchedQueryIds);
            Assert.NotNull(_sampleRecord.Bookmark);
            Assert.NotNull(_sampleRecord.LevelDisplayName);
            var opcodeDisplayName = _sampleRecord.OpcodeDisplayName;
            var taskDisplayName = _sampleRecord.TaskDisplayName;
            Assert.NotNull(_sampleRecord.KeywordsDisplayNames);
            Assert.NotNull(_sampleRecord.Properties);
        }

        [ConditionalFact(typeof(Helpers), nameof(Helpers.SupportsEventLogs))]
        public void Methods_ReturnNonEmptyValues()
        {
            Assert.NotNull(_sampleRecord.FormatDescription());
            Assert.NotNull(_sampleRecord.FormatDescription(new[] {"dummy"}));
            Assert.NotNull(_sampleRecord.GetPropertyValues(new EventLogPropertySelector(new []{"dummy"})));
            Assert.NotNull(_sampleRecord.ToXml());
        }
    }
}
