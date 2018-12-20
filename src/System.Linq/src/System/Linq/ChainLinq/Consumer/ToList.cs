using System.Collections.Generic;

namespace System.Linq.ChainLinq.Consumer
{
    sealed class ToList<T> : Consumer<T, List<T>>
    {
        public ToList() : base(new List<T>()) { }

        public ToList(int count) : base(new List<T>(count)) { }

        public override ChainStatus ProcessNext(T input)
        {
            Result.Add(input);
            return ChainStatus.Flow;
        }
    }
}
