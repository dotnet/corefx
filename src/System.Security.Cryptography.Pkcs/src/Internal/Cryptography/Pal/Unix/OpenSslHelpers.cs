// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Security.Cryptography.Pkcs;
using System.Security.Cryptography.X509Certificates;

using Microsoft.Win32.SafeHandles;

namespace Internal.Cryptography.Pal.OpenSsl
{
    internal static class OpenSslHelpers
    {
        public static string GetContentType(this SafeCmsHandle cmsHandle)
        {
            using (SafeSharedAsn1ObjectHandle oidAsn1 = Interop.Crypto.CmsGetMessageContentType(cmsHandle))
            {
                Interop.Crypto.CheckValidOpenSslHandle(oidAsn1);
                return Interop.Crypto.GetOidValue(oidAsn1);
            }
        }

        public static ContentInfo GetEmbeddedContent(this SafeCmsHandle cmsHandle)
        {
            string oid;
            Oid contentType;
            byte[] content;

            using (SafeSharedAsn1ObjectHandle oidAsn1 = Interop.Crypto.CmsGetEmbeddedContentType(cmsHandle))
            {
                Interop.Crypto.CheckValidOpenSslHandle(oidAsn1);
                oid = Interop.Crypto.GetOidValue(oidAsn1);
            }

            contentType = new Oid(oid);

            using (SafeSharedAsn1OctetStringHandle encodedContent = Interop.Crypto.CmsGetEmbeddedContent(cmsHandle))
            {
                // encodedContent can be a null pointer if there is no content. In this case the content should be set
                // to an empty byte array. 
                if (encodedContent.IsInvalid)
                    content = Array.Empty<byte>();
                else
                    content = Interop.Crypto.GetAsn1StringBytes(encodedContent);
            }

            return new ContentInfo(contentType, content);
        }

        public static X509Certificate2Collection GetOriginatorCerts(this SafeCmsHandle cmsHandle)
        {
            X509Certificate2Collection origCertCollection = new X509Certificate2Collection();

            using (SafeX509StackHandle origCertsPtr = Interop.Crypto.CmsGetOriginatorCerts(cmsHandle))
            {
                // origCertsPtr might be a nullptr as originator certs are optional, but in this case,
                // GetX509StackFieldCount will just return 0 and behavior will be as expected. 
                int certCount = Interop.Crypto.GetX509StackFieldCount(origCertsPtr);
                for (int i = 0; i < certCount; i++)
                {
                    IntPtr certRef = Interop.Crypto.GetX509StackField(origCertsPtr, i);
                    Interop.Crypto.CheckValidOpenSslHandle(certRef);
                    X509Certificate2 copyOfCert = new X509Certificate2(certRef);
                    origCertCollection.Add(copyOfCert);
                }
            }

            return origCertCollection;
        }

        public static AlgorithmIdentifier ReadEncryptionAlgorithm(this DerSequenceReader encodedCms)
        {
            DerSequenceReader encryptedContentInfo = encodedCms.ReadSequence();
            // EncryptedContentInfo ::= SEQUENCE {
            //     contentType ContentType,
            //     contentEncryptionAlgorithm ContentEncryptionAlgorithmIdentifier,
            //     encryptedContent[0] IMPLICIT EncryptedContent OPTIONAL }

            // Skip the content type
            Debug.Assert(
                encryptedContentInfo.PeekTag() == (byte)DerSequenceReader.DerTag.ObjectIdentifier,
                "Expected to skip an OID while reading EncryptedContentInfo");
            encryptedContentInfo.SkipValue();

            // The encoding for a ContentEncryptionAlgorithmIdentifier is just the OID and the parameters, and we just need the OID 
            DerSequenceReader contentEncryptionAlgorithmIdentifier = encryptedContentInfo.ReadSequence();
            string algoOid = contentEncryptionAlgorithmIdentifier.ReadOidAsString();
            int keyLength = 0;

            // TODO(3334): Get the Key length from the algorithm parameters.
            // The hardcoded values for RC2 and RC4 are not right. Will update accordingly when we work out how to
            // get them from the parameters. 
            switch (algoOid)
            {
                case Oids.Rc2:
                    keyLength = KeyLengths.Rc2_128Bit;
                    break;
                case Oids.Rc4:
                    int saltLength = 8;
                    keyLength = KeyLengths.Rc4Max_128Bit - 8 * saltLength;
                    break;
                case Oids.Des:
                    keyLength = KeyLengths.Des_64Bit;
                    break;
                case Oids.TripleDesCbc:
                    keyLength = KeyLengths.TripleDes_192Bit;
                    break;
                    // As we commented in the Windows implementation:
                    // All other algorithms are not set by the framework.  Key lengths are not a viable way of
                    // identifying algorithms in the long run so we will not extend this list any further.
            }

            return new AlgorithmIdentifier(new Oid(algoOid), keyLength);
        }

        public static RecipientInfoCollection GetRecipients(this SafeCmsHandle cmsHandle)
        {
            using (SafeSharedCmsRecipientInfoStackHandle recipientInfoStackHandle = Interop.Crypto.CmsGetRecipients(cmsHandle))
            {
                Interop.Crypto.CheckValidOpenSslHandle(recipientInfoStackHandle);
                int recipientCount = Interop.Crypto.CmsGetRecipientStackFieldCount(recipientInfoStackHandle);
                List<RecipientInfo> recipientInfoStack = new List<RecipientInfo>(recipientCount);

                for (int index = 0; index < recipientCount; index++)
                {
                    RecipientInfo recipient = recipientInfoStackHandle.GetRecipientAtIndex(index);
                    recipientInfoStack.Add(recipient);
                }

                return new RecipientInfoCollection(recipientInfoStack);
            }
        }

        private static RecipientInfo GetRecipientAtIndex(this SafeSharedCmsRecipientInfoStackHandle recipientStack, int index)
        {
            // Get the recipient at a specific index.
            SafeSharedCmsRecipientInfoHandle recipient = Interop.Crypto.CmsGetRecipientStackField(recipientStack, index);
            Interop.Crypto.CheckValidOpenSslHandle(recipient);

            // Get the type and delegate to the appropriate constructor
            // TODO(3334): Shim get type method, create the enum and decide on the constructor
            return new KeyTransRecipientInfo(new KeyTransRecipientInfoPalOpenSsl(recipient));
        }

        public static CryptographicAttributeObjectCollection ReadUnprotectedAttributes(this DerSequenceReader encodedCms)
        {
            var unprotectedAttributesCollection = new CryptographicAttributeObjectCollection();

            // As the unprotected attributes are optional we have to check if there's anything left to read and check if it's
            // tagged as context specific 1
            byte contextImplicit1 = (byte)(DerSequenceReader.ContextSpecificTagFlag | DerSequenceReader.ConstructedFlag | 0x01);

            if (encodedCms.HasData && encodedCms.PeekTag() == contextImplicit1)
            {
                DerSequenceReader encodedAttributesReader = encodedCms.ReadSequence();
                while (encodedAttributesReader.HasData)
                {
                    DerSequenceReader attributeReader = encodedAttributesReader.ReadSequence();

                    Oid attributeOid = attributeReader.ReadOid();

                    // This reads a set of unprotected attributes. If the set is empty then no CryptographicObject will 
                    // be created. In framework this works differently and is documented on issue 9252. In case this
                    // behavior wants to be emulated the only thing that would need to be done is create a 
                    // CryptographicObject with the Oid sorted in attributeOid
                    DerSequenceReader attributeSetReader = attributeReader.ReadSet();
                    
                    while (attributeSetReader.HasData)
                    {
                        byte[] singleEncodedAttribute = attributeSetReader.ReadNextEncodedValue();
                        AsnEncodedData singleAttribute = Helpers.CreateBestPkcs9AttributeObjectAvailable(attributeOid, singleEncodedAttribute);
                        unprotectedAttributesCollection.Add(singleAttribute);
                    }
                }
            }

            return unprotectedAttributesCollection;
        }
    }
}
