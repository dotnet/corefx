// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Runtime.CompilerServices;

#if !netstandard
using Internal.Runtime.CompilerServices;
#endif

namespace System
{
    internal static partial class SpanHelpers
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void Sort<T, TComparer>(
            this Span<T> span, TComparer comparer)
            where TComparer : IComparer<T>
        {
            if (comparer == null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.comparer);

            Sort(ref span.DangerousGetPinnableReference(), span.Length, comparer);
        }

        internal static void Sort<T, TComparer>(
            ref T spanStart, int length, TComparer comparer) 
            where TComparer : IComparer<T>
        {
            int lo = 0;
            int hi = length - 1;
            // If length == 0, hi == -1, and loop will not be entered
            while (lo <= hi)
            {
                // PERF: `lo` or `hi` will never be negative inside the loop,
                //       so computing median using uints is safe since we know 
                //       `length <= int.MaxValue`, and indices are >= 0
                //       and thus cannot overflow an uint. 
                //       Saves one subtraction per loop compared to 
                //       `int i = lo + ((hi - lo) >> 1);`
                int i = (int)(((uint)hi + (uint)lo) >> 1);

                // TODO: We probably need to add `ref readonly`/`in` methods e.g. `AddReadOnly` to unsafe
                //int c = comparable.CompareTo(Unsafe.Add(ref spanStart, i));
                //if (c == 0)
                //{
                //    return i;
                //}
                //else if (c > 0)
                //{
                //    lo = i + 1;
                //}
                //else
                //{
                //    hi = i - 1;
                //}
            }
            // If none found, then a negative number that is the bitwise complement
            // of the index of the next element that is larger than or, if there is
            // no larger element, the bitwise complement of `length`, which
            // is `lo` at this point.
            //return ~lo;
        }

        // Helper to allow sharing all code via IComparer<T> inlineable
        internal struct ComparableComparer<T> : IComparer<T>
            where T : IComparable<T>
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public int Compare(T x, T y) => x.CompareTo(y);
        }
        // Helper to allow sharing all code via IComparer<T> inlineable
        internal struct ComparisonComparer<T> : IComparer<T>
        {
            readonly Comparison<T> m_comparison;

            public ComparisonComparer(Comparison<T> comparison)
            {
                m_comparison = comparison;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public int Compare(T x, T y) => m_comparison(x, y);
        }
    }
}
