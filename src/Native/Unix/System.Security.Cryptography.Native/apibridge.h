// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Functions based on OpenSSL 1.1 API, used when building against/running with OpenSSL 1.0

#pragma once
#include "pal_types.h"

extern "C" const BIGNUM* local_DSA_get0_key(const DSA* dsa, const BIGNUM** pubKey, const BIGNUM** privKey);
extern "C" void local_DSA_get0_pqg(const DSA* dsa, const BIGNUM** p, const BIGNUM** q, const BIGNUM** g);
extern "C" const DSA_METHOD* local_DSA_get_method(const DSA* dsa);
extern "C" int32_t local_DSA_set0_key(DSA* dsa, BIGNUM* bnY, BIGNUM* bnX);
extern "C" int32_t local_DSA_set0_pqg(DSA* dsa, BIGNUM* bnP, BIGNUM* bnQ, BIGNUM* bnG);
extern "C" void local_EVP_CIPHER_CTX_free(EVP_CIPHER_CTX* ctx);
extern "C" EVP_CIPHER_CTX* local_EVP_CIPHER_CTX_new(void);
extern "C" int32_t local_EVP_CIPHER_CTX_reset(EVP_CIPHER_CTX* ctx);
extern "C" int32_t local_EVP_PKEY_up_ref(EVP_PKEY* pkey);
extern "C" void local_HMAC_CTX_free(HMAC_CTX* ctx);
extern "C" HMAC_CTX* local_HMAC_CTX_new(void);
extern "C" const char* local_OpenSSL_version(int t);
extern "C" void local_RSA_get0_crt_params(const RSA* rsa, const BIGNUM** dmp1, const BIGNUM** dmq1, const BIGNUM** iqmp);
extern "C" void local_RSA_get0_factors(const RSA* rsa, const BIGNUM** p, const BIGNUM** q);
extern "C" void local_RSA_get0_key(const RSA* rsa, const BIGNUM** n, const BIGNUM** e, const BIGNUM** d);
extern "C" int32_t local_RSA_meth_get_flags(const RSA_METHOD* meth);
extern "C" int32_t local_RSA_set0_crt_params(RSA* rsa, BIGNUM* dmp1, BIGNUM* dmq1, BIGNUM* iqmp);
extern "C" int32_t local_RSA_set0_factors(RSA* rsa, BIGNUM* p, BIGNUM* q);
extern "C" int32_t local_RSA_set0_key(RSA* rsa, BIGNUM* n, BIGNUM* e, BIGNUM* d);
extern "C" int32_t local_SSL_is_init_finished(const SSL* ssl);
extern "C" unsigned long local_SSL_CTX_set_options(SSL_CTX* ctx, unsigned long options);
extern "C" void local_SSL_CTX_set_security_level(SSL_CTX* ctx, int32_t level);
extern "C" int local_SSL_session_reused(SSL* ssl);
extern "C" const ASN1_TIME* local_X509_CRL_get0_nextUpdate(const X509_CRL* crl);
extern "C" int32_t local_X509_NAME_get0_der(X509_NAME* x509Name, const uint8_t** pder, size_t* pderlen);
extern "C" int32_t local_X509_PUBKEY_get0_param(
    ASN1_OBJECT** palgOid, const uint8_t** pkeyBytes, int* pkeyBytesLen, X509_ALGOR** palg, X509_PUBKEY* pubkey);
extern "C" X509* local_X509_STORE_CTX_get0_cert(X509_STORE_CTX* ctx);
extern "C" STACK_OF(X509) * local_X509_STORE_CTX_get0_untrusted(X509_STORE_CTX* ctx);
extern "C" const ASN1_TIME* local_X509_get0_notAfter(const X509* x509);
extern "C" const ASN1_TIME* local_X509_get0_notBefore(const X509* x509);
extern "C" ASN1_BIT_STRING* local_X509_get0_pubkey_bitstr(const X509* x509);
extern "C" const X509_ALGOR* local_X509_get0_tbs_sigalg(const X509* x509);
extern "C" X509_PUBKEY* local_X509_get_X509_PUBKEY(const X509* x509);
extern "C" int32_t local_X509_get_version(const X509* x509);
extern "C" int32_t local_X509_up_ref(X509* x509);
