// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;

namespace System.Linq
{
    public static partial class Enumerable
    {
        public static IEnumerable<TResult> Select<TSource, TResult>(
            this IEnumerable<TSource> source, Func<TSource, TResult> selector)
        {
            if (source == null)
            {
                throw Error.ArgumentNull(nameof(source));
            }

            if (selector == null)
            {
                throw Error.ArgumentNull(nameof(selector));
            }

            if (source is ChainLinq.ConsumableForMerging<TSource> consumable)
            {
                if (consumable.TailLink is ChainLinq.Optimizations.IMergeSelect<TSource> optimization)
                {
                    return optimization.MergeSelect(consumable, selector);
                }

                return consumable.AddTail(new ChainLinq.Links.Select<TSource, TResult>(selector));
            }
            else if (source is TSource[] array)
            {
                return new ChainLinq.Consumables.SelectArray<TSource, TResult>(array, selector);
            }
            else if (source is List<TSource> list)
            {
                return new ChainLinq.Consumables.SelectList<TSource, TResult>(list, selector);
            }
            else
            {
                return new ChainLinq.Consumables.SelectEnumerable<TSource, TResult>(source, selector);
            }
        }

        public static IEnumerable<TResult> Select<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, int, TResult> selector)
        {
            if (source == null)
            {
                throw Error.ArgumentNull(nameof(source));
            }

            if (selector == null)
            {
                throw Error.ArgumentNull(nameof(selector));
            }

            return ChainLinq.Utils.PushTUTransform(source, new ChainLinq.Links.SelectIndexed<TSource, TResult>(selector));
        }

    }
}
