using System.Collections.Generic;

namespace System.Linq.ChainLinq.Consumer
{
    sealed class ToArrayKnownSize<T> : Consumer<T, T[]>
    {
        private int _index;

        public ToArrayKnownSize(int count) : base(new T[count]) =>
            _index = 0;

        public override ChainStatus ProcessNext(T input)
        {
            Result[_index++] = input;
            return ChainStatus.Flow;
        }
    }

    sealed class ToArrayViaBuilder<T> : Consumer<T, T[]>
    {
        LargeArrayBuilder<T> builder;

        public ToArrayViaBuilder() : base(null) =>
            builder = new LargeArrayBuilder<T>(true);

        public override ChainStatus ProcessNext(T input)
        {
            builder.Add(input);
            return ChainStatus.Flow;
        }

        public override void ChainComplete()
        {
            Result = builder.ToArray();
        }
    }
}
