// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.Data.Common.Tests
{
    class DbTransactionTests
    {
        private static volatile bool _wasFinalized;

        private class FinalizingTransaction : DbTransaction
        {
            public static void CreateAndRelease()
            {
                new FinalizingTransaction();
            }

            protected override void Dispose(bool disposing)
            {
                if (!disposing)
                    _wasFinalized = true;
                base.Dispose(disposing);
            }

            public override IsolationLevel IsolationLevel
            {
                get
                {
                    throw new NotImplementedException();
                }
            }

            protected override DbConnection DbConnection
            {
                get
                {
                    throw new NotImplementedException();
                }
            }

            public override void Commit()
            {
                throw new NotImplementedException();
            }

            public override void Rollback()
            {
                throw new NotImplementedException();
            }
        }

        [Fact]
        public void CanBeFinalized()
        {
            FinalizingTransaction.CreateAndRelease();
            GC.Collect();
            GC.WaitForPendingFinalizers();
            Assert.True(_wasFinalized);
        }
    }
}
