// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#include "pal_keyagree.h"
#include "pal_error.h"

int32_t
AppleCryptoNative_EcdhKeyAgree(SecKeyRef privateKey, SecKeyRef publicKey, CFDataRef* pAgreeOut, CFErrorRef* pErrorOut)
{
    if (pAgreeOut != NULL)
        *pAgreeOut = NULL;
    if (pErrorOut != NULL)
        *pErrorOut = NULL;

    if (privateKey == NULL || publicKey == NULL)
        return PAL_Error_BadInput;

    CFDictionaryRef dict = NULL;

    *pAgreeOut =
        SecKeyCopyKeyExchangeResult(privateKey, kSecKeyAlgorithmECDHKeyExchangeStandard, publicKey, dict, pErrorOut);

    if (*pErrorOut != NULL)
        return PAL_Error_SeeError;

    return *pAgreeOut != NULL;
}
