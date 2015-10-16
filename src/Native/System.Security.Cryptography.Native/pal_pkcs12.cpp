// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

#include "pal_pkcs12.h"

extern "C" PKCS12* DecodePkcs12(const unsigned char* buf, int32_t len)
{
    if (!buf || !len)
    {
        return nullptr;
    }

    return d2i_PKCS12(nullptr, &buf, len);
}

extern "C" PKCS12* DecodePkcs12FromBio(BIO* bio)
{
    return d2i_PKCS12_bio(bio, nullptr);
}

extern "C" void Pkcs12Destroy(PKCS12* p12)
{
    if (p12 != nullptr)
    {
        PKCS12_free(p12);
    }
}

extern "C" PKCS12* Pkcs12Create(char* pass, EVP_PKEY* pkey, X509* cert, X509Stack* ca)
{
    return PKCS12_create(
        pass, nullptr, pkey, cert, ca, NID_undef, NID_undef, PKCS12_DEFAULT_ITER, PKCS12_DEFAULT_ITER, 0);
}

extern "C" int32_t GetPkcs12DerSize(PKCS12* p12)
{
    return i2d_PKCS12(p12, nullptr);
}

extern "C" int32_t EncodePkcs12(PKCS12* p12, unsigned char* buf)
{
    return i2d_PKCS12(p12, &buf);
}

extern "C" int32_t Pkcs12Parse(PKCS12* p12, const char* pass, EVP_PKEY** pkey, X509** cert, X509Stack** ca)
{
    return PKCS12_parse(p12, pass, pkey, cert, ca);
}
