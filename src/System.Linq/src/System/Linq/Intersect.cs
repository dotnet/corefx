﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;

namespace System.Linq
{
    public static partial class Enumerable
    {
        public static IEnumerable<TSource> Intersect<TSource>(this IEnumerable<TSource> first, IEnumerable<TSource> second)
        {
            if (first == null)
            {
                throw Error.ArgumentNull(nameof(first));
            }

            if (second == null)
            {
                throw Error.ArgumentNull(nameof(second));
            }
            
            return IntersectIteratorNoComparer(first, second);          
        }
        
        private static IEnumerable<TSource> IntersectIteratorNoComparer<TSource>(IEnumerable<TSource> first, IEnumerable<TSource> second)
        {
            IEnumerable<TSource> first2 = first.Distinct();
            IEnumerable<TSource> second2 = second.Distinct();
            
            foreach (TSource element in first2)
            {
                if (second2.Contains(element))
                {
                    yield return element;
                }
            }            
        }
        
        public static IEnumerable<TSource> Intersect<TSource>(this IEnumerable<TSource> first, IEnumerable<TSource> second, IEqualityComparer<TSource> comparer)
        {
            if (first == null)
            {
                throw Error.ArgumentNull(nameof(first));
            }

            if (second == null)
            {
                throw Error.ArgumentNull(nameof(second));
            }

            return IntersectIterator(first, second, comparer);
        }

        private static IEnumerable<TSource> IntersectIterator<TSource>(IEnumerable<TSource> first, IEnumerable<TSource> second, IEqualityComparer<TSource> comparer)
        {
            Set<TSource> set = new Set<TSource>(comparer);
            foreach (TSource element in second)
            {
                set.Add(element);
            }

            foreach (TSource element in first)
            {
                if (set.Remove(element))
                {
                    yield return element;
                }
            }
        }
    }
}
