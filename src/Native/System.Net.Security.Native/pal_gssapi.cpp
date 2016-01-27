// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

#include "pal_types.h"
#include "pal_utilities.h"
#include "pal_gssapi.h"

#if HAVE_GSSFW_HEADERS
#include <GSS/GSS.h>
#else
#include <gssapi/gssapi_ext.h>
#include <gssapi/gssapi_krb5.h>
#endif

#include <assert.h>
#include <string.h>

static_assert(PAL_GSS_C_DELEG_FLAG == GSS_C_DELEG_FLAG, "");
static_assert(PAL_GSS_C_MUTUAL_FLAG == GSS_C_MUTUAL_FLAG, "");
static_assert(PAL_GSS_C_REPLAY_FLAG == GSS_C_REPLAY_FLAG, "");
static_assert(PAL_GSS_C_SEQUENCE_FLAG == GSS_C_SEQUENCE_FLAG, "");
static_assert(PAL_GSS_C_CONF_FLAG == GSS_C_CONF_FLAG, "");
static_assert(PAL_GSS_C_INTEG_FLAG == GSS_C_INTEG_FLAG, "");
static_assert(PAL_GSS_C_ANON_FLAG == GSS_C_ANON_FLAG, "");
static_assert(PAL_GSS_C_PROT_READY_FLAG == GSS_C_PROT_READY_FLAG, "");
static_assert(PAL_GSS_C_TRANS_FLAG == GSS_C_TRANS_FLAG, "");
static_assert(PAL_GSS_C_DCE_STYLE == GSS_C_DCE_STYLE, "");
static_assert(PAL_GSS_C_IDENTIFY_FLAG == GSS_C_IDENTIFY_FLAG, "");
static_assert(PAL_GSS_C_EXTENDED_ERROR_FLAG == GSS_C_EXTENDED_ERROR_FLAG, "");
static_assert(PAL_GSS_C_DELEG_POLICY_FLAG == GSS_C_DELEG_POLICY_FLAG, "");

static_assert(PAL_GSS_COMPLETE == GSS_S_COMPLETE, "");
static_assert(PAL_GSS_CONTINUE_NEEDED == GSS_S_CONTINUE_NEEDED, "");

#if !(HAVE_GSS_SPNEGO_MECHANISM)
static char gss_mech_value[] = "\x2b\x06\x01\x05\x05\x02"; // Binary representation of SPNEGO Oid (RFC 4178)
#endif

static void NetSecurity_HandleError(uint32_t majorStatus, gss_buffer_t* outBufferHandle, uint32_t* outBufferLength)
{
    assert(outBufferHandle != nullptr);
    assert(outBufferLength != nullptr);
    if (GSS_ERROR(majorStatus) || (*outBufferHandle)->length > UnsignedCast(std::numeric_limits<uint32_t>::max()))
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

extern "C" uint32_t NetSecurity_AcquireCredSpNego(uint32_t* minorStatus,
                                                  gss_name_t desiredName,
                                                  int32_t isInitiate,
                                                  gss_cred_id_t* outputCredHandle)
{
    assert(minorStatus != nullptr);
    assert(outputCredHandle != nullptr);
    assert(isInitiate == 0 || isInitiate == 1);
#if HAVE_GSS_SPNEGO_MECHANISM
    gss_OID_set_desc gss_mech_spnego_OID_set_desc = {.count = 1, .elements = GSS_SPNEGO_MECHANISM};
#else
    gss_OID_desc gss_mech_spnego_OID_desc = {.length = 6, .value = static_cast<void*>(gss_mech_value)};
    gss_OID_set_desc gss_mech_spnego_OID_set_desc = {.count = 1, .elements = &gss_mech_spnego_OID_desc};
#endif
    gss_cred_usage_t credUsage = isInitiate ? GSS_C_INITIATE : GSS_C_ACCEPT;
    return gss_acquire_cred(
        minorStatus, desiredName, 0, &gss_mech_spnego_OID_set_desc, credUsage, outputCredHandle, nullptr, nullptr);
}

extern "C" uint32_t NetSecurity_DeleteSecContext(uint32_t* minorStatus, gss_ctx_id_t* contextHandle)
{
    assert(contextHandle != nullptr);
    assert(minorStatus != nullptr);
    return gss_delete_sec_context(minorStatus, contextHandle, GSS_C_NO_BUFFER);
}

extern "C" uint32_t NetSecurity_DisplayStatus(uint32_t* minorStatus,
                                              uint32_t statusValue,
                                              int32_t isGssMechCode,
                                              gss_buffer_t* outBufferHandle,
                                              uint32_t* statusLength)
{
    assert(minorStatus != nullptr);
    assert(outBufferHandle != nullptr);
    assert(statusLength != nullptr);
    assert(isGssMechCode == 0 || isGssMechCode == 1);
    int statusType = isGssMechCode ? GSS_C_MECH_CODE : GSS_C_GSS_CODE;
    *outBufferHandle = new gss_buffer_desc();
    uint32_t majorStatus =
        gss_display_status(minorStatus, statusValue, statusType, GSS_C_NO_OID, nullptr, *outBufferHandle);
    NetSecurity_HandleError(majorStatus, outBufferHandle, statusLength);
    return majorStatus;
}

extern "C" uint32_t
NetSecurity_ImportUserName(uint32_t* minorStatus, char* inputName, uint32_t inputNameLen, gss_name_t* outputName)
{
    assert(outputName != nullptr);
    assert(minorStatus != nullptr);
    assert(inputName != nullptr);
    assert(outputName != nullptr);
    gss_buffer_desc inputNameBuffer{.length = inputNameLen, .value = inputName};
    gss_OID nameType = const_cast<gss_OID>(GSS_C_NT_USER_NAME);
    return gss_import_name(minorStatus, &inputNameBuffer, nameType, outputName);
}

extern "C" uint32_t
NetSecurity_ImportPrincipalName(uint32_t* minorStatus, char* inputName, uint32_t inputNameLen, gss_name_t* outputName)
{
    assert(minorStatus != nullptr);
    assert(inputNameLen >= 0);
    assert(outputName != nullptr);
    gss_buffer_desc inputNameBuffer{.length = inputNameLen, .value = inputName};
    gss_OID nameType = const_cast<gss_OID>(GSS_KRB5_NT_PRINCIPAL_NAME);
    return gss_import_name(minorStatus, &inputNameBuffer, nameType, outputName);
}

extern "C" uint32_t NetSecurity_InitSecContext(uint32_t* minorStatus,
                                               gss_cred_id_t claimantCredHandle,
                                               gss_ctx_id_t* contextHandle,
                                               uint32_t isNtlm,
                                               gss_name_t targetName,
                                               uint32_t reqFlags,
                                               uint8_t* inputBytes,
                                               uint32_t inputLength,
                                               gss_buffer_t* outBufferHandle,
                                               uint32_t* outTokenLength,
                                               uint32_t* retFlags)
{
    assert(isNtlm == 0 || isNtlm == 1);
    assert(minorStatus != nullptr);
    assert(contextHandle != nullptr);
    assert(outBufferHandle != nullptr);
    assert(outTokenLength != nullptr);
    assert(retFlags != nullptr);

#if HAVE_GSS_SPNEGO_MECHANISM
    gss_OID desiredMech = isNtlm ? GSS_NTLM_MECHANISM : GSS_SPNEGO_MECHANISM;
#else
    assert(!isNtlm && "NTLM is not supported by MIT libgssapi_krb5");
    gss_OID_desc gss_mech_spnego_OID_desc = {.length = 6, .value = static_cast<void*>(gss_mech_value)};
    gss_OID desiredMech = &gss_mech_spnego_OID_desc;
#endif

    gss_buffer_desc inputToken{.length = UnsignedCast(inputLength), .value = inputBytes};
    *outBufferHandle = new gss_buffer_desc();

    uint32_t majorStatus = gss_init_sec_context(minorStatus,
                                                claimantCredHandle,
                                                contextHandle,
                                                targetName,
                                                desiredMech,
                                                reqFlags,
                                                0,
                                                GSS_C_NO_CHANNEL_BINDINGS,
                                                &inputToken,
                                                nullptr,
                                                *outBufferHandle,
                                                retFlags,
                                                nullptr);

    NetSecurity_HandleError(majorStatus, outBufferHandle, outTokenLength);
    return majorStatus;
}

extern "C" uint32_t NetSecurity_ReleaseCred(uint32_t* minorStatus, gss_cred_id_t* credHandle)
{
    assert(minorStatus != nullptr);
    assert(credHandle != nullptr);
    return gss_release_cred(minorStatus, credHandle);
}

extern "C" uint32_t NetSecurity_ReleaseBuffer(uint32_t* minorStatus, gss_buffer_t buffer)
{
    assert(minorStatus != nullptr);
    assert(buffer != nullptr);
    uint32_t status = gss_release_buffer(minorStatus, buffer);
    delete buffer;
    return status;
}

extern "C" void NetSecurity_CopyBuffer(gss_buffer_t bufferHandle, uint8_t* bytes, uint32_t capacity, uint32_t offset)
{
    assert(bufferHandle != nullptr);
    printf("capacity = %d, offset = %d, length = %zu \n", capacity, offset, bufferHandle->length);
    assert(bufferHandle->length <= (capacity - offset));
    memcpy(bytes + offset, bufferHandle->value, bufferHandle->length);
}

extern "C" uint32_t NetSecurity_ReleaseName(uint32_t* minorStatus, gss_name_t* inputName)
{
    assert(minorStatus != nullptr);
    assert(inputName != nullptr);
    return gss_release_name(minorStatus, inputName);
}

extern "C" uint32_t NetSecurity_Wrap(uint32_t* minorStatus,
                                     gss_ctx_id_t contextHandle,
                                     int32_t isEncrypt,
                                     uint8_t* inputBytes,
                                     int32_t offset,
                                     int32_t count,
                                     gss_buffer_t* outBufferHandle,
                                     uint32_t* outMsgLength)
{
    assert(minorStatus != nullptr);
    assert(contextHandle != nullptr);
    assert(isEncrypt == 1 || isEncrypt == 0);
    int confState;
    gss_buffer_desc inputMessageBuffer{.length = UnsignedCast(count), .value = inputBytes + offset};
    *outBufferHandle = new gss_buffer_desc();
    uint32_t majorStatus = gss_wrap(
        minorStatus, contextHandle, isEncrypt, GSS_C_QOP_DEFAULT, &inputMessageBuffer, &confState, *outBufferHandle);
    NetSecurity_HandleError(majorStatus, outBufferHandle, outMsgLength);
    return majorStatus;
}

extern "C" uint32_t NetSecurity_Unwrap(uint32_t* minorStatus,
                                       gss_ctx_id_t contextHandle,
                                       uint8_t* inputBytes,
                                       int32_t offset,
                                       int32_t count,
                                       gss_buffer_t* outBufferHandle,
                                       uint32_t* outMsgLength)
{
    assert(minorStatus != nullptr);
    assert(contextHandle != nullptr);
    assert(inputBytes != nullptr);
    assert(outBufferHandle != nullptr);
    assert(outMsgLength != nullptr);

    gss_buffer_desc inputMessageBuffer{.length = UnsignedCast(count), .value = inputBytes + offset};
    *outBufferHandle = new gss_buffer_desc();
    uint32_t majorStatus =
        gss_unwrap(minorStatus, contextHandle, &inputMessageBuffer, *outBufferHandle, nullptr, nullptr);
    NetSecurity_HandleError(majorStatus, outBufferHandle, outMsgLength);
    return majorStatus;
}

extern "C" uint32_t NetSecurity_AcquireCredWithPassword(uint32_t* minorStatus,
                                                        const gss_name_t desiredName,
                                                        char* password,
                                                        uint32_t passwdLen,
                                                        int32_t isInitiate,
                                                        gss_cred_id_t* outputCredHandle)
{
    assert(minorStatus != nullptr);
    assert(desiredName != nullptr);
    assert(outputCredHandle != nullptr);
    assert(isInitiate == 0 || isInitiate == 1);
    gss_cred_usage_t credUsage = isInitiate ? GSS_C_INITIATE : GSS_C_ACCEPT;
    gss_buffer_desc passwordBuffer{.length = passwdLen, .value = password};

    return gss_acquire_cred_with_password(
        minorStatus, desiredName, &passwordBuffer, 0, nullptr, credUsage, outputCredHandle, nullptr, nullptr);
}
