using System.Collections.Generic;

namespace System.Linq.ChainLinq
{
    static class Utils
    {
        internal static Consumable<U> CreateConsumable<T, U>(IEnumerable<T> e, Link<T, U> transform)
        {
            if (e is T[] array)
            {
                return
                    array.Length == 0
                      ? Consumables.Empty<U>.Instance
                      : new Consumables.Array<T, U>(array, transform);
            }
            else if (e is List<T> list)
            {
                return new Consumables.List<T, U>(list, transform);
            }
            else if (e is Consumables.IConsumableProvider<T> provider)
            {
                return provider.GetConsumable(transform);
            }
            /*
             * I don't think we should use IList in the general case?
             * 
                        else if (e is IList<T> ilist)
                        {
                            return new Consumables.IList<T, U>(ilist, 0, ilist.Count, transform);
                        }
            */
            else
            {
                return new Consumables.Enumerable<T, U>(e, transform);
            }
        }

        internal static Consumable<T> AsConsumable<T>(IEnumerable<T> e)
        {
            if (e is Consumable<T> c)
            {
                return c;
            }
            else
            {
                return CreateConsumable(e, Links.Identity<T>.Instance);
            }
        }

        // TTTransform is faster tahn TUTransform as AddTail version call can avoid
        // expensive JIT generic interface call
        internal static Consumable<T> PushTTTransform<T>(IEnumerable<T> e, Link<T, T> transform)
        {
            if (e is ConsumableForAddition<T> consumable)
            {
                return consumable.AddTail(transform);
            }
            else
            {
                return CreateConsumable(e, transform);
            }
        }

        // TUTrasform is more flexible but slower than TTTransform
        internal static Consumable<U> PushTUTransform<T, U>(IEnumerable<T> e, Link<T, U> transform)
        {
            if (e is ConsumableForAddition<T> consumable)
            {
                return consumable.AddTail(transform);
            }
            else
            {
                return CreateConsumable(e, transform);
            }
        }

        internal static Result Consume<T, Result>(IEnumerable<T> e, Consumer<T, Result> consumer)
        {
            if (e is T[] array)
            {
                ChainLinq.Consume.Array.Invoke(array, Links.Identity<T>.Instance, consumer);
            }
            else if (e is List<T> list)
            {
                ChainLinq.Consume.List.Invoke(list, Links.Identity<T>.Instance, consumer);
            }
            else if (e is Consumables.IConsumableProvider<T> provider)
            {
                var c = provider.GetConsumable(Links.Identity<T>.Instance);
                c.Consume(consumer);
            }
            else
            {
                ChainLinq.Consume.Enumerable.Invoke(e, Links.Identity<T>.Instance, consumer);
            }

            return consumer.Result;
        }
    }
}
