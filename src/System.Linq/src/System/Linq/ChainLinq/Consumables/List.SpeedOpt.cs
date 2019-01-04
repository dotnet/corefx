namespace System.Linq.ChainLinq.Consumables
{
    sealed partial class List<T, V>
        : Optimizations.ISkipTakeOnConsumable<V>
        , Optimizations.ICountOnConsumable
    {
        public int GetCount(bool onlyIfCheap) =>
            Optimizations.Count.GetCount(this, Link, _list.Count, onlyIfCheap);

        public V Last(bool orDefault) =>
            Optimizations.SkipTake.Last(this, _list, 0, _list.Count, orDefault);

        public Consumable<V> Skip(int toSkip) =>
            Optimizations.SkipTake.Skip(this, _list, 0, _list.Count, toSkip);

        public Consumable<V> Take(int toTake) =>
            Optimizations.SkipTake.Take(this, _list, 0, _list.Count, toTake);
    }
}
