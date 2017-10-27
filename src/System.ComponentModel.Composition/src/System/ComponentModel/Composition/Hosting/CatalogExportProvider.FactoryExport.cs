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
        internal abstract class FactoryExport : Export
        {
            private readonly ComposablePartDefinition _partDefinition;
            private readonly ExportDefinition _exportDefinition;
            private ExportDefinition _factoryExportDefinition;
            private FactoryExportPartDefinition _factoryExportPartDefinition;

            public FactoryExport(ComposablePartDefinition partDefinition, ExportDefinition exportDefinition)
            {
                this._partDefinition = partDefinition;
                this._exportDefinition = exportDefinition;
                this._factoryExportDefinition = new PartCreatorExportDefinition(this._exportDefinition);
            }

            public override ExportDefinition Definition
            {
                get { return this._factoryExportDefinition; }
            }

            protected override object GetExportedValueCore()
            {
                if (this._factoryExportPartDefinition == null)
                {
                    this._factoryExportPartDefinition = new FactoryExportPartDefinition(this);
                }
                return this._factoryExportPartDefinition;
            }

            protected ComposablePartDefinition UnderlyingPartDefinition
            {
                get
                {
                    return this._partDefinition;
                }
            }

            protected ExportDefinition UnderlyingExportDefinition
            {
                get
                {
                    return this._exportDefinition;
                }
            }

            public abstract Export CreateExportProduct();

            private class FactoryExportPartDefinition : ComposablePartDefinition
            {
                private readonly FactoryExport _FactoryExport;

                public FactoryExportPartDefinition(FactoryExport FactoryExport)
                {
                    this._FactoryExport = FactoryExport;
                }

                public override IEnumerable<ExportDefinition> ExportDefinitions
                {
                    get { return new ExportDefinition[] { this._FactoryExport.Definition }; }
                }

                public override IEnumerable<ImportDefinition> ImportDefinitions
                {
                    get { return Enumerable.Empty<ImportDefinition>(); }
                }

                public ExportDefinition FactoryExportDefinition
                {
                    get { return this._FactoryExport.Definition; }
                }

                public Export CreateProductExport()
                {
                    return this._FactoryExport.CreateExportProduct();
                }

                public override ComposablePart CreatePart()
                {
                    return new FactoryExportPart(this);
                }
            }

            private sealed class FactoryExportPart : ComposablePart, IDisposable
            {
                private readonly FactoryExportPartDefinition _definition;
                private readonly Export _export;

                public FactoryExportPart(FactoryExportPartDefinition definition)
                {
                    this._definition = definition;
                    this._export = definition.CreateProductExport();
                }

                public override IEnumerable<ExportDefinition> ExportDefinitions
                {
                    get { return this._definition.ExportDefinitions; }
                }

                public override IEnumerable<ImportDefinition> ImportDefinitions
                {
                    get { return this._definition.ImportDefinitions; }
                }

                public override object GetExportedValue(ExportDefinition definition)
                {
                    if (definition != this._definition.FactoryExportDefinition)
                    {
                        throw ExceptionBuilder.CreateExportDefinitionNotOnThisComposablePart("definition");
                    }

                    return this._export.Value;
                }

                public override void SetImport(ImportDefinition definition, IEnumerable<Export> exports)
                {
                    throw ExceptionBuilder.CreateImportDefinitionNotOnThisComposablePart("definition");
                }

                public void Dispose()
                {
                    IDisposable disposable = this._export as IDisposable;

                    if (disposable != null)
                    {
                        disposable.Dispose();
                    }
                }
            }
        }
    }
}
