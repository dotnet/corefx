namespace System.Linq.ChainLinq.Links
{
    sealed partial class SelectIndexed<T, U> : Optimizations.ISkipTakeOnConsumableLinkUpdate<T, U>
    {
        public ILink<T, U> Skip(int toSkip)
        {
            checked
            {
                return new SelectIndexed<T, U>(_selector, _startIndex + toSkip);
            }
        }
    }
}
