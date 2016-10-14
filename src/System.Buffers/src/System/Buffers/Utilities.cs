// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace System.Buffers
{
    internal static class Utilities
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static int RoundUpPowerOf2(int size)
        {
            // From: http://graphics.stanford.edu/~seander/bithacks.html#RoundUpPowerOf2
            size--;
            size |= size >> 1;
            size |= size >> 2;
            size |= size >> 4;
            size |= size >> 8;
            size |= size >> 16;
            return size + 1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static int GetShiftFromPowerOf2(int size)
        {
            // assuming that size is not 0 and has been computed through RoundUpPowerOf2
            int shift = -1;
            while (size != 0)
            {
                shift++;
                size = size/2;
            }
            return shift;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static int SelectBucketIndex(int shift, int bufferSize)
        {
            Debug.Assert(bufferSize > 0);

            uint bitsRemaining = ((uint)bufferSize - 1) >> shift;

            int poolIndex = 0;
            if (bitsRemaining > 0xFFFF) { bitsRemaining >>= 16; poolIndex = 16; }
            if (bitsRemaining > 0xFF)   { bitsRemaining >>= 8;  poolIndex += 8; }
            if (bitsRemaining > 0xF)    { bitsRemaining >>= 4;  poolIndex += 4; }
            if (bitsRemaining > 0x3)    { bitsRemaining >>= 2;  poolIndex += 2; }
            if (bitsRemaining > 0x1)    { bitsRemaining >>= 1;  poolIndex += 1; }

            return poolIndex + (int)bitsRemaining;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static int GetMaxSizeForBucket(int minSize, int binIndex)
        {
            int maxSize = minSize << binIndex;
            Debug.Assert(maxSize >= 0);
            return maxSize;
        }
    }
}
