﻿// Licensed to the .NET Foundation under one or more agreements.
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
        // We are using a ulong to represent our nested state, so we can only
        // go 64 levels deep without having to allocate.
        internal const int AllocationFreeMaxDepth = sizeof(ulong) * 8;

        private int[] _array;

        // This ulong container represents a tiny stack to track the state during nested transitions.
        // The first bit represents the state of the current depth (1 == object, 0 == array).
        // Each subsequent bit is the parent / containing type (object or array). Since this
        // reader does a linear scan, we only need to keep a single path as we go through the data.
        // This is primarily used as an optimization to avoid having to allocate an object for
        // depths up to 64 (which is the default max depth).
        internal ulong _allocationFreeContainer;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public CustomUncheckedBitArray(int bitLength)
        {
            Debug.Assert(bitLength > 0 && bitLength % 32 == 0, $"bitLength: {bitLength}");

            _array = new int[bitLength / 32];
            _allocationFreeContainer = default;
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void EnsureAndPushTrue(int depth)
        {
            if (depth < AllocationFreeMaxDepth)
            {
                _allocationFreeContainer = (_allocationFreeContainer << 1) | 1;
            }
            else
            {
                EnsureAndPush(depth, true);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void EnsureAndPushFalse(int depth)
        {
            if (depth < AllocationFreeMaxDepth)
            {
                _allocationFreeContainer = _allocationFreeContainer << 1;
            }
            else
            {
                EnsureAndPush(depth, false);
            }
        }

        // Allocate the bit array lazily only when it is absolutely necessary
        [MethodImpl(MethodImplOptions.NoInlining)]
        private void EnsureAndPush(int depth, bool value)
        {
            if (IsDefault)
            {
                _array = new int[2];
            }
            this[depth - AllocationFreeMaxDepth] = value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool UpdateAndPop(int depth)
        {
            bool inObject = false;
            if (depth < AllocationFreeMaxDepth)
            {
                _allocationFreeContainer >>= 1;
                inObject = (_allocationFreeContainer & 1) != 0;
            }
            else if (depth == AllocationFreeMaxDepth)
            {
                inObject = (_allocationFreeContainer & 1) != 0;
            }
            else
            {
                inObject = GetInObjectFromArray(depth);
            }
            return inObject;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private bool GetInObjectFromArray(int depth)
        {
            return this[depth - AllocationFreeMaxDepth - 1];
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
                    DoubleArray(elementIndex);
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
        private void DoubleArray(int minSize)
        {
            Debug.Assert(_array.Length < int.MaxValue / 2, $"Array too large - arrayLength: {_array.Length}");
            Debug.Assert(minSize >= 0 && minSize >= _array.Length);

            int nextDouble = Math.Max(minSize + 1, _array.Length * 2);
            Debug.Assert(nextDouble > minSize);

            Array.Resize(ref _array, nextDouble);
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
