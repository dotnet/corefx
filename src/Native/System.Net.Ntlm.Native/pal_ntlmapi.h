// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#pragma once

enum NtlmFlags : int32_t
{
    PAL_NTLMSSP_NEGOTIATE_UNICODE = 0x1,
    PAL_NTLMSSP_REQUEST_TARGET = 0x4,
    PAL_NTLMSSP_NEGOTIATE_SIGN = 0x10,
    PAL_NTLMSSP_NEGOTIATE_SEAL = 0x20,
    PAL_NTLMSSP_NEGOTIATE_NTLM = 0x200,
    PAL_NTLMSSP_NEGOTIATE_ALWAYS_SIGN = 0x8000,
    PAL_NTLMSSP_NEGOTIATE_EXTENDED_SESSIONSECURITY = 0x80000,
    PAL_NTLMSSP_NEGOTIATE_128 = 0x20000000,
    PAL_NTLMSSP_NEGOTIATE_KEY_EXCH = 0x40000000,
};

struct PAL_NtlmBuffer
{
    uint64_t length;
    void* data;
};
struct ntlm_type2;

/*
Shims heim_ntlm_free_buf method.
*/
extern "C" void NetNtlmNative_ReleaseNtlmBuffer(void* buffer, uint64_t length);

/*
Shims heim_ntlm_encode_type1 method.
*/
extern "C" int32_t NetNtlmNative_NtlmEncodeType1(uint32_t flags, struct PAL_NtlmBuffer* outBuffer);

/*
Shims heim_ntlm_decode_type2 method.
*/
extern "C" int32_t NetNtlmNative_NtlmDecodeType2(uint8_t* data, int32_t offset, int32_t count, ntlm_type2** type2);

/*
Shims heim_ntlm_free_type2 method.
*/
extern "C" void NetNtlmNative_NtlmFreeType2(ntlm_type2* type2);

/*
Implements Type3 msg proccessing logic
*/
extern "C" int32_t NetNtlmNative_CreateType3Message(const char* password,
                                                    ntlm_type2* type2,
                                                    char* username,
                                                    char* domain,
                                                    uint32_t flags,
                                                    struct PAL_NtlmBuffer* outSessionKey,
                                                    struct PAL_NtlmBuffer* outBuffer);
