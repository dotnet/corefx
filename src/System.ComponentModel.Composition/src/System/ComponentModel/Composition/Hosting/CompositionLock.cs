﻿// -----------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
#define SINGLETHREADEDLOCKENFORCEMENT
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Primitives;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using Microsoft.Internal;
using Microsoft.Internal.Collections;
using System.Threading;

namespace System.ComponentModel.Composition.Hosting
{
    // This a a lock class that needs to be held in order to perform any mutation of the parts/parts state in the composition
    // Today's implementation relies on the AppDomain-wide re-entrant lock for changes on the composition, and a narrow lock for changes in 
    // the state of the specific ImportEngine
    // Today we make several assumptions to ensure thread-safety:
    // 1. Each composition doesn't change lock affinity
    // 2. Every part of the system that updates the status of the parts (in our case ImportEngine) needs to hold the same wide - lock
    // 3. State of the import engine that gets accessed outside of the wide lock needs to be accessed in the context of the narrow lock
    // 4. Narrow lock CAN be taken inside the wide lock
    // 5. Wide lock CANNOT be taken inside the narrow lock
    // 6. No 3rd party code will EVER get called inside the narrow lock
    // Sadly, this means that we WILL be calling 3rd party code under a lock, but as long as the lock is re-entrant and they can't invoke us on anotehr thread
    // we have no issue, other than potential overlocking
    internal sealed class CompositionLock : IDisposable
    {
        // narrow lock
        private readonly Lock _stateLock = null;
        // wide lock
        private static object _compositionLock = new object();

        private int _isDisposed = 0;
        private bool _isThreadSafe = false;

        private static readonly EmptyLockHolder _EmptyLockHolder = new EmptyLockHolder();

        public CompositionLock(bool isThreadSafe)
        {
            this._isThreadSafe = isThreadSafe;
            if (isThreadSafe)
            {
                this._stateLock = new Lock();
            }
        }

        public void Dispose()
        {
            if (this._isThreadSafe)
            {
                if (Interlocked.CompareExchange(ref this._isDisposed, 1, 0) == 0)
                {
                    this._stateLock.Dispose();
                }
            }
        }

        public bool IsThreadSafe
        {
            get
            {
                return this._isThreadSafe;
            }
        }

        private void EnterCompositionLock()
        {
#pragma warning disable 618
            if (this._isThreadSafe)
            {
                Monitor.Enter(_compositionLock);
            }
#pragma warning restore 618
        }

        private void ExitCompositionLock()
        {
            if (this._isThreadSafe)
            {
                Monitor.Exit(_compositionLock);
            }
        }

        public IDisposable LockComposition()
        {
            if (this._isThreadSafe)
            {
                return new CompositionLockHolder(this);
            }
            else
            {
                return _EmptyLockHolder;
            }
        }

        public IDisposable LockStateForRead()
        {
            if (this._isThreadSafe)
            {
                return new ReadLock(this._stateLock);
            }
            else
            {
                return _EmptyLockHolder;
            }            
        }

        public IDisposable LockStateForWrite()
        {
            if (this._isThreadSafe)
            {
                return new WriteLock(this._stateLock);
            }
            else
            {
                return _EmptyLockHolder;
            }   
        }

        // NOTE : this should NOT be changed to a struct as ImportEngine relies on it
        public sealed class CompositionLockHolder : IDisposable
        {
            private CompositionLock _lock;
            private int _isDisposed;

            public CompositionLockHolder(CompositionLock @lock)
            {
                this._lock = @lock;

                this._isDisposed = 0;
                this._lock.EnterCompositionLock();
            }

            public void Dispose()
            {
                if (Interlocked.CompareExchange(ref this._isDisposed, 1, 0) == 0)
                {
                    this._lock.ExitCompositionLock();
                }
            }
        }

        private sealed class EmptyLockHolder : IDisposable
        {
            public void Dispose()
            {
            }
        }
    }
}
