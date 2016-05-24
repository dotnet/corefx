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
