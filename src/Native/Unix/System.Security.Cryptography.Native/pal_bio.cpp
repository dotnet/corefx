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
    return BIO_free(a);
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

extern "C" void CryptoNative_BioSetAppData(BIO* bio, void* data)
{
    BIO_set_ex_data(bio, 0, data);
}

extern "C" void* CryptoNative_BioGetAppData(BIO* bio)
{
    return BIO_get_ex_data(bio, 0);
}

/*
Set write flag for the custom bio
*/
extern "C" void CryptoNative_BioSetWriteFlag(BIO* bio)
{
    BIO_set_flags(bio, BIO_FLAGS_WRITE);
}

/*
Set the read and should retry flag for the custom bio
*/
extern "C" void CryptoNative_BioSetShoudRetryReadFlag(BIO* bio)
{
    BIO_set_flags(bio, BIO_FLAGS_SHOULD_RETRY | BIO_FLAGS_READ);
}

static long ControlCallback(BIO* bio, int cmd, long param, void* ptr)
{
    (void)bio, (void)param, (void)ptr; // deliberately unused parameters
    switch (cmd)
    {
        case BIO_CTRL_FLUSH:
        case BIO_CTRL_POP:
        case BIO_CTRL_PUSH:
            return 1;
    }
    return 0;
}

static int DestroyCallback(BIO* bio)
{
    (void)bio; // deliberately unused parameter
    return -1;
}

static int CreateCallback(BIO* bio)
{
    bio->init = 1;
    return 1;
}

static BIO_METHOD sslStreamCustomBio = {
    BIO_TYPE_SOURCE_SINK,
    "SslStreamCustomBio",
    nullptr,
    nullptr,
    nullptr,
    nullptr,
    ControlCallback,
    CreateCallback,
    DestroyCallback,
    nullptr,
};

extern "C" void CryptoNative_InitCustomBioMethod(BWriteCallback bwrite, BReadCallback bread)
{
    sslStreamCustomBio.bwrite = bwrite;
    sslStreamCustomBio.bread = bread;
}

extern "C" BIO* CryptoNative_CreateCustomBio()
{
    return BIO_new(&sslStreamCustomBio);
}
