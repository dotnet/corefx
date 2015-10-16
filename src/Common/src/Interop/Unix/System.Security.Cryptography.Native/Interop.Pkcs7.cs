﻿// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

internal static partial class Interop
{
    internal static partial class Crypto
    {
        [DllImport(Libraries.CryptoNative)]
        internal static extern SafePkcs7Handle PemReadBioPkcs7(SafeBioHandle bp);

        [DllImport(Libraries.CryptoNative)]
        internal static extern SafePkcs7Handle DecodePkcs7(byte[] buf, int len);

        [DllImport(Libraries.CryptoNative)]
        internal static extern SafePkcs7Handle D2IPkcs7Bio(SafeBioHandle bp);

        [DllImport(Libraries.CryptoNative)]
        internal static extern void Pkcs7Destroy(IntPtr p7);

        [DllImport(Libraries.CryptoNative)]
        private static extern int GetPkcs7Certificates(SafePkcs7Handle p7, out SafeSharedX509StackHandle certs);

        internal static SafeSharedX509StackHandle GetPkcs7Certificates(SafePkcs7Handle p7)
        {
            if (p7 == null || p7.IsInvalid)
            {
                return SafeSharedX509StackHandle.InvalidHandle;
            }

            SafeSharedX509StackHandle certs;
            int result = GetPkcs7Certificates(p7, out certs);

            if (result != 1)
            {
                throw Interop.Crypto.CreateOpenSslCryptographicException();
            }

            // Track the parent relationship for the interior pointer so lifetime is well-managed.
            certs.SetParent(p7);

            return certs;
        }
    }
}
