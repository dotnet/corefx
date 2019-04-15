// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Buffers;
using System.Diagnostics;

namespace System.Security.Cryptography
{
    internal static class CryptoPool
    {
        internal const int ClearAll = -1;

        private static readonly ArrayPool<byte> s_pool = ArrayPool<byte>.Create();

        internal static byte[] Rent(int minimumLength) => s_pool.Rent(minimumLength);

        internal static void Return(byte[] array, int clearSize = ClearAll)
        {
            Debug.Assert(clearSize <= array.Length);
            bool clearWholeArray = clearSize < 0;

            if (!clearWholeArray && clearSize != 0)
            {
                Array.Clear(array, 0, clearSize);
            }

            s_pool.Return(array, clearWholeArray);
        }
    }
}
