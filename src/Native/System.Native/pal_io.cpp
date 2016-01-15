// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

#include "pal_config.h"
#include "pal_errno.h"
#include "pal_io.h"
#include "pal_utilities.h"

#include <assert.h>
#include <errno.h>
#include <fcntl.h>
#include <fnmatch.h>
#include <poll.h>
#include <stdio.h>
#include <stdlib.h>
#include <sys/mman.h>
#include <sys/stat.h>
#include <sys/types.h>
#include <sys/file.h>
#include <sys/ioctl.h>
#include <syslog.h>
#include <termios.h>
#include <unistd.h>

#if HAVE_INOTIFY
#include <sys/inotify.h>
#endif

#if HAVE_STAT64
#define stat_ stat64
#define fstat_ fstat64
#define lstat_ lstat64
#else
#define stat_ stat
#define fstat_ fstat
#define lstat_ lstat
#endif

// These numeric values are specified by POSIX.
// Validate that our definitions match.
static_assert(PAL_S_IRWXU == S_IRWXU, "");
static_assert(PAL_S_IRUSR == S_IRUSR, "");
static_assert(PAL_S_IWUSR == S_IWUSR, "");
static_assert(PAL_S_IXUSR == S_IXUSR, "");
static_assert(PAL_S_IRWXG == S_IRWXG, "");
static_assert(PAL_S_IRGRP == S_IRGRP, "");
static_assert(PAL_S_IWGRP == S_IWGRP, "");
static_assert(PAL_S_IXGRP == S_IXGRP, "");
static_assert(PAL_S_IRWXO == S_IRWXO, "");
static_assert(PAL_S_IROTH == S_IROTH, "");
static_assert(PAL_S_IWOTH == S_IWOTH, "");
static_assert(PAL_S_IXOTH == S_IXOTH, "");
static_assert(PAL_S_ISUID == S_ISUID, "");
static_assert(PAL_S_ISGID == S_ISGID, "");

// These numeric values are not specified by POSIX, but the values
// are common to our current targets.  If these static asserts fail,
// ConvertFileStatus needs to be updated to twiddle mode bits
// accordingly.
static_assert(PAL_S_IFMT == S_IFMT, "");
static_assert(PAL_S_IFIFO == S_IFIFO, "");
static_assert(PAL_S_IFCHR == S_IFCHR, "");
static_assert(PAL_S_IFDIR == S_IFDIR, "");
static_assert(PAL_S_IFREG == S_IFREG, "");
static_assert(PAL_S_IFLNK == S_IFLNK, "");

// Validate that our enum for inode types is the same as what is
// declared by the dirent.h header on the local system.
static_assert(PAL_DT_UNKNOWN == DT_UNKNOWN, "");
static_assert(PAL_DT_FIFO == DT_FIFO, "");
static_assert(PAL_DT_CHR == DT_CHR, "");
static_assert(PAL_DT_DIR == DT_DIR, "");
static_assert(PAL_DT_BLK == DT_BLK, "");
static_assert(PAL_DT_REG == DT_REG, "");
static_assert(PAL_DT_LNK == DT_LNK, "");
static_assert(PAL_DT_SOCK == DT_SOCK, "");
static_assert(PAL_DT_WHT == DT_WHT, "");

// Validate that our Lock enum value are correct for the platform
static_assert(PAL_LOCK_SH == LOCK_SH, "");
static_assert(PAL_LOCK_EX == LOCK_EX, "");
static_assert(PAL_LOCK_NB == LOCK_NB, "");
static_assert(PAL_LOCK_UN == LOCK_UN, "");

// Validate our AccessMode enum values are correct for the platform
static_assert(PAL_F_OK == F_OK, "");
static_assert(PAL_X_OK == X_OK, "");
static_assert(PAL_W_OK == W_OK, "");
static_assert(PAL_R_OK == R_OK, "");

// Validate our SeekWhence enum values are correct for the platform
static_assert(PAL_SEEK_SET == SEEK_SET, "");
static_assert(PAL_SEEK_CUR == SEEK_CUR, "");
static_assert(PAL_SEEK_END == SEEK_END, "");

// Validate our PollFlags enum values are correct for the platform
static_assert(PAL_POLLIN == POLLIN, "");
static_assert(PAL_POLLOUT == POLLOUT, "");
static_assert(PAL_POLLERR == POLLERR, "");
static_assert(PAL_POLLHUP == POLLHUP, "");
static_assert(PAL_POLLNVAL == POLLNVAL, "");

// Validate our FileAdvice enum values are correct for the platform
#if HAVE_POSIX_ADVISE
static_assert(PAL_POSIX_FADV_NORMAL == POSIX_FADV_NORMAL, "");
static_assert(PAL_POSIX_FADV_RANDOM == POSIX_FADV_RANDOM, "");
static_assert(PAL_POSIX_FADV_SEQUENTIAL == POSIX_FADV_SEQUENTIAL, "");
static_assert(PAL_POSIX_FADV_WILLNEED == POSIX_FADV_WILLNEED, "");
static_assert(PAL_POSIX_FADV_DONTNEED == POSIX_FADV_DONTNEED, "");
static_assert(PAL_POSIX_FADV_NOREUSE == POSIX_FADV_NOREUSE, "");
#endif

// Validate our NotifyEvents enum values are correct for the platform
#if HAVE_INOTIFY
static_assert(PAL_IN_ACCESS == IN_ACCESS, "");
static_assert(PAL_IN_MODIFY == IN_MODIFY, "");
static_assert(PAL_IN_ATTRIB == IN_ATTRIB, "");
static_assert(PAL_IN_MOVED_FROM == IN_MOVED_FROM, "");
static_assert(PAL_IN_MOVED_TO == IN_MOVED_TO, "");
static_assert(PAL_IN_CREATE == IN_CREATE, "");
static_assert(PAL_IN_DELETE == IN_DELETE, "");
static_assert(PAL_IN_Q_OVERFLOW == IN_Q_OVERFLOW, "");
static_assert(PAL_IN_IGNORED == IN_IGNORED, "");
static_assert(PAL_IN_ONLYDIR == IN_ONLYDIR, "");
static_assert(PAL_IN_DONT_FOLLOW == IN_DONT_FOLLOW, "");
static_assert(PAL_IN_EXCL_UNLINK == IN_EXCL_UNLINK, "");
static_assert(PAL_IN_ISDIR == IN_ISDIR, "");
#endif

static void ConvertFileStatus(const struct stat_& src, FileStatus* dst)
{
    dst->Flags = FILESTATUS_FLAGS_NONE;
    dst->Mode = static_cast<int32_t>(src.st_mode);
    dst->Uid = src.st_uid;
    dst->Gid = src.st_gid;
    dst->Size = src.st_size;
    dst->ATime = src.st_atime;
    dst->MTime = src.st_mtime;
    dst->CTime = src.st_ctime;

#if HAVE_STAT_BIRTHTIME
    dst->BirthTime = src.st_birthtime;
    dst->Flags |= FILESTATUS_FLAGS_HAS_BIRTHTIME;
#else
    dst->BirthTime = 0;
#endif
}

// TODO: temporarily keeping the un-prefixed signature of this method
// to keep tests running in CI. This will be removed once the managed assemblies
// are synced up with the native assemblies.
extern "C" int32_t Stat(const char* path, FileStatus* output)
{
    return SystemNative_Stat(path, output);
}

extern "C" int32_t SystemNative_Stat(const char* path, FileStatus* output)
{
    struct stat_ result;
    int ret;
    while (CheckInterrupted(ret = stat_(path, &result)));

    if (ret == 0)
    {
        ConvertFileStatus(result, output);
    }

    return ret;
}

// TODO: temporarily keeping the un-prefixed signature of this method
// to keep tests running in CI. This will be removed once the managed assemblies
// are synced up with the native assemblies.
extern "C" int32_t FStat(intptr_t fd, FileStatus* output)
{
    return SystemNative_FStat(fd, output);
}

extern "C" int32_t SystemNative_FStat(intptr_t fd, FileStatus* output)
{
    struct stat_ result;
    int ret;
    while (CheckInterrupted(ret = fstat_(ToFileDescriptor(fd), &result)));

    if (ret == 0)
    {
        ConvertFileStatus(result, output);
    }

    return ret;
}

// TODO: temporarily keeping the un-prefixed signature of this method
// to keep tests running in CI. This will be removed once the managed assemblies
// are synced up with the native assemblies.
extern "C" int32_t LStat(const char* path, FileStatus* output)
{
    return SystemNative_LStat(path, output);
}

extern "C" int32_t SystemNative_LStat(const char* path, FileStatus* output)
{
    struct stat_ result;
    int ret = lstat_(path, &result);

    if (ret == 0)
    {
        ConvertFileStatus(result, output);
    }

    return ret;
}

static int32_t ConvertOpenFlags(int32_t flags)
{
    int32_t ret;
    switch (flags & PAL_O_ACCESS_MODE_MASK)
    {
        case PAL_O_RDONLY:
            ret = O_RDONLY;
            break;
        case PAL_O_RDWR:
            ret = O_RDWR;
            break;
        case PAL_O_WRONLY:
            ret = O_WRONLY;
            break;
        default:
            assert(false && "Unknown Open access mode.");
            return -1;
    }

    if (flags & ~(PAL_O_ACCESS_MODE_MASK | PAL_O_CLOEXEC | PAL_O_CREAT | PAL_O_EXCL | PAL_O_TRUNC | PAL_O_SYNC))
    {
        assert(false && "Unknown Open flag.");
        return -1;
    }

    if (flags & PAL_O_CLOEXEC)
        ret |= O_CLOEXEC;
    if (flags & PAL_O_CREAT)
        ret |= O_CREAT;
    if (flags & PAL_O_EXCL)
        ret |= O_EXCL;
    if (flags & PAL_O_TRUNC)
        ret |= O_TRUNC;
    if (flags & PAL_O_SYNC)
        ret |= O_SYNC;

    assert(ret != -1);
    return ret;
}

// TODO: temporarily keeping the un-prefixed signature of this method
// to keep tests running in CI. This will be removed once the managed assemblies
// are synced up with the native assemblies.
extern "C" intptr_t Open(const char* path, int32_t flags, int32_t mode)
{
    return SystemNative_Open(path, flags, mode);
}

extern "C" intptr_t SystemNative_Open(const char* path, int32_t flags, int32_t mode)
{
    flags = ConvertOpenFlags(flags);
    if (flags == -1)
    {
        errno = EINVAL;
        return -1;
    }

    int result;
    while (CheckInterrupted(result = open(path, flags, static_cast<mode_t>(mode))));
    return result;
}

// TODO: temporarily keeping the un-prefixed signature of this method
// to keep tests running in CI. This will be removed once the managed assemblies
// are synced up with the native assemblies.
extern "C" int32_t Close(intptr_t fd)
{
    return SystemNative_Close(fd);
}

extern "C" int32_t SystemNative_Close(intptr_t fd)
{
    return close(ToFileDescriptor(fd));
}

// TODO: temporarily keeping the un-prefixed signature of this method
// to keep tests running in CI. This will be removed once the managed assemblies
// are synced up with the native assemblies.
extern "C" intptr_t Dup(intptr_t oldfd)
{
    return SystemNative_Dup(oldfd);
}

extern "C" intptr_t SystemNative_Dup(intptr_t oldfd)
{
    int result;
    while (CheckInterrupted(result = dup(ToFileDescriptor(oldfd))));
    return result;
}

// TODO: temporarily keeping the un-prefixed signature of this method
// to keep tests running in CI. This will be removed once the managed assemblies
// are synced up with the native assemblies.
extern "C" int32_t Unlink(const char* path)
{
    return SystemNative_Unlink(path);
}

extern "C" int32_t SystemNative_Unlink(const char* path)
{
    int32_t result;
    while (CheckInterrupted(result = unlink(path)));
    return result;
}

// TODO: temporarily keeping the un-prefixed signature of this method
// to keep tests running in CI. This will be removed once the managed assemblies
// are synced up with the native assemblies.
extern "C" intptr_t ShmOpen(const char* name, int32_t flags, int32_t mode)
{
    return SystemNative_ShmOpen(name, flags, mode);
}

extern "C" intptr_t SystemNative_ShmOpen(const char* name, int32_t flags, int32_t mode)
{
#if HAVE_SHM_OPEN_THAT_WORKS_WELL_ENOUGH_WITH_MMAP
    flags = ConvertOpenFlags(flags);
    if (flags == -1)
    {
        errno = EINVAL;
        return -1;
    }

    return shm_open(name, flags, static_cast<mode_t>(mode));
#else
    (void)name, (void)flags, (void)mode;
    errno = ENOTSUP;
    return -1;
#endif
}

// TODO: temporarily keeping the un-prefixed signature of this method
// to keep tests running in CI. This will be removed once the managed assemblies
// are synced up with the native assemblies.
extern "C" int32_t ShmUnlink(const char* name)
{
    return SystemNative_ShmUnlink(name);
}

extern "C" int32_t SystemNative_ShmUnlink(const char* name)
{
    int32_t result;
    while (CheckInterrupted(result = shm_unlink(name)));
    return result;
}

static void ConvertDirent(const dirent& entry, DirectoryEntry* outputEntry)
{
    // We use Marshal.PtrToStringAnsi on the managed side, which takes a pointer to
    // the start of the unmanaged string. Give the caller back a pointer to the
    // location of the start of the string that exists in their own byte buffer.
    outputEntry->Name = entry.d_name;
    outputEntry->InodeType = static_cast<NodeType>(entry.d_type);

#if HAVE_DIRENT_NAME_LEN
    outputEntry->NameLength = entry.d_namlen;
#else
    outputEntry->NameLength = -1; // sentinel value to mean we have to walk to find the first \0
#endif
}

// TODO: temporarily keeping the un-prefixed signature of this method
// to keep tests running in CI. This will be removed once the managed assemblies
// are synced up with the native assemblies.
extern "C" int32_t GetDirentSize()
{
    return SystemNative_GetDirentSize();
}

extern "C" int32_t SystemNative_GetDirentSize()
{
    // dirent should be under 2k in size
    static_assert(sizeof(dirent) < 2048, "");
    return sizeof(dirent);
}

// TODO: temporarily keeping the un-prefixed signature of this method
// to keep tests running in CI. This will be removed once the managed assemblies
// are synced up with the native assemblies.
extern "C" int32_t ReadDirR(DIR* dir, void* buffer, int32_t bufferSize, DirectoryEntry* outputEntry)
{
    return SystemNative_ReadDirR(dir, buffer, bufferSize, outputEntry);
}

// To reduce the number of string copies, this function calling pattern works as follows:
// 1) The managed code calls GetDirentSize() to get the platform-specific
//    size of the dirent struct.
// 2) The managed code creates a byte[] buffer of the size of the native dirent
//    and passes a pointer to this buffer to this function.
// 3) This function passes input byte[] buffer to the OS to fill with dirent data
//    which makes the 1st strcpy.
// 4) The ConvertDirent function will set a pointer to the start of the inode name
//    in the byte[] buffer so the managed code and find it and copy it out of the
//    buffer into a managed string that the caller of the framework can use, making
//    the 2nd and final strcpy.
extern "C" int32_t SystemNative_ReadDirR(DIR* dir, void* buffer, int32_t bufferSize, DirectoryEntry* outputEntry)
{
    assert(buffer != nullptr);
    assert(dir != nullptr);
    assert(outputEntry != nullptr);

    if (bufferSize < static_cast<int32_t>(sizeof(dirent)))
    {
        assert(false && "Buffer size too small; use GetDirentSize to get required buffer size");
        return ERANGE;
    }

    dirent* result = nullptr;
    dirent* entry = static_cast<dirent*>(buffer);
    int error = readdir_r(dir, entry, &result);

    // positive error number returned -> failure
    if (error != 0)
    {
        assert(error > 0);
        *outputEntry = {}; // managed out param must be initialized
        return error;
    }

    // 0 returned with null result -> end-of-stream
    if (result == nullptr)
    {
        *outputEntry = {}; // managed out param must be initialized
        return -1;         // shim convention for end-of-stream
    }

    // 0 returned with non-null result (guaranteed to be set to entry arg) -> success
    assert(result == entry);
    ConvertDirent(*entry, outputEntry);
    return 0;
}

// TODO: temporarily keeping the un-prefixed signature of this method
// to keep tests running in CI. This will be removed once the managed assemblies
// are synced up with the native assemblies.
extern "C" DIR* OpenDir(const char* path)
{
    return SystemNative_OpenDir(path);
}

extern "C" DIR* SystemNative_OpenDir(const char* path)
{
    return opendir(path);
}

// TODO: temporarily keeping the un-prefixed signature of this method
// to keep tests running in CI. This will be removed once the managed assemblies
// are synced up with the native assemblies.
extern "C" int32_t CloseDir(DIR* dir)
{
    return SystemNative_CloseDir(dir);
}

extern "C" int32_t SystemNative_CloseDir(DIR* dir)
{
    return closedir(dir);
}

// TODO: temporarily keeping the un-prefixed signature of this method
// to keep tests running in CI. This will be removed once the managed assemblies
// are synced up with the native assemblies.
extern "C" int32_t Pipe(int32_t pipeFds[2], int32_t flags)
{
    return SystemNative_Pipe(pipeFds, flags);
}

extern "C" int32_t SystemNative_Pipe(int32_t pipeFds[2], int32_t flags)
{
    switch (flags)
    {
        case 0:
            break;
        case PAL_O_CLOEXEC:
            flags = O_CLOEXEC;
            break;
        default:
            assert(false && "Unknown flag.");
            errno = EINVAL;
            return -1;
    }

    int32_t result;
#if HAVE_PIPE2
    while (CheckInterrupted(result = pipe2(pipeFds, flags)));
#else
    while (CheckInterrupted(result = pipe(pipeFds)));         // CLOEXEC intentionally ignored on platforms without pipe2.
#endif
    return result;
}

// TODO: temporarily keeping the un-prefixed signature of this method
// to keep tests running in CI. This will be removed once the managed assemblies
// are synced up with the native assemblies.
extern "C" int32_t FcntlCanGetSetPipeSz()
{
    return SystemNative_FcntlCanGetSetPipeSz();
}

extern "C" int32_t SystemNative_FcntlCanGetSetPipeSz()
{
#if defined(F_GETPIPE_SZ) && defined(F_SETPIPE_SZ)
    return true;
#else
    return false;
#endif
}

// TODO: temporarily keeping the un-prefixed signature of this method
// to keep tests running in CI. This will be removed once the managed assemblies
// are synced up with the native assemblies.
extern "C" int32_t FcntlGetPipeSz(intptr_t fd)
{
    return SystemNative_FcntlGetPipeSz(fd);
}

extern "C" int32_t SystemNative_FcntlGetPipeSz(intptr_t fd)
{
#ifdef F_GETPIPE_SZ
    int32_t result;
    while (CheckInterrupted(result = fcntl(ToFileDescriptor(fd), F_GETPIPE_SZ)));
    return result;
#else
    (void)fd;
    errno = ENOTSUP;
    return -1;
#endif
}

// TODO: temporarily keeping the un-prefixed signature of this method
// to keep tests running in CI. This will be removed once the managed assemblies
// are synced up with the native assemblies.
extern "C" int32_t FcntlSetPipeSz(intptr_t fd, int32_t size)
{
    return SystemNative_FcntlSetPipeSz(fd, size);
}

extern "C" int32_t SystemNative_FcntlSetPipeSz(intptr_t fd, int32_t size)
{
#ifdef F_SETPIPE_SZ
    int32_t result;
    while (CheckInterrupted(result = fcntl(ToFileDescriptor(fd), F_SETPIPE_SZ, size)));
    return result;
#else
    (void)fd, (void)size;
    errno = ENOTSUP;
    return -1;
#endif
}

// TODO: temporarily keeping the un-prefixed signature of this method
// to keep tests running in CI. This will be removed once the managed assemblies
// are synced up with the native assemblies.
extern "C" int32_t FcntlSetIsNonBlocking(intptr_t fd, int32_t isNonBlocking)
{
    return SystemNative_FcntlSetIsNonBlocking(fd, isNonBlocking);
}

extern "C" int32_t SystemNative_FcntlSetIsNonBlocking(intptr_t fd, int32_t isNonBlocking)
{
    int fileDescriptor = ToFileDescriptor(fd);

    int flags = fcntl(fileDescriptor, F_GETFL);
    if (flags == -1)
    {
        return -1;
    }

    if (isNonBlocking == 0)
    {
        flags &= ~O_NONBLOCK;
    }
    else
    {
        flags |= O_NONBLOCK;
    }

    return fcntl(fileDescriptor, F_SETFL, flags);
}

// TODO: temporarily keeping the un-prefixed signature of this method
// to keep tests running in CI. This will be removed once the managed assemblies
// are synced up with the native assemblies.
extern "C" int32_t MkDir(const char* path, int32_t mode)
{
    return SystemNative_MkDir(path, mode);
}

extern "C" int32_t SystemNative_MkDir(const char* path, int32_t mode)
{
    int32_t result;
    while (CheckInterrupted(result = mkdir(path, static_cast<mode_t>(mode))));
    return result;
}

// TODO: temporarily keeping the un-prefixed signature of this method
// to keep tests running in CI. This will be removed once the managed assemblies
// are synced up with the native assemblies.
extern "C" int32_t ChMod(const char* path, int32_t mode)
{
    return SystemNative_ChMod(path, mode);
}

extern "C" int32_t SystemNative_ChMod(const char* path, int32_t mode)
{
    int32_t result;
    while (CheckInterrupted(result = chmod(path, static_cast<mode_t>(mode))));
    return result;
}

// TODO: temporarily keeping the un-prefixed signature of this method
// to keep tests running in CI. This will be removed once the managed assemblies
// are synced up with the native assemblies.
extern "C" int32_t MkFifo(const char* path, int32_t mode)
{
    return SystemNative_MkFifo(path, mode);
}

extern "C" int32_t SystemNative_MkFifo(const char* path, int32_t mode)
{
    int32_t result;
    while (CheckInterrupted(result = mkfifo(path, static_cast<mode_t>(mode))));
    return result;
}

// TODO: temporarily keeping the un-prefixed signature of this method
// to keep tests running in CI. This will be removed once the managed assemblies
// are synced up with the native assemblies.
extern "C" int32_t FSync(intptr_t fd)
{
    return SystemNative_FSync(fd);
}

extern "C" int32_t SystemNative_FSync(intptr_t fd)
{
    int32_t result;
    while (CheckInterrupted(result = fsync(ToFileDescriptor(fd))));
    return result;
}

// TODO: temporarily keeping the un-prefixed signature of this method
// to keep tests running in CI. This will be removed once the managed assemblies
// are synced up with the native assemblies.
extern "C" int32_t FLock(intptr_t fd, LockOperations operation)
{
    return SystemNative_FLock(fd, operation);
}

extern "C" int32_t SystemNative_FLock(intptr_t fd, LockOperations operation)
{
    int32_t result;
    while (CheckInterrupted(result = flock(ToFileDescriptor(fd), operation)));
    return result;
}

// TODO: temporarily keeping the un-prefixed signature of this method
// to keep tests running in CI. This will be removed once the managed assemblies
// are synced up with the native assemblies.
extern "C" int32_t ChDir(const char* path)
{
    return SystemNative_ChDir(path);
}

extern "C" int32_t SystemNative_ChDir(const char* path)
{
    int32_t result;
    while (CheckInterrupted(result = chdir(path)));
    return result;
}

// TODO: temporarily keeping the un-prefixed signature of this method
// to keep tests running in CI. This will be removed once the managed assemblies
// are synced up with the native assemblies.
extern "C" int32_t Access(const char* path, AccessMode mode)
{
    return SystemNative_Access(path, mode);
}

extern "C" int32_t SystemNative_Access(const char* path, AccessMode mode)
{
    return access(path, mode);
}

// TODO: temporarily keeping the un-prefixed signature of this method
// to keep tests running in CI. This will be removed once the managed assemblies
// are synced up with the native assemblies.
extern "C" int32_t FnMatch(const char* pattern, const char* path, FnMatchFlags flags)
{
    return SystemNative_FnMatch(pattern, path, flags);
}

extern "C" int32_t SystemNative_FnMatch(const char* pattern, const char* path, FnMatchFlags flags)
{
    return fnmatch(pattern, path, flags);
}

// TODO: temporarily keeping the un-prefixed signature of this method
// to keep tests running in CI. This will be removed once the managed assemblies
// are synced up with the native assemblies.
extern "C" int64_t LSeek(intptr_t fd, int64_t offset, SeekWhence whence)
{
    return SystemNative_LSeek(fd, offset, whence);
}

extern "C" int64_t SystemNative_LSeek(intptr_t fd, int64_t offset, SeekWhence whence)
{
    int64_t result;
    while (CheckInterrupted(result = lseek(ToFileDescriptor(fd), offset, whence)));
    return result;
}

// TODO: temporarily keeping the un-prefixed signature of this method
// to keep tests running in CI. This will be removed once the managed assemblies
// are synced up with the native assemblies.
extern "C" int32_t Link(const char* source, const char* linkTarget)
{
    return SystemNative_Link(source, linkTarget);
}

extern "C" int32_t SystemNative_Link(const char* source, const char* linkTarget)
{
    int32_t result;
    while (CheckInterrupted(result = link(source, linkTarget)));
    return result;
}

// TODO: temporarily keeping the un-prefixed signature of this method
// to keep tests running in CI. This will be removed once the managed assemblies
// are synced up with the native assemblies.
extern "C" intptr_t MksTemps(char* pathTemplate, int32_t suffixLength)
{
    return SystemNative_MksTemps(pathTemplate, suffixLength);
}

extern "C" intptr_t SystemNative_MksTemps(char* pathTemplate, int32_t suffixLength)
{
    intptr_t result;
    while (CheckInterrupted(result = mkstemps(pathTemplate, suffixLength)));
    return  result;
}

static int32_t ConvertMMapProtection(int32_t protection)
{
    if (protection == PAL_PROT_NONE)
        return PROT_NONE;

    if (protection & ~(PAL_PROT_READ | PAL_PROT_WRITE | PAL_PROT_EXEC))
    {
        assert(false && "Unknown protection.");
        return -1;
    }

    int32_t ret = 0;
    if (protection & PAL_PROT_READ)
        ret |= PROT_READ;
    if (protection & PAL_PROT_WRITE)
        ret |= PROT_WRITE;
    if (protection & PAL_PROT_EXEC)
        ret |= PROT_EXEC;

    assert(ret != -1);
    return ret;
}

static int32_t ConvertMMapFlags(int32_t flags)
{
    if (flags & ~(PAL_MAP_SHARED | PAL_MAP_PRIVATE | PAL_MAP_ANONYMOUS))
    {
        assert(false && "Unknown MMap flag.");
        return -1;
    }

    int32_t ret = 0;
    if (flags & PAL_MAP_PRIVATE)
        ret |= MAP_PRIVATE;
    if (flags & PAL_MAP_SHARED)
        ret |= MAP_SHARED;
    if (flags & PAL_MAP_ANONYMOUS)
        ret |= MAP_ANON;

    assert(ret != -1);
    return ret;
}

static int32_t ConvertMSyncFlags(int32_t flags)
{
    if (flags & ~(PAL_MS_SYNC | PAL_MS_ASYNC | PAL_MS_INVALIDATE))
    {
        assert(false && "Unknown MSync flag.");
        return -1;
    }

    int32_t ret = 0;
    if (flags & PAL_MS_SYNC)
        ret |= MS_SYNC;
    if (flags & PAL_MS_ASYNC)
        ret |= MS_ASYNC;
    if (flags & PAL_MS_INVALIDATE)
        ret |= MS_INVALIDATE;

    assert(ret != -1);
    return ret;
}

// TODO: temporarily keeping the un-prefixed signature of this method
// to keep tests running in CI. This will be removed once the managed assemblies
// are synced up with the native assemblies.
extern "C" void* MMap(void* address,
    uint64_t length,
    int32_t protection, // bitwise OR of PAL_PROT_*
    int32_t flags,      // bitwise OR of PAL_MAP_*, but PRIVATE and SHARED are mutually exclusive.
    intptr_t fd,
    int64_t offset)
{
    return SystemNative_MMap(address, length, protection, flags, fd, offset);
}

extern "C" void* SystemNative_MMap(void* address,
    uint64_t length,
    int32_t protection, // bitwise OR of PAL_PROT_*
    int32_t flags,      // bitwise OR of PAL_MAP_*, but PRIVATE and SHARED are mutually exclusive.
    intptr_t fd,
    int64_t offset)
{
        if (length > SIZE_MAX)
    {
        errno = ERANGE;
        return nullptr;
    }

    protection = ConvertMMapProtection(protection);
    flags = ConvertMMapFlags(flags);

    if (flags == -1 || protection == -1)
    {
        errno = EINVAL;
        return nullptr;
    }

    // Use ToFileDescriptorUnchecked to allow -1 to be passed for the file descriptor, since managed code explicitly uses -1
    void* ret = mmap(address, static_cast<size_t>(length), protection, flags, ToFileDescriptorUnchecked(fd), offset);
    if (ret == MAP_FAILED)
    {
        return nullptr;
    }

    assert(ret != nullptr);
    return ret;
}

// TODO: temporarily keeping the un-prefixed signature of this method
// to keep tests running in CI. This will be removed once the managed assemblies
// are synced up with the native assemblies.
extern "C" int32_t MUnmap(void* address, uint64_t length)
{
    return SystemNative_MUnmap(address, length);
}

extern "C" int32_t SystemNative_MUnmap(void* address, uint64_t length)
{
    if (length > SIZE_MAX)
    {
        errno = ERANGE;
        return -1;
    }

    return munmap(address, static_cast<size_t>(length));
}

// TODO: temporarily keeping the un-prefixed signature of this method
// to keep tests running in CI. This will be removed once the managed assemblies
// are synced up with the native assemblies.
extern "C" int32_t MAdvise(void* address, uint64_t length, MemoryAdvice advice)
{
    return SystemNative_MAdvise(address, length, advice);
}

extern "C" int32_t SystemNative_MAdvise(void* address, uint64_t length, MemoryAdvice advice)
{
    if (length > SIZE_MAX)
    {
        errno = ERANGE;
        return -1;
    }

    switch (advice)
    {
        case PAL_MADV_DONTFORK:
#ifdef MADV_DONTFORK
            return madvise(address, static_cast<size_t>(length), MADV_DONTFORK);
#else
            (void)address, (void)length, (void)advice;
            errno = ENOTSUP;
            return -1;
#endif
    }

    assert(false && "Unknown MemoryAdvice");
    errno = EINVAL;
    return -1;
}

// TODO: temporarily keeping the un-prefixed signature of this method
// to keep tests running in CI. This will be removed once the managed assemblies
// are synced up with the native assemblies.
extern "C" int32_t MLock(void* address, uint64_t length)
{
    return SystemNative_MLock(address, length);
}

extern "C" int32_t SystemNative_MLock(void* address, uint64_t length)
{
    if (length > SIZE_MAX)
    {
        errno = ERANGE;
        return -1;
    }

    return mlock(address, static_cast<size_t>(length));
}

// TODO: temporarily keeping the un-prefixed signature of this method
// to keep tests running in CI. This will be removed once the managed assemblies
// are synced up with the native assemblies.
extern "C" int32_t MUnlock(void* address, uint64_t length)
{
    return SystemNative_MUnlock(address, length);
}

extern "C" int32_t SystemNative_MUnlock(void* address, uint64_t length)
{
    if (length > SIZE_MAX)
    {
        errno = ERANGE;
        return -1;
    }

    return munlock(address, static_cast<size_t>(length));
}

// TODO: temporarily keeping the un-prefixed signature of this method
// to keep tests running in CI. This will be removed once the managed assemblies
// are synced up with the native assemblies.
extern "C" int32_t MProtect(void* address, uint64_t length, int32_t protection)
{
    return SystemNative_MProtect(address, length, protection);
}

extern "C" int32_t SystemNative_MProtect(void* address, uint64_t length, int32_t protection)
{
    if (length > SIZE_MAX)
    {
        errno = ERANGE;
        return -1;
    }

    protection = ConvertMMapProtection(protection);
    if (protection == -1)
    {
        errno = EINVAL;
        return -1;
    }

    return mprotect(address, static_cast<size_t>(length), protection);
}

// TODO: temporarily keeping the un-prefixed signature of this method
// to keep tests running in CI. This will be removed once the managed assemblies
// are synced up with the native assemblies.
extern "C" int32_t MSync(void* address, uint64_t length, int32_t flags)
{
    return SystemNative_MSync(address, length, flags);
}

extern "C" int32_t SystemNative_MSync(void* address, uint64_t length, int32_t flags)
{
    if (length > SIZE_MAX)
    {
        errno = ERANGE;
        return -1;
    }

    flags = ConvertMSyncFlags(flags);
    if (flags == -1)
    {
        errno = EINVAL;
        return -1;
    }

    return msync(address, static_cast<size_t>(length), flags);
}

// TODO: temporarily keeping the un-prefixed signature of this method
// to keep tests running in CI. This will be removed once the managed assemblies
// are synced up with the native assemblies.
extern "C" int64_t SysConf(SysConfName name)
{
    return SystemNative_SysConf(name);
}

extern "C" int64_t SystemNative_SysConf(SysConfName name)
{
    switch (name)
    {
        case PAL_SC_CLK_TCK:
            return sysconf(_SC_CLK_TCK);
        case PAL_SC_PAGESIZE:
            return sysconf(_SC_PAGESIZE);
    }

    assert(false && "Unknown SysConfName");
    errno = EINVAL;
    return -1;
}

// TODO: temporarily keeping the un-prefixed signature of this method
// to keep tests running in CI. This will be removed once the managed assemblies
// are synced up with the native assemblies.
extern "C" int32_t FTruncate(intptr_t fd, int64_t length)
{
    return SystemNative_FTruncate(fd, length);
}

extern "C" int32_t SystemNative_FTruncate(intptr_t fd, int64_t length)
{
    int32_t result;
    while (CheckInterrupted(result = ftruncate(ToFileDescriptor(fd), length)));
    return result;
}

// TODO: temporarily keeping the un-prefixed signature of this method
// to keep tests running in CI. This will be removed once the managed assemblies
// are synced up with the native assemblies.
extern "C" Error Poll(PollEvent* pollEvents, uint32_t eventCount, int32_t milliseconds, uint32_t* triggered)
{
    return SystemNative_Poll(pollEvents, eventCount, milliseconds, triggered);
}

extern "C" Error SystemNative_Poll(PollEvent* pollEvents, uint32_t eventCount, int32_t milliseconds, uint32_t* triggered)
{
    if (pollEvents == nullptr || triggered == nullptr)
    {
        return PAL_EFAULT;
    }

    if (milliseconds < -1)
    {
        return PAL_EINVAL;
    }

    size_t bufferSize = sizeof(pollfd) * static_cast<size_t>(eventCount);
    bool useStackBuffer = bufferSize <= 2048;
    pollfd* pollfds = reinterpret_cast<pollfd*>(useStackBuffer ? alloca(bufferSize) : malloc(bufferSize));

    for (uint32_t i = 0; i < eventCount; i++)
    {
        const PollEvent& event = pollEvents[i];
        pollfds[i] = { .fd = event.FileDescriptor, .events = event.Events, .revents = 0 };
    }

    int rv;
    while (CheckInterrupted(rv = poll(pollfds, static_cast<nfds_t>(eventCount), milliseconds)));

    if (rv < 0)
    {
        if (!useStackBuffer)
        {
            free(pollfds);
        }

        *triggered = 0;
        return SystemNative_ConvertErrorPlatformToPal(errno);
    }

    for (uint32_t i = 0; i < eventCount; i++)
    {
        const pollfd& pfd = pollfds[i];
        assert(pfd.fd == pollEvents[i].FileDescriptor);
        assert(pfd.events == pollEvents[i].Events);

        pollEvents[i].TriggeredEvents = static_cast<PollEvents>(pfd.revents);
    }

    *triggered = static_cast<uint32_t>(rv);

    if (!useStackBuffer)
    {
        free(pollfds);
    }

    return PAL_SUCCESS;
}

// TODO: temporarily keeping the un-prefixed signature of this method
// to keep tests running in CI. This will be removed once the managed assemblies
// are synced up with the native assemblies.
extern "C" int32_t PosixFAdvise(intptr_t fd, int64_t offset, int64_t length, FileAdvice advice)
{
    return SystemNative_PosixFAdvise(fd, offset, length, advice);
}

extern "C" int32_t SystemNative_PosixFAdvise(intptr_t fd, int64_t offset, int64_t length, FileAdvice advice)
{
#if HAVE_POSIX_ADVISE
    int32_t result;
    while (CheckInterrupted(result = posix_fadvise(ToFileDescriptor(fd), offset, length, advice)));
    return result;
#else
    // Not supported on this platform. Caller can ignore this failure since it's just a hint.
    (void)fd, (void)offset, (void)length, (void)advice;
    return ENOTSUP;
#endif
}

// TODO: temporarily keeping the un-prefixed signature of this method
// to keep tests running in CI. This will be removed once the managed assemblies
// are synced up with the native assemblies.
extern "C" int32_t Read(intptr_t fd, void* buffer, int32_t bufferSize)
{
    return SystemNative_Read(fd, buffer, bufferSize);
}

extern "C" int32_t SystemNative_Read(intptr_t fd, void* buffer, int32_t bufferSize)
{
    assert(buffer != nullptr || bufferSize == 0);
    assert(bufferSize >= 0);

    if (bufferSize < 0)
    {
        errno = EINVAL;
        return -1;
    }

    ssize_t count;
    while (CheckInterrupted(count = read(ToFileDescriptor(fd), buffer, UnsignedCast(bufferSize))));

    assert(count >= -1 && count <= bufferSize);
    return static_cast<int32_t>(count);
}

// TODO: temporarily keeping the un-prefixed signature of this method
// to keep tests running in CI. This will be removed once the managed assemblies
// are synced up with the native assemblies.
extern "C" int32_t ReadLink(const char* path, char* buffer, int32_t bufferSize)
{
    return SystemNative_ReadLink(path, buffer, bufferSize);
}

extern "C" int32_t SystemNative_ReadLink(const char* path, char* buffer, int32_t bufferSize)
{
    assert(buffer != nullptr || bufferSize == 0);
    assert(bufferSize >= 0);

    if (bufferSize < 0)
    {
        errno = EINVAL;
        return -1;
    }

    ssize_t count = readlink(path, buffer, static_cast<size_t>(bufferSize));
    assert(count >= -1 && count <= bufferSize);
    return static_cast<int32_t>(count);
}

// TODO: temporarily keeping the un-prefixed signature of this method
// to keep tests running in CI. This will be removed once the managed assemblies
// are synced up with the native assemblies.
extern "C" int32_t Rename(const char* oldPath, const char* newPath)
{
    return SystemNative_Rename(oldPath, newPath);
}

extern "C" int32_t SystemNative_Rename(const char* oldPath, const char* newPath)
{
    int32_t result;
    while (CheckInterrupted(result = rename(oldPath, newPath)));
    return result;
}

// TODO: temporarily keeping the un-prefixed signature of this method
// to keep tests running in CI. This will be removed once the managed assemblies
// are synced up with the native assemblies.
extern "C" int32_t RmDir(const char* path)
{
    return SystemNative_RmDir(path);
}

extern "C" int32_t SystemNative_RmDir(const char* path)
{
    int32_t result;
    while (CheckInterrupted(result = rmdir(path)));
    return result;
}

// TODO: temporarily keeping the un-prefixed signature of this method
// to keep tests running in CI. This will be removed once the managed assemblies
// are synced up with the native assemblies.
extern "C" void Sync()
{
    SystemNative_Sync();
}

extern "C" void SystemNative_Sync()
{
    sync();
}

// TODO: temporarily keeping the un-prefixed signature of this method
// to keep tests running in CI. This will be removed once the managed assemblies
// are synced up with the native assemblies.
extern "C" int32_t Write(intptr_t fd, const void* buffer, int32_t bufferSize)
{
    return SystemNative_Write(fd, buffer, bufferSize);
}

extern "C" int32_t SystemNative_Write(intptr_t fd, const void* buffer, int32_t bufferSize)
{
    assert(buffer != nullptr || bufferSize == 0);
    assert(bufferSize >= 0);

    if (bufferSize < 0)
    {
        errno = ERANGE;
        return -1;
    }

    ssize_t count;
    while (CheckInterrupted(count = write(ToFileDescriptor(fd), buffer, UnsignedCast(bufferSize))));

    assert(count >= -1 && count <= bufferSize);
    return static_cast<int32_t>(count);
}

extern "C" intptr_t SystemNative_INotifyInit()
{
#if HAVE_INOTIFY
    return inotify_init();
#else
    errno = ENOTSUP;
    return -1;
#endif
}

extern "C" int32_t SystemNative_INotifyAddWatch(intptr_t fd, const char* pathName, uint32_t mask)
{
    assert(fd >= 0);
    assert(pathName != nullptr);

#if HAVE_INOTIFY
    return inotify_add_watch(ToFileDescriptor(fd), pathName, mask);
#else
    (void)fd, (void)pathName, (void)mask;
    errno = ENOTSUP;
    return -1;
#endif
}

extern "C" int32_t SystemNative_INotifyRemoveWatch(intptr_t fd, int32_t wd)
{
    assert(fd >= 0);
    assert(wd >= 0);

#if HAVE_INOTIFY
    return inotify_rm_watch(ToFileDescriptor(fd), wd);
#else
    (void)fd, (void)wd;
    errno = ENOTSUP;
    return -1;
#endif
}
