// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

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

struct ntlm_buf;
struct ntlm_type2;

/*
Shims heim_ntlm_free_buf method.
*/
extern "C" void NetSecurity_HeimNtlmFreeBuf(ntlm_buf* data);

/*
Copies data from ntlm_buffer into an array of given size, from the offset.
*/
extern "C" void
NetSecurity_ExtractNtlmBuffer(const ntlm_buf* bufferHandle, uint8_t* destination, uint32_t capacity, uint32_t offset);

/*
Shims heim_ntlm_encode_type1 method.
*/
extern "C" int32_t NetSecurity_HeimNtlmEncodeType1(uint32_t flags, ntlm_buf** outBufferHandle, int* outLength);

/*
Shims heim_ntlm_decode_type2 method.
*/
extern "C" int32_t NetSecurity_HeimNtlmDecodeType2(uint8_t* data, int32_t offset, int32_t count, ntlm_type2** type2);

/*
Shims heim_ntlm_free_type2 method.
*/
extern "C" void NetSecurity_HeimNtlmFreeType2(ntlm_type2* type2);

/*
Shims heim_ntlm_nt_key method.
*/
extern "C" int32_t NetSecurity_HeimNtlmNtKey(const char* password, ntlm_buf** outBufferHandle, int* outLength);

/*
Shims heim_ntlm_calculate_lm2/_ntlm2 methods.
*/
extern "C" int32_t NetSecurity_HeimNtlmCalculateResponse(int32_t isLM,
                                                         const ntlm_buf* key,
                                                         ntlm_type2* type2,
                                                         char* username,
                                                         char* target,
                                                         uint8_t* baseSessionKey,
                                                         int32_t baseSessionKeyLen,
                                                         ntlm_buf** data,
                                                         int* outLength);

/*
Implements Type3 msg proccessing logic
*/
extern "C" int32_t NetSecurity_CreateType3Message(ntlm_buf* key,
                                                  ntlm_type2* type2,
                                                  const char* username,
                                                  const char* domain,
                                                  uint32_t flags,
                                                  ntlm_buf* lmResponse,
                                                  ntlm_buf* ntlmResponse,
                                                  uint8_t* baseSessionKey,
                                                  int32_t baseSessionKeyLen,
                                                  ntlm_buf** outSessionHandle,
                                                  int32_t* outSessionKeyLen,
                                                  ntlm_buf** outBufferHandle,
                                                  int32_t* outLength);
