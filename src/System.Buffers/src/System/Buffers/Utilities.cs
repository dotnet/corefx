// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.CompilerServices;

namespace System.Buffers
{
    internal static class Utilities
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static int SelectBucketIndex(int bufferSize)
        {
            uint bitsRemaining = ((uint)bufferSize - 1) >> 4;

            int poolIndex = 0;
            if (bitsRemaining > 0xFFFF) { bitsRemaining >>= 16; poolIndex = 16; }
            if (bitsRemaining > 0xFF)   { bitsRemaining >>= 8;  poolIndex += 8; }
            if (bitsRemaining > 0xF)    { bitsRemaining >>= 4;  poolIndex += 4; }
            if (bitsRemaining > 0x3)    { bitsRemaining >>= 2;  poolIndex += 2; }
            if (bitsRemaining > 0x1)    { bitsRemaining >>= 1;  poolIndex += 1; }

            return poolIndex + (int)bitsRemaining;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static int GetMaxSizeForBucket(int binIndex)
        {
            checked
            {
                int result = 2;
                int shifts = binIndex + 3;
                result <<= shifts;
                return result;
            }
        }

        internal static int GetPoolId<T>(ArrayPool<T> pool)
        {
            return pool.GetHashCode();
        }

        internal static int GetBufferId<T>(T[] buffer)
        {
            return buffer.GetHashCode();
        }

        internal static int GetBucketId<T>(DefaultArrayPoolBucket<T> bucket)
        {
            return bucket.GetHashCode();
        }
    }
}
