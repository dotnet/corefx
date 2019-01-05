using System.Collections.Generic;

namespace System.Linq.ChainLinq.GetEnumerator
{
    static partial class Lookup
    {
        public static IEnumerator<U> Get<TKey, TElement, U>(Grouping<TKey, TElement> lastGrouping, ILink<IGrouping<TKey, TElement>, U> link)
        {
            return new ConsumerEnumerators.Lookup<TKey, TElement, U>(lastGrouping, link);
        }

        public static IEnumerator<U> Get<TKey, TElement, TResult, U>(Grouping<TKey, TElement> lastGrouping, Func<TKey, IEnumerable<TElement>, TResult> resultSelector, ILink<TResult, U> link)
        {
            return new ConsumerEnumerators.Lookup<TKey, TElement, TResult, U>(lastGrouping, resultSelector, link);
        }
    }
}
