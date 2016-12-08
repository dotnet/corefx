run build-managed -binaries -- /p:"DebugSymbols=true" /p:"SkipImportCodeAnalysisTargets=true" /p:"SkipTests=true" /p:"RunTests=false" /p:"RestoreNuGetPackages=true" /flp:"v=diag" %*
