// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

internal static partial class Interop
{
    internal static partial class Crypto
    {
        [DllImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_EvpMdCtxCreate")]
        internal extern static SafeEvpMdCtxHandle EvpMdCtxCreate(IntPtr type);

        [DllImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_EvpMdCtxDestroy")]
        internal extern static void EvpMdCtxDestroy(IntPtr ctx);

        [DllImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_EvpDigestReset")]
        internal extern static int EvpDigestReset(SafeEvpMdCtxHandle ctx, IntPtr type);

        [DllImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_EvpDigestUpdate")]
        internal extern static unsafe int EvpDigestUpdate(SafeEvpMdCtxHandle ctx, byte* d, int cnt);

        [DllImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_EvpDigestFinalEx")]
        internal extern static unsafe int EvpDigestFinalEx(SafeEvpMdCtxHandle ctx, byte* md, ref uint s);

        [DllImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_EvpMdSize")]
        internal extern static int EvpMdSize(IntPtr md);


        [DllImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_EvpMd5")]
        internal extern static IntPtr EvpMd5();

        [DllImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_EvpSha1")]
        internal extern static IntPtr EvpSha1();

        [DllImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_EvpSha256")]
        internal extern static IntPtr EvpSha256();

        [DllImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_EvpSha384")]
        internal extern static IntPtr EvpSha384();

        [DllImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_EvpSha512")]
        internal extern static IntPtr EvpSha512();


        [DllImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_GetMaxMdSize")]
        private extern static int GetMaxMdSize();

        internal static readonly int EVP_MAX_MD_SIZE = GetMaxMdSize();
    }
}
