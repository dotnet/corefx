// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Runtime.InteropServices;
using System.Security.Cryptography;

internal static partial class Interop
{
    internal static partial class libcrypto
    {
        [DllImport(Libraries.LibCrypto)]
        internal static extern SafeEvpPKeyHandle EVP_PKEY_new();

        [DllImport(Libraries.LibCrypto)]
        internal static extern void EVP_PKEY_free(IntPtr pkey);
    }
}
