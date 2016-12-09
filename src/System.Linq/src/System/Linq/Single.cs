// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;

namespace System.Linq
{
    public static partial class Enumerable
    {
        private enum TrySingleResult
        {
            NoElements,
            SingleElement,
            MoreThanOneElement
        }

        public static TSource Single<TSource>(this IEnumerable<TSource> source)
        {
            TSource result;
            switch (TrySingleCore(source, out result))
            {
                case TrySingleResult.NoElements:
                    throw Error.NoElements();

                case TrySingleResult.SingleElement:
                    return result;
            }

            throw Error.MoreThanOneElement();
        }

        public static TSource Single<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
        {
            TSource result;
            switch (TrySingleCore(source, predicate, out result))
            {
                case TrySingleResult.NoElements:
                    throw Error.NoMatch();

                case TrySingleResult.SingleElement:
                    return result;
            }

            throw Error.MoreThanOneMatch();
        }

        public static TSource SingleOrDefault<TSource>(this IEnumerable<TSource> source)
        {
            TSource result;
            switch (TrySingleCore(source, out result))
            {
                case TrySingleResult.NoElements:
                case TrySingleResult.SingleElement:
                    return result;
            }

            throw Error.MoreThanOneElement();
        }

        public static TSource SingleOrDefault<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
        {
            TSource result;
            switch (TrySingleCore(source, predicate, out result))
            {
                case TrySingleResult.NoElements:
                case TrySingleResult.SingleElement:
                    return result;
            }

            throw Error.MoreThanOneElement();
        }

        public static bool TrySingle<TSource>(this IEnumerable<TSource> source, out TSource element)
        {
            return TrySingleCore(source, out element) == TrySingleResult.SingleElement;
        }

        public static bool TrySingle<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate, out TSource element)
        {
            return TrySingleCore(source, predicate, out element) == TrySingleResult.SingleElement;
        }

        private static TrySingleResult TrySingleCore<TSource>(IEnumerable<TSource> source, out TSource element)
        {
            if (source == null)
            {
                throw Error.ArgumentNull(nameof(source));
            }

            IList<TSource> list = source as IList<TSource>;
            if (list != null)
            {
                switch (list.Count)
                {
                    case 0:
                        {
                            element = default(TSource);
                            return TryGetSingleCoreResult.NoElements;
                        }
                    case 1:
                        {
                            element = list[0];
                            return TryGetSingleCoreResult.OneElement;
                        }
                }
            }
            else
            {
                using (IEnumerator<TSource> e = source.GetEnumerator())
                {
                    if (!e.MoveNext())
                    {
                        element = default(TSource);
                        return TryGetSingleCoreResult.NoElements;
                    }

                    TSource item = e.Current;
                    if (!e.MoveNext())
                    {
                        element = item;
                        return TryGetSingleCoreResult.OneElement;
                    }
                }
            }

            element = default(TSource);
            return TryGetSingleCoreResult.MoreThanOneElement;
        }

        private static TrySingleResult TrySingleCore<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate, out TSource element)
        {
            if (source == null)
            {
                throw Error.ArgumentNull(nameof(source));
            }

            if (predicate == null)
            {
                throw Error.ArgumentNull(nameof(predicate));
            }

            using (IEnumerator<TSource> e = source.GetEnumerator())
            {
                while (e.MoveNext())
                {
                    TSource item = e.Current;
                    if (predicate(item))
                    {
                        while (e.MoveNext())
                        {
                            if (predicate(e.Current))
                            {
                                element = default(TSource);
                                return TrySingleResult.MoreThanOneElement;
                            }
                        }

                        element = item;
                        return TrySingleResult.SingleElement;
                    }
                }
            }

            element = default(TSource);
            return TrySingleResult.NoElements;
        }
    }
}
