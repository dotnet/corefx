// -----------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Primitives;
using System.ComponentModel.Composition.ReflectionModel;
using System.Linq;
using System.Threading;

namespace System.ComponentModel.Composition.Hosting
{
    public partial class CatalogExportProvider
    {
        internal class ScopeFactoryExport : FactoryExport
        {
            private readonly ScopeManager _scopeManager;
            private readonly CompositionScopeDefinition _catalog;

            internal ScopeFactoryExport(ScopeManager scopeManager, CompositionScopeDefinition catalog, ComposablePartDefinition partDefinition, ExportDefinition exportDefinition) :
                base(partDefinition, exportDefinition)
            {
                this._scopeManager = scopeManager;
                this._catalog = catalog;
            }

            public override Export CreateExportProduct()
            {
                return new ScopeCatalogExport(this);
            }

            private sealed class ScopeCatalogExport : Export, IDisposable
            {
                private readonly ScopeFactoryExport _scopeFactoryExport;
                private CompositionContainer _childContainer;
                private Export _export;
                private readonly object _lock = new object();

                public ScopeCatalogExport(ScopeFactoryExport scopeFactoryExport)
                {
                    this._scopeFactoryExport = scopeFactoryExport;
                }

                public override ExportDefinition Definition
                {
                    get
                    {
                        return this._scopeFactoryExport.UnderlyingExportDefinition;
                    }
                }

                protected override object GetExportedValueCore()
                {
                    if (this._export == null)
                    {
                        var childContainer = this._scopeFactoryExport._scopeManager.CreateChildContainer(this._scopeFactoryExport._catalog);

                        var export = childContainer.CatalogExportProvider.CreateExport(this._scopeFactoryExport.UnderlyingPartDefinition, this._scopeFactoryExport.UnderlyingExportDefinition, false, CreationPolicy.Any);
                        lock (this._lock)
                        {
                            if (this._export == null)
                            {
                                this._childContainer = childContainer;
                                Thread.MemoryBarrier();
                                this._export = export;

                                childContainer = null;
                                export = null;
                            }
                        }
                        if (childContainer != null)
                        {
                            childContainer.Dispose();
                        }
                    }

                    return this._export.Value;
                }

                public void Dispose()
                {
                    CompositionContainer childContainer = null;
                    Export export = null;

                    if (this._export != null)
                    {
                        lock (this._lock)
                        {
                            export = this._export;
                            childContainer = this._childContainer;

                            this._childContainer = null;
                            Thread.MemoryBarrier();
                            this._export = null;
                        }
                    }

                    if(childContainer != null)
                    {
                        childContainer.Dispose();
                    }
                }
            }
        }
    }
}
