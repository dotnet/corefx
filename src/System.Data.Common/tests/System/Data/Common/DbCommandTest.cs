// Licensed to the .NET Foundation under one or more agreements.
// See the LICENSE file in the project root for more information.

using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace System.Data.Common.Tests
{
    public partial class DbCommandTest
    {
        private static volatile bool _wasFinalized;

        private class FinalizingCommand : DbCommand
        {
            public static void CreateAndRelease()
            {
                new FinalizingCommand();
            }

            protected override void Dispose(bool disposing)
            {
                if (!disposing)
                    _wasFinalized = true;
                base.Dispose(disposing);
            }

            public override string CommandText
            {
                get => throw new NotImplementedException();
                set => throw new NotImplementedException();
            }

            public override int CommandTimeout
            {
                get => throw new NotImplementedException();
                set => throw new NotImplementedException();
            }

            public override CommandType CommandType
            {
                get => throw new NotImplementedException();
                set => throw new NotImplementedException();
            }

            public override bool DesignTimeVisible
            {
                get => throw new NotImplementedException();
                set => throw new NotImplementedException();
            }

            public override UpdateRowSource UpdatedRowSource
            {
                get => throw new NotImplementedException();
                set => throw new NotImplementedException();
            }

            protected override DbConnection DbConnection
            {
                get => throw new NotImplementedException();
                set => throw new NotImplementedException();
            }

            protected override DbParameterCollection DbParameterCollection => throw new NotImplementedException();
            protected override DbTransaction DbTransaction
            {
                get => throw new NotImplementedException();
                set => throw new NotImplementedException();
            }

            public override void Cancel() => throw new NotImplementedException();
            public override int ExecuteNonQuery() => throw new NotImplementedException();
            public override object ExecuteScalar() => throw new NotImplementedException();
            public override void Prepare() => throw new NotImplementedException();
            protected override DbParameter CreateDbParameter() => throw new NotImplementedException();
            protected override DbDataReader ExecuteDbDataReader(CommandBehavior behavior) => throw new NotImplementedException();
        }

        [Fact]
        [SkipOnTargetFramework(TargetFrameworkMonikers.Mono, "GC has different behavior on Mono")]
        public void CanBeFinalized()
        {
            FinalizingCommand.CreateAndRelease();
            GC.Collect();
            GC.WaitForPendingFinalizers();
            Assert.True(_wasFinalized);
        }

        [Fact]
        public async Task ExecuteReaderAsyncCanceled1()
        {
            Task t = new FinalizingCommand().ExecuteReaderAsync(new CancellationToken(true));
            await Assert.ThrowsAsync<TaskCanceledException>(() => t);
        }

        [Fact]
        public async Task ExecuteReaderAsyncCanceled2()
        {
            Task t = new FinalizingCommand().ExecuteReaderAsync(CommandBehavior.Default, new CancellationToken(true));
            await Assert.ThrowsAsync<TaskCanceledException>(() => t);
        }

        [Fact]
        public async Task ExecuteNonQueryAsyncCanceled()
        {
            Task t = new FinalizingCommand().ExecuteNonQueryAsync(new CancellationToken(true));
            await Assert.ThrowsAsync<TaskCanceledException>(() => t);
        }

        [Fact]
        public async Task ExecuteScalarAsyncCanceled()
        {
            Task t = new FinalizingCommand().ExecuteScalarAsync(new CancellationToken(true));
            await Assert.ThrowsAsync<TaskCanceledException>(() => t);
        }
    }
}
