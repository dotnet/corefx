namespace System.Linq.ChainLinq.Consumables
{
    /// <summary>
    /// https://github.com/xunit/xunit/issues/1870
    /// </summary>
    /// <typeparam name="U"></typeparam>
    /// <typeparam name="T"></typeparam>
    internal abstract class Base_Generic_Arguments_Reversed_To_Work_Around_XUnit_Bug<U, T> : Consumable<U>
    {
        protected ILink<T, U> Link { get; }

        protected Base_Generic_Arguments_Reversed_To_Work_Around_XUnit_Bug(ILink<T, U> link) =>
            Link = link;

        public override Consumable<V> AddTail<V>(ILink<U, V> next) =>
            Create(Links.Composition.Create(Link, next));

        public abstract Consumable<V> Create<V>(ILink<T, V> first);

        protected bool IsIdentity => ReferenceEquals(Link, Links.Identity<T>.Instance);
    }
}
