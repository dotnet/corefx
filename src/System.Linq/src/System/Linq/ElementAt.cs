// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;

namespace System.Linq
{
    public static partial class Enumerable
    {
        public static TSource ElementAt<TSource>(this IEnumerable<TSource> source, int index)
        {
            TSource result;
            if (TryElementAt(source, index, out result))
            {
                return result;
            }

            throw Error.ArgumentOutOfRange(nameof(index));
        }

        public static TSource ElementAtOrDefault<TSource>(this IEnumerable<TSource> source, int index)
        {
            TSource result;
            TryElementAt(source, index, out result);
            return result;
        }

        public static bool TryElementAt<TSource>(this IEnumerable<TSource> source, int index, out TSource element)
        {
            if (source == null)
            {
                throw Error.ArgumentNull(nameof(source));
            }

            IPartition<TSource> partition = source as IPartition<TSource>;
            if (partition != null)
            {
                bool found;
                element = partition.TryGetElementAt(index, out found);
                return found;
            }

            if (index >= 0)
            {
                IList<TSource> list = source as IList<TSource>;
                if (list != null)
                {
                    if (index < list.Count)
                    {
                        element = list[index];
                        return true;
                    }
                }
                else
                {
                    using (IEnumerator<TSource> e = source.GetEnumerator())
                    {
                        while (e.MoveNext())
                        {
                            if (index == 0)
                            {
                                element = e.Current;
                                return true;
                            }

                            index--;
                        }
                    }
                }
            }

            element = default(TSource);
            return false;
        }
    }
}
