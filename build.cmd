@call %~dp0build-native.cmd %*
@if NOT [%ERRORLEVEL%]==[0] exit /b 1
@call %~dp0build-managed.cmd %*
@exit /b %ERRORLEVEL%