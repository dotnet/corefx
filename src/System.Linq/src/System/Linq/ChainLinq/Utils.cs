using System.Collections.Generic;

namespace System.Linq.ChainLinq
{
    static class Utils
    {
        internal static Consumable<U> CreateConsumable<T, U>(IEnumerable<T> e, ILink<T, U> transform)
        {
            switch (e)
            {
                case T[] array:
                    return 
                        array.Length == 0
                          ? Consumables.Empty<U>.Instance
                          : new Consumables.Array<T, U>(array, transform);

                case List<T> list:
                    return new Consumables.List<T, U>(list, transform);

                default:
                    return new Consumables.Enumerable<T, U>(e, transform);
            }
        }

        internal static Consumable<U> PushTransform<T, U>(IEnumerable<T> e, ILink<T, U> transform)
        {
            switch (e)
            {
                case ConsumableForAddition<T> consumable:
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
