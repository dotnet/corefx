// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace System.IO.Tests
{
    public partial class StreamDisposeAsyncTests
    {
        [Fact]
        public async Task DisposeAsync_MultipleDisposalSuccessful()
        {
            var s = new DelegateStream();
            await s.DisposeAsync();
            await s.DisposeAsync();
        }

        [Fact]
        public void DisposeAsync_QueuesDispose()
        {
            using (Barrier b = new Barrier(2))
            {
                var s = new DelegateStream(disposeFunc: disposing =>
                {
                    Assert.True(disposing);
                    b.SignalAndWait();
                    b.SignalAndWait();
                });

                ValueTask vt = s.DisposeAsync();
                b.SignalAndWait();
                Thread.Sleep(1);
                Assert.False(vt.IsCompleted);
                b.SignalAndWait();
                vt.AsTask().Wait();
            }
        }
    }
}
