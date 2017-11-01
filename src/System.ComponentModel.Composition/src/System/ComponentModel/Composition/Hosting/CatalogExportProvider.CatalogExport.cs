// -----------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.ComponentModel.Composition.Primitives;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Internal;
using System.Threading;

namespace System.ComponentModel.Composition.Hosting
{
    public partial class CatalogExportProvider
    {
        private class CatalogExport : Export
        {
            protected readonly CatalogExportProvider _catalogExportProvider;
            protected readonly ComposablePartDefinition _partDefinition;
            protected readonly ExportDefinition _definition;

            public CatalogExport(CatalogExportProvider catalogExportProvider,
                ComposablePartDefinition partDefinition, ExportDefinition definition)
            {
                this._catalogExportProvider = catalogExportProvider;
                this._partDefinition = partDefinition;
                this._definition = definition;
            }

            public override ExportDefinition Definition
            {
                get
                {
                    return this._definition;
                }
            }

            protected virtual bool IsSharedPart
            {
                get
                {
                    return true;
                }
            }

            protected CatalogPart GetPartCore()
            {
                return this._catalogExportProvider.GetComposablePart(this._partDefinition, this.IsSharedPart);
            }

            protected void DisposePartCore(CatalogPart part, object value)
            {
                this._catalogExportProvider.DisposePart(value, part, null);
            }

            protected virtual CatalogPart GetPart()
            {
                return this.GetPartCore();
            }

            protected override object GetExportedValueCore()
            {
                return this._catalogExportProvider.GetExportedValue(this.GetPart(), this._definition, this.IsSharedPart);
            }

            [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
            public static CatalogExport CreateExport(CatalogExportProvider catalogExportProvider,
                ComposablePartDefinition partDefinition, ExportDefinition definition, CreationPolicy importCreationPolicy)
            {
                CreationPolicy partPolicy = partDefinition.Metadata.GetValue<CreationPolicy>(CompositionConstants.PartCreationPolicyMetadataName);
                bool isSharedPart = ShouldUseSharedPart(partPolicy, importCreationPolicy);

                if (isSharedPart)
                {
                    return new CatalogExport(catalogExportProvider, partDefinition, definition);
                }
                else
                {
                    return new NonSharedCatalogExport(catalogExportProvider, partDefinition, definition);
                }
            }

            private static bool ShouldUseSharedPart(CreationPolicy partPolicy, CreationPolicy importPolicy)
            {
                // Matrix that details which policy to use for a given part to satisfy a given import.
                //                   Part.Any   Part.Shared  Part.NonShared
                // Import.Any        Shared     Shared       NonShared
                // Import.Shared     Shared     Shared       N/A
                // Import.NonShared  NonShared  N/A          NonShared

                switch (partPolicy)
                {
                    case CreationPolicy.Any:
                        {
                            if (importPolicy == CreationPolicy.Any ||
                                importPolicy == CreationPolicy.Shared)
                            {
                                return true;
                            }
                            return false;
                        }

                    case CreationPolicy.NonShared:
                        {
                            Assumes.IsTrue(importPolicy != CreationPolicy.Shared);
                            return false;
                        }

                    default:
                        {
                            Assumes.IsTrue(partPolicy == CreationPolicy.Shared);
                            Assumes.IsTrue(importPolicy != CreationPolicy.NonShared);
                            return true;
                        }
                }
            }
        }

        private sealed class NonSharedCatalogExport : CatalogExport, IDisposable
        {
            private CatalogPart _part;
            private readonly object _lock = new object();

            public NonSharedCatalogExport(CatalogExportProvider catalogExportProvider,
                ComposablePartDefinition partDefinition, ExportDefinition definition)
                : base(catalogExportProvider, partDefinition, definition)
            {
            }

            protected override CatalogPart GetPart()
            {
                // we need to ensure that the part gets created only once, as the export contract requires that the same value be returned on subsequent calls
                if (this._part == null)
                {
                    CatalogPart part = this.GetPartCore();

                    lock (this._lock)
                    {
                        if (this._part == null)
                        {
                            Thread.MemoryBarrier();
                            this._part = part;
                            part = null;
                        }
                    }

                    if (part != null)
                    {
                        this.DisposePartCore(part, null);
                    }
                }

                return this._part;
            }

            protected override bool IsSharedPart
            {
                get
                {
                    return false;
                }
            }

            void IDisposable.Dispose()
            {
                if (this._part != null)
                {
                    this.DisposePartCore(this._part, this.Value);
                    this._part = null;
                }
            }
        }
    }
}
