using System.Collections.Generic;

namespace System.Linq.ChainLinq.GetEnumerator
{
    static class Enumerable
    {
        public static IEnumerator<U> Get<T, U>(IEnumerable<T> e, ILink<T, U> link)
        {
            return new ConsumerEnumerators.Enumerable<T, U>(e, link);
        }
    }
}
