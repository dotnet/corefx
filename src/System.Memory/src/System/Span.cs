// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Runtime.CompilerServices;
using EditorBrowsableState = System.ComponentModel.EditorBrowsableState;
using EditorBrowsableAttribute = System.ComponentModel.EditorBrowsableAttribute;

#pragma warning disable 0809  //warning CS0809: Obsolete member 'Span<T>.Equals(object)' overrides non-obsolete member 'object.Equals(object)'

namespace System
{
    /// <summary>
    /// Span represents a contiguous region of arbitrary memory. Unlike arrays, it can point to either managed
    /// or native memory, or to memory allocated on the stack. It is type- and memory-safe.
    /// </summary>
    public struct Span<T>
    {
        /// <summary>
        /// Creates a new span over the entirety of the target array.
        /// </summary>
        /// <param name="array">The target array.</param>
        /// <exception cref="System.ArgumentNullException">Thrown when <paramref name="array"/> is a null
        /// reference (Nothing in Visual Basic).</exception>
        /// <exception cref="System.ArrayTypeMismatchException">Thrown when <paramref name="array"/> is covariant and array's type is not exactly T[].</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Span(T[] array)
        {
            if (array == null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.array);
            if (default(T) == null && array.GetType() != typeof(T[]))
                ThrowHelper.ThrowArrayTypeMismatchException_ArrayTypeMustBeExactMatch(typeof(T));

            _length = array.Length;
            _pinnable = Unsafe.As<Pinnable<T>>(array);
            _byteOffset = SpanHelpers.PerTypeValues<T>.ArrayAdjustment;
        }

        /// <summary>
        /// Creates a new span over the portion of the target array beginning
        /// at 'start' index and covering the remainder of the array.
        /// </summary>
        /// <param name="array">The target array.</param>
        /// <param name="start">The index at which to begin the span.</param>
        /// <exception cref="System.ArgumentNullException">Thrown when <paramref name="array"/> is a null
        /// reference (Nothing in Visual Basic).</exception>
        /// <exception cref="System.ArrayTypeMismatchException">Thrown when <paramref name="array"/> is covariant and array's type is not exactly T[].</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// Thrown when the specified <paramref name="start"/> is not in the range (&lt;0 or &gt;=Length).
        /// </exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Span(T[] array, int start)
        {
            if (array == null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.array);
            if (default(T) == null && array.GetType() != typeof(T[]))
                ThrowHelper.ThrowArrayTypeMismatchException_ArrayTypeMustBeExactMatch(typeof(T));

            int arrayLength = array.Length;
            if ((uint)start > (uint)arrayLength)
                ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.start);

            _length = arrayLength - start;
            _pinnable = Unsafe.As<Pinnable<T>>(array);
            _byteOffset = SpanHelpers.PerTypeValues<T>.ArrayAdjustment.Add<T>(start);
        }

        /// <summary>
        /// Creates a new span over the portion of the target array beginning
        /// at 'start' index and ending at 'end' index (exclusive).
        /// </summary>
        /// <param name="array">The target array.</param>
        /// <param name="start">The index at which to begin the span.</param>
        /// <param name="length">The number of items in the span.</param>
        /// <exception cref="System.ArgumentNullException">Thrown when <paramref name="array"/> is a null
        /// reference (Nothing in Visual Basic).</exception>
        /// <exception cref="System.ArrayTypeMismatchException">Thrown when <paramref name="array"/> is covariant and array's type is not exactly T[].</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// Thrown when the specified <paramref name="start"/> or end index is not in the range (&lt;0 or &gt;=Length).
        /// </exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Span(T[] array, int start, int length)
        {
            if (array == null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.array);
            if (default(T) == null && array.GetType() != typeof(T[]))
                ThrowHelper.ThrowArrayTypeMismatchException_ArrayTypeMustBeExactMatch(typeof(T));
            if ((uint)start > (uint)array.Length || (uint)length > (uint)(array.Length - start))
                ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.start);

            _length = length;
            _pinnable = Unsafe.As<Pinnable<T>>(array);
            _byteOffset = SpanHelpers.PerTypeValues<T>.ArrayAdjustment.Add<T>(start);
        }

        /// <summary>
        /// Creates a new span over the target unmanaged buffer.  Clearly this
        /// is quite dangerous, because we are creating arbitrarily typed T's
        /// out of a void*-typed block of memory.  And the length is not checked.
        /// But if this creation is correct, then all subsequent uses are correct.
        /// </summary>
        /// <param name="pointer">An unmanaged pointer to memory.</param>
        /// <param name="length">The number of <typeparamref name="T"/> elements the memory contains.</param>
        /// <exception cref="System.ArgumentException">
        /// Thrown when <typeparamref name="T"/> is reference type or contains pointers and hence cannot be stored in unmanaged memory.
        /// </exception>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// Thrown when the specified <paramref name="length"/> is negative.
        /// </exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe Span(void* pointer, int length)
        {
            if (!SpanHelpers.IsReferenceFree<T>())
                ThrowHelper.ThrowArgumentException_InvalidTypeWithPointersNotSupported(typeof(T));
            if (length < 0)
                ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.start);

            _length = length;
            _pinnable = null;
            _byteOffset = new IntPtr(pointer);
        }

        /// <summary>
        /// Create a new span over a portion of a regular managed object. This can be useful
        /// if part of a managed object represents a "fixed array." This is dangerous because
        /// "length" is not checked, nor is the fact that "rawPointer" actually lies within the object.
        /// </summary>
        /// <param name="obj">The managed object that contains the data to span over.</param>
        /// <param name="objectData">A reference to data within that object.</param>
        /// <param name="length">The number of <typeparamref name="T"/> elements the memory contains.</param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when the specified object is null.
        /// </exception>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// Thrown when the specified <paramref name="length"/> is negative.
        /// </exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Span<T> DangerousCreate(object obj, ref T objectData, int length)
        {
            if (obj == null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.obj);
            if (length < 0)
                ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.length);

            Pinnable<T> pinnable = Unsafe.As<Pinnable<T>>(obj);
            IntPtr byteOffset = Unsafe.ByteOffset<T>(ref pinnable.Data, ref objectData);
            return new Span<T>(pinnable, byteOffset, length);
        }

        // Constructor for internal use only.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private Span(Pinnable<T> pinnable, IntPtr byteOffset, int length)
        {
            Debug.Assert(length >= 0);

            _length = length;
            _pinnable = pinnable;
            _byteOffset = byteOffset;
        }

        /// <summary>
        /// The number of items in the span.
        /// </summary>
        public int Length => _length;

        /// <summary>
        /// Returns true if Length is 0.
        /// </summary>
        public bool IsEmpty => _length == 0;

        /// <summary>
        /// Returns a reference to specified element of the Span.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        /// <exception cref="System.IndexOutOfRangeException">
        /// Thrown when index less than 0 or index greater than or equal to Length
        /// </exception>

        // TODO: https://github.com/dotnet/corefx/issues/13681
        //   Until we get over the hurdle of C# 7 tooling, this indexer will return "T" and have a setter rather than a "ref T". (The doc comments
        //   continue to reflect the original intent of returning "ref T")
        public T this[int index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                if ((uint)index >= ((uint)_length))
                    ThrowHelper.ThrowIndexOutOfRangeException();

                if (_pinnable == null)
                    unsafe { return Unsafe.Add<T>(ref Unsafe.AsRef<T>(_byteOffset.ToPointer()), index); }
                else
                    return Unsafe.Add<T>(ref Unsafe.AddByteOffset<T>(ref _pinnable.Data, _byteOffset), index);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set
            {
                if ((uint) index >= ((uint) _length))
                    ThrowHelper.ThrowIndexOutOfRangeException();

                if (_pinnable == null)
                    unsafe { Unsafe.Add<T>(ref Unsafe.AsRef<T>(_byteOffset.ToPointer()), index) = value; }
                else
                    Unsafe.Add<T>(ref Unsafe.AddByteOffset<T>(ref _pinnable.Data, _byteOffset), index) = value;
            }
    }

        /// <summary>
        /// Returns a reference to specified element of the Span.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        /// <exception cref="System.IndexOutOfRangeException">
        /// Thrown when index less than 0 or index greater than or equal to Length
        /// </exception>

        // TODO: https://github.com/dotnet/corefx/issues/13681
        //   Until we get over the hurdle of C# 7 tooling, this temporary method will simulate the intended "ref T" indexer for those
        //   who need bypass the workaround for performance.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref T GetItem(int index)
        {
            if ((uint)index >= ((uint)_length))
                ThrowHelper.ThrowIndexOutOfRangeException();

            if (_pinnable == null)
                unsafe { return ref Unsafe.Add<T>(ref Unsafe.AsRef<T>(_byteOffset.ToPointer()), index); }
            else
                return ref Unsafe.Add<T>(ref Unsafe.AddByteOffset<T>(ref _pinnable.Data, _byteOffset), index);
        }

        /// <summary>
        /// Copies the contents of this span into destination span. If the source
        /// and destinations overlap, this method behaves as if the original values in
        /// a temporary location before the destination is overwritten.
        ///
        /// <param name="destination">The span to copy items into.</param>
        /// <exception cref="System.ArgumentException">
        /// Thrown when the destination Span is shorter than the source Span.
        /// </exception>
        /// </summary>
        public void CopyTo(Span<T> destination)
        {
            if (!TryCopyTo(destination))
                ThrowHelper.ThrowArgumentException_DestinationTooShort();
        }


        /// <summary>
        /// Copies the contents of this span into destination span. If the source
        /// and destinations overlap, this method behaves as if the original values in
        /// a temporary location before the destination is overwritten.
        ///
        /// <returns>If the destination span is shorter than the source span, this method
        /// return false and no data is written to the destination.</returns>
        /// </summary>
        /// <param name="destination">The span to copy items into.</param>
        public bool TryCopyTo(Span<T> destination)
        {
            if ((uint)_length > (uint)destination._length)
                return false;

            // TODO: This is a tide-over implementation as we plan to add a overlap-safe cpblk-based api to Unsafe. (https://github.com/dotnet/corefx/issues/13427)
            unsafe
            {
                ref T src = ref DangerousGetPinnableReference();
                ref T dst = ref destination.DangerousGetPinnableReference();
                IntPtr srcMinusDst = Unsafe.ByteOffset<T>(ref dst, ref src);
                int length = _length;

                bool srcGreaterThanDst = (sizeof(IntPtr) == sizeof(int)) ? srcMinusDst.ToInt32() >= 0 : srcMinusDst.ToInt64() >= 0;
                if (srcGreaterThanDst)
                {
                    // Source address greater than or equal to destination address. Can do normal copy.
                    for (int i = 0; i < length; i++)
                    {
                        Unsafe.Add<T>(ref dst, i) = Unsafe.Add<T>(ref src, i);
                    }
                }
                else
                {
                    // Source address less than destination address. Must do backward copy.
                    int i = length;
                    while (i-- != 0)
                    {
                        Unsafe.Add<T>(ref dst, i) = Unsafe.Add<T>(ref src, i);
                    }
                }
                return true;
            }
        }

        /// <summary>
        /// Returns true if left and right point at the same memory and have the same length.  Note that
        /// this does *not* check to see if the *contents* are equal.
        /// </summary>
        public static bool operator ==(Span<T> left, Span<T> right)
        {
            return left._length == right._length && Unsafe.AreSame<T>(ref left.DangerousGetPinnableReference(), ref right.DangerousGetPinnableReference());
        }

        /// <summary>
        /// Returns false if left and right point at the same memory and have the same length.  Note that
        /// this does *not* check to see if the *contents* are equal.
        /// </summary>
        public static bool operator !=(Span<T> left, Span<T> right) => !(left == right);

        /// <summary>
        /// This method is not supported as spans cannot be boxed. To compare two spans, use operator==.
        /// <exception cref="System.NotSupportedException">
        /// Always thrown by this method.
        /// </exception>
        /// </summary>
        [Obsolete("Equals() on Span will always throw an exception. Use == instead.")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override bool Equals(object obj)
        {
            throw new NotSupportedException(SR.CannotCallEqualsOnSpan);
        }

        /// <summary>
        /// This method is not supported as spans cannot be boxed.
        /// <exception cref="System.NotSupportedException">
        /// Always thrown by this method.
        /// </exception>
        /// </summary>
        [Obsolete("GetHashCode() on Span will always throw an exception.")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override int GetHashCode()
        {
            throw new NotSupportedException(SR.CannotCallGetHashCodeOnSpan);
        }

        /// <summary>
        /// Defines an implicit conversion of an array to a <see cref="Span{T}"/>
        /// </summary>
        public static implicit operator Span<T>(T[] array) => new Span<T>(array);

        /// <summary>
        /// Defines an implicit conversion of a <see cref="ArraySegment{T}"/> to a <see cref="Span{T}"/>
        /// </summary>
        public static implicit operator Span<T>(ArraySegment<T> arraySegment) => new Span<T>(arraySegment.Array, arraySegment.Offset, arraySegment.Count);

        /// <summary>
        /// Defines an implicit conversion of a <see cref="Span{T}"/> to a <see cref="ReadOnlySpan{T}"/>
        /// </summary>
        public static implicit operator ReadOnlySpan<T>(Span<T> span) => new ReadOnlySpan<T>(span._pinnable, span._byteOffset, span._length);

        /// <summary>
        /// Forms a slice out of the given span, beginning at 'start'.
        /// </summary>
        /// <param name="start">The index at which to begin this slice.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// Thrown when the specified <paramref name="start"/> index is not in range (&lt;0 or &gt;=Length).
        /// </exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Span<T> Slice(int start)
        {
            if ((uint)start > (uint)_length)
                ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.start);

            IntPtr newOffset = _byteOffset.Add<T>(start);
            int length = _length - start;
            return new Span<T>(_pinnable, newOffset, length);
        }

        /// <summary>
        /// Forms a slice out of the given span, beginning at 'start', of given length
        /// </summary>
        /// <param name="start">The index at which to begin this slice.</param>
        /// <param name="length">The desired length for the slice (exclusive).</param>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// Thrown when the specified <paramref name="start"/> or end index is not in range (&lt;0 or &gt;=Length).
        /// </exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Span<T> Slice(int start, int length)
        {
            if ((uint)start > (uint)_length || (uint)length > (uint)(_length - start))
                ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.start);

            IntPtr newOffset = _byteOffset.Add<T>(start);
            return new Span<T>(_pinnable, newOffset, length);
        }

        /// <summary>
        /// Copies the contents of this span into a new array.  This heap
        /// allocates, so should generally be avoided, however it is sometimes
        /// necessary to bridge the gap with APIs written in terms of arrays.
        /// </summary>
        public T[] ToArray()
        {
            if (_length == 0)
                return SpanHelpers.PerTypeValues<T>.EmptyArray;

            T[] result = new T[_length];
            CopyTo(result);
            return result;
        }

        /// <summary>
        /// Returns a 0-length span whose base is the null pointer.
        /// </summary>
        public static readonly Span<T> Empty = default(Span<T>);

        /// <summary>
        /// Returns a reference to the 0th element of the Span. If the Span is empty, returns a reference to the location where the 0th element
        /// would have been stored. Such a reference can be used for pinning but must never be dereferenced.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref T DangerousGetPinnableReference()
        {
            if (_pinnable == null)
                unsafe { return ref Unsafe.AsRef<T>(_byteOffset.ToPointer()); }
            else
                return ref Unsafe.AddByteOffset<T>(ref _pinnable.Data, _byteOffset);
        }

        //
        // If the Span was constructed from an object,
        //
        //   _pinnable   = that object (unsafe-casted to a Pinnable<T>)
        //   _byteOffset = offset in bytes from "ref _pinnable.Data" to "ref span[0]"
        //
        // If the Span was constructed from a native pointer,
        //
        //   _pinnable   = null
        //   _byteOffset = the pointer
        //
        private readonly Pinnable<T> _pinnable;
        private readonly IntPtr _byteOffset;
        private readonly int _length;
    }
}
