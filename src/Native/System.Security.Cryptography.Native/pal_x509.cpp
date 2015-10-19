// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

#include "pal_x509.h"

#include <assert.h>
#include <openssl/pem.h>
#include <openssl/x509v3.h>

static_assert(PAL_X509_V_OK == X509_V_OK, "");
static_assert(PAL_X509_V_ERR_UNABLE_TO_GET_ISSUER_CERT == X509_V_ERR_UNABLE_TO_GET_ISSUER_CERT, "");
static_assert(PAL_X509_V_ERR_UNABLE_TO_GET_CRL == X509_V_ERR_UNABLE_TO_GET_CRL, "");
static_assert(PAL_X509_V_ERR_UNABLE_TO_DECRYPT_CRL_SIGNATURE == X509_V_ERR_UNABLE_TO_DECRYPT_CRL_SIGNATURE, "");
static_assert(PAL_X509_V_ERR_CERT_SIGNATURE_FAILURE == X509_V_ERR_CERT_SIGNATURE_FAILURE, "");
static_assert(PAL_X509_V_ERR_CRL_SIGNATURE_FAILURE == X509_V_ERR_CRL_SIGNATURE_FAILURE, "");
static_assert(PAL_X509_V_ERR_CERT_NOT_YET_VALID == X509_V_ERR_CERT_NOT_YET_VALID, "");
static_assert(PAL_X509_V_ERR_CERT_HAS_EXPIRED == X509_V_ERR_CERT_HAS_EXPIRED, "");
static_assert(PAL_X509_V_ERR_CRL_NOT_YET_VALID == X509_V_ERR_CRL_NOT_YET_VALID, "");
static_assert(PAL_X509_V_ERR_CRL_HAS_EXPIRED == X509_V_ERR_CRL_HAS_EXPIRED, "");
static_assert(PAL_X509_V_ERR_ERROR_IN_CERT_NOT_BEFORE_FIELD == X509_V_ERR_ERROR_IN_CERT_NOT_BEFORE_FIELD, "");
static_assert(PAL_X509_V_ERR_ERROR_IN_CERT_NOT_AFTER_FIELD == X509_V_ERR_ERROR_IN_CERT_NOT_AFTER_FIELD, "");
static_assert(PAL_X509_V_ERR_ERROR_IN_CRL_LAST_UPDATE_FIELD == X509_V_ERR_ERROR_IN_CRL_LAST_UPDATE_FIELD, "");
static_assert(PAL_X509_V_ERR_ERROR_IN_CRL_NEXT_UPDATE_FIELD == X509_V_ERR_ERROR_IN_CRL_NEXT_UPDATE_FIELD, "");
static_assert(PAL_X509_V_ERR_OUT_OF_MEM == X509_V_ERR_OUT_OF_MEM, "");
static_assert(PAL_X509_V_ERR_DEPTH_ZERO_SELF_SIGNED_CERT == X509_V_ERR_DEPTH_ZERO_SELF_SIGNED_CERT, "");
static_assert(PAL_X509_V_ERR_SELF_SIGNED_CERT_IN_CHAIN == X509_V_ERR_SELF_SIGNED_CERT_IN_CHAIN, "");
static_assert(PAL_X509_V_ERR_UNABLE_TO_GET_ISSUER_CERT_LOCALLY == X509_V_ERR_UNABLE_TO_GET_ISSUER_CERT_LOCALLY, "");
static_assert(PAL_X509_V_ERR_UNABLE_TO_VERIFY_LEAF_SIGNATURE == X509_V_ERR_UNABLE_TO_VERIFY_LEAF_SIGNATURE, "");
static_assert(PAL_X509_V_ERR_CERT_CHAIN_TOO_LONG == X509_V_ERR_CERT_CHAIN_TOO_LONG, "");
static_assert(PAL_X509_V_ERR_CERT_REVOKED == X509_V_ERR_CERT_REVOKED, "");
static_assert(PAL_X509_V_ERR_INVALID_CA == X509_V_ERR_INVALID_CA, "");
static_assert(PAL_X509_V_ERR_PATH_LENGTH_EXCEEDED == X509_V_ERR_PATH_LENGTH_EXCEEDED, "");
static_assert(PAL_X509_V_ERR_INVALID_PURPOSE == X509_V_ERR_INVALID_PURPOSE, "");
static_assert(PAL_X509_V_ERR_CERT_UNTRUSTED == X509_V_ERR_CERT_UNTRUSTED, "");
static_assert(PAL_X509_V_ERR_CERT_REJECTED == X509_V_ERR_CERT_REJECTED, "");
static_assert(PAL_X509_V_ERR_KEYUSAGE_NO_CERTSIGN == X509_V_ERR_KEYUSAGE_NO_CERTSIGN, "");
static_assert(PAL_X509_V_ERR_UNABLE_TO_GET_CRL_ISSUER == X509_V_ERR_UNABLE_TO_GET_CRL_ISSUER, "");
static_assert(PAL_X509_V_ERR_UNHANDLED_CRITICAL_EXTENSION == X509_V_ERR_UNHANDLED_CRITICAL_EXTENSION, "");
static_assert(PAL_X509_V_ERR_KEYUSAGE_NO_CRL_SIGN == X509_V_ERR_KEYUSAGE_NO_CRL_SIGN, "");
static_assert(PAL_X509_V_ERR_UNHANDLED_CRITICAL_CRL_EXTENSION == X509_V_ERR_UNHANDLED_CRITICAL_CRL_EXTENSION, "");
static_assert(PAL_X509_V_ERR_INVALID_NON_CA == X509_V_ERR_INVALID_NON_CA, "");
static_assert(PAL_X509_V_ERR_KEYUSAGE_NO_DIGITAL_SIGNATURE == X509_V_ERR_KEYUSAGE_NO_DIGITAL_SIGNATURE, "");
static_assert(PAL_X509_V_ERR_INVALID_EXTENSION == X509_V_ERR_INVALID_EXTENSION, "");
static_assert(PAL_X509_V_ERR_INVALID_POLICY_EXTENSION == X509_V_ERR_INVALID_POLICY_EXTENSION, "");
static_assert(PAL_X509_V_ERR_NO_EXPLICIT_POLICY == X509_V_ERR_NO_EXPLICIT_POLICY, "");

extern "C" EVP_PKEY* GetX509EvpPublicKey(X509* x509)
{
    if (!x509)
    {
        return nullptr;
    }

    // X509_get_X509_PUBKEY returns an interior pointer, so should not be freed
    return X509_PUBKEY_get(X509_get_X509_PUBKEY(x509));
}

extern "C" X509_CRL* DecodeX509Crl(const unsigned char* buf, int32_t len)
{
    if (!buf || !len)
    {
        return nullptr;
    }

    return d2i_X509_CRL(nullptr, &buf, len);
}

extern "C" X509* DecodeX509(const unsigned char* buf, int32_t len)
{
    if (!buf || !len)
    {
        return nullptr;
    }

    return d2i_X509(nullptr, &buf, len);
}

extern "C" int32_t GetX509DerSize(X509* x)
{
    return i2d_X509(x, nullptr);
}

extern "C" int32_t EncodeX509(X509* x, uint8_t* buf)
{
    return i2d_X509(x, &buf);
}

extern "C" void X509Destroy(X509* a)
{
    if (a != nullptr)
    {
        X509_free(a);
    }
}

extern "C" X509* X509Duplicate(X509* x509)
{
    return X509_dup(x509);
}

extern "C" X509* PemReadX509FromBio(BIO* bio)
{
    return PEM_read_bio_X509_AUX(bio, nullptr, nullptr, nullptr);
}

extern "C" ASN1_INTEGER* X509GetSerialNumber(X509* x509)
{
    return X509_get_serialNumber(x509);
}

extern "C" X509_NAME* X509GetIssuerName(X509* x509)
{
    return X509_get_issuer_name(x509);
}

extern "C" X509_NAME* X509GetSubjectName(X509* x509)
{
    return X509_get_subject_name(x509);
}

extern "C" int32_t X509CheckPurpose(X509* x, int32_t id, int32_t ca)
{
    return X509_check_purpose(x, id, ca);
}

extern "C" int32_t X509CheckIssued(X509* issuer, X509* subject)
{
    return X509_check_issued(issuer, subject);
}

extern "C" uint64_t X509IssuerNameHash(X509* x)
{
    return X509_issuer_name_hash(x);
}

extern "C" int32_t X509GetExtCount(X509* x)
{
    return X509_get_ext_count(x);
}

extern "C" X509_EXTENSION* X509GetExt(X509* x, int32_t loc)
{
    return X509_get_ext(x, loc);
}

extern "C" ASN1_OBJECT* X509ExtensionGetOid(X509_EXTENSION* x)
{
    return X509_EXTENSION_get_object(x);
}

extern "C" ASN1_OCTET_STRING* X509ExtensionGetData(X509_EXTENSION* x)
{
    return X509_EXTENSION_get_data(x);
}

extern "C" int32_t X509ExtensionGetCritical(X509_EXTENSION* x)
{
    return X509_EXTENSION_get_critical(x);
}

extern "C" X509_STORE* X509StoreCreate()
{
    return X509_STORE_new();
}

extern "C" void X509StoreDestory(X509_STORE* v)
{
    if (v != nullptr)
    {
        X509_STORE_free(v);
    }
}

extern "C" int32_t X509StoreAddCert(X509_STORE* ctx, X509* x)
{
    return X509_STORE_add_cert(ctx, x);
}

extern "C" int32_t X509StoreAddCrl(X509_STORE* ctx, X509_CRL* x)
{
    return X509_STORE_add_crl(ctx, x);
}

extern "C" int32_t X509StoreSetRevocationFlag(X509_STORE* ctx, X509RevocationFlag revocationFlag)
{
    unsigned long verifyFlags = X509_V_FLAG_CRL_CHECK;

    if (revocationFlag != X509RevocationFlag::EndCertificateOnly)
    {
        verifyFlags |= X509_V_FLAG_CRL_CHECK_ALL;
    }

    return X509_STORE_set_flags(ctx, verifyFlags);
}

extern "C" X509_STORE_CTX* X509StoreCtxCreate()
{
    return X509_STORE_CTX_new();
}

extern "C" void X509StoreCtxDestroy(X509_STORE_CTX* v)
{
    if (v != nullptr)
    {
        X509_STORE_CTX_free(v);
    }
}

extern "C" int32_t X509StoreCtxInit(X509_STORE_CTX* ctx, X509_STORE* store, X509* x509)
{
    return X509_STORE_CTX_init(ctx, store, x509, nullptr);
}

extern "C" int32_t X509VerifyCert(X509_STORE_CTX* ctx)
{
    return X509_verify_cert(ctx);
}

extern "C" X509Stack* X509StoreCtxGetChain(X509_STORE_CTX* ctx)
{
    return X509_STORE_CTX_get1_chain(ctx);
}

extern "C" X509VerifyStatusCode X509StoreCtxGetError(X509_STORE_CTX* ctx)
{
    return static_cast<X509VerifyStatusCode>(X509_STORE_CTX_get_error(ctx));
}

extern "C" int32_t X509StoreCtxGetErrorDepth(X509_STORE_CTX* ctx)
{
    return X509_STORE_CTX_get_error_depth(ctx);
}

extern "C" const char* X509VerifyCertErrorString(X509VerifyStatusCode n)
{
    return X509_verify_cert_error_string(n);
}

extern "C" void X509CrlDestroy(X509_CRL* a)
{
    if (a != nullptr)
    {
        X509_CRL_free(a);
    }
}

extern "C" int32_t PemWriteBioX509Crl(BIO* bio, X509_CRL* crl)
{
    return PEM_write_bio_X509_CRL(bio, crl);
}

extern "C" X509_CRL* PemReadBioX509Crl(BIO* bio)
{
    return PEM_read_bio_X509_CRL(bio, nullptr, nullptr, nullptr);
}

extern "C" int32_t GetX509SubjectPublicKeyInfoDerSize(X509* x509)
{
    if (!x509)
    {
        return 0;
    }

    // X509_get_X509_PUBKEY returns an interior pointer, so should not be freed
    return i2d_X509_PUBKEY(X509_get_X509_PUBKEY(x509), nullptr);
}

extern "C" int32_t EncodeX509SubjectPublicKeyInfo(X509* x509, uint8_t* buf)
{
    if (!x509)
    {
        return 0;
    }

    // X509_get_X509_PUBKEY returns an interior pointer, so should not be freed
    return i2d_X509_PUBKEY(X509_get_X509_PUBKEY(x509), &buf);
}
