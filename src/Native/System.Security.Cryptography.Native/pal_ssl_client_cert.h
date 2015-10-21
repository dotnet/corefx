// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

#include "pal_types.h"

#include <openssl/ssl.h>

/*
Function:
GetSslContextFromSsl

returns the pointer to ssl_ctx from which the given ssl pointer was created
*/

extern "C" SSL_CTX* GetSslContextFromSsl(const SSL* ssl);

/*
Function:
SetSslContextClientCertCallback

sets the given delegate as the callback for handling the client certificate during TLS negotiations
*/
extern "C" void SetSslContextClientCertCallback(SSL_CTX* ssl_ctx,
                                                int (*client_cert_cb)(SSL* ssl, X509** x509, EVP_PKEY** evp_pkey));

/*
Function:
AddExtraCertToChain

adds certificate to the extra chain certificates associated with ssl_ctx

libssl frees the x509 object.
Returns 1 if success and 0 in case of failure
*/
extern "C" int32_t AddExtraChainCertToSslCtx(SSL_CTX* ssl, X509* x509);
