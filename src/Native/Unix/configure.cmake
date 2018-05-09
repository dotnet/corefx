include(CheckCSourceCompiles)
include(CheckCSourceRuns)
include(CheckFunctionExists)
include(CheckIncludeFiles)
include(CheckPrototypeDefinition)
include(CheckStructHasMember)
include(CheckSymbolExists)
include(CheckTypeSize)

if (CMAKE_SYSTEM_NAME STREQUAL Linux)
    set(PAL_UNIX_NAME \"LINUX\")
elseif (CMAKE_SYSTEM_NAME STREQUAL Darwin)
    set(PAL_UNIX_NAME \"OSX\")

    # Xcode's clang does not include /usr/local/include by default, but brew's does.
    # This ensures an even playing field.
    include_directories(SYSTEM /usr/local/include)
elseif (CMAKE_SYSTEM_NAME STREQUAL FreeBSD)
    set(PAL_UNIX_NAME \"FREEBSD\")
    include_directories(SYSTEM /usr/local/include)
elseif (CMAKE_SYSTEM_NAME STREQUAL NetBSD)
    set(PAL_UNIX_NAME \"NETBSD\")
elseif (CMAKE_SYSTEM_NAME STREQUAL Emscripten)
    set(PAL_UNIX_NAME \"WEBASSEMBLY\")
else ()
    message(FATAL_ERROR "Unknown platform.  Cannot define PAL_UNIX_NAME, used by RuntimeInformation.")
endif ()

# We compile with -Werror, so we need to make sure these code fragments compile without warnings.
# Older CMake versions (3.8) do not assign the result of their tests, causing unused-value errors
# which are not distinguished from the test failing. So no error for that one.
set(CMAKE_REQUIRED_FLAGS "-Werror -Wno-error=unused-value")

# in_pktinfo: Find whether this struct exists
check_include_files(
    "sys/socket.h;linux/in.h"
    HAVE_LINUX_IN_H)

if (HAVE_LINUX_IN_H)
    set (SOCKET_INCLUDES linux/in.h)
else ()
    set (SOCKET_INCLUDES netinet/in.h)
endif ()

check_c_source_compiles(
    "
    #include <sys/socket.h>
    #include <${SOCKET_INCLUDES}>
    int main(void)
    {
        struct in_pktinfo pktinfo;
        return 0;
    }
    "
    HAVE_IN_PKTINFO)

check_c_source_compiles(
    "
    #include <sys/socket.h>
    #include <${SOCKET_INCLUDES}>
    int main(void)
    {
        struct ip_mreqn mreqn;
        return 0;
    }
    "
    HAVE_IP_MREQN)

# /in_pktinfo

check_c_source_compiles(
    "
    #include <fcntl.h>
    int main(void)
    {
        struct flock64 l;
        return 0;
    }
    "
    HAVE_FLOCK64)

check_symbol_exists(
    O_CLOEXEC
    fcntl.h
    HAVE_O_CLOEXEC)

check_symbol_exists(
    F_DUPFD_CLOEXEC
    fcntl.h
    HAVE_F_DUPFD_CLOEXEC)

check_function_exists(
    getifaddrs
    HAVE_GETIFADDRS)

check_symbol_exists(
    lseek64
    unistd.h
    HAVE_LSEEK64)

check_symbol_exists(
    mmap64
    sys/mman.h
    HAVE_MMAP64)

check_symbol_exists(
    ftruncate64
    unistd.h
    HAVE_FTRUNCATE64)

check_symbol_Exists(
    posix_fadvise64
    fnctl.h
    HAVE_POSIX_FADVISE64)

check_symbol_exists(
    stat64
    sys/stat.h
    HAVE_STAT64)

check_symbol_exists(
    pipe2
    unistd.h
    HAVE_PIPE2)

check_symbol_exists(
    getmntinfo
    mount.h
    HAVE_MNTINFO)

check_symbol_exists(
    strcpy_s
    string.h
    HAVE_STRCPY_S)

check_function_exists(
    strlcpy
    HAVE_STRLCPY)

check_function_exists(
    posix_fadvise
    HAVE_POSIX_ADVISE)

check_function_exists(
    ioctl
    HAVE_IOCTL)

check_function_exists(
    sched_getaffinity
    HAVE_SCHED_GETAFFINITY)

check_function_exists(
    sched_setaffinity
    HAVE_SCHED_SETAFFINITY)

check_function_exists(
    arc4random
    HAVE_ARC4RANDOM)

check_symbol_exists(
    TIOCGWINSZ
    "sys/ioctl.h"
    HAVE_TIOCGWINSZ)

check_function_exists(
    tcgetattr
    HAVE_TCGETATTR)

check_function_exists(
    tcsetattr
    HAVE_TCSETATTR)

check_symbol_exists(
    ECHO
    "termios.h"
    HAVE_ECHO)

check_symbol_exists(
    ICANON
    "termios.h"
    HAVE_ICANON)

check_symbol_exists(
    TCSANOW
    "termios.h"
    HAVE_TCSANOW)

check_struct_has_member(
    "struct stat"
    st_birthtimespec
    "sys/types.h;sys/stat.h"
    HAVE_STAT_BIRTHTIME)

check_struct_has_member(
    "struct stat"
    st_atimespec
    "sys/types.h;sys/stat.h"
    HAVE_STAT_TIMESPEC)

check_struct_has_member(
    "struct stat"
    st_atim
    "sys/types.h;sys/stat.h"
    HAVE_STAT_TIM)

check_struct_has_member(
    "struct stat"
    st_atimensec
    "sys/types.h;sys/stat.h"
    HAVE_STAT_NSEC)

check_struct_has_member(
    "struct dirent"
    d_namlen
    "dirent.h"
    HAVE_DIRENT_NAME_LEN)

check_struct_has_member(
    "struct statfs"
    f_fstypename
    "sys/mount.h"
    HAVE_STATFS_FSTYPENAME)

check_struct_has_member(
    "struct statvfs"
    f_fstypename
    "sys/mount.h"
    HAVE_STATVFS_FSTYPENAME)

# statfs: Find whether this struct exists
if (HAVE_STATFS_FSTYPENAME OR HAVE_STATVFS_FSTYPENAME)
    set (STATFS_INCLUDES sys/mount.h)
else ()
    set (STATFS_INCLUDES sys/statfs.h)
endif ()

set(CMAKE_EXTRA_INCLUDE_FILES ${STATFS_INCLUDES})
check_symbol_exists(
    "statfs",
    STATFS_INCLUDES,
    HAVE_STATFS
)

check_type_size(
    "struct statfs"
    STATFS_SIZE
    BUILTIN_TYPES_ONLY)
set(CMAKE_EXTRA_INCLUDE_FILES) # reset CMAKE_EXTRA_INCLUDE_FILES
# /statfs

check_c_source_compiles(
    "
    #include <string.h>
    int main(void)
    {
        char buffer[1];
        char c = *strerror_r(0, buffer, 0);
        return 0;
    }
    "
    HAVE_GNU_STRERROR_R)

check_c_source_compiles(
    "
    #include <dirent.h>
    int main(void)
    {
        DIR* dir;
        struct dirent* entry;
        struct dirent* result;
        readdir_r(dir, entry, &result);
        return 0;
    }
    "
    HAVE_READDIR_R)

check_c_source_compiles(
    "
    #include <sys/types.h>
    #include <sys/event.h>
    int main(void)
    {
        struct kevent event;
        void* data;
        EV_SET(&event, 0, EVFILT_READ, 0, 0, 0, data);
        return 0;
    }
    "
    KEVENT_HAS_VOID_UDATA)

check_struct_has_member(
    "struct fd_set"
    fds_bits
    "sys/select.h"
    HAVE_FDS_BITS)

check_struct_has_member(
    "struct fd_set"
    __fds_bits
    "sys/select.h"
    HAVE_PRIVATE_FDS_BITS)

check_c_source_compiles(
    "
    #include <sys/sendfile.h>
    int main(void) { int i = sendfile(0, 0, 0, 0); return 0; }
    "
    HAVE_SENDFILE_4)

check_c_source_compiles(
    "
    #include <stdlib.h>
    #include <sys/types.h>
    #include <sys/socket.h>
    #include <sys/uio.h>
    int main(void) { int i = sendfile(0, 0, 0, NULL, NULL, 0); return 0; }
    "
    HAVE_SENDFILE_6)

check_symbol_exists(
    fcopyfile
    copyfile.h
    HAVE_FCOPYFILE)

check_include_files(
     "sys/poll.h"
     HAVE_SYS_POLL_H)

check_function_exists(
    epoll_create1
    HAVE_EPOLL)

check_function_exists(
    accept4
    HAVE_ACCEPT4)

check_function_exists(
    kqueue
    HAVE_KQUEUE)

set(CMAKE_REQUIRED_FLAGS "-Werror -Wsign-conversion")
check_c_source_compiles(
     "
     #include <sys/types.h>
     #include <netdb.h>

     int main(void)
     {
        const struct sockaddr *addr;
        socklen_t addrlen;
        char *host;
        socklen_t hostlen;
        char *serv;
        socklen_t servlen;
        int flags;
        int result = getnameinfo(addr, addrlen, host, hostlen, serv, servlen, flags);
        return 0;
     }
     "
     HAVE_GETNAMEINFO_SIGNED_FLAGS)
set(CMAKE_REQUIRED_FLAGS -Werror)

set(HAVE_SUPPORT_FOR_DUAL_MODE_IPV4_PACKET_INFO 0)

if (CMAKE_SYSTEM_NAME STREQUAL Linux)
    if (NOT CLR_CMAKE_PLATFORM_ANDROID)
        set(CMAKE_REQUIRED_LIBRARIES rt)
    endif ()

    set(HAVE_SUPPORT_FOR_DUAL_MODE_IPV4_PACKET_INFO 1)
    
endif ()

check_c_source_runs(
    "
    #include <stdlib.h>
    #include <time.h>
    #include <sys/time.h>
    int main(void)
    {
        int ret;
        struct timespec ts;
        ret = clock_gettime(CLOCK_MONOTONIC, &ts);
        exit(ret);
        return 0;
    }
    "
    HAVE_CLOCK_MONOTONIC)

check_c_source_runs(
    "
    #include <stdlib.h>
    #include <time.h>
    #include <sys/time.h>
    int main(void)
    {
        int ret;
        struct timespec ts;
        ret = clock_gettime(CLOCK_REALTIME, &ts);
        exit(ret);
        return 0;
    }
    "
    HAVE_CLOCK_REALTIME)

check_symbol_exists(
    mach_absolute_time
    mach/mach_time.h
    HAVE_MACH_ABSOLUTE_TIME)

check_symbol_exists(
    mach_timebase_info
    mach/mach_time.h
    HAVE_MACH_TIMEBASE_INFO)

check_function_exists(
    futimes
    HAVE_FUTIMES)

check_function_exists(
    futimens
    HAVE_FUTIMENS)

check_function_exists(
    utimensat
    HAVE_UTIMENSAT)

set (PREVIOUS_CMAKE_REQUIRED_FLAGS ${CMAKE_REQUIRED_FLAGS})
set (CMAKE_REQUIRED_FLAGS "-Werror -Wsign-conversion")

check_c_source_compiles(
    "
    #include <sys/socket.h>

    int main(void)
    {
        int fd;
        struct sockaddr* addr;
        socklen_t addrLen;

        int err = bind(fd, addr, addrLen);
        return 0;
    }
    "
    BIND_ADDRLEN_UNSIGNED
)

check_c_source_compiles(
    "
    #include <netinet/in.h>
    #include <netinet/tcp.h>

    int main(void)
    {
        struct ipv6_mreq opt;
        unsigned int index = 0;
        opt.ipv6mr_interface = index;
        return 0;
    }
    "
    IPV6MR_INTERFACE_UNSIGNED
)

check_c_source_compiles(
    "
    #include <sys/inotify.h>

    int main(void)
    {
        intptr_t fd;
        uint32_t wd;
        return inotify_rm_watch(fd, wd);
    }
    "
    INOTIFY_RM_WATCH_WD_UNSIGNED)

set (CMAKE_REQUIRED_FLAGS ${PREVIOUS_CMAKE_REQUIRED_FLAGS})

check_c_source_runs(
    "
    #include <sys/mman.h>
    #include <fcntl.h>
    #include <unistd.h>

    int main(void)
    {
        int fd = shm_open(\"/corefx_configure_shm_open\", O_CREAT | O_RDWR, 0777);
        if (fd == -1)
            return -1;

        shm_unlink(\"/corefx_configure_shm_open\");

        // NOTE: PROT_EXEC and MAP_PRIVATE don't work well with shm_open
        //       on at least the current version of Mac OS X

        if (mmap(NULL, 1, PROT_EXEC, MAP_PRIVATE, fd, 0) == MAP_FAILED)
            return -1;

        return 0;
    }
    "
    HAVE_SHM_OPEN_THAT_WORKS_WELL_ENOUGH_WITH_MMAP)

check_prototype_definition(
    getpriority
    "int getpriority(int which, int who)"
    0
    "sys/resource.h"
    PRIORITY_REQUIRES_INT_WHO)

check_prototype_definition(
    kevent
    "int kevent(int kg, const struct kevent* chagelist, int nchanges, struct kevent* eventlist, int nevents, const struct timespec* timeout)"
    0
    "sys/types.h;sys/event.h"
    KEVENT_REQUIRES_INT_PARAMS)

check_c_source_compiles(
    "
    #include <stdlib.h>
    #include <unistd.h>
    #include <string.h>

    int main(void)
    {
        return mkstemps(\"abc\", 3);
    }
    "
    HAVE_MKSTEMPS)

check_c_source_compiles(
    "
    #include <stdlib.h>
    #include <unistd.h>
    #include <string.h>

    int main(void)
    {
        return mkstemp(\"abc\");
    }
    "
    HAVE_MKSTEMP)

if (NOT HAVE_MKSTEMPS AND NOT HAVE_MKSTEMP)
    message(FATAL_ERROR "Cannot find mkstemp nor mkstemp on this platform.")
endif()

check_c_source_compiles(
    "
    #include <sys/types.h>
    #include <sys/socketvar.h>
    #include <netinet/ip.h>
    #include <netinet/tcp.h>
    #include <netinet/tcp_var.h>
    int main(void) { return 0; }
    "
    HAVE_TCP_VAR_H
)

check_include_files(
    sys/cdefs.h
    HAVE_SYS_CDEFS_H)

if (HAVE_SYS_CDEFS_H)
    set(CMAKE_REQUIRED_DEFINITIONS "-DHAVE_SYS_CDEFS_H")
endif()

# If sys/cdefs is not included on Android, this check will fail because
# __BEGIN_DECLS is not defined
check_c_source_compiles(
    "
#ifdef HAVE_SYS_CDEFS_H
    #include <sys/cdefs.h>
#endif
    #include <netinet/tcp.h>
    int main(void) { int x = TCP_ESTABLISHED; return x; }
    "
    HAVE_TCP_H_TCPSTATE_ENUM
)

set(CMAKE_REQUIRED_DEFINITIONS)

check_symbol_exists(
    TCPS_ESTABLISHED
    "netinet/tcp_fsm.h"
    HAVE_TCP_FSM_H
)

set(CMAKE_EXTRA_INCLUDE_FILES sys/types.h net/route.h)
check_type_size(
    "struct rt_msghdr"
     HAVE_RT_MSGHDR
     BUILTIN_TYPES_ONLY)
set(CMAKE_EXTRA_INCLUDE_FILES) # reset CMAKE_EXTRA_INCLUDE_FILES

check_include_files(
    "sys/types.h;sys/sysctl.h"
    HAVE_SYS_SYSCTL_H)

check_include_files(
    linux/rtnetlink.h
    HAVE_LINUX_RTNETLINK_H)

check_symbol_exists(
    getpeereid
    unistd.h
    HAVE_GETPEEREID)

check_function_exists(
    getdomainname
    HAVE_GETDOMAINNAME)

check_function_exists(
    uname
    HAVE_UNAME)

check_symbol_exists(
    ucred
    sys/socket.h
    HAVE_UCRED
)

# getdomainname on OSX takes an 'int' instead of a 'size_t'
# check if compiling with 'size_t' would cause a warning
set (PREVIOUS_CMAKE_REQUIRED_FLAGS ${CMAKE_REQUIRED_FLAGS})
set (CMAKE_REQUIRED_FLAGS "-Werror -Weverything")
check_c_source_compiles(
    "
    #include <unistd.h>
    int main(void) { size_t namelen = 20; char name[20]; getdomainname(name, namelen); return 0; }
    "
    HAVE_GETDOMAINNAME_SIZET
)
set (CMAKE_REQUIRED_FLAGS ${PREVIOUS_CMAKE_REQUIRED_FLAGS})

check_function_exists(
    inotify_init
    HAVE_INOTIFY_INIT)

check_function_exists(
    inotify_add_watch
    HAVE_INOTIFY_ADD_WATCH)

check_function_exists(
    inotify_rm_watch
    HAVE_INOTIFY_RM_WATCH)

set (HAVE_INOTIFY 0)
if (HAVE_INOTIFY_INIT AND HAVE_INOTIFY_ADD_WATCH AND HAVE_INOTIFY_RM_WATCH)
    set (HAVE_INOTIFY 1)
elseif (CMAKE_SYSTEM_NAME STREQUAL Linux)
    message(FATAL_ERROR "Cannot find inotify functions on a Linux platform.")
endif()

check_c_source_compiles(
    "
    #include <curl/multi.h>
    int main(void) { int i = CURLM_ADDED_ALREADY; return 0; }
    "
    HAVE_CURLM_ADDED_ALREADY)

check_c_source_compiles(
    "
    #include <curl/multi.h>
    int main(void) { int i = CURL_HTTP_VERSION_2TLS; return 0; }
    "
    HAVE_CURL_HTTP_VERSION_2TLS)

check_c_source_compiles(
    "
    #include <curl/multi.h>
    int main(void) { int i = CURLPIPE_MULTIPLEX; return 0; }
    "
    HAVE_CURLPIPE_MULTIPLEX)

check_c_source_compiles(
    "
    #include <curl/curl.h>
    int main(void)
    {
        int i = CURL_SSLVERSION_TLSv1_0;
        i = CURL_SSLVERSION_TLSv1_1;
        i = CURL_SSLVERSION_TLSv1_2;
        return 0;
    }
    "
    HAVE_CURL_SSLVERSION_TLSv1_012)

option(HeimdalGssApi "use heimdal implementation of GssApi" OFF)

if (HeimdalGssApi)
   check_include_files(
       gssapi/gssapi.h
       HAVE_HEIMDAL_HEADERS)
endif()

check_include_files(
    GSS/GSS.h
    HAVE_GSSFW_HEADERS)

if (HAVE_GSSFW_HEADERS)
    check_symbol_exists(
        GSS_SPNEGO_MECHANISM
        "GSS/GSS.h"
        HAVE_GSS_SPNEGO_MECHANISM)
else ()
    check_symbol_exists(
        GSS_SPNEGO_MECHANISM
        "gssapi/gssapi.h"
        HAVE_GSS_SPNEGO_MECHANISM)
endif ()

if (HAVE_GSSFW_HEADERS)
    check_symbol_exists(
        GSS_KRB5_CRED_NO_CI_FLAGS_X
        "GSS/GSS.h"
        HAVE_GSS_KRB5_CRED_NO_CI_FLAGS_X)
else ()
    check_symbol_exists(
        GSS_KRB5_CRED_NO_CI_FLAGS_X
        "gssapi/gssapi_krb5.h"
        HAVE_GSS_KRB5_CRED_NO_CI_FLAGS_X)
endif ()

check_include_files(crt_externs.h HAVE_CRT_EXTERNS_H)

if (HAVE_CRT_EXTERNS_H)
    check_c_source_compiles(
    "
    #include <crt_externs.h>
    int main(void) { char** e = *(_NSGetEnviron()); return 0; }
    "
    HAVE_NSGETENVIRON)
endif()

set (CMAKE_REQUIRED_LIBRARIES)

check_c_source_compiles(
    "
    #include <sys/inotify.h>
    int main(void)
    {
        uint32_t mask = IN_EXCL_UNLINK;
        return 0;
    }
    "
    HAVE_IN_EXCL_UNLINK)

check_c_source_compiles(
    "
    #include <netinet/tcp.h>
    int main(void)
    {
        int x = TCP_KEEPALIVE;
        return x;
    }
    "
    HAVE_TCP_H_TCP_KEEPALIVE
)

configure_file(
    ${CMAKE_CURRENT_SOURCE_DIR}/Common/pal_config.h.in
    ${CMAKE_CURRENT_BINARY_DIR}/Common/pal_config.h)
