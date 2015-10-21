// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

#include "pal_ssl_client_cert.h"

extern "C" int32_t AddExtraChainCertToSslCtx(SSL_CTX* ssl_ctx, X509* x509)
{
    if (!ssl_ctx || !x509)
    {
        return 0;
    }

    if (SSL_CTX_add_extra_chain_cert(ssl_ctx, x509) == 1)
    {
        return 1;
    }

    return 0;
}

extern "C" void SetSslContextClientCertCallback(SSL_CTX* ssl_ctx,
                                                int (*client_cert_cb)(SSL* ssl, X509** x509, EVP_PKEY** evp_pkey))
{
    if (ssl_ctx != nullptr)
    {
        SSL_CTX_set_client_cert_cb(ssl_ctx, client_cert_cb);
    }
}

extern "C" SSL_CTX* GetSslContextFromSsl(const SSL* ssl)
{
    return SSL_get_SSL_CTX(ssl);
}
