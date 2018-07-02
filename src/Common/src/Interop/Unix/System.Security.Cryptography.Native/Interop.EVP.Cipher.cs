// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

internal static partial class Interop
{
    internal static partial class Crypto
    {
        [DllImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_EvpCipherCreate2")]
        internal static extern SafeEvpCipherCtxHandle EvpCipherCreate(
            IntPtr cipher,
            ref byte key,
            int keyLength,
            int effectivekeyLength,
            ref byte iv,
            int enc);

        [DllImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_EvpCipherCreatePartial")]
        internal static extern SafeEvpCipherCtxHandle EvpCipherCreatePartial(
            IntPtr cipher,
            int keyLength,
            int effectivekeyLength);

        [DllImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_EvpCipherSetKeyAndIV")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool EvpCipherSetKeyAndIV(
            SafeEvpCipherCtxHandle ctx,
            ref byte key,
            ref byte iv,
            int enc);

        [DllImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_EvpAesGcmSetNonceLength")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool EvpAesGcmSetNonceLength(
            SafeEvpCipherCtxHandle ctx, int nonceLength);

        [DllImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_EvpAesCcmSetNonceLength")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool EvpAesCcmSetNonceLength(
            SafeEvpCipherCtxHandle ctx, int nonceLength);

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
        internal static extern bool EvpCipherUpdate(
            SafeEvpCipherCtxHandle ctx,
            ref byte @out,
            out int outl,
            ref byte @in,
            int inl);

        [DllImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_EvpCipherFinalEx")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool EvpCipherFinalEx(
            SafeEvpCipherCtxHandle ctx,
            ref byte outm,
            out int outl);

        [DllImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_EvpAesGcmGetTag")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool EvpAesGcmGetTag(
            SafeEvpCipherCtxHandle ctx,
            ref byte tag,
            int tagLength);

        [DllImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_EvpAesGcmSetTag")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool EvpAesGcmSetTag(
            SafeEvpCipherCtxHandle ctx,
            ref byte tag,
            int tagLength);

        [DllImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_EvpAesCcmGetTag")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool EvpAesCcmGetTag(
            SafeEvpCipherCtxHandle ctx,
            ref byte tag,
            int tagLength);

        [DllImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_EvpAesCcmSetTag")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool EvpAesCcmSetTag(
            SafeEvpCipherCtxHandle ctx,
            ref byte tag,
            int tagLength);

        [DllImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_EvpAes128Ecb")]
        internal static extern IntPtr EvpAes128Ecb();

        [DllImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_EvpAes128Cbc")]
        internal static extern IntPtr EvpAes128Cbc();

        [DllImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_EvpAes128Gcm")]
        internal static extern IntPtr EvpAes128Gcm();

        [DllImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_EvpAes128Ccm")]
        internal static extern IntPtr EvpAes128Ccm();

        [DllImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_EvpAes192Ecb")]
        internal static extern IntPtr EvpAes192Ecb();

        [DllImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_EvpAes192Cbc")]
        internal static extern IntPtr EvpAes192Cbc();

        [DllImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_EvpAes192Gcm")]
        internal static extern IntPtr EvpAes192Gcm();

        [DllImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_EvpAes192Ccm")]
        internal static extern IntPtr EvpAes192Ccm();

        [DllImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_EvpAes256Ecb")]
        internal static extern IntPtr EvpAes256Ecb();

        [DllImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_EvpAes256Cbc")]
        internal static extern IntPtr EvpAes256Cbc();

        [DllImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_EvpAes256Gcm")]
        internal static extern IntPtr EvpAes256Gcm();

        [DllImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_EvpAes256Ccm")]
        internal static extern IntPtr EvpAes256Ccm();

        [DllImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_EvpDesCbc")]
        internal static extern IntPtr EvpDesCbc();

        [DllImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_EvpDesEcb")]
        internal static extern IntPtr EvpDesEcb();

        [DllImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_EvpDes3Cbc")]
        internal static extern IntPtr EvpDes3Cbc();

        [DllImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_EvpDes3Ecb")]
        internal static extern IntPtr EvpDes3Ecb();

        [DllImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_EvpRC2Cbc")]
        internal static extern IntPtr EvpRC2Cbc();

        [DllImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_EvpRC2Ecb")]
        internal static extern IntPtr EvpRC2Ecb();
    }
}
