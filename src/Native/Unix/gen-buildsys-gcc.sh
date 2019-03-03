#!/usr/bin/env sh
#
# This file invokes cmake and generates the build system for gcc.
#

if [ $# -lt 4 ]
then
  echo "Usage..."
  echo "gen-buildsys-clang.sh <path to top level CMakeLists.txt> <GccMajorVersion> <GccMinorVersion> <Architecture> [build flavor] [cmakeargs]"
  echo "Specify the path to the top level CMake file - <ProjectK>/src/NDP"
  echo "Specify the gcc version to use, split into major and minor version"
  echo "Specify the target architecture."
  echo "Optionally specify the build configuration (flavor.) Defaults to DEBUG."
  echo "Optionally pass additional arguments to CMake call."
  exit 1
fi

# Locate gcc
gcc_prefix=""

if [ "$CROSSCOMPILE" = "1" ]; then
  # Locate gcc
  if [ -n "$TOOLCHAIN" ]; then
    gcc_prefix="$TOOLCHAIN-"
  fi
fi

# Set up the environment to be used for building with gcc.
if command -v "${gcc_prefix}gcc-$2.$3" > /dev/null
    then
        desired_gcc_version="-$2.$3"
elif command -v "${gcc_prefix}gcc$2$3" > /dev/null
    then
        desired_gcc_version="$2$3"
elif command -v "${gcc_prefix}gcc-$2$3" > /dev/null
    then
        desired_gcc_version="-$2$3"
elif command -v "${gcc_prefix}"gcc > /dev/null
    then
        desired_gcc_version=
else
    echo "Unable to find \"${gcc_prefix}\"gcc Compiler"
    exit 1
fi

if [ -z "$CC" ]; then
  CC="$(command -v "${gcc_prefix}"gcc"$desired_gcc_version")"
  export CC
fi

build_arch="$4"
buildtype=DEBUG
generator="Unix Makefiles"
__UnprocessedCMakeArgs=""

ITER=-1
for i in "$@"; do
    ITER=$((ITER + 1))
    if [ $ITER -lt 5 ]; then continue; fi
    upperI="$(echo "$i" | awk '{print toupper($0)}')"
    case $upperI in
      # Possible build types are DEBUG, CHECKED, RELEASE, RELWITHDEBINFO, MINSIZEREL.
      DEBUG | CHECKED | RELEASE | RELWITHDEBINFO | MINSIZEREL)
      buildtype=$upperI
      ;;
      NINJA)
      generator=Ninja
      ;;
      *)
      __UnprocessedCMakeArgs="${__UnprocessedCMakeArgs}${__UnprocessedCMakeArgs:+ }$i"
    esac
done

OS=$(uname)

locate_gcc_exec() {
  if command -v "$gcc_prefix$1$desired_gcc_version" > /dev/null 2>&1
  then
    command -v "$gcc_prefix$1$desired_gcc_version"
  elif command -v "$gcc_prefix$1" > /dev/null 2>&1
  then
    command -v "$gcc_prefix$1"
  else
    exit 1
  fi
}

if ! gcc_ar="$(locate_gcc_exec ar)"; then { echo "Unable to locate gcc-ar"; exit 1; } fi

if [ -z "$NM" ]; then
  if ! gcc_nm="$(locate_gcc_exec nm)"; then { echo "Unable to locate gcc-nm"; exit 1; } fi
else
  gcc_nm="$NM"
fi

if [ "$OS" = "Linux" ] || [ "$OS" = "FreeBSD" ] || [ "$OS" = "OpenBSD" ] || [ "$OS" = "NetBSD" ] || [ "$OS" = "SunOS" ]; then
  if [ -z "$OBJDUMP" ]; then
    if ! gcc_objdump="$(locate_gcc_exec objdump)"; then { echo "Unable to locate gcc-objdump"; exit 1; } fi
  else
    gcc_objdump="$OBJDUMP"
  fi
fi

if [ -z "$OBJCOPY" ]; then
  if ! gcc_objcopy="$(locate_gcc_exec objcopy)"; then { echo "Unable to locate gcc-objcopy"; exit 1; } fi
else
  gcc_objcopy="$OBJCOPY"
fi

if ! gcc_ranlib="$(locate_gcc_exec ranlib)"; then { echo "Unable to locate gcc-ranlib"; exit 1; } fi

cmake_extra_defines=
if [ -n "$LLDB_LIB_DIR" ]; then
    cmake_extra_defines="$cmake_extra_defines -DWITH_LLDB_LIBS=$LLDB_LIB_DIR"
fi
if [ -n "$LLDB_INCLUDE_DIR" ]; then
    cmake_extra_defines="$cmake_extra_defines -DWITH_LLDB_INCLUDES=$LLDB_INCLUDE_DIR"
fi
if [ "$CROSSCOMPILE" = "1" ]; then
    if [ -z "$ROOTFS_DIR" ]; then
        echo "ROOTFS_DIR not set for crosscompile"
        exit 1
    fi
    if [ -z "$CONFIG_DIR" ]; then
        CONFIG_DIR="$1/cross"
    fi
    export TARGET_BUILD_ARCH=$build_arch
    cmake_extra_defines="$cmake_extra_defines -C $CONFIG_DIR/tryrun.cmake"
    cmake_extra_defines="$cmake_extra_defines -DCMAKE_TOOLCHAIN_FILE=$CONFIG_DIR/toolchain.cmake"
fi
if [ "$build_arch" = "armel" ]; then
    cmake_extra_defines="$cmake_extra_defines -DARM_SOFTFP=1"
fi

cmake \
  -G "$generator" \
  "-DCMAKE_AR=$gcc_ar" \
  "-DCMAKE_NM=$gcc_nm" \
  "-DCMAKE_RANLIB=$gcc_ranlib" \
  "-DCMAKE_OBJCOPY=$gcc_objcopy" \
  "-DCMAKE_OBJDUMP=$gcc_objdump" \
  "-DCMAKE_BUILD_TYPE=$buildtype" \
  "-DCMAKE_EXPORT_COMPILE_COMMANDS=1 " \
  "$cmake_extra_defines" \
  "$__UnprocessedCMakeArgs" \
  "$1"
