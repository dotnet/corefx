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
        internal static void Sort<TKey, TValue>(
            ref TKey keys, ref TValue values, int length)
            where TKey : IComparable<TKey>
        {
            IntrospectiveSort(ref keys, ref values, length);
        }

        private static void IntrospectiveSort<TKey, TValue>(
            ref TKey keys, ref TValue values, int length)
            where TKey : IComparable<TKey>
        {
            var depthLimit = 2 * FloorLog2PlusOne(length);
            IntroSort(ref keys, ref values, 0, length - 1, depthLimit);
        }

        private static void IntroSort<TKey, TValue>(
            ref TKey keys, ref TValue values,
            int lo, int hi, int depthLimit)
            where TKey : IComparable<TKey>
        {
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
                        Sort2(ref keys, ref values, lo, hi);
                        return;
                    }
                    if (partitionSize == 3)
                    {
                        Sort3(ref keys, ref values, lo, hi - 1, hi);
                        return;
                    }
                    InsertionSort(ref keys, ref values, lo, hi);
                    return;
                }

                if (depthLimit == 0)
                {
                    HeapSort(ref keys, ref values, lo, hi);
                    return;
                }
                depthLimit--;

                // We should never reach here, unless > 3 elements due to partition size
                int p = PickPivotAndPartition(ref keys, ref values, lo, hi);
                // Note we've already partitioned around the pivot and do not have to move the pivot again.
                IntroSort(ref keys, ref values, p + 1, hi, depthLimit);
                hi = p - 1;
            }
        }

        private static int PickPivotAndPartition<TKey, TValue>(
            ref TKey keys, ref TValue values, int lo, int hi)
            where TKey : IComparable<TKey>
        {
            
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
            ref TKey keysAtMiddle = ref Sort3(ref keys, ref values, lo, middle, hi);

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

                if (pivot == null)
                {
                    while (left < (hi - 1) && Unsafe.Add(ref keys, ++left) == null) ;
                    while (right > lo && Unsafe.Add(ref keys, --right) != null) ;
                }
                else
                {
                    while (Unsafe.Add(ref keys, ++left).CompareTo(pivot) < 0) ;
                    while (pivot.CompareTo(Unsafe.Add(ref keys, --right)) < 0) ;
                }

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

        private static void HeapSort<TKey, TValue>(
            ref TKey keys, ref TValue values, int lo, int hi
            )
            where TKey : IComparable<TKey>
        {
            
            Debug.Assert(lo >= 0);
            Debug.Assert(hi > lo);

            int n = hi - lo + 1;
            for (int i = n / 2; i >= 1; --i)
            {
                DownHeap(ref keys, ref values, i, n, lo);
            }
            for (int i = n; i > 1; --i)
            {
                Swap(ref keys, lo, lo + i - 1);
                Swap(ref values, lo, lo + i - 1);
                DownHeap(ref keys, ref values, 1, i - 1, lo);
            }
        }

        private static void DownHeap<TKey, TValue>(
            ref TKey keys, ref TValue values, int i, int n, int lo)
            where TKey : IComparable<TKey>
        {
            
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

                //if (child < n && (keys[lo + child - 1] == null || keys[lo + child - 1].CompareTo(keys[lo + child]) < 0))
                if (child < n && 
                    (Unsafe.Add(ref keysAtLoMinus1, child) == null ||
                     Unsafe.Add(ref keysAtLoMinus1, child).CompareTo(Unsafe.Add(ref keysAtLo, child)) < 0))
                {
                    ++child;
                }

                //if (keys[lo + child - 1] == null || keys[lo + child - 1].CompareTo(d) < 0)
                if (Unsafe.Add(ref keysAtLoMinus1, child) == null || 
                    !(d.CompareTo(Unsafe.Add(ref keysAtLoMinus1, child)) < 0))
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
        private static void InsertionSort<TKey, TValue>(
            ref TKey keys, ref TValue values, int lo, int hi)
            where TKey : IComparable<TKey>
        {
            Debug.Assert(lo >= 0);
            Debug.Assert(hi >= lo);

            for (int i = lo; i < hi; ++i)
            {
                int j = i;
                //t = keys[i + 1];
                var t = Unsafe.Add(ref keys, j + 1);
                // TODO: Would be good to be able to update local ref here
                if (j >= lo && (t == null || t.CompareTo(Unsafe.Add(ref keys, j)) < 0))
                {
                    var v = Unsafe.Add(ref values, j + 1);
                    do
                    {
                        Unsafe.Add(ref keys, j + 1) = Unsafe.Add(ref keys, j);
                        Unsafe.Add(ref values, j + 1) = Unsafe.Add(ref values, j);
                        --j;
                    }
                    while (j >= lo && (t == null || t.CompareTo(Unsafe.Add(ref keys, j)) < 0));
                    //while (j >= lo && (t == null || t.CompareTo(keys[j]) < 0))

                    Unsafe.Add(ref keys, j + 1) = t;
                    Unsafe.Add(ref values, j + 1) = v;
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static ref TKey Sort3<TKey, TValue>(
            ref TKey keys, ref TValue values, int i0, int i1, int i2)
            where TKey : IComparable<TKey>
        {
            ref var r0 = ref Unsafe.Add(ref keys, i0);
            ref var r1 = ref Unsafe.Add(ref keys, i1);
            ref var r2 = ref Unsafe.Add(ref keys, i2);

            if (r0 != null && r0.CompareTo(r1) < 0) //r0 < r1)
            {
                if (r1 != null && r1.CompareTo(r2) < 0) //(r1 < r2)
                {
                    return ref r1;
                }
                else if (r0.CompareTo(r2) < 0) //(r0 < r2)
                {
                    Swap(ref r1, ref r2);
                    ref var v1 = ref Unsafe.Add(ref values, i1);
                    ref var v2 = ref Unsafe.Add(ref values, i2);
                    Swap(ref v1, ref v2);
                }
                else
                {
                    TKey tmp = r0;
                    r0 = r2;
                    r2 = r1;
                    r1 = tmp;
                    ref var v0 = ref Unsafe.Add(ref values, i0);
                    ref var v1 = ref Unsafe.Add(ref values, i1);
                    ref var v2 = ref Unsafe.Add(ref values, i2);
                    TValue vTemp = v0;
                    v0 = v2;
                    v2 = v1;
                    v1 = vTemp;
                }
            }
            else
            {
                if (r0 != null && r0.CompareTo(r2) < 0) //(r0 < r2)
                {
                    Swap(ref r0, ref r1);
                    ref var v0 = ref Unsafe.Add(ref values, i0);
                    ref var v1 = ref Unsafe.Add(ref values, i1);
                    Swap(ref v0, ref v1);
                }
                else if (r2 != null && r2.CompareTo(r1) < 0) //(r2 < r1)
                {
                    Swap(ref r0, ref r2);
                    ref var v0 = ref Unsafe.Add(ref values, i0);
                    ref var v2 = ref Unsafe.Add(ref values, i2);
                    Swap(ref v0, ref v2);
                }
                else
                {
                    TKey tmp = r0;
                    r0 = r1;
                    r1 = r2;
                    r2 = tmp;
                    ref var v0 = ref Unsafe.Add(ref values, i0);
                    ref var v1 = ref Unsafe.Add(ref values, i1);
                    ref var v2 = ref Unsafe.Add(ref values, i2);
                    TValue vTemp = v0;
                    v0 = v1;
                    v1 = v2;
                    v2 = vTemp;
                }
            }
            return ref r1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void Sort2<TKey, TValue>(
            ref TKey keys, ref TValue values, int i, int j)
            where TKey : IComparable<TKey>
        {
            Debug.Assert(i != j);

            ref TKey a = ref Unsafe.Add(ref keys, i);
            ref TKey b = ref Unsafe.Add(ref keys, j);
            if (a != null && a.CompareTo(b) > 0)
            {
                Swap(ref a, ref b);
                Swap(ref values, i, j);
            }
        }
    }
}
