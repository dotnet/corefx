using System.Collections.Generic;
using System.Linq.ChainLinq.Optimizations;

namespace System.Linq.ChainLinq.Consumables
{
    sealed partial class WhereArray<T> : IMergeSelect<T>, IMergeWhere<T>
    {
        Consumable<V> IMergeSelect<T>.MergeSelect<V>(ConsumableForMerging<T> _, Func<T, V> u2v) =>
            new Array<T, V>(Underlying, new Links.WhereSelect<T, V>(Predicate, u2v));

        Consumable<T> IMergeWhere<T>.MergeWhere(ConsumableForMerging<T> _, Func<T, bool> predicate) =>
            new WhereArray<T>(Underlying, t => Predicate(t) && predicate(t));
    }

    sealed partial class WhereList<T> : IMergeSelect<T>, IMergeWhere<T>
    {
        Consumable<V> IMergeSelect<T>.MergeSelect<V>(ConsumableForMerging<T> _, Func<T, V> u2v) =>
            new List<T, V>(Underlying, new Links.WhereSelect<T, V>(Predicate, u2v));

        Consumable<T> IMergeWhere<T>.MergeWhere(ConsumableForMerging<T> consumable, Func<T, bool> predicate) =>
            new WhereList<T>(Underlying, t => Predicate(t) && predicate(t));
    }

    sealed partial class WhereEnumerable<T> : IMergeSelect<T>, IMergeWhere<T>
    {
        Consumable<V> IMergeSelect<T>.MergeSelect<V>(ConsumableForMerging<T> consumable, Func<T, V> u2v) =>
            new Enumerable<T, V>(Underlying, new Links.WhereSelect<T,V>(Predicate, u2v));

        Consumable<T> IMergeWhere<T>.MergeWhere(ConsumableForMerging<T> consumable, Func<T, bool> predicate) =>
            new WhereEnumerable<T>(Underlying, t => Predicate(t) && predicate(t));
    }
}
