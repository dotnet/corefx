// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Win32.SafeHandles;

internal static partial class Interop
{
    internal static partial class Crypto
    {
        [DllImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_CreateMemoryBio")]
        internal static extern SafeBioHandle CreateMemoryBio();

        [DllImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_BioNewFile")]
        internal static extern SafeBioHandle BioNewFile(string filename, string mode);

        [DllImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_BioDestroy")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool BioDestroy(IntPtr a);

        [DllImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_BioGets")]
        internal static extern int BioGets(SafeBioHandle b, byte[] buf, int size);

        [DllImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_BioRead")]
        internal static extern int BioRead(SafeBioHandle b, byte[] data, int len);

        [DllImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_BioWrite")]
        internal static extern int BioWrite(SafeBioHandle b, byte[] data, int len);

        [DllImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_GetMemoryBioSize")]
        internal static extern int GetMemoryBioSize(SafeBioHandle bio);

        [DllImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_BioCtrlPending")]
        internal static extern int BioCtrlPending(SafeBioHandle bio);

        [DllImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_CreateCustomBio")]
        internal static extern SafeBioHandle CreateCustomBio();

        [DllImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_BioSetAppData")]
        internal static extern void BioSetAppData(SafeBioHandle bio, IntPtr data);

        [DllImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_BioGetAppData")]
        internal static extern IntPtr BioGetAppData(IntPtr bio);

        [DllImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_BioSetWriteFlag")]
        internal static extern void BioSetWriteFlag(SafeBioHandle bio);

        [DllImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_BioSetShoudRetryReadFlag")]
        internal static extern void BioSetShoudRetryReadFlag(SafeBioHandle bio);
    }
}
