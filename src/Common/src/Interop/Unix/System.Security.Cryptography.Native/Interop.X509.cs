// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Runtime.InteropServices;
using System.Security.Cryptography;
using Microsoft.Win32.SafeHandles;

internal static partial class Interop
{
    internal static partial class Crypto
    {
        [DllImport(Libraries.CryptoNative)]
        internal static extern SafeEvpPKeyHandle GetX509EvpPublicKey(SafeX509Handle x509);

        [DllImport(Libraries.CryptoNative)]
        internal static extern SafeX509CrlHandle DecodeX509Crl(byte[] buf, int len);

        [DllImport(Libraries.CryptoNative)]
        internal static extern SafeX509Handle DecodeX509(byte[] buf, int len);
    }
}
