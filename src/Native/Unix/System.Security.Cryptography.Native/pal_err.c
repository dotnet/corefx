// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#include "pal_err.h"
#include "pal_utilities.h"

void CryptoNative_ErrClearError()
{
    ERR_clear_error();
}

uint64_t CryptoNative_ErrGetError()
{
    return ERR_get_error();
}

uint64_t CryptoNative_ErrGetErrorAlloc(int32_t* isAllocFailure)
{
    unsigned long err = ERR_get_error();

    if (isAllocFailure)
    {
        *isAllocFailure = ERR_GET_REASON(err) == ERR_R_MALLOC_FAILURE;
    }

    return err;
}

uint64_t CryptoNative_ErrPeekError()
{
    return ERR_peek_error();
}

uint64_t CryptoNative_ErrPeekLastError()
{
    return ERR_peek_last_error();
}

const char* CryptoNative_ErrReasonErrorString(uint64_t error)
{
#ifdef OPENSSL_IS_BORINGSSL
    return ERR_reason_error_string((uint32_t)error);
#else
    return ERR_reason_error_string((unsigned long)error);
#endif
}

void CryptoNative_ErrErrorStringN(uint64_t e, char* buf, int32_t len)
{
#ifdef OPENSSL_IS_BORINGSSL
    ERR_error_string_n((uint32_t)e, buf, Int32ToSizeT(len));
#else
    ERR_error_string_n((unsigned long)e, buf, Int32ToSizeT(len));
#endif
}
