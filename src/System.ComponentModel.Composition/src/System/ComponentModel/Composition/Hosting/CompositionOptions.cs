// -----------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;

namespace System.ComponentModel.Composition.Hosting
{
    /// <summary>
    ///     Defines the Constructor settings for export providers.
    /// </summary>
    [Flags]
    public enum CompositionOptions
    {
        Default                = 0x0000,
        DisableSilentRejection = 0x0001,
        IsThreadSafe           = 0x0002,
        ExportCompositionService = 0x0004
    }
}
