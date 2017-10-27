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
    internal sealed class Lock : IDisposable
    {
#if (FEATURE_SLIMLOCK)
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

#else
        // ReaderWriterLockSlim is not yet implemented on SilverLight
        // Satisfies our requirements until it is implemented
        object _thisLock = new object();

        public Lock()
        {
        }

        public void EnterReadLock()
        {
            Monitor.Enter(this._thisLock);
        }

        public void EnterWriteLock()
        {
            Monitor.Enter(this._thisLock);
        }

        public void ExitReadLock()
        {
            Monitor.Exit(this._thisLock);
        }

        public void ExitWriteLock()
        {
            Monitor.Exit(this._thisLock);
        }

        public void Dispose()
        {
        }
#endif
    }
}
