// Licensed to the .NET Foundation under one or more agreements.
// See the LICENSE file in the project root for more information.

using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace System.Data.Common.Tests
{
    public partial class DbConnectionTest
    {
        [Fact]
        public async Task ChangeDatabaseAsyncCanceled()
        {
            Task t = new FinalizingConnection().ChangeDatabaseAsync("foo", new CancellationToken(true));
            await Assert.ThrowsAsync<TaskCanceledException>(() => t);
        }

        [Fact]
        public async Task BeginTransactionAsyncCanceled1()
        {
            Task t = new FinalizingConnection().BeginTransactionAsync(new CancellationToken(true)).AsTask();
            await Assert.ThrowsAsync<TaskCanceledException>(() => t);
        }

        [Fact]
        public async Task BeginTransactionAsyncCanceled2()
        {
            Task t = new FinalizingConnection().BeginTransactionAsync(IsolationLevel.ReadCommitted, new CancellationToken(true)).AsTask();
            await Assert.ThrowsAsync<TaskCanceledException>(() => t);
        }

        [Fact]
        public async Task CloseAsyncCanceled()
        {
            Task t = new FinalizingConnection().CloseAsync(new CancellationToken(true));
            await Assert.ThrowsAsync<TaskCanceledException>(() => t);
        }
    }
}
