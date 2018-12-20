namespace System.Linq.ChainLinq.Consumables
{
    internal partial class Range<T> : Optimizations.ISkipTakeOnConsumable<T>
    {
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
            if (count >= _count)
                return this;

            return new Range<T>(_start, _count, Link);
        }
    }
}
