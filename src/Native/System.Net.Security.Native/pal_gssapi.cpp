// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

#include "pal_gssapi.h"
#include <assert.h>
#include <string.h>

static void NetSecurity_HandleError(uint32_t majorStatus, gss_buffer_t *outBufferHandle, uint32_t *outBufferLength)
{
    if (GSS_ERROR(majorStatus) || *outBufferHandle == nullptr || (*outBufferHandle)->length > UnsignedCast(std::numeric_limits<int>::max()))
    {
        uint32_t relBufferStatus;
        gss_release_buffer(&relBufferStatus, *outBufferHandle);
        *outBufferHandle = nullptr;
        *outBufferLength = 0;
    }
    else
    {
        *outBufferLength = static_cast<uint32_t>((*outBufferHandle)->length);
    }
}

extern "C" uint32_t NetSecurity_AcquireCredSpNego(uint32_t* minorStatus, gss_name_t desiredName, int32_t isInitiate, gss_cred_id_t* outputCredHandle)
{
    assert(isInitiate == 0 || isInitiate == 1);
#if HAVE_GSS_SPNEGO_MECHANISM
    gss_OID_set_desc gss_mech_spnego_OID_set_desc = {1, GSS_SPNEGO_MECHANISM};
#else
    char gss_mech_value[] = "\x2b\x06\x01\x05\x05\x02";  // Binary representation of SPNEGO Oid (RFC 4178)
    gss_OID_desc gss_mech_spnego_OID_desc = {6, static_cast<void*>(gss_mech_value)};
    gss_OID_set_desc gss_mech_spnego_OID_set_desc = {1, &gss_mech_spnego_OID_desc};
#endif
    gss_cred_usage_t credUsage = isInitiate ? GSS_C_INITIATE : GSS_C_ACCEPT;
    return gss_acquire_cred(minorStatus, desiredName, 0, &gss_mech_spnego_OID_set_desc, credUsage, outputCredHandle, nullptr, nullptr);
}

extern "C" uint32_t NetSecurity_DeleteSecContext(uint32_t* minorStatus, gss_ctx_id_t* contextHandle)
{
    return gss_delete_sec_context(minorStatus, contextHandle, GSS_C_NO_BUFFER);
}

extern "C" uint32_t NetSecurity_DisplayName(uint32_t* minorStatus, gss_name_t inputName, gss_buffer_t outputNameBuffer)
{
    return gss_display_name(minorStatus, inputName, outputNameBuffer, nullptr);
}

extern "C" uint32_t NetSecurity_DisplayStatus(uint32_t* minorStatus, uint32_t statusValue, int32_t isGssMechCode, gss_buffer_t *outBufferHandle, uint32_t *statusLength)
{
    assert (isGssMechCode == 0 || isGssMechCode == 1);
    int statusType = isGssMechCode ? GSS_C_MECH_CODE : GSS_C_GSS_CODE;
    *outBufferHandle = reinterpret_cast<gss_buffer_t>(malloc(sizeof(gss_buffer_desc)));
    uint32_t majorStatus = gss_display_status(minorStatus, statusValue, statusType, GSS_C_NO_OID, nullptr, *outBufferHandle);
    NetSecurity_HandleError(majorStatus, outBufferHandle, statusLength);
    return majorStatus;
}

extern "C" uint32_t NetSecurity_ImportUserName(uint32_t* minorStatus, char* inputName, uint32_t inputNameLen, gss_name_t* outputName)
{
    gss_buffer_desc inputNameBuffer {inputNameLen, inputName};
    gss_OID nameType = const_cast<gss_OID>(GSS_C_NT_USER_NAME);
    return gss_import_name(minorStatus, &inputNameBuffer, nameType, outputName);
}

extern "C" uint32_t NetSecurity_ImportPrincipalName(uint32_t* minorStatus, char* inputName, uint32_t inputNameLen, gss_name_t* outputName)
{
    gss_buffer_desc inputNameBuffer {inputNameLen, inputName};
    gss_OID nameType = const_cast<gss_OID>(GSS_KRB5_NT_PRINCIPAL_NAME);
    return gss_import_name(minorStatus, &inputNameBuffer, nameType, outputName);
}

extern "C" uint32_t NetSecurity_InitSecContext(uint32_t* minorStatus, gss_cred_id_t claimantCredHandle, gss_ctx_id_t* contextHandle,
                                               uint32_t isNtlm, gss_name_t targetName, uint32_t reqFlags, uint8_t* inputBytes, uint32_t inputLength,
                                               gss_buffer_t *outBufferHandle, uint32_t *outTokenLength, uint32_t* retFlags)
{
    assert(isNtlm == 0 || isNtlm == 1);
#if HAVE_GSS_SPNEGO_MECHANISM
    gss_OID desiredMech = isNtlm? GSS_NTLM_MECHANISM : GSS_SPNEGO_MECHANISM;
#else
    assert(!isNtlm && "NTLM is not supported by MIT libgssapi_krb5");
    char gss_mech_value[] = "\x2b\x06\x01\x05\x05\x02"; // Binary representation of SPNEGO Oid (RFC 4178)
    gss_OID_desc gss_mech_spnego_OID_desc = {6, static_cast<void*>(gss_mech_value)};
    gss_OID desiredMech = &gss_mech_spnego_OID_desc;
#endif

    gss_buffer_desc inputToken {UnsignedCast(inputLength), inputBytes};
    *outBufferHandle =  reinterpret_cast<gss_buffer_t>(malloc(sizeof(gss_buffer_desc)));

    uint32_t majorStatus = gss_init_sec_context(minorStatus, claimantCredHandle, contextHandle, targetName, desiredMech, reqFlags,
                                                0, GSS_C_NO_CHANNEL_BINDINGS, &inputToken, nullptr, *outBufferHandle, retFlags, nullptr);

    NetSecurity_HandleError(majorStatus, outBufferHandle, outTokenLength);
    return majorStatus;
}

extern "C" uint32_t NetSecurity_ReleaseCred(uint32_t* minorStatus, gss_cred_id_t* credHandle)
{
    return gss_release_cred(minorStatus, credHandle);
}

extern "C" uint32_t NetSecurity_ReleaseBuffer(uint32_t* minor_status, gss_buffer_t buffer)
{
    return gss_release_buffer(minor_status, buffer);
}


extern "C" void NetSecurity_CopyBuffer(gss_buffer_t bufferHandle, uint8_t *bytes, uint32_t offset)
{
    memcpy(bytes + offset, bufferHandle->value, bufferHandle->length);
}

extern "C" uint32_t NetSecurity_ReleaseName(uint32_t* minorStatus, gss_name_t* inputName)
{
    return gss_release_name(minorStatus, inputName);
}

extern "C" uint32_t NetSecurity_Wrap(uint32_t* minorStatus, gss_ctx_id_t contextHandle, int32_t isEncrypt,
                                     uint8_t* inputBytes, int32_t offset, int32_t count, gss_buffer_t *outBufferHandle, uint32_t *outMsgLength)
{
    gss_buffer_desc inputMessageBuffer {UnsignedCast(count), inputBytes + offset};
    int confState;
    assert(isEncrypt == 1 || isEncrypt == 0);
    *outBufferHandle = reinterpret_cast<gss_buffer_t>(malloc(sizeof(gss_buffer_desc)));
    uint32_t majorStatus = gss_wrap(minorStatus, contextHandle, isEncrypt, GSS_C_QOP_DEFAULT, &inputMessageBuffer, &confState, *outBufferHandle);
    NetSecurity_HandleError(majorStatus, outBufferHandle, outMsgLength);
    return majorStatus;
}

extern "C" uint32_t NetSecurity_Unwrap(uint32_t* minorStatus, gss_ctx_id_t contextHandle,
                                       uint8_t* inputBytes, int32_t offset, int32_t count, gss_buffer_t *outBufferHandle, uint32_t *outMsgLength)
{
    gss_buffer_desc inputMessageBuffer {UnsignedCast(count), inputBytes + offset};
    *outBufferHandle = reinterpret_cast<gss_buffer_t>(malloc(sizeof(gss_buffer_desc)));
    uint32_t majorStatus = gss_unwrap(minorStatus, contextHandle, &inputMessageBuffer, *outBufferHandle, nullptr, nullptr);
    NetSecurity_HandleError(majorStatus, outBufferHandle, outMsgLength);
    return majorStatus;
}

extern "C" uint32_t NetSecurity_AcquireCredWithPassword(uint32_t* minorStatus, const gss_name_t desiredName, char* password, uint32_t passwdLen,
                                                        int32_t isInitiate, gss_cred_id_t* outputCredHandle)
{
    assert(isInitiate == 0 || isInitiate == 1);
    gss_cred_usage_t credUsage = isInitiate ? GSS_C_INITIATE : GSS_C_ACCEPT;
    gss_buffer_desc passwordBuffer {passwdLen, password};

    return gss_acquire_cred_with_password(minorStatus, desiredName, &passwordBuffer, 0, nullptr,
                                      credUsage, outputCredHandle, nullptr, nullptr);
}
