// Licensed to the .NET Foundation under one or more agreements.
// See the LICENSE file in the project root for more information.

using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace System.Data.Tests.Common
{
    public partial class DbTransactionTest
    {
        [Fact]
        public async Task CommitAsyncCanceled()
        {
            Task t = new MockTransaction().CommitAsync(new CancellationToken(true));
            await Assert.ThrowsAsync<TaskCanceledException>(() => t);
        }

        [Fact]
        public async Task RollbackAsyncCanceled()
        {
            Task t = new MockTransaction().RollbackAsync(new CancellationToken(true));
            await Assert.ThrowsAsync<TaskCanceledException>(() => t);
        }
    }
}
