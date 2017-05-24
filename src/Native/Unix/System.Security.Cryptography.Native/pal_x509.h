// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#include "pal_crypto_types.h"
#include "opensslshim.h"

/*
These values should be kept in sync with System.Security.Cryptography.X509Certificates.X509RevocationFlag.
*/
enum X509RevocationFlag : int32_t
{
    EndCertificateOnly = 0,
    EntireChain = 1,
    ExcludeRoot = 2,
};

/*
The error codes used when verifying X509 certificate chains.

These values should be kept in sync with Interop.Crypto.X509VerifyStatusCode.
*/
enum X509VerifyStatusCode : int32_t
{
    PAL_X509_V_OK = 0,
    PAL_X509_V_ERR_UNABLE_TO_GET_ISSUER_CERT = 2,
    PAL_X509_V_ERR_UNABLE_TO_GET_CRL = 3,
    PAL_X509_V_ERR_UNABLE_TO_DECRYPT_CRL_SIGNATURE = 5,
    PAL_X509_V_ERR_CERT_SIGNATURE_FAILURE = 7,
    PAL_X509_V_ERR_CRL_SIGNATURE_FAILURE = 8,
    PAL_X509_V_ERR_CERT_NOT_YET_VALID = 9,
    PAL_X509_V_ERR_CERT_HAS_EXPIRED = 10,
    PAL_X509_V_ERR_CRL_NOT_YET_VALID = 11,
    PAL_X509_V_ERR_CRL_HAS_EXPIRED = 12,
    PAL_X509_V_ERR_ERROR_IN_CERT_NOT_BEFORE_FIELD = 13,
    PAL_X509_V_ERR_ERROR_IN_CERT_NOT_AFTER_FIELD = 14,
    PAL_X509_V_ERR_ERROR_IN_CRL_LAST_UPDATE_FIELD = 15,
    PAL_X509_V_ERR_ERROR_IN_CRL_NEXT_UPDATE_FIELD = 16,
    PAL_X509_V_ERR_OUT_OF_MEM = 17,
    PAL_X509_V_ERR_DEPTH_ZERO_SELF_SIGNED_CERT = 18,
    PAL_X509_V_ERR_SELF_SIGNED_CERT_IN_CHAIN = 19,
    PAL_X509_V_ERR_UNABLE_TO_GET_ISSUER_CERT_LOCALLY = 20,
    PAL_X509_V_ERR_UNABLE_TO_VERIFY_LEAF_SIGNATURE = 21,
    PAL_X509_V_ERR_CERT_CHAIN_TOO_LONG = 22,
    PAL_X509_V_ERR_CERT_REVOKED = 23,
    PAL_X509_V_ERR_INVALID_CA = 24,
    PAL_X509_V_ERR_PATH_LENGTH_EXCEEDED = 25,
    PAL_X509_V_ERR_INVALID_PURPOSE = 26,
    PAL_X509_V_ERR_CERT_UNTRUSTED = 27,
    PAL_X509_V_ERR_CERT_REJECTED = 28,
    PAL_X509_V_ERR_KEYUSAGE_NO_CERTSIGN = 32,
    PAL_X509_V_ERR_UNABLE_TO_GET_CRL_ISSUER = 33,
    PAL_X509_V_ERR_UNHANDLED_CRITICAL_EXTENSION = 34,
    PAL_X509_V_ERR_KEYUSAGE_NO_CRL_SIGN = 35,
    PAL_X509_V_ERR_UNHANDLED_CRITICAL_CRL_EXTENSION = 36,
    PAL_X509_V_ERR_INVALID_NON_CA = 37,
    PAL_X509_V_ERR_KEYUSAGE_NO_DIGITAL_SIGNATURE = 39,
    PAL_X509_V_ERR_INVALID_EXTENSION = 41,
    PAL_X509_V_ERR_INVALID_POLICY_EXTENSION = 42,
    PAL_X509_V_ERR_NO_EXPLICIT_POLICY = 43,
};

typedef int32_t (*X509StoreVerifyCallback)(int32_t, X509_STORE_CTX*);

/*
Function:
GetX509EvpPublicKey

Returns a EVP_PKEY* equivalent to the public key of the certificate.
*/
extern "C" EVP_PKEY* CryptoNative_GetX509EvpPublicKey(X509* x509);

/*
Shims the d2i_X509_CRL method and makes it easier to invoke from managed code.
*/
extern "C" X509_CRL* CryptoNative_DecodeX509Crl(const uint8_t* buf, int32_t len);

/*
Shims the d2i_X509 method and makes it easier to invoke from managed code.
*/
extern "C" X509* CryptoNative_DecodeX509(const uint8_t* buf, int32_t len);

/*
Returns the number of bytes it will take to convert
the X509 to a DER format.
*/
extern "C" int32_t CryptoNative_GetX509DerSize(X509* x);

/*
Shims the i2d_X509 method.

Returns the number of bytes written to buf.
*/
extern "C" int32_t CryptoNative_EncodeX509(X509* x, uint8_t* buf);

/*
Cleans up and deletes an X509 instance.

Implemented by calling X509_free.

No-op if a is null.
The given X509 pointer is invalid after this call.
Always succeeds.
*/
extern "C" void CryptoNative_X509Destroy(X509* a);

/*
Shims the X509_dup method.

Returns the duplicated X509 instance.
*/
extern "C" X509* CryptoNative_X509Duplicate(X509* x509);

/*
Shims the PEM_read_bio_X509_AUX method.

Returns the read X509 instance.
*/
extern "C" X509* CryptoNative_PemReadX509FromBio(BIO* bio);

/*
Shims the X509_get_serialNumber method.

Returns the ASN1_INTEGER for the serial number.
*/
extern "C" ASN1_INTEGER* CryptoNative_X509GetSerialNumber(X509* x509);

/*
Shims the X509_get_issuer_name method.

Returns the ASN1_INTEGER for the issuer name.
*/
extern "C" X509_NAME* CryptoNative_X509GetIssuerName(X509* x509);

/*
Shims the X509_get_subject_name method.

Returns the X509_NAME for the subject name.
*/
extern "C" X509_NAME* CryptoNative_X509GetSubjectName(X509* x509);

/*
Shims the X509_check_purpose method.
*/
extern "C" int32_t CryptoNative_X509CheckPurpose(X509* x, int32_t id, int32_t ca);

/*
Shims the X509_check_issued method.
*/
extern "C" int32_t CryptoNative_X509CheckIssued(X509* issuer, X509* subject);

/*
Shims the X509_issuer_name_hash method.
*/
extern "C" uint64_t CryptoNative_X509IssuerNameHash(X509* x);

/*
Shims the X509_get_ext_count method.
*/
extern "C" int32_t CryptoNative_X509GetExtCount(X509* x);

/*
Shims the X509_get_ext method.
*/
extern "C" X509_EXTENSION* CryptoNative_X509GetExt(X509* x, int32_t loc);

/*
Shims the X509_EXTENSION_get_object method.
*/
extern "C" ASN1_OBJECT* CryptoNative_X509ExtensionGetOid(X509_EXTENSION* x);

/*
Shims the X509_EXTENSION_get_data method.
*/
extern "C" ASN1_OCTET_STRING* CryptoNative_X509ExtensionGetData(X509_EXTENSION* x);

/*
Shims the X509_EXTENSION_get_critical method.
*/
extern "C" int32_t CryptoNative_X509ExtensionGetCritical(X509_EXTENSION* x);

/*
Shims the X509_STORE_new method.
*/
extern "C" X509_STORE* CryptoNative_X509StoreCreate();

/*
Shims the X509_STORE_free method.
*/
extern "C" void CryptoNative_X509StoreDestory(X509_STORE* v);

/*
Shims the X509_STORE_add_cert method.
*/
extern "C" int32_t CryptoNative_X509StoreAddCert(X509_STORE* ctx, X509* x);

/*
Shims the X509_STORE_add_crl method.
*/
extern "C" int32_t CryptoNative_X509StoreAddCrl(X509_STORE* ctx, X509_CRL* x);

/*
Sets the correct flags on the X509_STORE for the specified X509RevocationFlag.

Shims the X509_STORE_set_flags method.
*/
extern "C" int32_t CryptoNative_X509StoreSetRevocationFlag(X509_STORE* ctx, X509RevocationFlag revocationFlag);

/*
Shims the X509_STORE_CTX_new method.
*/
extern "C" X509_STORE_CTX* CryptoNative_X509StoreCtxCreate();

/*
Shims the X509_STORE_CTX_free method.
*/
extern "C" void CryptoNative_X509StoreCtxDestroy(X509_STORE_CTX* v);

/*
Shims the X509_STORE_CTX_init method.
*/
extern "C" int32_t CryptoNative_X509StoreCtxInit(X509_STORE_CTX* ctx, X509_STORE* store, X509* x509, X509Stack* extraStore);

/*
Shims the X509_verify_cert method.
*/
extern "C" int32_t CryptoNative_X509VerifyCert(X509_STORE_CTX* ctx);

/*
Shims the X509_STORE_CTX_get1_chain method.
*/
extern "C" X509Stack* CryptoNative_X509StoreCtxGetChain(X509_STORE_CTX* ctx);

/*
Returns the interior pointer to the "untrusted" certificates collection for this X509_STORE_CTX
*/
extern "C" X509Stack* CryptoNative_X509StoreCtxGetSharedUntrusted(X509_STORE_CTX* ctx);

/*
Returns the interior pointer to the target certificate for an X509 certificate chain
*/
extern "C" X509* CryptoNative_X509StoreCtxGetTargetCert(X509_STORE_CTX* ctx);

/*
Shims the X509_STORE_CTX_get_error method.
*/
extern "C" X509VerifyStatusCode CryptoNative_X509StoreCtxGetError(X509_STORE_CTX* ctx);

/*
Shims the X509_STORE_CTX_get_error_depth method.
*/
extern "C" int32_t CryptoNative_X509StoreCtxGetErrorDepth(X509_STORE_CTX* ctx);

/*
Shims the X509_STORE_CTX_set_verify_cb function.
*/
extern "C" void CryptoNative_X509StoreCtxSetVerifyCallback(X509_STORE_CTX* ctx, X509StoreVerifyCallback callback);

/*
Shims the X509_verify_cert_error_string method.
*/
extern "C" const char* CryptoNative_X509VerifyCertErrorString(X509VerifyStatusCode n);

/*
Shims the X509_CRL_free method.
*/
extern "C" void CryptoNative_X509CrlDestroy(X509_CRL* a);

/*
Shims the PEM_write_bio_X509_CRL method.

Returns the number of bytes written.
*/
extern "C" int32_t CryptoNative_PemWriteBioX509Crl(BIO* bio, X509_CRL* crl);

/*
Shims the PEM_read_bio_X509_CRL method.

The new X509_CRL instance.
*/
extern "C" X509_CRL* CryptoNative_PemReadBioX509Crl(BIO* bio);

/*
Returns the number of bytes it will take to convert the SubjectPublicKeyInfo
portion of the X509 to DER format.
*/
extern "C" int32_t CryptoNative_GetX509SubjectPublicKeyInfoDerSize(X509* x);

/*
Shims the i2d_X509_PUBKEY method, providing X509_get_X509_PUBKEY(x) as the input.

Returns the number of bytes written to buf.
*/
extern "C" int32_t CryptoNative_EncodeX509SubjectPublicKeyInfo(X509* x, uint8_t* buf);

/*
Increases the reference count of the X509*, thereby increasing the number of calls
required to the free function.

Unlike X509Duplicate, this modifies an existing object, so no new memory is allocated.

Returns the input value.
*/
extern "C" X509* CryptoNative_X509UpRef(X509* x509);
