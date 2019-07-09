﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace System.Linq
{
    public static partial class Enumerable
    {
        public static TSource Single<TSource>(this IEnumerable<TSource> source)
        {
            if (source == null)
            {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.source);
            }

            if (source is IList<TSource> list)
            {
                switch (list.Count)
                {
                    case 0:
                        ThrowHelper.ThrowNoElementsException();
                        return default;
                    case 1:
                        return list[0];
                }
            }
            else
            {
                using (IEnumerator<TSource> e = source.GetEnumerator())
                {
                    if (!e.MoveNext())
                    {
                        ThrowHelper.ThrowNoElementsException();
                    }

                    TSource result = e.Current;
                    if (!e.MoveNext())
                    {
                        return result;
                    }
                }
            }

            ThrowHelper.ThrowMoreThanOneElementException();
            return default(TSource);
        }

        public static TSource Single<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
        {
            if (!TryGetSingle(source, predicate, out TSource result))
            {
                ThrowHelper.ThrowNoMatchException();
            }

            return result;
        }

        [return: MaybeNull]
        public static TSource SingleOrDefault<TSource>(this IEnumerable<TSource> source)
        {
            if (source == null)
            {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.source);
            }

            if (source is IList<TSource> list)
            {
                switch (list.Count)
                {
                    case 0:
                        return default!;
                    case 1:
                        return list[0];
                }
            }
            else
            {
                using (IEnumerator<TSource> e = source.GetEnumerator())
                {
                    if (!e.MoveNext())
                    {
                        return default!;
                    }

                    TSource result = e.Current;
                    if (!e.MoveNext())
                    {
                        return result;
                    }
                }
            }

            ThrowHelper.ThrowMoreThanOneElementException();
            return default;
        }

        [return: MaybeNull]
        public static TSource SingleOrDefault<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
        {
            TryGetSingle(source, predicate, out TSource result);

            return result;
        }

        private static bool TryGetSingle<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate, out TSource result)
        {
            if (source == null)
            {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.source);
            }

            if (predicate == null)
            {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.predicate);
            }

            result = default!;
            bool found = false;

            if (source is TSource[] array)
            {
                foreach (TSource element in array)
                {
                    if (predicate(element))
                    {
                        if (found)
                        {
                            ThrowHelper.ThrowMoreThanOneMatchException();
                        }

                        result = element;
                        found = true;
                    }
                }
            }
            else if (source is List<TSource> list)
            {
                TSource element;
                for (int i = 0; i < list.Count; ++i)
                {
                    element = list[i];

                    if (predicate(element))
                    {
                        if (found)
                        {
                            ThrowHelper.ThrowMoreThanOneMatchException();
                        }

                        result = element;
                        found = true;
                    }
                }
            }
            else
            {
                foreach (TSource element in source)
                {
                    if (predicate(element))
                    {
                        if (found)
                        {
                            ThrowHelper.ThrowMoreThanOneMatchException();
                        }

                        result = element;
                        found = true;
                    }
                }
            }

            return found;
        }
    }
}
