// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#include <assert.h>
#include "pal_cms.h"
#include "pal_x509.h"
#include <openssl/err.h>
#include <openssl/evp.h>

static X509* X509CloneWithImplicitSkid(X509* cert)
{
    if (cert == nullptr || X509_get_ext_by_NID(cert, NID_subject_key_identifier, -1) >= 0)
    {
        return nullptr;
    }

    uint8_t* buf = reinterpret_cast<uint8_t*>(OPENSSL_malloc(SHA_DIGEST_LENGTH));
    uint8_t* tmpPtr = buf;
    X509* copy = X509_dup(cert);
    ASN1_OCTET_STRING* data = nullptr;
    X509_EXTENSION* skidExt = nullptr;
    bool success = false;
    int status = -1;
    int i2dlen = -1;
    int i2dlen2 = -1;

    if (buf == nullptr || copy == nullptr)
    {
        goto err;
    }

    status = CryptoNative_X509GetImplicitSubjectKeyIdentifier(copy, buf, SHA_DIGEST_LENGTH);

    if (status != 1)
    {
        goto err;
    }

    data = ASN1_OCTET_STRING_new();

    if (data == nullptr)
    {
        goto err;
    }

    status = ASN1_OCTET_STRING_set(data, buf, SHA_DIGEST_LENGTH);

    if (status != 1)
    {
        goto err;
    }

    i2dlen = i2d_ASN1_OCTET_STRING(data, nullptr);

    if (i2dlen < 1)
    {
        goto err;
    }

    OPENSSL_free(buf);
    buf = reinterpret_cast<uint8_t*>(OPENSSL_malloc(i2dlen));
    tmpPtr = buf;

    if (buf == nullptr)
    {
        goto err;
    }

    i2dlen2 = i2d_ASN1_OCTET_STRING(data, &tmpPtr);
    assert(i2dlen == i2dlen2);

    status = ASN1_OCTET_STRING_set(data, buf, i2dlen);

    if (status != 1)
    {
        goto err;
    }

    skidExt = X509_EXTENSION_create_by_NID(nullptr, NID_subject_key_identifier, 0, data);

    if (skidExt == nullptr)
    {
        goto err;
    }

    if (X509_add_ext(copy, skidExt, -1) == 0)
    {
        goto err;
    }
    else
    {
        success = true;
    }

err:
    if (buf != nullptr)
    {
        OPENSSL_free(buf);
    }

    if (data != nullptr)
    {
        ASN1_OCTET_STRING_free(data);
    }

    if (skidExt != nullptr)
    {
        X509_EXTENSION_free(skidExt);
    }

    if (!success && copy != nullptr)
    {
        X509_free(copy);
    }

    return (success) ? copy : nullptr;
}

extern "C" CMS_ContentInfo* CryptoNative_CmsDecode(const uint8_t* buf, int32_t len)
{
    if (buf == nullptr || len <= 0)
    {
        return nullptr;
    }
    
    return d2i_CMS_ContentInfo(nullptr, &buf, len);
}

extern "C" int CryptoNative_CmsDecrypt(CMS_ContentInfo* cms, X509* cert, EVP_PKEY* pkey, BIO* out, SubjectIdentifierType type)
{
    if (cms == nullptr || cert == nullptr || pkey == nullptr || out == nullptr)
    {
        return 0;
    }

    X509* matchedCert = cert;
    X509* copyCert = nullptr;

    if (type == SubjectIdentifierType::SubjectKeyIdentifier && X509_get_ext_by_NID(cert, NID_subject_key_identifier, -1) < 0)
    {
        matchedCert = copyCert = X509CloneWithImplicitSkid(cert);
        if (matchedCert == nullptr)
        {
            return -1;
        }
    }

    int status = CMS_decrypt(cms, pkey, matchedCert, nullptr, out, CMS_BINARY);

    if (copyCert != nullptr)
    {
        X509_free(copyCert);
    }

    return status;
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

    X509* matchedCert = cert;
    X509* copyCert = nullptr;

    int pos = X509_get_ext_by_NID(cert, NID_subject_key_identifier, -1);

    if (pos < 0)
    {
        matchedCert = copyCert = X509CloneWithImplicitSkid(cert);

        if (matchedCert == nullptr)
        {
            return 0;
        }
    }

    int status = (CMS_add1_recipient_cert(cms, matchedCert, CMS_USE_KEYID) == nullptr) ? 0 : 1;

    if (copyCert != nullptr)
    {
        X509_free(copyCert);
    }

    return status;
}

extern "C" int CryptoNative_CmsAddIssuerAndSerialRecipient(CMS_ContentInfo* cms, X509* cert)
{
    if (cms == nullptr || cert == nullptr)
    {
        return -1;
    }

    return (CMS_add1_recipient_cert(cms, cert, 0) == nullptr) ? 0 : 1;
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

extern "C" int CryptoNative_CmsSetEmbeddedContentType(CMS_ContentInfo* cms, ASN1_OBJECT* oid)
{
    if (cms == nullptr || oid == nullptr)
    {
        return -1;
    }

    return CMS_set1_eContentType(cms, oid);
}

extern "C" int CryptoNative_CmsGetAlgorithmKeyLength(const uint8_t* algor, int32_t len)
{
    if (algor == nullptr || len < 1)
    {
        return -2;
    }

    X509_ALGOR* calg = d2i_X509_ALGOR(nullptr, &algor, len);
    EVP_CIPHER_CTX ctx;
    const EVP_CIPHER* ciph;
    int keylen;

    EVP_CIPHER_CTX_init(&ctx);

    if (calg == nullptr)
    {
        keylen = -1;
        goto err;
    }

    ciph = EVP_get_cipherbyobj(calg->algorithm);

    if (ciph == nullptr)
    {
        keylen = -1;
        goto err;
    }

    if (EVP_CipherInit_ex(&ctx, ciph, NULL, NULL, NULL, 0) <= 0)
    {
        keylen = -1;
        goto err;
    }

    if (EVP_CIPHER_asn1_to_param(&ctx, calg->parameter) <= 0) {
        keylen = -1;
        goto err;
    }

    // OpenSSL handles key lengths in bytes, but .NET does so in bits, so convert.
    keylen = 8*EVP_CIPHER_CTX_key_length(&ctx);

err:

    if (calg != nullptr)
    {
        X509_ALGOR_free(calg);
    }

    OPENSSL_cleanse(reinterpret_cast<char*>(&ctx), sizeof(ctx));

    return keylen;
}
