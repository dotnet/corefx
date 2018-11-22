// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#pragma once

#include "pal_compiler.h"
#include "pal_types.h"

typedef struct TimeSpec
{
    int64_t tv_sec; // seconds
    int64_t tv_nsec; // nanoseconds
} TimeSpec;

/**
 * Sets the last access and last modified time of a file
 *
 * Returns 0 on success; otherwise, returns -1 and errno is set.
 */
DLLEXPORT int32_t SystemNative_UTimensat(const char* path, TimeSpec* times);

/**
 * Gets the resolution of the timestamp, in counts per second.
 *
 * Returns 1 on success; otherwise, 0 on failure.
 */
DLLEXPORT int32_t SystemNative_GetTimestampResolution(uint64_t* resolution);

/**
 * Gets a high-resolution timestamp that can be used for time-interval measurements.
 *
 * Returns 1 on success; otherwise, 0 on failure.
 */
DLLEXPORT int32_t SystemNative_GetTimestamp(uint64_t* timestamp);

DLLEXPORT int32_t SystemNative_GetAbsoluteTime(uint64_t* timestamp);

DLLEXPORT int32_t SystemNative_GetTimebaseInfo(uint32_t* numer, uint32_t* denom);
