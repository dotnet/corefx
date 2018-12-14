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

#if PRE_CHAINLINQ
            using (IEnumerator<TSource> e = source.GetEnumerator())
            {
                return e.MoveNext();
            }
#else
            return ChainLinq.Utils.Consume(source, new ChainLinq.Consumer.Any<TSource>(_ => true));
#endif
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

#if PRE_CHAINLINQ
            foreach (TSource element in source)
            {
                if (predicate(element))
                {
                    return true;
                }
            }

            return false;
#else
            return ChainLinq.Utils.Consume(source, new ChainLinq.Consumer.Any<TSource>(predicate));
#endif
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

#if PRE_CHAINLINQ
            foreach (TSource element in source)
            {
                if (!predicate(element))
                {
                    return false;
                }
            }

            return true;
#else
            return ChainLinq.Utils.Consume(source, new ChainLinq.Consumer.All<TSource>(predicate));
#endif

        }
    }
}
