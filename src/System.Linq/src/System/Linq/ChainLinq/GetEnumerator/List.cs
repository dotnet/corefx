using System.Collections.Generic;

namespace System.Linq.ChainLinq.GetEnumerator
{
    static partial class List
    {
        static partial void Optimized<T, U>(List<T> list, Link<T, U> link, ref IEnumerator<U> enumerator);

        public static IEnumerator<U> Get<T, U>(List<T> list, Link<T, U> link)
        {
            IEnumerator<U> optimized = null;
            Optimized(list, link, ref optimized);
            if (optimized != null)
            {
                return optimized;
            }

            return new ConsumerEnumerators.List<T, U>(list, link);
        }
    }
}
