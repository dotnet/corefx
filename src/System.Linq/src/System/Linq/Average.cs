// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;

namespace System.Linq
{
    public static partial class Enumerable
    {
        public static double Average(this IEnumerable<int> source)
        {
            if (source == null)
            {
                throw Error.ArgumentNull(nameof(source));
            }

            return ChainLinq.Utils.Consume(source, new ChainLinq.Consumer.AverageInt());
        }

        public static double? Average(this IEnumerable<int?> source)
        {
            if (source == null)
            {
                throw Error.ArgumentNull(nameof(source));
            }

            return ChainLinq.Utils.Consume(source, new ChainLinq.Consumer.AverageNullableInt());
        }

        public static double Average(this IEnumerable<long> source)
        {
            if (source == null)
            {
                throw Error.ArgumentNull(nameof(source));
            }

            return ChainLinq.Utils.Consume(source, new ChainLinq.Consumer.AverageLong());
        }

        public static double? Average(this IEnumerable<long?> source)
        {
            if (source == null)
            {
                throw Error.ArgumentNull(nameof(source));
            }

            return ChainLinq.Utils.Consume(source, new ChainLinq.Consumer.AverageNullableLong());
        }

        public static float Average(this IEnumerable<float> source)
        {
            if (source == null)
            {
                throw Error.ArgumentNull(nameof(source));
            }

            return ChainLinq.Utils.Consume(source, new ChainLinq.Consumer.AverageFloat());
        }

        public static float? Average(this IEnumerable<float?> source)
        {
            if (source == null)
            {
                throw Error.ArgumentNull(nameof(source));
            }

            return ChainLinq.Utils.Consume(source, new ChainLinq.Consumer.AverageNullableFloat());
        }

        public static double Average(this IEnumerable<double> source)
        {
            if (source == null)
            {
                throw Error.ArgumentNull(nameof(source));
            }

            return ChainLinq.Utils.Consume(source, new ChainLinq.Consumer.AverageDouble());
        }

        public static double? Average(this IEnumerable<double?> source)
        {
            if (source == null)
            {
                throw Error.ArgumentNull(nameof(source));
            }

            return ChainLinq.Utils.Consume(source, new ChainLinq.Consumer.AverageNullableDouble());
        }

        public static decimal Average(this IEnumerable<decimal> source)
        {
            if (source == null)
            {
                throw Error.ArgumentNull(nameof(source));
            }

            return ChainLinq.Utils.Consume(source, new ChainLinq.Consumer.AverageDecimal());
        }

        public static decimal? Average(this IEnumerable<decimal?> source)
        {
            if (source == null)
            {
                throw Error.ArgumentNull(nameof(source));
            }

            return ChainLinq.Utils.Consume(source, new ChainLinq.Consumer.AverageNullableDecimal());
        }

        public static double Average<TSource>(this IEnumerable<TSource> source, Func<TSource, int> selector)
        {
            if (source == null)
            {
                throw Error.ArgumentNull(nameof(source));
            }

            if (selector == null)
            {
                throw Error.ArgumentNull(nameof(selector));
            }

            return ChainLinq.Utils.Consume(source, new ChainLinq.Consumer.AverageInt<TSource>(selector));
        }

        public static double? Average<TSource>(this IEnumerable<TSource> source, Func<TSource, int?> selector)
        {
            if (source == null)
            {
                throw Error.ArgumentNull(nameof(source));
            }

            if (selector == null)
            {
                throw Error.ArgumentNull(nameof(selector));
            }

            return ChainLinq.Utils.Consume(source, new ChainLinq.Consumer.AverageNullableInt<TSource>(selector));
        }

        public static double Average<TSource>(this IEnumerable<TSource> source, Func<TSource, long> selector)
        {
            if (source == null)
            {
                throw Error.ArgumentNull(nameof(source));
            }

            if (selector == null)
            {
                throw Error.ArgumentNull(nameof(selector));
            }

            return ChainLinq.Utils.Consume(source, new ChainLinq.Consumer.AverageLong<TSource>(selector));
        }

        public static double? Average<TSource>(this IEnumerable<TSource> source, Func<TSource, long?> selector)
        {
            if (source == null)
            {
                throw Error.ArgumentNull(nameof(source));
            }

            if (selector == null)
            {
                throw Error.ArgumentNull(nameof(selector));
            }

            return ChainLinq.Utils.Consume(source, new ChainLinq.Consumer.AverageNullableLong<TSource>(selector));
        }

        public static float Average<TSource>(this IEnumerable<TSource> source, Func<TSource, float> selector)
        {
            if (source == null)
            {
                throw Error.ArgumentNull(nameof(source));
            }

            if (selector == null)
            {
                throw Error.ArgumentNull(nameof(selector));
            }

            return ChainLinq.Utils.Consume(source, new ChainLinq.Consumer.AverageFloat<TSource>(selector));
        }

        public static float? Average<TSource>(this IEnumerable<TSource> source, Func<TSource, float?> selector)
        {
            if (source == null)
            {
                throw Error.ArgumentNull(nameof(source));
            }

            if (selector == null)
            {
                throw Error.ArgumentNull(nameof(selector));
            }

            return ChainLinq.Utils.Consume(source, new ChainLinq.Consumer.AverageNullableFloat<TSource>(selector));
        }

        public static double Average<TSource>(this IEnumerable<TSource> source, Func<TSource, double> selector)
        {
            if (source == null)
            {
                throw Error.ArgumentNull(nameof(source));
            }

            if (selector == null)
            {
                throw Error.ArgumentNull(nameof(selector));
            }

            return ChainLinq.Utils.Consume(source, new ChainLinq.Consumer.AverageDouble<TSource>(selector));
        }

        public static double? Average<TSource>(this IEnumerable<TSource> source, Func<TSource, double?> selector)
        {
            if (source == null)
            {
                throw Error.ArgumentNull(nameof(source));
            }

            if (selector == null)
            {
                throw Error.ArgumentNull(nameof(selector));
            }

            return ChainLinq.Utils.Consume(source, new ChainLinq.Consumer.AverageNullableDouble<TSource>(selector));
        }

        public static decimal Average<TSource>(this IEnumerable<TSource> source, Func<TSource, decimal> selector)
        {
            if (source == null)
            {
                throw Error.ArgumentNull(nameof(source));
            }

            if (selector == null)
            {
                throw Error.ArgumentNull(nameof(selector));
            }

            return ChainLinq.Utils.Consume(source, new ChainLinq.Consumer.AverageDecimal<TSource>(selector));
        }

        public static decimal? Average<TSource>(this IEnumerable<TSource> source, Func<TSource, decimal?> selector)
        {
            if (source == null)
            {
                throw Error.ArgumentNull(nameof(source));
            }

            if (selector == null)
            {
                throw Error.ArgumentNull(nameof(selector));
            }

            return ChainLinq.Utils.Consume(source, new ChainLinq.Consumer.AverageNullableDecimal<TSource>(selector));
        }
    }
}
