// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Security.Cryptography.Pkcs;
using System.Security.Cryptography.X509Certificates;

using Microsoft.Win32.SafeHandles;

namespace Internal.Cryptography.Pal.OpenSsl
{
    internal sealed partial class DecryptorPalOpenSsl : DecryptorPal
    {
        public static DecryptorPalOpenSsl Decode(
            byte[] encodedMessage,
            out int version,
            out ContentInfo contentInfo,
            out AlgorithmIdentifier contentEncryptionAlgorithm,
            out X509Certificate2Collection originatorCerts,
            out CryptographicAttributeObjectCollection unprotectedAttributes)
        {
            SafeCmsHandle cmsHandle = Interop.Crypto.CmsDecode(encodedMessage, encodedMessage.Length);
            Interop.Crypto.CheckValidOpenSslHandle(cmsHandle);

            string oid = cmsHandle.GetContentType();

            if (oid != Oids.Pkcs7Enveloped)
                throw new CryptographicException(SR.Cryptography_Cms_Invalid_Message_Type);

            originatorCerts = cmsHandle.GetOriginatorCerts();
            contentInfo = cmsHandle.GetEmbeddedContent();
            RecipientInfoCollection recipientInfos;
            try
            {
                // Some fields of the CMS are not exposed by the OpenSSL CMS API, we have to parse this values from
                // the DER encoded message. In case there's an error decoding we need to throw an exception and release the
                // message and certificates as at this point it makes no sense to assume metadata is correct nor that decrypt
                // can be called after this.
                ParseMissingValues(encodedMessage, out version, out recipientInfos, out contentEncryptionAlgorithm, out unprotectedAttributes);
            }
            catch (InvalidOperationException e)
            {
                foreach (X509Certificate2 cert in originatorCerts)
                {
                    cert.Dispose();
                }
                cmsHandle.Dispose();

                contentInfo = null;
                contentEncryptionAlgorithm = null;
                originatorCerts = null;
                unprotectedAttributes = null;

                throw new CryptographicException(SR.Cryptography_Der_Invalid_Encoding, e);
            }

            return new DecryptorPalOpenSsl(cmsHandle, recipientInfos);
        }

        private static void ParseMissingValues(
            byte[] encodedMessage,
            out int version,
            out RecipientInfoCollection recipientInfos,
            out AlgorithmIdentifier contentEncryptionAlgorithm,
            out CryptographicAttributeObjectCollection unprotectedAttributes)
        {
            // DerSequenceReader needed to expose the parts of the message that the OpenSsl library doesn't
            // expose in their CMS API. 
            DerSequenceReader encodedCms = new DerSequenceReader(encodedMessage);
            
            // While the other elements are obtained from interacting from OpenSsl, these values had to
            // be obtained from parsing the encoding as there seems to be no way of obtaining them through 
            // their API. As such, don't move these around as they need to be read in this order

            // According to RFC2630, the enveloped type we have support for has the following structure
            // EnvelopedData ::= SEQUENCE {
            //     version CMSVersion,
            //     originatorInfo[0] IMPLICIT OriginatorInfo OPTIONAL,
            //     recipientInfos RecipientInfos,
            //     encryptedContentInfo EncryptedContentInfo,
            //     unprotectedAttrs[1] IMPLICIT UnprotectedAttributes OPTIONAL }

            // Read the version, but first skip the OID associated with the message, and open the inner constructed
            // sequence. 
            Debug.Assert(
                encodedCms.PeekTag() == (byte)DerSequenceReader.DerTag.ObjectIdentifier,
                "Expected to skip an OID while reading version");

            encodedCms.SkipValue();

            DerSequenceReader innerSequenceReader = encodedCms.ReadExplicitContextSequence();
            version = innerSequenceReader.ReadInteger();

            // Skip the OriginatorInfo in case it is present as this is done by OpenSSL
            const byte constructedImplicitContext0 = DerSequenceReader.ContextSpecificTagFlag | DerSequenceReader.ConstructedFlag;
            if (innerSequenceReader.PeekTag() == constructedImplicitContext0)
            {
                innerSequenceReader.SkipValue();
            }

            recipientInfos = ReadRecipientInfos(innerSequenceReader);
            contentEncryptionAlgorithm = innerSequenceReader.ReadAlgoIdFromEncryptedContentInfo();
            unprotectedAttributes = innerSequenceReader.ReadUnprotectedAttributes();
        }
    }
}
