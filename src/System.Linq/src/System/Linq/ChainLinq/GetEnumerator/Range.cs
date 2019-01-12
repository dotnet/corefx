using System.Collections.Generic;

namespace System.Linq.ChainLinq.GetEnumerator
{
    static class Range
    {
        public static IEnumerator<U> Get<U>(int start, int count, Link<int, U> link)
        {
            return new ConsumerEnumerators.Range<U>(start, count, link);
        }
    }
}
