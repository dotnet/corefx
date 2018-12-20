using System.Collections.Generic;

namespace System.Linq.ChainLinq.GetEnumerator
{
    static class List
    {
        public static IEnumerator<U> Get<T, U>(List<T> array, ILink<T, U> link)
        {
            return new ConsumerEnumerators.List<T, U>(array, link);
        }
    }
}
