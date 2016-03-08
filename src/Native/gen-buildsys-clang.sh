#!/usr/bin/env bash
#
# This file invokes cmake and generates the build system for Clang.
#

if [ $# -lt 4 -o $# -gt 6 ]
then
  echo "Usage..."
  echo "gen-buildsys-clang.sh <path to top level> <ClangMajorVersion> <ClangMinorVersion> <Architecture> [build flavor] [cmakeargs]"
  echo "Specify the path to the top level folder - <corefx>"
  echo "Specify the clang version to use, split into major and minor version"
  echo "Specify the target architecture." 
  echo "Optionally specify the build configuration (flavor.) Defaults to DEBUG." 
  echo "Optionally pass additional arguments to CMake call."
  exit 1
fi

# Set up the environment to be used for building with clang.
if which "clang-$2.$3" > /dev/null 2>&1
    then
        export CC="$(which clang-$2.$3)"
        export CXX="$(which clang++-$2.$3)"
elif which "clang$2$3" > /dev/null 2>&1
    then
        export CC="$(which clang$2$3)"
        export CXX="$(which clang++$2$3)"
elif which clang > /dev/null 2>&1
    then
        export CC="$(which clang)"
        export CXX="$(which clang++)"
else
    echo "Unable to find Clang Compiler"
    exit 1
fi

build_arch="$4"
if [ -z "$5" ]; then
    echo "Defaulting to DEBUG build."
    build_type="DEBUG"
else
    # Possible build types are DEBUG, RELEASE
    build_type="$(echo $5 | awk '{print toupper($0)}')"
    if [ "$build_type" != "DEBUG" ] && [ "$build_type" != "RELEASE" ]; then
        echo "Invalid Build type, only debug or release is accepted."
        exit 1
    fi
fi

OS=`uname`

# Locate llvm
# This can be a little complicated, because the common use-case of Ubuntu with
# llvm-3.5 installed uses a rather unusual llvm installation with the version
# number postfixed (i.e. llvm-ar-3.5), so we check for that first.
# On FreeBSD the version number is appended without point and dash (i.e.
# llvm-ar35).
# Additionally, OSX doesn't use the llvm- prefix.
if [ $OS = "Linux" -o $OS = "FreeBSD" -o $OS = "OpenBSD" -o $OS = "NetBSD" ]; then
  llvm_prefix="llvm-"
elif [ $OS = "Darwin" ]; then
  llvm_prefix=""
else
  echo "Unable to determine build platform"
  exit 1
fi

desired_llvm_major_version=$2
desired_llvm_minor_version=$3
if [ $OS = "FreeBSD" ]; then
    desired_llvm_version="$desired_llvm_major_version$desired_llvm_minor_version"
elif [ $OS = "OpenBSD" ]; then
    desired_llvm_version=""
elif [ $OS = "NetBSD" ]; then
    desired_llvm_version=""
else
  desired_llvm_version="-$desired_llvm_major_version.$desired_llvm_minor_version"
fi
locate_llvm_exec() {
  if which "$llvm_prefix$1$desired_llvm_version" > /dev/null 2>&1
  then
    echo "$(which $llvm_prefix$1$desired_llvm_version)"
  elif which "$llvm_prefix$1" > /dev/null 2>&1
  then
    echo "$(which $llvm_prefix$1)"
  else
    exit 1
  fi
}

llvm_ar="$(locate_llvm_exec ar)"
[[ $? -eq 0 ]] || { echo "Unable to locate llvm-ar"; exit 1; }
llvm_link="$(locate_llvm_exec link)"
[[ $? -eq 0 ]] || { echo "Unable to locate llvm-link"; exit 1; }
llvm_nm="$(locate_llvm_exec nm)"
[[ $? -eq 0 ]] || { echo "Unable to locate llvm-nm"; exit 1; }
if [ $OS = "Linux" -o $OS = "FreeBSD" -o $OS = "OpenBSD" -o $OS = "NetBSD" ]; then
  llvm_objdump="$(locate_llvm_exec objdump)"
  [[ $? -eq 0 ]] || { echo "Unable to locate llvm-objdump"; exit 1; }
fi

cmake_extra_defines=
if [[ -n "$LLDB_LIB_DIR" ]]; then
    cmake_extra_defines="$cmake_extra_defines -DWITH_LLDB_LIBS=$LLDB_LIB_DIR"
fi
if [[ -n "$LLDB_INCLUDE_DIR" ]]; then
    cmake_extra_defines="$cmake_extra_defines -DWITH_LLDB_INCLUDES=$LLDB_INCLUDE_DIR"
fi
if [[ -n "$CROSSCOMPILE" ]]; then
    if ! [[ -n "$ROOTFS_DIR" ]]; then
        echo "ROOTFS_DIR not set for crosscompile"
        exit 1
    fi
    cmake_extra_defines="$cmake_extra_defines -C $1/cross/$build_arch/tryrun.cmake"
    cmake_extra_defines="$cmake_extra_defines -DCMAKE_TOOLCHAIN_FILE=$1/cross/$build_arch/toolchain.cmake"
fi

__UnprocessedCMakeArgs=""
if [ -z "$6" ]; then
    echo "No CMake extra Args specified"
else
    __UnprocessedCMakeArgs="$6"
fi

cmake \
  "-DCMAKE_AR=$llvm_ar" \
  "-DCMAKE_LINKER=$llvm_link" \
  "-DCMAKE_NM=$llvm_nm" \
  "-DCMAKE_OBJDUMP=$llvm_objdump" \
  "-DCMAKE_RANLIB=$llvm_ranlib" \
  "-DCMAKE_BUILD_TYPE=$build_type" \
  $cmake_extra_defines \
  $__UnprocessedCMakeArgs \
  "$1/src/Native"
