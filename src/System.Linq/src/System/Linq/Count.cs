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

            if (source is ChainLinq.Optimizations.ICountOnConsumable opt)
            {
                return opt.GetCount(false);
            }

            if (source is ICollection collection)
            {
                return collection.Count;
            }

            return ChainLinq.Utils.Consume(source, new ChainLinq.Consumer.Count<TSource>());
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

            return ChainLinq.Utils.Consume(source, new ChainLinq.Consumer.CountConditional<TSource>(predicate));
        }

        public static long LongCount<TSource>(this IEnumerable<TSource> source)
        {
            if (source == null)
            {
                throw Error.ArgumentNull(nameof(source));
            }

            return ChainLinq.Utils.Consume(source, new ChainLinq.Consumer.LongCount<TSource>());
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

            return ChainLinq.Utils.Consume(source, new ChainLinq.Consumer.LongCountConditional<TSource>(predicate));
        }
    }
}
