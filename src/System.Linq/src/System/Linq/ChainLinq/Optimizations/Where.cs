namespace System.Linq.ChainLinq.Optimizations
{
    interface IMergeWhere<T>
    {
        Consumable<T> MergeWhere(ConsumableForMerging<T> consumable, Func<T, bool> predicate);
    }
}
