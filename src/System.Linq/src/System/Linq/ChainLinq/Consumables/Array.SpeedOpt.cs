namespace System.Linq.ChainLinq.Consumables
{
    sealed partial class Array<T, V>
        : Optimizations.ISkipTakeOnConsumable<V>
        , Optimizations.ICountOnConsumable
    {
        public int GetCount(bool onlyIfCheap) =>
            Optimizations.Count.GetCount(this, this.Link, Underlying.Length, onlyIfCheap);

        public V Last(bool orDefault) =>
            Optimizations.SkipTake.Last(this, Underlying, 0, Underlying.Length, orDefault);

        public Consumable<V> Skip(int toSkip) =>
            Optimizations.SkipTake.Skip(this, Underlying, 0, Underlying.Length, toSkip);

        public Consumable<V> Take(int toTake) =>
            Optimizations.SkipTake.Take(this, Underlying, 0, Underlying.Length, toTake);
    }
}
