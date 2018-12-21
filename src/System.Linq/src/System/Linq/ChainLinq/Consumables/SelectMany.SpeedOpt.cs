using System.Collections.Generic;

namespace System.Linq.ChainLinq.Consumables
{
    sealed class SelectManyCount<T> : Consumer<IEnumerable<T>, int>
    {
        public SelectManyCount() : base(0) { }

        public override ChainStatus ProcessNext(IEnumerable<T> input)
        {
            checked
            {
                Result += input.Count();
            }
            return ChainStatus.Flow;
        }
    }

    sealed partial class SelectMany<T, V>
        : Optimizations.ICountOnConsumable
    {
        public int GetCount(bool onlyIfCheap)
        {
            if (onlyIfCheap)
            {
                return -1;
            }

            if (Link is Optimizations.ICountOnConsumableLink countLink)
            {
                var underlyingCount = _selectMany.Consume(new SelectManyCount<T>());

                var count = countLink.GetCount(underlyingCount);
                if (underlyingCount >= 0)
                    return underlyingCount;
            }

            return Consume(new Consumer.Count<V>());
        }
    }
}
