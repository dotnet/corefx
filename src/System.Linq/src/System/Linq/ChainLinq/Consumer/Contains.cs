using System.Collections.Generic;

namespace System.Linq.ChainLinq.Consumer
{
    sealed class Contains<T> : Consumer<T, bool>
    {
        private readonly T _value;

        public Contains(T value) : base(false) =>
            _value = value;

        public override ChainStatus ProcessNext(T input)
        {
            if (EqualityComparer<T>.Default.Equals(input, _value)) // benefits from devirtualization and likely inlining
            {
                Result = true;
                return ChainStatus.StoppedConsumer;
            }
            return ChainStatus.Flow;
        }
    }

    sealed class ContainsWithComparer<T> : Consumer<T, bool>
    {
        private IEqualityComparer<T> _comparer;
        private readonly T _value;

        public ContainsWithComparer(T value, IEqualityComparer<T> comparer) : base(false) =>
            (_value, _comparer) = (value, comparer);

        public override ChainStatus ProcessNext(T input)
        {
            if (_comparer.Equals(input, _value))
            {
                Result = true;
                return ChainStatus.StoppedConsumer;
            }
            return ChainStatus.Flow;
        }
    }
}
