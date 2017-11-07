// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#include <dlfcn.h>

extern "C" void* SystemNative_DlOpen(const char *file, int mode)
{
    return dlopen(file, mode);
}

extern "C" void* SystemNative_DlSym(void *handle, const char *name)
{
    return dlsym(handle, name);
}
