// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Text;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Security.Cryptography.Pkcs;
using System.Security.Cryptography.X509Certificates;

namespace Internal.Cryptography
{
    internal abstract partial class PkcsPal
    {
        protected PkcsPal()
        {
        }

        /// <summary>
        /// Encrypt and encode a CMS. Return value is the RFC-compliant representation of the CMS that can be transmitted "on the wire."
        /// </summary>
        public abstract byte[] Encrypt(CmsRecipientCollection recipients, ContentInfo contentInfo, AlgorithmIdentifier contentEncryptionAlgorithm, X509Certificate2Collection originatorCerts, CryptographicAttributeObjectCollection unprotectedAttributes);

        /// <summary>
        /// Decode an encoded CMS.
        ///    Call RecipientInfos on the returned pal object to get the recipients.
        ///    Call TryDecrypt() on the returned pal object to attempt a decrypt for a single recipient.
        /// </summary>
        public abstract DecryptorPal Decode(byte[] encodedMessage, out int version, out ContentInfo contentInfo, out AlgorithmIdentifier contentEncryptionAlgorithm, out X509Certificate2Collection originatorCerts, out CryptographicAttributeObjectCollection unprotectedAttributes);

        // 
        // Encoders and decoders. These should be moved out of the Pal once we have a managed DER encoder/decoder api.
        //
        public abstract byte[] EncodeOctetString(byte[] octets);
        public abstract byte[] DecodeOctetString(byte[] encodedOctets);

        public abstract byte[] EncodeUtcTime(DateTime utcTime);
        public abstract DateTime DecodeUtcTime(byte[] encodedUtcTime);

        public abstract string DecodeOid(byte[] encodedOid);

        /// <summary>
        /// Implements the ContentInfo.GetContentType() behavior.
        /// </summary>
        public abstract Oid GetEncodedMessageType(byte[] encodedMessage);

        /// <summary>
        /// EnvelopedCms.Decrypt() looks for qualifying certs from the "MY" store (as well as any "extraStore" passed to Decrypt()).
        /// This method encapsulates exactly what a particular OS considers to be "the MY store."
        /// </summary>
        public abstract void AddCertsFromStoreForDecryption(X509Certificate2Collection certs);

        /// <summary>
        /// If EnvelopedCms.Decrypt() fails to find any matching certs for any recipients, it throws CryptographicException(CRYPT_E_RECIPIENT_NOT_FOUND) on Windows.
        /// This method encapsulates what other OS's decide to throw in this situation.
        /// </summary>
        public abstract Exception CreateRecipientsNotFoundException();

        /// <summary>
        /// If you call RecipientInfos after an Encrypt(), the framework throws CryptographicException(CRYPT_E_INVALID_MSG_TYPE) on Windows.
        /// This method encapsulates what other OS's decide to throw in this situation.
        /// </summary>
        public abstract Exception CreateRecipientInfosAfterEncryptException();

        /// <summary>
        /// If you call Decrypt() after an Encrypt(), the framework throws CryptographicException(CRYPT_E_INVALID_MSG_TYPE) on Windows.
        /// This method encapsulates what other OS's decide to throw in this situation.
        /// </summary>
        public abstract Exception CreateDecryptAfterEncryptException();

        /// <summary>
        /// If you call Decrypt() after a Encrypt(), the framework throws CryptographicException(CRYPT_E_INVALID_MSG_TYPE) on Windows.
        /// This method encapsulates what other OS's decide to throw in this situation.
        /// </summary>
        public abstract Exception CreateDecryptTwiceException();

        /// <summary>
        /// Retrieve the certificate's subject key identifier value.
        /// </summary>
        public abstract byte[] GetSubjectKeyIdentifier(X509Certificate2 certificate);

        /// <summary>
        /// Get the one (and only) instance of PkcsPal.
        /// </summary>
        public static PkcsPal Instance
        {
            get
            {
                // Wondering where "s_instance" is declared? It's declared in Pal\Windows\PkcsPal.cs and Pal\Unix\PkcsPal.cs, since the static initializer
                // for that field is platform-specific.
                return s_instance;
            }
        }
    }
}

