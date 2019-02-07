// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace System.Collections
{
    internal static partial class CollectionServices
    {
        public static ReadOnlyCollection<T> ToReadOnlyCollection<T>(this IEnumerable<T> source)
        {
            if(source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            return new ReadOnlyCollection<T>(source.AsArray());
        }

        public static IEnumerable<T> ConcatAllowingNull<T>(this IEnumerable<T> source, IEnumerable<T> second)
        {
            if (second == null || !second.FastAny())
            {
                return source;
            }

            if (source == null || !source.FastAny())
            {
                return second;
            }

            return source.Concat(second);
        }

        public static List<T> FastAppendToListAllowNulls<T>(this List<T> source, IEnumerable<T> second)
        {
            if (second == null)
            {
                return source;
            }

            // if there's nothing in the source, return the second
            if ((source == null) || (source.Count == 0))
            {
                return second as List<T> ?? second.ToList();
            }

            // if the second is List<T>, and contains very few elements there's no need for AddRange
            if (second is List<T> secondAsList)
            {
                if (secondAsList.Count == 0)
                {
                    return source;
                }
                else if (secondAsList.Count == 1)
                {
                    source.Add(secondAsList[0]);
                    return source;
                }
            }

            // last resort - nothing is null, need to append
            source.AddRange(second);
            return source;
        }

        private static List<T> FastAppendToListAllowNulls<T>(this List<T> source, T value)
        {
            if (source == null)
            {
                source = new List<T>();
            }
            source.Add(value);

            return source;
        }

        public static List<T> FastAppendToListAllowNulls<T>(this List<T> source, T value, IEnumerable<T> second)
        {
            if (second == null)
            {
                source = source.FastAppendToListAllowNulls(value);
            }
            else
            {
                source = source.FastAppendToListAllowNulls(second);
            }
            return source;
        }

        public static bool FastAny<T>(this IEnumerable<T> source)
        {
            // Enumerable.Any<T> underneath doesn't cast to ICollection, 
            // like it does with many of the other LINQ methods.
            // Below is significantly (4x) when mainly working with ICollection
            // sources and a little slower if working with mainly IEnumerable<T> 
            // sources.

            // Cast to ICollection instead of ICollection<T> for performance reasons.
            if (source is ICollection collection)
            {
                return collection.Count > 0;
            }

            return source.Any();
        }

        public static T[] AsArray<T>(this IEnumerable<T> enumerable)
        {
            return enumerable as T[] ?? enumerable.ToArray();
        }
    }
}
