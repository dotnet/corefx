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
            ref TKey keys, int length)
            where TKey : IComparable<TKey>
        {
            IntrospectiveSort(ref keys, length);
        }

        private static void IntrospectiveSort<TKey>(
            ref TKey keys, int length)
            where TKey : IComparable<TKey>
        {
            var depthLimit = 2 * FloorLog2PlusOne(length);
            IntroSort(ref keys, 0, length - 1, depthLimit);
        }

        private static void IntroSort<TKey>(
            ref TKey keys, 
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
                        Sort2(ref keys, lo, hi);
                        return;
                    }
                    if (partitionSize == 3)
                    {
                        ref TKey loRef = ref Unsafe.Add(ref keys, lo);
                        ref TKey miRef = ref Unsafe.Add(ref keys, hi - 1);
                        ref TKey hiRef = ref Unsafe.Add(ref keys, hi);
                        //ref TKey miRef = ref Unsafe.SubtractByteOffset(ref hiRef, new IntPtr(Unsafe.SizeOf<TKey>()));
                        Sort3(ref loRef, ref miRef, ref hiRef);
                        return;
                    }

                    InsertionSort(ref keys, lo, hi);
                    return;
                }

                if (depthLimit == 0)
                {
                    HeapSort(ref keys, lo, hi);
                    return;
                }
                depthLimit--;

                // We should never reach here, unless > 3 elements due to partition size
                int p = PickPivotAndPartition(ref keys, lo, hi);
                // Note we've already partitioned around the pivot and do not have to move the pivot again.
                IntroSort(ref keys, p + 1, hi, depthLimit);
                hi = p - 1;
            }
        }

        private static int PickPivotAndPartition<TKey>(
            ref TKey keys, int lo, int hi)
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
            ref TKey keysAtLo = ref Unsafe.Add(ref keys, lo);
            ref TKey keysAtMiddle = ref Unsafe.Add(ref keys, middle);
            ref TKey keysAtHi = ref Unsafe.Add(ref keys, hi);
            Sort3(ref keysAtLo, ref keysAtMiddle, ref keysAtHi);

            TKey pivot = keysAtMiddle;

            int left = lo;
            int right = hi - 1;
            // We already partitioned lo and hi and put the pivot in hi - 1.  
            // And we pre-increment & decrement below.
            Swap(ref keysAtMiddle, ref Unsafe.Add(ref keys, right));

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
                    // TODO: Possible buffer over/underflow here if custom CompareTo? What to do?
                    //       Here we bound the expression like in the above loop, but is that the same in coreclr?
                    //       This is the reason for "catch (IndexOutOfRangeException) => IntrospectiveSortUtilities.ThrowOrIgnoreBadComparer(comparer);"
                    // NOTE: Inserted check to ensure no out of bounds
                    while (left < (hi - 1) && pivot.CompareTo(Unsafe.Add(ref keys, ++left)) > 0) ;
                    while (right > lo && pivot.CompareTo(Unsafe.Add(ref keys, --right)) < 0) ;
                }

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
            ref TKey keys, int lo, int hi)
            where TKey : IComparable<TKey>
        {
            
            Debug.Assert(lo >= 0);
            Debug.Assert(hi > lo);

            int n = hi - lo + 1;
            for (int i = n / 2; i >= 1; --i)
            {
                DownHeap(ref keys, i, n, lo);
            }
            for (int i = n; i > 1; --i)
            {
                Swap(ref keys, lo, lo + i - 1);
                DownHeap(ref keys, 1, i - 1, lo);
            }
        }

        private static void DownHeap<TKey>(
            ref TKey keys, int i, int n, int lo)
            where TKey : IComparable<TKey>
        {
            
            Debug.Assert(lo >= 0);

            //TKey d = keys[lo + i - 1];
            ref TKey keysAtLo = ref Unsafe.Add(ref keys, lo);
            ref TKey keysAtLoMinus1 = ref Unsafe.Add(ref keysAtLo, -1); // No Subtract??
            TKey d = Unsafe.Add(ref keysAtLoMinus1, i);
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

                i = child;
            }
            //keys[lo + i - 1] = d;
            Unsafe.Add(ref keysAtLoMinus1, i) = d;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void InsertionSort<TKey>(
            ref TKey keys, int lo, int hi)
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
                    do
                    {
                        Unsafe.Add(ref keys, j + 1) = Unsafe.Add(ref keys, j);
                        --j;
                    }
                    while (j >= lo && (t == null || t.CompareTo(Unsafe.Add(ref keys, j)) < 0));
                    //while (j >= lo && (t == null || t.CompareTo(keys[j]) < 0))

                    Unsafe.Add(ref keys, j + 1) = t;
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void Sort3<TKey>(
            ref TKey r0, ref TKey r1, ref TKey r2)
            where TKey : IComparable<TKey>
        {
            Sort2(ref r0, ref r1);
            Sort2(ref r0, ref r2);
            Sort2(ref r1, ref r2);

            // Below works but does not give exactly the same result as Array.Sort
            // i.e. order could be a bit different for keys that are equal
            //if (r0 != null && r0.CompareTo(r1) <= 0) //r0 <= r1)
            //{
            //    if (r1 != null && r1.CompareTo(r2) <= 0) //(r1 <= r2)
            //    {
            //        return;
            //    }
            //    else if (r0.CompareTo(r2) < 0) //(r0 < r2)
            //    {
            //        Swap(ref r1, ref r2);
            //    }
            //    else
            //    {
            //        TKey tmp = r0;
            //        r0 = r2;
            //        r2 = r1;
            //        r1 = tmp;
            //    }
            //}
            //else
            //{
            //    if (r0 != null && r0.CompareTo(r2) < 0) //(r0 < r2)
            //    {
            //        Swap(ref r0, ref r1);
            //    }
            //    else if (r2 != null && r2.CompareTo(r1) < 0) //(r2 < r1)
            //    {
            //        Swap(ref r0, ref r2);
            //    }
            //    else
            //    {
            //        TKey tmp = r0;
            //        r0 = r1;
            //        r1 = r2;
            //        r2 = tmp;
            //    }
            //}
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void Sort2<TKey>(
            ref TKey keys, int i, int j)
            where TKey : IComparable<TKey>
        {
            Debug.Assert(i != j);

            ref TKey a = ref Unsafe.Add(ref keys, i);
            ref TKey b = ref Unsafe.Add(ref keys, j);
            Sort2(ref a, ref b);
        }

        private static void Sort2<TKey>(ref TKey a, ref TKey b) 
            where TKey : IComparable<TKey>
        {
            if (a != null && a.CompareTo(b) > 0)
            {
                TKey temp = a;
                a = b;
                b = temp;
            }
        }
    }
}
