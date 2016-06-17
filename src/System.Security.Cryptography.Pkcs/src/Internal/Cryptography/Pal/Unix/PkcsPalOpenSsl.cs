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
            // TODO(3334): see CMS_ContentInfo *CMS_encrypt(...) in cms.h
            throw new NotImplementedException();
        }

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
            // TODO(3334): Use certificate.PublicKey and the Subject properties with the encoder. 
            throw new NotImplementedException();
        }
    }
}
