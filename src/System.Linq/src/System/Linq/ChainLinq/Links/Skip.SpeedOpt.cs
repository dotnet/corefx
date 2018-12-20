namespace System.Linq.ChainLinq.Links
{
    sealed partial class Skip<T> : Optimizations.ISkipMerge<T>
    {
        public Consumable<T> Merge(ConsumableForMerging<T> consumable, int count)
        {
            if ((long)_count + count > int.MaxValue)
                return consumable.AddTail(new Skip<T>(count));

            var totalCount = _count + count;
            return consumable.ReplaceTailLink(new Skip<T>(totalCount));
        }
    }
}
