// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Diagnostics;
using static System.Linq.Utilities;

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

            if (source is ChainLinq.ConsumableForMerging<TSource> merger)
            {
                if (merger.TailLink is ChainLinq.Optimizations.IMergeSelect<TSource> selectMerge)
                {
                    return selectMerge.MergeSelect(merger, selector);
                }

                return merger.AddTail(CreateSelectLink(selector));
            }

            return ChainLinq.Utils.PushTransform(source, CreateSelectLink(selector));
        }

        private static ChainLinq.Links.Select<TSource, TResult> CreateSelectLink<TSource, TResult>(Func<TSource, TResult> selector)
        {
            return new ChainLinq.Links.Select<TSource, TResult>(selector);
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

            return ChainLinq.Utils.PushTransform(source, new ChainLinq.Links.SelectIndexed<TSource, TResult>(selector));
        }

    }
}
