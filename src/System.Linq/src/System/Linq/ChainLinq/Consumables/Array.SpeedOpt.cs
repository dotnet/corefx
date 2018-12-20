namespace System.Linq.ChainLinq.Consumables
{
    internal partial class Array<T, V>
        : Optimizations.ICountOnConsumable
    {
        public int GetCount(bool onlyIfCheap)
        {
            if (Link is Optimizations.ICountOnConsumableLink countLink)
            {
                var count = countLink.GetCount(_array.Length);
                if (count >= 0)
                    return count;
            }

            return onlyIfCheap ? -1 : Consume(new Consumer.Count<V>());
        }
    }
}
