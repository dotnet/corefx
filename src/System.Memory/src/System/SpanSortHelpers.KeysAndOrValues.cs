// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

#if !netstandard
using Internal.Runtime.CompilerServices;
#endif

using static System.SpanSortHelpersCommon;
using S = System.SpanSortHelpersKeysAndOrValues;

namespace System
{
    // TODO: This is my futile attempt to consolidate all variants into a 
    //       single code base. Performance suffered though and this
    //       would still have the issues with canonical representation of 
    //       generic types and methods when using a reference type.
    //       It also has issues with null references etc.

    internal static partial class SpanSortHelpersKeysAndOrValues
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void Sort<TKey>(this Span<TKey> keys)
        {
            int length = keys.Length;
            if (length < 2)
                return;

            // PERF: Try specialized here for optimal performance
            // Code-gen is weird unless used in loop outside
            if (!TrySortSpecialized(
                ref keys.DangerousGetPinnableReference(), length))
            {
                Span<Void> values = default;
                DefaultSpanSortHelper<TKey, Void>.s_default.Sort(
                    ref keys.DangerousGetPinnableReference(),
                    ref values.DangerousGetPinnableReference(),
                    length);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void Sort<TKey, TComparer>(
            this Span<TKey> keys, TComparer comparer)
            where TComparer : IComparer<TKey>
        {
            int length = keys.Length;
            if (length < 2)
                return;

            Span<Void> values = default;
            DefaultSpanSortHelper<TKey, Void, TComparer>.s_default.Sort(
                ref keys.DangerousGetPinnableReference(),
                ref values.DangerousGetPinnableReference(),
                length, comparer);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void Sort<TKey, TValue>(this Span<TKey> keys, Span<TValue> values)
        {
            int length = keys.Length;
            if (length != values.Length)
                ThrowHelper.ThrowArgumentException_ItemsMustHaveSameLength();
            if (length < 2)
                return;

            // PERF: Try specialized here for optimal performance
            // Code-gen is weird unless used in loop outside
            if (!TrySortSpecializedWithValues(
                ref keys.DangerousGetPinnableReference(),
                ref values.DangerousGetPinnableReference(),
                length))
            {
                DefaultSpanSortHelper<TKey, TValue>.s_default.Sort(
                    ref keys.DangerousGetPinnableReference(),
                    ref values.DangerousGetPinnableReference(),
                    keys.Length);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void Sort<TKey, TValue, TComparer>(
            this Span<TKey> keys, Span<TValue> values, TComparer comparer)
            where TComparer : IComparer<TKey>
        {
            int length = keys.Length;
            if (length != values.Length)
                ThrowHelper.ThrowArgumentException_ItemsMustHaveSameLength();
            if (length < 2)
                return;

            DefaultSpanSortHelper<TKey, TValue, TComparer>.s_default.Sort(
                ref keys.DangerousGetPinnableReference(),
                ref values.DangerousGetPinnableReference(),
                keys.Length, comparer);
        }

        // https://github.com/dotnet/coreclr/blob/master/src/mscorlib/src/System/Collections/Generic/ArraySortHelper.cs
        // https://github.com/dotnet/coreclr/blob/master/src/classlibnative/bcltype/arrayhelpers.cpp

        internal struct Void { }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static bool TrySortSpecialized<TKey>(
            ref TKey keys, int length)
        {
            Void values;
            return TrySortSpecialized(ref keys, ref values, length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static bool TrySortSpecializedWithValues<TKey, TValue>(
            ref TKey keys, ref TValue values, int length)
        {
            return TrySortSpecialized(ref keys, ref values, length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static bool TrySortSpecialized<TKey, TValue>(
            ref TKey keys, ref TValue values, int length)
        {
            // Types unfolded adopted from https://github.com/dotnet/coreclr/blob/master/src/classlibnative/bcltype/arrayhelpers.cpp#L268
            if (typeof(TKey) == typeof(sbyte))
            {
                ref var specificKeys = ref Unsafe.As<TKey, sbyte>(ref keys);
                Sort(ref specificKeys, ref values, length, new SByteDirectComparer());
                return true;
            }
            else if (typeof(TKey) == typeof(byte) ||
                     typeof(TKey) == typeof(bool)) // Use byte for bools to reduce code size
            {
                ref var specificKeys = ref Unsafe.As<TKey, byte>(ref keys);
                Sort(ref specificKeys, ref values, length, new ByteDirectComparer());
                return true;
            }
            else if (typeof(TKey) == typeof(short) ||
                     typeof(TKey) == typeof(char)) // Use short for chars to reduce code size
            {
                ref var specificKeys = ref Unsafe.As<TKey, short>(ref keys);
                Sort(ref specificKeys, ref values, length, new Int16DirectComparer());
                return true;
            }
            else if (typeof(TKey) == typeof(ushort))
            {
                ref var specificKeys = ref Unsafe.As<TKey, ushort>(ref keys);
                Sort(ref specificKeys, ref values, length, new UInt16DirectComparer());
                return true;
            }
            else if (typeof(TKey) == typeof(int))
            {
                ref var specificKeys = ref Unsafe.As<TKey, int>(ref keys);
                Sort(ref specificKeys, ref values, length, new Int32DirectComparer());
                return true;
            }
            else if (typeof(TKey) == typeof(uint))
            {
                ref var specificKeys = ref Unsafe.As<TKey, uint>(ref keys);
                Sort(ref specificKeys, ref values, length, new UInt32DirectComparer());
                return true;
            }
            else if (typeof(TKey) == typeof(long))
            {
                ref var specificKeys = ref Unsafe.As<TKey, long>(ref keys);
                Sort(ref specificKeys, ref values, length, new Int64DirectComparer());
                return true;
            }
            else if (typeof(TKey) == typeof(ulong))
            {
                ref var specificKeys = ref Unsafe.As<TKey, ulong>(ref keys);
                Sort(ref specificKeys, ref values, length, new UInt64DirectComparer());
                return true;
            }
            else if (typeof(TKey) == typeof(float))
            {
                ref var specificKeys = ref Unsafe.As<TKey, float>(ref keys);

                // Comparison to NaN is always false, so do a linear pass 
                // and swap all NaNs to the front of the array
                var left = NaNPrepass(ref specificKeys, ref values, length, new SingleIsNaN());

                ref var afterNaNsKeys = ref Unsafe.Add(ref specificKeys, left);
                ref var afterNaNsValues = ref Unsafe.Add(ref values, left);
                Sort(ref afterNaNsKeys, ref afterNaNsValues, length - left, new SingleDirectComparer());

                return true;
            }
            else if (typeof(TKey) == typeof(double))
            {
                ref var specificKeys = ref Unsafe.As<TKey, double>(ref keys);

                // Comparison to NaN is always false, so do a linear pass 
                // and swap all NaNs to the front of the array
                var left = NaNPrepass(ref specificKeys, ref values, length, new DoubleIsNaN());

                ref var afterNaNsKeys = ref Unsafe.Add(ref specificKeys, left);
                ref var afterNaNsValues = ref Unsafe.Add(ref values, left);
                Sort(ref afterNaNsKeys, ref afterNaNsValues, length - left, new DoubleDirectComparer());

                return true;
            }
            else
            {
                return false;
            }
        }

        // For sorting, move all NaN instances to front of the input array
        private static int NaNPrepass<TKey, TValue, TIsNaN>(
            ref TKey keys, ref TValue values, int length,
            TIsNaN isNaN)
            where TIsNaN : struct, IIsNaN<TKey>
        {
            int left = 0;
            for (int i = 0; i <= length; i++)
            {
                ref TKey current = ref Unsafe.Add(ref keys, i);
                if (isNaN.IsNaN(current))
                {
                    ref TKey previous = ref Unsafe.Add(ref keys, left);

                    Swap(ref previous, ref current);

                    ++left;
                }
            }
            return left;
        }

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
                        // Unfortunately the jit outputs some unnecessary stack stuff
                        // when passing ref values for Void it seems... despite inlining :|
                        Sort3(ref keys, ref values, lo, hi - 1, hi, comparer);
                        return;
                    }
                    // Unfortunately the jit outputs some unnecessary stack stuff
                    // when passing ref values for Void it seems... despite inlining :|
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
            if (typeof(TValue) != typeof(Void))
            {
                Swap(ref values, middle, right);
            }

            while (left < right)
            {
                // TODO: Would be good to be able to update local ref here
                while (comparer.LessThan(Unsafe.Add(ref keys, ++left), pivot)) ;
                // TODO: Would be good to be able to update local ref here
                while (comparer.LessThan(pivot, Unsafe.Add(ref keys, --right))) ;

                if (left >= right)
                    break;

                Swap(ref keys, left, right);
                if (typeof(TValue) != typeof(Void))
                {
                    Swap(ref values, left, right);
                }
            }
            // Put pivot in the right location.
            right = hi - 1;
            if (left != right)
            {
                Swap(ref keys, left, right);
                if (typeof(TValue) != typeof(Void))
                {
                    Swap(ref values, left, right);
                }
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
                if (typeof(TValue) != typeof(Void))
                {
                    Swap(ref values, lo, lo + i - 1);
                }
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

            ref TValue valuesAtLoMinus1 = ref typeof(TValue) != typeof(Void) ? ref Unsafe.Add(ref values, lo - 1) : ref values;

            TKey d = Unsafe.Add(ref keysAtLoMinus1, i);
            TValue dValue = typeof(TValue) != typeof(Void) ? Unsafe.Add(ref valuesAtLoMinus1, i) : default;

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
                if (typeof(TValue) != typeof(Void))
                {
                    Unsafe.Add(ref valuesAtLoMinus1, i) = Unsafe.Add(ref valuesAtLoMinus1, child);
                }

                i = child;
            }
            //keys[lo + i - 1] = d;
            Unsafe.Add(ref keysAtLoMinus1, i) = d;
            if (typeof(TValue) != typeof(Void))
            {
                Unsafe.Add(ref values, lo + i - 1) = dValue;
            }
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
                    var v = typeof(TValue) != typeof(Void) ? Unsafe.Add(ref values, j + 1) : default;
                    do
                    {
                        Unsafe.Add(ref keys, j + 1) = Unsafe.Add(ref keys, j);
                        if (typeof(TValue) != typeof(Void))
                        {
                            Unsafe.Add(ref values, j + 1) = Unsafe.Add(ref values, j);
                        }
                        --j;
                    }
                    while (j >= lo && comparer.LessThan(t, Unsafe.Add(ref keys, j)));

                    Unsafe.Add(ref keys, j + 1) = t;
                    if (typeof(TValue) != typeof(Void))
                    {
                        Unsafe.Add(ref values, j + 1) = v;
                    }
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

            if (comparer.LessThan(r0, r1)) //r0 < r1)
            {
                if (comparer.LessThan(r1, r2)) //(r1 < r2)
                {
                    return ref r1;
                }
                else if (comparer.LessThan(r0, r2)) //(r0 < r2)
                {
                    Swap(ref r1, ref r2);
                    if (typeof(TValue) != typeof(Void))
                    {
                        ref var v1 = ref Unsafe.Add(ref values, i1);
                        ref var v2 = ref Unsafe.Add(ref values, i2);
                        Swap(ref v1, ref v2);
                    }
                }
                else
                {
                    TKey tmp = r0;
                    r0 = r2;
                    r2 = r1;
                    r1 = tmp;
                    if (typeof(TValue) != typeof(Void))
                    {
                        ref var v0 = ref Unsafe.Add(ref values, i0);
                        ref var v1 = ref Unsafe.Add(ref values, i1);
                        ref var v2 = ref Unsafe.Add(ref values, i2);
                        TValue vTemp = v0;
                        v0 = v2;
                        v2 = v1;
                        v1 = vTemp;
                    }
                }
            }
            else
            {
                if (comparer.LessThan(r0, r2)) //(r0 < r2)
                {
                    Swap(ref r0, ref r1);
                    if (typeof(TValue) != typeof(Void))
                    {
                        ref var v0 = ref Unsafe.Add(ref values, i0);
                        ref var v1 = ref Unsafe.Add(ref values, i1);
                        Swap(ref v0, ref v1);
                    }
                }
                else if (comparer.LessThan(r2, r1)) //(r2 < r1)
                {
                    Swap(ref r0, ref r2);
                    if (typeof(TValue) != typeof(Void))
                    {
                        ref var v0 = ref Unsafe.Add(ref values, i0);
                        ref var v2 = ref Unsafe.Add(ref values, i2);
                        Swap(ref v0, ref v2);
                    }
                }
                else
                {
                    TKey tmp = r0;
                    r0 = r1;
                    r1 = r2;
                    r2 = tmp;
                    if (typeof(TValue) != typeof(Void))
                    {
                        ref var v0 = ref Unsafe.Add(ref values, i0);
                        ref var v1 = ref Unsafe.Add(ref values, i1);
                        ref var v2 = ref Unsafe.Add(ref values, i2);
                        TValue vTemp = v0;
                        v0 = v1;
                        v1 = v2;
                        v2 = vTemp;
                    }
                }
            }
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
            if (comparer.LessThan(b, a))
            {
                Swap(ref a, ref b);
                if (typeof(TValue) != typeof(Void))
                {
                    Swap(ref values, i, j);
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void Swap<T>(ref T items, int i, int j)
        {
            Debug.Assert(i != j);
            Swap(ref Unsafe.Add(ref items, i), ref Unsafe.Add(ref items, j));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void Swap<T>(ref T a, ref T b)
        {
            T temp = a;
            a = b;
            b = temp;
        }


        internal static class DefaultSpanSortHelper<TKey, TValue>
        {
            internal static readonly ISpanSortHelper<TKey, TValue> s_default = CreateSortHelper();

            private static ISpanSortHelper<TKey, TValue> CreateSortHelper()
            {
                if (typeof(IComparable<TKey>).IsAssignableFrom(typeof(TKey)))
                {
                    // TODO: Is there a faster way? A way without heap alloc? 
                    // Albeit, this only happens once for each type combination
                    var ctor = typeof(ComparableSpanSortHelper<,>)
                        .MakeGenericType(new Type[] { typeof(TKey), typeof(TValue) })
                        .GetConstructor(Array.Empty<Type>());

                    return (ISpanSortHelper<TKey, TValue>)ctor.Invoke(Array.Empty<object>());
                    // coreclr does the following:
                    //return (IArraySortHelper<T, TComparer>)
                    //    RuntimeTypeHandle.Allocate(
                    //        .TypeHandle.Instantiate());
                }
                else
                {
                    return new SpanSortHelper<TKey, TValue>();
                }
            }
        }

        internal interface ISpanSortHelper<TKey, TValue>
        {
            void Sort(ref TKey keys, ref TValue values, int length);
        }

        internal class SpanSortHelper<TKey, TValue> : ISpanSortHelper<TKey, TValue>
        {
            public void Sort(ref TKey keys, ref TValue values, int length)
            {
                S.Sort(ref keys, ref values, length,
                    new ComparerDirectComparer<TKey, IComparer<TKey>>(Comparer<TKey>.Default));
            }
        }

        internal class ComparableSpanSortHelper<TKey, TValue>
            : ISpanSortHelper<TKey, TValue>
            where TKey : IComparable<TKey>
        {
            public void Sort(ref TKey keys, ref TValue values, int length)
            {
                S.Sort(ref keys, ref values, length,
                    new ComparableDirectComparer<TKey>());
            }
        }


        internal static class DefaultSpanSortHelper<TKey, TValue, TComparer>
            where TComparer : IComparer<TKey>
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
            internal static readonly ISpanSortHelper<TKey, TValue, TComparer> s_default = CreateSortHelper();

            private static ISpanSortHelper<TKey, TValue, TComparer> CreateSortHelper()
            {
                if (typeof(IComparable<TKey>).IsAssignableFrom(typeof(TKey)))
                {
                    // TODO: Is there a faster way? A way without heap alloc? 
                    // Albeit, this only happens once for each type combination
                    var ctor = typeof(ComparableSpanSortHelper<,,>)
                        .MakeGenericType(new Type[] { typeof(TKey), typeof(TValue), typeof(TComparer) })
                        .GetConstructor(Array.Empty<Type>());

                    return (ISpanSortHelper<TKey, TValue, TComparer>)ctor.Invoke(Array.Empty<object>());
                    // coreclr does the following:
                    //return (IArraySortHelper<T, TComparer>)
                    //    RuntimeTypeHandle.Allocate(
                    //        .TypeHandle.Instantiate());
                }
                else
                {
                    return new SpanSortHelper<TKey, TValue, TComparer>();
                }
            }
        }

        // https://github.com/dotnet/coreclr/blob/master/src/mscorlib/src/System/Collections/Generic/ArraySortHelper.cs
        internal interface ISpanSortHelper<TKey, TValue, TComparer>
            where TComparer : IComparer<TKey>
        {
            void Sort(ref TKey keys, ref TValue values, int length, TComparer comparer);
        }

        internal class SpanSortHelper<TKey, TValue, TComparer> : ISpanSortHelper<TKey, TValue, TComparer>
            where TComparer : IComparer<TKey>
        {
            public void Sort(ref TKey keys, ref TValue values, int length, TComparer comparer)
            {
                // Add a try block here to detect IComparers (or their
                // underlying IComparables, etc) that are bogus.
                //
                // TODO: Do we need the try/catch?
                //try
                //{
                if (typeof(TComparer) == typeof(IComparer<TKey>) && comparer == null)
                {
                    S.Sort(ref keys, ref values, length,
                        new ComparerDirectComparer<TKey, IComparer<TKey>>(Comparer<TKey>.Default));
                }
                else
                {
                    S.Sort(ref keys, ref values, length,
                        new ComparerDirectComparer<TKey, IComparer<TKey>>(comparer));
                }
                //}
                //catch (IndexOutOfRangeException e)
                //{
                //    throw e;
                //    //IntrospectiveSortUtilities.ThrowOrIgnoreBadComparer(comparer);
                //}
                //catch (Exception e)
                //{
                //    throw e;
                //    //throw new InvalidOperationException(SR.InvalidOperation_IComparerFailed, e);
                //}
            }
        }

        internal class ComparableSpanSortHelper<TKey, TValue, TComparer>
            : ISpanSortHelper<TKey, TValue, TComparer>
            where TKey : IComparable<TKey>
            where TComparer : IComparer<TKey>
        {
            public void Sort(ref TKey keys, ref TValue values, int length,
                TComparer comparer)
            {
                // Add a try block here to detect IComparers (or their
                // underlying IComparables, etc) that are bogus.
                //
                // TODO: Do we need the try/catch?
                //try
                //{
                if (comparer == null ||
                    // Cache this in generic traits helper class perhaps
                    (!typeof(TComparer).IsValueType &&
                     object.ReferenceEquals(comparer, Comparer<TKey>.Default))) // Or "=="?
                {
                    if (!S.TrySortSpecialized(ref keys, ref values, length))
                    {
                        S.Sort(ref keys, ref values, length,
                            new ComparableDirectComparer<TKey>());
                    }
                }
                else
                {
                    S.Sort(ref keys, ref values, length,
                        new ComparerDirectComparer<TKey, TComparer>(comparer));
                }
                //}
                //catch (IndexOutOfRangeException e)
                //{
                //    throw e;
                //    //IntrospectiveSortUtilities.ThrowOrIgnoreBadComparer(comparer);
                //}
                //catch (Exception e)
                //{
                //    throw e;
                //    //throw new InvalidOperationException(SR.InvalidOperation_IComparerFailed, e);
                //}
            }
        }
    }
}
