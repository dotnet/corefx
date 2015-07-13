// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Runtime.InteropServices;

using Microsoft.Win32.SafeHandles;

internal static partial class Interop
{
    internal static partial class NativeCrypto
    {
        private delegate int NegativeSizeReadMethod<in THandle>(THandle handle, byte[] buf, int cBuf);

        [DllImport(Libraries.CryptoInterop)]
        private static extern int GetX509Thumbprint(SafeX509Handle x509, byte[] buf, int cBuf);

        [DllImport(Libraries.CryptoInterop)]
        private static extern int GetX509NameRawBytes(IntPtr x509Name, byte[] buf, int cBuf);

        [DllImport(Libraries.CryptoInterop)]
        internal static extern IntPtr GetX509NotBefore(SafeX509Handle x509);

        [DllImport(Libraries.CryptoInterop)]
        internal static extern IntPtr GetX509NotAfter(SafeX509Handle x509);

        [DllImport(Libraries.CryptoInterop)]
        internal static extern int GetX509Version(SafeX509Handle x509);

        [DllImport(Libraries.CryptoInterop)]
        internal static extern IntPtr GetX509SignatureAlgorithm(SafeX509Handle x509);

        [DllImport(Libraries.CryptoInterop)]
        internal static extern IntPtr GetX509PublicKeyAlgorithm(SafeX509Handle x509);

        [DllImport(Libraries.CryptoInterop)]
        private static extern int GetX509PublicKeyParameterBytes(SafeX509Handle x509, byte[] buf, int cBuf);

        [DllImport(Libraries.CryptoInterop)]
        internal static extern IntPtr GetX509PublicKeyBytes(SafeX509Handle x509);

        [DllImport(Libraries.CryptoInterop)]
        internal static extern int GetX509EkuFieldCount(SafeEkuExtensionHandle eku);

        [DllImport(Libraries.CryptoInterop)]
        internal static extern IntPtr GetX509EkuField(SafeEkuExtensionHandle eku, int loc);

        [DllImport(Libraries.CryptoInterop)]
        internal static extern SafeBioHandle GetX509NameInfo(SafeX509Handle x509, int nameType, [MarshalAs(UnmanagedType.Bool)] bool forIssuer);

        [DllImport(Libraries.CryptoInterop)]
        private static extern int GetAsn1StringBytes(IntPtr asn1, byte[] buf, int cBuf);

        [DllImport(Libraries.CryptoInterop)]
        internal static extern int GetX509StackFieldCount(SafeX509StackHandle stack);

        /// <summary>
        /// Gets a pointer to a certificate within a STACK_OF(X509). This pointer will later
        /// be freed, so it should be cloned via new X509Certificate2(IntPtr)
        /// </summary>
        [DllImport(Libraries.CryptoInterop)]
        internal static extern IntPtr GetX509StackField(SafeX509StackHandle stack, int loc);

        [DllImport(Libraries.CryptoInterop)]
        internal static extern void RecursiveFreeX509Stack(IntPtr stack);

        [DllImport(Libraries.CryptoInterop, CharSet = CharSet.Ansi)]
        internal static extern string GetX509RootStorePath();

        [DllImport(Libraries.CryptoInterop)]
        private static extern int SetX509ChainVerifyTime(
            SafeX509StoreCtxHandle ctx,
            int year,
            int month,
            int day,
            int hour,
            int minute,
            int second,
            [MarshalAs(UnmanagedType.Bool)] bool isDst);

        internal static byte[] GetAsn1StringBytes(IntPtr asn1)
        {
            return GetDynamicBuffer((ptr, buf, i) => GetAsn1StringBytes(ptr, buf, i), asn1);
        }

        internal static byte[] GetX509Thumbprint(SafeX509Handle x509)
        {
            return GetDynamicBuffer((handle, buf, i) => GetX509Thumbprint(handle, buf, i), x509);
        }

        internal static byte[] GetX509NameRawBytes(IntPtr x509Name)
        {
            return GetDynamicBuffer((ptr, buf, i) => GetX509NameRawBytes(ptr, buf, i), x509Name);
        }

        internal static byte[] GetX509PublicKeyParameterBytes(SafeX509Handle x509)
        {
            return GetDynamicBuffer((handle, buf, i) => GetX509PublicKeyParameterBytes(handle, buf, i), x509);
        }

        internal static void SetX509ChainVerifyTime(SafeX509StoreCtxHandle ctx, DateTime verifyTime)
        {
            // Let Unspecified mean Local, so only convert if the source was UTC.
            if (verifyTime.Kind == DateTimeKind.Utc)
            {
                verifyTime = verifyTime.ToLocalTime();
            }

            int succeeded = SetX509ChainVerifyTime(
                ctx,
                verifyTime.Year,
                verifyTime.Month,
                verifyTime.Day,
                verifyTime.Hour,
                verifyTime.Minute,
                verifyTime.Second,
                verifyTime.IsDaylightSavingTime());

            if (succeeded != 1)
            {
                throw Interop.libcrypto.CreateOpenSslCryptographicException();
            }
        }

        private static byte[] GetDynamicBuffer<THandle>(NegativeSizeReadMethod<THandle> method, THandle handle)
        {
            int negativeSize = method(handle, null, 0);

            if (negativeSize > 0)
            {
                throw Interop.libcrypto.CreateOpenSslCryptographicException();
            }

            byte[] bytes = new byte[-negativeSize];

            int ret = method(handle, bytes, bytes.Length);

            if (ret != 1)
            {
                throw Interop.libcrypto.CreateOpenSslCryptographicException();
            }

            return bytes;
        }
    }
}
