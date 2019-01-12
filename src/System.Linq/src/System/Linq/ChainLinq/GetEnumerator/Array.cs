using System.Collections.Generic;

namespace System.Linq.ChainLinq.GetEnumerator
{
    static partial class Array
    {
        static partial void Optimized<T, U>(T[] array, Link<T, U> link, ref IEnumerator<U> enumerator);

        public static IEnumerator<U> Get<T, U>(T[] array, Link<T, U> link)
        {
            IEnumerator<U> optimized = null;
            Optimized(array, link, ref optimized);
            if (optimized != null)
            {
                return optimized;
            }

            return new ConsumerEnumerators.Array<T, U>(array, link);
        }
    }
}
