// -----------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;

namespace System.ComponentModel.Composition.Primitives
{
    internal interface IPartCreatorImportDefinition
    {
        ContractBasedImportDefinition ProductImportDefinition { get; }
    }
}
