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
            return default;
        }

        public static TSource Single<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
        {
            if (source == null)
            {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.source);
            }

            if (predicate == null)
            {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.predicate);
            }

            var result = TryGetSingle(source, predicate, out bool found, out bool foundSingle);

            if (!foundSingle)
            {
                ThrowHelper.ThrowMoreThanOneMatchException();
                return default;
            }

            if (!found)
            {
                ThrowHelper.ThrowNoMatchException();
                return default;
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
            if (source == null)
            {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.source);
            }

            if (predicate == null)
            {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.predicate);
            }

            var result = TryGetSingle(source, predicate, out bool found, out bool foundSingle);

            if (!foundSingle)
            {
                ThrowHelper.ThrowMoreThanOneMatchException();

                return default;
            }

            return result;
        }

        private static TSource TryGetSingle<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate, out bool found, out bool foundSingle)
        {
            if (source == null)
            {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.source);
            }

            if (predicate == null)
            {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.predicate);
            }

            TSource result = default!;
            found = false;

            if (source is TSource[] array)
            {
                foreach (TSource element in array)
                {
                    if (predicate(element))
                    {
                        if (found)
                        {
                            foundSingle = false;
                            return default!;
                        }

                        found = true;
                        result = element;
                    }
                }

                foundSingle = true;
            }
            else if (source is List<TSource> list)
            {
                foreach (TSource element in list)
                {
                    if (predicate(element))
                    {
                        if (found)
                        {
                            foundSingle = false;
                            return default!;
                        }

                        found = true;
                        result = element;
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
                            foundSingle = false;
                            return default!;
                        }

                        found = true;
                        result = element;
                    }
                }
            }

            foundSingle = true;
            return result;
        }
    }
}
