namespace System.Linq.ChainLinq.Links
{
    sealed partial class Take<T> : Optimizations.ICountOnConsumableLink
    {
        public int GetCount(int count)
        {
            checked
            {
                return Math.Min(_count, count);
            }
        }
    }
}
