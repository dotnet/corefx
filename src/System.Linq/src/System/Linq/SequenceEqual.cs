// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;

namespace System.Linq
{
    public static partial class Enumerable
    {
        public static bool SequenceEqual<TSource>(this IEnumerable<TSource> first, IEnumerable<TSource> second)
        {
            return SequenceEqual(first, second, null);
        }

        public static bool SequenceEqual<TSource>(this IEnumerable<TSource> first, IEnumerable<TSource> second, IEqualityComparer<TSource> comparer)
        {
            EnumerableHelpers.ThrowIfNull(comparer, nameof(comparer));
            EnumerableHelpers.ThrowIfNull(first, nameof(first));
            EnumerableHelpers.ThrowIfNull(second, nameof(second));

            ICollection<TSource> firstCol = first as ICollection<TSource>;
            if (firstCol != null)
            {
                ICollection<TSource> secondCol = second as ICollection<TSource>;
                if (secondCol != null && firstCol.Count != secondCol.Count)
                {
                    return false;
                }
            }

            using (IEnumerator<TSource> e1 = first.GetEnumerator())
            using (IEnumerator<TSource> e2 = second.GetEnumerator())
            {
                while (e1.MoveNext())
                {
                    if (!(e2.MoveNext() && comparer.Equals(e1.Current, e2.Current)))
                    {
                        return false;
                    }
                }

                return !e2.MoveNext();
            }
        }
    }
}
