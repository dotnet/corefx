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
        internal static extern void BigNumDestroy(IntPtr a);

        [DllImport(Libraries.CryptoNative)]
        private static extern IntPtr BigNumFromBinary(byte[] s, int len);

        [DllImport(Libraries.CryptoNative)]
        private static extern unsafe int BigNumToBinary(SafeBignumHandle a, byte* to);

        [DllImport(Libraries.CryptoNative)]
        private static extern int GetBigNumBytes(SafeBignumHandle a);

        private static IntPtr CreateBignumPtr(byte[] bigEndianValue)
        {
            if (bigEndianValue == null)
            {
                return IntPtr.Zero;
            }

            return BigNumFromBinary(bigEndianValue, bigEndianValue.Length);
        }

        internal static SafeBignumHandle CreateBignum(byte[] bigEndianValue)
        {
            IntPtr handle = CreateBignumPtr(bigEndianValue);
            return new SafeBignumHandle(handle, true);
        }

        internal static byte[] ExtractBignum(IntPtr bignum, int targetSize)
        {
            // Given that the only reference held to bignum is an IntPtr, create an unowned SafeHandle
            // to ensure that we don't destroy the key after extraction.
            using (SafeBignumHandle handle = new SafeBignumHandle(bignum, ownsHandle: false))
            {
                return ExtractBignum(handle, targetSize);
            }
        }

        private static unsafe byte[] ExtractBignum(SafeBignumHandle bignum, int targetSize)
        {
            if (bignum == null || bignum.IsInvalid)
            {
                return null;
            }

            int compactSize = GetBigNumBytes(bignum);

            if (targetSize < compactSize)
            {
                targetSize = compactSize;
            }

            // OpenSSL BIGNUM values do not record leading zeroes.
            // Windows Crypt32 does.
            //
            // Since RSACryptoServiceProvider already checks that RSAParameters.DP.Length is
            // exactly half of RSAParameters.Modulus.Length, we need to left-pad (big-endian)
            // the array with zeroes.
            int offset = targetSize - compactSize;

            byte[] buf = new byte[targetSize];

            fixed (byte* to = buf)
            {
                byte* start = to + offset;
                BigNumToBinary(bignum, start);
            }

            return buf;
        }
    }
}
