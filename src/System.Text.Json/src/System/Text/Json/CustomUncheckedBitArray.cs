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
            Debug.Assert(bitLength > 0);
            Debug.Assert(integerLength > 0 && integerLength < int.MaxValue / 32);
            Debug.Assert(integerLength * 32 == bitLength);

            _array = new int[integerLength];
            Length = bitLength;
        }

        public bool this[int index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                Debug.Assert(index >= 0 && index <= Length);

                int quotient = Div32Rem(index, out int remainder);

                Debug.Assert(quotient < _array.Length);

                return (_array[quotient] & (1 << remainder)) != 0;
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set
            {
                Debug.Assert(index >= 0);

                // Grow the array when setting a bit if it isn't big enough
                // This way the caller doesn't have to check.
                if (index >= Length)
                {
                    Grow(index);
                }

                int quotient = Div32Rem(index, out int remainder);

                Debug.Assert(quotient < _array.Length);

                if (value)
                {
                    _array[quotient] |= 1 << remainder;
                }
                else
                {
                    _array[quotient] &= ~(1 << remainder);
                }
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private void Grow(int index)
        {
            Debug.Assert(index >= Length);
            int newints = ((index - 1) >> 5) + 1;   // (value + 31) / 32 without overflow

            // If index is a multiple of 32
            if ((index & 31) == 0)
            {
                newints++;
            }

            if (newints > _array.Length)
            {
                int[] newArray = new int[newints];
                _array.AsSpan().CopyTo(newArray);
                _array = newArray;
            }
            Length = index;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int Div32Rem(int number, out int remainder)
        {
            int quotient = number >> 5;   // Divide by 32.
            remainder = number - (quotient << 5);   // Multiply by 32.
            return quotient;
        }
    }
}
