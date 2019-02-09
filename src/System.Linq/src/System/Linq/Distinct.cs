// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;

namespace System.Linq
{
    public static partial class Enumerable
    {
        public static IEnumerable<TSource> Distinct<TSource>(this IEnumerable<TSource> source) => Distinct(source, null);

        public static IEnumerable<TSource> Distinct<TSource>(this IEnumerable<TSource> source, IEqualityComparer<TSource> comparer)
        {
            if (source == null)
            {
                throw Error.ArgumentNull(nameof(source));
            }

            var distinctLink =
                (comparer == null || ReferenceEquals(comparer, EqualityComparer<TSource>.Default))
                    ? ChainLinq.Links.DistinctDefaultComparer<TSource>.Instance
                    : new ChainLinq.Links.Distinct<TSource>(comparer);

            return ChainLinq.Utils.PushTTTransform(source, distinctLink);
        }
    }
}
