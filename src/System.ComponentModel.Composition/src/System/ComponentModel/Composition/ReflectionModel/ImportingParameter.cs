// -----------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.ComponentModel.Composition.Primitives;

namespace System.ComponentModel.Composition.ReflectionModel
{
    internal class ImportingParameter : ImportingItem
    {
        public ImportingParameter(ContractBasedImportDefinition definition, ImportType importType)
            : base(definition, importType)
        {
        }
    }
}
