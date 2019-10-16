// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

using Microsoft.Win32.SafeHandles;

internal partial class Interop
{
    internal partial class BCrypt
    {
        internal static unsafe NTSTATUS BCryptCreateHash(SafeBCryptAlgorithmHandle hAlgorithm, out SafeBCryptHashHandle phHash, IntPtr pbHashObject, int cbHashObject, ReadOnlySpan<byte> secret, int cbSecret, BCryptCreateHashFlags dwFlags)
        {
            fixed (byte* pbSecret = secret)
            {
                return BCryptCreateHash(hAlgorithm, out phHash, pbHashObject, cbHashObject, pbSecret, cbSecret, dwFlags);
            }
        }

        [DllImport(Libraries.BCrypt, CharSet = CharSet.Unicode)]
        private static unsafe extern NTSTATUS BCryptCreateHash(SafeBCryptAlgorithmHandle hAlgorithm, out SafeBCryptHashHandle phHash, IntPtr pbHashObject, int cbHashObject, byte* pbSecret, int cbSecret, BCryptCreateHashFlags dwFlags);

        [Flags]
        internal enum BCryptCreateHashFlags : int
        {
            None = 0x00000000,
            BCRYPT_HASH_REUSABLE_FLAG = 0x00000020,
        }
    }
}
