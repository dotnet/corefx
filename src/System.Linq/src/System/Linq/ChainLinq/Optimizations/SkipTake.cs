namespace System.Linq.ChainLinq.Optimizations
{
    interface ISkipTakeOnConsumable<T>
    {
        Consumable<T> Skip(int count);
        Consumable<T> Take(int count);
    }

    interface ISkipTakeOnConsumableLinkUpdate<T, U>
    {
        ILink<T, U> Skip(int count);
    }

    interface IMergeSkip<T>
    {
        Consumable<T> MergeSkip(ConsumableForMerging<T> consumable, int count);
    }
}
