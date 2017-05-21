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
        [DllImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_RsaCreate")]
        internal static extern SafeRsaHandle RsaCreate();

        [DllImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_RsaUpRef")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool RsaUpRef(IntPtr rsa);

        [DllImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_RsaDestroy")]
        internal static extern void RsaDestroy(IntPtr rsa);

        [DllImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_DecodeRsaPublicKey")]
        internal static extern SafeRsaHandle DecodeRsaPublicKey(byte[] buf, int len);

        [DllImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_RsaPublicEncrypt")]
        internal extern static int RsaPublicEncrypt(
            int flen,
            byte[] from,
            byte[] to,
            SafeRsaHandle rsa,
            RsaPadding padding);

        [DllImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_RsaPrivateDecrypt")]
        internal extern static int RsaPrivateDecrypt(
            int flen,
            byte[] from,
            byte[] to,
            SafeRsaHandle rsa,
            RsaPadding padding);

        [DllImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_RsaSize")]
        internal static extern int RsaSize(SafeRsaHandle rsa);

        [DllImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_RsaGenerateKeyEx")]
        internal static extern int RsaGenerateKeyEx(SafeRsaHandle rsa, int bits, SafeBignumHandle e);

        [DllImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_RsaSign")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool RsaSign(int type, byte[] m, int m_len, byte[] sigret, out int siglen, SafeRsaHandle rsa);

        [DllImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_RsaVerify")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool RsaVerify(int type, byte[] m, int m_len, byte[] sigbuf, int siglen, SafeRsaHandle rsa);

        internal static RSAParameters ExportRsaParameters(SafeRsaHandle key, bool includePrivateParameters)
        {
            Debug.Assert(
                key != null && !key.IsInvalid,
                "Callers should check the key is invalid and throw an exception with a message");

            if (key == null || key.IsInvalid)
            {
                throw new CryptographicException();
            }

            bool addedRef = false;

            try
            {
                key.DangerousAddRef(ref addedRef);

                IntPtr n, e, d, p, dmp1, q, dmq1, iqmp;
                if (!GetRsaParameters(key, out n, out e, out d, out p, out dmp1, out q, out dmq1, out iqmp))
                {
                    throw new CryptographicException();
                }

                int modulusSize = Crypto.RsaSize(key);

                // RSACryptoServiceProvider expects P, DP, Q, DQ, and InverseQ to all
                // be padded up to half the modulus size.
                int halfModulus = modulusSize / 2;

                RSAParameters rsaParameters = new RSAParameters
                {
                    Modulus = Crypto.ExtractBignum(n, modulusSize),
                    Exponent = Crypto.ExtractBignum(e, 0),
                };

                if (includePrivateParameters)
                {
                    rsaParameters.D = Crypto.ExtractBignum(d, modulusSize);
                    rsaParameters.P = Crypto.ExtractBignum(p, halfModulus);
                    rsaParameters.DP = Crypto.ExtractBignum(dmp1, halfModulus);
                    rsaParameters.Q = Crypto.ExtractBignum(q, halfModulus);
                    rsaParameters.DQ = Crypto.ExtractBignum(dmq1, halfModulus);
                    rsaParameters.InverseQ = Crypto.ExtractBignum(iqmp, halfModulus);
                }

                return rsaParameters;
            }
            finally
            {
                if (addedRef)
                    key.DangerousRelease();
            }
        }

        [DllImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_GetRsaParameters")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetRsaParameters(
            SafeRsaHandle key,
            out IntPtr n,
            out IntPtr e,
            out IntPtr d,
            out IntPtr p,
            out IntPtr dmp1,
            out IntPtr q,
            out IntPtr dmq1,
            out IntPtr iqmp);

        [DllImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_SetRsaParameters")]
        internal static extern void SetRsaParameters(
            SafeRsaHandle key,
            byte[] n,
            int nLength,
            byte[] e,
            int eLength,
            byte[] d,
            int dLength,
            byte[] p,
            int pLength,
            byte[] dmp1,
            int dmp1Length,
            byte[] q,
            int qLength,
            byte[] dmq1,
            int dmq1Length,
            byte[] iqmp,
            int iqmpLength);

        internal enum RsaPadding : int
        {
            Pkcs1 = 0,
            OaepSHA1 = 1,
        }
    }
}
