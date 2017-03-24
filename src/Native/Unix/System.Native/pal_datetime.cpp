// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#include <stdlib.h>
#include <stdint.h>
#include <sys/time.h>

static const int64_t SECS_TO_100NS = 10000000; /* 10^7 */
static const int64_t MICROSECONDS_TO_100NS = 10; /* 1000 / 100 */

//
// SystemNative_GetSystemTimeAsTicks return the system time as ticks (100 nanoseconds) 
// since 00:00 01 January 1970 UTC (Unix epoch) 
//
extern "C" int64_t SystemNative_GetSystemTimeAsTicks()
{
    struct timeval time;

    if (gettimeofday(&time, NULL) != 0)
    {
        // in failure we return 00:00 01 January 1970 UTC (Unix epoch)
        return 0;
    }
    
    return ((int64_t) time.tv_sec) * SECS_TO_100NS + (time.tv_usec * MICROSECONDS_TO_100NS); 
}
