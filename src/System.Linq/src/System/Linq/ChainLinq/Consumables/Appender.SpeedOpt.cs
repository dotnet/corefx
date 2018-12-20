namespace System.Linq.ChainLinq.Consumables
{
    internal partial class Appender<T>
        : Optimizations.ICountOnConsumable
    {
        public int GetCount(bool onlyIfCheap)
        {
            if (_count < 0)
                throw new OverflowException();

            return _count;
        }
    }
}
