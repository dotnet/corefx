// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

#include "pal_types.h"

#include <openssl/bio.h>

/*
Creates a new memory-backed BIO instance.
*/
extern "C" BIO* CryptoNative_CreateMemoryBio();

/*
Direct shim to BIO_new_file.
*/
extern "C" BIO* CryptoNative_BioNewFile(const char* filename, const char* mode);

/*
Cleans up and deletes a BIO instance.

Implemented by:
1) Calling BIO_free

No-op if a is null.
The given BIO pointer is invalid after this call.
*/
extern "C" int32_t CryptoNative_BioDestroy(BIO* a);

/*
Direct shim to BIO_gets.
*/
extern "C" int32_t CryptoNative_BioGets(BIO* b, char* buf, int32_t size);

/*
Direct shim to BIO_read.
*/
extern "C" int32_t CryptoNative_BioRead(BIO* b, void* buf, int32_t len);

/*
Direct shim to BIO_write.
*/
extern "C" int32_t CryptoNative_BioWrite(BIO* b, const void* buf, int32_t len);

/*
Gets the size of data available in the BIO.

Shims the BIO_get_mem_data method.
*/
extern "C" int32_t CryptoNative_GetMemoryBioSize(BIO* bio);

/*
Shims the BIO_ctrl_pending method.

Returns the number of pending characters in the BIOs read and write buffers.
*/
extern "C" int32_t CryptoNative_BioCtrlPending(BIO* bio);
