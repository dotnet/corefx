// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;

namespace System.Security.Cryptography.Pkcs.Tests
{
    internal static class SupportedBehaviors
    {
        // As of OpenSSL 1.0.2g, there is no support for the use of KeyAgreement certificates to encrypt
        // enveloped messages
        public static bool SupportsKeyAgreementCerts => RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
        public static bool DoesNotSupportKeyAgreementCerts => !SupportsKeyAgreementCerts;

        // As of OpenSSL 1.0.2g, there's no support for adding originator certificates to a newly instantiated CMS_ContentInfo structure
        // so we can't support this for the encryption process.
        public static bool EncryptionSupportsAddingOriginatorCerts =>
            RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

        public static bool EncryptionDoesNotSupportAddingOriginatorCerts => !EncryptionSupportsAddingOriginatorCerts;

        // As OpenSSL uses EVP_CIPHER_CTX to support ciphers with variable key length but their CMS API uses a EVP_CIPHER
        // there's no direct way to support changing the key length.
        public static bool EncryptionSupportsVariableKeyLengths => RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

        public static bool EncryptionDoesNotSupportVariableKeyLengths => !EncryptionSupportsVariableKeyLengths;
    }
}
