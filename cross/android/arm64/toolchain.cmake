set(CROSS_NDK_TOOLCHAIN $ENV{ROOTFS_DIR}/..)
set(CROSS_ROOTFS ${CROSS_NDK_TOOLCHAIN}/sysroot)
set(CLR_CMAKE_PLATFORM_ANDROID "Android")

set(CMAKE_SYSTEM_NAME Linux)
set(CMAKE_SYSTEM_VERSION 1)
set(CMAKE_SYSTEM_PROCESSOR aarch64)

## Specify the toolchain
set(TOOLCHAIN "aarch64-linux-android")
set(TOOLCHAIN_PREFIX ${CROSS_NDK_TOOLCHAIN}/bin/${TOOLCHAIN}-)
set(CMAKE_C_COMPILER ${TOOLCHAIN_PREFIX}clang)
set(CMAKE_CXX_COMPILER ${TOOLCHAIN_PREFIX}clang++)
set(CMAKE_ASM_COMPILER ${TOOLCHAIN_PREFIX}clang)
set(CMAKE_OBJCOPY ${TOOLCHAIN_PREFIX}objcopy)
set(CMAKE_OBJDUMP ${TOOLCHAIN_PREFIX}objdump)

add_compile_options(--sysroot=${CROSS_ROOTFS})
add_compile_options(-fPIE)

set(CROSS_LINK_FLAGS "${CROSS_LINK_FLAGS} -B ${CROSS_ROOTFS}/usr/lib/gcc/${TOOLCHAIN}")
set(CROSS_LINK_FLAGS "${CROSS_LINK_FLAGS} -L${CROSS_ROOTFS}/lib/${TOOLCHAIN}")
set(CROSS_LINK_FLAGS "${CROSS_LINK_FLAGS} --sysroot=${CROSS_ROOTFS}")
set(CROSS_LINK_FLAGS "${CROSS_LINK_FLAGS} -fPIE -pie")

set(CMAKE_EXE_LINKER_FLAGS    "${CMAKE_EXE_LINKER_FLAGS}    ${CROSS_LINK_FLAGS}" CACHE STRING "" FORCE)
set(CMAKE_SHARED_LINKER_FLAGS "${CMAKE_SHARED_LINKER_FLAGS} ${CROSS_LINK_FLAGS}" CACHE STRING "" FORCE)
set(CMAKE_MODULE_LINKER_FLAGS "${CMAKE_MODULE_LINKER_FLAGS} ${CROSS_LINK_FLAGS}" CACHE STRING "" FORCE)

set(CMAKE_FIND_ROOT_PATH "${CROSS_ROOTFS}")
set(CMAKE_FIND_ROOT_PATH_MODE_PROGRAM NEVER)
set(CMAKE_FIND_ROOT_PATH_MODE_LIBRARY ONLY)
set(CMAKE_FIND_ROOT_PATH_MODE_INCLUDE ONLY)
set(CMAKE_FIND_ROOT_PATH_MODE_PACKAGE ONLY)
