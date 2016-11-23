// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;

namespace System.Collections.Generic
{
    /// <summary>Internal helper functions for working with enumerables.</summary>
    internal static partial class EnumerableHelpers
    {
        /// <summary>Gets the count of the specified enumerable.</summary>
        /// <param name="source">The enumerable to count.</param>
        /// <returns>The count of the enumerable.</returns>
        internal static int Count<T>(IEnumerable<T> source)
        {
            Debug.Assert(source != null);

            var collectionOfT = source as ICollection<T>;
            if (collectionOfT != null)
            {
                return collectionOfT.Count;
            }
            
            var collection = source as ICollection;
            if (collection != null)
            {
                return collection.Count;
            }
            
            int count = 0;
            using (IEnumerator<T> e = source.GetEnumerator())
            {
                checked
                {
                    while (e.MoveNext())
                    {
                        count++;
                    }
                }
            }

            return count;
        }
    }
}
