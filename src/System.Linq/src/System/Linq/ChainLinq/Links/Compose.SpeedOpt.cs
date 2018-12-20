namespace System.Linq.ChainLinq.Links
{
    partial class Composition<T, U, V>
        : Optimizations.ICountOnConsumableLink
    {
        public int GetCount(int count)
        {
            if (_first is Optimizations.ICountOnConsumableLink first && _second is Optimizations.ICountOnConsumableLink second)
            {
                count = first.GetCount(count);
                return count < 0 ? count : second.GetCount(count);
            }
            return -1;
        }
    }
}
