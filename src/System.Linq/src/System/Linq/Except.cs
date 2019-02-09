// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;

namespace System.Linq
{
    public static partial class Enumerable
    {
        public static IEnumerable<TSource> Except<TSource>(this IEnumerable<TSource> first, IEnumerable<TSource> second)
        {
            if (first == null)
            {
                throw Error.ArgumentNull(nameof(first));
            }

            if (second == null)
            {
                throw Error.ArgumentNull(nameof(second));
            }

            return ExceptConsumer(first, second, null);
        }

        public static IEnumerable<TSource> Except<TSource>(this IEnumerable<TSource> first, IEnumerable<TSource> second, IEqualityComparer<TSource> comparer)
        {
            if (first == null)
            {
                throw Error.ArgumentNull(nameof(first));
            }

            if (second == null)
            {
                throw Error.ArgumentNull(nameof(second));
            }

            return ExceptConsumer(first, second, comparer);
        }

        private static IEnumerable<TSource> ExceptConsumer<TSource>(IEnumerable<TSource> first, IEnumerable<TSource> second, IEqualityComparer<TSource> comparer)
        {
            ChainLinq.Link<TSource, TSource> exceptLink =
                (comparer == null || ReferenceEquals(comparer, EqualityComparer<TSource>.Default))
                    ? (ChainLinq.Link<TSource, TSource>) new ChainLinq.Links.ExceptDefaultComparer<TSource>(second)
                    : (ChainLinq.Link<TSource, TSource>) new ChainLinq.Links.Except<TSource>(comparer, second);

            return ChainLinq.Utils.PushTTTransform(first, exceptLink);
        }
    }
}
