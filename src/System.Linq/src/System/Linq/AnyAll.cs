// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;

namespace System.Linq
{
    public static partial class Enumerable
    {
        public static bool Any<TSource>(this IEnumerable<TSource> source)
        {
            if (source == null)
            {
                throw Error.ArgumentNull(nameof(source));
            }

            if (source is ICollection<TSource> collection)
            {
                return collection.Count > 0;
            }

            using (IEnumerator<TSource> e = source.GetEnumerator())
            {
                return e.MoveNext();
            }
        }

        public static bool Any<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
        {
            if (source == null)
            {
                throw Error.ArgumentNull(nameof(source));
            }

            if (predicate == null)
            {
                throw Error.ArgumentNull(nameof(predicate));
            }

            if (source is IList<TSource> list)
            {
                for (var i = 0; i < list.Count; i++)
                {
                    if (predicate(list[i]))
                    {
                        return true;
                    }
                }
            }
            else
            {
                foreach (TSource element in source)
                {
                    if (predicate(element))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public static bool All<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
        {
            if (source == null)
            {
                throw Error.ArgumentNull(nameof(source));
            }

            if (predicate == null)
            {
                throw Error.ArgumentNull(nameof(predicate));
            }

            if (source is IList<TSource> list)
            {
                for (var i = 0; i < list.Count; i++)
                {
                    if (!predicate(list[i]))
                    {
                        return false;
                    }
                }
            }
            else
            {
                foreach (TSource element in source)
                {
                    if (!predicate(element))
                    {
                        return false;
                    }
                }
            }

            return true;
        }
    }
}
