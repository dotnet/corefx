@if "%_echo%" neq "on" echo off
rem
rem This file invokes cmake and generates the build system for windows.

set argC=0
for %%x in (%*) do Set /A argC+=1

if %argC% LSS 5 GOTO :USAGE
if %1=="/?" GOTO :USAGE

setlocal
set __sourceDir=%~dp0

:: VS 2015 is the minimum supported toolset
if "%__VSVersion%" == "vs2017" (
  set __VSString=15 2017
) else (
  set __VSString=14 2015
set "__OverrideScriptsFolderPath=%5"
set "__StrictToolVersionMatch=%6"

:: Set the target architecture to a format cmake understands. ANYCPU defaults to x64
if /i "%3" == "x86"     (set __VSString=%__VSString%)
if /i "%3" == "x64"     (set __VSString=%__VSString% Win64)
if /i "%3" == "arm"     (set __VSString=%__VSString% ARM)
if /i "%3" == "arm64"   (set __VSString=%__VSString% Win64)

if defined CMakePath goto DoGen

:: Get the path to CMake.
pushd "%__sourceDir%"
setlocal EnableDelayedExpansion

for /f "Tokens=* Delims=" %%x in ('powershell -NoProfile -ExecutionPolicy ByPass "& %__rootRepo%\tools-local\windows\probe-tool.ps1 cmake %__rootRepo% %__OverrideScriptsFolderPath% %__StrictToolVersionMatch%"') do set ProbeValue=!ProbeValue!%%x

:: Evaluate the output from probe-tool.ps1
if exist "%ProbeValue%" (
    set "CMakePath=%ProbeValue%"
    echo CMakePath=!CMakePath!
) else (
    echo "%ProbeValue%"
    EXIT /B 1
)

popd

:DoGen
"%CMakePath%" %__SDKVersion% "-DCMAKE_BUILD_TYPE=%CMAKE_BUILD_TYPE%" "-DCMAKE_INSTALL_PREFIX=%__CMakeBinDir%" -G "Visual Studio %__VSString%" -B. -H%1
endlocal
GOTO :DONE

:USAGE
  echo "Usage..."
  echo "gen-buildsys-win.bat <path to top level CMakeLists.txt> <VSVersion> <Target Architecture"
  echo "Specify the path to the top level CMake file - <ProjectK>/src/NDP"
  echo "Specify the VSVersion to be used - VS2015 or VS2017"
  echo "Specify the Target Architecture - x86, AnyCPU, ARM, or x64."
  EXIT /B 1

:DONE
  EXIT /B 0
