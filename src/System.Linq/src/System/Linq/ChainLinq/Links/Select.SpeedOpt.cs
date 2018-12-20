namespace System.Linq.ChainLinq.Links
{
    internal partial class Select<T, U> : Optimizations.ISkipTakeOnConsumableLinkUpdate<T, U>
    {
        public ILink<T, U> Skip(int count) => this;
    }
}
