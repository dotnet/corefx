using System.Collections;
using System.Collections.Generic;

namespace System.Linq.ChainLinq.Consumables
{
    sealed partial class Concat<T, V>
        : Optimizations.ICountOnConsumable
    {
        private static int GetCount(IEnumerable<T> e, bool onlyIfCheap)
        {
            if (e == null)
                return 0;

            if (e is ICollection<T> ct)
            {
                return ct.Count;
            }
            else if (e is Optimizations.ICountOnConsumable cc)
            {
                return cc.GetCount(onlyIfCheap);
            }
            else if (e is ICollection c)
            {
                return c.Count;
            }
            else
            { 
                return -1;
            }
        }

        public int GetCount(bool onlyIfCheap)
        {
            if (Link is Optimizations.ICountOnConsumableLink countLink)
            {
                checked
                {
                    int count = 0, tmp = 0;

                    tmp = GetCount(_firstOrNull, onlyIfCheap);
                    if (tmp >= 0)
                    {
                        count += tmp;
                        tmp = GetCount(_second, onlyIfCheap);
                        if (tmp >= 0)
                        {
                            count += tmp;
                            tmp = GetCount(_thirdOrNull, onlyIfCheap);
                            if (tmp >= 0)
                            {
                                count += tmp;
                                count = countLink.GetCount(count);
                                if (count >= 0)
                                    return count;
                            }
                        }
                    }
                }
            }

            return onlyIfCheap ? -1 : Consume(new Consumer.Count<V>());
        }
    }
}
