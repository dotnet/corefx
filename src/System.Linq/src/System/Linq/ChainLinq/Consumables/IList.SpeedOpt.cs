﻿namespace System.Linq.ChainLinq.Consumables
{
    sealed partial class IList<T, V>
        : Optimizations.ISkipTakeOnConsumable<V>
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

            return onlyIfCheap ? -1 : Consume(new Consumer.Count<V>());
        }

        public Consumable<V> Skip(int toSkip)
        {
            if (toSkip == 0)
                return this;

            if (Link is Optimizations.ISkipTakeOnConsumableLinkUpdate<T, V> skipLink)
            {
                checked
                {
                    var newCount = _count - toSkip;
                    if (newCount <= 0)
                    {
                        return Empty<V>.Instance;
                    }

                    var newStart = _start + toSkip;
                    var newLink = skipLink.Skip(toSkip);

                    return new IList<T, V>(_list, newStart, newCount, newLink);
                }
            }
            return AddTail(new Links.Skip<V>(toSkip));
        }

        public Consumable<V> Take(int toTake)
        {
            if (toTake <= 0)
            {
                return Empty<V>.Instance;
            }

            if (toTake >= _count)
            {
                return this;
            }

            if (Link is Optimizations.ISkipTakeOnConsumableLinkUpdate<T, V>)
            {
                return new IList<T, V>(_list, _start, toTake, Link);
            }

            return AddTail(new Links.Take<V>(toTake));
        }
    }
}
