// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#include "pal_bio.h"

#include <assert.h>

typedef struct ReadWriteMethodStruct
{
    BioWriteCallback write;
    BioReadCallback read;
} ReadWriteMethodStruct;

static ReadWriteMethodStruct managedMethods = {nullptr, nullptr};

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

static int WriteCallback(BIO* b, const char* buf, int32_t len)
{
    void* ptr = BIO_get_ex_data(b, 0);
    if (ptr == nullptr)
    {
        return -1;
    }
    return managedMethods.write(b, buf, len, ptr);
}

static int ReadCallback(BIO* b, char* buf, int32_t len)
{
    void* ptr = BIO_get_ex_data(b, 0);
    if (ptr == nullptr)
    {
        return -1;
    }
    return managedMethods.read(b, buf, len, ptr);
}

static BIO_METHOD managedSslBio = {
    BIO_TYPE_SOURCE_SINK,
    "Managed Ssl Bio",
    WriteCallback,
    ReadCallback,
    nullptr,
    nullptr,
    ControlCallback,
    CreateCallback,
    DestroyCallback,
    nullptr,
};

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

extern "C" void CryptoNative_BioSetWriteFlag(BIO* bio)
{
    BIO_set_flags(bio, BIO_FLAGS_WRITE);
}

extern "C" void CryptoNative_BioSetShoudRetryReadFlag(BIO* bio)
{
    BIO_set_flags(bio, BIO_FLAGS_SHOULD_RETRY | BIO_FLAGS_READ);
}

extern "C" void CryptoNative_InitManagedSslBioMethod(BioWriteCallback bwrite, BioReadCallback bread)
{
    managedMethods.write = bwrite;
    managedMethods.read = bread;
}

extern "C" BIO* CryptoNative_CreateManagedSslBio()
{
    return BIO_new(&managedSslBio);
}
