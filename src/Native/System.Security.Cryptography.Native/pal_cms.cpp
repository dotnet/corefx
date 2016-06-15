// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#include "pal_cms.h"

extern "C" CMS_ContentInfo* CryptoNative_CmsDecode(const uint8_t* buf, int32_t len)
{
    if (buf == nullptr || len <= 0)
    {
        return nullptr;
    }
    
    return d2i_CMS_ContentInfo(nullptr, &buf, len);
}

extern "C" void CryptoNative_CmsDestroy(CMS_ContentInfo* cms)
{
    if (cms != nullptr)
    {
        CMS_ContentInfo_free(cms);
    }
}

extern "C" ASN1_OCTET_STRING* CryptoNative_CmsGetEmbeddedContent(CMS_ContentInfo* cms)
{
    if (cms == nullptr)
    {
        return nullptr;
    }

    ASN1_OCTET_STRING** contentPtr = CMS_get0_content(cms);
    
    return contentPtr == nullptr ? nullptr : *contentPtr; 
}

extern "C" const ASN1_OBJECT* CryptoNative_CmsGetEmbeddedContentType(CMS_ContentInfo* cms)
{
    if (cms == nullptr)
    {
        return nullptr;
    }

    return CMS_get0_eContentType(cms);   
}

extern "C" const ASN1_OBJECT* CryptoNative_CmsGetMessageContentType(CMS_ContentInfo* cms)
{
    if (cms == nullptr)
    {
        return nullptr;
    }

    return CMS_get0_type(cms);
}

extern "C" X509Stack* CryptoNative_CmsGetOriginatorCerts(CMS_ContentInfo* cms)
{
    if (cms == nullptr)
    {
        return nullptr;
    }

    return CMS_get1_certs(cms);
}

extern "C" CmsRecipientStack* CryptoNative_CmsGetRecipients(CMS_ContentInfo* cms)
{
    if (cms == nullptr)
    {
        return nullptr;
    }

    return CMS_get0_RecipientInfos(cms);
}

extern "C" CMS_RecipientInfo* CryptoNative_CmsGetRecipientStackField(CmsRecipientStack* recipientStack, int32_t index)
{
    if (recipientStack == nullptr)
    {
        return nullptr;
    }

    return sk_CMS_RecipientInfo_value(recipientStack, index);
}

extern "C" int CryptoNative_CmsGetRecipientStackFieldCount(CmsRecipientStack* recipientStack)
{
    if (recipientStack == nullptr)
    {
        return 0;
    }

    return sk_CMS_RecipientInfo_num(recipientStack);
}
