// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

#include "pal_types.h"

#include <openssl/evp.h>

/*
Shims the EVP_PKEY_get1_RSA method.

Returns the RSA instance for the EVP_PKEY.
*/
extern "C" RSA* CryptoNative_EvpPkeyGetRsa(EVP_PKEY* pkey);

/*
Shims the EVP_PKEY_set1_RSA method to set the RSA
instance on the EVP_KEY.

Returns 1 upon success, otherwise 0.
*/
extern "C" int32_t CryptoNative_EvpPkeySetRsa(EVP_PKEY* pkey, RSA* rsa);
