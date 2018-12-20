namespace System.Linq.ChainLinq.Links
{
    partial class Identity<T>
        : Optimizations.ISkipTakeOnConsumableLinkUpdate<T, T>
        , Optimizations.ICountOnConsumableLink
    {
        public int GetCount(int count) => count;

        public ILink<T, T> Skip(int count) => this;
    }
}
