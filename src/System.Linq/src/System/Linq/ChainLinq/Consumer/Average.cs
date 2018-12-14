using System;
using System.Collections.Generic;
using System.Text;

namespace System.Linq.ChainLinq.Consumer
{
    class AverageInt : Consumer<int, double>
    {
        long _sum;
        long _count;

        public AverageInt() : base(default) =>
            _count = 0;

        public override ChainStatus ProcessNext(int input)
        {
            if (_count == 0)
            {
                _sum = input;
                _count = 1;
            }
            else
            {
                checked
                {
                    _sum += input;
                    ++_count;
                }
            }
            return ChainStatus.Flow;
        }

        public override void ChainComplete()
        {
            if (_count == 0)
            {
                throw Error.NoElements();
            }
            Result = (double)_sum / _count;
        }
    }

    class AverageNullableInt : Consumer<int?, double?>
    {
        long _sum;
        long _count;

        public AverageNullableInt() : base(default) =>
            _count = 0;

        public override ChainStatus ProcessNext(int? input)
        {
            if (!input.HasValue)
                return ChainStatus.Filter;

            if (_count == 0)
            {
                _sum = input.GetValueOrDefault();
                _count = 1;
            }
            else
            {
                checked
                {
                    _sum += input.GetValueOrDefault();
                    ++_count;
                }
            }
            return ChainStatus.Flow;
        }

        public override void ChainComplete()
        {
            if (_count != 0)
            {
                Result = (double)_sum / _count;
            }
        }
    }

    class AverageLong : Consumer<long, double>
    {
        long _sum;
        long _count;

        public AverageLong() : base(default) =>
            _count = 0;

        public override ChainStatus ProcessNext(long input)
        {
            if (_count == 0)
            {
                _sum = input;
                _count = 1;
            }
            else
            {
                checked
                {
                    _sum += input;
                    ++_count;
                }
            }
            return ChainStatus.Flow;
        }

        public override void ChainComplete()
        {
            if (_count == 0)
            {
                throw Error.NoElements();
            }
            Result = (double)_sum / _count;
        }
    }

    class AverageNullableLong : Consumer<long?, double?>
    {
        long _sum;
        long _count;

        public AverageNullableLong() : base(default) =>
            _count = 0;

        public override ChainStatus ProcessNext(long? input)
        {
            if (!input.HasValue)
                return ChainStatus.Filter;

            if (_count == 0)
            {
                _sum = input.GetValueOrDefault();
                _count = 1;
            }
            else
            {
                checked
                {
                    _sum += input.GetValueOrDefault();
                    ++_count;
                }
            }
            return ChainStatus.Flow;
        }

        public override void ChainComplete()
        {
            if (_count != 0)
            {
                Result = (double)_sum / _count;
            }
        }
    }

    class AverageFloat : Consumer<float, float>
    {
        double _sum;
        long _count;

        public AverageFloat() : base(default) =>
            _count = 0;

        public override ChainStatus ProcessNext(float input)
        {
            if (_count == 0)
            {
                _sum = input;
                _count = 1;
            }
            else
            {
                _sum += input;
                ++_count;
            }
            return ChainStatus.Flow;
        }

        public override void ChainComplete()
        {
            if (_count == 0)
            {
                throw Error.NoElements();
            }
            Result = (float)(_sum / _count);
        }
    }

    class AverageNullableFloat : Consumer<float?, float?>
    {
        double _sum;
        long _count;

        public AverageNullableFloat() : base(default) =>
            _count = 0;

        public override ChainStatus ProcessNext(float? input)
        {
            if (!input.HasValue)
                return ChainStatus.Filter;

            if (_count == 0)
            {
                _sum = input.GetValueOrDefault();
                _count = 1;
            }
            else
            {
                _sum += input.GetValueOrDefault();
                ++_count;
            }
            return ChainStatus.Flow;
        }

        public override void ChainComplete()
        {
            if (_count != 0)
            {
                Result = (float)(_sum / _count);
            }
        }
    }


    class AverageDouble : Consumer<double, double>
    {
        double _sum;
        long _count;

        public AverageDouble() : base(default) =>
            _count = 0;

        public override ChainStatus ProcessNext(double input)
        {
            if (_count == 0)
            {
                _sum = input;
                _count = 1;
            }
            else
            {
                // There is an opportunity to short-circuit here, in that if e.Current is
                // ever NaN then the result will always be NaN. Assuming that this case is
                // rare enough that not checking is the better approach generally.
                _sum += input;
                ++_count;
            }
            return ChainStatus.Flow;
        }

        public override void ChainComplete()
        {
            if (_count == 0)
            {
                throw Error.NoElements();
            }
            Result = _sum / _count;
        }
    }

    class AverageNullableDouble : Consumer<double?, double?>
    {
        double _sum;
        long _count;

        public AverageNullableDouble() : base(default) =>
            _count = 0;

        public override ChainStatus ProcessNext(double? input)
        {
            if (!input.HasValue)
                return ChainStatus.Filter;

            if (_count == 0)
            {
                _sum = input.GetValueOrDefault();
                _count = 1;
            }
            else
            {
                _sum += input.GetValueOrDefault();
                ++_count;
            }
            return ChainStatus.Flow;
        }

        public override void ChainComplete()
        {
            if (_count != 0)
            {
                Result = _sum / _count;
            }
        }
    }

    class AverageDecimal : Consumer<decimal, decimal>
    {
        decimal _sum;
        long _count;

        public AverageDecimal() : base(default) =>
            _count = 0;

        public override ChainStatus ProcessNext(decimal input)
        {
            if (_count == 0)
            {
                _sum = input;
                _count = 1;
            }
            else
            {
                _sum += input;
                ++_count;
            }
            return ChainStatus.Flow;
        }

        public override void ChainComplete()
        {
            if (_count == 0)
            {
                throw Error.NoElements();
            }
            Result = _sum / _count;
        }
    }

    class AverageNullableDecimal : Consumer<decimal?, decimal?>
    {
        decimal _sum;
        long _count;

        public AverageNullableDecimal() : base(default) =>
            _count = 0;

        public override ChainStatus ProcessNext(decimal? input)
        {
            if (!input.HasValue)
                return ChainStatus.Filter;

            if (_count == 0)
            {
                _sum = input.GetValueOrDefault();
                _count = 1;
            }
            else
            {
                _sum += input.GetValueOrDefault();
                ++_count;
            }
            return ChainStatus.Flow;
        }

        public override void ChainComplete()
        {
            if (_count != 0)
            {
                Result = _sum / _count;
            }
        }
    }

    class AverageInt<TSource> : Consumer<TSource, double>
    {
        readonly Func<TSource, int> _selector;

        long _sum;
        long _count;

        public AverageInt(Func<TSource, int> selector) : base(default) =>
            (_selector, _count) = (selector, 0);

        public override ChainStatus ProcessNext(TSource source)
        {
            var input = _selector(source);
            if (_count == 0)
            {
                _sum = input;
                _count = 1;
            }
            else
            {
                checked
                {
                    _sum += input;
                    ++_count;
                }
            }
            return ChainStatus.Flow;
        }

        public override void ChainComplete()
        {
            if (_count == 0)
            {
                throw Error.NoElements();
            }
            Result = (double)_sum / _count;
        }
    }

    class AverageNullableInt<TSource> : Consumer<TSource, double?>
    {
        readonly Func<TSource, int?> _selector;

        long _sum;
        long _count;

        public AverageNullableInt(Func<TSource, int?> selector) : base(default) =>
            (_selector, _count) = (selector, 0);

        public override ChainStatus ProcessNext(TSource source)
        {
            var input = _selector(source);
            if (!input.HasValue)
                return ChainStatus.Filter;

            if (_count == 0)
            {
                _sum = input.GetValueOrDefault();
                _count = 1;
            }
            else
            {
                checked
                {
                    _sum += input.GetValueOrDefault();
                    ++_count;
                }
            }
            return ChainStatus.Flow;
        }

        public override void ChainComplete()
        {
            if (_count != 0)
            {
                Result = (double)_sum / _count;
            }
        }
    }

    class AverageLong<TSource> : Consumer<TSource, double>
    {
        readonly Func<TSource, long> _selector;

        long _sum;
        long _count;

        public AverageLong(Func<TSource, long> selector) : base(default) =>
            (_selector, _count) = (selector, 0);

        public override ChainStatus ProcessNext(TSource source)
        {
            var input = _selector(source);
            if (_count == 0)
            {
                _sum = input;
                _count = 1;
            }
            else
            {
                checked
                {
                    _sum += input;
                    ++_count;
                }
            }
            return ChainStatus.Flow;
        }

        public override void ChainComplete()
        {
            if (_count == 0)
            {
                throw Error.NoElements();
            }
            Result = (double)_sum / _count;
        }
    }

    class AverageNullableLong<TSource> : Consumer<TSource, double?>
    {
        readonly Func<TSource, long?> _selector;

        long _sum;
        long _count;

        public AverageNullableLong(Func<TSource, long?> selector) : base(default) =>
            (_selector, _count) = (selector, 0);

        public override ChainStatus ProcessNext(TSource source)
        {
            var input = _selector(source);
            if (!input.HasValue)
                return ChainStatus.Filter;

            if (_count == 0)
            {
                _sum = input.GetValueOrDefault();
                _count = 1;
            }
            else
            {
                checked
                {
                    _sum += input.GetValueOrDefault();
                    ++_count;
                }
            }
            return ChainStatus.Flow;
        }

        public override void ChainComplete()
        {
            if (_count != 0)
            {
                Result = (double)_sum / _count;
            }
        }
    }

    class AverageFloat<TSource> : Consumer<TSource, float>
    {
        readonly Func<TSource, float> _selector;

        double _sum;
        long _count;

        public AverageFloat(Func<TSource, float> selector) : base(default) =>
            (_selector, _count) = (selector, 0);

        public override ChainStatus ProcessNext(TSource source)
        {
            var input = _selector(source);
            if (_count == 0)
            {
                _sum = input;
                _count = 1;
            }
            else
            {
                _sum += input;
                ++_count;
            }
            return ChainStatus.Flow;
        }

        public override void ChainComplete()
        {
            if (_count == 0)
            {
                throw Error.NoElements();
            }
            Result = (float)(_sum / _count);
        }
    }

    class AverageNullableFloat<TSource> : Consumer<TSource, float?>
    {
        readonly Func<TSource, float?> _selector;

        double _sum;
        long _count;

        public AverageNullableFloat(Func<TSource, float?> selector) : base(default) =>
            (_selector, _count) = (selector, 0);

        public override ChainStatus ProcessNext(TSource source)
        {
            var input = _selector(source);
            if (!input.HasValue)
                return ChainStatus.Filter;

            if (_count == 0)
            {
                _sum = input.GetValueOrDefault();
                _count = 1;
            }
            else
            {
                _sum += input.GetValueOrDefault();
                ++_count;
            }
            return ChainStatus.Flow;
        }

        public override void ChainComplete()
        {
            if (_count != 0)
            {
                Result = (float)(_sum / _count);
            }
        }
    }


    class AverageDouble<TSource> : Consumer<TSource, double>
    {
        readonly Func<TSource, double> _selector;

        double _sum;
        long _count;

        public AverageDouble(Func<TSource, double> selector) : base(default) =>
            (_selector, _count) = (selector, 0);

        public override ChainStatus ProcessNext(TSource source)
        {
            var input = _selector(source);
            if (_count == 0)
            {
                _sum = input;
                _count = 1;
            }
            else
            {
                // There is an opportunity to short-circuit here, in that if e.Current is
                // ever NaN then the result will always be NaN. Assuming that this case is
                // rare enough that not checking is the better approach generally.
                _sum += input;
                ++_count;
            }
            return ChainStatus.Flow;
        }

        public override void ChainComplete()
        {
            if (_count == 0)
            {
                throw Error.NoElements();
            }
            Result = _sum / _count;
        }
    }

    class AverageNullableDouble<TSource> : Consumer<TSource, double?>
    {
        readonly Func<TSource, double?> _selector;

        double _sum;
        long _count;

        public AverageNullableDouble(Func<TSource, double?> selector) : base(default) =>
            (_selector, _count) = (selector, 0);

        public override ChainStatus ProcessNext(TSource source)
        {
            var input = _selector(source);
            if (!input.HasValue)
                return ChainStatus.Filter;

            if (_count == 0)
            {
                _sum = input.GetValueOrDefault();
                _count = 1;
            }
            else
            {
                _sum += input.GetValueOrDefault();
                ++_count;
            }
            return ChainStatus.Flow;
        }

        public override void ChainComplete()
        {
            if (_count != 0)
            {
                Result = _sum / _count;
            }
        }
    }

    class AverageDecimal<TSource> : Consumer<TSource, decimal>
    {
        readonly Func<TSource, decimal> _selector;

        decimal _sum;
        long _count;

        public AverageDecimal(Func<TSource, decimal> selector) : base(default) =>
            (_selector, _count) = (selector, 0);

        public override ChainStatus ProcessNext(TSource source)
        {
            var input = _selector(source);
            if (_count == 0)
            {
                _sum = input;
                _count = 1;
            }
            else
            {
                _sum += input;
                ++_count;
            }
            return ChainStatus.Flow;
        }

        public override void ChainComplete()
        {
            if (_count == 0)
            {
                throw Error.NoElements();
            }
            Result = _sum / _count;
        }
    }

    class AverageNullableDecimal<TSource> : Consumer<TSource, decimal?>
    {
        readonly Func<TSource, decimal?> _selector;

        decimal _sum;
        long _count;

        public AverageNullableDecimal(Func<TSource, decimal?> selector) : base(default) =>
            (_selector, _count) = (selector, 0);

        public override ChainStatus ProcessNext(TSource source)
        {
            var input = _selector(source);
            if (!input.HasValue)
                return ChainStatus.Filter;

            if (_count == 0)
            {
                _sum = input.GetValueOrDefault();
                _count = 1;
            }
            else
            {
                _sum += input.GetValueOrDefault();
                ++_count;
            }
            return ChainStatus.Flow;
        }

        public override void ChainComplete()
        {
            if (_count != 0)
            {
                Result = _sum / _count;
            }
        }
    }

}
