// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Diagnostics;
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
            //if (comparer == null)
            //    ThrowHelper.ThrowArgumentNullException(ExceptionArgument.comparer);

            ArraySortHelper<T, TComparer>.Default.Sort(span, comparer);
        }

        internal static class SpanSortHelper<T, TComparer>
            where TComparer : IComparer<T>
        {
            internal static void Sort(ref T spanStart, int length, TComparer comparer)
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


            internal static void IntrospectiveSort(ref T spanStart, int length, TComparer comparer)
            {
                if (length < 2)
                    return;

                var depthLimit = 2 * IntrospectiveSortUtilities.FloorLog2PlusOne(length);
                IntroSort(ref spanStart, 0, length - 1, depthLimit, comparer);
            }

            private static void IntroSort(ref T keys, int lo, int hi, int depthLimit, TComparer comparer)
            {
                Debug.Assert(comparer != null);
                Debug.Assert(lo >= 0);

                while (hi > lo)
                {
                    int partitionSize = hi - lo + 1;
                    if (partitionSize <= IntrospectiveSortUtilities.IntrosortSizeThreshold)
                    {
                        if (partitionSize == 1)
                        {
                            return;
                        }
                        if (partitionSize == 2)
                        {
                            SwapIfGreater(keys, comparer, lo, hi);
                            return;
                        }
                        if (partitionSize == 3)
                        {
                            SwapIfGreater(keys, comparer, lo, hi - 1);
                            SwapIfGreater(keys, comparer, lo, hi);
                            SwapIfGreater(keys, comparer, hi - 1, hi);
                            return;
                        }

                        InsertionSort(keys, lo, hi, comparer);
                        return;
                    }

                    if (depthLimit == 0)
                    {
                        Heapsort(keys, lo, hi, comparer);
                        return;
                    }
                    depthLimit--;

                    int p = PickPivotAndPartition(keys, lo, hi, comparer);
                    // Note we've already partitioned around the pivot and do not have to move the pivot again.
                    IntroSort(keys, p + 1, hi, depthLimit, comparer);
                    hi = p - 1;
                }
            }

            private static int PickPivotAndPartition(T[] keys, int lo, int hi, Comparison<T> comparer)
            {
                Debug.Assert(keys != null);
                Debug.Assert(comparer != null);
                Debug.Assert(lo >= 0);
                Debug.Assert(hi > lo);
                Debug.Assert(hi < keys.Length);

                // Compute median-of-three.  But also partition them, since we've done the comparison.
                int middle = lo + ((hi - lo) / 2);

                // Sort lo, mid and hi appropriately, then pick mid as the pivot.
                SwapIfGreater(keys, comparer, lo, middle);  // swap the low with the mid point
                SwapIfGreater(keys, comparer, lo, hi);   // swap the low with the high
                SwapIfGreater(keys, comparer, middle, hi); // swap the middle with the high

                T pivot = keys[middle];
                Swap(keys, middle, hi - 1);
                int left = lo, right = hi - 1;  // We already partitioned lo and hi and put the pivot in hi - 1.  And we pre-increment & decrement below.

                while (left < right)
                {
                    while (comparer(keys[++left], pivot) < 0)
                        ;
                    while (comparer(pivot, keys[--right]) < 0)
                        ;

                    if (left >= right)
                        break;

                    Swap(keys, left, right);
                }

                // Put pivot in the right location.
                Swap(keys, left, (hi - 1));
                return left;
            }

            private static void Heapsort(T[] keys, int lo, int hi, Comparison<T> comparer)
            {
                Debug.Assert(keys != null);
                Debug.Assert(comparer != null);
                Debug.Assert(lo >= 0);
                Debug.Assert(hi > lo);
                Debug.Assert(hi < keys.Length);

                int n = hi - lo + 1;
                for (int i = n / 2; i >= 1; i = i - 1)
                {
                    DownHeap(keys, i, n, lo, comparer);
                }
                for (int i = n; i > 1; i = i - 1)
                {
                    Swap(keys, lo, lo + i - 1);
                    DownHeap(keys, 1, i - 1, lo, comparer);
                }
            }

            private static void DownHeap(T[] keys, int i, int n, int lo, Comparison<T> comparer)
            {
                Debug.Assert(keys != null);
                Debug.Assert(comparer != null);
                Debug.Assert(lo >= 0);
                Debug.Assert(lo < keys.Length);

                T d = keys[lo + i - 1];
                int child;
                while (i <= n / 2)
                {
                    child = 2 * i;
                    if (child < n && comparer(keys[lo + child - 1], keys[lo + child]) < 0)
                    {
                        child++;
                    }
                    if (!(comparer(d, keys[lo + child - 1]) < 0))
                        break;
                    keys[lo + i - 1] = keys[lo + child - 1];
                    i = child;
                }
                keys[lo + i - 1] = d;
            }

            private static void InsertionSort(T[] keys, int lo, int hi, Comparison<T> comparer)
            {
                Debug.Assert(keys != null);
                Debug.Assert(lo >= 0);
                Debug.Assert(hi >= lo);
                Debug.Assert(hi <= keys.Length);

                int i, j;
                T t;
                for (i = lo; i < hi; i++)
                {
                    j = i;
                    t = keys[i + 1];
                    while (j >= lo && comparer(t, keys[j]) < 0)
                    {
                        keys[j + 1] = keys[j];
                        j--;
                    }
                    keys[j + 1] = t;
                }
            }

            private static void SwapIfGreater<T, TComparer>(T[] keys, Comparison<T> comparer, int a, int b)
            {
                if (a != b)
                {
                    if (comparer(keys[a], keys[b]) > 0)
                    {
                        T key = keys[a];
                        keys[a] = keys[b];
                        keys[b] = key;
                    }
                }
            }

            private static void Swap(T[] a, int i, int j)
            {
                if (i != j)
                {
                    T t = a[i];
                    a[i] = a[j];
                    a[j] = t;
                }
            }
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

        // https://github.com/dotnet/coreclr/blob/master/src/mscorlib/src/System/Collections/Generic/ArraySortHelper.cs
        internal interface IArraySortHelper<TKey, TComparer>
            where TComparer : IComparer<TKey>
        {
            void Sort(Span<TKey> keys, in TComparer comparer);
            //int BinarySearch(Span<TKey> keys, TKey value, IComparer<TKey> comparer);
        }

        internal static class IntrospectiveSortUtilities
        {
            // https://github.com/dotnet/coreclr/blob/master/src/mscorlib/src/System/Collections/Generic/ArraySortHelper.cs
            // https://github.com/dotnet/coreclr/blob/master/src/classlibnative/bcltype/arrayhelpers.cpp

            // This is the threshold where Introspective sort switches to Insertion sort.
            // Empirically, 16 seems to speed up most cases without slowing down others, at least for integers.
            // Large value types may benefit from a smaller number.
            internal const int IntrosortSizeThreshold = 16;

            internal static int FloorLog2PlusOne(int n)
            {
                int result = 0;
                while (n >= 1)
                {
                    result++;
                    n = n / 2;
                }
                return result;
            }
        }

        internal class ArraySortHelper<T, TComparer>
            : IArraySortHelper<T, TComparer>
            where TComparer : IComparer<T>
        {
            private static volatile IArraySortHelper<T, TComparer> defaultArraySortHelper;

            public static IArraySortHelper<T, TComparer> Default
            {
                get
                {
                    IArraySortHelper<T, TComparer> sorter = defaultArraySortHelper;
                    if (sorter == null)
                        sorter = CreateArraySortHelper();

                    return sorter;
                }
            }

            private static IArraySortHelper<T, TComparer> CreateArraySortHelper()
            {
                if (typeof(IComparable<T>).IsAssignableFrom(typeof(T)))
                {
                    defaultArraySortHelper = (IArraySortHelper<T, TComparer>)
                        RuntimeTypeHandle.Allocate(
                            typeof(GenericArraySortHelper<string, TComparer>).TypeHandle.Instantiate(new Type[] { typeof(T), typeof(TComparer) }));
                }
                else
                {
                    defaultArraySortHelper = new ArraySortHelper<T, TComparer>();
                }
                return defaultArraySortHelper;
            }

            #region IArraySortHelper<T> Members

            public void Sort(Span<T> keys, TComparer comparer)
            {
                // Add a try block here to detect IComparers (or their
                // underlying IComparables, etc) that are bogus.
                try
                {
                    if (typeof(TComparer) == typeof(IComparer<T>) && comparer == null)
                    {
                        Sort<T, IComparer<T>>(ref keys.DangerousGetPinnableReference(), keys.Length, Comparer<T>.Default);
                    }
                    else
                    {
                        Sort<T, TComparer>(ref keys.DangerousGetPinnableReference(), keys.Length, comparer);
                    }
                }
                catch (IndexOutOfRangeException)
                {
                    //IntrospectiveSortUtilities.ThrowOrIgnoreBadComparer(comparer);
                }
                catch (Exception e)
                {
                    throw e;
                    //throw new InvalidOperationException(SR.InvalidOperation_IComparerFailed, e);
                }
            }

            //public int BinarySearch(Span<T> array, T value, TComparer comparer)
            //{
            //    try
            //    {
            //        if (comparer == null)
            //        {
            //            comparer = Comparer<T>.Default;
            //        }

            //        return InternalBinarySearch(array, index, length, value, comparer);
            //    }
            //    catch (Exception e)
            //    {
            //        throw new InvalidOperationException(SR.InvalidOperation_IComparerFailed, e);
            //    }
            //}

            //internal static void Sort(Span<T> keys, Comparison<T> comparer)
            //{
            //    Debug.Assert(keys != null, "Check the arguments in the caller!");
            //    Debug.Assert(index >= 0 && length >= 0 && (keys.Length - index >= length), "Check the arguments in the caller!");
            //    Debug.Assert(comparer != null, "Check the arguments in the caller!");

            //    // Add a try block here to detect bogus comparisons
            //    try
            //    {
            //        IntrospectiveSort(keys, index, length, comparer);
            //    }
            //    catch (IndexOutOfRangeException)
            //    {
            //        IntrospectiveSortUtilities.ThrowOrIgnoreBadComparer(comparer);
            //    }
            //    catch (Exception e)
            //    {
            //        throw new InvalidOperationException(SR.InvalidOperation_IComparerFailed, e);
            //    }
            //}

            //internal static int InternalBinarySearch(T[] array, int index, int length, T value, IComparer<T> comparer)
            //{
            //    Debug.Assert(array != null, "Check the arguments in the caller!");
            //    Debug.Assert(index >= 0 && length >= 0 && (array.Length - index >= length), "Check the arguments in the caller!");

            //    int lo = index;
            //    int hi = index + length - 1;
            //    while (lo <= hi)
            //    {
            //        int i = lo + ((hi - lo) >> 1);
            //        int order = comparer.Compare(array[i], value);

            //        if (order == 0)
            //            return i;
            //        if (order < 0)
            //        {
            //            lo = i + 1;
            //        }
            //        else
            //        {
            //            hi = i - 1;
            //        }
            //    }

            //    return ~lo;
            //}

        }

    }
}
