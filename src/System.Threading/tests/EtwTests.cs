// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Diagnostics.Tracing;
using Xunit;

namespace System.Threading.Tests
{
    public class EtwTests
    {
        [Fact]
        public void TestEtw()
        {
            using (var listener = new TestEventListener("System.Threading.SynchronizationEventSource", EventLevel.Verbose))
            {
                const int BarrierPhaseFinishedId = 3;
                const int ExpectedInvocations = 5;
                int eventsRaised = 0;
                int delegateInvocations = 0;
                listener.RunWithCallback(ev => {
                        Assert.Equal(expected: BarrierPhaseFinishedId, actual: ev.EventId);
                        eventsRaised++;
                    },
                    () => {
                        Barrier b = new Barrier(1, _ => delegateInvocations++);
                        for (int i = 0; i < ExpectedInvocations; i++)
                            b.SignalAndWait();
                    });
                Assert.Equal(ExpectedInvocations, delegateInvocations);
                Assert.Equal(ExpectedInvocations, eventsRaised);
            }
        }
    }
}
