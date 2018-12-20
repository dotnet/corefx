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
            if (source == null)
            {
                throw Error.ArgumentNull(nameof(source));
            }

            if (source is ICollection<TSource> collectionoft)
            {
                return collectionoft.Count;
            }

#if !PRE_CHAINLINQ
            if (source is ChainLinq.Optimizations.ICountOnConsumable opt)
            {
                return opt.GetCount(false);
            }

            if (source is ICollection collection)
            {
                return collection.Count;
            }

            return ChainLinq.Utils.Consume(source, new ChainLinq.Consumer.Count<TSource>());
#else
            if (source is IIListProvider<TSource> listProv)
            {
                return listProv.GetCount(onlyIfCheap: false);
            }

            if (source is ICollection collection)
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
#endif
        }

        public static int Count<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
        {
            if (source == null)
            {
                throw Error.ArgumentNull(nameof(source));
            }

            if (predicate == null)
            {
                throw Error.ArgumentNull(nameof(predicate));
            }

#if !PRE_CHAINLINQ
            return ChainLinq.Utils.Consume(source, new ChainLinq.Consumer.CountConditional<TSource>(predicate));
#else
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
#endif
        }

        public static long LongCount<TSource>(this IEnumerable<TSource> source)
        {
            if (source == null)
            {
                throw Error.ArgumentNull(nameof(source));
            }

#if !PRE_CHAINLINQ
            return ChainLinq.Utils.Consume(source, new ChainLinq.Consumer.LongCount<TSource>());
#else
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
#endif
        }

        public static long LongCount<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
        {
            if (source == null)
            {
                throw Error.ArgumentNull(nameof(source));
            }

            if (predicate == null)
            {
                throw Error.ArgumentNull(nameof(predicate));
            }

#if !PRE_CHAINLINQ
            return ChainLinq.Utils.Consume(source, new ChainLinq.Consumer.LongCountConditional<TSource>(predicate));
#else

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
#endif
        }
    }
}
