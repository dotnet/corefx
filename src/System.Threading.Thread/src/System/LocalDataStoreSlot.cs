// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading;

namespace System
{
    public sealed class LocalDataStoreSlot
    {
        private readonly ThreadLocal<object> _data;

        internal LocalDataStoreSlot(ThreadLocal<object> data)
        {
            _data = data;
            GC.SuppressFinalize(this);
        }

        internal ThreadLocal<object> Data => _data;

        ~LocalDataStoreSlot()
        {
        }
    }
}
