// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#pragma once

#include "pal_types.h"
#include "pal_compiler.h"

// Unless another interpretation is "obvious", pal_seckey functions return 1 on success.
// functions which represent a boolean return 0 on "successful false"
// otherwise functions will return one of the following return values:
enum
{
    PAL_Error_False = 0,
    PAL_Error_True = 1,

    PAL_Error_BadInput = -1,
    PAL_Error_SeeError = -2,
    PAL_Error_SeeStatus = -3,
    PAL_Error_Platform = -4,
    PAL_Error_UnknownState = -5,
    PAL_Error_UnknownAlgorithm = -6,

    PAL_Error_UserTrust = -7,
    PAL_Error_AdminTrust = -8,
    PAL_Error_OutItemsNull = -9,
    PAL_Error_OutItemsEmpty = -10
};
typedef int32_t PAL_Error;
