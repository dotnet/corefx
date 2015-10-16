// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

#include "pal_crypto_types.h"

#include <openssl/pkcs12.h>

/*
Shims the d2i_PKCS12 method and makes it easier to invoke from managed code.
*/
extern "C" PKCS12* DecodePkcs12(const unsigned char* buf, int32_t len);

/*
Shims the d2i_PKCS12_bio method.

Returns the new PKCS12 instance.
*/
extern "C" PKCS12* DecodePkcs12FromBio(BIO* bio);

/*
Cleans up and deletes a PKCS12 instance.

Implemented by calling PKCS12_free.

No-op if p12 is null.
The given PKCS12 pointer is invalid after this call.
Always succeeds.
*/
extern "C" void Pkcs12Destroy(PKCS12* p12);

/*
Shims the PKCS12_create method.

Returns the new PKCS12 instance.
*/
extern "C" PKCS12* Pkcs12Create(char* pass, EVP_PKEY* pkey, X509* cert, X509Stack* ca);

/*
Returns the number of bytes it will take to convert
the PKCS12 to a DER format.
*/
extern "C" int32_t GetPkcs12DerSize(PKCS12* p12);

/*
Shims the i2d_PKCS12 method.

Returns the number of bytes written to buf.
*/
extern "C" int32_t EncodePkcs12(PKCS12* p12, unsigned char* buf);

/*
Shims the PKCS12_parse method.

Returns 1 on success, otherwise 0.
*/
extern "C" int32_t Pkcs12Parse(PKCS12* p12, const char* pass, EVP_PKEY** pkey, X509** cert, X509Stack** ca);
