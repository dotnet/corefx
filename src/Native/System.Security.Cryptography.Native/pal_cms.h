// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#include "pal_crypto_types.h"

#include <openssl/cms.h>

/*
Shims the d2i_CMS_ContentInfo method
*/
extern "C" CMS_ContentInfo* CryptoNative_CmsDecode(const uint8_t* buf, int32_t len);

/*
Shims the CMS_decrypt method
*/
extern "C" int CryptoNative_CmsDecrypt(CMS_ContentInfo* cms, X509* cert, EVP_PKEY* pkey, BIO* out);

/*
Shim the CMS_ContentInfo_free method
*/
extern "C" void CryptoNative_CmsDestroy(CMS_ContentInfo* cms);

/*
Shims the CMS_get0_content method 
*/
extern "C" ASN1_OCTET_STRING* CryptoNative_CmsGetEmbeddedContent(CMS_ContentInfo* cms);

/*
Shims the CMS_get0_eContentType method, returning the ASN1_OBJECT pointer to the type of 
the message inside the envelope.
*/
extern "C" const ASN1_OBJECT* CryptoNative_CmsGetEmbeddedContentType(CMS_ContentInfo* cms);

/*
Shims the CMS_get0_type method. Returning the ASN1_OBJECT pointer to the message's type
*/
extern "C" const ASN1_OBJECT* CryptoNative_CmsGetMessageContentType(CMS_ContentInfo* cms);

/*
Shims the CMS_get1_Certs method
*/
extern "C" X509Stack* CryptoNative_CmsGetOriginatorCerts(CMS_ContentInfo* cms);

/*
Shims the CMS_get0_RecipientInfos method
*/
extern "C" CmsRecipientStack* CryptoNative_CmsGetRecipients(CMS_ContentInfo* cms);

/*
Shims the sk_CMS_RecipientInfo_value method
*/
extern "C" CMS_RecipientInfo* CryptoNative_CmsGetRecipientStackField(CmsRecipientStack* recipientStack, int32_t index);

/*
Shims the sk_CMS_RecipientInfo_num method returning the recipient count. 
*/
extern "C" int CryptoNative_CmsGetRecipientStackFieldCount(CmsRecipientStack* recipientStack);

/*
Shims the CMS_RecipientInfo_free method
*/
extern "C" void CryptoNative_CmsRecipientDestroy(CMS_RecipientInfo* recipient);

/*
Initializes a CMS_ContentInfo of type EnvelopedData with the cipher given by the oid in algorithmOid
and returns a pointer to it. 

Status is set to 1 on success, 0 on unsupported algorithm, and -1 on invalid input.
*/
extern "C" CMS_ContentInfo* CryptoNative_CmsInitializeEnvelope(ASN1_OBJECT* algorithmOid, int32_t* status);

/*
Adds an originator certificate to the CMS_ContentInfo structure. If the CMS_ContentInfo structure
is of type EnvelopedData it will be added to OriginatorInfo, and for SignedData it will be added to
the certificates.

Returns 1 on success, 0 on OpenSSL failure, and -1 on invalid input.
*/
extern "C" int CryptoNative_CmsAddOriginatorCert(CMS_ContentInfo* cms, X509* cert);

/*
Creates a KeyTransportRecipientInfo using the IssuerAndSerial identification method for
the provided certificate and adds it to the CMS_ContentInfo structure.

Returns 1 on success, 0 on OpenSSL failure, and -1 on invalid input.
*/
extern "C" int CryptoNative_CmsAddIssuerAndSerialRecipient(CMS_ContentInfo* cms, X509* cert);

/*
Creates a KeyTransportRecipientInfo using the SubjectKeyIdentifier identification method for
the provided certificate and adds it to the CMS_ContentInfo structure.

Returns 1 on success, 0 on OpenSSL failure, and -1 on invalid input.
*/
extern "C" int CryptoNative_CmsAddSkidRecipient(CMS_ContentInfo* cms, X509* cert);

/*
Finalizes the message after all the recipients and certificates have been added. If detached is
set to true, in the case of a CMS_ContentInfo of signed type, the signature will remain detached,
in case of an enveloped type, the encrypted content will be detached.

Returns 1 on success, 0 on OpenSSL failure, and -1 on invalid input.
*/
extern "C" int CryptoNative_CmsCompleteMessage(CMS_ContentInfo* cms, BIO* data, bool detached);

/*
Gets the byte length of the CMS_ContentInfo to encode.
*/
extern "C" int CryptoNative_CmsGetDerSize(CMS_ContentInfo* cms);

/*
Shims the i2d_CMS_ContentInfo method.
*/
extern "C" int CryptoNative_CmsEncode(CMS_ContentInfo* cms, uint8_t* buf);
