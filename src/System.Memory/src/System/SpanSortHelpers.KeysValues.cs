// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

#if !netstandard
using Internal.Runtime.CompilerServices;
#endif

using static System.SpanSortHelpersCommon;
using S = System.SpanSortHelpersKeysValues;
using SDC = System.SpanSortHelpersKeysValues_DirectComparer;

namespace System
{
    internal static partial class SpanSortHelpersKeysValues
    {
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
            if (!SDC.TrySortSpecialized(
                ref MemoryMarshal.GetReference(keys),
                ref MemoryMarshal.GetReference(values),
                length))
            {
                DefaultSpanSortHelper<TKey, TValue>.s_default.Sort(
                    ref MemoryMarshal.GetReference(keys),
                    ref MemoryMarshal.GetReference(values),
                    length);
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
                ref MemoryMarshal.GetReference(keys),
                ref MemoryMarshal.GetReference(values),
                length, comparer);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void Sort<TKey, TValue>(
            this Span<TKey> keys, Span<TValue> values, Comparison<TKey> comparison)
        {
            int length = keys.Length;
            if (length != values.Length)
                ThrowHelper.ThrowArgumentException_ItemsMustHaveSameLength();
            if (length < 2)
                return;

            DefaultSpanSortHelper<TKey, TValue>.s_default.Sort(
                ref MemoryMarshal.GetReference(keys),
                ref MemoryMarshal.GetReference(values),
                length, comparison);
        }

        internal static class DefaultSpanSortHelper<TKey, TValue>
        {
            internal static readonly ISpanSortHelper<TKey, TValue> s_default = CreateSortHelper();

            private static ISpanSortHelper<TKey, TValue> CreateSortHelper()
            {
                if (typeof(IComparable<TKey>).IsAssignableFrom(typeof(TKey)))
                {
                    // coreclr uses RuntimeTypeHandle.Allocate
                    var ctor = typeof(ComparableSpanSortHelper<,>)
                        .MakeGenericType(new Type[] { typeof(TKey), typeof(TValue) })
                        .GetConstructor(Array.Empty<Type>());

                    return (ISpanSortHelper<TKey, TValue>)ctor.Invoke(Array.Empty<object>());
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
            void Sort(ref TKey keys, ref TValue values, int length, Comparison<TKey> comparison);
        }

        internal class SpanSortHelper<TKey, TValue> : ISpanSortHelper<TKey, TValue>
        {
            public void Sort(ref TKey keys, ref TValue values, int length)
            {
                S.Sort(ref keys, ref values, length, Comparer<TKey>.Default);
            }

            public void Sort(ref TKey keys, ref TValue values, int length, Comparison<TKey> comparison)
            {
                S.Sort(ref keys, ref values, length, comparison);
            }
        }

        internal class ComparableSpanSortHelper<TKey, TValue>
            : ISpanSortHelper<TKey, TValue>
            where TKey : IComparable<TKey>
        {
            public void Sort(ref TKey keys, ref TValue values, int length)
            {
                S.Sort(ref keys, ref values, length);
            }

            public void Sort(ref TKey keys, ref TValue values, int length, Comparison<TKey> comparison)
            {
                // TODO: Check if comparison is Comparer<TKey>.Default.Compare

                S.Sort(ref keys, ref values, length, comparison);
            }
        }


        internal static class DefaultSpanSortHelper<TKey, TValue, TComparer>
            where TComparer : IComparer<TKey>
        {
            internal static readonly ISpanSortHelper<TKey, TValue, TComparer> s_default = CreateSortHelper();

            private static ISpanSortHelper<TKey, TValue, TComparer> CreateSortHelper()
            {
                if (typeof(IComparable<TKey>).IsAssignableFrom(typeof(TKey)))
                {
                    // coreclr uses RuntimeTypeHandle.Allocate
                    var ctor = typeof(ComparableSpanSortHelper<,,>)
                        .MakeGenericType(new Type[] { typeof(TKey), typeof(TValue), typeof(TComparer) })
                        .GetConstructor(Array.Empty<Type>());

                    return (ISpanSortHelper<TKey, TValue, TComparer>)ctor.Invoke(Array.Empty<object>());
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
                    S.Sort(ref keys, ref values, length, Comparer<TKey>.Default);
                }
                else
                {
                    S.Sort(ref keys, ref values, length, comparer);
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
                    if (!SDC.TrySortSpecialized(ref keys, ref values, length))
                    {
                        // NOTE: For Bogus Comparable the exception message will be different, when using Comparer<TKey>.Default
                        //       Since the exception message is thrown internally without knowledge of the comparer
                        S.Sort(ref keys, ref values, length);
                    }
                }
                else
                {
                    S.Sort(ref keys, ref values, length, comparer);
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
