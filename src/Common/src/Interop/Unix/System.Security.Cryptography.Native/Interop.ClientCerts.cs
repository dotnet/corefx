// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

internal static partial class Interop
{
    internal static partial class Crypto
    {
        [DllImport(Libraries.CryptoNative)]
        internal static extern Interop.libssl.SafeSslContextHandle GetSslContextFromSsl(Interop.libssl.SafeSslHandle  ssl);

        [DllImport(Libraries.CryptoNative)]
        internal static extern void SetSslContextClientCertCallback(
            IntPtr sslContext,
            Interop.libssl.client_cert_cb callback);

        [DllImport(Libraries.CryptoNative)]
        internal static extern int AddExtraChainCertToSslCtx(Interop.libssl.SafeSslContextHandle  sslContext, SafeX509Handle x509);
    }
}
