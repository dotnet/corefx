namespace System.Linq.ChainLinq.Consumables
{
    sealed partial class Range<T>
        : Optimizations.ISkipTakeOnConsumable<T>
        , Optimizations.ICountOnConsumable
    {
        public int GetCount(bool onlyIfCheap)
        {
            if (Link is Optimizations.ICountOnConsumableLink countLink)
            {
                var count = countLink.GetCount(_count);
                if (count >= 0)
                    return count;
            }

            return onlyIfCheap ? -1 : Consume(new Consumer.Count<T>());
        }

        public Consumable<T> Skip(int count)
        {
            if (Link is Optimizations.ISkipTakeOnConsumableLinkUpdate<int,T> skipLink)
            {
                checked
                {
                    var newCount = _count - count;
                    if (newCount <= 0)
                    {
                        return Empty<T>.Instance;
                    }

                    var newStart = _start + count;
                    var newLink = skipLink.Skip(count);

                    return new Range<T>(newStart, newCount, newLink);
                }
            }
            return AddTail(new Links.Skip<T>(count));
        }

        public Consumable<T> Take(int count)
        {
            if (count <= 0)
            {
                return Empty<T>.Instance;
            }

            if (count >= _count)
            {
                return this;
            }

            if (Link is Optimizations.ISkipTakeOnConsumableLinkUpdate<int, T>)
            {
                return new Range<T>(_start, count, Link);
            }

            return AddTail(new Links.Take<T>(count));
        }
    }
}
