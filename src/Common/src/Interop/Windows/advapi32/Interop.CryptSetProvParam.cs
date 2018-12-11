// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;
using System.Security.Cryptography;

internal partial class Interop
{
    internal partial class Advapi32
    {
        [DllImport(Libraries.Advapi32, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool CryptSetProvParam(
            SafeHandle safeProvHandle,
            CryptProvParam dwParam,
            IntPtr pbData,
            int dwFlags);

        [DllImport(Libraries.Advapi32, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool CryptSetProvParam(SafeProvHandle hProv, CryptProvParam dwParam, ref IntPtr pbData, int dwFlags);
    }
}
