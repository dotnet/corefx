// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
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

            if (originatorCerts.Count != 0)
            {
                // OpenSSL doesn't support adding certificates to a freshly created EnvelopedCMS (CMS_ContentInfo inside OpenSSL)
                // these are used normally for KeyAgreement, but OpenSSL doesn't have support for it so we can't add these.
                throw new PlatformNotSupportedException(SR.Cryptography_Cms_AddOriginatorCertsPlatformNotSupported);
            }

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

                    if (contentInfo.ContentType.Value != Oids.Pkcs7Data)
                    {
                        using (SafeAsn1ObjectHandle oid = Interop.Crypto.ObjTxt2Obj(contentInfo.ContentType.Value))
                        {
                            Interop.Crypto.CheckValidOpenSslHandle(oid);
                            status = Interop.Crypto.CmsSetEmbeddedContentType(cms, oid);
                            CheckStatus(status);
                        }
                    }

                    Interop.Crypto.BioWrite(contentBio, contentInfo.Content, contentInfo.Content.Length);

                    // We have to do this to simulate the behavior for zero length content  
                    bool detached = (contentInfo.Content.Length == 0);

                    status = Interop.Crypto.CmsCompleteMessage(cms, contentBio, detached);
                    CheckStatus(status);

                    byte[] encryptedMessage = Interop.Crypto.OpenSslEncode(
                        handle => Interop.Crypto.CmsGetDerSize(handle),
                        (handle, buf) => Interop.Crypto.CmsEncode(handle, buf),
                        cms);
                    
                    if (unprotectedAttributes.Count != 0)
                    {
                        // TODO(3334): Add support for unprotected attributes here.
                        encryptedMessage = AddAttributesToEncoding(encryptedMessage, unprotectedAttributes);
                    }

                    return encryptedMessage;
                }
            }
        }

        private byte[] AddAttributesToEncoding(byte[] encryptedMessage, CryptographicAttributeObjectCollection unprotectedAttributes)
        {
            DerSequenceReader originalMessage = new DerSequenceReader(encryptedMessage);

            // EnvelopedCms has the following underlying structure:
            //
            // ContentInfo ::= SEQUENCE {
            //      contentType ContentType,
            //      content[0] EXPLICIT ANY DEFINED BY contentType }
            //
            // ContentType ::= OBJECT IDENTIFIER
            //
            // According to RFC2630, the enveloped type we have support for has the following structure
            // EnvelopedData ::= SEQUENCE {
            //     version CMSVersion,
            //     originatorInfo[0] IMPLICIT OriginatorInfo OPTIONAL,
            //     recipientInfos RecipientInfos,
            //     encryptedContentInfo EncryptedContentInfo,
            //     unprotectedAttrs[1] IMPLICIT UnprotectedAttributes OPTIONAL }

            byte[][] contentType = originalMessage.ReadAndSplitNextEncodedValue();
            DerSequenceReader envelopedDataReader = originalMessage.ReadExplicitContextSequence();

            byte[][] version = envelopedDataReader.ReadAndSplitNextEncodedValue();

            const byte ConstructedContextSpecificTag = DerSequenceReader.ContextSpecificTagFlag | DerSequenceReader.ConstructedFlag;

            byte[][] originatorInfo = (envelopedDataReader.PeekTag() == ConstructedContextSpecificTag) ?
                originatorInfo = envelopedDataReader.ReadAndSplitNextEncodedValue() :
                new byte[][] { Array.Empty<byte>(), Array.Empty<byte>(), Array.Empty<byte>() };

            byte[][] recipientInfos = envelopedDataReader.ReadAndSplitNextEncodedValue();
            byte[][] encryptedContentInfo = envelopedDataReader.ReadAndSplitNextEncodedValue();

            byte[][] envelopedData = DerEncoder.ConstructSegmentedExplicitSequence(
                0 /* Context number */,
                version,
                originatorInfo,
                recipientInfos,
                encryptedContentInfo,
                EncodeAttributes(unprotectedAttributes));

            return DerEncoder.ConstructSequence(
                contentType,
                envelopedData);
        }

        private byte[][] EncodeAttributes(CryptographicAttributeObjectCollection unprotectedAttributes)
        {
            // In EnvelopedData UnprotectedAttributes are included as:
            //
            // unprotectedAttrs[1] IMPLICIT UnprotectedAttributes OPTIONAL
            //
            // UnprotectedAttributes ::= SET SIZE (1..MAX) OF Attribute
            // Attribute::= SEQUENCE {
            //      attrType OBJECT IDENTIFIER,
            //      attrValues SET OF AttributeValue }

            byte[][][] setOfAttrs = new byte[unprotectedAttributes.Count][][];
            
            for (int i = 0; i < unprotectedAttributes.Count; i++)
            {
                byte[][] attrType = DerEncoder.SegmentedEncodeOid(unprotectedAttributes[i].Oid);
                byte[][][] attrValues = new byte[unprotectedAttributes[i].Values.Count][][];

                for (int j = 0; j < attrValues.Length; j++)
                {
                    attrValues[j] = DerSequenceReader.SplitValue(unprotectedAttributes[i].Values[j].RawData);
                }

                byte[][] attrValueSet = DerEncoder.ConstructSegmentedSet(attrValues);
                byte[][] encodedAttribute = DerEncoder.ConstructSegmentedSequence(attrType, attrValueSet);
                setOfAttrs[i] = encodedAttribute;
            }

            // There's no difference between an implicit set and an implicit sequence in the tag,
            // so we can just call this.
            return DerEncoder.ConstructSegmentedImplicitSequence(1 /* Context number */, setOfAttrs);
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
