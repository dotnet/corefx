namespace System.Linq.ChainLinq.Consumables
{
    sealed partial class List<T, V>
        : Optimizations.ISkipTakeOnConsumable<V>
        , Optimizations.ICountOnConsumable
    {
        public int GetCount(bool onlyIfCheap) =>
            Optimizations.Count.GetCount(this, Link, Underlying.Count, onlyIfCheap);

        public V Last(bool orDefault) =>
            Optimizations.SkipTake.Last(this, Underlying, 0, Underlying.Count, orDefault);

        public Consumable<V> Skip(int toSkip) =>
            Optimizations.SkipTake.Skip(this, Underlying, 0, Underlying.Count, toSkip);

        public Consumable<V> Take(int toTake) =>
            Optimizations.SkipTake.Take(this, Underlying, 0, Underlying.Count, toTake);
    }
}
