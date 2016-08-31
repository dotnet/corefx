@setlocal
@if [%1]==[] set __args=-p
@call %~dp0..\..\run.cmd sync %__args% %*
@exit /b %ERRORLEVEL%