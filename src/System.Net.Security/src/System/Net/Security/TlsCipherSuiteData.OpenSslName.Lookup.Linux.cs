// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Win32.SafeHandles;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Authentication;
using System.Security.Cryptography;

namespace System.Net.Security
{
    using CS = TlsCipherSuite;

    internal partial struct TlsCipherSuiteData
    {
        public static string GetOpenSslName(SafeSslHandle ssl, CS cipherSuite, out bool isTls12OrLower)
        {
            return Interop.Ssl.GetOpenSslCipherSuiteName(ssl, (ushort)cipherSuite, out isTls12OrLower);
        }
    }
}
