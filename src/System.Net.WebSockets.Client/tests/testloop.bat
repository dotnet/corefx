SETLOCAL EnableDelayedExpansion
SETLOCAL ENABLEEXTENSIONS

for /L %%x in (1,1,500) do (

mkdir "D:/TestDiag/Run %%x"
msbuild /t:rebuildandtest > "D:/TestDiag/Run %%x/output.log"

IF NOT ERRORLEVEL 1 (
rmdir "D:/TestDiag/Run %%x/" /s /q
) ELSE (
timeout /T 5
)
)
