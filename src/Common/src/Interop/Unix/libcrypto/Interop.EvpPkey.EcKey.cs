// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Runtime.InteropServices;
using System.Security.Cryptography;

using Microsoft.Win32.SafeHandles;

internal static partial class Interop
{
    internal static partial class libcrypto
    {
        [DllImport(Libraries.LibCrypto)]
        internal static extern SafeEcKeyHandle EVP_PKEY_get1_EC_KEY(SafeEvpPKeyHandle pkey);

        [DllImport(Libraries.LibCrypto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool EVP_PKEY_set1_EC_KEY(SafeEvpPKeyHandle pkey, SafeEcKeyHandle rsa);
    }
}
