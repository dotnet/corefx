// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#include "pal_bio.h"

#include <assert.h>

extern "C" BIO* CryptoNative_CreateMemoryBio()
{
    return BIO_new(BIO_s_mem());
}

extern "C" BIO* CryptoNative_BioNewFile(const char* filename, const char* mode)
{
    return BIO_new_file(filename, mode);
}

extern "C" int32_t CryptoNative_BioDestroy(BIO* a)
{
    int32_t r = BIO_free(a);
    if (r == 0)
    {
        ERR_clear_error();
    }

    return r;
}

extern "C" int32_t CryptoNative_BioGets(BIO* b, char* buf, int32_t size)
{
    return BIO_gets(b, buf, size);
}

extern "C" int32_t CryptoNative_BioRead(BIO* b, void* buf, int32_t len)
{
    return BIO_read(b, buf, len);
}

extern "C" int32_t CryptoNative_BioWrite(BIO* b, const void* buf, int32_t len)
{
    return BIO_write(b, buf, len);
}

extern "C" int32_t CryptoNative_GetMemoryBioSize(BIO* bio)
{
    long ret = BIO_get_mem_data(bio, nullptr);

    // BIO_get_mem_data returns the memory size, which will always be
    // an int32.
    assert(ret <= INT32_MAX);
    return static_cast<int32_t>(ret);
}

extern "C" int32_t CryptoNative_BioCtrlPending(BIO* bio)
{
    size_t result = BIO_ctrl_pending(bio);
    assert(result <= INT32_MAX);
    return static_cast<int32_t>(result);
}
