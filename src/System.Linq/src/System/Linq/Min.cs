// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;

namespace System.Linq
{
    public static partial class Enumerable
    {
        public static int Min(this IEnumerable<int> source)
        {
            if (source == null)
            {
                throw Error.ArgumentNull(nameof(source));
            }

            return ChainLinq.Utils.Consume(source, new ChainLinq.Consumer.MinInt());
        }

        public static int? Min(this IEnumerable<int?> source)
        {
            if (source == null)
            {
                throw Error.ArgumentNull(nameof(source));
            }

            return ChainLinq.Utils.Consume(source, new ChainLinq.Consumer.MinNullableInt());
        }

        public static long Min(this IEnumerable<long> source)
        {
            if (source == null)
            {
                throw Error.ArgumentNull(nameof(source));
            }

            return ChainLinq.Utils.Consume(source, new ChainLinq.Consumer.MinLong());
        }

        public static long? Min(this IEnumerable<long?> source)
        {
            if (source == null)
            {
                throw Error.ArgumentNull(nameof(source));
            }

            return ChainLinq.Utils.Consume(source, new ChainLinq.Consumer.MinNullableLong());
        }

        public static float Min(this IEnumerable<float> source)
        {
            if (source == null)
            {
                throw Error.ArgumentNull(nameof(source));
            }

            return ChainLinq.Utils.Consume(source, new ChainLinq.Consumer.MinFloat());
        }

        public static float? Min(this IEnumerable<float?> source)
        {
            if (source == null)
            {
                throw Error.ArgumentNull(nameof(source));
            }

            return ChainLinq.Utils.Consume(source, new ChainLinq.Consumer.MinNullableFloat());
        }

        public static double Min(this IEnumerable<double> source)
        {
            if (source == null)
            {
                throw Error.ArgumentNull(nameof(source));
            }

            return ChainLinq.Utils.Consume(source, new ChainLinq.Consumer.MinDouble());
        }

        public static double? Min(this IEnumerable<double?> source)
        {
            if (source == null)
            {
                throw Error.ArgumentNull(nameof(source));
            }

            return ChainLinq.Utils.Consume(source, new ChainLinq.Consumer.MinNullableDouble());
        }

        public static decimal Min(this IEnumerable<decimal> source)
        {
            if (source == null)
            {
                throw Error.ArgumentNull(nameof(source));
            }

            return ChainLinq.Utils.Consume(source, new ChainLinq.Consumer.MinDecimal());
        }

        public static decimal? Min(this IEnumerable<decimal?> source)
        {
            if (source == null)
            {
                throw Error.ArgumentNull(nameof(source));
            }

            return ChainLinq.Utils.Consume(source, new ChainLinq.Consumer.MinNullableDecimal());
        }

        public static TSource Min<TSource>(this IEnumerable<TSource> source)
        {
            if (source == null)
            {
                throw Error.ArgumentNull(nameof(source));
            }

            if (default(TSource) == null)
            {
                return ChainLinq.Utils.Consume(source, new ChainLinq.Consumer.MinRefType<TSource>());
            }
            else
            {
                return ChainLinq.Utils.Consume(source, new ChainLinq.Consumer.MinValueType<TSource>());
            }
        }

        public static int Min<TSource>(this IEnumerable<TSource> source, Func<TSource, int> selector)
        {
            if (source == null)
            {
                throw Error.ArgumentNull(nameof(source));
            }

            if (selector == null)
            {
                throw Error.ArgumentNull(nameof(selector));
            }

            return ChainLinq.Utils.Consume(source, new ChainLinq.Consumer.MinInt<TSource>(selector));
        }

        public static int? Min<TSource>(this IEnumerable<TSource> source, Func<TSource, int?> selector)
        {
            if (source == null)
            {
                throw Error.ArgumentNull(nameof(source));
            }

            if (selector == null)
            {
                throw Error.ArgumentNull(nameof(selector));
            }

            return ChainLinq.Utils.Consume(source, new ChainLinq.Consumer.MinNullableInt<TSource>(selector));
        }

        public static long Min<TSource>(this IEnumerable<TSource> source, Func<TSource, long> selector)
        {
            if (source == null)
            {
                throw Error.ArgumentNull(nameof(source));
            }

            if (selector == null)
            {
                throw Error.ArgumentNull(nameof(selector));
            }

            return ChainLinq.Utils.Consume(source, new ChainLinq.Consumer.MinLong<TSource>(selector));
        }

        public static long? Min<TSource>(this IEnumerable<TSource> source, Func<TSource, long?> selector)
        {
            if (source == null)
            {
                throw Error.ArgumentNull(nameof(source));
            }

            if (selector == null)
            {
                throw Error.ArgumentNull(nameof(selector));
            }

            return ChainLinq.Utils.Consume(source, new ChainLinq.Consumer.MinNullableLong<TSource>(selector));
        }

        public static float Min<TSource>(this IEnumerable<TSource> source, Func<TSource, float> selector)
        {
            if (source == null)
            {
                throw Error.ArgumentNull(nameof(source));
            }

            if (selector == null)
            {
                throw Error.ArgumentNull(nameof(selector));
            }

            return ChainLinq.Utils.Consume(source, new ChainLinq.Consumer.MinFloat<TSource>(selector));
        }

        public static float? Min<TSource>(this IEnumerable<TSource> source, Func<TSource, float?> selector)
        {
            if (source == null)
            {
                throw Error.ArgumentNull(nameof(source));
            }

            if (selector == null)
            {
                throw Error.ArgumentNull(nameof(selector));
            }

            return ChainLinq.Utils.Consume(source, new ChainLinq.Consumer.MinNullableFloat<TSource>(selector));
        }

        public static double Min<TSource>(this IEnumerable<TSource> source, Func<TSource, double> selector)
        {
            if (source == null)
            {
                throw Error.ArgumentNull(nameof(source));
            }

            if (selector == null)
            {
                throw Error.ArgumentNull(nameof(selector));
            }

            return ChainLinq.Utils.Consume(source, new ChainLinq.Consumer.MinDouble<TSource>(selector));
        }

        public static double? Min<TSource>(this IEnumerable<TSource> source, Func<TSource, double?> selector)
        {
            if (source == null)
            {
                throw Error.ArgumentNull(nameof(source));
            }

            if (selector == null)
            {
                throw Error.ArgumentNull(nameof(selector));
            }

            return ChainLinq.Utils.Consume(source, new ChainLinq.Consumer.MinNullableDouble<TSource>(selector));
        }

        public static decimal Min<TSource>(this IEnumerable<TSource> source, Func<TSource, decimal> selector)
        {
            if (source == null)
            {
                throw Error.ArgumentNull(nameof(source));
            }

            if (selector == null)
            {
                throw Error.ArgumentNull(nameof(selector));
            }

            return ChainLinq.Utils.Consume(source, new ChainLinq.Consumer.MinDecimal<TSource>(selector));
        }

        public static decimal? Min<TSource>(this IEnumerable<TSource> source, Func<TSource, decimal?> selector)
        {
            if (source == null)
            {
                throw Error.ArgumentNull(nameof(source));
            }

            if (selector == null)
            {
                throw Error.ArgumentNull(nameof(selector));
            }

            return ChainLinq.Utils.Consume(source, new ChainLinq.Consumer.MinNullableDecimal<TSource>(selector));
        }

        public static TResult Min<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult> selector)
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
                return ChainLinq.Utils.Consume(source, new ChainLinq.Consumer.MinRefType<TSource, TResult>(selector));
            }
            else
            {
                return ChainLinq.Utils.Consume(source, new ChainLinq.Consumer.MinValueType<TSource, TResult>(selector));
            }
        }
    }
}
