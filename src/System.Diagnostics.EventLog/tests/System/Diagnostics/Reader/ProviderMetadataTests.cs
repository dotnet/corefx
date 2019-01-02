// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Threading;
using Xunit;

namespace System.Diagnostics.Tests
{
    public class ProviderMetadataTests
    {
        [ConditionalFact(typeof(Helpers), nameof(Helpers.SupportsEventLogs))]
        public void ProviderNameTests()
        {
            string log = "Application";
            string source = "Source_" + nameof(ProviderNameTests);

            try
            {
                if (EventLog.SourceExists(source))
                {
                    EventLog.DeleteEventSource(source);
                }

                EventLog.CreateEventSource(source, log);
            }
            finally
            {
                Assert.Throws<EventLogNotFoundException>(() => new ProviderMetadata("Source_Does_Not_Exist"));
                foreach (string sourceName in new [] { "", source})
                {
                    var providerMetadata = new ProviderMetadata(sourceName);
                    Assert.Null(providerMetadata.DisplayName);
                    Assert.Equal(sourceName, providerMetadata.Name);
                    Assert.Equal(new Guid(), providerMetadata.Id);
                    Assert.Empty(providerMetadata.Events);
                    Assert.Empty(providerMetadata.Keywords);
                    Assert.Empty(providerMetadata.Levels);
                    Assert.Empty(providerMetadata.Opcodes);
                    Assert.Empty(providerMetadata.Tasks);
                    Assert.NotEmpty(providerMetadata.LogLinks);
                    if (sourceName.Equals(source))
                    {
                        foreach (var logLink in providerMetadata.LogLinks)
                        {
                            Assert.True(logLink.IsImported);
                            Assert.Equal(log, logLink.DisplayName);
                            Assert.Equal(log, logLink.LogName);
                        }
                        Assert.Contains("EventLogMessages.dll", providerMetadata.MessageFilePath);
                        Assert.Contains("EventLogMessages.dll", providerMetadata.HelpLink.ToString());
                    }
                    else
                    {
                        Assert.Null(providerMetadata.MessageFilePath);
                        Assert.Null(providerMetadata.HelpLink);
                    }
                    Assert.Null(providerMetadata.ResourceFilePath);
                    Assert.Null(providerMetadata.ParameterFilePath);
                    providerMetadata.Dispose();
                }
            }
        }
    }
}
