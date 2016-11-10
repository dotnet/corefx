call init-tools.cmd
call msbuild /t:BatchRestorePackages /v:m /m
call msbuild src\src.builds /v:m /m
call msbuild src\packages.builds /v:m /m /clp:Summary
exit /b 0
