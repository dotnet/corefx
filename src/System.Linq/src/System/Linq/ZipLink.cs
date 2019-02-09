// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;

namespace System.Linq
{
    public static partial class Enumerable
    {
        public static IEnumerable<TResult> ZipLink<T, TResult>(this IEnumerable<T> source, Func<T, T, TResult> resultSelector)
        {
            if (source is null)
            {
                throw Error.ArgumentNull(nameof(source));
            }

            if (resultSelector is null)
            {
                throw Error.ArgumentNull(nameof(resultSelector));
            }

            return ZipLinkIterator(source, resultSelector);
        }

        private static IEnumerable<TResult> ZipLinkIterator<T, TResult>(IEnumerable<T> source, Func<T, T, TResult> resultSelector)
        {
            using (IEnumerator<T> e = source.GetEnumerator())
            {
                if (e.MoveNext())
                {
                    T last = e.Current;
                    while (e.MoveNext())
                    {
                        yield return resultSelector(last, last = e.Current);
                    }
                }
            }
        }
    }
}
