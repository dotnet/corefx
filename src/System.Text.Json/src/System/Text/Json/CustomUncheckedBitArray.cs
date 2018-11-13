// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace System.Text.Json
{
    // Inspired by BitArray
    // https://github.com/dotnet/corefx/blob/master/src/System.Collections/src/System/Collections/BitArray.cs
    internal struct CustomUncheckedBitArray
    {
        private int[] _array;

        public int Length { get; private set; }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public CustomUncheckedBitArray(int bitLength, int integerLength)
        {
            Debug.Assert(bitLength > 0, $"bitLength: {bitLength}, integerLength: {integerLength}");
            Debug.Assert(integerLength > 0 && integerLength <= int.MaxValue / 32 + 1, $"bitLength: {bitLength}, integerLength: {integerLength}");
            Debug.Assert(bitLength <= (long)integerLength * 32, $"bitLength: {bitLength}, integerLength: {integerLength}");

            _array = new int[integerLength];
            Length = bitLength;
        }

        public bool this[int index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                Debug.Assert(index >= 0 && index < Length, $"index: {index}, Length: {Length}");

                int elementIndex = Div32Rem(index, out int extraBits);

                Debug.Assert(elementIndex < _array.Length, $"index: {index}, Length: {Length}, elementIndex: {elementIndex}, arrayLength: {_array.Length}, extraBits: {extraBits}");

                return (_array[elementIndex] & (1 << extraBits)) != 0;
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set
            {
                Debug.Assert(index >= 0, $"index: {index}, Length: {Length}");

                // Grow the array when setting a bit if it isn't big enough
                // This way the caller doesn't have to check.
                if (index >= Length)
                {
                    Grow(index);
                }

                int elementIndex = Div32Rem(index, out int extraBits);

                Debug.Assert(elementIndex < _array.Length, $"index: {index}, Length: {Length}, elementIndex: {elementIndex}, arrayLength: {_array.Length}, extraBits: {extraBits}");

                int newValue = _array[elementIndex];
                if (value)
                {
                    newValue |= 1 << extraBits;
                }
                else
                {
                    newValue &= ~(1 << extraBits);
                }
                _array[elementIndex] = newValue;
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private void Grow(int index)
        {
            Debug.Assert(index >= Length, $"index: {index}, Length: {Length}");

            // If index is a multiple of 32, add 1.
            int newints = (index & 31) == 0 ? 1 : 0;

            if ((uint)index / 2 < int.MaxValue)
            {
                index *= 2; // Grow by doubling, if possible.
            }
            else
            {
                index++;
            }

            newints += ((index - 1) >> 5) + 1;   // (value + 31) / 32 without overflow

            if (newints > _array.Length)
            {
                var newArray = new int[newints];
                _array.AsSpan().CopyTo(newArray);
                _array = newArray;
            }

            if (index > Length)
            {
                // clear high bit values in the last int
                int last = (Length - 1) >> 5;
                Div32Rem(Length, out int bits);
                if (bits > 0)
                {
                    _array[last] &= (1 << bits) - 1;
                }

                // clear remaining int values
                _array.AsSpan(last + 1, newints - last - 1).Clear();
            }

            Length = index;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int Div32Rem(int number, out int remainder)
        {
            uint quotient = (uint)number / 32;
            remainder = number & 31;    // number & 31 == number % 32
            return (int)quotient;
        }
    }
}
