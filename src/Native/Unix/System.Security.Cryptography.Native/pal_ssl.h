// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#include "pal_crypto_types.h"
#include "pal_compiler.h"
#include "opensslshim.h"

/*
These values should be kept in sync with System.Security.Authentication.SslProtocols.
*/
typedef enum
{
    PAL_SSL_NONE = 0,
    PAL_SSL_SSL2 = 12,
    PAL_SSL_SSL3 = 48,
    PAL_SSL_TLS = 192,
    PAL_SSL_TLS11 = 768,
    PAL_SSL_TLS12 = 3072
} SslProtocols;

/*
These values should be kept in sync with System.Net.Security.EncryptionPolicy.
*/
typedef enum
{
    RequireEncryption = 0,
    AllowNoEncryption,
    NoEncryption
} EncryptionPolicy;

/*
These values should be kept in sync with System.Security.Authentication.CipherAlgorithmType.
*/
typedef enum
{
    CipherAlgorithmType_None = 0,
    Null = 24576,
    Des = 26113,
    Rc2 = 26114,
    TripleDes = 26115,
    Aes128 = 26126,
    Aes192 = 26127,
    Aes256 = 26128,
    Aes = 26129,
    Rc4 = 26625,

    // Algorithm constants which are not present in the managed CipherAlgorithmType enum.
    SSL_IDEA = 229380,
    SSL_CAMELLIA128 = 229381,
    SSL_CAMELLIA256 = 229382,
    SSL_eGOST2814789CNT = 229383,
    SSL_SEED = 229384,
} CipherAlgorithmType;

/*
These values should be kept in sync with System.Security.Authentication.ExchangeAlgorithmType.
*/
typedef enum
{
    ExchangeAlgorithmType_None,
    RsaSign = 9216,
    RsaKeyX = 41984,
    DiffieHellman = 43522,

    // ExchangeAlgorithm constants which are not present in the managed ExchangeAlgorithmType enum.
    SSL_ECDH = 43525,
    SSL_ECDSA = 41475,
    SSL_ECDHE = 44550,
    SSL_kPSK = 229390,
    SSL_kGOST = 229391,
    SSL_kSRP = 229392,
    SSL_kKRB5 = 229393,
} ExchangeAlgorithmType;

/*
These values should be kept in sync with System.Security.Authentication.HashAlgorithmType.
*/
typedef enum
{
    HashAlgorithmType_None = 0,
    Md5 = 32771,
    Sha1 = 32772,

    // HashAlgorithm constants which are not present in the managed HashAlgorithmType enum.
    SSL_SHA256 = 32780,
    SSL_SHA384 = 32781,
    SSL_GOST94 = 229410,
    SSL_GOST89 = 229411,
    SSL_AEAD = 229412,
} HashAlgorithmType;

typedef enum
{
    MD5_HashKeySize = 8 * MD5_DIGEST_LENGTH,
    SHA1_HashKeySize = 8 * SHA_DIGEST_LENGTH,
    SHA256_HashKeySize = 8 * SHA256_DIGEST_LENGTH,
    SHA384_HashKeySize = 8 * SHA384_DIGEST_LENGTH,
    GOST_HashKeySize = 256,
    Default = 0,
} DataHashSize;

typedef enum
{
    PAL_SSL_ERROR_NONE = 0,
    PAL_SSL_ERROR_SSL = 1,
    PAL_SSL_ERROR_WANT_READ = 2,
    PAL_SSL_ERROR_WANT_WRITE = 3,
    PAL_SSL_ERROR_SYSCALL = 5,
    PAL_SSL_ERROR_ZERO_RETURN = 6,
} SslErrorCode;

// the function pointer definition for the callback used in SslCtxSetVerify
typedef int32_t (*SslCtxSetVerifyCallback)(int32_t, X509_STORE_CTX*);

// the function pointer definition for the callback used in SslCtxSetCertVerifyCallback
typedef int32_t (*SslCtxSetCertVerifyCallbackCallback)(X509_STORE_CTX*, void* arg);

// the function pointer definition for the callback used in SslCtxSetClientCertCallback
typedef int32_t (*SslClientCertCallback)(SSL* ssl, X509** x509, EVP_PKEY** pkey);

// the function pointer definition for the callback used in SslCtxSetAlpnSelectCb
typedef int32_t (*SslCtxSetAlpnCallback)(SSL* ssl,
    const uint8_t** out,
    uint8_t* outlen,
    const uint8_t* in,
    uint32_t inlen,
    void* arg);
/*
Ensures that libssl is correctly initialized and ready to use.
*/
DLLEXPORT void CryptoNative_EnsureLibSslInitialized(void);

/*
Shims the SSLv23_method method.

Returns the requested SSL_METHOD.
*/
DLLEXPORT const SSL_METHOD* CryptoNative_SslV2_3Method(void);

/*
Shims the SSL_CTX_new method.

Returns the new SSL_CTX instance.
*/
DLLEXPORT SSL_CTX* CryptoNative_SslCtxCreate(SSL_METHOD* method);

/*
Sets the specified protocols in the SSL_CTX options.
*/
DLLEXPORT void CryptoNative_SetProtocolOptions(SSL_CTX* ctx, SslProtocols protocols);

/*
Shims the SSL_new method.

Returns the new SSL instance.
*/
DLLEXPORT SSL* CryptoNative_SslCreate(SSL_CTX* ctx);

/*
Shims the SSL_get_error method.

Returns the error code for the specified result.
*/
DLLEXPORT int32_t CryptoNative_SslGetError(SSL* ssl, int32_t ret);

/*
Cleans up and deletes an SSL instance.

Implemented by calling SSL_free.

No-op if ssl is null.
The given X509 SSL is invalid after this call.
Always succeeds.
*/
DLLEXPORT void CryptoNative_SslDestroy(SSL* ssl);

/*
Cleans up and deletes an SSL_CTX instance.

Implemented by calling SSL_CTX_free.

No-op if ctx is null.
The given X509 SSL_CTX is invalid after this call.
Always succeeds.
*/
DLLEXPORT void CryptoNative_SslCtxDestroy(SSL_CTX* ctx);

/*
Shims the SSL_set_connect_state method.
*/
DLLEXPORT void CryptoNative_SslSetConnectState(SSL* ssl);

/*
Shims the SSL_set_accept_state method.
*/
DLLEXPORT void CryptoNative_SslSetAcceptState(SSL* ssl);

/*
Shims the SSL_get_version method.

Returns the protocol version string for the SSL instance.
*/
DLLEXPORT const char* CryptoNative_SslGetVersion(SSL* ssl);

/*
Returns the connection information for the SSL instance.

Returns 1 upon success, otherwise 0.
*/

DLLEXPORT int32_t CryptoNative_GetSslConnectionInfo(SSL* ssl,
                                                     CipherAlgorithmType* dataCipherAlg,
                                                     ExchangeAlgorithmType* keyExchangeAlg,
                                                     HashAlgorithmType* dataHashAlg,
                                                     int32_t* dataKeySize,
                                                     DataHashSize* hashKeySize);

/*
Shims the SSL_write method.

Returns the positive number of bytes written when successful, 0 or a negative number
when an error is encountered.
*/
DLLEXPORT int32_t CryptoNative_SslWrite(SSL* ssl, const void* buf, int32_t num);

/*
Shims the SSL_read method.

Returns the positive number of bytes read when successful, 0 or a negative number
when an error is encountered.
*/
DLLEXPORT int32_t CryptoNative_SslRead(SSL* ssl, void* buf, int32_t num);

/*
Shims the SSL_renegotiate_pending method.

Returns 1 when negotiation is requested; 0 once a handshake has finished.
*/
DLLEXPORT int32_t CryptoNative_IsSslRenegotiatePending(SSL* ssl);

/*
Shims the SSL_shutdown method.

Returns:
1 if the shutdown was successfully completed;
0 if the shutdown is not yet finished;
<0 if the shutdown was not successful because a fatal error.
*/
DLLEXPORT int32_t CryptoNative_SslShutdown(SSL* ssl);

/*
Shims the SSL_set_bio method.
*/
DLLEXPORT void CryptoNative_SslSetBio(SSL* ssl, BIO* rbio, BIO* wbio);

/*
Shims the SSL_do_handshake method.

Returns:
1 if the handshake was successful;
0 if the handshake was not successful but was shut down controlled
and by the specifications of the TLS/SSL protocol;
<0 if the handshake was not successful because of a fatal error.
*/
DLLEXPORT int32_t CryptoNative_SslDoHandshake(SSL* ssl);

/*
Gets a value indicating whether the SSL_state is SSL_ST_OK.

Returns 1 if the state is OK, otherwise 0.
*/
DLLEXPORT int32_t CryptoNative_IsSslStateOK(SSL* ssl);

/*
Shims the SSL_get_peer_certificate method.

Returns the certificate presented by the peer.
*/
DLLEXPORT X509* CryptoNative_SslGetPeerCertificate(SSL* ssl);

/*
Shims the SSL_get_peer_cert_chain method.

Returns the certificate chain presented by the peer.
*/
DLLEXPORT X509Stack* CryptoNative_SslGetPeerCertChain(SSL* ssl);

/*
Shims the SSL_CTX_use_certificate method.

Returns 1 upon success, otherwise 0.
*/
DLLEXPORT int32_t CryptoNative_SslCtxUseCertificate(SSL_CTX* ctx, X509* x);

/*
Shims the SSL_CTX_use_PrivateKey method.

Returns 1 upon success, otherwise 0.
*/
DLLEXPORT int32_t CryptoNative_SslCtxUsePrivateKey(SSL_CTX* ctx, EVP_PKEY* pkey);

/*
Shims the SSL_CTX_check_private_key method.

Returns 1 upon success, otherwise 0.
*/
DLLEXPORT int32_t CryptoNative_SslCtxCheckPrivateKey(SSL_CTX* ctx);

/*
Shims the SSL_CTX_set_quiet_shutdown method.
*/
DLLEXPORT void CryptoNative_SslCtxSetQuietShutdown(SSL_CTX* ctx);

/*
Shims the SSL_set_quiet_shutdown method.
*/
DLLEXPORT void CryptoNative_SslSetQuietShutdown(SSL* ctx, int mode);

/*
Shims the SSL_get_client_CA_list method.

Returns the list of CA names explicity set.
*/
DLLEXPORT X509NameStack* CryptoNative_SslGetClientCAList(SSL* ssl);

/*
Shims the SSL_CTX_set_verify method.
*/
DLLEXPORT void CryptoNative_SslCtxSetVerify(SSL_CTX* ctx, SslCtxSetVerifyCallback callback);

/*
Shims the SSL_CTX_set_cert_verify_callback method.
*/
DLLEXPORT void
CryptoNative_SslCtxSetCertVerifyCallback(SSL_CTX* ctx, SslCtxSetCertVerifyCallbackCallback callback, void* arg);

/*
Sets the specified encryption policy on the SSL_CTX.
Returns 1 if any cipher could be selected, and 0 if none were available.
*/
DLLEXPORT int32_t CryptoNative_SetEncryptionPolicy(SSL_CTX* ctx, EncryptionPolicy policy);

/*
Shims the SSL_CTX_set_client_cert_cb method
*/
DLLEXPORT void CryptoNative_SslCtxSetClientCertCallback(SSL_CTX* ctx, SslClientCertCallback callback);

/*
Shims the SSL_get_finished method.
*/
DLLEXPORT int32_t CryptoNative_SslGetFinished(SSL* ssl, void* buf, int32_t count);

/*
Shims the SSL_get_peer_finished method.
*/
DLLEXPORT int32_t CryptoNative_SslGetPeerFinished(SSL* ssl, void* buf, int32_t count);

/*
Returns true/false based on if existing ssl session was re-used or not.
Shims the SSL_session_reused macro.
*/
DLLEXPORT int32_t CryptoNative_SslSessionReused(SSL* ssl);

/*
adds the given certificate to the extra chain certificates associated with ctx that is associated with the ssl.

libssl frees the x509 object.
Returns 1 if success and 0 in case of failure
*/
DLLEXPORT int32_t CryptoNative_SslAddExtraChainCert(SSL* ssl, X509* x509);

/*
Shims the ssl_ctx_set_alpn_select_cb method.
*/
DLLEXPORT void CryptoNative_SslCtxSetAlpnSelectCb(SSL_CTX* ctx, SslCtxSetAlpnCallback cb, void *arg);

/*
Shims the ssl_ctx_set_alpn_protos method.
Returns 0 on success, non-zero on failure.
*/
DLLEXPORT int32_t CryptoNative_SslCtxSetAlpnProtos(SSL_CTX* ctx, const uint8_t* protos, uint32_t protos_len);

/*
Shims the ssl_get0_alpn_selected method.
*/
DLLEXPORT void CryptoNative_SslGet0AlpnSelected(SSL* ssl, const uint8_t** protocol, uint32_t* len);

/*
Shims the SSL_set_tlsext_host_name method.
*/
DLLEXPORT int32_t CryptoNative_SslSetTlsExtHostName(SSL* ssl, uint8_t* name);

