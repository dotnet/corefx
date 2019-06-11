// Licensed to the .NET Foundation under one or more agreements.
// See the LICENSE file in the project root for more information.

using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace System.Data.Common.Tests
{
    public partial class DbCommandTest
    {
        [Fact]
        public async Task PrepareAsyncCanceled()
        {
            Task t = new FinalizingCommand().PrepareAsync(new CancellationToken(true));
            await Assert.ThrowsAsync<TaskCanceledException>(() => t);
        }
    }
}
