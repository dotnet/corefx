// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Security.Cryptography;
using System.Security.Cryptography.Pkcs;
using System.Security.Cryptography.X509Certificates;

using Microsoft.Win32.SafeHandles;

namespace Internal.Cryptography.Pal.OpenSsl
{
    internal sealed partial class PkcsPalOpenSsl : PkcsPal
    {
        public sealed override byte[] Encrypt(
            CmsRecipientCollection recipients,
            ContentInfo contentInfo,
            AlgorithmIdentifier contentEncryptionAlgorithm,
            X509Certificate2Collection originatorCerts,
            CryptographicAttributeObjectCollection unprotectedAttributes)
        {
            int status;

            using (SafeBioHandle contentBio = Interop.Crypto.CreateMemoryBio())
            using (SafeAsn1ObjectHandle algoOid = Interop.Crypto.ObjTxt2Obj(contentEncryptionAlgorithm.Oid.Value))
            {
                Interop.Crypto.CheckValidOpenSslHandle(algoOid);
                Interop.Crypto.CheckValidOpenSslHandle(contentBio);

                using (SafeCmsHandle cms = Interop.Crypto.CmsInitializeEnvelope(algoOid, out status))
                {
                    if (status == 0)
                        throw new CryptographicException(SR.Cryptography_Cms_UnknownAlgorithm);

                    Interop.Crypto.CheckValidOpenSslHandle(cms);

                    ClassifyAndAddRecipients(cms, recipients);

                    foreach (X509Certificate2 cert in originatorCerts)
                    {
                        using (SafeX509Handle certHandle = Interop.Crypto.X509Duplicate(cert.Handle))
                        {
                            Interop.Crypto.CheckValidOpenSslHandle(certHandle);
                            status = Interop.Crypto.CmsAddOriginatorCert(cms, certHandle);
                            CheckStatus(status);
                        }
                    }
                        
                    Interop.Crypto.BioWrite(contentBio, contentInfo.Content, contentInfo.Content.Length);
                    status = Interop.Crypto.CmsCompleteMessage(cms, contentBio, false);
                    CheckStatus(status);

                    byte[] encryptedMessage = Interop.Crypto.OpenSslEncode(
                        handle => Interop.Crypto.CmsGetDerSize(handle),
                        (handle, buf) => Interop.Crypto.CmsEncode(handle, buf),
                        cms);

                    // TODO(3334): Add support for unprotected attributes here.

                    return encryptedMessage;
                }
            }
        }

        private void CheckStatus(int status)
        {
            switch(status)
            {
                case -1:
                    // We should never reach this state. It checks for null inputs given to shim functions, but as we
                    // always call CheckValidOpenSslHandle on a handle before using it there should never be such problem.
                    System.Diagnostics.Debug.Fail("Call to the shim recieved unexpected invalid input.");
                    throw new ArgumentNullException();

                case 0:
                    throw Interop.Crypto.CreateOpenSslCryptographicException();

                case 1:
                    // This is the expected state.
                    return;
            }
            System.Diagnostics.Debug.Fail($"Unexpected status from shim ({status})");
        }

        private void ClassifyAndAddRecipients(SafeCmsHandle cms, CmsRecipientCollection recipients)
        {
            int status;

            foreach (CmsRecipient recipient in recipients)
            {
                // This shouldn't happen as the CmsRecipient constructor throws an exception when a null recipient is passed,
                // but for desktop compat in case someone manages to change the certificate, this is the exception Framework throws.
                if (recipient.Certificate == null)
                    throw new ArgumentNullException(SR.Cryptography_Cms_RecipientCertificate_Not_Found);

                // We cannot encrypt KeyAgreement now as there is no native support for it in OpenSSL and it would require manual parsing
                // of the data, so we have to make sure it's KeyTransport.
                if (recipient.Certificate.GetKeyAlgorithm() != Oids.Rsa)
                    throw new PlatformNotSupportedException(SR.Cryptography_Cms_KeyAgreementPlatformNotSupported);

                using (SafeX509Handle certHandle = Interop.Crypto.X509Duplicate(recipient.Certificate.Handle))
                {
                    Interop.Crypto.CheckValidOpenSslHandle(certHandle);

                    switch (recipient.RecipientIdentifierType)
                    {
                        case SubjectIdentifierType.SubjectKeyIdentifier:
                            status = Interop.Crypto.CmsAddSkidRecipient(cms, certHandle);
                            CheckStatus(status);
                            break;

                        case SubjectIdentifierType.Unknown:
                        // For desktop compat, this needs to fall back to IssuerAndSerialNumber
                        case SubjectIdentifierType.IssuerAndSerialNumber:
                            status = Interop.Crypto.CmsAddIssuerAndSerialRecipient(cms, certHandle);
                            CheckStatus(status);
                            break;

                        default:
                            throw new CryptographicException(
                                SR.Format(SR.Cryptography_Cms_Invalid_Subject_Identifier_Type, recipient.RecipientIdentifierType));
                    }
                }
            }
        }
    }
}
