// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;

namespace System.Linq
{
    public static partial class Enumerable
    {
        public static int Max(this IEnumerable<int> source)
        {
            if (source == null)
            {
                throw Error.ArgumentNull(nameof(source));
            }

            return ChainLinq.Utils.Consume(source, new ChainLinq.Consumer.MaxInt());
        }

        public static int? Max(this IEnumerable<int?> source)
        {
            if (source == null)
            {
                throw Error.ArgumentNull(nameof(source));
            }

            return ChainLinq.Utils.Consume(source, new ChainLinq.Consumer.MaxNullableInt());
        }

        public static long Max(this IEnumerable<long> source)
        {
            if (source == null)
            {
                throw Error.ArgumentNull(nameof(source));
            }

            return ChainLinq.Utils.Consume(source, new ChainLinq.Consumer.MaxLong());
        }

        public static long? Max(this IEnumerable<long?> source)
        {
            if (source == null)
            {
                throw Error.ArgumentNull(nameof(source));
            }

            return ChainLinq.Utils.Consume(source, new ChainLinq.Consumer.MaxNullableLong());
        }

        public static double Max(this IEnumerable<double> source)
        {
            if (source == null)
            {
                throw Error.ArgumentNull(nameof(source));
            }

            return ChainLinq.Utils.Consume(source, new ChainLinq.Consumer.MaxDouble());
        }

        public static double? Max(this IEnumerable<double?> source)
        {
            if (source == null)
            {
                throw Error.ArgumentNull(nameof(source));
            }

            return ChainLinq.Utils.Consume(source, new ChainLinq.Consumer.MaxNullableDouble());
        }

        public static float Max(this IEnumerable<float> source)
        {
            if (source == null)
            {
                throw Error.ArgumentNull(nameof(source));
            }

            return ChainLinq.Utils.Consume(source, new ChainLinq.Consumer.MaxFloat());
        }

        public static float? Max(this IEnumerable<float?> source)
        {
            if (source == null)
            {
                throw Error.ArgumentNull(nameof(source));
            }

            return ChainLinq.Utils.Consume(source, new ChainLinq.Consumer.MaxNullableFloat());
        }

        public static decimal Max(this IEnumerable<decimal> source)
        {
            if (source == null)
            {
                throw Error.ArgumentNull(nameof(source));
            }

            return ChainLinq.Utils.Consume(source, new ChainLinq.Consumer.MaxDecimal());
        }

        public static decimal? Max(this IEnumerable<decimal?> source)
        {
            if (source == null)
            {
                throw Error.ArgumentNull(nameof(source));
            }

            return ChainLinq.Utils.Consume(source, new ChainLinq.Consumer.MaxNullableDecimal());
        }

        public static TSource Max<TSource>(this IEnumerable<TSource> source)
        {
            if (source == null)
            {
                throw Error.ArgumentNull(nameof(source));
            }

            if (default(TSource) == null)
            {
                return ChainLinq.Utils.Consume(source, new ChainLinq.Consumer.MaxRefType<TSource>());
            }
            else
            {
                return ChainLinq.Utils.Consume(source, new ChainLinq.Consumer.MaxValueType<TSource>());
            }
        }

        public static int Max<TSource>(this IEnumerable<TSource> source, Func<TSource, int> selector)
        {
            if (source == null)
            {
                throw Error.ArgumentNull(nameof(source));
            }

            if (selector == null)
            {
                throw Error.ArgumentNull(nameof(selector));
            }

            return ChainLinq.Utils.Consume(source, new ChainLinq.Consumer.MaxInt<TSource>(selector));
        }

        public static int? Max<TSource>(this IEnumerable<TSource> source, Func<TSource, int?> selector)
        {
            if (source == null)
            {
                throw Error.ArgumentNull(nameof(source));
            }

            if (selector == null)
            {
                throw Error.ArgumentNull(nameof(selector));
            }

            return ChainLinq.Utils.Consume(source, new ChainLinq.Consumer.MaxNullableInt<TSource>(selector));
        }

        public static long Max<TSource>(this IEnumerable<TSource> source, Func<TSource, long> selector)
        {
            if (source == null)
            {
                throw Error.ArgumentNull(nameof(source));
            }

            if (selector == null)
            {
                throw Error.ArgumentNull(nameof(selector));
            }

            return ChainLinq.Utils.Consume(source, new ChainLinq.Consumer.MaxLong<TSource>(selector));
        }

        public static long? Max<TSource>(this IEnumerable<TSource> source, Func<TSource, long?> selector)
        {
            if (source == null)
            {
                throw Error.ArgumentNull(nameof(source));
            }

            if (selector == null)
            {
                throw Error.ArgumentNull(nameof(selector));
            }

            return ChainLinq.Utils.Consume(source, new ChainLinq.Consumer.MaxNullableLong<TSource>(selector));
        }

        public static float Max<TSource>(this IEnumerable<TSource> source, Func<TSource, float> selector)
        {
            if (source == null)
            {
                throw Error.ArgumentNull(nameof(source));
            }

            if (selector == null)
            {
                throw Error.ArgumentNull(nameof(selector));
            }

            return ChainLinq.Utils.Consume(source, new ChainLinq.Consumer.MaxFloat<TSource>(selector));
        }

        public static float? Max<TSource>(this IEnumerable<TSource> source, Func<TSource, float?> selector)
        {
            if (source == null)
            {
                throw Error.ArgumentNull(nameof(source));
            }

            if (selector == null)
            {
                throw Error.ArgumentNull(nameof(selector));
            }

            return ChainLinq.Utils.Consume(source, new ChainLinq.Consumer.MaxNullableFloat<TSource>(selector));
        }

        public static double Max<TSource>(this IEnumerable<TSource> source, Func<TSource, double> selector)
        {
            if (source == null)
            {
                throw Error.ArgumentNull(nameof(source));
            }

            if (selector == null)
            {
                throw Error.ArgumentNull(nameof(selector));
            }

            return ChainLinq.Utils.Consume(source, new ChainLinq.Consumer.MaxDouble<TSource>(selector));
        }

        public static double? Max<TSource>(this IEnumerable<TSource> source, Func<TSource, double?> selector)
        {
            if (source == null)
            {
                throw Error.ArgumentNull(nameof(source));
            }

            if (selector == null)
            {
                throw Error.ArgumentNull(nameof(selector));
            }

            return ChainLinq.Utils.Consume(source, new ChainLinq.Consumer.MaxNullableDouble<TSource>(selector));
        }

        public static decimal Max<TSource>(this IEnumerable<TSource> source, Func<TSource, decimal> selector)
        {
            if (source == null)
            {
                throw Error.ArgumentNull(nameof(source));
            }

            if (selector == null)
            {
                throw Error.ArgumentNull(nameof(selector));
            }

            return ChainLinq.Utils.Consume(source, new ChainLinq.Consumer.MaxDecimal<TSource>(selector));
        }

        public static decimal? Max<TSource>(this IEnumerable<TSource> source, Func<TSource, decimal?> selector)
        {
            if (source == null)
            {
                throw Error.ArgumentNull(nameof(source));
            }

            if (selector == null)
            {
                throw Error.ArgumentNull(nameof(selector));
            }

            return ChainLinq.Utils.Consume(source, new ChainLinq.Consumer.MaxNullableDecimal<TSource>(selector));
        }

        public static TResult Max<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult> selector)
        {
            if (source == null)
            {
                throw Error.ArgumentNull(nameof(source));
            }

            if (selector == null)
            {
                throw Error.ArgumentNull(nameof(selector));
            }

            if (default(TResult) == null)
            {
                return ChainLinq.Utils.Consume(source, new ChainLinq.Consumer.MaxRefType<TSource, TResult>(selector));
            }
            else
            {
                return ChainLinq.Utils.Consume(source, new ChainLinq.Consumer.MaxValueType<TSource, TResult>(selector));
            }
        }
    }
}
