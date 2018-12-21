namespace System.Linq.ChainLinq.Consumer
{
    sealed class SumInt : Consumer<int, int>
    {
        public SumInt() : base(0) { }

        public override ChainStatus ProcessNext(int input)
        {
            checked
            {
                Result += input;
            }
            return ChainStatus.Flow;
        }
    }

    sealed class SumNullableInt : Consumer<int?, int?>
    {
        public SumNullableInt() : base(0) { }

        public override ChainStatus ProcessNext(int? input)
        {
            checked
            {
                Result += input.GetValueOrDefault();
            }
            return ChainStatus.Flow;
        }
    }

    sealed class SumLong : Consumer<long, long>
    {
        public SumLong() : base(0L) { }

        public override ChainStatus ProcessNext(long input)
        {
            checked
            {
                Result += input;
            }
            return ChainStatus.Flow;
        }
    }

    sealed class SumNullableLong : Consumer<long?, long?>
    {
        public SumNullableLong() : base(0L) { }

        public override ChainStatus ProcessNext(long? input)
        {
            checked
            {
                Result += input.GetValueOrDefault();
            }
            return ChainStatus.Flow;
        }
    }


    sealed class SumFloat : Consumer<float, float>
    {
        double _sum = 0.0;

        public SumFloat() : base(default) { }

        public override ChainStatus ProcessNext(float input)
        {
            _sum += input;
            return ChainStatus.Flow;
        }

        public override void ChainComplete()
        {
            Result = (float)_sum;
        }
    }

    sealed class SumNullableFloat : Consumer<float?, float?>
    {
        double _sum = 0.0;

        public SumNullableFloat() : base(default) { }

        public override ChainStatus ProcessNext(float? input)
        {
            _sum += input.GetValueOrDefault();
            return ChainStatus.Flow;
        }

        public override void ChainComplete()
        {
            Result = (float)_sum;
        }
    }

    sealed class SumDouble : Consumer<double, double>
    {
        public SumDouble() : base(0.0) { }

        public override ChainStatus ProcessNext(double input)
        {
            Result += input;
            return ChainStatus.Flow;
        }
    }

    sealed class SumNullableDouble : Consumer<double?, double?>
    {
        public SumNullableDouble() : base(0.0) { }

        public override ChainStatus ProcessNext(double? input)
        {
            Result += input.GetValueOrDefault();
            return ChainStatus.Flow;
        }
    }

    sealed class SumDecimal : Consumer<decimal, decimal>
    {
        public SumDecimal() : base(0M) { }

        public override ChainStatus ProcessNext(decimal input)
        {
            Result += input;
            return ChainStatus.Flow;
        }
    }

    sealed class SumNullableDecimal : Consumer<decimal?, decimal?>
    {
        public SumNullableDecimal() : base(0M) { }

        public override ChainStatus ProcessNext(decimal? input)
        {
            Result += input.GetValueOrDefault();
            return ChainStatus.Flow;
        }
    }


    sealed class SumInt<TSource> : Consumer<TSource, int>
    {
        Func<TSource, int> _selector;

        public SumInt(Func<TSource, int> selector) : base(0) =>
            _selector = selector;

        public override ChainStatus ProcessNext(TSource input)
        {
            checked
            {
                Result += _selector(input);
            }
            return ChainStatus.Flow;
        }
    }

    sealed class SumNullableInt<TSource> : Consumer<TSource, int?>
    {
        Func<TSource, int?> _selector;

        public SumNullableInt(Func<TSource, int?> selector) : base(0) =>
            _selector = selector;

        public override ChainStatus ProcessNext(TSource input)
        {
            checked
            {
                Result += _selector(input).GetValueOrDefault();
            }
            return ChainStatus.Flow;
        }
    }

    sealed class SumLong<TSource> : Consumer<TSource, long>
    {
        Func<TSource, long> _selector;

        public SumLong(Func<TSource, long> selector) : base(0L) =>
            _selector = selector;

        public override ChainStatus ProcessNext(TSource input)
        {
            checked
            {
                Result += _selector(input);
            }
            return ChainStatus.Flow;
        }
    }

    sealed class SumNullableLong<TSource> : Consumer<TSource, long?>
    {
        Func<TSource, long?> _selector;

        public SumNullableLong(Func<TSource, long?> selector) : base(0L) =>
            _selector = selector;

        public override ChainStatus ProcessNext(TSource input)
        {
            checked
            {
                Result += _selector(input).GetValueOrDefault();
            }
            return ChainStatus.Flow;
        }
    }


    sealed class SumFloat<TSource> : Consumer<TSource, float>
    {
        double _sum = 0.0;

        Func<TSource, float> _selector;

        public SumFloat(Func<TSource, float> selector) : base(default) =>
            _selector = selector;

        public override ChainStatus ProcessNext(TSource input)
        {
            _sum += _selector(input);
            return ChainStatus.Flow;
        }

        public override void ChainComplete()
        {
            Result = (float)_sum;
        }
    }

    sealed class SumNullableFloat<TSource> : Consumer<TSource, float?>
    {
        double _sum = 0.0;

        Func<TSource, float?> _selector;

        public SumNullableFloat(Func<TSource, float?> selector) : base(default) =>
            _selector = selector;

        public override ChainStatus ProcessNext(TSource input)
        {
            _sum += _selector(input).GetValueOrDefault();
            return ChainStatus.Flow;
        }

        public override void ChainComplete()
        {
            Result = (float)_sum;
        }
    }

    sealed class SumDouble<TSource> : Consumer<TSource, double>
    {
        Func<TSource, double> _selector;

        public SumDouble(Func<TSource, double> selector) : base(0.0) =>
            _selector = selector;

        public override ChainStatus ProcessNext(TSource input)
        {
            Result += _selector(input);
            return ChainStatus.Flow;
        }
    }

    sealed class SumNullableDouble<TSource> : Consumer<TSource, double?>
    {
        Func<TSource, double?> _selector;

        public SumNullableDouble(Func<TSource, double?> selector) : base(0.0) =>
            _selector = selector;

        public override ChainStatus ProcessNext(TSource input)
        {
            Result += _selector(input).GetValueOrDefault();
            return ChainStatus.Flow;
        }
    }

    sealed class SumDecimal<TSource> : Consumer<TSource, decimal>
    {
        Func<TSource, decimal> _selector;

        public SumDecimal(Func<TSource, decimal> selector) : base(0M) =>
            _selector = selector;

        public override ChainStatus ProcessNext(TSource input)
        {
            Result += _selector(input);
            return ChainStatus.Flow;
        }
    }

    sealed class SumNullableDecimal<TSource> : Consumer<TSource, decimal?>
    {
        Func<TSource, decimal?> _selector;

        public SumNullableDecimal(Func<TSource, decimal?> selector) : base(0M) =>
            _selector = selector;

        public override ChainStatus ProcessNext(TSource input)
        {
            Result += _selector(input).GetValueOrDefault();
            return ChainStatus.Flow;
        }
    }
}
