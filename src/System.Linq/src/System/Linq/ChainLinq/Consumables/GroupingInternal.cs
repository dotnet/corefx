using System.Diagnostics;

namespace System.Linq.ChainLinq.Consumables
{
    internal interface IConsumableProvider<TElement>
    {
        Consumable<U> GetConsumable<U>(Link<TElement, U> transform);
    }

    // Grouping is a publically exposed class, so we provide this class get the Consumable
    [DebuggerDisplay("Key = {Key}")]
    [DebuggerTypeProxy(typeof(SystemLinq_GroupingDebugView<,>))]
    internal class GroupingInternal<TKey, TElement>
        : Grouping<TKey, TElement>
        , IConsumableProvider<TElement>
    {
        internal GroupingInternal(GroupingArrayPool<TElement> pool) : base(pool)
        {
        }

        public Consumable<U> GetConsumable<U>(Link<TElement, U> transform) =>
            new Array<TElement, U>(_elements, 0, _count, transform);
    }
}
