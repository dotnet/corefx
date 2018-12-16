using System.Collections.Generic;

namespace System.Linq.ChainLinq.GetEnumerator
{
    static class Array
    {
        public static IEnumerator<U> Get<T, U>(T[] array, ILink<T, U> link)
        {
            return new ConsumerEnumerators.Array<T, U>(array, link);
        }
    }
}
