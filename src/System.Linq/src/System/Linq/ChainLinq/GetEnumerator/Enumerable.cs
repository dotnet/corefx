using System.Collections.Generic;

namespace System.Linq.ChainLinq.GetEnumerator
{
    static partial class Enumerable
    {
        static partial void Optimized<T, U>(Consumables.Enumerable<T, U> consumable, ref IEnumerator<U> enumerator);

        public static IEnumerator<U> Get<T, U>(Consumables.Enumerable<T, U> consumable)
        {
            IEnumerator<U> optimized = null;
            Optimized(consumable, ref optimized);
            if (optimized != null)
            {
                return optimized;
            }

            return new ConsumerEnumerators.Enumerable<T, U>(consumable.Underlying, consumable.Link);
        }
    }
}
