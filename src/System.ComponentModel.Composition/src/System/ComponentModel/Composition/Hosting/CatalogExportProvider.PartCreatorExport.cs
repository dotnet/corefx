// -----------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Primitives;
using System.ComponentModel.Composition.ReflectionModel;
using System.Linq;

namespace System.ComponentModel.Composition.Hosting
{
    public partial class CatalogExportProvider
    {
        internal class PartCreatorExport : FactoryExport
        {
            private readonly CatalogExportProvider _catalogExportProvider;

            public PartCreatorExport(CatalogExportProvider catalogExportProvider, ComposablePartDefinition partDefinition, ExportDefinition exportDefinition) : 
                base(partDefinition, exportDefinition)
            {
                this._catalogExportProvider = catalogExportProvider;
            }

            public override Export CreateExportProduct()
            {
                return new NonSharedCatalogExport(this._catalogExportProvider, this.UnderlyingPartDefinition, this.UnderlyingExportDefinition);
            }
        }
    }
}
