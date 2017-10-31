// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Microsoft.Internal
{
    internal sealed class Lock : IDisposable
    {
        private ReaderWriterLockSlim _thisLock = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);
        private int _isDisposed = 0;
        public void EnterReadLock()
        {
            this._thisLock.EnterReadLock();
        }

        public void EnterWriteLock()
        {
            this._thisLock.EnterWriteLock();
        }

        public void ExitReadLock()
        {
            this._thisLock.ExitReadLock();
        }

        public void ExitWriteLock()
        {
            this._thisLock.ExitWriteLock();
        }

        public void Dispose()
        {
            if (Interlocked.CompareExchange(ref this._isDisposed, 1, 0) == 0)
            {
                this._thisLock.Dispose();
            }
        }
    }
}
