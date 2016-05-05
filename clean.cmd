@if "%_echo%" neq "on" echo off
setlocal EnableDelayedExpansion

set cleanlog=%~dp0clean.log
echo Running Clean.cmd %* > %cleanlog%

set options=/nologo /v:minimal /clp:Summary /flp:v=detailed;Append;LogFile=%cleanlog%
set unprocessedBuildArgs=
set allargs=%*
set thisArgs=
set clean_successful=true

if [%1] == [] (
  set clean_targets=Clean;
  goto Begin
)

set clean_targets=
set clean_src=
set clean_tools=
set clean_environment=
set clean_all=

:Loop
if [%1] == [] goto Begin

if /I [%1] == [/?] goto Usage

if /I [%1] == [/b] (
  set clean_targets=Clean;%clean_targets%
  set thisArgs=!thisArgs!%1
  goto Next
)

if /I [%1] == [/p] (
  set clean_targets=CleanPackages;%clean_targets%
  set thisArgs=!thisArgs!%1
  goto Next
)

if /I [%1] == [/c] (
  set clean_targets=CleanPackagesCache;%clean_targets%
  set thisArgs=!thisArgs!%1
  goto Next
)

if /I [%1] == [/s] (
  set clean_src=true
  set thisArgs=!thisArgs!%1
  goto Next
)

if /I [%1] == [/t] (
  set clean_tools=true
  set thisArgs=!thisArgs!%1
  goto Next
)

if /I [%1] == [/e] (
  set clean_environment=true
  set thisArgs=!thisArgs!%1
  goto Next
)

if /I [%1] == [/all] (
  set clean_src=
  set clean_tools=
  set clean_environment=
  set clean_targets=Clean;CleanPackages;CleanPackagesCache;
  set clean_all=true
  set thisArgs=!thisArgs!%1
  goto Next
)

if [!thisArgs!]==[] (
  set unprocessedBuildArgs=!allargs!
) else (
  call set unprocessedBuildArgs=%%allargs:*!thisArgs!=%%
)

:Next
shift /1
goto Loop

:Begin
if /I [%clean_environment%] == [true] (
  call :CleanEnvironment
)

if /I [%clean_src%] == [true] (
  echo Cleaning src directory ...
  echo. >> %cleanlog% && echo git clean -xdf %~dp0src >> %cleanlog%
  call git clean -xdf %~dp0src >> %cleanlog%
  call :CheckErrorLevel
)

if NOT "%clean_targets%" == "" (
  echo Running init-tools.cmd
  call %~dp0init-tools.cmd
  
  echo Running msbuild clean targets "%clean_targets:~0,-1%" ...
  echo. >> %cleanlog% && echo msbuild.exe %~dp0build.proj /t:%clean_targets:~0,-1% !options! !unprocessedBuildArgs! >> %cleanlog%
  call msbuild.exe %~dp0build.proj /t:%clean_targets:~0,-1% !options! !unprocessedBuildArgs!
  call :CheckErrorLevel
)

if /I [%clean_tools%] == [true] (
  call :CleanEnvironment
  echo Cleaning tools directory ...
  echo. >> %cleanlog% && echo rmdir /s /q %~dp0tools >> %cleanlog%
  rmdir /s /q %~dp0tools >> %cleanlog%
  REM Don't call CheckErrorLevel because if the Tools directory didn't exist when this script was
  REM invoked, then it sometimes exits with error level 3 despite successfully deleting the directory.
)

if /I [%clean_all%] == [true] (
  call :CleanEnvironment
  echo Cleaning entire working directory ...
  echo. >> %cleanlog% && echo git clean -xdf -e clean.log %~dp0 >> %cleanlog%
  call git clean -xdf -e clean.log %~dp0 >> %cleanlog%
  call :CheckErrorLevel
)

if /I [%clean_successful%] == [true] (
  echo Clean completed successfully.
  echo. >> %cleanlog% && echo Clean completed successfully. >> %cleanlog%
  exit /b 0
) else (
  echo An error occured while cleaning; see %cleanlog% for more details.
  echo. >> %cleanlog% && echo Clean completed with errors. >> %cleanlog%
  exit /b 1
)

:Usage
echo.
echo Repository cleaning script.
echo.
echo Options:
echo     /b     - Deletes the binary output directory.
echo     /p     - Deletes the repo-local nuget package directory.
echo     /c     - Deletes the user-local nuget package cache.
echo     /t     - Deletes the tools directory.
echo     /s     - Deletes the untracked files under src directory (git clean src -xdf).
echo     /e     - Kills and/or stops the processes that are still running, for example VBCSCompiler.exe
echo     /all   - Combines all of the above.
echo.
echo If no option is specified then clean.cmd /b is implied.

exit /b 1

:CheckErrorLevel
if NOT [%ERRORLEVEL%]==[0] (
  echo Command exited with ERRORLEVEL %ERRORLEVEL% >> %cleanlog%
  set clean_successful=false
)
exit /b

:CleanEnvironment
REM If VBCSCompiler.exe is running we need to kill it
echo Stop VBCSCompiler.exe execution.
echo. >> %cleanlog% && echo Stop VBCSCompiler.exe execution. >> %cleanlog% 
for /f "tokens=2 delims=," %%F in ('tasklist /nh /fi "imagename eq VBCSCompiler.exe" /fo csv') do taskkill /f /PID %%~F >> %cleanlog%
exit /b