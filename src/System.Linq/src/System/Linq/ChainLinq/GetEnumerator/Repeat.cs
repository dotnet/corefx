using System.Collections.Generic;

namespace System.Linq.ChainLinq.GetEnumerator
{
    static class Repeat
    {
        public static IEnumerator<U> Get<T, U>(T element, int count, ILink<T, U> link)
        {
            return new ConsumerEnumerators.Repeat<T, U>(element, count, link);
        }
    }
}
