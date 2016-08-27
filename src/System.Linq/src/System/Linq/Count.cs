// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Collections.Generic;

namespace System.Linq
{
    public static partial class Enumerable
    {
        public static int Count<TSource>(this IEnumerable<TSource> source)
        {
            EnumerableHelpers.ThrowIfNull(source, nameof(source));

            ICollection<TSource> collectionoft = source as ICollection<TSource>;
            if (collectionoft != null)
            {
                return collectionoft.Count;
            }

            IIListProvider<TSource> listProv = source as IIListProvider<TSource>;
            if (listProv != null)
            {
                return listProv.GetCount(onlyIfCheap: false);
            }

            ICollection collection = source as ICollection;
            if (collection != null)
            {
                return collection.Count;
            }

            int count = 0;
            using (IEnumerator<TSource> e = source.GetEnumerator())
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

        public static int Count<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
        {
            EnumerableHelpers.ThrowIfNull(source, nameof(source));
            EnumerableHelpers.ThrowIfNull(predicate, nameof(predicate));

            int count = 0;
            foreach (TSource element in source)
            {
                checked
                {
                    if (predicate(element))
                    {
                        count++;
                    }
                }
            }

            return count;
        }

        public static long LongCount<TSource>(this IEnumerable<TSource> source)
        {
            EnumerableHelpers.ThrowIfNull(source, nameof(source));

            long count = 0;
            using (IEnumerator<TSource> e = source.GetEnumerator())
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

        public static long LongCount<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
        {
            EnumerableHelpers.ThrowIfNull(source, nameof(source));
            EnumerableHelpers.ThrowIfNull(predicate, nameof(predicate));

            long count = 0;
            foreach (TSource element in source)
            {
                checked
                {
                    if (predicate(element))
                    {
                        count++;
                    }
                }
            }

            return count;
        }
    }
}
