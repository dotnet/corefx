// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include "pal_types.h"
#include "pal_ntlmapi.h"
#include "pal_utilities.h"
#include <stddef.h>
#include <assert.h>
#include <time.h>
#include "heimntlm.h"
#include "openssl/hmac.h"
#include "openssl/evp.h"

static_assert(PAL_NTLMSSP_NEGOTIATE_UNICODE == NTLM_NEG_UNICODE, "");
static_assert(PAL_NTLMSSP_REQUEST_TARGET == NTLM_NEG_TARGET, "");
static_assert(PAL_NTLMSSP_NEGOTIATE_SIGN == NTLM_NEG_SIGN, "");
static_assert(PAL_NTLMSSP_NEGOTIATE_SEAL == NTLM_NEG_SEAL, "");
static_assert(PAL_NTLMSSP_NEGOTIATE_NTLM == NTLM_NEG_NTLM, "");
static_assert(PAL_NTLMSSP_NEGOTIATE_ALWAYS_SIGN == NTLM_NEG_ALWAYS_SIGN, "");
static_assert(PAL_NTLMSSP_NEGOTIATE_EXTENDED_SESSIONSECURITY == NTLM_NEG_NTLM2_SESSION, "");
static_assert(PAL_NTLMSSP_NEGOTIATE_128 == NTLM_ENC_128, "");
static_assert(PAL_NTLMSSP_NEGOTIATE_KEY_EXCH == NTLM_NEG_KEYEX, "");

const int32_t MD5_DIGEST_LENGTH = 16;

static inline int32_t
NetNtlmNative_SetBufferLength(int32_t status, ntlm_buf* ntlmBuffer, struct PAL_NtlmBuffer* targetBuffer)
{
    assert(ntlmBuffer != nullptr);
    assert(targetBuffer != nullptr);

    if (status != 0)
    {
        targetBuffer->length = 0;
        targetBuffer->data = nullptr;
    }
    else
    {
        assert(targetBuffer->length == 0 || targetBuffer->data != nullptr);

        targetBuffer->length = ntlmBuffer->length;
        targetBuffer->data = ntlmBuffer->data;
    }

    return status;
}

extern "C" void NetNtlmNative_ReleaseNtlmBuffer(void* buffer, uint64_t length)
{
    assert(buffer != nullptr);

    ntlm_buf ntlmBuffer{.length = length, .data = buffer};
    heim_ntlm_free_buf(&ntlmBuffer);
}

extern "C" int32_t NetNtlmNative_NtlmEncodeType1(uint32_t flags, struct PAL_NtlmBuffer* outBuffer)
{
    assert(outBuffer != nullptr);

    ntlm_type1 type1;
    ntlm_buf ntlmBuffer{.length = 0, .data = nullptr};
    memset(&type1, 0, sizeof(ntlm_type1));
    type1.flags = flags;
    return NetNtlmNative_SetBufferLength(heim_ntlm_encode_type1(&type1, &ntlmBuffer), &ntlmBuffer, outBuffer);
}

extern "C" int32_t NetNtlmNative_NtlmDecodeType2(uint8_t* data, int32_t offset, int32_t count, ntlm_type2** type2)
{
    assert(data != nullptr);
    assert(offset >= 0);
    assert(count >= 0);
    assert(type2 != nullptr);

    ntlm_buf buffer{.length = UnsignedCast(count), .data = data + offset};
    *type2 = new ntlm_type2();
    int32_t stat = heim_ntlm_decode_type2(&buffer, *type2);
    if (stat != 0)
    {
        delete *type2;
        *type2 = nullptr;
    }

    return stat;
}

extern "C" void NetNtlmNative_NtlmFreeType2(ntlm_type2* type2)
{
    assert(type2 != nullptr);

    heim_ntlm_free_type2(type2);
    delete type2;
}

static int32_t NetNtlmNative_NtlmNtKey(const char* password, struct PAL_NtlmBuffer* outBuffer)
{
    assert(outBuffer != nullptr);
    assert(password != nullptr);

    ntlm_buf ntlmBuffer{.length = 0, .data = nullptr};
    return NetNtlmNative_SetBufferLength(heim_ntlm_nt_key(password, &ntlmBuffer), &ntlmBuffer, outBuffer);
}

static int32_t NetNtlmNative_NtlmCalculateResponse(int32_t isLM,
                                                   const struct PAL_NtlmBuffer* key,
                                                   ntlm_type2* type2,
                                                   char* username,
                                                   char* target,
                                                   uint8_t* baseSessionKey,
                                                   int32_t baseSessionKeyLen,
                                                   struct PAL_NtlmBuffer* outBuffer)
{
    // reference doc: http://msdn.microsoft.com/en-us/library/cc236700.aspx
    assert(isLM == 0 || isLM == 1);
    assert(key != nullptr);
    assert(type2 != nullptr);
    assert(username != nullptr);
    assert(target != nullptr);
    assert(baseSessionKey != nullptr);
    assert(baseSessionKeyLen == MD5_DIGEST_LENGTH);
    assert(outBuffer != nullptr);

    ntlm_buf ntlmBuffer{.length = 0, .data = nullptr};
    if (isLM)
    {
        return NetNtlmNative_SetBufferLength(
            heim_ntlm_calculate_lm2(
                key->data, key->length, username, target, type2->challenge, baseSessionKey, &ntlmBuffer),
            &ntlmBuffer,
            outBuffer);
    }
    else
    {
        if (type2->targetinfo.length == 0)
        {
            return NetNtlmNative_SetBufferLength(
                heim_ntlm_calculate_ntlm1(key->data, key->length, type2->challenge, &ntlmBuffer),
                &ntlmBuffer,
                outBuffer);
        }
        else
        {
            return NetNtlmNative_SetBufferLength(heim_ntlm_calculate_ntlm2(key->data,
                                                                           key->length,
                                                                           username,
                                                                           target,
                                                                           type2->challenge,
                                                                           &type2->targetinfo,
                                                                           baseSessionKey,
                                                                           &ntlmBuffer),
                                                 &ntlmBuffer,
                                                 outBuffer);
        }
    }
}

static uint8_t* NetNtlmNative_HMACDigest(uint8_t* key, int32_t keylen, void* input, size_t inputlen)
{
    HMAC_CTX ctx;
    uint8_t* output = new uint8_t[16];

    HMAC_CTX_init(&ctx);
    HMAC_Init_ex(&ctx, key, keylen, EVP_md5(), nullptr);
    HMAC_Update(&ctx, static_cast<uint8_t*>(input), inputlen);
    uint32_t hashLength;
    HMAC_Final(&ctx, output, &hashLength);
    HMAC_CTX_cleanup(&ctx);
    return output;
}

static uint8_t* NetNtlmNative_EVPEncrypt(uint8_t* key, void* input, size_t inputlen)
{
    EVP_CIPHER_CTX ctx;
    EVP_CIPHER_CTX_init(&ctx);
    EVP_CipherInit_ex(&ctx, EVP_rc4(), nullptr, key, nullptr, 1);

    uint8_t* output = new uint8_t[inputlen];
    EVP_Cipher(&ctx, output, static_cast<uint8_t*>(input), static_cast<uint32_t>(inputlen));

    EVP_CIPHER_CTX_cleanup(&ctx);
    return output;
}

static int32_t NetNtlmNative_build_ntlm2_master(
    uint8_t* key, int32_t keylen, ntlm_buf* blob, ntlm_buf* sessionKey, ntlm_buf* masterKey)
{
    // reference: https://msdn.microsoft.com/en-us/library/cc236709.aspx
    uint8_t* ntlmv2hash = NetNtlmNative_HMACDigest(key, keylen, blob->data, blob->length);
    int32_t status = heim_ntlm_build_ntlm1_master(ntlmv2hash, UnsignedCast(keylen), sessionKey, masterKey);
    if (status)
    {
        delete[] ntlmv2hash;
        return status;
    }

    heim_ntlm_free_buf(masterKey);
    uint8_t* exportKey = NetNtlmNative_EVPEncrypt(ntlmv2hash, sessionKey->data, sessionKey->length);
    delete[] ntlmv2hash;
    masterKey->length = sessionKey->length;
    masterKey->data = exportKey;
    return status;
}

extern "C" int32_t NetNtlmNative_CreateType3Message(const char* password,
                                                    ntlm_type2* type2,
                                                    char* username,
                                                    char* domain,
                                                    uint32_t flags,
                                                    struct PAL_NtlmBuffer* outSessionKey,
                                                    struct PAL_NtlmBuffer* outBuffer)
{
    assert(type2 != nullptr);
    assert(username != nullptr);
    assert(domain != nullptr);
    assert(outSessionKey != nullptr);
    assert(outBuffer != nullptr);

    outBuffer->length = 0;
    outBuffer->data = nullptr;
    outSessionKey->length = 0;
    outSessionKey->data = nullptr;

    struct PAL_NtlmBuffer key, lmResponse, ntlmResponse;
    int32_t status = NetNtlmNative_NtlmNtKey(password, &key);
    if (status)
    {
        return status;
    }

    // reference doc: http://msdn.microsoft.com/en-us/library/cc236700.aspx
    uint8_t baseSessionKey[MD5_DIGEST_LENGTH];
    int32_t baseSessionKeyLen = static_cast<int32_t>(sizeof(baseSessionKey));
    status = NetNtlmNative_NtlmCalculateResponse(
        true, &key, type2, username, domain, baseSessionKey, baseSessionKeyLen, &lmResponse);
    if (status)
    {
        NetNtlmNative_ReleaseNtlmBuffer(key.data, key.length);
        return status;
    }

    status = NetNtlmNative_NtlmCalculateResponse(
        false, &key, type2, username, domain, baseSessionKey, baseSessionKeyLen, &ntlmResponse);
    if (status)
    {
        NetNtlmNative_ReleaseNtlmBuffer(key.data, key.length);
        NetNtlmNative_ReleaseNtlmBuffer(lmResponse.data, lmResponse.length);
        return status;
    }

    static char* workstation = static_cast<char*>(calloc(1, sizeof(char))); // empty string
    ntlm_type3 type3;
    memset(&type3, 0, sizeof(ntlm_type3));
    type3.username = const_cast<char*>(username);
    type3.targetname = const_cast<char*>(domain);
    type3.lm = {.length = lmResponse.length, .data = lmResponse.data};
    type3.ntlm = {.length = ntlmResponse.length, .data = ntlmResponse.data};
    type3.ws = workstation;
    type3.flags = flags;

    ntlm_buf masterKey{.length = 0, .data = nullptr};
    ntlm_buf ntlmSessionKey{.length = 0, .data = nullptr};
    ntlm_buf ntlmBuffer{.length = 0, .data = nullptr};

    if (type2->targetinfo.length == 0)
    {
        status = heim_ntlm_build_ntlm1_master(key.data, key.length, &ntlmSessionKey, &masterKey);
    }
    else
    {
        // Only first 16 bytes of the NTLMv2 response should be passed
        assert(ntlmResponse.length >= MD5_DIGEST_LENGTH);
        ntlm_buf blob{.length = MD5_DIGEST_LENGTH, .data = ntlmResponse.data};
        status =
            NetNtlmNative_build_ntlm2_master(baseSessionKey, baseSessionKeyLen, &blob, &ntlmSessionKey, &masterKey);
    }
    NetNtlmNative_ReleaseNtlmBuffer(key.data, key.length);

    status = NetNtlmNative_SetBufferLength(status, &ntlmSessionKey, outSessionKey);
    if (status != 0)
    {
        NetNtlmNative_ReleaseNtlmBuffer(lmResponse.data, lmResponse.length);
        NetNtlmNative_ReleaseNtlmBuffer(ntlmResponse.data, ntlmResponse.length);
        return status;
    }

    type3.sessionkey = masterKey;
    status = heim_ntlm_encode_type3(&type3, &ntlmBuffer);
    NetNtlmNative_ReleaseNtlmBuffer(lmResponse.data, lmResponse.length);
    NetNtlmNative_ReleaseNtlmBuffer(ntlmResponse.data, ntlmResponse.length);
    if (status != 0)
    {
        heim_ntlm_free_buf(&ntlmBuffer);
        return status;
    }

    if (type2->targetinfo.length == 0)
    {
        heim_ntlm_free_buf(&masterKey);
    }
    else
    {
        // in case of v2, masterKey.data is created by NetNtlmNative_build_ntlm2_master function and free_buf cannot
        // be
        // called.
        delete[] static_cast<uint8_t*>(masterKey.data);
    }

    return NetNtlmNative_SetBufferLength(status, &ntlmBuffer, outBuffer);
}
