run build-managed -binaries -- /p:"SkipTests=true" /p:"RunTests=false" /p:"RestoreNuGetPackages=true" /flp:"v=diag" %*
:/p:"BuildProjectReferences=false"