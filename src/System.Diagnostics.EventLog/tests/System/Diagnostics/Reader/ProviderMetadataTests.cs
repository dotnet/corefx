// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Globalization;
using System.Linq;
using System.Threading;
using Xunit;

namespace System.Diagnostics.Tests
{
    public class ProviderMetadataTests
    {
        [ConditionalFact(typeof(Helpers), nameof(Helpers.SupportsEventLogs))]
        public void SourceDoesNotExist_Throws()
        {
            Assert.Throws<EventLogNotFoundException>(() => new ProviderMetadata("Source_Does_Not_Exist"));
        }

        [ConditionalTheory(typeof(Helpers), nameof(Helpers.SupportsEventLogs))]
        [InlineData(true)]
        [InlineData(false)]
        public void ProviderNameTests(bool noProviderName)
        {
            string log = "Application";
            string source = "Source_" + nameof(ProviderNameTests);
            
            using (var session = new EventLogSession())
            {
                try
                {
                    EventLog.CreateEventSource(source, log);
                    
                    string providerName = noProviderName ? "" : source;
                    using (var providerMetadata = new ProviderMetadata(providerName))
                    {
                        Assert.Null(providerMetadata.DisplayName);
                        Assert.Equal(providerName, providerMetadata.Name);
                        Assert.Equal(new Guid(), providerMetadata.Id);
                        Assert.Empty(providerMetadata.Events);
                        Assert.Empty(providerMetadata.Keywords);
                        Assert.Empty(providerMetadata.Levels);
                        Assert.Empty(providerMetadata.Opcodes);
                        Assert.Empty(providerMetadata.Tasks);
                        Assert.NotEmpty(providerMetadata.LogLinks);
                        if (!string.IsNullOrEmpty(providerName))
                        {
                            foreach (var logLink in providerMetadata.LogLinks)
                            {
                                Assert.True(logLink.IsImported);
                                Assert.Equal(log, logLink.LogName);
                                Assert.NotEmpty(logLink.DisplayName);
                                if (CultureInfo.CurrentCulture.Name.Split('-')[0] == "en" )
                                {
                                    Assert.Equal("Application", logLink.DisplayName);
                                }
                                else if (CultureInfo.CurrentCulture.Name.Split('-')[0] == "es" )
                                {
                                    Assert.Equal("Aplicación", logLink.DisplayName);
                                }
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
                    }
                }
                finally
                {
                    EventLog.DeleteEventSource(source);
                }
                session.CancelCurrentOperations();
            }

        }

        [ConditionalFact(typeof(Helpers), nameof(Helpers.SupportsEventLogs))]
        public void GetProviderNames_AssertProperties()
        {
            List<string> standardOpcodeNames = new List<string>(Enum.GetNames(typeof(StandardEventOpcode))).Select(x => "win:" + x).ToList();
            using (var session = new EventLogSession())
            {
                Assert.NotEmpty(session.GetProviderNames());
                foreach (string providerName in session.GetProviderNames())
                {
                    ProviderMetadata providerMetadata = null;
                    try
                    {
                        providerMetadata = new ProviderMetadata(providerName);
                        foreach (var keyword in providerMetadata.Keywords)
                        {
                            Assert.NotEmpty(keyword.Name);
                            Assert.NotNull(keyword.Value);
                        }
                        foreach (var logLink in providerMetadata.LogLinks)
                        {
                            Assert.NotEmpty(logLink.LogName);
                        }
                        foreach (var opcode in providerMetadata.Opcodes)
                        {
                            if (opcode != null && standardOpcodeNames.Contains(opcode.Name))
                            {
                                Assert.Contains((((StandardEventOpcode)(opcode.Value)).ToString()), opcode.Name);
                            }
                        }
                        foreach (var eventMetadata in providerMetadata.Events)
                        {
                            EventLogLink logLink = eventMetadata.LogLink;
                            if(logLink != null)
                            {
                                if (logLink.DisplayName != null && logLink.DisplayName.Equals("System"))
                                {
                                    Assert.Equal("System", logLink.LogName);
                                    Assert.True(logLink.IsImported);
                                }
                            }
                            EventLevel eventLevel = eventMetadata.Level;
                            if(eventLevel != null)
                            {
                                if (eventLevel.Name != null)
                                {
                                    // https://github.com/Microsoft/perfview/blob/d4b044abdfb4c8e40a344ca05383e04b5b6dc13a/src/related/EventRegister/winmeta.xml#L39
                                    if (eventLevel.Name.StartsWith("win:") && !eventLevel.Name.Contains("ReservedLevel"))
                                    {
                                        Assert.True(System.Enum.IsDefined(typeof(StandardEventLevel), eventLevel.Value));
                                        Assert.Contains(eventLevel.Name.Substring(4), Enum.GetNames(typeof(StandardEventLevel)));
                                    }
                                }
                            }
                            EventOpcode opcode = eventMetadata.Opcode;
                            if(opcode != null)
                            {
                                if (opcode.Name != null && opcode.DisplayName != null && opcode.DisplayName.ToLower().Equals("apprun"))
                                {
                                    Assert.Contains(opcode.DisplayName.ToLower(), opcode.Name.ToLower());
                                }
                            }
                            EventTask task = eventMetadata.Task;
                            if(eventMetadata.Task != null)
                            {
                                if (task.Value == (int)StandardEventTask.None)
                                {
                                    Assert.True(task.DisplayName == null || task.DisplayName == "None");
                                    Assert.True(task.Name == null || task.Name == "win:None");
                                    Assert.Equal(new Guid(), task.EventGuid);
                                    Assert.Equal(0, task.Value);
                                }
                            }
                            IEnumerable<EventKeyword> keywords = eventMetadata.Keywords;
                            if(eventMetadata.Keywords != null)
                            {
                                foreach(var keyword in eventMetadata.Keywords)
                                {
                                    if (keyword.Name != null)
                                    {
                                        if (keyword.Name.StartsWith("win:"))
                                        {
                                            Assert.True(System.Enum.IsDefined(typeof(StandardEventKeywords), keyword.Value));
                                        }
                                    }
                                }
                            }
                            Assert.NotNull(eventMetadata.Template);
                        }
                    }
                    catch (EventLogException)
                    {
                        continue;
                    }
                    finally
                    {
                        providerMetadata?.Dispose();
                    }
                }
            }
        }
    }
}
