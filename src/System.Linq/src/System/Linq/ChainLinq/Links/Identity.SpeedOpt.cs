namespace System.Linq.ChainLinq.Links
{
    partial class Identity<T> : Optimizations.ISkipTakeOnConsumableLinkUpdate<T, T>
    {
        public ILink<T, T> Skip(int count) => this;
    }
}
