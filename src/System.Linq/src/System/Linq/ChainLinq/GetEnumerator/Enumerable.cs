using System.Collections.Generic;

namespace System.Linq.ChainLinq.GetEnumerator
{
    class Enumerable
    {
        public static IEnumerator<U> Get<T, U>(IEnumerable<T> e, ILink<T, U> composition)
        {
            return new Enumerators.ConsumerEnumeratorEnumerable<T, U>(e, composition);
        }
    }
}
