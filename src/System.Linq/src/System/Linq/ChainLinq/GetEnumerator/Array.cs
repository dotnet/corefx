using System.Collections.Generic;

namespace System.Linq.ChainLinq.GetEnumerator
{
    static partial class Array
    {
        public static IEnumerator<U> Get<T, U>(T[] array, int start, int length, Link<T, U> link)
        {
            return new ConsumerEnumerators.Array<T, U>(array, start, length, link);
        }
    }
}
