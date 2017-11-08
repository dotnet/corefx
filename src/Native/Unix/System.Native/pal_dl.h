// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#pragma once

// Values for the mode flag in dlopen
enum
{
    PAL_RTLD_LAZY = 1,
    PAL_RTLD_NOW = 2
};
extern "C" void* SystemNative_DlOpen(const char *file, int mode);

extern "C" void* SystemNative_DlSym(void *handle, const char *name);
