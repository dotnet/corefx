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

            if (source is ChainLinq.ConsumableForMerging<TSource> merger)
            {
                if (merger.TailLink is ChainLinq.Optimizations.IMergeWhere<TSource> whereMerge)
                {
                    return whereMerge.MergeWhere(merger, predicate);
                }

                return merger.AddTail(CreateWhereLink(predicate));
            }

            return ChainLinq.Utils.PushTransform(source, CreateWhereLink(predicate));
        }

        private static ChainLinq.Links.Where<TSource> CreateWhereLink<TSource>(Func<TSource, bool> predicate)
        {
            return new ChainLinq.Links.Where<TSource>(predicate);
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

            return ChainLinq.Utils.PushTransform(source, new ChainLinq.Links.WhereIndexed<TSource>(predicate));
        }
    }
}
