// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Diagnostics;

namespace System.Linq
{
    public static partial class Enumerable
    {
        private static IEnumerable<TSource> TakeIterator<TSource>(IEnumerable<TSource> source, int count)
        {
            Debug.Assert(count > 0);
            
            foreach (TSource element in source)
            {
                yield return element;
                if (--count == 0) break;
            }
        }

        private static IEnumerable<TSource> TakeLastIterator<TSource>(IEnumerable<TSource> source, int count)
        {
            Debug.Assert(count > 0);

            if (source is IList<TSource> sourceList)
            {
                if (sourceList.Count > count)
                {
                    return sourceList.Skip(sourceList.Count - count);
                }
                else if (sourceList.Count > 0)
                {
                    return TakeIterator<TSource>(sourceList, sourceList.Count);
                }
                else
                {
                    return Empty<TSource>();
                }
            }

            return TakeLastRegularIterator<TSource>(source, count);
        }
    }
}
