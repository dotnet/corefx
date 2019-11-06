// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Concurrent;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Reflection;
using System.Threading;
using Microsoft.DotNet.XUnitExtensions;
using Xunit;
using Xunit.Abstractions;

namespace System.Net.NameResolution.Tests
{
    using Configuration = System.Net.Test.Common.Configuration;

    [Collection("NoParallelTests")]
    public static class LoggingTest
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

        [Fact]
        public static void EventSource_LookupErrro_Success()
        {
            using (var listener = new TestEventListener("Microsoft-System-Net-NameResolution", EventLevel.Verbose))
            {
                var events = new ConcurrentQueue<EventWrittenEventArgs>();

                //const int AcquiringAllLocksEventId = 3;
                Clear(events);
                listener.RunWithCallback(ev => events.Enqueue(ev), () =>
                {
                    try
                    {
                        Dns.GetHostEntry(Configuration.Sockets.InvalidHost);
                        throw new SkipTestException("GetHostEntry() should fail but it did not.");
                    }
                    catch (Exception e)
                    { Console.WriteLine(e); };
                    Console.WriteLine("ALl DONE");
                    Thread.Sleep(100);
                });

                Assert.True(events.Count() > 0);
                Console.WriteLine($"Count = {events.Count()}");

                foreach (EventWrittenEventArgs ev in events)
                {
                    Console.WriteLine(ev.ToString());
                    Console.WriteLine($"git {ev.EventId},  m='{ev.Message}' , p='{ev.Payload}' {ev.Level} anbd {ev.EventSource.ToString()}");

                }
            }
        }

        private static void Clear<T>(ConcurrentQueue<T> queue)
        {
            T item;
            while (queue.TryDequeue(out item)) ;
        }
    }
}
