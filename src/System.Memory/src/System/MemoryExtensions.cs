// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace System
{
    /// <summary>
    /// Extension methods for Span{T}, Memory{T}, and friends.
    /// </summary>
    public static partial class MemoryExtensions
    {
        /// <summary>
        /// Searches for the specified value and returns the index of its first occurrence. If not found, returns -1. Values are compared using IEquatable{T}.Equals(T). 
        /// </summary>
        /// <param name="span">The span to search.</param>
        /// <param name="value">The value to search for.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int IndexOf<T>(this Span<T> span, T value)
            where T : IEquatable<T>
        {
            if (typeof(T) == typeof(byte)) 
                return SpanHelpers.IndexOf(
                    ref Unsafe.As<T, byte>(ref span.DangerousGetPinnableReference()), 
                    Unsafe.As<T, byte>(ref value), 
                    span.Length);
            return SpanHelpers.IndexOf<T>(ref span.DangerousGetPinnableReference(), value, span.Length);
        }

        /// <summary>
        /// Searches for the specified sequence and returns the index of its first occurrence. If not found, returns -1. Values are compared using IEquatable{T}.Equals(T). 
        /// </summary>
        /// <param name="span">The span to search.</param>
        /// <param name="value">The sequence to search for.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int IndexOf<T>(this Span<T> span, ReadOnlySpan<T> value)
            where T : IEquatable<T>
        {
            if (typeof(T) == typeof(byte)) 
                return SpanHelpers.IndexOf(
                    ref Unsafe.As<T, byte>(ref span.DangerousGetPinnableReference()), 
                    span.Length, 
                    ref Unsafe.As<T, byte>(ref value.DangerousGetPinnableReference()), 
                    value.Length);
            return SpanHelpers.IndexOf<T>(ref span.DangerousGetPinnableReference(), span.Length, ref value.DangerousGetPinnableReference(), value.Length);
        }

        /// <summary>
        /// Determines whether two sequences are equal by comparing the elements using IEquatable{T}.Equals(T). 
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SequenceEqual<T>(this Span<T> first, ReadOnlySpan<T> second)
            where T : IEquatable<T>
        {
            int length = first.Length;
            if (typeof(T) == typeof(byte))
                return length == second.Length &&
                SpanHelpers.SequenceEqual(
                    ref Unsafe.As<T, byte>(ref first.DangerousGetPinnableReference()),
                    ref Unsafe.As<T, byte>(ref second.DangerousGetPinnableReference()),
                    length);
            return length == second.Length && SpanHelpers.SequenceEqual(ref first.DangerousGetPinnableReference(), ref second.DangerousGetPinnableReference(), length);
        }

        /// <summary>
        /// Searches for the specified value and returns the index of its first occurrence. If not found, returns -1. Values are compared using IEquatable{T}.Equals(T). 
        /// </summary>
        /// <param name="span">The span to search.</param>
        /// <param name="value">The value to search for.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int IndexOf<T>(this ReadOnlySpan<T> span, T value)
            where T : IEquatable<T>
        {
            if (typeof(T) == typeof(byte)) 
                return SpanHelpers.IndexOf(
                    ref Unsafe.As<T, byte>(ref span.DangerousGetPinnableReference()), 
                    Unsafe.As<T, byte>(ref value), 
                    span.Length);
            return SpanHelpers.IndexOf<T>(ref span.DangerousGetPinnableReference(), value, span.Length);
        }

        /// <summary>
        /// Searches for the specified sequence and returns the index of its first occurrence. If not found, returns -1. Values are compared using IEquatable{T}.Equals(T). 
        /// </summary>
        /// <param name="span">The span to search.</param>
        /// <param name="value">The sequence to search for.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int IndexOf<T>(this ReadOnlySpan<T> span, ReadOnlySpan<T> value)
            where T : IEquatable<T>
        {
            if (typeof(T) == typeof(byte)) 
                return SpanHelpers.IndexOf(
                    ref Unsafe.As<T, byte>(ref span.DangerousGetPinnableReference()), 
                    span.Length, 
                    ref Unsafe.As<T, byte>(ref value.DangerousGetPinnableReference()), 
                    value.Length);
            return SpanHelpers.IndexOf<T>(ref span.DangerousGetPinnableReference(), span.Length, ref value.DangerousGetPinnableReference(), value.Length);
        }

        /// <summary>
        /// Searches for the first index of any of the specified values similar to calling IndexOf several times with the logical OR operator. If not found, returns -1.
        /// </summary>
        /// <param name="span">The span to search.</param>
        /// <param name="value0">One of the values to search for.</param>
        /// <param name="value1">One of the values to search for.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int IndexOfAny(this Span<byte> span, byte value0, byte value1)
        {
            return SpanHelpers.IndexOfAny(ref span.DangerousGetPinnableReference(), value0, value1, span.Length);
        }

        /// <summary>
        /// Searches for the first index of any of the specified values similar to calling IndexOf several times with the logical OR operator. If not found, returns -1.
        /// </summary>
        /// <param name="span">The span to search.</param>
        /// <param name="value0">One of the values to search for.</param>
        /// <param name="value1">One of the values to search for.</param>
        /// <param name="value2">One of the values to search for.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int IndexOfAny(this Span<byte> span, byte value0, byte value1, byte value2)
        {
            return SpanHelpers.IndexOfAny(ref span.DangerousGetPinnableReference(), value0, value1, value2, span.Length);
        }

        /// <summary>
        /// Searches for the first index of any of the specified values similar to calling IndexOf several times with the logical OR operator. If not found, returns -1. 
        /// </summary>
        /// <param name="span">The span to search.</param>
        /// <param name="values">The set of values to search for.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int IndexOfAny(this Span<byte> span, ReadOnlySpan<byte> values)
        {
            return SpanHelpers.IndexOfAny(ref span.DangerousGetPinnableReference(), span.Length, ref values.DangerousGetPinnableReference(), values.Length);
        }

        /// <summary>
        /// Searches for the first index of any of the specified values similar to calling IndexOf several times with the logical OR operator. If not found, returns -1.
        /// </summary>
        /// <param name="span">The span to search.</param>
        /// <param name="value0">One of the values to search for.</param>
        /// <param name="value1">One of the values to search for.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int IndexOfAny(this ReadOnlySpan<byte> span, byte value0, byte value1)
        {
            return SpanHelpers.IndexOfAny(ref span.DangerousGetPinnableReference(), value0, value1, span.Length);
        }

        /// <summary>
        /// Searches for the first index of any of the specified values similar to calling IndexOf several times with the logical OR operator. If not found, returns -1. 
        /// </summary>
        /// <param name="span">The span to search.</param>
        /// <param name="value0">One of the values to search for.</param>
        /// <param name="value1">One of the values to search for.</param>
        /// <param name="value2">One of the values to search for.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int IndexOfAny(this ReadOnlySpan<byte> span, byte value0, byte value1, byte value2)
        {
            return SpanHelpers.IndexOfAny(ref span.DangerousGetPinnableReference(), value0, value1, value2, span.Length);
        }

        /// <summary>
        /// Searches for the first index of any of the specified values similar to calling IndexOf several times with the logical OR operator. If not found, returns -1. 
        /// </summary>
        /// <param name="span">The span to search.</param>
        /// <param name="values">The set of values to search for.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int IndexOfAny(this ReadOnlySpan<byte> span, ReadOnlySpan<byte> values)
        {
            return SpanHelpers.IndexOfAny(ref span.DangerousGetPinnableReference(), span.Length, ref values.DangerousGetPinnableReference(), values.Length);
        }

        /// <summary>
        /// Determines whether two sequences are equal by comparing the elements using IEquatable{T}.Equals(T). 
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SequenceEqual<T>(this ReadOnlySpan<T> first, ReadOnlySpan<T> second)
            where T : IEquatable<T>
        {
            int length = first.Length;
            if (typeof(T) == typeof(byte))
                return length == second.Length &&
                SpanHelpers.SequenceEqual(
                    ref Unsafe.As<T, byte>(ref first.DangerousGetPinnableReference()),
                    ref Unsafe.As<T, byte>(ref second.DangerousGetPinnableReference()),
                    length);
            return length == second.Length && SpanHelpers.SequenceEqual(ref first.DangerousGetPinnableReference(), ref second.DangerousGetPinnableReference(), length);
        }

        /// <summary>
        /// Determines whether the specified sequence appears at the start of the span.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool StartsWith<T>(this Span<T> span, ReadOnlySpan<T> value)
            where T : IEquatable<T>
        {
            int valueLength = value.Length;
            if (typeof(T) == typeof(byte))
                return valueLength <= span.Length && 
                SpanHelpers.SequenceEqual(
                    ref Unsafe.As<T, byte>(ref span.DangerousGetPinnableReference()),
                    ref Unsafe.As<T, byte>(ref value.DangerousGetPinnableReference()),
                    valueLength);
            return valueLength <= span.Length && SpanHelpers.SequenceEqual(ref span.DangerousGetPinnableReference(), ref value.DangerousGetPinnableReference(), valueLength);
        }

        /// <summary>
        /// Determines whether the specified sequence appears at the start of the span.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool StartsWith<T>(this ReadOnlySpan<T> span, ReadOnlySpan<T> value)
            where T : IEquatable<T>
        {
            int valueLength = value.Length;
            if (typeof(T) == typeof(byte))
                return valueLength <= span.Length && 
                SpanHelpers.SequenceEqual(
                    ref Unsafe.As<T, byte>(ref span.DangerousGetPinnableReference()),
                    ref Unsafe.As<T, byte>(ref value.DangerousGetPinnableReference()),
                    valueLength);
            return valueLength <= span.Length && SpanHelpers.SequenceEqual(ref span.DangerousGetPinnableReference(), ref value.DangerousGetPinnableReference(), valueLength);
        }

        /// <summary>
        /// Creates a new  span over the portion of the target array.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Span<T> AsSpan<T>(this T[] array)
        {
            return new Span<T>(array);
        }

        /// <summary>
        /// Creates a new  span over the portion of the target array segment.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Span<T> AsSpan<T>(this ArraySegment<T> arraySegment)
        {
            return new Span<T>(arraySegment.Array, arraySegment.Offset, arraySegment.Count);
        }

        /// <summary>
        /// Creates a new readonly span over the entire target array.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ReadOnlySpan<T> AsReadOnlySpan<T>(this T[] array)
        {
            return new ReadOnlySpan<T>(array);
        }

        /// <summary>
        /// Creates a new readonly span over the target array segment.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ReadOnlySpan<T> AsReadOnlySpan<T>(this ArraySegment<T> arraySegment)
        {
            return new ReadOnlySpan<T>(arraySegment.Array, arraySegment.Offset, arraySegment.Count);
        }

        /// <summary>
        /// Copies the contents of the array into the span. If the source
        /// and destinations overlap, this method behaves as if the original values in
        /// a temporary location before the destination is overwritten.
        /// 
        ///<param name="array">The array to copy items from.</param>
        /// <param name="destination">The span to copy items into.</param>
        /// <exception cref="System.ArgumentException">
        /// Thrown when the destination Span is shorter than the source array.
        /// </exception>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CopyTo<T>(this T[] array, Span<T> destination)
        {
            new ReadOnlySpan<T>(array).CopyTo(destination);
        }

        /// <summary>
        /// Copies the contents of the array into the memory. If the source
        /// and destinations overlap, this method behaves as if the original values are in
        /// a temporary location before the destination is overwritten.
        /// 
        ///<param name="array">The array to copy items from.</param>
        /// <param name="destination">The memory to copy items into.</param>
        /// <exception cref="System.ArgumentException">
        /// Thrown when the destination is shorter than the source array.
        /// </exception>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CopyTo<T>(this T[] array, Memory<T> destination)
        {
            array.CopyTo(destination.Span);
        }

        /// Determines whether two sequences overlap.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Overlaps<T>(this Span<T> first, ReadOnlySpan<T> second)
        {
            return Overlaps((ReadOnlySpan<T>)first, second);
        }

        /// <summary>
        /// Determines whether two sequences overlap and outputs the element offset.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Overlaps<T>(this Span<T> first, ReadOnlySpan<T> second, out int elementOffset)
        {
            return Overlaps((ReadOnlySpan<T>)first, second, out elementOffset);
        }

        /// <summary>
        /// Determines whether two sequences overlap.
        /// </summary>
        public static bool Overlaps<T>(this ReadOnlySpan<T> first, ReadOnlySpan<T> second)
        {
            // note: the calculations below overflow if second.IsEmpty;
            //       the case second.IsEmpty MUST be handled separately
            if (first.IsEmpty || second.IsEmpty)
            {
                return false;
            }

            IntPtr byteOffset = Unsafe.ByteOffset(
                ref first.DangerousGetPinnableReference(),
                ref second.DangerousGetPinnableReference());

            if (Unsafe.SizeOf<IntPtr>() == sizeof(int))
            {
                return (uint)byteOffset < (uint)(first.Length * Unsafe.SizeOf<T>()) ||
                       (uint)byteOffset > (uint)-(second.Length * Unsafe.SizeOf<T>());
            }
            else
            {
                return (ulong)byteOffset < (ulong)((long)first.Length * Unsafe.SizeOf<T>()) ||
                       (ulong)byteOffset > (ulong)-((long)second.Length * Unsafe.SizeOf<T>());
            }
        }

        /// <summary>
        /// Determines whether two sequences overlap and outputs the element offset.
        /// </summary>
        public static bool Overlaps<T>(this ReadOnlySpan<T> first, ReadOnlySpan<T> second, out int elementOffset)
        {
            // note: the calculations below overflow if second.IsEmpty;
            //       the case second.IsEmpty MUST be handled separately
            if (first.IsEmpty || second.IsEmpty)
            {
                elementOffset = 0;
                return false;
            }

            IntPtr byteOffset = Unsafe.ByteOffset(
                ref first.DangerousGetPinnableReference(),
                ref second.DangerousGetPinnableReference());

            if (Unsafe.SizeOf<IntPtr>() == sizeof(int))
            {
                if ((uint)byteOffset < (uint)(first.Length * Unsafe.SizeOf<T>()) ||
                    (uint)byteOffset > (uint)-(second.Length * Unsafe.SizeOf<T>()))
                {
                    if ((int)byteOffset % Unsafe.SizeOf<T>() != 0)
                        ThrowHelper.ThrowArgumentException_OverlapAlignmentMismatch();

                    elementOffset = (int)byteOffset / Unsafe.SizeOf<T>();
                    return true;
                }
                else
                {
                    elementOffset = 0;
                    return false;
                }
            }
            else
            {
                if ((ulong)byteOffset < (ulong)((long)first.Length * Unsafe.SizeOf<T>()) ||
                    (ulong)byteOffset > (ulong)-((long)second.Length * Unsafe.SizeOf<T>()))
                {
                    if ((long)byteOffset % Unsafe.SizeOf<T>() != 0)
                        ThrowHelper.ThrowArgumentException_OverlapAlignmentMismatch();

                    elementOffset = (int)((long)byteOffset / Unsafe.SizeOf<T>());
                    return true;
                }
                else
                {
                    elementOffset = 0;
                    return false;
                }
            }
        }
    }
}
