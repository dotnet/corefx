// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Security.Cryptography.Pkcs;
using System.Security.Cryptography.X509Certificates;

using Internal.Cryptography.Pal;
using Microsoft.Win32.SafeHandles;
using System.Collections.Generic;

namespace Internal.Cryptography.Pal.OpenSsl
{
    internal sealed partial class DecryptorPalOpenSsl : DecryptorPal
    {
        public sealed override ContentInfo TryDecrypt(
            RecipientInfo recipientInfo,
            X509Certificate2 cert,
            X509Certificate2Collection originatorCerts,
            X509Certificate2Collection extraStore,
            out Exception exception)
        {
            Debug.Assert(recipientInfo != null);
            Debug.Assert(cert != null);
            Debug.Assert(originatorCerts != null);
            Debug.Assert(extraStore != null);

            ContentInfo decryptedContent = null;
            RecipientInfoType type = recipientInfo.Type;

            switch (type)
            {
                case RecipientInfoType.KeyTransport:
                    decryptedContent = TryDecryptTrans(cert, out exception, recipientInfo.RecipientIdentifier.Type);
                    break;

                case RecipientInfoType.KeyAgreement:
                    // OpenSSL doesn't seem to support KeyAgreement directly, the only way to work around this seems to be to extract
                    // the information by parsing it and to store the appropriate keys to call the EVP_PKEY_derive method on them
                    // to obtain the secret and proceed with manual decryption. For further documentation see:
                    // https://wiki.openssl.org/index.php/EVP_Key_Agreement
                    exception = new PlatformNotSupportedException();
                    break;

                default:
                    // As we commented in the Windows implementation:
                    // Since only the framework can construct RecipientInfo's, we're at fault if we get here. So it's okay to assert and throw rather than
                    // returning to the caller.
                    Debug.Fail($"Unexpected RecipientInfoType: {type}");
                    throw new NotSupportedException();
            }

            if (exception != null)
                return null;

            return decryptedContent;
        }

        private ContentInfo TryDecryptTrans(X509Certificate2 recipientCert, out Exception exception, SubjectIdentifierType type)
        {
            // If there's no content OpenSSL will fail to decrypt, so do the check manually before
            // delegating to OpenSSL
            ContentInfo currentContent = _decodedMessage.GetEmbeddedContent();
            if (currentContent.Content.Length == 0)
            {
                exception = null;
                return currentContent;
            }

            byte[] decryptedMessage = null;

            using (RSA rsa = recipientCert.GetRSAPrivateKey())
            {
                RSAOpenSsl rsaOpenSsl = rsa as RSAOpenSsl;

                // If rsa is null, the certificate has no private key and we have no way of decrypting it.
                if (rsaOpenSsl == null)
                {
                    exception = new PlatformNotSupportedException();
                    return null;
                }

                using (SafeBioHandle decryptionBuffer = Interop.Crypto.CreateMemoryBio())
                using (SafeX509Handle recipientCertHandle = Interop.Crypto.X509Duplicate(recipientCert.Handle))
                using (SafeEvpPKeyHandle pKey = rsaOpenSsl.DuplicateKeyHandle())
                {
                    Interop.Crypto.CheckValidOpenSslHandle(decryptionBuffer);
                    Interop.Crypto.CheckValidOpenSslHandle(recipientCertHandle);
                    Interop.Crypto.CheckValidOpenSslHandle(pKey);

                    int status = Interop.Crypto.CmsDecrypt(_decodedMessage, recipientCertHandle, pKey, decryptionBuffer, type);

                    if (status != 1)
                    {
                        exception = Interop.Crypto.CreateOpenSslCryptographicException();
                        return null;
                    }

                    int bioSize = Interop.Crypto.GetMemoryBioSize(decryptionBuffer);

                    decryptedMessage = new byte[bioSize];

                    int read = Interop.Crypto.BioRead(decryptionBuffer, decryptedMessage, decryptedMessage.Length);

                    if (read < 0)
                        throw Interop.Crypto.CreateOpenSslCryptographicException();
                }
            }

            string oid;
            using (SafeSharedAsn1ObjectHandle oidAsn1 = Interop.Crypto.CmsGetEmbeddedContentType(_decodedMessage))
            {
                Interop.Crypto.CheckValidOpenSslHandle(oidAsn1);
                oid = Interop.Crypto.GetOidValue(oidAsn1);
            }

            exception = null;
            return new ContentInfo(new Oid(oid), decryptedMessage);
        }
    }
}
