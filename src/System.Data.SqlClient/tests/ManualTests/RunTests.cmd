@echo off
IF "%1"=="/?" (
ECHO Usage: "%0" [TestClass]
) ELSE IF "%1"=="" (
echo Running all tests.
msbuild /t:BuildAndTest /p:EnableManualTests=true
) ELSE (
echo Running SqlClient test suite "%1".
msbuild /t:BuildAndTest /p:EnableManualTests=true "/p:XunitOptions=-class System.Data.SqlClient.ManualTesting.Tests.%1"
)
