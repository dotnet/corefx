@if "%_echo%" neq "on" echo off
rem
rem This file invokes cmake and generates the build system for windows.

set argC=0
for %%x in (%*) do Set /A argC+=1

if NOT %argC%==3 GOTO :USAGE
if %1=="/?" GOTO :USAGE

setlocal EnableDelayedExpansion
set __sourceDir=%~dp0
set __ExtraCmakeParams=

:: VS 2017 is the minimum supported toolset
if "%__VSVersion%" == "vs2019" (
  set __VSString=16 2019

  :: VS 2019 generator is not per arch, and the arch needs to be passed with -A option
  :: x86 is the default arch selected by cmake.
  :: CMAKE 3.14 or later is required to use VS2019
  if /i "%3" == "x64"     (set __ExtraCmakeParams=%__ExtraCmakeParams% -A x64)
  if /i "%3" == "arm"     (set __ExtraCmakeParams=%__ExtraCmakeParams% -A ARM)
) else (
  set __VSString=15 2017

  :: VS2017 generator needs to be specified with the arch as part of its name.
  :: x86 is the default arch selected by cmake.
  if /i "%3" == "x64"     (set __VSString=!__VSString! Win64)
  if /i "%3" == "arm"     (set __VSString=!__VSString! ARM)
)

:: arm64 doesn't have a specific generator so we need to pass it through -A option.
if /i "%3" == "arm64"   (set __ExtraCmakeParams=%__ExtraCmakeParams% -A ARM64)
if /i "%3" == "wasm"    (set __sourceDir=%~dp0..\Unix && goto DoGen)

if defined CMakePath goto DoGen

:: Eval the output from probe-win1.ps1
pushd "%__sourceDir%"
for /f "delims=" %%a in ('powershell -NoProfile -ExecutionPolicy ByPass "& .\probe-win.ps1"') do %%a
popd

:DoGen

if "%3" == "wasm" (
    emcmake cmake "-DEMSCRIPTEN_GENERATE_BITCODE_STATIC_LIBRARIES=1" "-DCMAKE_TOOLCHAIN_FILE=%EMSCRIPTEN%/cmake/Modules/Platform/Emscripten.cmake" "-DCMAKE_BUILD_TYPE=%CMAKE_BUILD_TYPE%" -G "NMake Makefiles" %__sourceDir%
) else (
    "%CMakePath%" %__SDKVersion% "-DCMAKE_BUILD_TYPE=%CMAKE_BUILD_TYPE%" "-DCMAKE_INSTALL_PREFIX=%__CMakeBinDir%" -G "Visual Studio %__VSString%" -B. -H%1 %__ExtraCmakeParams%
)
endlocal
GOTO :DONE

:USAGE
  echo "Usage..."
  echo "gen-buildsys-win.bat <path to top level CMakeLists.txt> <VSVersion> <Target Architecture>"
  echo "Specify the path to the top level CMake file"
  echo "Specify the VSVersion to be used - VS2017"
  echo "Specify the Target Architecture - AnyCPU, x86, x64, ARM, or ARM64."
  EXIT /B 1

:DONE
  EXIT /B 0
