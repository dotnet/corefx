// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;

namespace System.Linq
{
    public static partial class Enumerable
    {
        public static TSource Last<TSource>(this IEnumerable<TSource> source)
        {
            TSource result;
            if (TryLast(source, out result))
            {
                return result;
            }

            throw Error.NoElements();
        }

        public static TSource Last<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
        {
            TSource result;
            if (TryLast(source, predicate, out result))
            {
                return result;
            }

            throw Error.NoMatch();
        }

        public static TSource LastOrDefault<TSource>(this IEnumerable<TSource> source)
        {
            TSource result;
            TryLast(source, out result);
            return result;
        }

        public static TSource LastOrDefault<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
        {
            TSource result;
            TryLast(source, predicate, out result);
            return result;
        }

        public static bool TryLast<TSource>(this IEnumerable<TSource> source, out TSource element)
        {
            if (source == null)
            {
                throw Error.ArgumentNull(nameof(source));
            }

            IPartition<TSource> partition = source as IPartition<TSource>;
            if (partition != null)
            {
                bool found;
                element = partition.TryGetLast(out found);
                return found;
            }

            IList<TSource> list = source as IList<TSource>;
            if (list != null)
            {
                int count = list.Count;
                if (count > 0)
                {
                    element = list[count - 1];
                    return true;
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

                        element = result;
                        return true;
                    }
                }
            }

            element = default(TSource);
            return false;
        }

        public static bool TryLast<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate, out TSource element)
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
                bool found;
                element = ordered.TryGetLast(predicate, out found);
                return found;
            }

            IList<TSource> list = source as IList<TSource>;
            if (list != null)
            {
                for (int i = list.Count - 1; i >= 0; --i)
                {
                    TSource item = list[i];
                    if (predicate(item))
                    {
                        element = item;
                        return true;
                    }
                }
            }
            else
            {
                TSource result = default(TSource);
                bool found = false;

                foreach (TSource item in source)
                {
                    if (predicate(item))
                    {
                        result = item;
                        found = true;
                    }
                }

                element = result;
                return found;
            }

            element = default(TSource);
            return false;
        }
    }
}
