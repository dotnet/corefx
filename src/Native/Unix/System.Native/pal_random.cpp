// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#include <stdlib.h>
#include <stdint.h>
#include <sys/stat.h>
#include <fcntl.h>
#include <assert.h>
#include <unistd.h>
#include <time.h>
#include <errno.h>

/*

Generate random bytes. The generated bytes are not cryptographically strong.

*/
extern "C" void SystemNative_GetNonCryptographicallySecureRandomBytes(uint8_t* buffer, int32_t bufferLength)
{
    assert(buffer != NULL);

    static volatile int rand_des = -1;
    long num = 0;
    static bool sMissingDevURandom;
    static bool sInitializedMRand;

    if (!sMissingDevURandom)
    {
        if (rand_des == -1)
        {
            int fd = open("/dev/urandom", O_RDONLY, O_CLOEXEC);
            if (fd != -1)
            {
                if (!__sync_bool_compare_and_swap(&rand_des, -1, fd))
                {
                    // Another thread has already set the rand_des
                    close(fd);
                }
            }
            else if (errno == ENOENT)
            {                
                sMissingDevURandom = true;
            }
        }

        if (rand_des != -1)
        {
            int32_t offset = 0;
            do
            {
                ssize_t n = read(rand_des, buffer + offset , (size_t)(bufferLength - offset));
                if (n == -1)
                {
                    if (errno == EINTR)
                    {
                        continue;
                    }

                    break;
                }

                offset += n;
            }
            while (offset != bufferLength);

            assert(offset == bufferLength);
        }
    }

    if (!sInitializedMRand)
    {
        srand48(time(NULL));
        sInitializedMRand = true;
    }

    // always xor srand48 over the whole buffer to get some randomness
    // in case /dev/urandom is not really random

    for (int i = 0; i < bufferLength; i++)
    {
        if (i % 4 == 0)
        {
            num = lrand48();
        }

        *(buffer + i) ^= num;
        num >>= 8;
    }
}
