// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Linq;

namespace System.Collections.Generic
{
    /// <summary>
    /// Internal helper functions for working with enumerables.
    /// </summary>
    internal static partial class EnumerableHelpers
    {
        /// <summary>
        /// Tries to get the count of the enumerable cheaply.
        /// </summary>
        /// <typeparam name="T">The element type of the source enumerable.</typeparam>
        /// <param name="source">The enumerable to count.</param>
        /// <param name="count">The count of the enumerable, if it could be obtained cheaply.</param>
        /// <returns><c>true</c> if the enumerable could be counted cheaply; otherwise, <c>false</c>.</returns>
        internal static bool TryGetCount<T>(IEnumerable<T> source, out int count)
        {
            Debug.Assert(source != null);

            var collection = source as ICollection<T>;
            if (collection != null)
            {
                count = collection.Count;
                return true;
            }

            var provider = source as IIListProvider<T>;
            if (provider != null)
            {
                count = provider.GetCount(onlyIfCheap: true);
                return count >= 0;
            }

            count = -1;
            return false;
        }

        // TODO: Add XML docs for all of these.
        internal static T TryGetElementAt<T>(int index, out bool found, IEnumerable<T> source)
        {
            Debug.Assert(source != null);
            Debug.Assert(!(source is IList<T>));

            if (index >= 0)
            {
                using (IEnumerator<T> e = source.GetEnumerator())
                {
                    while (e.MoveNext())
                    {
                        if (index == 0)
                        {
                            found = true;
                            return e.Current;
                        }

                        index--;
                    }
                }
            }

            found = false;
            return default(T);
        }

        internal static T TryGetFirst<T>(Func<T, bool> predicate, out bool found, IEnumerable<T> source)
        {
            Debug.Assert(predicate != null);
            Debug.Assert(source != null);
            Debug.Assert(!(source is IList<T>));

            foreach (T item in source)
            {
                if (predicate(item))
                {
                    found = true;
                    return item;
                }
            }

            found = false;
            return default(T);
        }

        internal static T TryGetFirst<T>(Func<T, bool> predicate, out bool found, T[] array)
        {
            Debug.Assert(predicate != null);
            Debug.Assert(array != null);

            foreach (T item in array)
            {
                if (predicate(item))
                {
                    found = true;
                    return item;
                }
            }

            found = false;
            return default(T);
        }

        internal static T TryGetFirst<T>(Func<T, bool> predicate, out bool found, List<T> list)
        {
            Debug.Assert(predicate != null);
            Debug.Assert(list != null);

            for (int i = 0; i < list.Count; i++)
            {
                T item = list[i];
                if (predicate(item))
                {
                    found = true;
                    return item;
                }
            }

            found = false;
            return default(T);
        }

        internal static T TryGetFirst<T>(Func<T, bool> predicate, out bool found, IList<T> ilist)
        {
            Debug.Assert(predicate != null);
            Debug.Assert(ilist != null);
            Debug.Assert(!(ilist is T[] || ilist is List<T>));

            int count = ilist.Count;
            for (int i = 0; i < count; i++)
            {
                T item = ilist[i];
                if (predicate(item))
                {
                    found = true;
                    return item;
                }
            }

            found = false;
            return default(T);
        }

        internal static T TryGetLast<T>(Func<T, bool> predicate, out bool found, IEnumerable<T> source)
        {
            Debug.Assert(predicate != null);
            Debug.Assert(source != null);
            Debug.Assert(!(source is IList<T>));

            using (IEnumerator<T> e = source.GetEnumerator())
            {
                while (e.MoveNext())
                {
                    T result = e.Current;
                    if (predicate(result))
                    {
                        while (e.MoveNext())
                        {
                            T item = e.Current;
                            if (predicate(item))
                            {
                                result = item;
                            }
                        }

                        found = true;
                        return result;
                    }
                }
            }

            found = false;
            return default(T);
        }

        internal static T TryGetLast<T>(Func<T, bool> predicate, out bool found, T[] array)
        {
            Debug.Assert(predicate != null);
            Debug.Assert(array != null);

            for (int i = array.Length - 1; i >= 0; i--)
            {
                T item = array[i];
                if (predicate(item))
                {
                    found = true;
                    return item;
                }
            }

            found = false;
            return default(T);
        }

        internal static T TryGetLast<T>(Func<T, bool> predicate, out bool found, List<T> list)
        {
            Debug.Assert(predicate != null);
            Debug.Assert(list != null);

            for (int i = list.Count - 1; i >= 0; i--)
            {
                T item = list[i];
                if (predicate(item))
                {
                    found = true;
                    return item;
                }
            }

            found = false;
            return default(T);
        }

        internal static T TryGetLast<T>(Func<T, bool> predicate, out bool found, IList<T> ilist)
        {
            Debug.Assert(predicate != null);
            Debug.Assert(ilist != null);
            Debug.Assert(!(ilist is T[] || ilist is List<T>));

            for (int i = ilist.Count - 1; i >= 0; i--)
            {
                T item = ilist[i];
                if (predicate(item))
                {
                    found = true;
                    return item;
                }
            }

            found = false;
            return default(T);
        }
    }
}
