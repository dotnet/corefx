// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.CompilerServices;

namespace System.Runtime.InteropServices
{
    /// <summary>
    /// Provides a collection of methods for interoperating with <see cref="Memory{T}"/>, <see cref="ReadOnlyMemory{T}"/>,
    /// <see cref="Span{T}"/>, and <see cref="ReadOnlySpan{T}"/>.
    /// </summary>
    public static partial class MemoryMarshal
    {
        /// <summary>Creates a <see cref="Memory{T}"/> from a <see cref="ReadOnlyMemory{T}"/>.</summary>
        /// <param name="readOnlyMemory">The <see cref="ReadOnlyMemory{T}"/>.</param>
        /// <returns>A <see cref="Memory{T}"/> representing the same memory as the <see cref="ReadOnlyMemory{T}"/>, but writable.</returns>
        /// <remarks>
        /// <see cref="AsMemory{T}(ReadOnlyMemory{T})"/> must be used with extreme caution.  <see cref="ReadOnlyMemory{T}"/> is used
        /// to represent immutable data and other memory that is not meant to be written to; <see cref="Memory{T}"/> instances created
        /// by <see cref="AsMemory{T}(ReadOnlyMemory{T})"/> should not be written to.  The method exists to enable variables typed
        /// as <see cref="Memory{T}"/> but only used for reading to store a <see cref="ReadOnlyMemory{T}"/>.
        /// </remarks>
        public static Memory<T> AsMemory<T>(ReadOnlyMemory<T> readOnlyMemory) =>
            Unsafe.As<ReadOnlyMemory<T>, Memory<T>>(ref readOnlyMemory);

        /// <summary>
        /// Returns a reference to the 0th element of the Span. If the Span is empty, returns a reference to the location where the 0th element
        /// would have been stored. Such a reference can be used for pinning but must never be dereferenced.
        /// </summary>
        public static ref T GetReference<T>(Span<T> span)
        {
            if (span.Pinnable == null)
                unsafe { return ref Unsafe.AsRef<T>(span.ByteOffset.ToPointer()); }
            else
                return ref Unsafe.AddByteOffset<T>(ref span.Pinnable.Data, span.ByteOffset);
        }

        /// <summary>
        /// Returns a reference to the 0th element of the ReadOnlySpan. If the Span is empty, returns a reference to the location where the 0th element
        /// would have been stored. Such a reference can be used for pinning but must never be dereferenced.
        /// </summary>
        public static ref T GetReference<T>(ReadOnlySpan<T> span)
        {
            if (span.Pinnable == null)
                unsafe { return ref Unsafe.AsRef<T>(span.ByteOffset.ToPointer()); }
            else
                return ref Unsafe.AddByteOffset<T>(ref span.Pinnable.Data, span.ByteOffset);
        }

        /// <summary>
        /// Casts a Span of one primitive type <typeparamref name="TFrom"/> to another primitive type <typeparamref name="TTo"/>.
        /// These types may not contain pointers or references. This is checked at runtime in order to preserve type safety.
        /// </summary>
        /// <remarks>
        /// Supported only for platforms that support misaligned memory access.
        /// </remarks>
        /// <param name="source">The source slice, of type <typeparamref name="TFrom"/>.</param>
        /// <exception cref="System.ArgumentException">
        /// Thrown when <typeparamref name="TFrom"/> or <typeparamref name="TTo"/> contains pointers.
        /// </exception>
        public static Span<TTo> Cast<TFrom, TTo>(Span<TFrom> source)
            where TFrom : struct
            where TTo : struct
        {
            if (SpanHelpers.IsReferenceOrContainsReferences<TFrom>())
                ThrowHelper.ThrowArgumentException_InvalidTypeWithPointersNotSupported(typeof(TFrom));

            if (SpanHelpers.IsReferenceOrContainsReferences<TTo>())
                ThrowHelper.ThrowArgumentException_InvalidTypeWithPointersNotSupported(typeof(TTo));

            int newLength = checked((int)((long)source.Length * Unsafe.SizeOf<TFrom>() / Unsafe.SizeOf<TTo>()));
            return new Span<TTo>(Unsafe.As<Pinnable<TTo>>(source.Pinnable), source.ByteOffset, newLength);
        }

        /// <summary>
        /// Casts a ReadOnlySpan of one primitive type <typeparamref name="TFrom"/> to another primitive type <typeparamref name="TTo"/>.
        /// These types may not contain pointers or references. This is checked at runtime in order to preserve type safety.
        /// </summary>
        /// <remarks>
        /// Supported only for platforms that support misaligned memory access.
        /// </remarks>
        /// <param name="source">The source slice, of type <typeparamref name="TFrom"/>.</param>
        /// <exception cref="System.ArgumentException">
        /// Thrown when <typeparamref name="TFrom"/> or <typeparamref name="TTo"/> contains pointers.
        /// </exception>
        public static ReadOnlySpan<TTo> Cast<TFrom, TTo>(ReadOnlySpan<TFrom> source)
            where TFrom : struct
            where TTo : struct
        {
            if (SpanHelpers.IsReferenceOrContainsReferences<TFrom>())
                ThrowHelper.ThrowArgumentException_InvalidTypeWithPointersNotSupported(typeof(TFrom));

            if (SpanHelpers.IsReferenceOrContainsReferences<TTo>())
                ThrowHelper.ThrowArgumentException_InvalidTypeWithPointersNotSupported(typeof(TTo));

            int newLength = checked((int)((long)source.Length * Unsafe.SizeOf<TFrom>() / Unsafe.SizeOf<TTo>()));
            return new ReadOnlySpan<TTo>(Unsafe.As<Pinnable<TTo>>(source.Pinnable), source.ByteOffset, newLength);
        }
    }
}
