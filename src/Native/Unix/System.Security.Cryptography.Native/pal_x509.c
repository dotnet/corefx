// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#include "pal_x509.h"

#include <stdbool.h>
#include <assert.h>

c_static_assert(PAL_X509_V_OK == X509_V_OK);
c_static_assert(PAL_X509_V_ERR_UNABLE_TO_GET_ISSUER_CERT == X509_V_ERR_UNABLE_TO_GET_ISSUER_CERT);
c_static_assert(PAL_X509_V_ERR_UNABLE_TO_GET_CRL == X509_V_ERR_UNABLE_TO_GET_CRL);
c_static_assert(PAL_X509_V_ERR_UNABLE_TO_DECRYPT_CRL_SIGNATURE == X509_V_ERR_UNABLE_TO_DECRYPT_CRL_SIGNATURE);
c_static_assert(PAL_X509_V_ERR_UNABLE_TO_DECODE_ISSUER_PUBLIC_KEY == X509_V_ERR_UNABLE_TO_DECODE_ISSUER_PUBLIC_KEY);
c_static_assert(PAL_X509_V_ERR_CERT_SIGNATURE_FAILURE == X509_V_ERR_CERT_SIGNATURE_FAILURE);
c_static_assert(PAL_X509_V_ERR_CRL_SIGNATURE_FAILURE == X509_V_ERR_CRL_SIGNATURE_FAILURE);
c_static_assert(PAL_X509_V_ERR_CERT_NOT_YET_VALID == X509_V_ERR_CERT_NOT_YET_VALID);
c_static_assert(PAL_X509_V_ERR_CERT_HAS_EXPIRED == X509_V_ERR_CERT_HAS_EXPIRED);
c_static_assert(PAL_X509_V_ERR_CRL_NOT_YET_VALID == X509_V_ERR_CRL_NOT_YET_VALID);
c_static_assert(PAL_X509_V_ERR_CRL_HAS_EXPIRED == X509_V_ERR_CRL_HAS_EXPIRED);
c_static_assert(PAL_X509_V_ERR_ERROR_IN_CERT_NOT_BEFORE_FIELD == X509_V_ERR_ERROR_IN_CERT_NOT_BEFORE_FIELD);
c_static_assert(PAL_X509_V_ERR_ERROR_IN_CERT_NOT_AFTER_FIELD == X509_V_ERR_ERROR_IN_CERT_NOT_AFTER_FIELD);
c_static_assert(PAL_X509_V_ERR_ERROR_IN_CRL_LAST_UPDATE_FIELD == X509_V_ERR_ERROR_IN_CRL_LAST_UPDATE_FIELD);
c_static_assert(PAL_X509_V_ERR_ERROR_IN_CRL_NEXT_UPDATE_FIELD == X509_V_ERR_ERROR_IN_CRL_NEXT_UPDATE_FIELD);
c_static_assert(PAL_X509_V_ERR_OUT_OF_MEM == X509_V_ERR_OUT_OF_MEM);
c_static_assert(PAL_X509_V_ERR_DEPTH_ZERO_SELF_SIGNED_CERT == X509_V_ERR_DEPTH_ZERO_SELF_SIGNED_CERT);
c_static_assert(PAL_X509_V_ERR_SELF_SIGNED_CERT_IN_CHAIN == X509_V_ERR_SELF_SIGNED_CERT_IN_CHAIN);
c_static_assert(PAL_X509_V_ERR_UNABLE_TO_GET_ISSUER_CERT_LOCALLY == X509_V_ERR_UNABLE_TO_GET_ISSUER_CERT_LOCALLY);
c_static_assert(PAL_X509_V_ERR_UNABLE_TO_VERIFY_LEAF_SIGNATURE == X509_V_ERR_UNABLE_TO_VERIFY_LEAF_SIGNATURE);
c_static_assert(PAL_X509_V_ERR_CERT_CHAIN_TOO_LONG == X509_V_ERR_CERT_CHAIN_TOO_LONG);
c_static_assert(PAL_X509_V_ERR_CERT_REVOKED == X509_V_ERR_CERT_REVOKED);
c_static_assert(PAL_X509_V_ERR_INVALID_CA == X509_V_ERR_INVALID_CA);
c_static_assert(PAL_X509_V_ERR_PATH_LENGTH_EXCEEDED == X509_V_ERR_PATH_LENGTH_EXCEEDED);
c_static_assert(PAL_X509_V_ERR_INVALID_PURPOSE == X509_V_ERR_INVALID_PURPOSE);
c_static_assert(PAL_X509_V_ERR_CERT_UNTRUSTED == X509_V_ERR_CERT_UNTRUSTED);
c_static_assert(PAL_X509_V_ERR_CERT_REJECTED == X509_V_ERR_CERT_REJECTED);
c_static_assert(PAL_X509_V_ERR_KEYUSAGE_NO_CERTSIGN == X509_V_ERR_KEYUSAGE_NO_CERTSIGN);
c_static_assert(PAL_X509_V_ERR_UNABLE_TO_GET_CRL_ISSUER == X509_V_ERR_UNABLE_TO_GET_CRL_ISSUER);
c_static_assert(PAL_X509_V_ERR_UNHANDLED_CRITICAL_EXTENSION == X509_V_ERR_UNHANDLED_CRITICAL_EXTENSION);
c_static_assert(PAL_X509_V_ERR_KEYUSAGE_NO_CRL_SIGN == X509_V_ERR_KEYUSAGE_NO_CRL_SIGN);
c_static_assert(PAL_X509_V_ERR_UNHANDLED_CRITICAL_CRL_EXTENSION == X509_V_ERR_UNHANDLED_CRITICAL_CRL_EXTENSION);
c_static_assert(PAL_X509_V_ERR_INVALID_NON_CA == X509_V_ERR_INVALID_NON_CA);
c_static_assert(PAL_X509_V_ERR_KEYUSAGE_NO_DIGITAL_SIGNATURE == X509_V_ERR_KEYUSAGE_NO_DIGITAL_SIGNATURE);
c_static_assert(PAL_X509_V_ERR_INVALID_EXTENSION == X509_V_ERR_INVALID_EXTENSION);
c_static_assert(PAL_X509_V_ERR_INVALID_POLICY_EXTENSION == X509_V_ERR_INVALID_POLICY_EXTENSION);
c_static_assert(PAL_X509_V_ERR_NO_EXPLICIT_POLICY == X509_V_ERR_NO_EXPLICIT_POLICY);

EVP_PKEY* CryptoNative_GetX509EvpPublicKey(X509* x509)
{
    if (!x509)
    {
        return NULL;
    }

    // X509_get_X509_PUBKEY returns an interior pointer, so should not be freed
    return X509_PUBKEY_get(X509_get_X509_PUBKEY(x509));
}

X509_CRL* CryptoNative_DecodeX509Crl(const uint8_t* buf, int32_t len)
{
    if (!buf || !len)
    {
        return NULL;
    }

    return d2i_X509_CRL(NULL, &buf, len);
}

X509* CryptoNative_DecodeX509(const uint8_t* buf, int32_t len)
{
    if (!buf || !len)
    {
        return NULL;
    }

    return d2i_X509(NULL, &buf, len);
}

int32_t CryptoNative_GetX509DerSize(X509* x)
{
    return i2d_X509(x, NULL);
}

int32_t CryptoNative_EncodeX509(X509* x, uint8_t* buf)
{
    return i2d_X509(x, &buf);
}

void CryptoNative_X509Destroy(X509* a)
{
    if (a != NULL)
    {
        X509_free(a);
    }
}

X509* CryptoNative_X509Duplicate(X509* x509)
{
    return X509_dup(x509);
}

X509* CryptoNative_PemReadX509FromBio(BIO* bio)
{
    return PEM_read_bio_X509_AUX(bio, NULL, NULL, NULL);
}

ASN1_INTEGER* CryptoNative_X509GetSerialNumber(X509* x509)
{
    return X509_get_serialNumber(x509);
}

X509_NAME* CryptoNative_X509GetIssuerName(X509* x509)
{
    return X509_get_issuer_name(x509);
}

X509_NAME* CryptoNative_X509GetSubjectName(X509* x509)
{
    return X509_get_subject_name(x509);
}

int32_t CryptoNative_X509CheckPurpose(X509* x, int32_t id, int32_t ca)
{
    return X509_check_purpose(x, id, ca);
}

int32_t CryptoNative_X509CheckIssued(X509* issuer, X509* subject)
{
    return X509_check_issued(issuer, subject);
}

uint64_t CryptoNative_X509IssuerNameHash(X509* x)
{
    return X509_issuer_name_hash(x);
}

int32_t CryptoNative_X509GetExtCount(X509* x)
{
    return X509_get_ext_count(x);
}

X509_EXTENSION* CryptoNative_X509GetExt(X509* x, int32_t loc)
{
    return X509_get_ext(x, loc);
}

ASN1_OBJECT* CryptoNative_X509ExtensionGetOid(X509_EXTENSION* x)
{
    return X509_EXTENSION_get_object(x);
}

ASN1_OCTET_STRING* CryptoNative_X509ExtensionGetData(X509_EXTENSION* x)
{
    return X509_EXTENSION_get_data(x);
}

int32_t CryptoNative_X509ExtensionGetCritical(X509_EXTENSION* x)
{
    return X509_EXTENSION_get_critical(x);
}

X509_STORE* CryptoNative_X509StoreCreate()
{
    return X509_STORE_new();
}

void CryptoNative_X509StoreDestory(X509_STORE* v)
{
    if (v != NULL)
    {
        X509_STORE_free(v);
    }
}

int32_t CryptoNative_X509StoreAddCert(X509_STORE* ctx, X509* x)
{
    return X509_STORE_add_cert(ctx, x);
}

int32_t CryptoNative_X509StoreAddCrl(X509_STORE* ctx, X509_CRL* x)
{
    return X509_STORE_add_crl(ctx, x);
}

int32_t CryptoNative_X509StoreSetRevocationFlag(X509_STORE* ctx, X509RevocationFlag revocationFlag)
{
    unsigned long verifyFlags = X509_V_FLAG_CRL_CHECK;

    if (revocationFlag != EndCertificateOnly)
    {
        verifyFlags |= X509_V_FLAG_CRL_CHECK_ALL;
    }

    return X509_STORE_set_flags(ctx, verifyFlags);
}

X509_STORE_CTX* CryptoNative_X509StoreCtxCreate()
{
    return X509_STORE_CTX_new();
}

void CryptoNative_X509StoreCtxDestroy(X509_STORE_CTX* v)
{
    if (v != NULL)
    {
        X509_STORE_CTX_free(v);
    }
}

int32_t CryptoNative_X509StoreCtxInit(X509_STORE_CTX* ctx, X509_STORE* store, X509* x509, X509Stack* extraStore)
{
    int32_t val = X509_STORE_CTX_init(ctx, store, x509, extraStore);

    if (val != 0)
    {
        X509_STORE_CTX_set_flags(ctx, X509_V_FLAG_CHECK_SS_SIGNATURE);
    }

    return val;
}

int32_t CryptoNative_X509VerifyCert(X509_STORE_CTX* ctx)
{
    return X509_verify_cert(ctx);
}

X509Stack* CryptoNative_X509StoreCtxGetChain(X509_STORE_CTX* ctx)
{
    return X509_STORE_CTX_get1_chain(ctx);
}

X509Stack* CryptoNative_X509StoreCtxGetSharedUntrusted(X509_STORE_CTX* ctx)
{
    if (ctx)
    {
        return X509_STORE_CTX_get0_untrusted(ctx);
    }

    return NULL;
}

X509* CryptoNative_X509StoreCtxGetTargetCert(X509_STORE_CTX* ctx)
{
    if (ctx)
    {
        return X509_STORE_CTX_get0_cert(ctx);
    }

    return NULL;
}

X509VerifyStatusCode CryptoNative_X509StoreCtxGetError(X509_STORE_CTX* ctx)
{
    return (unsigned int)X509_STORE_CTX_get_error(ctx);
}

void CryptoNative_X509StoreCtxSetVerifyCallback(X509_STORE_CTX* ctx, X509StoreVerifyCallback callback)
{
    X509_STORE_CTX_set_verify_cb(ctx, callback);
}

int32_t CryptoNative_X509StoreCtxGetErrorDepth(X509_STORE_CTX* ctx)
{
    return X509_STORE_CTX_get_error_depth(ctx);
}

const char* CryptoNative_X509VerifyCertErrorString(X509VerifyStatusCode n)
{
    return X509_verify_cert_error_string(n);
}

void CryptoNative_X509CrlDestroy(X509_CRL* a)
{
    if (a != NULL)
    {
        X509_CRL_free(a);
    }
}

int32_t CryptoNative_PemWriteBioX509Crl(BIO* bio, X509_CRL* crl)
{
    return PEM_write_bio_X509_CRL(bio, crl);
}

X509_CRL* CryptoNative_PemReadBioX509Crl(BIO* bio)
{
    return PEM_read_bio_X509_CRL(bio, NULL, NULL, NULL);
}

int32_t CryptoNative_GetX509SubjectPublicKeyInfoDerSize(X509* x509)
{
    if (!x509)
    {
        return 0;
    }

    // X509_get_X509_PUBKEY returns an interior pointer, so should not be freed
    return i2d_X509_PUBKEY(X509_get_X509_PUBKEY(x509), NULL);
}

int32_t CryptoNative_EncodeX509SubjectPublicKeyInfo(X509* x509, uint8_t* buf)
{
    if (!x509)
    {
        return 0;
    }

    // X509_get_X509_PUBKEY returns an interior pointer, so should not be freed
    return i2d_X509_PUBKEY(X509_get_X509_PUBKEY(x509), &buf);
}

X509* CryptoNative_X509UpRef(X509* x509)
{
    if (x509 != NULL)
    {
        X509_up_ref(x509);
    }

    return x509;
}
