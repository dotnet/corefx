using System.Linq.ChainLinq.Optimizations;

namespace System.Linq.ChainLinq.Consumables
{
    sealed partial class SelectArray<T, U> : IMergeSelect<U>, IMergeWhere<U>
    {
        Consumable<V> IMergeSelect<U>.MergeSelect<V>(ConsumableForMerging<U> _, Func<U, V> u2v) =>
            new SelectArray<T, V>(Underlying, t => u2v(Selector(t)));

        Consumable<U> IMergeWhere<U>.MergeWhere(ConsumableForMerging<U> consumable, Func<U, bool> predicate) =>
            new Array<T, U>(Underlying, new Links.SelectWhere<T, U>(Selector, predicate));
    }

    sealed partial class SelectList<T, U> : IMergeSelect<U>, IMergeWhere<U>
    {
        Consumable<V> IMergeSelect<U>.MergeSelect<V>(ConsumableForMerging<U> _, Func<U, V> u2v) =>
            new SelectList<T, V>(Underlying, t=> u2v(Selector(t)));

        Consumable<U> IMergeWhere<U>.MergeWhere(ConsumableForMerging<U> consumable, Func<U, bool> predicate) =>
            new List<T, U>(Underlying, new Links.SelectWhere<T, U>(Selector, predicate));
    }

    sealed partial class SelectEnumerable<T, U> : IMergeSelect<U>, IMergeWhere<U>
    {
        Consumable<V> IMergeSelect<U>.MergeSelect<V>(ConsumableForMerging<U> consumable, Func<U, V> u2v) =>
            new SelectEnumerable<T, V>(Underlying, t => u2v(Selector(t)));

        Consumable<U> IMergeWhere<U>.MergeWhere(ConsumableForMerging<U> consumable, Func<U, bool> predicate) =>
            new Enumerable<T, U>(Underlying, new Links.SelectWhere<T, U>(Selector, predicate));
    }
}
