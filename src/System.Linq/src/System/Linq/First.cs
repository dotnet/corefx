// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;

namespace System.Linq
{
    public static partial class Enumerable
    {
        public static TSource First<TSource>(this IEnumerable<TSource> source)
        {
            TSource result;
            if (TryFirst(source, out result))
                return result;

            throw Error.NoElements();
        }

        public static TSource First<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
        {
            TSource result;
            if (TryFirst(source, predicate, out result))
                return result;

            throw Error.NoMatch();
        }

        public static TSource FirstOrDefault<TSource>(this IEnumerable<TSource> source)
        {
            TSource result;
            TryFirst(source, out result);
            return result;
        }

        public static TSource FirstOrDefault<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
        {
            TSource result;
            TryFirst(source, predicate, out result);
            return result;
        }

        public static bool TryFirst<TSource>(this IEnumerable<TSource> source, out TSource element)
        {
            if (source == null)
            {
                throw Error.ArgumentNull(nameof(source));
            }

            IPartition<TSource> partition = source as IPartition<TSource>;
            if (partition != null)
            {
                bool found;
                element = partition.TryGetFirst(out found);
                return found;
            }

            IList<TSource> list = source as IList<TSource>;
            if (list != null)
            {
                if (list.Count > 0)
                {
                    element = list[0];
                    return true;
                }
            }
            else
            {
                foreach (TSource item in source)
                {
                    element = item;
                    return true;
                }
            }

            element = default(TSource);
            return false;
        }

        public static bool TryFirst<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate, out TSource element)
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
                element = ordered.TryGetFirst(predicate, out found);
                return found;
            }
            else
            {
                foreach (TSource item in source)
                {
                    if (predicate(item))
                    {
                        element = item;
                        return true;
                    }
                }
            }

            element = default(TSource);
            return false;
        }
    }
}
