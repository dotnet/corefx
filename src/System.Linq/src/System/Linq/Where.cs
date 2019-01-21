// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;

namespace System.Linq
{
    public static partial class Enumerable
    {
        public static IEnumerable<TSource> Where<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
        {
            if (source == null)
            {
                throw Error.ArgumentNull(nameof(source));
            }

            if (predicate == null)
            {
                throw Error.ArgumentNull(nameof(predicate));
            }

            if (source is ChainLinq.ConsumableForMerging<TSource> consumable)
            {
                if (consumable.TailLink is ChainLinq.Optimizations.IMergeWhere<TSource> optimization)
                {
                    return optimization.MergeWhere(consumable, predicate);
                }

                return consumable.AddTail(new ChainLinq.Links.Where<TSource>(predicate));
            }
            else if (source is TSource[] array)
            {
                return new ChainLinq.Consumables.WhereArray<TSource>(array, predicate);
            }
            else if (source is List<TSource> list)
            {
                return new ChainLinq.Consumables.WhereList<TSource>(list, predicate);
            }
            else
            {
                return new ChainLinq.Consumables.WhereEnumerable<TSource>(source, predicate);
            }
        }

        public static IEnumerable<TSource> Where<TSource>(this IEnumerable<TSource> source, Func<TSource, int, bool> predicate)
        {
            if (source == null)
            {
                throw Error.ArgumentNull(nameof(source));
            }

            if (predicate == null)
            {
                throw Error.ArgumentNull(nameof(predicate));
            }

            return ChainLinq.Utils.PushTTTransform(source, new ChainLinq.Links.WhereIndexed<TSource>(predicate));
        }
    }
}
