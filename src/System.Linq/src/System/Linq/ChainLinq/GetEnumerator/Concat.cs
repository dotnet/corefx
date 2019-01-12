using System.Collections.Generic;

namespace System.Linq.ChainLinq.GetEnumerator
{
    static class Concat
    {
        public static IEnumerator<U> Get<T, U>(IEnumerable<T> firstOrNull, IEnumerable<T> second, IEnumerable<T> third, Link<T, U> link)
        {
            return new ConsumerEnumerators.Concat<T, U>(firstOrNull, second, third, link);
        }
    }
}
