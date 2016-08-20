﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;

namespace System.Linq
{
    public static partial class Enumerable
    {
        public static TSource Aggregate<TSource>(this IEnumerable<TSource> source, Func<TSource, TSource, TSource> func)
        {
            EnumerableHelpers.ThrowIfNull(source, nameof(source));
            EnumerableHelpers.ThrowIfNull(func, nameof(func));

            using (IEnumerator<TSource> e = source.GetEnumerator())
            {
                if (!e.MoveNext())
                {
                    throw Error.NoElements();
                }

                TSource result = e.Current;
                while (e.MoveNext())
                {
                    result = func(result, e.Current);
                }

                return result;
            }
        }

        public static TAccumulate Aggregate<TSource, TAccumulate>(this IEnumerable<TSource> source, TAccumulate seed, Func<TAccumulate, TSource, TAccumulate> func)
        {
            return source.Aggregate(seed, func, result => result);
        }

        public static TResult Aggregate<TSource, TAccumulate, TResult>(this IEnumerable<TSource> source, TAccumulate seed, Func<TAccumulate, TSource, TAccumulate> func, Func<TAccumulate, TResult> resultSelector)
        {
            EnumerableHelpers.ThrowIfNull(source, nameof(source));
            EnumerableHelpers.ThrowIfNull(func, nameof(func));
            EnumerableHelpers.ThrowIfNull(resultSelector, nameof(resultSelector));

            TAccumulate result = seed;
            foreach (TSource element in source)
            {
                result = func(result, element);
            }

            return resultSelector(result);
        }
    }
}
