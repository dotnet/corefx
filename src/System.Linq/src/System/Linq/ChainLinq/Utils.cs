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

        internal static Consumable<TSource> Where<TSource>(IEnumerable<TSource> source, Func<TSource, bool> predicate)
        {
            if (source is ConsumableForMerging<TSource> consumable)
            {
                if (consumable.TailLink is Optimizations.IMergeWhere<TSource> optimization)
                {
                    return optimization.MergeWhere(consumable, predicate);
                }

                return consumable.AddTail(new Links.Where<TSource>(predicate));
            }
            else if (source is TSource[] array)
            {
                return new Consumables.WhereArray<TSource>(array, predicate);
            }
            else if (source is List<TSource> list)
            {
                return new Consumables.WhereList<TSource>(list, predicate);
            }
            else
            {
                return new Consumables.WhereEnumerable<TSource>(source, predicate);
            }
        }

        internal static Consumable<TResult> Select<TSource, TResult>(IEnumerable<TSource> source, Func<TSource, TResult> selector)
        {
            if (source is ConsumableForMerging<TSource> consumable)
            {
                if (consumable.TailLink is Optimizations.IMergeSelect<TSource> optimization)
                {
                    return optimization.MergeSelect(consumable, selector);
                }

                return consumable.AddTail(new Links.Select<TSource, TResult>(selector));
            }
            else if (source is TSource[] array)
            {
                return new Consumables.SelectArray<TSource, TResult>(array, selector);
            }
            else if (source is List<TSource> list)
            {
                return new Consumables.SelectList<TSource, TResult>(list, selector);
            }
            else
            {
                return new Consumables.SelectEnumerable<TSource, TResult>(source, selector);
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
            if (e is Consumable<T> consumable)
            {
                consumable.Consume(consumer);
            }
            else if (e is T[] array)
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
