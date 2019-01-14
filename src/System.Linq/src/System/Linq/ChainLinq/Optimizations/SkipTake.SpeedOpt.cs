using System.Collections.Generic;

namespace System.Linq.ChainLinq.Optimizations
{
    static class SkipTake
    {
        public static V Last<T, V>(Consumables.Base_Generic_Arguments_Reversed_To_Work_Around_XUnit_Bug<V, T> c, IList<T> list, int start, int count, bool orDefault)
        {
            if (c.Link is ISkipTakeOnConsumableLinkUpdate<T, V> skipLink)
            {
                var skipped = Skip(c, list, start, count, count - 1);
                var skippedLast = new Consumer.Last<V>(orDefault);
                skipped.Consume(skippedLast);
                return skippedLast.Result;;
            }

            var last = new Consumer.Last<V>(orDefault);
            c.Consume(last);
            return last.Result;
        }

        public static Consumable<V> Skip<T, V>(Consumables.Base_Generic_Arguments_Reversed_To_Work_Around_XUnit_Bug<V, T> c, IList<T> list, int start, int count, int toSkip)
        {
            if (toSkip <= 0)
                return c;

            if (c.Link is ISkipTakeOnConsumableLinkUpdate<T, V> skipLink)
            {
                checked
                {
                    var newCount = count - toSkip;
                    if (newCount <= 0)
                    {
                        return Consumables.Empty<V>.Instance;
                    }

                    var newStart = start + toSkip;
                    var newLink = skipLink.Skip(toSkip);

                    return new Consumables.IList<T, V>(list, newStart, newCount, newLink);
                }
            }
            return c.AddTail(new Links.Skip<V>(toSkip));
        }

        public static Consumable<V> Take<T, V>(Consumables.Base_Generic_Arguments_Reversed_To_Work_Around_XUnit_Bug<V, T> c, IList<T> list, int start, int count, int toTake)
        {
            if (toTake <= 0)
            {
                return Consumables.Empty<V>.Instance;
            }

            if (toTake >= count)
            {
                return c;
            }

            if (c.Link is ISkipTakeOnConsumableLinkUpdate<T, V>)
            {
                return new Consumables.IList<T, V>(list, start, toTake, c.Link);
            }

            return c.AddTail(new Links.Take<V>(toTake));
        }

    }
}
