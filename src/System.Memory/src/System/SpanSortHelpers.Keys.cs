// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

#if !netstandard
using Internal.Runtime.CompilerServices;
#endif

namespace System
{
    internal static partial class SpanSortHelpersKeys
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void Sort<TKey>(this Span<TKey> keys)
        {
            int length = keys.Length;
            if (length < 2)
                return;

            // PERF: Try specialized here for optimal performance
            // Code-gen is weird unless used in loop outside
            if (!SpanSortHelpersKeys_DirectComparer.TrySortSpecialized(
                ref MemoryMarshal.GetReference(keys),
                length))
            {
                DefaultSpanSortHelper<TKey>.s_default.Sort(
                    ref MemoryMarshal.GetReference(keys),
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

            DefaultSpanSortHelper<TKey, TComparer>.s_default.Sort(
                ref MemoryMarshal.GetReference(keys),
                length, comparer);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void Sort<TKey>(
            this Span<TKey> keys, Comparison<TKey> comparison)
        {
            int length = keys.Length;
            if (length < 2)
                return;

            DefaultSpanSortHelper<TKey>.s_default.Sort(
                ref MemoryMarshal.GetReference(keys),
                length, comparison);
        }

        internal static class DefaultSpanSortHelper<TKey>
        {
            internal static readonly ISpanSortHelper<TKey> s_default = CreateSortHelper();

            private static ISpanSortHelper<TKey> CreateSortHelper()
            {
                if (typeof(IComparable<TKey>).IsAssignableFrom(typeof(TKey)))
                {
                    // coreclr uses RuntimeTypeHandle.Allocate
                    var ctor = typeof(ComparableSpanSortHelper<>)
                        .MakeGenericType(new Type[] { typeof(TKey) })
                        .GetConstructor(Array.Empty<Type>());

                    return (ISpanSortHelper<TKey>)ctor.Invoke(Array.Empty<object>());
                }
                else
                {
                    return new SpanSortHelper<TKey>();
                }
            }
        }

        internal interface ISpanSortHelper<TKey>
        {
            void Sort(ref TKey keys, int length);
            void Sort(ref TKey keys, int length, Comparison<TKey> comparison);
        }

        internal class SpanSortHelper<TKey> : ISpanSortHelper<TKey>
        {
            public void Sort(ref TKey keys, int length)
            {
                SpanSortHelpersKeys_Comparer.Sort(ref keys, length, Comparer<TKey>.Default);
            }

            public void Sort(ref TKey keys, int length, Comparison<TKey> comparison)
            {
                SpanSortHelpersKeys.Sort(ref keys, length, comparison);
            }
        }

        internal class ComparableSpanSortHelper<TKey>
            : ISpanSortHelper<TKey>
            where TKey : IComparable<TKey>
        {
            public void Sort(ref TKey keys, int length)
            {
                SpanSortHelpersKeys.Sort(ref keys, length);
            }

            public void Sort(ref TKey keys, int length, Comparison<TKey> comparison)
            {
                SpanSortHelpersKeys.Sort(ref keys, length, comparison);
            }
        }


        internal static class DefaultSpanSortHelper<TKey, TComparer>
            where TComparer : IComparer<TKey>
        {
            internal static readonly ISpanSortHelper<TKey, TComparer> s_default = CreateSortHelper();

            private static ISpanSortHelper<TKey, TComparer> CreateSortHelper()
            {
                if (typeof(IComparable<TKey>).IsAssignableFrom(typeof(TKey)))
                {
                    // coreclr uses RuntimeTypeHandle.Allocate
                    var ctor = typeof(ComparableSpanSortHelper<,>)
                        .MakeGenericType(new Type[] { typeof(TKey), typeof(TComparer) })
                        .GetConstructor(Array.Empty<Type>());

                    return (ISpanSortHelper<TKey, TComparer>)ctor.Invoke(Array.Empty<object>());
                }
                else
                {
                    return new SpanSortHelper<TKey, TComparer>();
                }
            }
        }

        // https://github.com/dotnet/coreclr/blob/master/src/mscorlib/src/System/Collections/Generic/ArraySortHelper.cs
        internal interface ISpanSortHelper<TKey, TComparer>
            where TComparer : IComparer<TKey>
        {
            void Sort(ref TKey keys, int length, TComparer comparer);
        }

        internal class SpanSortHelper<TKey, TComparer> : ISpanSortHelper<TKey, TComparer>
            where TComparer : IComparer<TKey>
        {
            public void Sort(ref TKey keys, int length, TComparer comparer)
            {
                if (typeof(TComparer) == typeof(IComparer<TKey>) && comparer == null)
                {
                    SpanSortHelpersKeys_Comparer.Sort(ref keys, length, Comparer<TKey>.Default);
                }
                else
                {
                    SpanSortHelpersKeys_Comparer.Sort(ref keys, length, comparer);
                }
            }
        }

        internal class ComparableSpanSortHelper<TKey, TComparer>
            : ISpanSortHelper<TKey, TComparer>
            where TKey : IComparable<TKey>
            where TComparer : IComparer<TKey>
        {
            public void Sort(ref TKey keys, int length,
                TComparer comparer)
            {
                if (comparer == null ||
                    // Cache this in generic traits helper class perhaps
                    (!typeof(TComparer).IsValueType &&
                     object.ReferenceEquals(comparer, Comparer<TKey>.Default))) // Or "=="?
                {
                    if (!SpanSortHelpersKeys_DirectComparer.TrySortSpecialized(ref keys, length))
                    {
                        // NOTE: For Bogus Comparable the exception message will be different, when using Comparer<TKey>.Default
                        //       Since the exception message is thrown internally without knowledge of the comparer
                        SpanSortHelpersKeys.Sort(ref keys, length); 
                    }
                }
                else
                {
                    SpanSortHelpersKeys_Comparer.Sort(ref keys, length, comparer);
                }
            }
        }
    }
}
