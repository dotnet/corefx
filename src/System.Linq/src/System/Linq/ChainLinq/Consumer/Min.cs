using System.Collections.Generic;

namespace System.Linq.ChainLinq.Consumer
{
    class MinInt : Consumer<int, int>
    {
        bool _first;

        public MinInt() : base(int.MaxValue) =>
            _first = true;

        public override ChainStatus ProcessNext(int input)
        {
            _first = false;
            if (input < Result)
            {
                Result = input;
            }
            return ChainStatus.Flow;
        }

        public override void ChainComplete()
        {
            if (_first)
            {
                throw Error.NoElements();
            }
        }
    }

    class MinNullableInt : Consumer<int?, int?>
    {
        public MinNullableInt() : base(null) { }

        public override ChainStatus ProcessNext(int? input)
        {
            var maybeValue = input.GetValueOrDefault();
            if (!Result.HasValue || (input.HasValue && maybeValue < Result))
            {
                Result = input;
            }
            return ChainStatus.Flow;
        }
    }

    class MinLong : Consumer<long, long>
    {
        bool _first;

        public MinLong() : base(long.MaxValue) =>
            _first = true;

        public override ChainStatus ProcessNext(long input)
        {
            _first = false;
            if (input < Result)
            {
                Result = input;
            }
            return ChainStatus.Flow;
        }

        public override void ChainComplete()
        {
            if (_first)
            {
                throw Error.NoElements();
            }
        }
    }

    class MinNullableLong : Consumer<long?, long?>
    {
        public MinNullableLong() : base(null) { }

        public override ChainStatus ProcessNext(long? input)
        {
            var maybeValue = input.GetValueOrDefault();
            if (!Result.HasValue || (input.HasValue && maybeValue < Result))
            {
                Result = input;
            }
            return ChainStatus.Flow;
        }
    }

    class MinFloat : Consumer<float, float>
    {
        bool _first;

        public MinFloat() : base(float.PositiveInfinity) =>
            _first = true;

        public override ChainStatus ProcessNext(float input)
        {
            _first = false;
            if (input < Result)
            {
                Result = input;
            }
            else if (float.IsNaN(input))
            {
                Result = float.NaN;
                return ChainStatus.Stop;
            }
            return ChainStatus.Flow;
        }

        public override void ChainComplete()
        {
            if (_first)
            {
                throw Error.NoElements();
            }
        }
    }

    class MinNullableFloat : Consumer<float?, float?>
    {
        public MinNullableFloat() : base(null) { }

        public override ChainStatus ProcessNext(float? input)
        {
            if (!Result.HasValue)
            {
                if (!input.HasValue)
                {
                    return ChainStatus.Flow;
                }

                Result = float.PositiveInfinity;
            }

            if (input.HasValue)
            {
                var value = input.GetValueOrDefault();
                if (value < Result.GetValueOrDefault())
                {
                    Result = value;
                }
                else if (float.IsNaN(value))
                {
                    Result = float.NaN;
                    return ChainStatus.Stop;
                }
            }

            return ChainStatus.Flow;
        }
    }

    class MinDouble : Consumer<double, double>
    {
        bool _first;

        public MinDouble() : base(double.PositiveInfinity) =>
            _first = true;

        public override ChainStatus ProcessNext(double input)
        {
            _first = false;
            if (input < Result)
            {
                Result = input;
            }
            else if (double.IsNaN(input))
            {
                Result = double.NaN;
                return ChainStatus.Stop;
            }
            return ChainStatus.Flow;
        }

        public override void ChainComplete()
        {
            if (_first)
            {
                throw Error.NoElements();
            }
        }
    }

    class MinNullableDouble : Consumer<double?, double?>
    {
        public MinNullableDouble() : base(null) { }

        public override ChainStatus ProcessNext(double? input)
        {
            if (!Result.HasValue)
            {
                if (!input.HasValue)
                {
                    return ChainStatus.Flow;
                }

                Result = double.PositiveInfinity;
            }

            if (input.HasValue)
            {
                var value = input.GetValueOrDefault();
                if (value < Result.GetValueOrDefault())
                {
                    Result = value;
                }
                else if (double.IsNaN(value))
                {
                    Result = double.NaN;
                    return ChainStatus.Stop;
                }
            }

            return ChainStatus.Flow;
        }
    }

    class MinDecimal : Consumer<decimal, decimal>
    {
        bool _first;

        public MinDecimal() : base(decimal.MaxValue) =>
            _first = true;

        public override ChainStatus ProcessNext(decimal input)
        {
            _first = false;
            if (input < Result)
            {
                Result = input;
            }
            return ChainStatus.Flow;
        }

        public override void ChainComplete()
        {
            if (_first)
            {
                throw Error.NoElements();
            }
        }
    }

    class MinNullableDecimal : Consumer<decimal?, decimal?>
    {
        public MinNullableDecimal() : base(null) { }

        public override ChainStatus ProcessNext(decimal? input)
        {
            if (!Result.HasValue)
            {
                Result = input;
            }
            else if (input.HasValue)
            {
                var value = input.GetValueOrDefault();
                if (value < Result.GetValueOrDefault())
                {
                    Result = value;
                }
            }

            return ChainStatus.Flow;
        }
    }

    class MinValueType<T> : Consumer<T, T>
    {
        bool _first;

        public MinValueType() : base(default) =>
            _first = true;

        public override ChainStatus ProcessNext(T input)
        {
            if (_first)
            {
                _first = false;
                Result = input;
            }
            else if (Comparer<T>.Default.Compare(input, Result) < 0)
            {
                Result = input;
            }

            return ChainStatus.Flow;
        }

        public override void ChainComplete()
        {
            if (_first)
            {
                throw Error.NoElements();
            }
        }
    }

    class MinRefType<T> : Consumer<T, T>
    {
        public MinRefType() : base(default) { }

        public override ChainStatus ProcessNext(T input)
        {
            if (Result == null || (input != null && Comparer<T>.Default.Compare(input, Result) < 0))
            {
                Result = input;
            }

            return ChainStatus.Flow;
        }
    }

    class MinInt<TSource> : Consumer<TSource, int>
    {
        private readonly Func<TSource, int> _selector;

        bool _first;

        public MinInt(Func<TSource, int> selector) : base(int.MaxValue) =>
            (_selector, _first) = (selector, true);

        public override ChainStatus ProcessNext(TSource source)
        {
            var input = _selector(source);

            _first = false;
            if (input < Result)
            {
                Result = input;
            }
            return ChainStatus.Flow;
        }

        public override void ChainComplete()
        {
            if (_first)
            {
                throw Error.NoElements();
            }
        }
    }

    class MinNullableInt<TSource> : Consumer<TSource, int?>
    {
        private readonly Func<TSource, int?> _selector;

        public MinNullableInt(Func<TSource, int?> selector) : base(null) =>
            _selector = selector;

        public override ChainStatus ProcessNext(TSource source)
        {
            var input = _selector(source);

            var maybeValue = input.GetValueOrDefault();
            if (!Result.HasValue || (input.HasValue && maybeValue < Result))
            {
                Result = input;
            }
            return ChainStatus.Flow;
        }
    }

    class MinLong<TSource> : Consumer<TSource, long>
    {
        private readonly Func<TSource, long> _selector;

        bool _first;

        public MinLong(Func<TSource, long> selector) : base(long.MaxValue) =>
            (_selector, _first) = (selector, true);

        public override ChainStatus ProcessNext(TSource source)
        {
            var input = _selector(source);

            _first = false;
            if (input < Result)
            {
                Result = input;
            }
            return ChainStatus.Flow;
        }

        public override void ChainComplete()
        {
            if (_first)
            {
                throw Error.NoElements();
            }
        }
    }

    class MinNullableLong<TSource> : Consumer<TSource, long?>
    {
        private readonly Func<TSource, long?> _selector;

        public MinNullableLong(Func<TSource, long?> selector) : base(null) =>
            _selector = selector;

        public override ChainStatus ProcessNext(TSource source)
        {
            var input = _selector(source);

            var maybeValue = input.GetValueOrDefault();
            if (!Result.HasValue || (input.HasValue && maybeValue < Result))
            {
                Result = input;
            }
            return ChainStatus.Flow;
        }
    }

    class MinFloat<TSource> : Consumer<TSource, float>
    {
        private readonly Func<TSource, float> _selector;

        bool _first;

        public MinFloat(Func<TSource, float> selector) : base(float.PositiveInfinity) =>
            (_selector, _first) = (selector, true);

        public override ChainStatus ProcessNext(TSource source)
        {
            var input = _selector(source);

            _first = false;
            if (input < Result)
            {
                Result = input;
            }
            else if (float.IsNaN(input))
            {
                Result = float.NaN;
                return ChainStatus.Stop;
            }
            return ChainStatus.Flow;
        }

        public override void ChainComplete()
        {
            if (_first)
            {
                throw Error.NoElements();
            }
        }
    }

    class MinNullableFloat<TSource> : Consumer<TSource, float?>
    {
        private readonly Func<TSource, float?> _selector;

        public MinNullableFloat(Func<TSource, float?> selector) : base(null) =>
            _selector = selector;

        public override ChainStatus ProcessNext(TSource source)
        {
            var input = _selector(source);

            if (!Result.HasValue)
            {
                if (!input.HasValue)
                {
                    return ChainStatus.Flow;
                }

                Result = float.PositiveInfinity;
            }

            if (input.HasValue)
            {
                var value = input.GetValueOrDefault();
                if (value < Result.GetValueOrDefault())
                {
                    Result = value;
                }
                else if (float.IsNaN(value))
                {
                    Result = float.NaN;
                    return ChainStatus.Stop;
                }
            }

            return ChainStatus.Flow;
        }
    }

    class MinDouble<TSource> : Consumer<TSource, double>
    {
        private readonly Func<TSource, double> _selector;

        bool _first;

        public MinDouble(Func<TSource, double> selector) : base(double.PositiveInfinity) =>
            (_selector, _first) = (selector, true);

        public override ChainStatus ProcessNext(TSource source)
        {
            var input = _selector(source);

            _first = false;
            if (input < Result)
            {
                Result = input;
            }
            else if (double.IsNaN(input))
            {
                Result = double.NaN;
                return ChainStatus.Stop;
            }
            return ChainStatus.Flow;
        }

        public override void ChainComplete()
        {
            if (_first)
            {
                throw Error.NoElements();
            }
        }
    }

    class MinNullableDouble<TSource> : Consumer<TSource, double?>
    {
        private readonly Func<TSource, double?> _selector;

        public MinNullableDouble(Func<TSource, double?> selector) : base(null) =>
            _selector = selector;

        public override ChainStatus ProcessNext(TSource source)
        {
            var input = _selector(source);

            if (!Result.HasValue)
            {
                if (!input.HasValue)
                {
                    return ChainStatus.Flow;
                }

                Result = double.PositiveInfinity;
            }

            if (input.HasValue)
            {
                var value = input.GetValueOrDefault();
                if (value < Result.GetValueOrDefault())
                {
                    Result = value;
                }
                else if (double.IsNaN(value))
                {
                    Result = double.NaN;
                    return ChainStatus.Stop;
                }
            }

            return ChainStatus.Flow;
        }
    }

    class MinDecimal<TSource> : Consumer<TSource, decimal>
    {
        private readonly Func<TSource, decimal> _selector;

        bool _first;

        public MinDecimal(Func<TSource, decimal> selector) : base(decimal.MaxValue) =>
            (_selector, _first) = (selector, true);

        public override ChainStatus ProcessNext(TSource source)
        {
            var input = _selector(source);

            _first = false;
            if (input < Result)
            {
                Result = input;
            }
            return ChainStatus.Flow;
        }

        public override void ChainComplete()
        {
            if (_first)
            {
                throw Error.NoElements();
            }
        }
    }

    class MinNullableDecimal<TSource> : Consumer<TSource, decimal?>
    {
        private readonly Func<TSource, decimal?> _selector;

        public MinNullableDecimal(Func<TSource, decimal?> selector) : base(null) =>
            _selector = selector;

        public override ChainStatus ProcessNext(TSource source)
        {
            var input = _selector(source);

            if (!Result.HasValue)
            {
                Result = input;
            }
            else if (input.HasValue)
            {
                var value = input.GetValueOrDefault();
                if (value < Result.GetValueOrDefault())
                {
                    Result = value;
                }
            }

            return ChainStatus.Flow;
        }
    }

    class MinValueType<TSource, T> : Consumer<TSource, T>
    {
        private readonly Func<TSource, T> _selector;

        bool _first;

        public MinValueType(Func<TSource, T> selector) : base(default) =>
            (_selector, _first) = (selector, true);

        public override ChainStatus ProcessNext(TSource source)
        {
            var input = _selector(source);

            if (_first)
            {
                _first = false;
                Result = input;
            }
            else if (Comparer<T>.Default.Compare(input, Result) < 0)
            {
                Result = input;
            }

            return ChainStatus.Flow;
        }

        public override void ChainComplete()
        {
            if (_first)
            {
                throw Error.NoElements();
            }
        }
    }

    class MinRefType<TSource, T> : Consumer<TSource, T>
    {
        private readonly Func<TSource, T> _selector;

        public MinRefType(Func<TSource, T> selector) : base(default) =>
            _selector = selector;

        public override ChainStatus ProcessNext(TSource source)
        {
            var input = _selector(source);

            if (Result == null || (input != null && Comparer<T>.Default.Compare(input, Result) < 0))
            {
                Result = input;
            }

            return ChainStatus.Flow;
        }
    }

}
