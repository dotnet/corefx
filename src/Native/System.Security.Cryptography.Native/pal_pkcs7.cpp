// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

#include "pal_pkcs7.h"

#include <openssl/pem.h>

extern "C" PKCS7* PemReadBioPkcs7(BIO* bp)
{
    return PEM_read_bio_PKCS7(bp, nullptr, nullptr, nullptr);
}

extern "C" PKCS7* DecodePkcs7(const unsigned char* buf, int32_t len)
{
    if (!buf || !len)
    {
        return nullptr;
    }

    return d2i_PKCS7(nullptr, &buf, len);
}

extern "C" PKCS7* D2IPkcs7Bio(BIO* bp)
{
    return d2i_PKCS7_bio(bp, nullptr);
}

extern "C" void Pkcs7Destroy(PKCS7* p7)
{
    if (p7 != nullptr)
    {
        PKCS7_free(p7);
    }
}

extern "C" int32_t GetPkcs7Certificates(PKCS7* p7, X509Stack** certs)
{
    if (!p7 || !certs)
    {
        return 0;
    }

    switch (OBJ_obj2nid(p7->type))
    {
        case NID_pkcs7_signed:
            *certs = p7->d.sign->cert;
            return 1;
        case NID_pkcs7_signedAndEnveloped:
            *certs = p7->d.signed_and_enveloped->cert;
            return 1;
    }

    return 0;
}
