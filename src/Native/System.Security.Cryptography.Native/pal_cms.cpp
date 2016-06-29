// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#include "pal_cms.h"
#include <openssl/err.h>
#include <openssl/evp.h>

extern "C" CMS_ContentInfo* CryptoNative_CmsDecode(const uint8_t* buf, int32_t len)
{
    if (buf == nullptr || len <= 0)
    {
        return nullptr;
    }
    
    return d2i_CMS_ContentInfo(nullptr, &buf, len);
}

extern "C" int CryptoNative_CmsDecrypt(CMS_ContentInfo* cms, X509* cert, EVP_PKEY* pkey, BIO* out)
{
    if (cms == nullptr || cert == nullptr || pkey == nullptr || out == nullptr)
    {
        return 0;
    }

    return CMS_decrypt(cms, pkey, cert, nullptr, out, CMS_BINARY);
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

extern "C" CMS_ContentInfo* CryptoNative_CmsInitializeEnvelope(ASN1_OBJECT* algorithmOid, int32_t* status)
{
    if (algorithmOid == nullptr || status == nullptr)
    {
        *status = -1;
        return nullptr;
    }

    const EVP_CIPHER* cipher = EVP_get_cipherbyobj(algorithmOid);

    if (cipher == nullptr)
    {
        *status = 0;
        return nullptr;
    }

    *status = 1;
    return CMS_EnvelopedData_create(cipher);
}

extern "C" int CryptoNative_CmsAddSkidRecipient(CMS_ContentInfo* cms, X509* cert)
{
    if (cms == nullptr || cert == nullptr)
    {
        return -1;
    }

    // TODO (3334): here we must evaluate if there's an explicit skid, or we must generate
    // one according to the fallback behavior seen in CryptoApi. Eg: The SHA-1 hash of the publicKeyInfo
    // blob of the encoded certificate.

    return (CMS_add1_recipient_cert(cms, cert, CMS_USE_KEYID) == nullptr) ? 0 : 1;
}

extern "C" int CryptoNative_CmsAddIssuerAndSerialRecipient(CMS_ContentInfo* cms, X509* cert)
{
    if (cms == nullptr || cert == nullptr)
    {
        return -1;
    }

    return (CMS_add1_recipient_cert(cms, cert, 0) == nullptr) ? 0 : 1;
}

extern "C" int CryptoNative_CmsAddOriginatorCert(CMS_ContentInfo* cms, X509* cert)
{
    if (cms == nullptr || cert == nullptr)
    {
        return -1;
    }

    return CMS_add1_cert(cms, cert);
}

extern "C" int CryptoNative_CmsCompleteMessage(CMS_ContentInfo* cms, BIO* data, bool detached)
{
    if (cms == nullptr || data == nullptr)
    {
        return -1;
    }

    // This function will allocate a new ASN1_OCTET_STRING to store the message's/signature's content and flag it
    // as a created octet string instead of one that was read in if detached is false, otherwise it will free the object in the content
    // of the CMS_ContentInfo structure and set the pointer to null.
    // If this function is not used with the detached flag as false, the content won't get attached to the CMS_ContentInfo
    // structure and the i2d function will return the encoding for a CMS with no content inside of it.
    if (CMS_set_detached(cms, detached) == 0)
    {
        return 0;
    }

    return CMS_final(cms, data, nullptr, 0);
}

extern "C" int CryptoNative_CmsGetDerSize(CMS_ContentInfo* cms)
{
    return i2d_CMS_ContentInfo(cms, nullptr);
}

extern "C" int CryptoNative_CmsEncode(CMS_ContentInfo* cms, uint8_t* buf)
{
    return i2d_CMS_ContentInfo(cms, &buf);
}
