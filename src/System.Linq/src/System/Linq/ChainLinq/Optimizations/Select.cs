namespace System.Linq.ChainLinq.Optimizations
{
    interface IMergeSelect<T>
    {
        Consumable<U> MergeSelect<U>(ConsumableForMerging<T> consumable, Func<T, U> selector);
    }
}
