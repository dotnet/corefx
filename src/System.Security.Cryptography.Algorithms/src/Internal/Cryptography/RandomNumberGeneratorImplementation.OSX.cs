// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Runtime.InteropServices;

namespace System.Security.Cryptography
{
    partial class RandomNumberGeneratorImplementation
    {
        private static void GetBytes(ref byte pbBuffer, int count)
        {
            Debug.Assert(count > 0);

            Interop.AppleCrypto.GetRandomBytes(ref pbBuffer, count);
        }

        internal static void FillSpan(Span<byte> data)
        {
            if (data.Length > 0)
            {
                GetBytes(ref MemoryMarshal.GetReference(data), data.Length);
            }
        }
    }
}
