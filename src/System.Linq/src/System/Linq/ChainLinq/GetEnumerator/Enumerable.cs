using System.Collections.Generic;

namespace System.Linq.ChainLinq.GetEnumerator
{
    static partial class Enumerable
    {
        static partial void Optimized<T, U>(IEnumerable<T> enumerable, Link<T, U> link, ref IEnumerator<U> enumerator);

        public static IEnumerator<U> Get<T, U>(IEnumerable<T> enumerable, Link<T, U> link)
        {
            IEnumerator<U> optimized = null;
            Optimized(enumerable, link, ref optimized);
            if (optimized != null)
            {
                return optimized;
            }

            return new ConsumerEnumerators.Enumerable<T, U>(enumerable, link);
        }
    }
}
