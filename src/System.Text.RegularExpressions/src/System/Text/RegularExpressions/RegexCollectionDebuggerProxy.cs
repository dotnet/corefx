// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Diagnostics;

namespace System.Text.RegularExpressions
{
    internal sealed class RegexCollectionDebuggerProxy<T>
    {
        private readonly ICollection<T> _collection;

        public RegexCollectionDebuggerProxy(ICollection<T> collection)
        {
            if (collection == null)
                throw new ArgumentNullException("collection");

            _collection = collection;
        }

        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public T[] Items
        {
            get
            {
                T[] items = new T[_collection.Count];
                _collection.CopyTo(items, 0);
                return items;
            }
        }
    }
}
