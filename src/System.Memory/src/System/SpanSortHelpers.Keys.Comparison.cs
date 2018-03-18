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
    internal static partial class SpanSortHelpersKeys
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void Sort<TKey>(
            ref TKey keys, int length,
            Comparison<TKey> comparison)
        {
            IntrospectiveSort(ref keys, length, comparison);
        }

        private static void IntrospectiveSort<TKey>(
            ref TKey keys, int length,
            Comparison<TKey> comparison)
        {
            int depthLimit = 2 * FloorLog2PlusOne(length);
            IntroSort(ref keys, 0, length - 1, depthLimit, comparison);
        }

        private static void IntroSort<TKey>(
            ref TKey keys, 
            int lo, int hi, int depthLimit,
            Comparison<TKey> comparison)
        {
            Debug.Assert(comparison != null);
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
                        Sort2(ref keys, lo, hi, comparison);
                        return;
                    }
                    if (partitionSize == 3)
                    {
                        ref TKey loRef = ref Unsafe.Add(ref keys, lo);
                        ref TKey miRef = ref Unsafe.Add(ref keys, hi - 1);
                        ref TKey hiRef = ref Unsafe.Add(ref keys, hi);
                        Sort3(ref loRef, ref miRef, ref hiRef, comparison);
                        return;
                    }

                    InsertionSort(ref keys, lo, hi, comparison);
                    return;
                }

                if (depthLimit == 0)
                {
                    HeapSort(ref keys, lo, hi, comparison);
                    return;
                }
                depthLimit--;

                // We should never reach here, unless > 3 elements due to partition size
                Debug.Assert(partitionSize > 3);

                int p = PickPivotAndPartition(ref keys, lo, hi, comparison);
                // Note we've already partitioned around the pivot and do not have to move the pivot again.
                IntroSort(ref keys, p + 1, hi, depthLimit, comparison);
                hi = p - 1;
            }
        }

        private static int PickPivotAndPartition<TKey>(
            ref TKey keys, int lo, int hi,
            Comparison<TKey> comparison)
        {
            Debug.Assert(comparison != null);
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
            ref TKey keysAtLo = ref Unsafe.Add(ref keys, lo);
            ref TKey keysAtMiddle = ref Unsafe.Add(ref keys, middle);
            ref TKey keysAtHi = ref Unsafe.Add(ref keys, hi);
            Sort3(ref keysAtLo, ref keysAtMiddle, ref keysAtHi, comparison);

            TKey pivot = keysAtMiddle;

            int left = lo;
            int right = hi - 1;
            // We already partitioned lo and hi and put the pivot in hi - 1.  
            // And we pre-increment & decrement below.
            Swap(ref keysAtMiddle, ref Unsafe.Add(ref keys, right));

            while (left < right)
            {
                while (left < (hi - 1) && comparison(Unsafe.Add(ref keys, ++left), pivot) < 0) ;
                // Check if bad comparable/comparison
                if (left == (hi - 1) && comparison(Unsafe.Add(ref keys, left), pivot) < 0)
                    ThrowHelper.ThrowArgumentException_BadComparer(comparison);

                while (right > lo && comparison(pivot, Unsafe.Add(ref keys, --right)) < 0) ;
                // Check if bad comparable/comparison
                if (right == lo && comparison(pivot, Unsafe.Add(ref keys, right)) < 0)
                    ThrowHelper.ThrowArgumentException_BadComparer(comparison);

                if (left >= right)
                    break;

                Swap(ref keys, left, right);
            }
            // Put pivot in the right location.
            right = hi - 1;
            if (left != right)
            {
                Swap(ref keys, left, right);
            }
            return left;
        }

        private static void HeapSort<TKey>(
            ref TKey keys, int lo, int hi,
            Comparison<TKey> comparison)
            
        {
            Debug.Assert(comparison != null);
            Debug.Assert(lo >= 0);
            Debug.Assert(hi > lo);

            int n = hi - lo + 1;
            for (int i = n / 2; i >= 1; --i)
            {
                DownHeap(ref keys, i, n, lo, comparison);
            }
            for (int i = n; i > 1; --i)
            {
                Swap(ref keys, lo, lo + i - 1);
                DownHeap(ref keys, 1, i - 1, lo, comparison);
            }
        }

        private static void DownHeap<TKey>(
            ref TKey keys, int i, int n, int lo,
            Comparison<TKey> comparison)
            
        {
            Debug.Assert(comparison != null);
            Debug.Assert(lo >= 0);

            //TKey d = keys[lo + i - 1];
            ref TKey keysAtLo = ref Unsafe.Add(ref keys, lo);
            ref TKey keysAtLoMinus1 = ref Unsafe.Add(ref keysAtLo, -1); // TODO: Use Subtract when available
            TKey d = Unsafe.Add(ref keysAtLoMinus1, i);
            int nHalf = n / 2;
            while (i <= nHalf)
            {
                int child = i << 1;

                //if (child < n && comparison(keys[lo + child - 1], keys[lo + child]) < 0)
                if (child < n &&
                    comparison(Unsafe.Add(ref keysAtLoMinus1, child), Unsafe.Add(ref keysAtLo, child)) < 0)
                {
                    ++child;
                }

                //if (!(comparison(d, keys[lo + child - 1]) < 0))
                if (!(comparison(d, Unsafe.Add(ref keysAtLoMinus1, child)) < 0))
                    break;

                // keys[lo + i - 1] = keys[lo + child - 1]
                Unsafe.Add(ref keysAtLoMinus1, i) = Unsafe.Add(ref keysAtLoMinus1, child);

                i = child;
            }
            //keys[lo + i - 1] = d;
            Unsafe.Add(ref keysAtLoMinus1, i) = d;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void InsertionSort<TKey>(
            ref TKey keys, int lo, int hi,
            Comparison<TKey> comparison)
            
        {
            Debug.Assert(lo >= 0);
            Debug.Assert(hi >= lo);

            for (int i = lo; i < hi; ++i)
            {
                int j = i;
                
                TKey t = Unsafe.Add(ref keys, j + 1);

                if (j >= lo && comparison(t, Unsafe.Add(ref keys, j)) < 0)
                {
                    do
                    {
                        Unsafe.Add(ref keys, j + 1) = Unsafe.Add(ref keys, j);
                        --j;
                    }
                    while (j >= lo && comparison(t, Unsafe.Add(ref keys, j)) < 0);

                    Unsafe.Add(ref keys, j + 1) = t;
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void Sort3<TKey>(
            ref TKey r0, ref TKey r1, ref TKey r2,
            Comparison<TKey> comparison)
            
        {
            Sort2(ref r0, ref r1, comparison);
            Sort2(ref r0, ref r2, comparison);
            Sort2(ref r1, ref r2, comparison);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void Sort2<TKey>(
            ref TKey keys, int i, int j,
            Comparison<TKey> comparison)
            
        {
            Debug.Assert(i != j);

            ref TKey a = ref Unsafe.Add(ref keys, i);
            ref TKey b = ref Unsafe.Add(ref keys, j);
            Sort2(ref a, ref b, comparison);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void Sort2<TKey>(
            ref TKey a, ref TKey b, Comparison<TKey> comparison)
            
        {
            // This is one of the only places GreaterThan is needed
            // but we need to preserve this due to bogus comparers or similar
            if (comparison(a, b) > 0)
            {
                TKey temp = a;
                a = b;
                b = temp;
            }
        }
    }
}
