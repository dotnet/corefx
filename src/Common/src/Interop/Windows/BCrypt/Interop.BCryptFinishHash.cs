// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

internal partial class Interop
{
    internal partial class BCrypt
    {
        internal static unsafe NTSTATUS BCryptFinishHash(SafeBCryptHashHandle hHash, byte[] pbOutput, int cbOutput, int dwFlags)
        {
            fixed (byte* ptr = pbOutput)
            {
                return BCryptFinishHash(hHash, ptr, cbOutput, dwFlags);
            }
        }

        [DllImport(Libraries.BCrypt, CharSet = CharSet.Unicode)]
        internal static unsafe extern NTSTATUS BCryptFinishHash(SafeBCryptHashHandle hHash, byte* pbOutput, int cbOutput, int dwFlags);
    }
}

