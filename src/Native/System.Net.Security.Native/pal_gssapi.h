// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

#pragma once

#if HAVE_GSSFW_HEADERS
typedef struct gss_name_t_desc_struct GssName;
typedef struct gss_ctx_id_t_desc_struct GssCtxId;
typedef struct gss_cred_id_t_desc_struct GssCredId;
typedef struct gss_buffer_desc_struct GssBuffer;
#else
typedef struct gss_name_struct GssName;
typedef struct gss_ctx_id_struct GssCtxId;
typedef struct gss_cred_id_struct GssCredId;
typedef struct gss_buffer_desc_struct GssBuffer;
#endif

enum PAL_GssStatus : uint32_t
{
    PAL_GSS_COMPLETE = 0,
    PAL_GSS_CONTINUE_NEEDED = 1
};

enum PAL_GssFlags : uint32_t
{
    PAL_GSS_C_DELEG_FLAG = 0x1,
    PAL_GSS_C_MUTUAL_FLAG = 0x2,
    PAL_GSS_C_REPLAY_FLAG = 0x4,
    PAL_GSS_C_SEQUENCE_FLAG = 0x8,
    PAL_GSS_C_CONF_FLAG = 0x10,
    PAL_GSS_C_INTEG_FLAG = 0x20,
    PAL_GSS_C_ANON_FLAG = 0x40,
    PAL_GSS_C_PROT_READY_FLAG = 0x80,
    PAL_GSS_C_TRANS_FLAG = 0x100,
    PAL_GSS_C_DCE_STYLE = 0x1000,
    PAL_GSS_C_IDENTIFY_FLAG = 0x2000,
    PAL_GSS_C_EXTENDED_ERROR_FLAG = 0x4000,
    PAL_GSS_C_DELEG_POLICY_FLAG = 0x8000
};

/*
Shims the gss_release_buffer method.
*/
extern "C" uint32_t NetSecurity_ReleaseBuffer(uint32_t* minorStatus, GssBuffer* buffer);

/*
Copies gss_buffer_t->value to the given byte array
*/
extern "C" void NetSecurity_CopyBuffer(GssBuffer* bufferHandle, uint8_t* bytes, uint32_t capacity, uint32_t offset);

/*
Shims the gss_display_status method.
*/
extern "C" uint32_t NetSecurity_DisplayStatus(uint32_t* minorStatus,
                                              uint32_t statusValue,
                                              int32_t isGssMechCode,
                                              GssBuffer** outBufferHandle,
                                              uint32_t* statusLength);

/*
Shims the gss_import_name method with nametype = GSS_C_NT_USER_NAME.
*/
extern "C" uint32_t
NetSecurity_ImportUserName(uint32_t* minorStatus, char* inputName, uint32_t inputNameLen, GssName** outputName);

/*
Shims the gss_import_name method with nametype = GSS_C_NT_USER_NAME.
*/
extern "C" uint32_t
NetSecurity_ImportPrincipalName(uint32_t* minorStatus, char* inputName, uint32_t inputNameLen, GssName** outputName);

/*
Shims the gss_release_name method.
*/
extern "C" uint32_t NetSecurity_ReleaseName(uint32_t* minorStatus, GssName** inputName);

/*
Shims the gss_acquire_cred method with SPNEGO oids.
*/
extern "C" uint32_t NetSecurity_AcquireCredSpNego(uint32_t* minorStatus,
                                                  GssName* desiredName,
                                                  int32_t isInitiate,
                                                  GssCredId** outputCredHandle);

/*
Shims the gss_release_cred method.
*/
extern "C" uint32_t NetSecurity_ReleaseCred(uint32_t* minorStatus, GssCredId** credHandle);

/*
Shims the gss_init_sec_context method with SPNEGO oids.
*/
extern "C" uint32_t NetSecurity_InitSecContext(uint32_t* minorStatus,
                                               GssCredId* claimantCredHandle,
                                               GssCtxId** contextHandle,
                                               uint32_t isNtlm,
                                               GssName* targetName,
                                               uint32_t reqFlags,
                                               uint8_t* inputBytes,
                                               uint32_t inputLength,
                                               GssBuffer** outBufferHandle,
                                               uint32_t* outTokenLength,
                                               uint32_t* retFlags);

/*
Shims the gss_delete_sec_context method.
*/
extern "C" uint32_t NetSecurity_DeleteSecContext(uint32_t* minorStatus, GssCtxId** contextHandle);

/*
Shims the gss_wrap method.
*/
extern "C" uint32_t NetSecurity_Wrap(uint32_t* minorStatus,
                                     GssCtxId* contextHandle,
                                     int32_t isEncrypt,
                                     uint8_t* inputBytes,
                                     int32_t offset,
                                     int32_t count,
                                     GssBuffer** outBufferHandle,
                                     uint32_t* outMsgLength);

/*
Shims the gss_unwrap method.
*/
extern "C" uint32_t NetSecurity_Unwrap(uint32_t* minorStatus,
                                       GssCtxId* contextHandle,
                                       uint8_t* inputBytes,
                                       int32_t offset,
                                       int32_t count,
                                       GssBuffer** outBufferHandle,
                                       uint32_t* outMsgLength);

/*
Shims the gss_acquire_cred_with_password method.
*/
extern "C" uint32_t NetSecurity_AcquireCredWithPassword(uint32_t* minorStatus,
                                                        GssName* desiredName,
                                                        char* password,
                                                        uint32_t passwdLen,
                                                        int32_t isInitiate,
                                                        GssCredId** outputCredHandle);
