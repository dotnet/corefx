// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics.Eventing.Reader;
using System.Threading;
using Xunit;

namespace System.Diagnostics.Tests
{
    public class EventLogConfigurationTests
    {
        [ConditionalFact(typeof(Helpers), nameof(Helpers.SupportsEventLogs))]
        public void Properties_DoNotThrow()
        {
            var configuration = new EventLogConfiguration("Application");

            var LogName = configuration.LogName;
            var LogType = configuration.LogType;
            var LogIsolation = configuration.LogIsolation;
            var IsEnabled = configuration.IsEnabled;
            var IsClassicLog = configuration.IsClassicLog;
            var SecurityDescriptor = configuration.SecurityDescriptor;
            var LogFilePath = configuration.LogFilePath;
            var MaximumSizeInBytes = configuration.MaximumSizeInBytes;
            var LogMode = configuration.LogMode;
            var OwningProviderName = configuration.OwningProviderName;
            var ProviderNames = configuration.ProviderNames;
            var ProviderLevel = configuration.ProviderLevel;
            var ProviderKeywords = configuration.ProviderKeywords;
            var ProviderBufferSize = configuration.ProviderBufferSize;
            var ProviderMinimumNumberOfBuffers = configuration.ProviderMinimumNumberOfBuffers;
            var ProviderMaximumNumberOfBuffers = configuration.ProviderMaximumNumberOfBuffers;
            var ProviderLatency = configuration.ProviderLatency;
            var ProviderControlGuid = configuration.ProviderControlGuid;
        }
    }
}
