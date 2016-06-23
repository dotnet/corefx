// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Security.Cryptography.Pkcs;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;

namespace Internal.Cryptography.Pal.OpenSsl
{
    internal sealed partial class DecryptorPalOpenSsl : DecryptorPal
    {
        private static RecipientInfoCollection ReadRecipientInfos(DerSequenceReader encodedCms)
        {
            DerSequenceReader recipientSet = encodedCms.ReadSet();
            List<RecipientInfo> recipientInfoStack = new List<RecipientInfo>();

            // RecipientInfo objects are defined as (in RFC 2630) 
            //
            // RecipientInfo ::= CHOICE {
            //      ktri KeyTransRecipientInfo
            //      kari [1] KeyAgreementRecipientInfo
            //      kekri [2] KEKRecipientInfo }
            //
            // New types have been added but we don't support them

            const byte ConstructedSequence = (byte)DerSequenceReader.DerTag.Sequence | DerSequenceReader.ConstructedFlag;
            const byte ContextSpecific1 = DerSequenceReader.ConstructedFlag | DerSequenceReader.ContextSpecificTagFlag | 0x01;

            while (recipientSet.HasData)
            {
                byte tag = recipientSet.PeekTag();
                
                switch (tag)
                {
                    case ConstructedSequence:
                        KeyTransRecipientInfo ktri = ReadKeyTrans(recipientSet);
                        recipientInfoStack.Add(ktri);
                        break;
                    case ContextSpecific1:
                        ReadAndAddKeyAgrees(recipientSet, recipientInfoStack);
                        break;
                    default:
                        // If the tag is another context specific, it corresponds to a recipient
                        // type we don't support. A PlatformNotSupportedException seems more fit for this,
                        // but for compat reasons with the Windows implementation this is the exception thrown. 
                        throw new CryptographicException(SR.Cryptography_Err_Not_Impl);
                }
            }

            return new RecipientInfoCollection(recipientInfoStack);
        }

        private static void ReadAndAddKeyAgrees(DerSequenceReader recipientSet, List<RecipientInfo> recipientInfoStack)
        {
            // KeyAgreeRecipientInfo::= SEQUENCE {
            //      version CMSVersion,  --always set to 3
            //      originator[0] EXPLICIT OriginatorIdentifierOrKey,
            //      ukm[1] EXPLICIT UserKeyingMaterial OPTIONAL,
            //      keyEncryptionAlgorithm KeyEncryptionAlgorithmIdentifier,
            //      recipientEncryptedKeys RecipientEncryptedKeys }

            DerSequenceReader encodedKeyAgreement = recipientSet.ReadSequence();
            int version = encodedKeyAgreement.ReadInteger();

            // OriginatorIdentifierOrKey::= CHOICE {
            //      issuerAndSerialNumber IssuerAndSerialNumber,
            //      subjectKeyIdentifier[0] SubjectKeyIdentifier,
            //      originatorKey[1] OriginatorPublicKey }

            const byte ConstructedSequence = (byte)DerSequenceReader.DerTag.Sequence | DerSequenceReader.ConstructedFlag;
            const byte ContextSpecific1 = DerSequenceReader.ConstructedFlag | DerSequenceReader.ContextSpecificTagFlag | 0x01;

            DerSequenceReader originator = encodedKeyAgreement.ReadSequence();
            byte tag = originator.PeekTag();

            SubjectIdentifierOrKey originatorIdentifierOrKey;

            switch (tag)
            {
                case ConstructedSequence:
                    X509IssuerSerial issuerSerial = ReadIssuerSerial(originator);

                    originatorIdentifierOrKey =
                        new SubjectIdentifierOrKey(SubjectIdentifierOrKeyType.IssuerAndSerialNumber, issuerSerial);

                    break;

                case DerSequenceReader.ContextSpecificTagFlag:
                    string skid = ReadSubjectKeyId(originator);

                    originatorIdentifierOrKey =
                        new SubjectIdentifierOrKey(SubjectIdentifierOrKeyType.SubjectKeyIdentifier, skid);

                    break;

                case ContextSpecific1:
                    PublicKeyInfo publicKeyInfo = ReadOriginatorPublicKey(originator);

                    originatorIdentifierOrKey =
                        new SubjectIdentifierOrKey(SubjectIdentifierOrKeyType.PublicKeyInfo, publicKeyInfo);

                    break;

                default:
                    throw new CryptographicException(SR.Format(SR.Cryptography_Cms_Invalid_Originator_Identifier_Choice, tag));
            }

            if (encodedKeyAgreement.PeekTag() == ContextSpecific1)
            {
                // Skip extra keying material; this one isn't exposed in the API and Framework does
                // nothing with it.
                encodedKeyAgreement.SkipValue();
            }

            AlgorithmIdentifier keyEncryptionAlgo = encodedKeyAgreement.ReadAlgoIdentifier();

            // As there might be more than one element in RecipientEncryptedKeys, we have to create one KeyAgreeRecipientInfo for each
            // one of the elements hold all the previously read values as common elements. We delegate this to ReadRecipientEncryptedKeys
            ReadRecipientEncryptedKeys(encodedKeyAgreement, version, originatorIdentifierOrKey, keyEncryptionAlgo, recipientInfoStack);            
        }

        private static PublicKeyInfo ReadOriginatorPublicKey(DerSequenceReader encodedKeyAgreement)
        {
            // OriginatorPublicKey::= SEQUENCE {
            //      algorithm AlgorithmIdentifier,
            //      publicKey BIT STRING }
            DerSequenceReader publicKeyReader = encodedKeyAgreement.ReadSequence();

            AlgorithmIdentifier publicKeyAlgorithm = publicKeyReader.ReadAlgoIdentifier();
            byte[] publicKey = publicKeyReader.ReadBitString();

            return new PublicKeyInfo(publicKeyAlgorithm, publicKey);
        }

        private static KeyTransRecipientInfo ReadKeyTrans(DerSequenceReader recipientSet)
        {

            // KeyTransRecipientInfo are defined in RFC 2630 as
            //
            // KeyTransRecipientInfo::= SEQUENCE {
            //      version CMSVersion,  --always set to 0 or 2
            //      rid RecipientIdentifier,
            //      keyEncryptionAlgorithm KeyEncryptionAlgorithmIdentifier,
            //      encryptedKey EncryptedKey }
            //
            // EncryptedKey::= OCTET STRING

            DerSequenceReader encodedKeyTransport = recipientSet.ReadSequence();
            int version = encodedKeyTransport.ReadInteger();

            // RecipientIdentifier is either SubjectKeyIdentifier or IssuerAndSerialNumber
            //
            // RecipientIdentifier ::= CHOICE {
            //      issuerAndSerialNumber IssuerAndSerialNumber,
            //      subjectKeyIdentifier[0] SubjectKeyIdentifier }

            byte tag = encodedKeyTransport.PeekTag();
            const byte ConstructedSequence = (byte)DerSequenceReader.DerTag.Sequence | DerSequenceReader.ConstructedFlag;

            SubjectIdentifier subject;
            switch (tag)
            {
                case ConstructedSequence:
                    subject = new SubjectIdentifier(SubjectIdentifierType.IssuerAndSerialNumber, ReadIssuerSerial(encodedKeyTransport));
                    break;
                case DerSequenceReader.ContextSpecificTagFlag:
                    subject = new SubjectIdentifier(SubjectIdentifierType.SubjectKeyIdentifier, ReadSubjectKeyId(encodedKeyTransport));
                    break;
                default:
                    throw new CryptographicException(SR.Format(SR.Cryptography_Cms_Invalid_Subject_Identifier_Type, tag));
            }

            AlgorithmIdentifier kekAlgoId = encodedKeyTransport.ReadAlgoIdentifier();
            byte[] encryptedKey = encodedKeyTransport.ReadOctetString();

            KeyTransRecipientInfoPalOpenSsl ktriObj =
                new KeyTransRecipientInfoPalOpenSsl(version, subject, kekAlgoId, encryptedKey);
            return new KeyTransRecipientInfo(ktriObj);
        }

        private static string ReadSubjectKeyId(DerSequenceReader recipientSet)
        {
            return recipientSet.ReadOctetString().ToSkiString();
        }

        private static X509IssuerSerial ReadIssuerSerial(DerSequenceReader recipientSet)
        {
            // According to RFC 2630:
            //
            // IssuerAndSerialNumber::= SEQUENCE {
            //    issuer Name,
            //    serialNumber CertificateSerialNumber }

            DerSequenceReader issuerAndSerial = recipientSet.ReadSequence();

            byte[] issuerNameEncoded = issuerAndSerial.ReadNextEncodedValue();
            string issuer = new X500DistinguishedName(issuerNameEncoded).Name;

            // CertificateSerialNumber ::= INTEGER
            //
            // Getting bytes as it can be a big integer. Order of the bytes in the reader is big endian so use
            // ToUpperHexString without reversing it.
            
            byte[] serial = issuerAndSerial.ReadIntegerBytes();
            return new X509IssuerSerial(issuer, serial.ToUpperHexString());
        }

        private static void ReadRecipientEncryptedKeys(
            DerSequenceReader keyAgreement,
            int version,
            SubjectIdentifierOrKey originatorId,
            AlgorithmIdentifier keyEncryptAlgo,
            List<RecipientInfo> recipientStack)
        {
            // RecipientEncryptedKeys::= SEQUENCE OF RecipientEncryptedKey
            
            DerSequenceReader recipientEncryptedKeySequence = keyAgreement.ReadSequence();

            const byte ContextConstructed0 = DerSequenceReader.ContextSpecificTagFlag | DerSequenceReader.ConstructedFlag;
            const byte ConstructedSequence = (byte)DerSequenceReader.DerTag.Sequence | DerSequenceReader.ConstructedFlag;

            while (recipientEncryptedKeySequence.HasData)
            {   
                // This is to match the default behavior from Framework in case the values are not set as they are
                // optional in the encoding
                DateTime date = DateTime.FromFileTimeUtc(0);
                CryptographicAttributeObject otherKeyMaterial = null;

                // RecipientEncryptedKey::= SEQUENCE {
                //      rid KeyAgreeRecipientIdentifier,
                //      encryptedKey EncryptedKey }

                DerSequenceReader recipientEncryptedKey = recipientEncryptedKeySequence.ReadSequence();
                SubjectIdentifier recipientId;
                byte tag = recipientEncryptedKey.PeekTag();
                
                // KeyAgreeRecipientIdentifier::= CHOICE {
                //    issuerAndSerialNumber IssuerAndSerialNumber,
                //    rKeyId[0] IMPLICIT RecipientKeyIdentifier }

                switch (tag)
                { 
                    case ContextConstructed0:
                        // RecipientKeyIdentifier::= SEQUENCE {
                        //      subjectKeyIdentifier SubjectKeyIdentifier,
                        //      date GeneralizedTime OPTIONAL,
                        //      other OtherKeyAttribute OPTIONAL }
                        DerSequenceReader recipientKeyIdentifier = recipientEncryptedKey.ReadSequence();
                        recipientId
                            = new SubjectIdentifier(SubjectIdentifierType.SubjectKeyIdentifier, ReadSubjectKeyId(recipientKeyIdentifier));

                        if (recipientKeyIdentifier.PeekTag() == (byte)DerSequenceReader.DerTag.GeneralizedTime)
                        {
                            date = recipientKeyIdentifier.ReadGeneralizedTime();
                        }

                        if (recipientKeyIdentifier.PeekTag() == ConstructedSequence)
                        {
                            // OtherKeyAttribute::= SEQUENCE {
                            //      keyAttrId OBJECT IDENTIFIER,
                            //      keyAttr ANY DEFINED BY keyAttrId OPTIONAL }

                            DerSequenceReader otherKeyAttribute = recipientKeyIdentifier.ReadSequence();
                            Oid oid = otherKeyAttribute.ReadOid();
                            byte[] payload = otherKeyAttribute.ReadNextEncodedValue();
                            AsnEncodedData attr = Helpers.CreateBestPkcs9AttributeObjectAvailable(oid, payload);
                            otherKeyMaterial = new CryptographicAttributeObject(oid, new AsnEncodedDataCollection(attr));
                        }
                        break;

                    case ConstructedSequence:
                        recipientId = new SubjectIdentifier(SubjectIdentifierType.IssuerAndSerialNumber, ReadIssuerSerial(recipientEncryptedKey));
                        break;

                    default:
                        throw new CryptographicException(SR.Format(SR.Cryptography_Cms_Invalid_Subject_Identifier_Type, tag));
                }

                byte[] encryptionKey = recipientEncryptedKey.ReadOctetString();

                KeyAgreeRecipientInfoPalOpenSsl kari = new KeyAgreeRecipientInfoPalOpenSsl(
                        version,
                        recipientId,
                        originatorId,
                        otherKeyMaterial,
                        keyEncryptAlgo,
                        encryptionKey,
                        date);
                recipientStack.Add(new KeyAgreeRecipientInfo(kari));
            }
        }
    }
}
