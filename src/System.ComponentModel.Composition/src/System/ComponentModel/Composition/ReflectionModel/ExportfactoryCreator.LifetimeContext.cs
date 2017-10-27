// -----------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.Linq;
using System.Reflection;
using System.Threading;
using Microsoft.Internal;

namespace System.ComponentModel.Composition.ReflectionModel
{
    internal sealed partial class ExportFactoryCreator
    {
        private class LifetimeContext
        {
            static Type[] types = { typeof(ComposablePartDefinition) };

            public Tuple<T, Action> GetExportLifetimeContextFromExport<T>(Export export)
            {
                T exportedValue;
                Action disposeAction;
                IDisposable disposable = null;

                CatalogExportProvider.ScopeFactoryExport scopeFactoryExport = export as CatalogExportProvider.ScopeFactoryExport;

                if (scopeFactoryExport != null)
                {
                    // Scoped PartCreatorExport
                    Export exportProduct = scopeFactoryExport.CreateExportProduct();
                    exportedValue = ExportServices.GetCastedExportedValue<T>(exportProduct);
                    disposable = exportProduct as IDisposable;
                }
                else
                {
                    CatalogExportProvider.FactoryExport factoryExport = export as CatalogExportProvider.FactoryExport;

                    if (factoryExport != null)
                    {
                        // PartCreatorExport is the more optimized route
                        Export exportProduct = factoryExport.CreateExportProduct();
                        exportedValue = ExportServices.GetCastedExportedValue<T>(exportProduct);
                        disposable = exportProduct as IDisposable;
                    }
                    else
                    {
                        // If it comes from somewhere else we walk through the ComposablePartDefinition
                        var factoryPartDefinition = ExportServices.GetCastedExportedValue<ComposablePartDefinition>(export);
                        var part = factoryPartDefinition.CreatePart();
                        var exportDef = factoryPartDefinition.ExportDefinitions.Single();

                        exportedValue = ExportServices.CastExportedValue<T>(part.ToElement(), part.GetExportedValue(exportDef));
                        disposable = part as IDisposable;
                    }
                }

                if (disposable != null)
                {
                    disposeAction = () => disposable.Dispose();
                }
                else
                {
                    disposeAction = () => { };
                }

                return new Tuple<T, Action>(exportedValue, disposeAction);
            }
        }
    }
}
