@if not defined _echo @echo off
setlocal

call %~dp0sync.cmd
if NOT [%ERRORLEVEL%]==[0] exit /b 1
call %~dp0build-managed.cmd -Project:%~dp0src\SharedFrameworkValidation\SharedFrameworkValidation.proj -- /t:CreateScriptToDownloadSharedFrameworkZip
if NOT [%ERRORLEVEL%]==[0] exit /b 1
powershell -NoProfile -ExecutionPolicy unrestricted -Command "%~dp0bin\DownloadSharedFramework.ps1"
call %~dp0build-managed.cmd -Project:%~dp0src\SharedFrameworkValidation\SharedFrameworkValidation.proj
if NOT [%ERRORLEVEL%]==[0] exit /b 1
call %~dp0bin\CloneAndRunTests.cmd
exit /b %ERRORLEVEL%
