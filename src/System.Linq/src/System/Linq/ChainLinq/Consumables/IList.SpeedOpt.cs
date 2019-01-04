namespace System.Linq.ChainLinq.Consumables
{
    sealed partial class IList<T, V>
        : Optimizations.ISkipTakeOnConsumable<V>
        , Optimizations.ICountOnConsumable
    {
        public int GetCount(bool onlyIfCheap) =>
            Optimizations.Count.GetCount(this, Link, _count, onlyIfCheap);

        public V Last(bool orDefault) =>
            Optimizations.SkipTake.Last(this, _list, _start, _count, orDefault);
        
        public Consumable<V> Skip(int toSkip) =>
            Optimizations.SkipTake.Skip(this, _list, _start, _count, toSkip);

        public Consumable<V> Take(int toTake) =>
            Optimizations.SkipTake.Take(this, _list, _start, _count, toTake);
    }
}
