using System.Collections.Generic;

namespace System.Linq.ChainLinq.GetEnumerator
{
    static class SelectMany
    {
        public static IEnumerator<V> Get<T, V>(Consumable<IEnumerable<T>> selectMany, Link<T, V> link)
        {
            return new ConsumerEnumerators.SelectMany<T, V>(selectMany, link);
        }

        public static IEnumerator<V> Get<TSource, TCollection, T, V>(Consumable<(TSource, IEnumerable<TCollection>)> selectMany, Func<TSource, TCollection, T> resultSelector, Link<T, V> link)
        {
            return new ConsumerEnumerators.SelectMany<TSource, TCollection, T, V>(selectMany, resultSelector, link);
        }
    }
}
