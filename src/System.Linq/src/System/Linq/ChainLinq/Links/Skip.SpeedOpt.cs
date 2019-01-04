namespace System.Linq.ChainLinq.Links
{
    sealed partial class Skip<T>
        : Optimizations.IMergeSkip<T>
        , Optimizations.ICountOnConsumableLink
    {
        public int GetCount(int count)
        {
            checked
            {
                return Math.Max(0, count - _toSkip);
            }
        }

        public Consumable<T> MergeSkip(ConsumableForMerging<T> consumable, int count)
        {
            if ((long)_toSkip + count > int.MaxValue)
                return consumable.AddTail(new Skip<T>(count));

            var totalCount = _toSkip + count;
            return consumable.ReplaceTailLink(new Skip<T>(totalCount));
        }
    }
}
