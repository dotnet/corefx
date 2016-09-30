// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System
{
    public sealed class LocalDataStoreSlot
    {
        private object m_data;

        public LocalDataStoreSlot(object data)
        {
            m_data = data;
        }

        public object Data => m_data;

        ~LocalDataStoreSlot()
        {
        }
    }
}
