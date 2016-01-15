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

// TODO: temporarily keeping the un-prefixed signature of this method  
// to keep tests running in CI. This will be removed once the managed assemblies  
// are synced up with the native assemblies.
extern "C" EVP_PKEY* GetX509EvpPublicKey(X509* x509)
{
    return CryptoNative_GetX509EvpPublicKey(x509);
}

extern "C" EVP_PKEY* CryptoNative_GetX509EvpPublicKey(X509* x509)
{
    if (!x509)
    {
        return nullptr;
    }

    // X509_get_X509_PUBKEY returns an interior pointer, so should not be freed
    return X509_PUBKEY_get(X509_get_X509_PUBKEY(x509));
}

// TODO: temporarily keeping the un-prefixed signature of this method  
// to keep tests running in CI. This will be removed once the managed assemblies  
// are synced up with the native assemblies.
extern "C" X509_CRL* DecodeX509Crl(const uint8_t* buf, int32_t len)
{
    return CryptoNative_DecodeX509Crl(buf, len);
}

extern "C" X509_CRL* CryptoNative_DecodeX509Crl(const uint8_t* buf, int32_t len)
{
    if (!buf || !len)
    {
        return nullptr;
    }

    return d2i_X509_CRL(nullptr, &buf, len);
}

// TODO: temporarily keeping the un-prefixed signature of this method  
// to keep tests running in CI. This will be removed once the managed assemblies  
// are synced up with the native assemblies.
extern "C" X509* DecodeX509(const uint8_t* buf, int32_t len)
{
    return CryptoNative_DecodeX509(buf, len);
}

extern "C" X509* CryptoNative_DecodeX509(const uint8_t* buf, int32_t len)
{
    if (!buf || !len)
    {
        return nullptr;
    }

    return d2i_X509(nullptr, &buf, len);
}

// TODO: temporarily keeping the un-prefixed signature of this method  
// to keep tests running in CI. This will be removed once the managed assemblies  
// are synced up with the native assemblies.
extern "C" int32_t GetX509DerSize(X509* x)
{
    return CryptoNative_GetX509DerSize(x);
}

extern "C" int32_t CryptoNative_GetX509DerSize(X509* x)
{
    return i2d_X509(x, nullptr);
}

// TODO: temporarily keeping the un-prefixed signature of this method  
// to keep tests running in CI. This will be removed once the managed assemblies  
// are synced up with the native assemblies.
extern "C" int32_t EncodeX509(X509* x, uint8_t* buf)
{
    return CryptoNative_EncodeX509(x, buf);
}

extern "C" int32_t CryptoNative_EncodeX509(X509* x, uint8_t* buf)
{
    return i2d_X509(x, &buf);
}

// TODO: temporarily keeping the un-prefixed signature of this method  
// to keep tests running in CI. This will be removed once the managed assemblies  
// are synced up with the native assemblies.
extern "C" void X509Destroy(X509* a)
{
    return CryptoNative_X509Destroy(a);
}

extern "C" void CryptoNative_X509Destroy(X509* a)
{
    if (a != nullptr)
    {
        X509_free(a);
    }
}

// TODO: temporarily keeping the un-prefixed signature of this method  
// to keep tests running in CI. This will be removed once the managed assemblies  
// are synced up with the native assemblies.
extern "C" X509* X509Duplicate(X509* x509)
{
    return CryptoNative_X509Duplicate(x509);
}

extern "C" X509* CryptoNative_X509Duplicate(X509* x509)
{
    return X509_dup(x509);
}

// TODO: temporarily keeping the un-prefixed signature of this method  
// to keep tests running in CI. This will be removed once the managed assemblies  
// are synced up with the native assemblies.
extern "C" X509* PemReadX509FromBio(BIO* bio)
{
    return CryptoNative_PemReadX509FromBio(bio);
}

extern "C" X509* CryptoNative_PemReadX509FromBio(BIO* bio)
{
    return PEM_read_bio_X509_AUX(bio, nullptr, nullptr, nullptr);
}

// TODO: temporarily keeping the un-prefixed signature of this method  
// to keep tests running in CI. This will be removed once the managed assemblies  
// are synced up with the native assemblies.
extern "C" ASN1_INTEGER* X509GetSerialNumber(X509* x509)
{
    return CryptoNative_X509GetSerialNumber(x509);
}

extern "C" ASN1_INTEGER* CryptoNative_X509GetSerialNumber(X509* x509)
{
    return X509_get_serialNumber(x509);
}

// TODO: temporarily keeping the un-prefixed signature of this method  
// to keep tests running in CI. This will be removed once the managed assemblies  
// are synced up with the native assemblies.
extern "C" X509_NAME* X509GetIssuerName(X509* x509)
{
    return CryptoNative_X509GetIssuerName(x509);
}

extern "C" X509_NAME* CryptoNative_X509GetIssuerName(X509* x509)
{
    return X509_get_issuer_name(x509);
}

// TODO: temporarily keeping the un-prefixed signature of this method  
// to keep tests running in CI. This will be removed once the managed assemblies  
// are synced up with the native assemblies.
extern "C" X509_NAME* X509GetSubjectName(X509* x509)
{
    return CryptoNative_X509GetSubjectName(x509);
}

extern "C" X509_NAME* CryptoNative_X509GetSubjectName(X509* x509)
{
    return X509_get_subject_name(x509);
}

// TODO: temporarily keeping the un-prefixed signature of this method  
// to keep tests running in CI. This will be removed once the managed assemblies  
// are synced up with the native assemblies.
extern "C" int32_t X509CheckPurpose(X509* x, int32_t id, int32_t ca)
{
    return CryptoNative_X509CheckPurpose(x, id, ca);
}

extern "C" int32_t CryptoNative_X509CheckPurpose(X509* x, int32_t id, int32_t ca)
{
    return X509_check_purpose(x, id, ca);
}

// TODO: temporarily keeping the un-prefixed signature of this method  
// to keep tests running in CI. This will be removed once the managed assemblies  
// are synced up with the native assemblies.
extern "C" int32_t X509CheckIssued(X509* issuer, X509* subject)
{
    return CryptoNative_X509CheckIssued(issuer, subject);
}

extern "C" int32_t CryptoNative_X509CheckIssued(X509* issuer, X509* subject)
{
    return X509_check_issued(issuer, subject);
}

// TODO: temporarily keeping the un-prefixed signature of this method  
// to keep tests running in CI. This will be removed once the managed assemblies  
// are synced up with the native assemblies.
extern "C" uint64_t X509IssuerNameHash(X509* x)
{
    return CryptoNative_X509IssuerNameHash(x);
}

extern "C" uint64_t CryptoNative_X509IssuerNameHash(X509* x)
{
    return X509_issuer_name_hash(x);
}

// TODO: temporarily keeping the un-prefixed signature of this method  
// to keep tests running in CI. This will be removed once the managed assemblies  
// are synced up with the native assemblies.
extern "C" int32_t X509GetExtCount(X509* x)
{
    return CryptoNative_X509GetExtCount(x);
}

extern "C" int32_t CryptoNative_X509GetExtCount(X509* x)
{
    return X509_get_ext_count(x);
}

// TODO: temporarily keeping the un-prefixed signature of this method  
// to keep tests running in CI. This will be removed once the managed assemblies  
// are synced up with the native assemblies.
extern "C" X509_EXTENSION* X509GetExt(X509* x, int32_t loc)
{
    return CryptoNative_X509GetExt(x, loc);
}

extern "C" X509_EXTENSION* CryptoNative_X509GetExt(X509* x, int32_t loc)
{
    return X509_get_ext(x, loc);
}

// TODO: temporarily keeping the un-prefixed signature of this method  
// to keep tests running in CI. This will be removed once the managed assemblies  
// are synced up with the native assemblies.
extern "C" ASN1_OBJECT* X509ExtensionGetOid(X509_EXTENSION* x)
{
    return CryptoNative_X509ExtensionGetOid(x);
}

extern "C" ASN1_OBJECT* CryptoNative_X509ExtensionGetOid(X509_EXTENSION* x)
{
    return X509_EXTENSION_get_object(x);
}

// TODO: temporarily keeping the un-prefixed signature of this method  
// to keep tests running in CI. This will be removed once the managed assemblies  
// are synced up with the native assemblies.
extern "C" ASN1_OCTET_STRING* X509ExtensionGetData(X509_EXTENSION* x)
{
    return CryptoNative_X509ExtensionGetData(x);
}

extern "C" ASN1_OCTET_STRING* CryptoNative_X509ExtensionGetData(X509_EXTENSION* x)
{
    return X509_EXTENSION_get_data(x);
}

// TODO: temporarily keeping the un-prefixed signature of this method  
// to keep tests running in CI. This will be removed once the managed assemblies  
// are synced up with the native assemblies.
extern "C" int32_t X509ExtensionGetCritical(X509_EXTENSION* x)
{
    return CryptoNative_X509ExtensionGetCritical(x);
}

extern "C" int32_t CryptoNative_X509ExtensionGetCritical(X509_EXTENSION* x)
{
    return X509_EXTENSION_get_critical(x);
}

// TODO: temporarily keeping the un-prefixed signature of this method  
// to keep tests running in CI. This will be removed once the managed assemblies  
// are synced up with the native assemblies.
extern "C" X509_STORE* X509StoreCreate()
{
    return CryptoNative_X509StoreCreate();
}

extern "C" X509_STORE* CryptoNative_X509StoreCreate()
{
    return X509_STORE_new();
}

// TODO: temporarily keeping the un-prefixed signature of this method  
// to keep tests running in CI. This will be removed once the managed assemblies  
// are synced up with the native assemblies.
extern "C" void X509StoreDestory(X509_STORE* v)
{
    return CryptoNative_X509StoreDestory(v);
}

extern "C" void CryptoNative_X509StoreDestory(X509_STORE* v)
{
    if (v != nullptr)
    {
        X509_STORE_free(v);
    }
}

// TODO: temporarily keeping the un-prefixed signature of this method  
// to keep tests running in CI. This will be removed once the managed assemblies  
// are synced up with the native assemblies.
extern "C" int32_t X509StoreAddCert(X509_STORE* ctx, X509* x)
{
    return CryptoNative_X509StoreAddCert(ctx, x);
}

extern "C" int32_t CryptoNative_X509StoreAddCert(X509_STORE* ctx, X509* x)
{
    return X509_STORE_add_cert(ctx, x);
}

// TODO: temporarily keeping the un-prefixed signature of this method  
// to keep tests running in CI. This will be removed once the managed assemblies  
// are synced up with the native assemblies.
extern "C" int32_t X509StoreAddCrl(X509_STORE* ctx, X509_CRL* x)
{
    return CryptoNative_X509StoreAddCrl(ctx, x);
}

extern "C" int32_t CryptoNative_X509StoreAddCrl(X509_STORE* ctx, X509_CRL* x)
{
    return X509_STORE_add_crl(ctx, x);
}

// TODO: temporarily keeping the un-prefixed signature of this method  
// to keep tests running in CI. This will be removed once the managed assemblies  
// are synced up with the native assemblies.
extern "C" int32_t X509StoreSetRevocationFlag(X509_STORE* ctx, X509RevocationFlag revocationFlag)
{
    return CryptoNative_X509StoreSetRevocationFlag(ctx, revocationFlag);
}

extern "C" int32_t CryptoNative_X509StoreSetRevocationFlag(X509_STORE* ctx, X509RevocationFlag revocationFlag)
{
    unsigned long verifyFlags = X509_V_FLAG_CRL_CHECK;

    if (revocationFlag != X509RevocationFlag::EndCertificateOnly)
    {
        verifyFlags |= X509_V_FLAG_CRL_CHECK_ALL;
    }

    return X509_STORE_set_flags(ctx, verifyFlags);
}

// TODO: temporarily keeping the un-prefixed signature of this method  
// to keep tests running in CI. This will be removed once the managed assemblies  
// are synced up with the native assemblies.
extern "C" X509_STORE_CTX* X509StoreCtxCreate()
{
    return CryptoNative_X509StoreCtxCreate();
}

extern "C" X509_STORE_CTX* CryptoNative_X509StoreCtxCreate()
{
    return X509_STORE_CTX_new();
}

// TODO: temporarily keeping the un-prefixed signature of this method  
// to keep tests running in CI. This will be removed once the managed assemblies  
// are synced up with the native assemblies.
extern "C" void X509StoreCtxDestroy(X509_STORE_CTX* v)
{
    return CryptoNative_X509StoreCtxDestroy(v);
}

extern "C" void CryptoNative_X509StoreCtxDestroy(X509_STORE_CTX* v)
{
    if (v != nullptr)
    {
        X509_STORE_CTX_free(v);
    }
}

// TODO: temporarily keeping the un-prefixed signature of this method  
// to keep tests running in CI. This will be removed once the managed assemblies  
// are synced up with the native assemblies.
extern "C" int32_t X509StoreCtxInit(X509_STORE_CTX* ctx, X509_STORE* store, X509* x509)
{
    return CryptoNative_X509StoreCtxInit(ctx, store, x509);
}

extern "C" int32_t CryptoNative_X509StoreCtxInit(X509_STORE_CTX* ctx, X509_STORE* store, X509* x509)
{
    return X509_STORE_CTX_init(ctx, store, x509, nullptr);
}

// TODO: temporarily keeping the un-prefixed signature of this method  
// to keep tests running in CI. This will be removed once the managed assemblies  
// are synced up with the native assemblies.
extern "C" int32_t X509VerifyCert(X509_STORE_CTX* ctx)
{
    return CryptoNative_X509VerifyCert(ctx);
}

extern "C" int32_t CryptoNative_X509VerifyCert(X509_STORE_CTX* ctx)
{
    return X509_verify_cert(ctx);
}

// TODO: temporarily keeping the un-prefixed signature of this method  
// to keep tests running in CI. This will be removed once the managed assemblies  
// are synced up with the native assemblies.
extern "C" X509Stack* X509StoreCtxGetChain(X509_STORE_CTX* ctx)
{
    return CryptoNative_X509StoreCtxGetChain(ctx);
}

extern "C" X509Stack* CryptoNative_X509StoreCtxGetChain(X509_STORE_CTX* ctx)
{
    return X509_STORE_CTX_get1_chain(ctx);
}

// TODO: temporarily keeping the un-prefixed signature of this method  
// to keep tests running in CI. This will be removed once the managed assemblies  
// are synced up with the native assemblies.
extern "C" X509Stack* X509StoreCtxGetSharedUntrusted(X509_STORE_CTX* ctx)
{
    return CryptoNative_X509StoreCtxGetSharedUntrusted(ctx);
}

extern "C" X509Stack* CryptoNative_X509StoreCtxGetSharedUntrusted(X509_STORE_CTX* ctx)
{
    return ctx ? ctx->untrusted : nullptr;
}

// TODO: temporarily keeping the un-prefixed signature of this method  
// to keep tests running in CI. This will be removed once the managed assemblies  
// are synced up with the native assemblies.
extern "C" X509* X509StoreCtxGetTargetCert(X509_STORE_CTX* ctx)
{
    return CryptoNative_X509StoreCtxGetTargetCert(ctx);
}

extern "C" X509* CryptoNative_X509StoreCtxGetTargetCert(X509_STORE_CTX* ctx)
{
    return ctx ? ctx->cert : nullptr;
}

// TODO: temporarily keeping the un-prefixed signature of this method  
// to keep tests running in CI. This will be removed once the managed assemblies  
// are synced up with the native assemblies.
extern "C" X509VerifyStatusCode X509StoreCtxGetError(X509_STORE_CTX* ctx)
{
    return CryptoNative_X509StoreCtxGetError(ctx);
}

extern "C" X509VerifyStatusCode CryptoNative_X509StoreCtxGetError(X509_STORE_CTX* ctx)
{
    return static_cast<X509VerifyStatusCode>(X509_STORE_CTX_get_error(ctx));
}

// TODO: temporarily keeping the un-prefixed signature of this method  
// to keep tests running in CI. This will be removed once the managed assemblies  
// are synced up with the native assemblies.
extern "C" void X509StoreCtxSetVerifyCallback(X509_STORE_CTX* ctx, X509StoreVerifyCallback callback)
{
    return CryptoNative_X509StoreCtxSetVerifyCallback(ctx, callback);
}

extern "C" void CryptoNative_X509StoreCtxSetVerifyCallback(X509_STORE_CTX* ctx, X509StoreVerifyCallback callback)
{
    X509_STORE_CTX_set_verify_cb(ctx, callback);
}

// TODO: temporarily keeping the un-prefixed signature of this method  
// to keep tests running in CI. This will be removed once the managed assemblies  
// are synced up with the native assemblies.
extern "C" int32_t X509StoreCtxGetErrorDepth(X509_STORE_CTX* ctx)
{
    return CryptoNative_X509StoreCtxGetErrorDepth(ctx);
}

extern "C" int32_t CryptoNative_X509StoreCtxGetErrorDepth(X509_STORE_CTX* ctx)
{
    return X509_STORE_CTX_get_error_depth(ctx);
}

// TODO: temporarily keeping the un-prefixed signature of this method  
// to keep tests running in CI. This will be removed once the managed assemblies  
// are synced up with the native assemblies.
extern "C" const char* X509VerifyCertErrorString(X509VerifyStatusCode n)
{
    return CryptoNative_X509VerifyCertErrorString(n);
}

extern "C" const char* CryptoNative_X509VerifyCertErrorString(X509VerifyStatusCode n)
{
    return X509_verify_cert_error_string(n);
}

// TODO: temporarily keeping the un-prefixed signature of this method  
// to keep tests running in CI. This will be removed once the managed assemblies  
// are synced up with the native assemblies.
extern "C" void X509CrlDestroy(X509_CRL* a)
{
    return CryptoNative_X509CrlDestroy(a);
}

extern "C" void CryptoNative_X509CrlDestroy(X509_CRL* a)
{
    if (a != nullptr)
    {
        X509_CRL_free(a);
    }
}

// TODO: temporarily keeping the un-prefixed signature of this method  
// to keep tests running in CI. This will be removed once the managed assemblies  
// are synced up with the native assemblies.
extern "C" int32_t PemWriteBioX509Crl(BIO* bio, X509_CRL* crl)
{
    return CryptoNative_PemWriteBioX509Crl(bio, crl);
}

extern "C" int32_t CryptoNative_PemWriteBioX509Crl(BIO* bio, X509_CRL* crl)
{
    return PEM_write_bio_X509_CRL(bio, crl);
}

// TODO: temporarily keeping the un-prefixed signature of this method  
// to keep tests running in CI. This will be removed once the managed assemblies  
// are synced up with the native assemblies.
extern "C" X509_CRL* PemReadBioX509Crl(BIO* bio)
{
    return CryptoNative_PemReadBioX509Crl(bio);
}

extern "C" X509_CRL* CryptoNative_PemReadBioX509Crl(BIO* bio)
{
    return PEM_read_bio_X509_CRL(bio, nullptr, nullptr, nullptr);
}

// TODO: temporarily keeping the un-prefixed signature of this method  
// to keep tests running in CI. This will be removed once the managed assemblies  
// are synced up with the native assemblies.
extern "C" int32_t GetX509SubjectPublicKeyInfoDerSize(X509* x509)
{
    return CryptoNative_GetX509SubjectPublicKeyInfoDerSize(x509);
}

extern "C" int32_t CryptoNative_GetX509SubjectPublicKeyInfoDerSize(X509* x509)
{
    if (!x509)
    {
        return 0;
    }

    // X509_get_X509_PUBKEY returns an interior pointer, so should not be freed
    return i2d_X509_PUBKEY(X509_get_X509_PUBKEY(x509), nullptr);
}

// TODO: temporarily keeping the un-prefixed signature of this method  
// to keep tests running in CI. This will be removed once the managed assemblies  
// are synced up with the native assemblies.
extern "C" int32_t EncodeX509SubjectPublicKeyInfo(X509* x509, uint8_t* buf)
{
    return CryptoNative_EncodeX509SubjectPublicKeyInfo(x509, buf);
}

extern "C" int32_t CryptoNative_EncodeX509SubjectPublicKeyInfo(X509* x509, uint8_t* buf)
{
    if (!x509)
    {
        return 0;
    }

    // X509_get_X509_PUBKEY returns an interior pointer, so should not be freed
    return i2d_X509_PUBKEY(X509_get_X509_PUBKEY(x509), &buf);
}
