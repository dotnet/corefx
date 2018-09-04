// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#include "pal_config.h"
#include "pal_utilities.h"
#include "pal_hmac.h"

#include <assert.h>

extern "C" HMAC_CTX* CryptoNative_HmacCreate(const uint8_t* key, int32_t keyLen, const EVP_MD* md)
{
    assert(key != nullptr || keyLen == 0);
    assert(keyLen >= 0);
    assert(md != nullptr);

    HMAC_CTX* ctx = HMAC_CTX_new();
    if (ctx == nullptr)
    {
        // Allocation failed
        return nullptr;
    }

    // NOTE: We can't pass nullptr as empty key since HMAC_Init_ex will interpret
    // that as request to reuse the "existing" key.
    uint8_t _;
    if (keyLen == 0)
        key = &_;

    int ret = HMAC_Init_ex(ctx, key, keyLen, md, nullptr);

    if (!ret)
    {
        free(ctx);
        return nullptr;
    }

    return ctx;
}

extern "C" void CryptoNative_HmacDestroy(HMAC_CTX* ctx)
{
    if (ctx != nullptr)
    {
        HMAC_CTX_free(ctx);
    }
}

extern "C" int32_t CryptoNative_HmacReset(HMAC_CTX* ctx)
{
    assert(ctx != nullptr);

    return HMAC_Init_ex(ctx, nullptr, 0, nullptr, nullptr);
}

extern "C" int32_t CryptoNative_HmacUpdate(HMAC_CTX* ctx, const uint8_t* data, int32_t len)
{
    assert(ctx != nullptr);
    assert(data != nullptr || len == 0);
    assert(len >= 0);

    if (len < 0)
    {
        return 0;
    }

    return HMAC_Update(ctx, data, UnsignedCast(len));
}

extern "C" int32_t CryptoNative_HmacFinal(HMAC_CTX* ctx, uint8_t* md, int32_t* len)
{
    assert(ctx != nullptr);
    assert(len != nullptr);
    assert(md != nullptr || *len == 0);
    assert(*len >= 0);

    if (len == nullptr || *len < 0)
    {
        return 0;
    }

    unsigned int unsignedLen = UnsignedCast(*len);
    int ret = HMAC_Final(ctx, md, &unsignedLen);
    *len = SignedCast(unsignedLen);
    return ret;
}
