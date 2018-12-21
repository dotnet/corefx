// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Diagnostics;

namespace System.Linq
{
    public static partial class Enumerable
    {
        public static IEnumerable<TResult> SelectMany<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, IEnumerable<TResult>> selector)
        {
            if (source == null)
            {
                throw Error.ArgumentNull(nameof(source));
            }

            if (selector == null)
            {
                throw Error.ArgumentNull(nameof(selector));
            }

            var selectMany = ChainLinq.Utils.PushTransform(source, new ChainLinq.Links.Select<TSource, IEnumerable<TResult>>(selector));
            return new ChainLinq.Consumables.SelectMany<TResult, TResult>(selectMany, ChainLinq.Links.Identity<TResult>.Instance);
        }

        public static IEnumerable<TResult> SelectMany<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, int, IEnumerable<TResult>> selector)
        {
            if (source == null)
            {
                throw Error.ArgumentNull(nameof(source));
            }

            if (selector == null)
            {
                throw Error.ArgumentNull(nameof(selector));
            }
            var selectMany = ChainLinq.Utils.PushTransform(source, new ChainLinq.Links.SelectIndexed<TSource, IEnumerable<TResult>>(selector));
            return new ChainLinq.Consumables.SelectMany<TResult, TResult>(selectMany, ChainLinq.Links.Identity<TResult>.Instance);
        }

        public static IEnumerable<TResult> SelectMany<TSource, TCollection, TResult>(this IEnumerable<TSource> source, Func<TSource, int, IEnumerable<TCollection>> collectionSelector, Func<TSource, TCollection, TResult> resultSelector)
        {
            if (source == null)
            {
                throw Error.ArgumentNull(nameof(source));
            }

            if (collectionSelector == null)
            {
                throw Error.ArgumentNull(nameof(collectionSelector));
            }

            if (resultSelector == null)
            {
                throw Error.ArgumentNull(nameof(resultSelector));
            }

            var selectMany = ChainLinq.Utils.PushTransform(source, new ChainLinq.Links.SelectManyIndexed<TSource, TCollection>(collectionSelector));
            return new ChainLinq.Consumables.SelectMany<TSource, TCollection, TResult, TResult>(selectMany, resultSelector, ChainLinq.Links.Identity<TResult>.Instance);
        }

        public static IEnumerable<TResult> SelectMany<TSource, TCollection, TResult>(this IEnumerable<TSource> source, Func<TSource, IEnumerable<TCollection>> collectionSelector, Func<TSource, TCollection, TResult> resultSelector)
        {
            if (source == null)
            {
                throw Error.ArgumentNull(nameof(source));
            }

            if (collectionSelector == null)
            {
                throw Error.ArgumentNull(nameof(collectionSelector));
            }

            if (resultSelector == null)
            {
                throw Error.ArgumentNull(nameof(resultSelector));
            }

            var selectMany = ChainLinq.Utils.PushTransform(source, new ChainLinq.Links.SelectMany<TSource, TCollection>(collectionSelector));
            return new ChainLinq.Consumables.SelectMany<TSource, TCollection, TResult, TResult>(selectMany, resultSelector, ChainLinq.Links.Identity<TResult>.Instance);
        }

    }
}
