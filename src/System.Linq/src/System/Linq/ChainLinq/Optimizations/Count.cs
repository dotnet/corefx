namespace System.Linq.ChainLinq.Optimizations
{
    interface ICountOnConsumable
    {
        int GetCount(bool onlyIfCheap);
    }

    interface ICountOnConsumableLink
    {
        int GetCount(int count);
    }
}
