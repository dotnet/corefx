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
        public sealed override DecryptorPal Decode(byte[] encodedMessage,
            out int version,
            out ContentInfo contentInfo,
            out AlgorithmIdentifier contentEncryptionAlgorithm,
            out X509Certificate2Collection originatorCerts,
            out CryptographicAttributeObjectCollection unprotectedAttributes)
        {
            return DecryptorPalOpenSsl.Decode(
                encodedMessage,
                out version,
                out contentInfo,
                out contentEncryptionAlgorithm,
                out originatorCerts,
                out unprotectedAttributes);
        }

        public sealed override byte[] EncodeOctetString(byte[] octets)
        {
            return DerEncoder.EncodeOctetString(octets);
        }

        public sealed override byte[] DecodeOctetString(byte[] encodedOctets)
        {
            try
            {
                return DerSequenceReader.CreateForPayload(encodedOctets).ReadOctetString();
            }
            catch (InvalidOperationException e)
            {
                throw new CryptographicException(SR.Cryptography_Der_Invalid_Encoding, e);
            }
        }

        public sealed override byte[] EncodeUtcTime(DateTime utcTime)
        {
            return DerEncoder.EncodeUtcTime(utcTime);
        }

        public sealed override DateTime DecodeUtcTime(byte[] encodedUtcTime)
        {
            try
            {
                return DerSequenceReader.CreateForPayload(encodedUtcTime).ReadUtcTime();
            }
            catch (InvalidOperationException e)
            {
                throw new CryptographicException(SR.Cryptography_Der_Invalid_Encoding, e);
            }
        }

        public sealed override string DecodeOid(byte[] encodedOid)
        {
            try
            {
                return DerSequenceReader.CreateForPayload(encodedOid).ReadOidAsString();
            }
            catch(InvalidOperationException e)
            {
                throw new CryptographicException(SR.Cryptography_Der_Invalid_Encoding, e);
            }
        }

        /// <summary>
        /// Implements the ContentInfo.GetContentType() behavior.
        /// </summary>
        public sealed override Oid GetEncodedMessageType(byte[] encodedMessage)
        {
            using (SafeCmsHandle cmsHandle = Interop.Crypto.CmsDecode(encodedMessage, encodedMessage.Length))
            {
                Interop.Crypto.CheckValidOpenSslHandle(cmsHandle);
                return new Oid(cmsHandle.GetContentType());
            }
        }

        /// <summary>
        /// EnvelopedCms.Decrypt() looks for qualifying certs from the "MY" store (as well as any "extraStore" passed to Decrypt()).
        /// This method encapsulates exactly what a particular OS considers to be "the MY store."
        /// </summary>
        public sealed override void AddCertsFromStoreForDecryption(X509Certificate2Collection certs)
        {
            // Currently not searching the LM\My store as it's not supported on Unix
            certs.AddRange(Helpers.GetStoreCertificates(StoreName.My, StoreLocation.CurrentUser, openExistingOnly: false));
        }

        /// <summary>
        /// If EnvelopedCms.Decrypt() fails to find any matching certs for any recipients, it throws CryptographicException(CRYPT_E_RECIPIENT_NOT_FOUND) on Windows.
        /// This method encapsulates what other OS's decide to throw in this situation.
        /// </summary>
        public sealed override Exception CreateRecipientsNotFoundException()
        {
            return new CryptographicException(SR.Cryptography_Cms_Recipient_Not_Found);
        }

        /// <summary>
        /// If you call RecipientInfos after an Encrypt(), the framework throws CryptographicException(CRYPT_E_INVALID_MSG_TYPE) on Windows.
        /// This method encapsulates what other OS's decide to throw in this situation.
        /// </summary>
        public sealed override Exception CreateRecipientInfosAfterEncryptException()
        {
            return new CryptographicException(SR.Cryptography_Cms_Invalid_Message_Type);
        }

        /// <summary>
        /// If you call Decrypt() after an Encrypt(), the framework throws CryptographicException(CRYPT_E_INVALID_MSG_TYPE) on Windows.
        /// This method encapsulates what other OS's decide to throw in this situation.
        /// </summary>
        public sealed override Exception CreateDecryptAfterEncryptException()
        {
            return new CryptographicException(SR.Cryptography_Cms_Invalid_Message_Type);
        }

        /// <summary>
        /// If you call Decrupt() after a Decrypt(), the framework throws CryptographicException(CRYPT_E_INVALID_MSG_TYPE) on Windows.
        /// This method encapsulates what other OS's decide to throw in this situation.
        /// </summary>
        public sealed override Exception CreateDecryptTwiceException()
        {
            return new CryptographicException(SR.Cryptography_Cms_Invalid_Message_Type);
        }

        /// <summary>
        /// Retrieve the certificate's subject key identifier value.
        /// </summary>
        public sealed override byte[] GetSubjectKeyIdentifier(X509Certificate2 certificate)
        {
            X509ExtensionCollection certExtensions = certificate.Extensions;
            foreach (X509Extension ext in certExtensions)
            {
                if (ext != null && ext.Oid != null &&
                    StringComparer.Ordinal.Equals(Oids.SubjectKeyIdentifier, ext.Oid.Value))
                {
                    return DerSequenceReader.CreateForPayload(ext.RawData).ReadOctetString();
                }
            }

            // If we've reached this point, it means the certificate doesn't explicitly have a SubjectKeyIdentifier
            // extension, so one must be generated using SHA1
            //
            // As stated in OpenSslCertificateFinder:
            // The Desktop/Windows version of this method use CertGetCertificateContextProperty
            // with a property ID of CERT_KEY_IDENTIFIER_PROP_ID.
            //
            // MSDN says that when there's no extension, this method takes the SHA-1 of the
            // SubjectPublicKeyInfo block, and returns that.
            //
            // https://msdn.microsoft.com/en-us/library/windows/desktop/aa376079%28v=vs.85%29.aspx
            //
            // As of now we don't support this fallback, but we can't throw PlatformNotSupportedException yet
            // as there might be a recipient for which we have a certificate which we can use to decrypt.
            // If all certificates don't match then TryDecrypt will throw a "recipient not found" cryptographic
            // exception which explains OpenSSL's behavior.
            using (SafeX509Handle certHandle = Interop.Crypto.X509Duplicate(certificate.Handle))
            {
                return Interop.Crypto.X509GetImplicitSubjectKeyIdentifier(certHandle);
            }
        }
    }
}
