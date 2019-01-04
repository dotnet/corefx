using System.Collections.Generic;

namespace System.Linq.ChainLinq.GetEnumerator
{
    static partial class IList
    {
        static partial void Optimized<T, U>(IList<T> list, int start, int count, ILink<T, U> link, ref IEnumerator<U> enumerator);

        public static IEnumerator<U> Get<T, U>(IList<T> list, int start, int count, ILink<T, U> link)
        {
            IEnumerator<U> optimized = null;
            Optimized(list, start, count, link, ref optimized);
            if (optimized != null)
            {
                return optimized;
            }

            return new ConsumerEnumerators.IList<T, U>(list, start, count, link);
        }
    }
}
