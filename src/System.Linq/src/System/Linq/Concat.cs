// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Diagnostics;

namespace System.Linq
{
    public static partial class Enumerable
    {
        public static IEnumerable<TSource> Concat<TSource>(this IEnumerable<TSource> first, IEnumerable<TSource> second)
        {
            if (first == null)
            {
                throw Error.ArgumentNull(nameof(first));
            }

            if (second == null)
            {
                throw Error.ArgumentNull(nameof(second));
            }

            if (first is ChainLinq.Consumables.Concat<TSource, TSource> forAppending)
            {
                return forAppending.Append(second);
            }
            else if (second is ChainLinq.Consumables.Concat<TSource, TSource> forPrepending)
            {
                return forPrepending.Prepend(first);
            }

            return new ChainLinq.Consumables.Concat<TSource, TSource>(null, first, second, ChainLinq.Links.Identity<TSource>.Instance);
        }
    }
}
