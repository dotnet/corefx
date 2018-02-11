// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Runtime.CompilerServices;

#if !netstandard
using Internal.Runtime.CompilerServices;
#endif

using static System.SpanSortHelpersCommon;

namespace System
{
    internal static partial class SpanSortHelpersKeysValues
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void Sort<TKey, TValue, TComparer>(
            ref TKey keys, ref TValue values, int length,
            TComparer comparer)
            where TComparer : IDirectComparer<TKey>
        {
            IntrospectiveSort(ref keys, ref values, length, comparer);
        }

        private static void IntrospectiveSort<TKey, TValue, TComparer>(
            ref TKey keys, ref TValue values, int length,
            TComparer comparer)
            where TComparer : IDirectComparer<TKey>
        {
            var depthLimit = 2 * FloorLog2PlusOne(length);
            IntroSort(ref keys, ref values, 0, length - 1, depthLimit, comparer);
        }

        private static void IntroSort<TKey, TValue, TComparer>(
            ref TKey keys, ref TValue values,
            int lo, int hi, int depthLimit,
            TComparer comparer)
            where TComparer : IDirectComparer<TKey>
        {
            Debug.Assert(comparer != null);
            Debug.Assert(lo >= 0);

            while (hi > lo)
            {
                int partitionSize = hi - lo + 1;
                if (partitionSize <= IntrosortSizeThreshold)
                {
                    if (partitionSize == 1)
                    {
                        return;
                    }
                    if (partitionSize == 2)
                    {
                        Sort2(ref keys, ref values, lo, hi, comparer);
                        return;
                    }
                    if (partitionSize == 3)
                    {
                        Sort3(ref keys, ref values, lo, hi - 1, hi, comparer);
                        return;
                    }
                    InsertionSort(ref keys, ref values, lo, hi, comparer);
                    return;
                }

                if (depthLimit == 0)
                {
                    HeapSort(ref keys, ref values, lo, hi, comparer);
                    return;
                }
                depthLimit--;

                // We should never reach here, unless > 3 elements due to partition size
                int p = PickPivotAndPartition(ref keys, ref values, lo, hi, comparer);
                // Note we've already partitioned around the pivot and do not have to move the pivot again.
                IntroSort(ref keys, ref values, p + 1, hi, depthLimit, comparer);
                hi = p - 1;
            }
        }

        private static int PickPivotAndPartition<TKey, TValue, TComparer>(
            ref TKey keys, ref TValue values, int lo, int hi,
            TComparer comparer)
            where TComparer : IDirectComparer<TKey>
        {
            Debug.Assert(comparer != null);
            Debug.Assert(lo >= 0);
            Debug.Assert(hi > lo);

            // Compute median-of-three.  But also partition them, since we've done the comparison.

            // PERF: `lo` or `hi` will never be negative inside the loop,
            //       so computing median using uints is safe since we know 
            //       `length <= int.MaxValue`, and indices are >= 0
            //       and thus cannot overflow an uint. 
            //       Saves one subtraction per loop compared to 
            //       `int middle = lo + ((hi - lo) >> 1);`
            int middle = (int)(((uint)hi + (uint)lo) >> 1);

            // Sort lo, mid and hi appropriately, then pick mid as the pivot.
            ref TKey keysAtMiddle = ref Sort3(ref keys, ref values, lo, middle, hi, comparer);

            TKey pivot = keysAtMiddle;

            int left = lo;
            int right = hi - 1;
            // We already partitioned lo and hi and put the pivot in hi - 1.  
            // And we pre-increment & decrement below.
            Swap(ref keysAtMiddle, ref Unsafe.Add(ref keys, right));
            Swap(ref values, middle, right);

            while (left < right)
            {
                // TODO: Would be good to be able to update local ref here

                // TODO: For primitives and internal comparers the range checks can be eliminated

                while (left < (hi - 1) && comparer.LessThan(Unsafe.Add(ref keys, ++left), pivot)) ;
                // Check if bad comparable/comparer
                if (left == (hi - 1) && comparer.LessThan(Unsafe.Add(ref keys, left), pivot))
                    ThrowHelper.ThrowArgumentException_BadComparer(comparer);

                while (right > lo && comparer.LessThan(pivot, Unsafe.Add(ref keys, --right))) ;
                // Check if bad comparable/comparer
                if (right == lo && comparer.LessThan(pivot, Unsafe.Add(ref keys, right)))
                    ThrowHelper.ThrowArgumentException_BadComparer(comparer);

                if (left >= right)
                    break;

                Swap(ref keys, left, right);
                Swap(ref values, left, right);
            }
            // Put pivot in the right location.
            right = hi - 1;
            if (left != right)
            {
                Swap(ref keys, left, right);
                Swap(ref values, left, right);
            }
            return left;
        }

        private static void HeapSort<TKey, TValue, TComparer>(
            ref TKey keys, ref TValue values, int lo, int hi,
            TComparer comparer)
            where TComparer : IDirectComparer<TKey>
        {
            Debug.Assert(comparer != null);
            Debug.Assert(lo >= 0);
            Debug.Assert(hi > lo);

            int n = hi - lo + 1;
            for (int i = n / 2; i >= 1; --i)
            {
                DownHeap(ref keys, ref values, i, n, lo, comparer);
            }
            for (int i = n; i > 1; --i)
            {
                Swap(ref keys, lo, lo + i - 1);
                Swap(ref values, lo, lo + i - 1);
                DownHeap(ref keys, ref values, 1, i - 1, lo, comparer);
            }
        }

        private static void DownHeap<TKey, TValue, TComparer>(
            ref TKey keys, ref TValue values, int i, int n, int lo,
            TComparer comparer)
            where TComparer : IDirectComparer<TKey>
        {
            Debug.Assert(comparer != null);
            Debug.Assert(lo >= 0);

            //TKey d = keys[lo + i - 1];
            ref TKey keysAtLo = ref Unsafe.Add(ref keys, lo);
            ref TKey keysAtLoMinus1 = ref Unsafe.Add(ref keysAtLo, -1); // No Subtract??

            ref TValue valuesAtLoMinus1 = ref Unsafe.Add(ref values, lo - 1);

            TKey d = Unsafe.Add(ref keysAtLoMinus1, i);
            TValue dValue = Unsafe.Add(ref valuesAtLoMinus1, i);

            var nHalf = n / 2;
            while (i <= nHalf)
            {
                int child = i << 1;

                //if (child < n && comparer(keys[lo + child - 1], keys[lo + child]) < 0)
                if (child < n &&
                    comparer.LessThan(Unsafe.Add(ref keysAtLoMinus1, child), Unsafe.Add(ref keysAtLo, child)))
                {
                    ++child;
                }

                //if (!(comparer(d, keys[lo + child - 1]) < 0))
                if (!(comparer.LessThan(d, Unsafe.Add(ref keysAtLoMinus1, child))))
                    break;

                // keys[lo + i - 1] = keys[lo + child - 1]
                Unsafe.Add(ref keysAtLoMinus1, i) = Unsafe.Add(ref keysAtLoMinus1, child);
                Unsafe.Add(ref valuesAtLoMinus1, i) = Unsafe.Add(ref valuesAtLoMinus1, child);

                i = child;
            }
            //keys[lo + i - 1] = d;
            Unsafe.Add(ref keysAtLoMinus1, i) = d;
            Unsafe.Add(ref valuesAtLoMinus1, i) = dValue;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void InsertionSort<TKey, TValue, TComparer>(
            ref TKey keys, ref TValue values, int lo, int hi,
            TComparer comparer)
            where TComparer : IDirectComparer<TKey>
        {
            Debug.Assert(lo >= 0);
            Debug.Assert(hi >= lo);

            for (int i = lo; i < hi; ++i)
            {
                int j = i;
                //t = keys[i + 1];
                var t = Unsafe.Add(ref keys, j + 1);
                // TODO: Would be good to be able to update local ref here
                if (j >= lo && comparer.LessThan(t, Unsafe.Add(ref keys, j)))
                {
                    var v = Unsafe.Add(ref values, j + 1);
                    do
                    {
                        Unsafe.Add(ref keys, j + 1) = Unsafe.Add(ref keys, j);
                        Unsafe.Add(ref values, j + 1) = Unsafe.Add(ref values, j);
                        --j;
                    }
                    while (j >= lo && comparer.LessThan(t, Unsafe.Add(ref keys, j)));

                    Unsafe.Add(ref keys, j + 1) = t;
                    Unsafe.Add(ref values, j + 1) = v;
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static ref TKey Sort3<TKey, TValue, TComparer>(
            ref TKey keys, ref TValue values, int i0, int i1, int i2,
            TComparer comparer)
            where TComparer : IDirectComparer<TKey>
        {
            ref var r0 = ref Unsafe.Add(ref keys, i0);
            ref var r1 = ref Unsafe.Add(ref keys, i1);
            ref var r2 = ref Unsafe.Add(ref keys, i2);
            Sort2(ref r0, ref r1, comparer, ref values, i0, i1);
            Sort2(ref r0, ref r2, comparer, ref values, i0, i2);
            Sort2(ref r1, ref r2, comparer, ref values, i1, i2);
            return ref r1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void Sort2<TKey, TValue, TComparer>(
            ref TKey keys, ref TValue values, int i, int j,
            TComparer comparer)
            where TComparer : IDirectComparer<TKey>
        {
            Debug.Assert(i != j);

            ref TKey a = ref Unsafe.Add(ref keys, i);
            ref TKey b = ref Unsafe.Add(ref keys, j);
            Sort2(ref a, ref b, comparer, ref values, i, j);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void Sort2<TKey, TValue, TComparer>(
            ref TKey a, ref TKey b, TComparer comparer, 
            ref TValue values, int i, int j) 
            where TComparer : IDirectComparer<TKey>
        {
            // This is one of the only places GreaterThan is needed
            // but we need to preserve this due to bogus comparers or similar
            if (comparer.GreaterThan(a, b))
            {
                Swap(ref a, ref b);
                Swap(ref values, i, j);
            }
        }
    }
}
