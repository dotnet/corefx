// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace System.Text.Encodings.Web
{
    /// <summary>
    /// Similar to ReadOnlySpan, but operates on raw strings and char[] arrays.
    /// Provides a bounds-safe way of accessing contiguous memory.
    /// </summary>
    internal readonly unsafe ref struct PinnedCharBuffer
    {
        private readonly char* _ptr;
        private readonly int _length;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal PinnedCharBuffer(char* pinnedValue, string value, int offset, int count)
            : this(pinnedValue, value.Length)
        {
#if DEBUG
            fixed (char* pValue = value)
            {
                Debug.Assert(pValue == pinnedValue, "Caller didn't pin the string before calling the ctor.");
            }
#endif

            this = Slice(offset, count);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal PinnedCharBuffer(char* pinnedValue, char[] value, int offset, int count)
            : this(pinnedValue, value.Length)
        {
#if DEBUG
            fixed (char* pValue = value)
            {
                Debug.Assert(pValue == pinnedValue, "Caller didn't pin the array before calling the ctor.");
            }
#endif

            this = Slice(offset, count);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private PinnedCharBuffer(char* ptr, int length)
        {
            Debug.Assert(length >= 0);
            Debug.Assert(length == 0 || ptr != null);

            _ptr = ptr;
            _length = length;
        }

        internal int Length
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return _length;
            }
        }

        internal char* Pointer
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return _ptr;
            }
        }

        internal char this[int index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                if ((uint)index >= (uint)_length)
                {
                    ThrowArgumentOutOfRangeException();
                }

                return _ptr[index];
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal PinnedCharBuffer Slice(int offset)
        {
            if ((uint)offset > (uint)_length)
            {
                ThrowArgumentOutOfRangeException();
            }

            return new PinnedCharBuffer(_ptr + offset, _length - offset);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal PinnedCharBuffer Slice(int offset, int count)
        {
            PinnedCharBuffer firstSlice = Slice(offset);

            if ((uint)count > (uint)firstSlice._length)
            {
                ThrowArgumentOutOfRangeException();
            }

            return new PinnedCharBuffer(firstSlice._ptr, count);
        }

        private static void ThrowArgumentOutOfRangeException()
        {
            throw new ArgumentOutOfRangeException();
        }
    }
}
