using System.Linq.ChainLinq.Optimizations;

namespace System.Linq.ChainLinq.Consumables
{
    sealed partial class SelectArray<T, U> : IMergeSelect<U>, IMergeWhere<U>
    {
        Consumable<V> IMergeSelect<U>.MergeSelect<V>(ConsumableForMerging<U> _, Func<U, V> u2v) =>
            new SelectArray<T, V>(Underlying, t => u2v(Selector(t)));

        Consumable<U> IMergeWhere<U>.MergeWhere(ConsumableForMerging<U> _, Func<U, bool> predicate) =>
            new SelectWhereArray<T, U>(Underlying, Selector, predicate);
    }

    sealed partial class SelectList<T, U> : IMergeSelect<U>, IMergeWhere<U>
    {
        Consumable<V> IMergeSelect<U>.MergeSelect<V>(ConsumableForMerging<U> _, Func<U, V> u2v) =>
            new SelectList<T, V>(Underlying, t => u2v(Selector(t)));

        Consumable<U> IMergeWhere<U>.MergeWhere(ConsumableForMerging<U> _, Func<U, bool> predicate) =>
            new SelectWhereList<T, U>(Underlying, Selector, predicate);
    }

    sealed partial class SelectEnumerable<T, U> : IMergeSelect<U>, IMergeWhere<U>
    {
        Consumable<V> IMergeSelect<U>.MergeSelect<V>(ConsumableForMerging<U> _, Func<U, V> u2v) =>
            new SelectEnumerable<T, V>(Underlying, t => u2v(Selector(t)));

        Consumable<U> IMergeWhere<U>.MergeWhere(ConsumableForMerging<U> _, Func<U, bool> predicate) =>
            new SelectWhereEnumerable<T, U>(Underlying, Selector, predicate);
    }
}
