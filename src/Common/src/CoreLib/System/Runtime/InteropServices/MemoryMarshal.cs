// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Buffers;
using System.Runtime.CompilerServices;
using System.Collections.Generic;

namespace System.Runtime.InteropServices
{
    /// <summary>
    /// Provides a collection of methods for interoperating with <see cref="Memory{T}"/>, <see cref="ReadOnlyMemory{T}"/>,
    /// <see cref="Span{T}"/>, and <see cref="ReadOnlySpan{T}"/>.
    /// </summary>
    public static partial class MemoryMarshal
    {
        /// <summary>
        /// Get an array segment from the underlying memory.
        /// If unable to get the array segment, return false with a default array segment.
        /// </summary>
        public static bool TryGetArray<T>(ReadOnlyMemory<T> memory, out ArraySegment<T> segment)
        {
            object obj = memory.GetObjectStartLength(out int index, out int length);
            if (index < 0)
            {
                if (((OwnedMemory<T>)obj).TryGetArray(out ArraySegment<T> arraySegment))
                {
                    segment = new ArraySegment<T>(arraySegment.Array, arraySegment.Offset + (index & ReadOnlyMemory<T>.RemoveOwnedFlagBitMask), length);
                    return true;
                }
            }
            else if (obj is T[] arr)
            {
                segment = new ArraySegment<T>(arr, index, length);
                return true;
            }

            if (length == 0)
            {
#if FEATURE_PORTABLE_SPAN
                segment = new ArraySegment<T>(SpanHelpers.PerTypeValues<T>.EmptyArray);
#else
                segment = ArraySegment<T>.Empty;
#endif // FEATURE_PORTABLE_SPAN
                return true;
            }

            segment = default;
            return false;
        }

        /// <summary>
        /// Gets an <see cref="OwnedMemory{T}"/> from the underlying read-only memory.
        /// If unable to get the <typeparamref name="TOwner"/> type, returns false.
        /// </summary>
        /// <typeparam name="T">The element type of the <paramref name="memory" />.</typeparam>
        /// <typeparam name="TOwner">The type of <see cref="OwnedMemory{T}"/> to try and retrive.</typeparam>
        /// <param name="memory">The memory to get the owner for.</param>
        /// <param name="owner">The returned owner of the <see cref="ReadOnlyMemory{T}"/>.</param>
        /// <returns>A <see cref="bool"/> indicating if it was successful.</returns>
        public static bool TryGetOwnedMemory<T, TOwner>(ReadOnlyMemory<T> memory, out TOwner owner)
            where TOwner : OwnedMemory<T>
        {
            TOwner localOwner; // Use register for null comparison rather than byref
            owner = localOwner = memory.GetObjectStartLength(out int index, out int length) as TOwner;
            return !ReferenceEquals(owner, null);
        }

        /// <summary>
        /// Gets an <see cref="OwnedMemory{T}"/> and <paramref name="start" />, <paramref name="length" /> from the underlying read-only memory.
        /// If unable to get the <typeparamref name="TOwner"/> type, returns false.
        /// </summary>
        /// <typeparam name="T">The element type of the <paramref name="memory" />.</typeparam>
        /// <typeparam name="TOwner">The type of <see cref="OwnedMemory{T}"/> to try and retrive.</typeparam>
        /// <param name="memory">The memory to get the owner for.</param>
        /// <param name="owner">The returned owner of the <see cref="ReadOnlyMemory{T}"/>.</param>
        /// <param name="start">The offset from the start of the <paramref name="owner" /> that the <paramref name="memory" /> represents.</param>
        /// <param name="length">The length of the <paramref name="owner" /> that the <paramref name="memory" /> represents.</param>
        /// <returns>A <see cref="bool"/> indicating if it was successful.</returns>
        public static bool TryGetOwnedMemory<T, TOwner>(ReadOnlyMemory<T> memory, out TOwner owner, out int start, out int length)
           where TOwner : OwnedMemory<T>
        {
            TOwner localOwner; // Use register for null comparison rather than byref
            owner = localOwner = memory.GetObjectStartLength(out start, out length) as TOwner;
            start &= ReadOnlyMemory<T>.RemoveOwnedFlagBitMask;
            return !ReferenceEquals(owner, null);
        }

        /// <summary>
        /// Creates an <see cref="IEnumerable{T}"/> view of the given <paramref name="memory" /> to allow
        /// the <paramref name="memory" /> to be used in existing APIs that take an <see cref="IEnumerable{T}"/>.
        /// </summary>
        /// <typeparam name="T">The element type of the <paramref name="memory" />.</typeparam>
        /// <param name="memory">The ReadOnlyMemory to view as an <see cref="IEnumerable{T}"/></param>
        /// <returns>An <see cref="IEnumerable{T}"/> view of the given <paramref name="memory" /></returns>
        public static IEnumerable<T> ToEnumerable<T>(ReadOnlyMemory<T> memory)
        {
            for (int i = 0; i < memory.Length; i++)
                yield return memory.Span[i];
        }

        /// <summary>Attempts to get the underlying <see cref="string"/> from a <see cref="ReadOnlyMemory{T}"/>.</summary>
        /// <param name="memory">The memory that may be wrapping a <see cref="string"/> object.</param>
        /// <param name="text">The string.</param>
        /// <param name="start">The starting location in <paramref name="text"/>.</param>
        /// <param name="length">The number of items in <paramref name="text"/>.</param>
        /// <returns></returns>
        public static bool TryGetString(ReadOnlyMemory<char> memory, out string text, out int start, out int length)
        {
            if (memory.GetObjectStartLength(out int offset, out int count) is string s)
            {
                text = s;
                start = offset;
                length = count;
                return true;
            }
            else
            {
                text = null;
                start = 0;
                length = 0;
                return false;
            }
        }
    }
}
