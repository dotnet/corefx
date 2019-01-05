using System.Collections.Generic;

namespace System.Linq.ChainLinq.GetEnumerator
{
    static partial class Lookup
    {
        public static IEnumerator<U> Get<TKey, TElement, U>(Grouping<TKey, TElement> lastGrouping, ILink<IGrouping<TKey, TElement>, U> link)
        {
            return new ConsumerEnumerators.Lookup<TKey, TElement, U>(lastGrouping, link);
        }
    }
}
