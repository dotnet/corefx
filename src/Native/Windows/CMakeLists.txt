cmake_minimum_required(VERSION 2.8.12)

# C Compiler flags
SET (CMAKE_C_FLAGS_INIT                     "/W0 /FC")
SET (CMAKE_C_FLAGS_DEBUG_INIT               "/Od /Zi")
SET (CMAKE_C_FLAGS_RELEASE_INIT             "/Ox")
SET (CMAKE_C_FLAGS_RELWITHDEBINFO_INIT      "/O2 /Zi")

# CXX Compiler flags
SET (CMAKE_CXX_FLAGS                        "-std=c++11")
SET (CMAKE_CXX_FLAGS_INIT                   "/W0 /FC")
SET (CMAKE_CXX_FLAGS_DEBUG_INIT             "/Od /Zi")
SET (CMAKE_CXX_FLAGS_RELEASE_INIT           "/Ox")
SET (CMAKE_CXX_FLAGS_RELWITHDEBINFO_INIT    "/O2 /Zi")

# Allow explicit specification of the C Compiler
if (DEFINED ENV{__CCompiler})
    SET (COMPILER_HAS_DEPRECATED            0)
    SET (COMPILER_HAS_DEPRECATED_ATTR       0)
    SET (CMAKE_C_ABI_COMPILED               1)
    SET (CMAKE_C_COMPILER                   $ENV{__CCompiler})
    SET (CMAKE_C_COMPILER_ID                1)
    SET (CMAKE_C_COMPILER_ID_RUN            1)
    SET (CMAKE_C_COMPILER_WORKS             1)
endif ()

# Allow explicit specification of the Cpp Compiler
if (DEFINED ENV{__CXXCompiler})
    SET (COMPILER_HAS_DEPRECATED            0)
    SET (COMPILER_HAS_DEPRECATED_ATTR       0)
    SET (CMAKE_CXX_ABI_COMPILED             1)
    SET (CMAKE_CXX_COMPILER                 $ENV{__CXXCompiler})
    SET (CMAKE_CXX_COMPILER_ID              1)
    SET (CMAKE_CXX_COMPILER_ID_RUN          1)
    SET (CMAKE_CXX_COMPILER_WORKS           1)
endif ()

# CLR definitions
SET (CLR_DEFINES_DEBUG_INIT                 DEBUG _DEBUG _DBG URTBLDENV_FRIENDLY=Checked BUILDENV_CHECKED=1 WRITE_BARRIER_CHECK=1)
SET (CLR_DEFINES_RELEASE_INIT               NDEBUG URTBLDENV_FRIENDLY=Retail)
SET (CLR_DEFINES_RELWITHDEBINFO_INIT        NDEBUG URTBLDENV_FRIENDLY=Retail)

# Configuration of our libray specs and our directories
SET (CMAKE_INSTALL_PREFIX                   $ENV{__CMakeBinDir})
SET (CMAKE_INCLUDE_CURRENT_DIR              ON)
SET (CMAKE_SHARED_LIBRARY_PREFIX            "")

# Force an out of source build
if("${CMAKE_SOURCE_DIR}" STREQUAL "${CMAKE_BINARY_DIR}")
    message(FATAL_ERROR "Binary directory isn't being correctly set before calling Cmake. Tree must be built in separate directory from source.")
endif()

project(CoreFX)

if (CMAKE_SYSTEM_PROCESSOR STREQUAL x86_64 OR CMAKE_SYSTEM_PROCESSOR STREQUAL amd64)
    add_definitions(-DBIT64=1)
elseif (CMAKE_SYSTEM_PROCESSOR STREQUAL armv7l)
    add_definitions(-DBIT32=1)
    # Because we don't use CMAKE_C_COMPILER/CMAKE_CXX_COMPILER to use clang
    # we have to set the triple by adding a compiler argument
    add_compile_options(-target armv7-linux-gnueabihf)
    add_compile_options(-mthumb)
    add_compile_options(-mfpu=vfpv3)
endif ()

string(TOUPPER ${CMAKE_BUILD_TYPE} UPPERCASE_CMAKE_BUILD_TYPE)
if (UPPERCASE_CMAKE_BUILD_TYPE STREQUAL DEBUG)
    add_definitions(-DDEBUG)
elseif (UPPERCASE_CMAKE_BUILD_TYPE STREQUAL RELEASE)
    add_definitions(-DNDEBUG)
else ()
    message(FATAL_ERROR "Unknown build type. Set CMAKE_BUILD_TYPE to DEBUG or RELEASE.")
endif ()

add_subdirectory(clrcompression)