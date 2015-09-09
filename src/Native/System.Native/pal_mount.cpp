// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

#include "../config.h"
#include "pal_mount.h"
#include "pal_utilities.h"
#include <assert.h>
#include <string.h>
#include <errno.h>

// Check if we should use getmntinfo or /proc/mounts
#if HAVE_MNTINFO
#include <sys/mount.h>
#else
#include <sys/statfs.h>
#include <mntent.h>
#define STRING_BUFFER_SIZE 8192
#endif

#if HAVE_MNTINFO

int GetMountInfo(MountPointFound onFound)
{
    // getmntinfo returns pointers to OS-internal structs, so we don't need to worry about free'ing the object
    struct statfs* mounts = nullptr;
    int count = getmntinfo(&mounts, 0);
    for (int32_t i = 0; i < count; i++)
    {
        onFound(mounts[i].f_mntonname);
    }

    return 0;
}

#else

int GetMountInfo(MountPointFound onFound)
{
    int result = -1;
    FILE* fp = setmntent("/proc/mounts", MNTOPT_RO);
    if (fp != nullptr)
    {
        // The _r version of getmntent needs all buffers to be passed in; however, we don't know how big of a string
        // buffer we will need, so pick something that seems like it will be big enough.
        char buffer[STRING_BUFFER_SIZE] = { };
        mntent entry;
        while (getmntent_r(fp, &entry, buffer, STRING_BUFFER_SIZE) != nullptr)
        {
            onFound(entry.mnt_dir);
        }

        result = endmntent(fp);
        assert(result == 1); // documented to always return 1
        result = 0; // We need to standardize a success return code between our implementations, so settle on 0 for success
    }

    return result;
}

#endif

extern "C"
int GetAllMountPoints(MountPointFound onFound)
{
    return GetMountInfo(onFound);
}

extern "C"
int GetSpaceInfoForMountPoint(
    const char*             name,
    MountPointInformation*  mpi)
{
    assert(name != nullptr);
    assert(mpi != nullptr);

    struct statfs stats = { };
    int result = statfs(name, &stats);
    if (result == 0)
    {
        mpi->AvailableFreeSpace = stats.f_bsize * stats.f_bavail;
        mpi->TotalFreeSpace = stats.f_bsize * stats.f_bfree;
        mpi->TotalSize = stats.f_bsize * stats.f_blocks;
    }
    else
    {
        *mpi = { };
    }

    return result;
}

extern "C"
int GetFormatInfoForMountPoint(
    const char* name, 
    char*       formatNameBuffer, 
    int32_t     bufferLength, 
    int64_t*    formatType)
{
    assert((formatNameBuffer != nullptr) && (formatType != nullptr));
    assert(bufferLength > 0);

    struct statfs stats;
    int result = statfs(name, &stats);
    if (result == 0)
    {

#if HAVE_STATFS_FSTYPENAME
        if (bufferLength < MFSNAMELEN)
        {
            result = ERANGE;
            *formatType = 0;
        }
        else
        {
            SafeStringCopy(formatNameBuffer, bufferLength, stats.f_fstypename);
            *formatType = -1;
        }
#else
        assert(formatType != nullptr);
        *formatType = stats.f_type;
        *formatNameBuffer = '\0';
#endif

    }
    else
    {
        *formatType = 0;
    }

    return result;
}
