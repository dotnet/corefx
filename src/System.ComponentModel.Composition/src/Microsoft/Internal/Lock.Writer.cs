// -----------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Microsoft.Internal
{
    internal struct WriteLock : IDisposable
    {
        private readonly Lock _lock;
        private int _isDisposed;

        public WriteLock(Lock @lock)
        {
            this._isDisposed = 0;
            this._lock = @lock;
            this._lock.EnterWriteLock();
        }

        public void Dispose()
        {
            if (Interlocked.CompareExchange(ref this._isDisposed, 1, 0) == 0)
            {
                this._lock.ExitWriteLock();
            }
        }
    }
}
