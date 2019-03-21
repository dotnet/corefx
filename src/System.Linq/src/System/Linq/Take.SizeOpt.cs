// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Diagnostics;

namespace System.Linq
{
    public static partial class Enumerable
    {
        private static IEnumerable<TSource> TakeIterator<TSource>(IEnumerable<TSource> source, int count)
        {
            Debug.Assert(count > 0);

            foreach (TSource element in source)
            {
                yield return element;
                if (--count == 0) break;
            }
        }

        private static IEnumerable<TSource> TakeLastIterator<TSource>(IEnumerable<TSource> source, int count)
        {
            Debug.Assert(count > 0);

            if (source is ICollection<TSource> collection)
            {
                if (collection.Count > count)
                {
                    return collection.Skip(collection.Count - count);
                }
                else if (collection.Count > 0)
                {
                    return TakeIterator<TSource>(collection, collection.Count);
                }
                else
                {
                    return Empty<TSource>();
                }
            }
            else if (source is IReadOnlyCollection<TSource> roCollection)
            {
                if (roCollection.Count > count)
                {
                    return roCollection.Skip(roCollection.Count - count);
                }
                else if (roCollection.Count > 0)
                {
                    return TakeIterator<TSource>(roCollection, roCollection.Count);
                }
                else
                {
                    return Empty<TSource>();
                }
            }

            return TakeLastRegularIterator<TSource>(source, count);
        }
    }
}
