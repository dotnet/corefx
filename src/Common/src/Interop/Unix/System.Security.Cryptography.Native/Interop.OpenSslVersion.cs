// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;

internal static partial class Interop
{
    internal static partial class OpenSsl
    {
        private static Version s_opensslVersion;

        [DllImport(Libraries.CryptoNative, EntryPoint = "CryptoNative_OpenSslVersionNumber")]
        internal static extern long OpenSslVersionNumber();

        internal static Version OpenSslVersion
        {
            get
            {
                if (s_opensslVersion == null)
                {
                    // OpenSSL version numbers are encoded as
                    // 0xMNNFFPPS: major (one nybble), minor (one byte, unaligned),
                    // "fix" (one byte, unaligned), patch (one byte, unaligned), status (one nybble)
                    //
                    // e.g. 1.0.2a final is 0x1000201F
                    //
                    // Currently they don't exceed 29-bit values, but we use long here to account
                    // for the expanded range on their 64-bit C-long return value.
                    long versionNumber = OpenSslVersionNumber();
                    int major = (int)((versionNumber >> 28) & 0xF);
                    int minor = (int)((versionNumber >> 20) & 0xFF);
                    int fix = (int)((versionNumber >> 12) & 0xFF);

                    s_opensslVersion = new Version(major, minor, fix);
                }

                return s_opensslVersion;
            }
        }
    }
}
