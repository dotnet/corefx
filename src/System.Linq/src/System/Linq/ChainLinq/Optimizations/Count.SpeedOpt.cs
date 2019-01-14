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

            if (onlyIfCheap)
            {
                return -1;
            }

            var counter = new Consumer.Count<V>();
            c.Consume(counter);
            return counter.Result;
        }
    }
}
