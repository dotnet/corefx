// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#include "pal_crypto_types.h"
#include "pal_compiler.h"
#include "opensslshim.h"

DLLEXPORT void CryptoNative_OcspRequestDestroy(OCSP_REQUEST* request);

DLLEXPORT int32_t CryptoNative_GetOcspRequestDerSize(OCSP_REQUEST* req);

DLLEXPORT int32_t CryptoNative_EncodeOcspRequest(OCSP_REQUEST* req, uint8_t* buf);

DLLEXPORT OCSP_RESPONSE* CryptoNative_DecodeOcspResponse(const uint8_t* buf, int32_t len);

DLLEXPORT void CryptoNative_OcspResponseDestroy(OCSP_RESPONSE* response);
