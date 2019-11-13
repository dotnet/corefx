// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Concurrent;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Net.Sockets;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.DotNet.XUnitExtensions;
using Xunit;
using Xunit.Abstractions;

namespace System.Net.NameResolution.Tests
{
    using Configuration = System.Net.Test.Common.Configuration;

    [Collection("NoParallelTests")]
    public class LoggingTest
    {
        [Fact]
        public static void EventSource_ExistsWithCorrectId()
        {
            Type esType = typeof(Dns).GetTypeInfo().Assembly.GetType("System.Net.NetEventSource", throwOnError: true, ignoreCase: false);
            Assert.NotNull(esType);

            Assert.Equal("Microsoft-System-Net-NameResolution", EventSource.GetName(esType));
            Assert.Equal(Guid.Parse("5f302add-3825-520e-8fa0-627b206e2e7e"), EventSource.GetGuid(esType));

            Assert.NotEmpty(EventSource.GenerateManifest(esType, "assemblyPathToIncludeInManifest"));
        }

        [ConditionalFact]
        public void GetHostEntry_InvalidHost_LogsError()
        {
            using (var listener = new TestEventListener("Microsoft-System-Net-NameResolution", EventLevel.Error))
            {
                var events = new ConcurrentQueue<EventWrittenEventArgs>();

                listener.RunWithCallback(ev => events.Enqueue(ev), () =>
                {
                    try
                    {
                        Dns.GetHostEntry(Configuration.Sockets.InvalidHost);
                        throw new SkipTestException("GetHostEntry() should fail but it did not.");
                    }
                    catch (SocketException e) when (e.SocketErrorCode == SocketError.HostNotFound)
                    {
                    }
                    catch (Exception e)
                    {
                        throw new SkipTestException($"GetHostEntry failed unexpectedly: {e.Message}");
                    }
                });

                Assert.True(events.Count() > 0);
                foreach (EventWrittenEventArgs ev in events)
                {
                    Assert.True(ev.Payload.Count >= 3);
                    Assert.NotNull(ev.Payload[0]);
                    Assert.NotNull(ev.Payload[1]);
                    Assert.NotNull(ev.Payload[2]);
                }
            }
        }

        [ConditionalFact]
        [PlatformSpecific(~TestPlatforms.Windows)]  // Unreliable on Windows.
        public void GetHostEntryAsync_InvalidHost_LogsError()
        {
            using (var listener = new TestEventListener("Microsoft-System-Net-NameResolution", EventLevel.Error))
            {
                var events = new ConcurrentQueue<EventWrittenEventArgs>();

                listener.RunWithCallback(ev => events.Enqueue(ev), () =>
                {
                    try
                    {
                        Dns.GetHostEntryAsync(Configuration.Sockets.InvalidHost).GetAwaiter().GetResult();
                        throw new SkipTestException("GetHostEntry() should fail but it did not.");
                    }
                    catch (SocketException e) when (e.SocketErrorCode == SocketError.HostNotFound)
                    {
                    }
                    catch (Exception e)
                    {
                        throw new SkipTestException($"GetHostEntry failed unexpectedly: {e.Message}");
                    }
                });

                Assert.True(events.Count() > 0);
                foreach (EventWrittenEventArgs ev in events)
                {
                    Assert.True(ev.Payload.Count >= 3);
                    Assert.NotNull(ev.Payload[0]);
                    Assert.NotNull(ev.Payload[1]);
                    Assert.NotNull(ev.Payload[2]);
                }
            }
        }

        [ConditionalFact]
        public void GetHostEntry_ValidName_NoErrors()
        {
            using (var listener = new TestEventListener("Microsoft-System-Net-NameResolution", EventLevel.Verbose))
            {
                var events = new ConcurrentQueue<EventWrittenEventArgs>();

                listener.RunWithCallback(ev => events.Enqueue(ev), () =>
                {
                    try
                    {
                        Dns.GetHostEntryAsync("localhost").GetAwaiter().GetResult();
                        Dns.GetHostEntryAsync(IPAddress.Loopback).GetAwaiter().GetResult();
                        Dns.GetHostEntry("localhost");
                        Dns.GetHostEntry(IPAddress.Loopback);
                    }
                    catch (Exception e)
                    {
                        throw new SkipTestException($"Localhost lookup failed unexpectedly: {e.Message}");
                    }
                });

                // We get some traces.
                Assert.True(events.Count() > 0);
                // No errors or warning for successful query.
                Assert.True(events.Count(ev => (int)ev.Level > (int)EventLevel.Informational) == 0);
            }
        }
    }
}
