using System.Collections.Generic;
using System.Diagnostics;

namespace System.Linq.ChainLinq.Consumables
{
    sealed class Enumerable<T, V> : Base_Generic_Arguments_Reversed_To_Work_Around_XUnit_Bug<V, T>
    {
        private readonly IEnumerable<T> _enumerable;

        public Enumerable(IEnumerable<T> enumerable, Link<T, V> link) : base(link) =>
            _enumerable = enumerable;

        public override Consumable<W> Create<W>(Link<T, W> first) =>
            new Enumerable<T, W>(_enumerable, first);

        public override IEnumerator<V> GetEnumerator()
        {
            if (Link.LinkType == Links.LinkType.Where)
            {
                Debug.Assert(typeof(T) == typeof(V));
                return GetWhereEnumerator(this);
            }

            return ChainLinq.GetEnumerator.Enumerable.Get(_enumerable, Link);
        }

        // Moved up from optimization area as fairly common and avoids an expensive IEnumerator<V> cast
        private static IEnumerator<V> GetWhereEnumerator(object thisAsObject)
        {
            var eVV = (Enumerable<V, V>)thisAsObject;
            return new GetEnumerator.Enumerable.WhereEnumerator<V>(eVV._enumerable, ((Links.Where<V>)eVV.Link).Predicate);
        }

        public override TResult Consume<TResult>(Consumer<V, TResult> consumer) =>
            ChainLinq.Consume.Enumerable.Invoke(_enumerable, Link, consumer);
    }
}
