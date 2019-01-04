namespace System.Linq.ChainLinq.Consumables
{
    sealed partial class Array<T, V>
        : Optimizations.ISkipTakeOnConsumable<V>
        , Optimizations.ICountOnConsumable
    {
        public int GetCount(bool onlyIfCheap) =>
            Optimizations.Count.GetCount(this, this.Link, _array.Length, onlyIfCheap);

        public V Last(bool orDefault) =>
            Optimizations.SkipTake.Last(this, _array, 0, _array.Length, orDefault);

        public Consumable<V> Skip(int toSkip) =>
            Optimizations.SkipTake.Skip(this, _array, 0, _array.Length, toSkip);

        public Consumable<V> Take(int toTake) =>
            Optimizations.SkipTake.Take(this, _array, 0, _array.Length, toTake);
    }
}
