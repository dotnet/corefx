@echo off
powershell -ExecutionPolicy ByPass -NoProfile %~dp0build.ps1 -inittools
powershell -ExecutionPolicy ByPass -NoProfile %~dp0common\msbuild.ps1 %~dp0..\build.proj %*
exit /b %ERRORLEVEL%
