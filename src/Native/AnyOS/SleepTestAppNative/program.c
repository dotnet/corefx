// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#include <stdio.h>
#include <stdlib.h>
#include <inttypes.h>

#ifdef _WIN32
#include <windows.h>
#define SLEEP_IN_SEC(seconds) Sleep(seconds * 1000)
#define INPUT_LIMIT 4294967
#else
#include <unistd.h>
#define SLEEP_IN_SEC sleep
#define INPUT_LIMIT 4294967295
#endif

#define ENSURE(condition, ...)        \
    if (!(condition))                 \
    {                                 \
        fprintf(stderr, __VA_ARGS__); \
        return 1;                     \
    }                                 \

#ifdef __GNUC__
// gcc doesn't like cdecl attribute and throws:
//   error: ‘cdecl’ attribute ignored [-Werror=attributes]
int main(int argc, char *argv[])
#else
// __cdecl is primarily required by msvc-x86, but other architectures
// with msvc and clang are fine with it.
int __cdecl main(int argc, char *argv[])
#endif
{
    ENSURE(argc == 2, "Usage: %s SECONDS\n", argv[0])

    char *endptr;
    unsigned int seconds = (unsigned int)strtoumax(argv[1], &endptr, 10);

    ENSURE(*endptr == '\0', "Integer value expected.\n")
    ENSURE(seconds <= INPUT_LIMIT, "Seconds value is expected to be in the range of [0..%ld].\n", (long)INPUT_LIMIT)

    SLEEP_IN_SEC(seconds);

    return 0;
}
