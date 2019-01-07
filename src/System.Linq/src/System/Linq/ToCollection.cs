// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;

namespace System.Linq
{
    public static partial class Enumerable
    {
        public static TSource[] ToArray<TSource>(this IEnumerable<TSource> source)
        {
            if (source == null)
            {
                throw Error.ArgumentNull(nameof(source));
            }

            if (source is ChainLinq.Consumable<TSource> consumable)
            {
                if (source is ChainLinq.Optimizations.ICountOnConsumable counter)
                {
                    var count = counter.GetCount(true);
                    if (count >= 0)
                    {
                        return ChainLinq.Utils.Consume(source, new ChainLinq.Consumer.ToArrayKnownSize<TSource>(count));
                    }
                }

                return consumable.Consume(new ChainLinq.Consumer.ToArrayViaBuilder<TSource>());
            }

            return EnumerableHelpers.ToArray(source);
        }

        public static List<TSource> ToList<TSource>(this IEnumerable<TSource> source)
        {
            if (source == null)
            {
                throw Error.ArgumentNull(nameof(source));
            }

            if (source is ChainLinq.Consumable<TSource> consumable)
            {
                if (source is ChainLinq.Optimizations.ICountOnConsumable counter)
                {
                    var count = counter.GetCount(true);
                    if (count >= 0)
                    {
                        return ChainLinq.Utils.Consume(source, new ChainLinq.Consumer.ToList<TSource>(count));
                    }
                }

                return consumable.Consume(new ChainLinq.Consumer.ToList<TSource>());
            }

            return new List<TSource>(source);
        }

        public static Dictionary<TKey, TSource> ToDictionary<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector) =>
            ToDictionary(source, keySelector, null);

        public static Dictionary<TKey, TSource> ToDictionary<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, IEqualityComparer<TKey> comparer)
        {
            if (source == null)
            {
                throw Error.ArgumentNull(nameof(source));
            }

            if (keySelector == null)
            {
                throw Error.ArgumentNull(nameof(keySelector));
            }

            var consumable = ChainLinq.Utils.AsConsumable(source);

            if (consumable is ChainLinq.Optimizations.ICountOnConsumable counter)
            {
                var count = counter.GetCount(true);
                if (count >= 0)
                {
                    consumable.Consume(new ChainLinq.Consumer.ToDictionary<TSource, TKey>(keySelector, count, comparer));
                }
            }

            return consumable.Consume(new ChainLinq.Consumer.ToDictionary<TSource, TKey>(keySelector, comparer));
        }

        public static Dictionary<TKey, TElement> ToDictionary<TSource, TKey, TElement>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector) =>
            ToDictionary(source, keySelector, elementSelector, null);

        public static Dictionary<TKey, TElement> ToDictionary<TSource, TKey, TElement>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, IEqualityComparer<TKey> comparer)
        {
            if (source == null)
            {
                throw Error.ArgumentNull(nameof(source));
            }

            if (keySelector == null)
            {
                throw Error.ArgumentNull(nameof(keySelector));
            }

            if (elementSelector == null)
            {
                throw Error.ArgumentNull(nameof(elementSelector));
            }

            var consumable = ChainLinq.Utils.AsConsumable(source);

            if (consumable is ChainLinq.Optimizations.ICountOnConsumable counter)
            {
                var count = counter.GetCount(true);
                if (count >= 0)
                {
                    consumable.Consume(new ChainLinq.Consumer.ToDictionary<TSource, TKey, TElement>(keySelector, elementSelector, count, comparer));
                }
            }

            return consumable.Consume(new ChainLinq.Consumer.ToDictionary<TSource, TKey, TElement>(keySelector, elementSelector, comparer));
        }

        public static HashSet<TSource> ToHashSet<TSource>(this IEnumerable<TSource> source) => source.ToHashSet(comparer: null);

        public static HashSet<TSource> ToHashSet<TSource>(this IEnumerable<TSource> source, IEqualityComparer<TSource> comparer)
        {
            if (source == null)
            {
                throw Error.ArgumentNull(nameof(source));
            }

            // Don't pre-allocate based on knowledge of size, as potentially many elements will be dropped.
            return new HashSet<TSource>(source, comparer);
        }
    }
}
