@ECHO OFF
SETLOCAL

IF "%ASPNETCORE_REPO%" == "" (
  echo The 'ASPNETCORE_REPO' environment variable is not set, aborting.
  exit /b 1
)

echo ASPNETCORE_REPO: %ASPNETCORE_REPO%

robocopy . %ASPNETCORE_REPO%\src\Shared\Http2 /MIR