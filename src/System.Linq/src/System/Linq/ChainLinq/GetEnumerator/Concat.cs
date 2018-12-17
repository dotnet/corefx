using System.Collections.Generic;

namespace System.Linq.ChainLinq.GetEnumerator
{
    static class Concat
    {
        public static IEnumerator<U> Get<T, U>(IEnumerable<T> first, IEnumerable<T> second, ILink<T, U> link)
        {
            return new ConsumerEnumerators.Concat<T, U>(first, second, link);
        }
    }
}
