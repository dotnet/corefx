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
            if (source == null)
            {
                throw Error.ArgumentNull(nameof(source));
            }

            if (func == null)
            {
                throw Error.ArgumentNull(nameof(func));
            }

            return ChainLinq.Utils.Consume(source, new ChainLinq.Consumer.Reduce<TSource>(func));
        }

        public static TAccumulate Aggregate<TSource, TAccumulate>(this IEnumerable<TSource> source, TAccumulate seed, Func<TAccumulate, TSource, TAccumulate> func)
        {
            if (source == null)
            {
                throw Error.ArgumentNull(nameof(source));
            }

            if (func == null)
            {
                throw Error.ArgumentNull(nameof(func));
            }

            return ChainLinq.Utils.Consume(source, new ChainLinq.Consumer.Aggregate<TSource, TAccumulate, TAccumulate>(seed, func, x=>x));
        }

        public static TResult Aggregate<TSource, TAccumulate, TResult>(this IEnumerable<TSource> source, TAccumulate seed, Func<TAccumulate, TSource, TAccumulate> func, Func<TAccumulate, TResult> resultSelector)
        {
            if (source == null)
            {
                throw Error.ArgumentNull(nameof(source));
            }

            if (func == null)
            {
                throw Error.ArgumentNull(nameof(func));
            }

            if (resultSelector == null)
            {
                throw Error.ArgumentNull(nameof(resultSelector));
            }

            return ChainLinq.Utils.Consume(source, new ChainLinq.Consumer.Aggregate<TSource, TAccumulate, TResult>(seed, func, resultSelector));
        }
    }
}
