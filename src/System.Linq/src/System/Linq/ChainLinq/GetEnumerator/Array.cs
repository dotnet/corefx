using System.Collections.Generic;

namespace System.Linq.ChainLinq.GetEnumerator
{
    static partial class Array
    {
        public static IEnumerator<U> Get<T, U>(Consumables.Array<T, U> consumable)
        {
            return new ConsumerEnumerators.Array<T, U>(consumable.Underlying, consumable.Link);
        }
    }
}
