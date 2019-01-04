// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics.Eventing.Reader;
using Xunit;

namespace System.Diagnostics.Tests
{
    public class EventLogInformationTests
    {
        [ConditionalTheory(typeof(Helpers), nameof(Helpers.SupportsEventLogs))]
        [InlineData(true)]
        [InlineData(false)]
        public void GetLogInformation_NullLogName_Throws(bool usingDefaultCtor)
        {
            using (var session = usingDefaultCtor ? new EventLogSession() : new EventLogSession(null))
            {
                Assert.Throws<ArgumentNullException>(() => session.GetLogInformation(null, PathType.LogName));
            }
        }

        [ConditionalTheory(typeof(Helpers), nameof(Helpers.SupportsEventLogs))]
        [InlineData("Microsoft-Windows-TaskScheduler/Operational")]
        [InlineData("Application")]
        public void GetLogInformation_UsingLogName_DoesNotThrow(string logName)
        {
            using (var session = new EventLogSession())
            {
                EventLogConfiguration configuration = null;
                try
                {
                    configuration = new EventLogConfiguration(logName, session);
                }
                catch (EventLogNotFoundException)
                {
                    configuration?.Dispose();
                    return;
                }
                
                Assert.Equal(logName, configuration.LogName);
                Assert.True(configuration.IsEnabled);
                Assert.NotEmpty(configuration.SecurityDescriptor);
                Assert.Equal(EventLogMode.Circular, configuration.LogMode);
                Assert.Equal(64, configuration.ProviderBufferSize);
                Assert.Equal(0, configuration.ProviderMinimumNumberOfBuffers);
                Assert.Equal(64, configuration.ProviderMaximumNumberOfBuffers);
                Assert.NotEmpty(configuration.ProviderNames);
                Assert.False(configuration.ProviderLevel.HasValue);
                Assert.False(configuration.ProviderKeywords.HasValue);
                Assert.False(configuration.ProviderControlGuid.HasValue);
                Assert.Equal(1000, configuration.ProviderLatency);
                Assert.Equal("Application", configuration.LogIsolation.ToString());
                if (logName.Equals("Application"))
                {
                    Assert.Equal(EventLogType.Administrative, configuration.LogType);
                    Assert.True(configuration.IsClassicLog);
                    Assert.Contains("Application.evtx", configuration.LogFilePath);
                    Assert.Empty(configuration.OwningProviderName);
                }
                else
                {
                    Assert.Equal(EventLogType.Operational, configuration.LogType);
                    Assert.False(configuration.IsClassicLog);
                    Assert.Contains("Microsoft-Windows-TaskScheduler%4Operational.evtx", configuration.LogFilePath);
                    Assert.Equal("Microsoft-Windows-TaskScheduler", configuration.OwningProviderName);
                }

                EventLogInformation logInfo = session.GetLogInformation(configuration.LogName, PathType.LogName);
                Assert.NotNull(logInfo.CreationTime);
                Assert.NotNull(logInfo.LastAccessTime);
                Assert.NotNull(logInfo.LastWriteTime);
                Assert.NotNull(logInfo.FileSize);
                Assert.NotNull(logInfo.Attributes);
                Assert.NotNull(logInfo.RecordCount);
                Assert.NotNull(logInfo.OldestRecordNumber);
                Assert.NotNull(logInfo.IsLogFull);
            }
        }
    }
}