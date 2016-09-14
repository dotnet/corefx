// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.CompilerServices;

namespace System.Numerics.Hashing
{
    internal static class HashHelpers
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Combine(int h1, int h2)
        {
            // This is the same algorithm that is used by Boost for combining hashes.
            // You can find an explanation of what the magic number is here:
            // http://stackoverflow.com/a/4948967/4077294

            uint mask = (uint)h2 + 0x9e3779b9 + ((uint)h1 << 6) + ((uint)h1 >> 2);
            h1 ^= (int)mask;
            return h1;
        }
    }
}

