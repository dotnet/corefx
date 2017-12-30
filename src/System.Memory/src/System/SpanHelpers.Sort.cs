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
        internal static void Sort<T>(this Span<T> span)
        {
            if (typeof(T) == typeof(int))
            {
                //ref var intRef = ref Unsafe.As<T, int>(ref MemoryManager.GetReference(span));
                ref var intRef = ref Unsafe.As<T, int>(ref span.DangerousGetPinnableReference());
                SpanSortHelper<int, ComparableComparer<int>>.Sort(ref intRef, span.Length, new ComparableComparer<int>());
            }
            else
            {
                Sort(span, Comparer<T>.Default);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void Sort<T, TComparer>(
            this Span<T> span, TComparer comparer)
            where TComparer : IComparer<T>
        {
            SpanSortHelper<T, TComparer>.s_default.Sort(span, comparer);
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
        internal interface ISpanSortHelper<TKey, TComparer>
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


        internal class SpanSortHelper<T, TComparer> : ISpanSortHelper<T, TComparer>
            where TComparer : IComparer<T>
        {
            //private static volatile ISpanSortHelper<T, TComparer> defaultArraySortHelper;

            //public static ISpanSortHelper<T, TComparer> Default
            //{
            //    get
            //    {
            //        ISpanSortHelper<T, TComparer> sorter = defaultArraySortHelper;
            //        if (sorter == null)
            //            sorter = CreateArraySortHelper();

            //        return sorter;
            //    }
            //}
            internal static readonly ISpanSortHelper<T, TComparer> s_default = CreateSortHelper();

            private static ISpanSortHelper<T, TComparer> CreateSortHelper()
            {
                if (typeof(IComparable<T>).IsAssignableFrom(typeof(T)))
                {
                    // TODO: Is there a faster way?
                    var ctor = typeof(ComparableSpanSortHelper<,>)
                        .MakeGenericType(new Type[] { typeof(T), typeof(TComparer) })
                        .GetConstructor(Array.Empty<Type>());

                    return (ISpanSortHelper<T, TComparer>)ctor.Invoke(Array.Empty<object>());
                    // coreclr does the following:
                    //return (IArraySortHelper<T, TComparer>)
                    //    RuntimeTypeHandle.Allocate(
                    //        .TypeHandle.Instantiate());
                }
                else
                {
                    return new SpanSortHelper<T, TComparer>();
                }
            }

            public void Sort(Span<T> keys, in TComparer comparer)
            {
                // Add a try block here to detect IComparers (or their
                // underlying IComparables, etc) that are bogus.
                // TODO: Do we need the try/catch?? Only when using default comparer?
                try
                {
                    if (typeof(TComparer) == typeof(IComparer<T>) && comparer == null)
                    {
                        SpanSortHelper<T, IComparer<T>>.Sort(
                            ref keys.DangerousGetPinnableReference(), keys.Length, 
                            Comparer<T>.Default);
                    }
                    else
                    {
                        Sort(ref keys.DangerousGetPinnableReference(), keys.Length, comparer);
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

            internal static void Sort(ref T spanStart, int length, in TComparer comparer)
            {
                IntrospectiveSort(ref spanStart, length, comparer);
            }

            private static void IntrospectiveSort(ref T spanStart, int length, in TComparer comparer)
            {
                if (length < 2)
                    return;

                // Note how old used the full length of keys array to limit,
                //IntroSort(keys, left, length + left - 1, 2 * IntrospectiveSortUtilities.FloorLog2PlusOne(keys.Length), comparer);
                var depthLimit = 2 * IntrospectiveSortUtilities.FloorLog2PlusOne(length);
                IntroSort(ref spanStart, 0, length - 1, depthLimit, comparer);
            }

            private static void IntroSort(ref T keys, int lo, int hi, int depthLimit, in TComparer comparer)
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
                            // No indeces equal here!
                            SwapIfGreater(ref keys, comparer, lo, hi);
                            return;
                        }
                        if (partitionSize == 3)
                        {
                            // No indeces equal here! Many indeces can be reused here...
                            SwapIfGreater(ref keys, comparer, lo, hi - 1);
                            SwapIfGreater(ref keys, comparer, lo, hi);
                            SwapIfGreater(ref keys, comparer, hi - 1, hi);
                            // Replace with optimal 3 element sort
                            // if a[0] < a[1]:
                            //    if a[1] > a[2]:
                            //        if a[0] < a[2]:
                            //            temp = a[1]
                            //            a[1] = a[2]
                            //            a[2] = temp
                            //        else:
                            //            temp = a[0]
                            //            a[0] = a[2]
                            //            a[2] = a[1]
                            //            a[1] = temp
                            //    else:
                            //        # do nothing
                            //else:
                            //    if a[1] < a[2]:
                            //        if a[0] < a[2]:
                            //            temp = a[0]
                            //            a[0] = a[1]
                            //            a[1] = temp
                            //        else:
                            //            temp = a[0]
                            //            a[0] = a[1]
                            //            a[1] = a[2]
                            //            a[2] = temp
                            //    else:
                            //        temp = a[0]
                            //        a[0] = a[2]
                            //        a[2] = temp
                            //template < typename T >
                            //void sort3(T(&a)[3])
                            //{
                            //    if (a[0] < a[1])
                            //    {
                            //        if (a[1] < a[2])
                            //        {
                            //            return;
                            //        }
                            //        else if (a[0] < a[2])
                            //        {
                            //            std::swap(a[1], a[2]);
                            //        }
                            //        else
                            //        {
                            //            T tmp = std::move(a[0]);
                            //            a[0] = std::move(a[2]);
                            //            a[2] = std::move(a[1]);
                            //            a[1] = std::move(tmp);
                            //        }
                            //    }
                            //    else
                            //    {
                            //        if (a[0] < a[2])
                            //        {
                            //            std::swap(a[0], a[1]);
                            //        }
                            //        else if (a[2] < a[1])
                            //        {
                            //            std::swap(a[0], a[2]);
                            //        }
                            //        else
                            //        {
                            //            T tmp = std::move(a[0]);
                            //            a[0] = std::move(a[1]);
                            //            a[1] = std::move(a[2]);
                            //            a[2] = std::move(tmp);
                            //        }
                            //    }
                            //}
                            return;
                        }

                        InsertionSort(ref keys, lo, hi, comparer);
                        return;
                    }

                    if (depthLimit == 0)
                    {
                        HeapSort(ref keys, lo, hi, comparer);
                        return;
                    }
                    depthLimit--;

                    // We should never reach here, unless > 3 elements due to partition size
                    int p = PickPivotAndPartition(ref keys, lo, hi, comparer);
                    // Note we've already partitioned around the pivot and do not have to move the pivot again.
                    IntroSort(ref keys, p + 1, hi, depthLimit, comparer);
                    hi = p - 1;
                }
            }

            private static int PickPivotAndPartition(ref T keys, int lo, int hi, in TComparer comparer)
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
                SwapIfGreater(ref keys, comparer, lo, middle);  // swap the low with the mid point
                SwapIfGreater(ref keys, comparer, lo, hi);   // swap the low with the high
                SwapIfGreater(ref keys, comparer, middle, hi); // swap the middle with the high

                T pivot = Unsafe.Add(ref keys, middle);
                // Swap in different way
                Swap(ref keys, middle, hi - 1);
                int left = lo, right = hi - 1;  // We already partitioned lo and hi and put the pivot in hi - 1.  And we pre-increment & decrement below.

                while (left < right)
                {
                    // TODO: Would be good to update local ref here
                    while (comparer.Compare(Unsafe.Add(ref keys, ++left), pivot) < 0) ;
                    // TODO: Would be good to update local ref here
                    while (comparer.Compare(pivot, Unsafe.Add(ref keys, --right)) < 0) ;

                    if (left >= right)
                        break;

                    // Indeces cannot be equal here
                    Swap(ref keys, left, right);
                }

                // Put pivot in the right location.
                right = (hi - 1);
                if (left != right)
                {
                    Swap(ref keys, left, right);
                }
                return left;
            }

            private static void HeapSort(ref T keys, int lo, int hi, in TComparer comparer)
            {
                Debug.Assert(keys != null);
                Debug.Assert(comparer != null);
                Debug.Assert(lo >= 0);
                Debug.Assert(hi > lo);

                int n = hi - lo + 1;
                for (int i = n / 2; i >= 1; --i)
                {
                    DownHeap(ref keys, i, n, lo, comparer);
                }
                for (int i = n; i > 1; --i)
                {
                    Swap(ref keys, lo, lo + i - 1);
                    DownHeap(ref keys, 1, i - 1, lo, comparer);
                }
            }

            private static void DownHeap(ref T keys, int i, int n, int lo, in TComparer comparer)
            {
                Debug.Assert(keys != null);
                Debug.Assert(comparer != null);
                Debug.Assert(lo >= 0);

                ////T d = keys[lo + i - 1];
                //T d = Unsafe.Add(ref keys, lo + i - 1);
                //int child;
                //while (i <= n / 2)
                //{
                //    child = 2 * i;
                //    //if (child < n && comparer(keys[lo + child - 1], keys[lo + child]) < 0)
                //    if (child < n && comparer.Compare(Unsafe.Add(ref keys, lo + child - 1), 
                //        Unsafe.Add(ref keys, lo + child)) < 0)
                //    {
                //            child++;
                //    }
                //    //if (!(comparer(d, keys[lo + child - 1]) < 0))
                //    if (!(comparer.Compare(d, Unsafe.Add(ref keys, lo + child - 1)) < 0))
                //        break;
                //    // keys[lo + i - 1] = keys[lo + child - 1]
                //    Unsafe.Add(ref keys, lo + i - 1) = Unsafe.Add(ref keys, lo + child - 1);
                //    i = child;
                //}
                ////keys[lo + i - 1] = d;
                //Unsafe.Add(ref keys, lo + i - 1) = d;


                ref T d = ref Unsafe.Add(ref keys, lo + i - 1);
                T v = d;
                int child;
                while (i <= n / 2)
                {
                    child = 2 * i;
                    // TODO: Local ref updates needed
                    //ref var l = ref Unsafe.Add(ref keys, lo + child - 1);
                    //ref var r = ref Unsafe.Add(ref keys, lo + child);
                    if (child < n &&
                        comparer.Compare(Unsafe.Add(ref keys, lo + child - 1),
                            Unsafe.Add(ref keys, lo + child)) < 0)
                    {
                        child++;
                    }
                    ref T c = ref Unsafe.Add(ref keys, lo + child - 1);
                    if (!(comparer.Compare(v, c) < 0))
                        break;
                    //keys[lo + i - 1] = keys[lo + child - 1];
                    d = c;
                    i = child;
                }
                //keys[lo + i - 1] = d;
                d = v;
            }

            private static void InsertionSort(ref T keys, int lo, int hi, in TComparer comparer)
            {
                Debug.Assert(keys != null);
                Debug.Assert(lo >= 0);
                Debug.Assert(hi >= lo);

                int i, j;
                T t;
                for (i = lo; i < hi; i++)
                {
                    j = i;
                    //t = keys[i + 1];
                    t = Unsafe.Add(ref keys, i + 1);
                    // Need local ref that can be updated
                    while (j >= lo && comparer.Compare(t, Unsafe.Add(ref keys, j)) < 0)
                    {
                        Unsafe.Add(ref keys, j + 1) = Unsafe.Add(ref keys, j);
                        j--;
                    }
                    Unsafe.Add(ref keys, j + 1) = t;
                }
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private static void SwapIfGreater(ref T start, TComparer comparer, int i, int j)
            {
                Debug.Assert(i != j);
                // Check moved to the one case actually needing it, not all!
                //if (i != j)
                {
                    ref var iElement = ref Unsafe.Add(ref start, i);
                    ref var jElement = ref Unsafe.Add(ref start, j);
                    if (comparer.Compare(iElement, jElement) > 0)
                    {
                        T temp = iElement;
                        iElement = jElement;
                        jElement = temp;
                    }
                }
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private static void Swap(ref T start, int i, int j)
            {
                // TODO: Is the i!=j check necessary? Most cases not needed?
                // Only in one case it seems, REFACTOR
                Debug.Assert(i != j);
                // No place needs this it seems
                //if (i != j)
                {
                    ref var iElement = ref Unsafe.Add(ref start, i);
                    ref var jElement = ref Unsafe.Add(ref start, j);
                    T temp = iElement;
                    iElement = jElement;
                    jElement = temp;
                }
            }
        }

        internal class ComparableSpanSortHelper<T, TComparer>
        : ISpanSortHelper<T, TComparer>
        where T : IComparable<T>
        where TComparer : IComparer<T>
        {
            // Do not add a constructor to this class because SpanSortHelper<T>.CreateSortHelper will not execute it

            public void Sort(Span<T> keys, in TComparer comparer)
            {
                try
                {
                    if (comparer == null ||
                        // Cache this in generic traits helper class perhaps
                        (!typeof(TComparer).IsValueType &&
                         object.ReferenceEquals(comparer, Comparer<T>.Default)))
                    {
                        SpanSortHelper<T, ComparableComparer<T>>.Sort(
                            ref keys.DangerousGetPinnableReference(), keys.Length,
                            new ComparableComparer<T>());
                    }
                    else
                    {
                        SpanSortHelper<T, TComparer>.Sort(
                            ref keys.DangerousGetPinnableReference(), keys.Length,
                            comparer);
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

            //internal class ArraySortHelper<T, TComparer>
            //    : ISpanSortHelper<T, TComparer>
            //    where TComparer : IComparer<T>
            //{

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

            //}

        }
    }
}
