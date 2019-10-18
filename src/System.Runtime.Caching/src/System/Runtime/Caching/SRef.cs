﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Globalization;
using System.Reflection;
using System.Security;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace System.Runtime.Caching
{
    // until then we provide a stub
    internal class SRefMultiple
    {
        internal SRefMultiple(object[] targets)
        {
        }
        internal long ApproximateSize => 0;
        internal void Dispose()
        {
        }
    }

    internal class GCHandleRef<T> : IDisposable
    where T : class, IDisposable
    {
        private GCHandle _handle;
        private T _t;

        public GCHandleRef(T t)
        {
            _handle = GCHandle.Alloc(t);
        }

        public T Target
        {
            get
            {
                try
                {
                    T t = (T)_handle.Target;
                    if (t != null)
                    {
                        return t;
                    }
                }
                catch (InvalidOperationException)
                {
                    // use the normal reference instead of throwing an exception when _handle is already freed
                }
                return _t;
            }
        }

        public void Dispose()
        {
            Target.Dispose();
            // Safe to call Dispose more than once but not thread-safe
            if (_handle.IsAllocated)
            {
                // We must free the GC handle to avoid leaks.
                // However after _handle is freed we no longer have access to its Target
                // which will cause AVs and various concurrency issues under stress.
                // We revert to using normal references after disposing the GC handle
                _t = (T)_handle.Target;
                _handle.Free();
            }
        }
    }
}
