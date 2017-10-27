// -----------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;

namespace System.ComponentModel.Composition
{
    internal enum ExportCardinalityCheckResult : int
    {
        Match,
        NoExports,
        TooManyExports
    }
}
