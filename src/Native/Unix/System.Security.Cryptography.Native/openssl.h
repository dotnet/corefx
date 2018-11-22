// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
//

#pragma once

#include "pal_compiler.h"
#include "opensslshim.h"

DLLEXPORT int32_t CryptoNative_GetX509Thumbprint(X509* x509, uint8_t* pBuf, int32_t cBuf);

DLLEXPORT const ASN1_TIME* CryptoNative_GetX509NotBefore(X509* x509);

DLLEXPORT const ASN1_TIME* CryptoNative_GetX509NotAfter(X509* x509);

DLLEXPORT const ASN1_TIME* CryptoNative_GetX509CrlNextUpdate(X509_CRL* crl);

DLLEXPORT int32_t CryptoNative_GetX509Version(X509* x509);

DLLEXPORT ASN1_OBJECT* CryptoNative_GetX509PublicKeyAlgorithm(X509* x509);

DLLEXPORT ASN1_OBJECT* CryptoNative_GetX509SignatureAlgorithm(X509* x509);

DLLEXPORT int32_t CryptoNative_GetX509PublicKeyParameterBytes(X509* x509, uint8_t* pBuf, int32_t cBuf);

DLLEXPORT ASN1_BIT_STRING* CryptoNative_GetX509PublicKeyBytes(X509* x509);

DLLEXPORT int32_t CryptoNative_GetAsn1StringBytes(ASN1_STRING* asn1, uint8_t* pBuf, int32_t cBuf);

DLLEXPORT int32_t CryptoNative_GetX509NameRawBytes(X509_NAME* x509Name, uint8_t* pBuf, int32_t cBuf);

DLLEXPORT int32_t CryptoNative_GetX509EkuFieldCount(EXTENDED_KEY_USAGE* eku);

DLLEXPORT ASN1_OBJECT* CryptoNative_GetX509EkuField(EXTENDED_KEY_USAGE* eku, int32_t loc);

DLLEXPORT BIO* CryptoNative_GetX509NameInfo(X509* x509, int32_t nameType, int32_t forIssuer);

DLLEXPORT int32_t CryptoNative_CheckX509Hostname(X509* x509, const char* hostname, int32_t cchHostname);

DLLEXPORT int32_t CryptoNative_CheckX509IpAddress(
    X509* x509, const uint8_t* addressBytes, int32_t addressBytesLen, const char* hostname, int32_t cchHostname);

DLLEXPORT int32_t CryptoNative_GetX509StackFieldCount(STACK_OF(X509) * stack);

DLLEXPORT X509* CryptoNative_GetX509StackField(STACK_OF(X509) * stack, int loc);

DLLEXPORT void CryptoNative_RecursiveFreeX509Stack(STACK_OF(X509) * stack);

DLLEXPORT int32_t CryptoNative_SetX509ChainVerifyTime(X509_STORE_CTX* ctx,
                                                      int32_t year,
                                                      int32_t month,
                                                      int32_t day,
                                                      int32_t hour,
                                                      int32_t minute,
                                                      int32_t second,
                                                      int32_t isDst);

DLLEXPORT X509* CryptoNative_ReadX509AsDerFromBio(BIO* bio);

DLLEXPORT int32_t CryptoNative_BioTell(BIO* bio);

DLLEXPORT int32_t CryptoNative_BioSeek(BIO* bio, int32_t ofs);

DLLEXPORT STACK_OF(X509) * CryptoNative_NewX509Stack(void);

DLLEXPORT int32_t CryptoNative_PushX509StackField(STACK_OF(X509) * stack, X509* x509);

DLLEXPORT int32_t CryptoNative_GetRandomBytes(uint8_t* buf, int32_t num);

DLLEXPORT int32_t CryptoNative_LookupFriendlyNameByOid(const char* oidValue, const char** friendlyName);

DLLEXPORT int32_t CryptoNative_EnsureOpenSslInitialized(void);

DLLEXPORT int64_t CryptoNative_OpenSslVersionNumber(void);
