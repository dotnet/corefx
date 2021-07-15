// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
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

        internal static int EvpDigestUpdate(SafeEvpMdCtxHandle ctx, ReadOnlySpan<byte> d, int cnt) =>
            EvpDigestUpdate(ctx, ref MemoryMarshal.GetReference(d), cnt);

        [DllImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_EvpDigestUpdate")]
        private extern static int EvpDigestUpdate(SafeEvpMdCtxHandle ctx, ref byte d, int cnt);

        [DllImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_EvpDigestFinalEx")]
        internal extern static int EvpDigestFinalEx(SafeEvpMdCtxHandle ctx, ref byte md, ref uint s);

        [DllImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_EvpMdSize")]
        internal extern static int EvpMdSize(IntPtr md);

        [DllImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_GetMaxMdSize")]
        private extern static int GetMaxMdSize();

        internal static readonly int EVP_MAX_MD_SIZE = GetMaxMdSize();
    }
}
