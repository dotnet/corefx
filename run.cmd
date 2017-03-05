@if not defined _echo @echo off
setlocal

if not defined VisualStudioVersion (
  if defined VS150COMNTOOLS (
    call "%VS150COMNTOOLS%\VsDevCmd.bat"
    goto :Run
  ) else if defined VS140COMNTOOLS (
    call "%VS140COMNTOOLS%\VsDevCmd.bat"
    goto :Run
  )
  echo Error: Visual Studio 2015 or 2017 required.
  echo        Please see https://github.com/dotnet/corefx/blob/master/Documentation/project-docs/developer-guide.md for build instructions.
  exit /b 1
)

:Run
:: Clear the 'Platform' env variable for this session, as it's a per-project setting within the build, and
:: misleading value (such as 'MCD' in HP PCs) may lead to build breakage (issue: #69).
set Platform=

:: Restore the Tools directory
call %~dp0init-tools.cmd
if NOT [%ERRORLEVEL%]==[0] exit /b 1

:: Always copy over the Tools-Override
xcopy %~dp0Tools-Override\* %~dp0Tools /y >nul

set _toolRuntime=%~dp0Tools
set _dotnet=%_toolRuntime%\dotnetcli\dotnet.exe

call %_dotnet% %_toolRuntime%\run.exe %*
exit /b %ERRORLEVEL%
