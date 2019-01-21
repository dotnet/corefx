using System.Collections.Generic;

namespace System.Linq.ChainLinq.GetEnumerator
{
    static partial class List
    {
        public static IEnumerator<U> Get<T, U>(Consumables.List<T, U> consumable)
        {
            return new ConsumerEnumerators.List<T, U>(consumable.Underlying, consumable.Link);
        }
    }
}
