namespace System.Linq.ChainLinq.Optimizations
{
    internal static class Count
    {
        public static int GetCount<V>(Consumable<V> c, object link, int originalCount, bool onlyIfCheap)
        {
            if (link is ICountOnConsumableLink countLink)
            {
                var count = countLink.GetCount(originalCount);
                if (count >= 0)
                    return count;
            }

            return onlyIfCheap ? -1 : c.Consume(new Consumer.Count<V>());
        }
    }
}
