// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#include "pal_crypto_types.h"

#include <openssl/pkcs7.h>

/*
Reads a PKCS7 instance in PEM format from a BIO.

Direct shim to PEM_read_bio_PKCS7.

Returns the new PKCS7 instance.
*/
extern "C" PKCS7* CryptoNative_PemReadBioPkcs7(BIO* bp);

/*
Shims the d2i_PKCS7 method and makes it easier to invoke from managed code.
*/
extern "C" PKCS7* CryptoNative_DecodePkcs7(const uint8_t* buf, int32_t len);

/*
Reads a PKCS7 instance in DER format from a BIO.

Direct shim to d2i_PKCS7_bio.

Returns the new PKCS7 instance.
*/
extern "C" PKCS7* CryptoNative_D2IPkcs7Bio(BIO* bp);

/*
Create a new PKCS7 instance and prepare it to be a signed PKCS7
with a data payload.

Returns the new PKCS7 instance.
*/
extern "C" PKCS7* CryptoNative_Pkcs7CreateSigned();

/*
Cleans up and deletes a PKCS7 instance.

Implemented by calling PKCS7_free.

No-op if p7 is null.
The given PKCS7 pointer is invalid after this call.
Always succeeds.
*/
extern "C" void CryptoNative_Pkcs7Destroy(PKCS7* p7);

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
extern "C" int32_t CryptoNative_GetPkcs7Certificates(PKCS7* p7, X509Stack** certs);

/*
Shims the PKCS7_add_certificate function and makes it easier to invoke from managed code.
*/
extern "C" int32_t CryptoNative_Pkcs7AddCertificate(PKCS7* p7, X509* x509);

/*
Returns the number of bytes it will take to convert
the PKCS7 to a DER format.
*/
extern "C" int32_t CryptoNative_GetPkcs7DerSize(PKCS7* p7);

/*
Shims the i2d_PKCS7 method.

Returns the number of bytes written to buf.
*/
extern "C" int32_t CryptoNative_EncodePkcs7(PKCS7* p7, uint8_t* buf);
