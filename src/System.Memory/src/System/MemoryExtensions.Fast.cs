// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Internal.Runtime.CompilerServices;

namespace System
{
    /// <summary>
    /// Extension methods for Span{T}, Memory{T}, and friends.
    /// </summary>
    public static partial class MemoryExtensions
    {
        /// <summary>
        /// Casts a Span of one primitive type <typeparamref name="T"/> to Span of bytes.
        /// That type may not contain pointers or references. This is checked at runtime in order to preserve type safety.
        /// </summary>
        /// <param name="source">The source slice, of type <typeparamref name="T"/>.</param>
        /// <exception cref="System.ArgumentException">
        /// Thrown when <typeparamref name="T"/> contains pointers.
        /// </exception>
        /// <exception cref="System.OverflowException">
        /// Thrown if the Length property of the new Span would exceed Int32.MaxValue.
        /// </exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Span<byte> AsBytes<T>(this Span<T> source) where T : struct => Span.AsBytes(source);

        /// <summary>
        /// Casts a ReadOnlySpan of one primitive type <typeparamref name="T"/> to ReadOnlySpan of bytes.
        /// That type may not contain pointers or references. This is checked at runtime in order to preserve type safety.
        /// </summary>
        /// <param name="source">The source slice, of type <typeparamref name="T"/>.</param>
        /// <exception cref="System.ArgumentException">
        /// Thrown when <typeparamref name="T"/> contains pointers.
        /// </exception>
        /// <exception cref="System.OverflowException">
        /// Thrown if the Length property of the new Span would exceed Int32.MaxValue.
        /// </exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ReadOnlySpan<byte> AsBytes<T>(this ReadOnlySpan<T> source) where T : struct => Span.AsBytes(source);

        /// <summary>
        /// Creates a new readonly span over the portion of the target string.
        /// </summary>
        /// <param name="text">The target string.</param>
        /// <remarks>Returns default when <paramref name="text"/> is null.</remarks>
        public static ReadOnlySpan<char> AsReadOnlySpan(this string text) => Span.AsReadOnlySpan(text);

        /// <summary>
        /// Creates a new readonly span over the portion of the target string.
        /// </summary>
        /// <param name="text">The target string.</param>
        /// <param name="start">The index at which to begin this slice.</param>
        /// <exception cref="System.ArgumentNullException">Thrown when <paramref name="text"/> is null.</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// Thrown when the specified <paramref name="start"/> index is not in range (&lt;0 or &gt;text.Length).
        /// </exception>
        public static ReadOnlySpan<char> AsReadOnlySpan(this string text, int start) => Span.AsReadOnlySpan(text, start);

        /// <summary>
        /// Creates a new readonly span over the portion of the target string.
        /// </summary>
        /// <param name="text">The target string.</param>
        /// <param name="start">The index at which to begin this slice.</param>
        /// <param name="length">The desired length for the slice (exclusive).</param>
        /// <remarks>Returns default when <paramref name="text"/> is null.</remarks>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// Thrown when the specified <paramref name="start"/> index or <paramref name="length"/> is not in range.
        /// </exception>
        public static ReadOnlySpan<char> AsReadOnlySpan(this string text, int start, int length) => Span.AsReadOnlySpan(text, start, length);

        /// <summary>Creates a new <see cref="ReadOnlyMemory{T}"/> over the portion of the target string.</summary>
        /// <param name="text">The target string.</param>
        /// <remarks>Returns default when <paramref name="text"/> is null.</remarks>
        public static ReadOnlyMemory<char> AsReadOnlyMemory(this string text) => Span.AsReadOnlyMemory(text);

        /// <summary>Creates a new <see cref="ReadOnlyMemory{T}"/> over the portion of the target string.</summary>
        /// <param name="text">The target string.</param>
        /// <param name="start">The index at which to begin this slice.</param>
        /// <remarks>Returns default when <paramref name="text"/> is null.</remarks>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// Thrown when the specified <paramref name="start"/> index is not in range (&lt;0 or &gt;text.Length).
        /// </exception>
        public static ReadOnlyMemory<char> AsReadOnlyMemory(this string text, int start) => Span.AsReadOnlyMemory(text, start);

        /// <summary>Creates a new <see cref="ReadOnlyMemory{T}"/> over the portion of the target string.</summary>
        /// <param name="text">The target string.</param>
        /// <param name="start">The index at which to begin this slice.</param>
        /// <param name="length">The desired length for the slice (exclusive).</param>
        /// <remarks>Returns default when <paramref name="text"/> is null.</remarks>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// Thrown when the specified <paramref name="start"/> index or <paramref name="length"/> is not in range.
        /// </exception>
        public static ReadOnlyMemory<char> AsReadOnlyMemory(this string text, int start, int length) => Span.AsReadOnlyMemory(text, start, length);

        /// <summary>Attempts to get the underlying <see cref="string"/> from a <see cref="ReadOnlyMemory{T}"/>.</summary>
        /// <param name="readOnlyMemory">The memory that may be wrapping a <see cref="string"/> object.</param>
        /// <param name="text">The string.</param>
        /// <param name="start">The starting location in <paramref name="text"/>.</param>
        /// <param name="length">The number of items in <paramref name="text"/>.</param>
        /// <returns></returns>
        public static bool TryGetString(this ReadOnlyMemory<char> readOnlyMemory, out string text, out int start, out int length) =>
            Span.TryGetString(readOnlyMemory, out text, out start, out length);

        /// <summary>
        /// Attempts to cast a ReadOnlySpan of one primitive type <typeparamref name="TFrom"/> to another primitive type <typeparamref name="TTo"/>.
        /// </summary>
        /// <param name="source">The source slice, of type <typeparamref name="TFrom"/>.</param>
        /// <param name="output">The destination of type <typeparamref name="TTo"/>.</param>
        /// <remarks>If <typeparamref name="TTo"/> is 8-byte aligned and <paramref name="source"/> points to a valid aligned <typeparamref name="TTo"/> address,
        /// this will always return true. This is because all C# primitives are aligned against at most 8 bytes, so a pointer that is 8 byte aligned 
        /// will be aligned to all primitives.</remarks>
        /// <remarks>If <paramref name="source"/> doesn't point to an address that follows the os-specific alignment rules of <typeparamref name="TFrom"/>, then 
        /// this will always return false</remarks>
        /// <remarks>Does not require the memory being spanned over to be fixed.</remarks>
        /// <returns>True if successful; else False</returns>
        public static bool TryCast<TFrom, TTo>(this ReadOnlySpan<TFrom> source, out ReadOnlySpan<TTo> output) where TFrom : struct where TTo : struct
        {
            if (RuntimeHelpers.IsReferenceOrContainsReferences<TFrom>())
                ThrowHelper.ThrowArgumentException_InvalidTypeWithPointersNotSupported(typeof(TFrom));
            if (RuntimeHelpers.IsReferenceOrContainsReferences<TTo>())
                ThrowHelper.ThrowArgumentException_InvalidTypeWithPointersNotSupported(typeof(TTo));

            if (RuntimeInformation.OSArchitecture == Architecture.X64 ||
                RuntimeInformation.OSArchitecture == Architecture.X86)
            {
                output = MemoryMarshal.Cast<TFrom, TTo>(source);
                return true;
            }
            else
            {
                unsafe
                {
                    // Test that the source pointer is aligned to either the size of the type or the max alignment size (8), whichever is smaller.

                    // This check will return false in the case where TTo is a struct that is 8 bytes long but only 1/2/4-byte aligned and the source pointer
                    // is 1/2/4 byte-aligned but not also 8-byte aligned. Unfortunately, to catch that case we would have to walk the 
                    // struct element sizes to determine the actual struct alignment.

                    // This check also returns false if it's coincidentally aligned to TTo in the case where sizeof(TFrom)<sizeof(TTo). This is
                    // because the source location can move such that it no longer adheres to the alignment requirement of TTo while still
                    // following the alignment rules for it's declared/constructed type, TFrom.
                    void* pointer = Unsafe.AsPointer(ref MemoryMarshal.GetReference(source));

                    // First we have to make sure the given pointer follows the alignment rules of its TFrom type. Since the TFrom alignment will
                    // be preserved even if the memory is moved, we can safely make assumptions about the alignment of TTo without the need for pinning.
                    int sizeOfTFrom = Math.Min(8, Unsafe.SizeOf<TFrom>());
                    if (new IntPtr(pointer).ToInt64() % sizeOfTFrom == 0)
                    {
                        if (sizeOfTFrom == 8 || Math.Min(8, Unsafe.SizeOf<TTo>()) <= sizeOfTFrom)
                        {
                            output = MemoryMarshal.Cast<TFrom, TTo>(source);
                            return true;
                        }
                    }
                    output = null;
                    return false;
                }
            }
        }

        /// <summary>
        /// Attempts to cast a Span of one primitive type <typeparamref name="TFrom"/> to another primitive type <typeparamref name="TTo"/>.
        /// </summary>
        /// <param name="source">The source slice, of type <typeparamref name="TFrom"/>.</param>
        /// <param name="output">The destination  of type <typeparamref name="TTo"/>.</param>
        /// <remarks>If <typeparamref name="TTo"/> is 8-byte aligned and <paramref name="source"/> points to a valid aligned <typeparamref name="TTo"/> address,
        /// this will always return true. This is because all C# primitives are aligned against at most 8 bytes, so a pointer that is 8 byte aligned 
        /// will be aligned to all primitives.</remarks>
        /// <remarks>If <paramref name="source"/> doesn't point to an address that follows the os-specific alignment rules of <typeparamref name="TFrom"/>, then 
        /// this will always return false</remarks>
        /// <remarks>Does not require the memory being spanned over to be fixed.</remarks>
        /// <returns>True if successful; else False</returns>
        public static bool TryCast<TFrom, TTo>(this Span<TFrom> source, out Span<TTo> output) where TFrom : struct where TTo : struct
        {
            if (RuntimeHelpers.IsReferenceOrContainsReferences<TFrom>())
                ThrowHelper.ThrowArgumentException_InvalidTypeWithPointersNotSupported(typeof(TFrom));
            if (RuntimeHelpers.IsReferenceOrContainsReferences<TTo>())
                ThrowHelper.ThrowArgumentException_InvalidTypeWithPointersNotSupported(typeof(TTo));

            output = null;
            if (RuntimeInformation.OSArchitecture == Architecture.X64 ||
                RuntimeInformation.OSArchitecture == Architecture.X86)
            {
                output = MemoryMarshal.Cast<TFrom, TTo>(source);
                return true;
            }
            else
            {
                unsafe
                {
                    // Test that the source pointer is aligned to either the size of TTo or the max alignment size (8), whichever is smaller.

                    // This check will return false in the case where TTo is a struct that is 8 bytes long but only 1/2/4-byte aligned and the source pointer
                    // is 1/2/4 byte-aligned but not also 8-byte aligned. Unfortunately, to catch that case we would have to walk the 
                    // struct element sizes to determine the actual struct alignment.

                    // This check also returns false if it's coincidentally aligned to TTo in the case where sizeof(TFrom)<sizeof(TTo). This is
                    // because the source location can move such that it no longer adheres to the alignment requirement of TTo while still
                    // following the alignment rules for it's declared/constructed type, TFrom.
                    void* pointer = Unsafe.AsPointer(ref MemoryMarshal.GetReference(source));

                    // First we have to make sure the given pointer follows the alignment rules of its TFrom type. Since the TFrom alignment will
                    // be preserved even if the memory is moved, we can safely make assumptions about the alignment of TTo without the need for pinning.
                    int sizeOfTFrom = Math.Min(8, Unsafe.SizeOf<TFrom>());
                    if (new IntPtr(pointer).ToInt64() % sizeOfTFrom == 0)
                    {
                        if (sizeOfTFrom == 8 || Math.Min(8, Unsafe.SizeOf<TTo>()) <= sizeOfTFrom)
                        {
                            output = MemoryMarshal.Cast<TFrom, TTo>(source);
                            return true;
                        }
                    }
                    output = null;
                    return false;
                }
            }
        }
    }
}
