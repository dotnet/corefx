namespace System.Linq.ChainLinq.Optimizations
{
    interface ISkipTakeOnConsumable<T>
    {
        Consumable<T> Skip(int toSkip);
        Consumable<T> Take(int toTake);
    }

    interface ISkipTakeOnConsumableLinkUpdate<T, U>
    {
        ILink<T, U> Skip(int toSkip);
    }

    interface IMergeSkip<T>
    {
        Consumable<T> MergeSkip(ConsumableForMerging<T> consumable, int count);
    }
}
