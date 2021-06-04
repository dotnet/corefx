// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using Microsoft.Win32.SafeHandles;

internal static partial class Interop
{
    internal static partial class Crypto
    {
        [DllImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_RsaGenerateKey")]
        private static extern SafeEvpPKeyHandle CryptoNative_RsaGenerateKey(int keySize);

        internal static SafeEvpPKeyHandle RsaGenerateKey(int keySize)
        {
            SafeEvpPKeyHandle pkey = CryptoNative_RsaGenerateKey(keySize);

            if (pkey.IsInvalid)
            {
                pkey.Dispose();
                throw CreateOpenSslCryptographicException();
            }

            return pkey;
        }

        [DllImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_RsaDecrypt")]
        private static extern int CryptoNative_RsaDecrypt(
            SafeEvpPKeyHandle pkey,
            ref byte source,
            int sourceLength,
            RSAEncryptionPaddingMode paddingMode,
            IntPtr digestAlgorithm,
            ref byte destination,
            int destinationLength);

        internal static int RsaDecrypt(
            SafeEvpPKeyHandle pkey,
            ReadOnlySpan<byte> source,
            RSAEncryptionPaddingMode paddingMode,
            IntPtr digestAlgorithm,
            Span<byte> destination)
        {
            int written = CryptoNative_RsaDecrypt(
                pkey,
                ref MemoryMarshal.GetReference(source),
                source.Length,
                paddingMode,
                digestAlgorithm,
                ref MemoryMarshal.GetReference(destination),
                destination.Length);

            if (written < 0)
            {
                Debug.Assert(written == -1);
                throw CreateOpenSslCryptographicException();
            }

            return written;
        }

        [DllImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_RsaSignHash")]
        private static extern int CryptoNative_RsaSignHash(
            SafeEvpPKeyHandle pkey,
            RSASignaturePaddingMode paddingMode,
            IntPtr digestAlgorithm,
            ref byte hash,
            int hashLength,
            ref byte destination,
            int destinationLength);

        internal static int RsaSignHash(
            SafeEvpPKeyHandle pkey,
            RSASignaturePaddingMode paddingMode,
            IntPtr digestAlgorithm,
            ReadOnlySpan<byte> hash,
            Span<byte> destination)
        {
            int written = CryptoNative_RsaSignHash(
                pkey,
                paddingMode,
                digestAlgorithm,
                ref MemoryMarshal.GetReference(hash),
                hash.Length,
                ref MemoryMarshal.GetReference(destination),
                destination.Length);

            if (written < 0)
            {
                Debug.Assert(written == -1);
                throw CreateOpenSslCryptographicException();
            }

            return written;
        }

        [DllImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_EvpPkeyGetRsa")]
        internal static extern SafeRsaHandle EvpPkeyGetRsa(SafeEvpPKeyHandle pkey);

        [DllImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_EvpPkeySetRsa")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool EvpPkeySetRsa(SafeEvpPKeyHandle pkey, SafeRsaHandle rsa);
    }
}
