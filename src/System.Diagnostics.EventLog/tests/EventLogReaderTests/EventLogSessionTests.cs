// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics.Eventing.Reader;
using System.IO;
using System.Threading;
using Xunit;

namespace System.Diagnostics.Tests
{
    public class EventLogSessionTests : FileCleanupTestBase
    {
        [ConditionalFact(typeof(Helpers), nameof(Helpers.SupportsEventLogs))]
        public void Methods_DoNotThrow()
        {
            var session = new EventLogSession();
            Assert.NotEmpty(session.GetProviderNames());
            Assert.NotEmpty(session.GetLogNames());
            session.ExportLog("Application", PathType.LogName, "Application", GetTestFilePath());
            session.ExportLogAndMessages("Application", PathType.LogName, "Application", GetTestFilePath());
            session.GetLogInformation("Application", PathType.LogName);
            session.CancelCurrentOperations();
            session.Dispose();
        }

        [ConditionalFact(typeof(Helpers), nameof(Helpers.SupportsEventLogs))]
        public void Methods_Throws()
        {
            var session = new EventLogSession();
            Assert.Throws<EventLogException>(() => session.ExportLogAndMessages("Application", PathType.LogName, "Application", Guid.NewGuid().ToString("N")));
            session.Dispose();
        }
    }
}
