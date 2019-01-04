// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Globalization;
using System.IO;
using System.Security;
using System.Threading;
using Xunit;

namespace System.Diagnostics.Tests
{
    public class EventLogSessionTests : FileCleanupTestBase
    {
        private const string LogName = "Application";

        [ConditionalTheory(typeof(Helpers), nameof(Helpers.SupportsEventLogs))]
        [InlineData(true)]
        [InlineData(false)]
        public void Ctors_ProviderNames_LogNames_NotEmpty(bool usingDefaultCtor)
        {
            using (var session = usingDefaultCtor ? new EventLogSession() : new EventLogSession(null))
            {
                Assert.NotEmpty(session.GetProviderNames());
                Assert.NotEmpty(session.GetLogNames());
            }
        }

        [ConditionalTheory(typeof(Helpers), nameof(Helpers.SupportsEventLogs))]
        [InlineData(true)]
        [InlineData(false)]
        public void ExportLogAndMessages_NullPath_Throws(bool usingDefaultCtor)
        {
            using (var session = usingDefaultCtor ? new EventLogSession() : new EventLogSession(null))
            {
                Assert.Throws<ArgumentNullException>(() => session.ExportLogAndMessages(null, PathType.LogName, LogName, GetTestFilePath()));
                // Does not throw:
                session.ExportLogAndMessages(LogName, PathType.LogName, LogName, GetTestFilePath());
                session.ExportLogAndMessages(LogName, PathType.LogName, LogName, GetTestFilePath(), false, targetCultureInfo: CultureInfo.CurrentCulture);
                session.CancelCurrentOperations();
            }
        }

        [ConditionalTheory(typeof(Helpers), nameof(Helpers.SupportsEventLogs))]
        [InlineData(true)]
        [InlineData(false)]
        public void ExportLog_InvalidInputCombinations_Throws(bool usingDefaultCtor)
        {
            using (var session = usingDefaultCtor ? new EventLogSession() : new EventLogSession(null))
            {
                Assert.Throws<ArgumentNullException>(() => session.ExportLog(null, PathType.LogName, LogName, GetTestFilePath()));
                Assert.Throws<ArgumentNullException>(() => session.ExportLog(LogName, PathType.LogName, LogName, null));
                Assert.Throws<ArgumentOutOfRangeException>(() => session.ExportLog(LogName, (PathType)0, LogName, GetTestFilePath()));
                Assert.Throws<EventLogNotFoundException>(() => session.ExportLog(LogName, PathType.FilePath, LogName, GetTestFilePath()));
                // Does not throw:
                session.ExportLog(LogName, PathType.LogName, LogName, GetTestFilePath(), tolerateQueryErrors: true);
                session.CancelCurrentOperations();
            }
        }

        [ConditionalFact(typeof(Helpers), nameof(Helpers.SupportsEventLogs))]
        public void GetProviderNames_WithPassword_Throws()
        {
            var password = new SecureString();
            password.AppendChar('a');
            using (var session = new EventLogSession(null, null, null, password, SessionAuthentication.Default))
            {
                Assert.Throws<UnauthorizedAccessException>(() => session.GetProviderNames());
            }
        }

        [ConditionalTheory(typeof(Helpers), nameof(Helpers.SupportsEventLogs))]
        [InlineData(true)]
        [InlineData(false)]
        public void ClearLog_NullOrEmptyLogName_Throws(bool usingDefaultCtor)
        {
            using (var session = usingDefaultCtor ? new EventLogSession() : new EventLogSession(null))
            {
                Assert.Throws<ArgumentNullException>(() => session.ClearLog(null));
                Assert.Throws<ArgumentNullException>(() => session.ClearLog(null, backupPath: GetTestFilePath()));
                Assert.Throws<EventLogException>(() => session.ClearLog(""));
                
                session.CancelCurrentOperations();
            }
        }
    }
}
