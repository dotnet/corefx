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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public CustomUncheckedBitArray(int bitLength)
        {
            Debug.Assert(bitLength > 0 && bitLength % 32 == 0, $"bitLength: {bitLength}");

            _array = new int[bitLength / 32];
        }

        public bool IsDefault => _array == default;

        public int MaxIndexableLength
        {
            get
            {
                if (IsDefault)
                {
                    return -1;
                }

                // Maximum possible array length if bitLength was int.MaxValue (i.e. 67_108_864)
                Debug.Assert(_array.Length <= int.MaxValue / 32 + 1, $"arrayLength: {_array.Length}");

                // This multiplication can overflow, so cast to uint first.
                int indexableLength = (int)((uint)_array.Length * 32 - 1);

                Debug.Assert(indexableLength >= -1, $"maximum indexable length: {indexableLength}");

                return indexableLength;
            }
        }

        public bool this[int index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                Debug.Assert(!IsDefault);
                Debug.Assert(index >= 0, $"Get - Negative - index: {index}, arrayLength: {_array.Length}");

                int elementIndex = Div32Rem(index, out int extraBits);

                Debug.Assert(elementIndex < _array.Length, $"Get - index: {index}, elementIndex: {elementIndex}, arrayLength: {_array.Length}, extraBits: {extraBits}");

                return (_array[elementIndex] & (1 << extraBits)) != 0;
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set
            {
                Debug.Assert(!IsDefault);
                Debug.Assert(index >= 0, $"Set - Negative - index: {index}, arrayLength: {_array.Length}");

                // Maximum possible array length if bitLength was int.MaxValue (i.e. 67_108_864)
                Debug.Assert(_array.Length <= int.MaxValue / 32 + 1, $"index: {index}, arrayLength: {_array.Length}");

                int elementIndex = Div32Rem(index, out int extraBits);

                // Grow the array when setting a bit if it isn't big enough
                // This way the caller doesn't have to check.
                if (elementIndex >= _array.Length)
                {
                    // This multiplication can overflow, so cast to uint first.
                    Debug.Assert(index >= 0 && index > (int)((uint)_array.Length * 32 - 1), $"Only grow when necessary - index: {index}, arrayLength: {_array.Length}");
                    DoubleArray();
                }

                Debug.Assert(elementIndex < _array.Length, $"Set - index: {index}, elementIndex: {elementIndex}, arrayLength: {_array.Length}, extraBits: {extraBits}");

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
        private void DoubleArray()
        {
            Debug.Assert(_array.Length < int.MaxValue / 2, $"Array too large - arrayLength: {_array.Length}");

            // This is gauranteed to never overflow since the maximum integer array length
            // for int.MaxValue bits is 67_108_864.
            Array.Resize(ref _array, _array.Length * 2);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int Div32Rem(int number, out int remainder)
        {
            uint quotient = (uint)number / 32;
            remainder = number & (32 - 1);   // equivalent to number % 32, since 32 is a power of 2
            return (int)quotient;
        }
    }
}
