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
            switch (source)
            {
                case null:
                    throw Error.ArgumentNull(nameof(source));

                case IPartition<TSource> partition:
                    TSource element = partition.TryGetElementAt(index, out bool found);
                    if (found)
                    {
                        return element;
                    }

                    break;
                case IList<TSource> list:
                    return list[index];

                default:
                    if (index >= 0)
                    {
                        using (IEnumerator<TSource> e = source.GetEnumerator())
                        {
                            while (e.MoveNext())
                            {
                                if (index == 0)
                                {
                                    return e.Current;
                                }

                                index--;
                            }
                        }
                    }

                    break;
            }

            throw Error.ArgumentOutOfRange(nameof(index));
        }

        public static TSource ElementAtOrDefault<TSource>(this IEnumerable<TSource> source, int index)
        {
            switch (source)
            {
                case null:
                    throw Error.ArgumentNull(nameof(source));

                case IPartition<TSource> partition:
                    bool found;
                    return partition.TryGetElementAt(index, out found);
                case IList<TSource> list:
                    if (index >= 0 && index < list.Count)
                    {
                        return list[index];
                    }

                    break;
                default:
                    if (index >= 0)
                    {
                        using (IEnumerator<TSource> e = source.GetEnumerator())
                        {
                            while (e.MoveNext())
                            {
                                if (index == 0)
                                {
                                    return e.Current;
                                }

                                index--;
                            }
                        }
                    }

                    break;
            }

            return default(TSource);
        }
    }
}
