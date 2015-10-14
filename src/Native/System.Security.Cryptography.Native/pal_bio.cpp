// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

#include "pal_bio.h"

#include <assert.h>

extern "C" BIO* CreateMemoryBio()
{
    return BIO_new(BIO_s_mem());
}

extern "C" BIO* BioNewFile(const char* filename, const char* mode)
{
    return BIO_new_file(filename, mode);
}

extern "C" int32_t BioDestroy(BIO* a)
{
    return BIO_free(a);
}

extern "C" int32_t BioGets(BIO* b, char* buf, int32_t size)
{
    return BIO_gets(b, buf, size);
}

extern "C" int32_t BioRead(BIO* b, void* buf, int32_t len)
{
    return BIO_read(b, buf, len);
}

extern "C" int32_t BioWrite(BIO* b, const void* buf, int32_t len)
{
    return BIO_write(b, buf, len);
}

extern "C" int32_t GetMemoryBioSize(BIO* bio)
{
    long ret = BIO_get_mem_data(bio, nullptr);

    // BIO_get_mem_data returns the memory size, which will always be
    // an int32.
    assert(ret <= INT32_MAX);
    return static_cast<int32_t>(ret);
}
