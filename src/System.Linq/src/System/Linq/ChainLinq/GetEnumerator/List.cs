using System.Collections.Generic;

namespace System.Linq.ChainLinq.GetEnumerator
{
    static partial class List
    {
        static partial void Optimized<T, U>(Consumables.List<T, U> consumable, ref IEnumerator<U> enumerator);

        public static IEnumerator<U> Get<T, U>(Consumables.List<T, U> consumable)
        {
            IEnumerator<U> optimized = null;
            Optimized(consumable, ref optimized);
            if (optimized != null)
            {
                return optimized;
            }

            return new ConsumerEnumerators.List<T, U>(consumable.Underlying, consumable.Link);
        }
    }
}
