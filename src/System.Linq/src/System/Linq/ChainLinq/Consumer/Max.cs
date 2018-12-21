using System.Collections.Generic;

namespace System.Linq.ChainLinq.Consumer
{
    sealed class MaxInt : Consumer<int, int>
    {
        bool _first;

        public MaxInt() : base(int.MinValue) =>
            _first = true;

        public override ChainStatus ProcessNext(int input)
        {
            _first = false;
            if (input > Result)
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

    sealed class MaxNullableInt : Consumer<int?, int?>
    {
        public MaxNullableInt() : base(null) { }

        public override ChainStatus ProcessNext(int? input)
        {
            var maybeValue = input.GetValueOrDefault();
            if (!Result.HasValue || (input.HasValue && maybeValue > Result))
            {
                Result = input;
            }
            return ChainStatus.Flow;
        }
    }

    sealed class MaxLong : Consumer<long, long>
    {
        bool _first;

        public MaxLong() : base(long.MinValue) =>
            _first = true;

        public override ChainStatus ProcessNext(long input)
        {
            _first = false;
            if (input > Result)
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

    sealed class MaxNullableLong : Consumer<long?, long?>
    {
        public MaxNullableLong() : base(null) { }

        public override ChainStatus ProcessNext(long? input)
        {
            var maybeValue = input.GetValueOrDefault();
            if (!Result.HasValue || (input.HasValue && maybeValue > Result))
            {
                Result = input;
            }
            return ChainStatus.Flow;
        }
    }

    sealed class MaxFloat : Consumer<float, float>
    {
        bool _first;

        public MaxFloat() : base(float.NaN) =>
            _first = true;

        public override ChainStatus ProcessNext(float input)
        {
            _first = false;
            if (input > Result || float.IsNaN(Result))
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

    sealed class MaxNullableFloat : Consumer<float?, float?>
    {
        public MaxNullableFloat() : base(null) { }

        public override ChainStatus ProcessNext(float? input)
        {
            if (!Result.HasValue)
            {
                if (!input.HasValue)
                {
                    return ChainStatus.Flow;
                }

                Result = float.NaN;
            }

            if (input.HasValue)
            {
                var value = input.GetValueOrDefault();
                var result = Result.GetValueOrDefault();
                if (value > result || float.IsNaN(result))
                {
                    Result = value;
                }
            }

            return ChainStatus.Flow;
        }
    }

    sealed class MaxDouble : Consumer<double, double>
    {
        bool _first;

        public MaxDouble() : base(double.NaN) =>
            _first = true;

        public override ChainStatus ProcessNext(double input)
        {
            _first = false;
            if (input > Result || double.IsNaN(Result))
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

    sealed class MaxNullableDouble : Consumer<double?, double?>
    {
        public MaxNullableDouble() : base(null) { }

        public override ChainStatus ProcessNext(double? input)
        {
            if (!Result.HasValue)
            {
                if (!input.HasValue)
                {
                    return ChainStatus.Flow;
                }

                Result = double.NaN;
            }

            if (input.HasValue)
            {
                var value = input.GetValueOrDefault();
                var result = Result.GetValueOrDefault();
                if (value > result || double.IsNaN(result))
                {
                    Result = value;
                }
            }

            return ChainStatus.Flow;
        }
    }

    sealed class MaxDecimal : Consumer<decimal, decimal>
    {
        bool _first;

        public MaxDecimal() : base(decimal.MinValue) =>
            _first = true;

        public override ChainStatus ProcessNext(decimal input)
        {
            _first = false;
            if (input > Result)
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

    sealed class MaxNullableDecimal : Consumer<decimal?, decimal?>
    {
        public MaxNullableDecimal() : base(null) { }

        public override ChainStatus ProcessNext(decimal? input)
        {
            if (!Result.HasValue)
            {
                Result = input;
            }
            else if (input.HasValue)
            {
                var value = input.GetValueOrDefault();
                if (value > Result.GetValueOrDefault())
                {
                    Result = value;
                }
            }

            return ChainStatus.Flow;
        }
    }

    sealed class MaxValueType<T> : Consumer<T, T>
    {
        bool _first;

        public MaxValueType() : base(default) =>
            _first = true;

        public override ChainStatus ProcessNext(T input)
        {
            if (_first)
            {
                _first = false;
                Result = input;
            }
            else if (Comparer<T>.Default.Compare(input, Result) > 0)
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

    sealed class MaxRefType<T> : Consumer<T, T>
    {
        public MaxRefType() : base(default) { }

        public override ChainStatus ProcessNext(T input)
        {
            if (Result == null || (input != null && Comparer<T>.Default.Compare(input, Result) > 0))
            {
                Result = input;
            }

            return ChainStatus.Flow;
        }
    }

    sealed class MaxInt<TSource> : Consumer<TSource, int>
    {
        private readonly Func<TSource, int> _selector;

        bool _first;

        public MaxInt(Func<TSource, int> selector) : base(int.MinValue) =>
            (_selector, _first) = (selector, true);

        public override ChainStatus ProcessNext(TSource source)
        {
            var input = _selector(source);

            _first = false;
            if (input > Result)
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

    sealed class MaxNullableInt<TSource> : Consumer<TSource, int?>
    {
        private readonly Func<TSource, int?> _selector;

        public MaxNullableInt(Func<TSource, int?> selector) : base(null) =>
            _selector = selector;

        public override ChainStatus ProcessNext(TSource source)
        {
            var input = _selector(source);

            var maybeValue = input.GetValueOrDefault();
            if (!Result.HasValue || (input.HasValue && maybeValue > Result))
            {
                Result = input;
            }
            return ChainStatus.Flow;
        }
    }

    sealed class MaxLong<TSource> : Consumer<TSource, long>
    {
        private readonly Func<TSource, long> _selector;

        bool _first;

        public MaxLong(Func<TSource, long> selector) : base(long.MinValue) =>
            (_selector, _first) = (selector, true);

        public override ChainStatus ProcessNext(TSource source)
        {
            var input = _selector(source);

            _first = false;
            if (input > Result)
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

    sealed class MaxNullableLong<TSource> : Consumer<TSource, long?>
    {
        private readonly Func<TSource, long?> _selector;

        public MaxNullableLong(Func<TSource, long?> selector) : base(null) =>
            _selector = selector;

        public override ChainStatus ProcessNext(TSource source)
        {
            var input = _selector(source);

            var maybeValue = input.GetValueOrDefault();
            if (!Result.HasValue || (input.HasValue && maybeValue > Result))
            {
                Result = input;
            }
            return ChainStatus.Flow;
        }
    }

    sealed class MaxFloat<TSource> : Consumer<TSource, float>
    {
        private readonly Func<TSource, float> _selector;

        bool _first;

        public MaxFloat(Func<TSource, float> selector) : base(float.NaN) =>
            (_selector, _first) = (selector, true);

        public override ChainStatus ProcessNext(TSource source)
        {
            var input = _selector(source);

            _first = false;
            if (input > Result || float.IsNaN(Result))
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

    sealed class MaxNullableFloat<TSource> : Consumer<TSource, float?>
    {
        private readonly Func<TSource, float?> _selector;

        public MaxNullableFloat(Func<TSource, float?> selector) : base(null) =>
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

                Result = float.NaN;
            }

            if (input.HasValue)
            {
                var value = input.GetValueOrDefault();
                var result = Result.GetValueOrDefault();
                if (value > result || double.IsNaN(result))
                {
                    Result = value;
                }
            }

            return ChainStatus.Flow;
        }
    }

    sealed class MaxDouble<TSource> : Consumer<TSource, double>
    {
        private readonly Func<TSource, double> _selector;

        bool _first;

        public MaxDouble(Func<TSource, double> selector) : base(double.NaN) =>
            (_selector, _first) = (selector, true);

        public override ChainStatus ProcessNext(TSource source)
        {
            var input = _selector(source);

            _first = false;
            if (input > Result || double.IsNaN(Result))
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

    sealed class MaxNullableDouble<TSource> : Consumer<TSource, double?>
    {
        private readonly Func<TSource, double?> _selector;

        public MaxNullableDouble(Func<TSource, double?> selector) : base(null) =>
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

                Result = double.NaN;
            }

            if (input.HasValue)
            {
                var value = input.GetValueOrDefault();
                var result = Result.GetValueOrDefault();
                if (value > result || double.IsNaN(result))
                {
                    Result = value;
                }
            }

            return ChainStatus.Flow;
        }
    }

    sealed class MaxDecimal<TSource> : Consumer<TSource, decimal>
    {
        private readonly Func<TSource, decimal> _selector;

        bool _first;

        public MaxDecimal(Func<TSource, decimal> selector) : base(decimal.MinValue) =>
            (_selector, _first) = (selector, true);

        public override ChainStatus ProcessNext(TSource source)
        {
            var input = _selector(source);

            _first = false;
            if (input > Result)
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

    sealed class MaxNullableDecimal<TSource> : Consumer<TSource, decimal?>
    {
        private readonly Func<TSource, decimal?> _selector;

        public MaxNullableDecimal(Func<TSource, decimal?> selector) : base(null) =>
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
                if (value > Result.GetValueOrDefault())
                {
                    Result = value;
                }
            }

            return ChainStatus.Flow;
        }
    }

    sealed class MaxValueType<TSource, T> : Consumer<TSource, T>
    {
        private readonly Func<TSource, T> _selector;

        bool _first;

        public MaxValueType(Func<TSource, T> selector) : base(default) =>
            (_selector, _first) = (selector, true);

        public override ChainStatus ProcessNext(TSource source)
        {
            var input = _selector(source);

            if (_first)
            {
                _first = false;
                Result = input;
            }
            else if (Comparer<T>.Default.Compare(input, Result) > 0)
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

    sealed class MaxRefType<TSource, T> : Consumer<TSource, T>
    {
        private readonly Func<TSource, T> _selector;

        public MaxRefType(Func<TSource, T> selector) : base(default) =>
            _selector = selector;

        public override ChainStatus ProcessNext(TSource source)
        {
            var input = _selector(source);

            if (Result == null || (input != null && Comparer<T>.Default.Compare(input, Result) > 0))
            {
                Result = input;
            }

            return ChainStatus.Flow;
        }
    }

}
