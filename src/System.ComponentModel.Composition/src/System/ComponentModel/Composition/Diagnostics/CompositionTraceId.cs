// -----------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;

namespace System.ComponentModel.Composition.Diagnostics
{
    // NOTE: Do not change the trace ids of values that have already shipped, 
    // these leak out to TraceListerners which could take a dependency on them.
    // This enum is a ushort deliberately, the maximum value of a trace id is 65535.
    internal enum CompositionTraceId : ushort
    {
        // Rejection

        Rejection_DefinitionRejected = 1,
        Rejection_DefinitionResurrected = 2,

        Discovery_AssemblyLoadFailed = 3,
        Discovery_DefinitionMarkedWithPartNotDiscoverableAttribute = 4,
        Discovery_DefinitionMismatchedExportArity = 5,
        Discovery_DefinitionContainsNoExports = 6,
        Discovery_MemberMarkedWithMultipleImportAndImportMany = 7,
    }
}
