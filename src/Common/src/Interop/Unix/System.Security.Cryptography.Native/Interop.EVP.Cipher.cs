﻿// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

internal static partial class Interop
{
    internal static partial class Crypto
    {
        [DllImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_EvpCipherCreate")]
        internal static extern SafeEvpCipherCtxHandle EvpCipherCreate(
            IntPtr cipher,
            byte[] key,
            byte[] iv,
            int enc);

        [DllImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_EvpCipherDestroy")]
        internal static extern void EvpCipherDestroy(IntPtr ctx);

        [DllImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_EvpCipherReset")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool EvpCipherReset(SafeEvpCipherCtxHandle ctx);

        [DllImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_EvpCipherCtxSetPadding")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool EvpCipherCtxSetPadding(SafeEvpCipherCtxHandle x, int padding);

        [DllImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_EvpCipherUpdate")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static unsafe extern bool EvpCipherUpdate(
            SafeEvpCipherCtxHandle ctx,
            byte* @out,
            out int outl,
            byte* @in,
            int inl);

        [DllImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_EvpCipherFinalEx")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern unsafe bool EvpCipherFinalEx(
            SafeEvpCipherCtxHandle ctx,
            byte* outm,
            out int outl);

        [DllImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_EvpAes128Ecb")]
        internal static extern IntPtr EvpAes128Ecb();

        [DllImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_EvpAes128Cbc")]
        internal static extern IntPtr EvpAes128Cbc();

        [DllImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_EvpAes192Ecb")]
        internal static extern IntPtr EvpAes192Ecb();

        [DllImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_EvpAes192Cbc")]
        internal static extern IntPtr EvpAes192Cbc();

        [DllImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_EvpAes256Ecb")]
        internal static extern IntPtr EvpAes256Ecb();

        [DllImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_EvpAes256Cbc")]
        internal static extern IntPtr EvpAes256Cbc();

        [DllImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_EvpDes3Ecb")]
        internal static extern IntPtr EvpDes3Ecb();

        [DllImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_EvpDes3Cbc")]
        internal static extern IntPtr EvpDes3Cbc();
    }
}
