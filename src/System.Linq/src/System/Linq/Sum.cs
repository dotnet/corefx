// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;

namespace System.Linq
{
    public static partial class Enumerable
    {
        public static int Sum(this IEnumerable<int> source)
        {
            if (source == null)
            {
                throw Error.ArgumentNull(nameof(source));
            }

#if !PRE_CHAINLINQ
            return ChainLinq.Utils.Consume(source, new ChainLinq.Consumer.SumInt());
#else
            int sum = 0;
            checked
            {
                foreach (int v in source)
                {
                    sum += v;
                }
            }

            return sum;
#endif
        }

        public static int? Sum(this IEnumerable<int?> source)
        {
            if (source == null)
            {
                throw Error.ArgumentNull(nameof(source));
            }

#if !PRE_CHAINLINQ
            return ChainLinq.Utils.Consume(source, new ChainLinq.Consumer.SumNullableInt());
#else
            int sum = 0;
            checked
            {
                foreach (int? v in source)
                {
                    if (v != null)
                    {
                        sum += v.GetValueOrDefault();
                    }
                }
            }

            return sum;
#endif
        }

        public static long Sum(this IEnumerable<long> source)
        {
            if (source == null)
            {
                throw Error.ArgumentNull(nameof(source));
            }

#if !PRE_CHAINLINQ
            return ChainLinq.Utils.Consume(source, new ChainLinq.Consumer.SumLong());
#else
            long sum = 0;
            checked
            {
                foreach (long v in source)
                {
                    sum += v;
                }
            }

            return sum;
#endif
        }

        public static long? Sum(this IEnumerable<long?> source)
        {
            if (source == null)
            {
                throw Error.ArgumentNull(nameof(source));
            }

#if !PRE_CHAINLINQ
            return ChainLinq.Utils.Consume(source, new ChainLinq.Consumer.SumNullableLong());
#else
            long sum = 0;
            checked
            {
                foreach (long? v in source)
                {
                    if (v != null)
                    {
                        sum += v.GetValueOrDefault();
                    }
                }
            }

            return sum;
#endif
        }

        public static float Sum(this IEnumerable<float> source)
        {
            if (source == null)
            {
                throw Error.ArgumentNull(nameof(source));
            }

#if !PRE_CHAINLINQ
            return ChainLinq.Utils.Consume(source, new ChainLinq.Consumer.SumFloat());
#else
            double sum = 0;
            foreach (float v in source)
            {
                sum += v;
            }

            return (float)sum;
#endif
        }

        public static float? Sum(this IEnumerable<float?> source)
        {
            if (source == null)
            {
                throw Error.ArgumentNull(nameof(source));
            }

#if !PRE_CHAINLINQ
            return ChainLinq.Utils.Consume(source, new ChainLinq.Consumer.SumNullableFloat());
#else

            double sum = 0;
            foreach (float? v in source)
            {
                if (v != null)
                {
                    sum += v.GetValueOrDefault();
                }
            }

            return (float)sum;
#endif
        }

        public static double Sum(this IEnumerable<double> source)
        {
            if (source == null)
            {
                throw Error.ArgumentNull(nameof(source));
            }

#if !PRE_CHAINLINQ
            return ChainLinq.Utils.Consume(source, new ChainLinq.Consumer.SumDouble());
#else

            double sum = 0;
            foreach (double v in source)
            {
                sum += v;
            }

            return sum;
#endif
        }

        public static double? Sum(this IEnumerable<double?> source)
        {
            if (source == null)
            {
                throw Error.ArgumentNull(nameof(source));
            }

#if !PRE_CHAINLINQ
            return ChainLinq.Utils.Consume(source, new ChainLinq.Consumer.SumNullableDouble());
#else
            double sum = 0;
            foreach (double? v in source)
            {
                if (v != null)
                {
                    sum += v.GetValueOrDefault();
                }
            }

            return sum;
#endif
        }

        public static decimal Sum(this IEnumerable<decimal> source)
        {
            if (source == null)
            {
                throw Error.ArgumentNull(nameof(source));
            }

#if !PRE_CHAINLINQ
            return ChainLinq.Utils.Consume(source, new ChainLinq.Consumer.SumDecimal());
#else
            decimal sum = 0;
            foreach (decimal v in source)
            {
                sum += v;
            }

            return sum;
#endif
        }

        public static decimal? Sum(this IEnumerable<decimal?> source)
        {
            if (source == null)
            {
                throw Error.ArgumentNull(nameof(source));
            }

#if !PRE_CHAINLINQ
            return ChainLinq.Utils.Consume(source, new ChainLinq.Consumer.SumNullableDecimal());
#else
            decimal sum = 0;
            foreach (decimal? v in source)
            {
                if (v != null)
                {
                    sum += v.GetValueOrDefault();
                }
            }

            return sum;
#endif
        }

        public static int Sum<TSource>(this IEnumerable<TSource> source, Func<TSource, int> selector)
        {
            if (source == null)
            {
                throw Error.ArgumentNull(nameof(source));
            }

            if (selector == null)
            {
                throw Error.ArgumentNull(nameof(selector));
            }

#if !PRE_CHAINLINQ
            return ChainLinq.Utils.Consume(source, new ChainLinq.Consumer.SumInt<TSource>(selector));
#else
            int sum = 0;
            checked
            {
                foreach (TSource item in source)
                {
                    sum += selector(item);
                }
            }

            return sum;
#endif
        }

        public static int? Sum<TSource>(this IEnumerable<TSource> source, Func<TSource, int?> selector)
        {
            if (source == null)
            {
                throw Error.ArgumentNull(nameof(source));
            }

            if (selector == null)
            {
                throw Error.ArgumentNull(nameof(selector));
            }

#if !PRE_CHAINLINQ
            return ChainLinq.Utils.Consume(source, new ChainLinq.Consumer.SumNullableInt<TSource> (selector));
#else
            int sum = 0;
            checked
            {
                foreach (TSource item in source)
                {
                    int? v = selector(item);
                    if (v != null)
                    {
                        sum += v.GetValueOrDefault();
                    }
                }
            }

            return sum;
#endif
        }

        public static long Sum<TSource>(this IEnumerable<TSource> source, Func<TSource, long> selector)
        {
            if (selector == null)
            {
                throw Error.ArgumentNull(nameof(selector));
            }

            if (source == null)
            {
                throw Error.ArgumentNull(nameof(source));
            }

#if !PRE_CHAINLINQ
            return ChainLinq.Utils.Consume(source, new ChainLinq.Consumer.SumLong<TSource>(selector));
#else
            long sum = 0;
            checked
            {
                foreach (TSource item in source)
                {
                    sum += selector(item);
                }
            }

            return sum;
#endif
        }

        public static long? Sum<TSource>(this IEnumerable<TSource> source, Func<TSource, long?> selector)
        {
            if (source == null)
            {
                throw Error.ArgumentNull(nameof(source));
            }

            if (selector == null)
            {
                throw Error.ArgumentNull(nameof(selector));
            }

#if !PRE_CHAINLINQ
            return ChainLinq.Utils.Consume(source, new ChainLinq.Consumer.SumNullableLong<TSource>(selector));
#else
            long sum = 0;
            checked
            {
                foreach (TSource item in source)
                {
                    long? v = selector(item);
                    if (v != null)
                    {
                        sum += v.GetValueOrDefault();
                    }
                }
            }

            return sum;
#endif
        }

        public static float Sum<TSource>(this IEnumerable<TSource> source, Func<TSource, float> selector)
        {
            if (source == null)
            {
                throw Error.ArgumentNull(nameof(source));
            }

            if (selector == null)
            {
                throw Error.ArgumentNull(nameof(selector));
            }

#if !PRE_CHAINLINQ
            return ChainLinq.Utils.Consume(source, new ChainLinq.Consumer.SumFloat<TSource>(selector));
#else
            double sum = 0;
            foreach (TSource item in source)
            {
                sum += selector(item);
            }

            return (float)sum;
#endif
        }

        public static float? Sum<TSource>(this IEnumerable<TSource> source, Func<TSource, float?> selector)
        {
            if (source == null)
            {
                throw Error.ArgumentNull(nameof(source));
            }

            if (selector == null)
            {
                throw Error.ArgumentNull(nameof(selector));
            }

#if !PRE_CHAINLINQ
            return ChainLinq.Utils.Consume(source, new ChainLinq.Consumer.SumNullableFloat<TSource>(selector));
#else
            double sum = 0;
            foreach (TSource item in source)
            {
                float? v = selector(item);
                if (v != null)
                {
                    sum += v.GetValueOrDefault();
                }
            }

            return (float)sum;
#endif
        }

        public static double Sum<TSource>(this IEnumerable<TSource> source, Func<TSource, double> selector)
        {
            if (source == null)
            {
                throw Error.ArgumentNull(nameof(source));
            }

            if (selector == null)
            {
                throw Error.ArgumentNull(nameof(selector));
            }

#if !PRE_CHAINLINQ
            return ChainLinq.Utils.Consume(source, new ChainLinq.Consumer.SumDouble<TSource>(selector));
#else
            double sum = 0;
            foreach (TSource item in source)
            {
                sum += selector(item);
            }

            return sum;
#endif
        }

        public static double? Sum<TSource>(this IEnumerable<TSource> source, Func<TSource, double?> selector)
        {
            if (source == null)
            {
                throw Error.ArgumentNull(nameof(source));
            }

            if (selector == null)
            {
                throw Error.ArgumentNull(nameof(selector));
            }

#if !PRE_CHAINLINQ
            return ChainLinq.Utils.Consume(source, new ChainLinq.Consumer.SumNullableDouble<TSource>(selector));
#else
            double sum = 0;
            foreach (TSource item in source)
            {
                double? v = selector(item);
                if (v != null)
                {
                    sum += v.GetValueOrDefault();
                }
            }

            return sum;
#endif
        }

        public static decimal Sum<TSource>(this IEnumerable<TSource> source, Func<TSource, decimal> selector)
        {
            if (source == null)
            {
                throw Error.ArgumentNull(nameof(source));
            }

            if (selector == null)
            {
                throw Error.ArgumentNull(nameof(selector));
            }

#if !PRE_CHAINLINQ
            return ChainLinq.Utils.Consume(source, new ChainLinq.Consumer.SumDecimal<TSource>(selector));
#else
            decimal sum = 0;
            foreach (TSource item in source)
            {
                sum += selector(item);
            }

            return sum;
#endif
        }

        public static decimal? Sum<TSource>(this IEnumerable<TSource> source, Func<TSource, decimal?> selector)
        {
            if (source == null)
            {
                throw Error.ArgumentNull(nameof(source));
            }

            if (selector == null)
            {
                throw Error.ArgumentNull(nameof(selector));
            }

#if !PRE_CHAINLINQ
            return ChainLinq.Utils.Consume(source, new ChainLinq.Consumer.SumNullableDecimal<TSource>(selector));
#else
            decimal sum = 0;
            foreach (TSource item in source)
            {
                decimal? v = selector(item);
                if (v != null)
                {
                    sum += v.GetValueOrDefault();
                }
            }

            return sum;
#endif
        }
    }
}
