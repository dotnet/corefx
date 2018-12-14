using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Linq.ChainLinq
{
    class Utils
    {
        internal static Consumable<U> CreateConsumable<T, U>(IEnumerable<T> e, ILink<T, U> transform)
        {
            return new Consumables.Enumerable<T, U>(e, transform);
        }

        internal static Consumable<U> PushTransform<T, U>(IEnumerable<T> e, ILink<T, U> transform)
        {
            switch (e)
            {
                case Consumable<T> consumable:
                    return consumable.AddTail(transform);

                default:
                    return CreateConsumable(e, transform);
            }
        }

        internal static Result Consume<T, Result>(IEnumerable<T> e, Consumer<T, Result> consumer)
        {
            switch (e)
            {
                case Consumable<T> consumable:
                    return consumable.Consume(consumer);

                default:
                    return CreateConsumable(e, Links.Identity<T>.Instance).Consume(consumer);
            }
        }
    }
}
