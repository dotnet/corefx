// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Win32.SafeHandles;

internal static partial class Interop
{
    internal static partial class Crypto
    {
        private delegate int NegativeSizeReadMethod<in THandle>(THandle handle, byte[] buf, int cBuf);

        [DllImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_BioTell")]
        internal static extern int BioTell(SafeBioHandle bio);

        [DllImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_BioSeek")]
        internal static extern int BioSeek(SafeBioHandle bio, int pos);

        [DllImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_GetX509Thumbprint")]
        private static extern int GetX509Thumbprint(SafeX509Handle x509, byte[] buf, int cBuf);

        [DllImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_GetX509NameRawBytes")]
        private static extern int GetX509NameRawBytes(IntPtr x509Name, byte[] buf, int cBuf);

        [DllImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_ReadX509AsDerFromBio")]
        internal static extern SafeX509Handle ReadX509AsDerFromBio(SafeBioHandle bio);

        [DllImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_GetX509NotBefore")]
        internal static extern IntPtr GetX509NotBefore(SafeX509Handle x509);

        [DllImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_GetX509NotAfter")]
        internal static extern IntPtr GetX509NotAfter(SafeX509Handle x509);

        [DllImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_GetX509CrlNextUpdate")]
        internal static extern IntPtr GetX509CrlNextUpdate(SafeX509CrlHandle crl);

        [DllImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_GetX509Version")]
        internal static extern int GetX509Version(SafeX509Handle x509);

        [DllImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_GetX509SignatureAlgorithm")]
        internal static extern IntPtr GetX509SignatureAlgorithm(SafeX509Handle x509);

        [DllImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_GetX509PublicKeyAlgorithm")]
        internal static extern IntPtr GetX509PublicKeyAlgorithm(SafeX509Handle x509);

        [DllImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_GetX509PublicKeyParameterBytes")]
        private static extern int GetX509PublicKeyParameterBytes(SafeX509Handle x509, byte[] buf, int cBuf);

        [DllImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_GetX509PublicKeyBytes")]
        internal static extern IntPtr GetX509PublicKeyBytes(SafeX509Handle x509);

        [DllImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_GetX509EkuFieldCount")]
        internal static extern int GetX509EkuFieldCount(SafeEkuExtensionHandle eku);

        [DllImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_GetX509EkuField")]
        internal static extern IntPtr GetX509EkuField(SafeEkuExtensionHandle eku, int loc);

        [DllImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_GetX509NameInfo")]
        internal static extern SafeBioHandle GetX509NameInfo(SafeX509Handle x509, int nameType, [MarshalAs(UnmanagedType.Bool)] bool forIssuer);

        [DllImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_GetAsn1StringBytes")]
        private static extern int GetAsn1StringBytes(IntPtr asn1, byte[] buf, int cBuf);

        [DllImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_PushX509StackField")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool PushX509StackField(SafeX509StackHandle stack, SafeX509Handle x509);

        internal static string GetX509RootStorePath()
        {
            return Marshal.PtrToStringAnsi(GetX509RootStorePath_private());
        }

        [DllImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_GetX509RootStorePath")]
        private static extern IntPtr GetX509RootStorePath_private();

        [DllImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_SetX509ChainVerifyTime")]
        private static extern int SetX509ChainVerifyTime(
            SafeX509StoreCtxHandle ctx,
            int year,
            int month,
            int day,
            int hour,
            int minute,
            int second,
            [MarshalAs(UnmanagedType.Bool)] bool isDst);

        [DllImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_CheckX509IpAddress")]
        internal static extern int CheckX509IpAddress(SafeX509Handle x509, [In]byte[] addressBytes, int addressLen, string hostname, int cchHostname);

        [DllImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_CheckX509Hostname")]
        internal static extern int CheckX509Hostname(SafeX509Handle x509, string hostname, int cchHostname);

        internal static byte[] GetAsn1StringBytes(IntPtr asn1)
        {
            return GetDynamicBuffer((ptr, buf, i) => GetAsn1StringBytes(ptr, buf, i), asn1);
        }

        internal static byte[] GetX509Thumbprint(SafeX509Handle x509)
        {
            return GetDynamicBuffer((handle, buf, i) => GetX509Thumbprint(handle, buf, i), x509);
        }

        internal static X500DistinguishedName LoadX500Name(IntPtr namePtr)
        {
            CheckValidOpenSslHandle(namePtr);

            byte[] buf = GetDynamicBuffer((ptr, buf1, i) => GetX509NameRawBytes(ptr, buf1, i), namePtr);
            return new X500DistinguishedName(buf);
        }

        internal static byte[] GetX509PublicKeyParameterBytes(SafeX509Handle x509)
        {
            return GetDynamicBuffer((handle, buf, i) => GetX509PublicKeyParameterBytes(handle, buf, i), x509);
        }

        internal static void SetX509ChainVerifyTime(SafeX509StoreCtxHandle ctx, DateTime verifyTime)
        {
            // OpenSSL is going to convert our input time to universal, so we should be in Local or
            // Unspecified (local-assumed).
            Debug.Assert(verifyTime.Kind != DateTimeKind.Utc, "UTC verifyTime should have been normalized to Local");
            
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
                throw Interop.Crypto.CreateOpenSslCryptographicException();
            }
        }

        private static byte[] GetDynamicBuffer<THandle>(NegativeSizeReadMethod<THandle> method, THandle handle)
        {
            int negativeSize = method(handle, null, 0);

            if (negativeSize > 0)
            {
                throw Interop.Crypto.CreateOpenSslCryptographicException();
            }

            byte[] bytes = new byte[-negativeSize];

            int ret = method(handle, bytes, bytes.Length);

            if (ret != 1)
            {
                throw Interop.Crypto.CreateOpenSslCryptographicException();
            }

            return bytes;
        }
    }
}
