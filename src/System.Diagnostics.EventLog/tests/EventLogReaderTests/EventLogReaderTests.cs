// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Globalization;
using System.IO;
using Xunit;
using System.Diagnostics.Eventing.Reader;

namespace System.Diagnostics.Tests
{
    public class EventLogReaderTests
    {
        [ConditionalFact(typeof(Helpers), nameof(Helpers.SupportsEventLogs))]
        public void ApplicationEventLog_Record()
        {
            var eventLog = new EventLogReader("Application");
            var record = eventLog.ReadEvent();
            Assert.NotNull(record);
            Assert.Equal(record.LogName, "Application");
        }

        [ConditionalFact(typeof(Helpers), nameof(Helpers.SupportsEventLogs))]
        public void ApplicationEventLogQuery_Record()
        {
            var query = new EventLogQuery("Application", PathType.LogName) { ReverseDirection = true };
            var eventLog = new EventLogReader(query);
            var record = eventLog.ReadEvent();
            Assert.NotNull(record);
            Assert.Equal(record.LogName, "Application");
        }
    }
}

