namespace System.Linq.ChainLinq.Links
{
    internal partial class Where<T> : Optimizations.IMergeSelect<T>
    {
        public Consumable<U> MergeSelect<U>(ConsumableForMerging<T> consumable, Func<T, U> selector) =>
            consumable.ReplaceTailLink(new WhereSelect<T, U>(Predicate, selector));
    }
}
