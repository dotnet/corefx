// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#pragma once

#include "pal_types.h"

extern "C" int32_t SystemNative_GetUnixVersion(char* version, int* capacity);

extern "C" int32_t SystemNative_GetUnixArchitecture();

enum 
{
    ARCH_X86,
    ARCH_X64,
    ARCH_ARM
};
