// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#pragma once

// These defines are temporary until all files have been migrated from C++ to C
#ifdef __cplusplus
#define BEGIN_EXTERN_C extern "C" {
#define END_EXTERN_C   }
#else
#define BEGIN_EXTERN_C
#define END_EXTERN_C
#endif
