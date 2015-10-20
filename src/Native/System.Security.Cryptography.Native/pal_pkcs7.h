// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

#include "pal_crypto_types.h"

#include <openssl/pkcs7.h>

/*
Reads a PKCS7 instance in PEM format from a BIO.

Direct shim to PEM_read_bio_PKCS7.

Returns the new PKCS7 instance.
*/
extern "C" PKCS7* PemReadBioPkcs7(BIO* bp);

/*
Shims the d2i_PKCS7 method and makes it easier to invoke from managed code.
*/
extern "C" PKCS7* DecodePkcs7(const unsigned char* buf, int32_t len);

/*
Reads a PKCS7 instance in DER format from a BIO.

Direct shim to d2i_PKCS7_bio.

Returns the new PKCS7 instance.
*/
extern "C" PKCS7* D2IPkcs7Bio(BIO* bp);

/*
Cleans up and deletes a PKCS7 instance.

Implemented by calling PKCS7_free.

No-op if p7 is null.
The given PKCS7 pointer is invalid after this call.
Always succeeds.
*/
extern "C" void Pkcs7Destroy(PKCS7* p7);

/*
Function:
GetPkcs7Certificates

Used by System.Security.Cryptography.X509Certificates' CertificatePal when
reading the contents of a PKCS#7 file or blob.

Return values:
0 on NULL inputs, or a PKCS#7 file whose layout is not understood
1 when the file format is understood, and *certs is assigned to the
certificate contents of the structure.
*/
extern "C" int32_t GetPkcs7Certificates(PKCS7* p7, X509Stack** certs);
