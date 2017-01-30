﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;

namespace System.Linq
{
    public static partial class Enumerable
    {
        public static TSource Last<TSource>(this IEnumerable<TSource> source)
        {
            bool found;
            TSource last = source.TryGetLast(out found);
            
            if (!found)
            {
                throw Error.NoElements();
            }

            return last;
        }

        public static TSource Last<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
        {
            bool found;
            TSource last = source.TryGetLast(predicate, out found);

            if (!found)
            {
                throw Error.NoMatch();
            }

            return last;
        }

        public static TSource LastOrDefault<TSource>(this IEnumerable<TSource> source)
        {
            bool found;
            return source.TryGetLast(out found);
        }

        public static TSource LastOrDefault<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
        {
            bool found;
            return source.TryGetLast(predicate, out found);
        }

        internal static TSource TryGetLast<TSource>(this IEnumerable<TSource> source, out bool found)
        {
            if (source == null)
            {
                throw Error.ArgumentNull(nameof(source));
            }

            IPartition<TSource> partition = source as IPartition<TSource>;
            if (partition != null)
            {
                return partition.TryGetLast(out found);
            }
            
            IList<TSource> list = source as IList<TSource>;
            if (list != null)
            {
                int count = list.Count;
                if (count > 0)
                {
                    found = true;
                    return list[count - 1];
                }
            }
            else
            {
                using (IEnumerator<TSource> e = source.GetEnumerator())
                {
                    if (e.MoveNext())
                    {
                        TSource result;
                        do
                        {
                            result = e.Current;
                        }
                        while (e.MoveNext());

                        found = true;
                        return result;
                    }
                }
            }
            
            found = false;
            return default(TSource);
        }

        internal static TSource TryGetLast<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate, out bool found)
        {
            if (source == null)
            {
                throw Error.ArgumentNull(nameof(source));
            }

            if (predicate == null)
            {
                throw Error.ArgumentNull(nameof(predicate));
            }

            OrderedEnumerable<TSource> ordered = source as OrderedEnumerable<TSource>;
            if (ordered != null)
            {
                return ordered.TryGetLast(predicate, out found);
            }

            IList<TSource> ilist = source as IList<TSource>;
            if (ilist != null)
            {
                var array = source as TSource[];
                if (array != null)
                {
                    return EnumerableHelpers.TryGetLast(predicate, out found, array: array);
                }

                var list = source as List<TSource>;
                if (list != null)
                {
                    return EnumerableHelpers.TryGetLast(predicate, out found, list: list);
                }

                return EnumerableHelpers.TryGetLast(predicate, out found, ilist: ilist);
            }
            
            return EnumerableHelpers.TryGetLast(predicate, out found, source: source);
        }
    }
}
