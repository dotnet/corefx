// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Globalization;
using System.Diagnostics.CodeAnalysis;
using System.ComponentModel.Composition.Primitives;
using System.ComponentModel.Composition.Adaptation;
using Microsoft.Internal;
using System.ComponentModel.Composition.ReflectionModel;

namespace System.ComponentModel.Composition.Hosting
{
    partial class AdaptingExportProvider
    {
        // Represents an adapter method that adapts from one contract to another
        private class AdapterDefinition
        {
            private Func<Export, Export> _adaptMethod;
            private readonly string _fromContractName;
            private readonly string _toContractName;
            private readonly Export _export;

            public AdapterDefinition(Export export)
            {
                Assumes.NotNull(export);

                this._export = export;
                this._fromContractName = this.GetContractName(AdaptationConstants.AdapterFromContractMetadataName);
                this._toContractName = this.GetContractName(AdaptationConstants.AdapterToContractMetadataName);
            }

            public Export Export
            {
                get { return _export; }
            }

            private string GetExportDisplayName()
            {
                ICompositionElement element = this.Export as ICompositionElement;
                if (element != null)
                {
                    return element.DisplayName;
                }
                else
                {
                    return this.Export.ToString();
                }
            }

            public string FromContractName
            {
                get { return _fromContractName; }                
            }

            public string ToContractName
            {
                get { return _toContractName; }                
            }

            [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
            public Export Adapt(Export export)
            {
                Assumes.NotNull(_adaptMethod, export);

                Export adaptedExport = null;

                try
                {
                    adaptedExport = this._adaptMethod(export);
                }
                catch (Exception exception)
                {   // Adapter threw an exception. Avoid letting this 
                    // leak out as a 'raw' unhandled exception, instead,
                    // we'll add some context and rethrow.

                    throw new CompositionException(new CompositionError[]
                    {
                        new CompositionError(string.Format(
                            CultureInfo.CurrentCulture,
                            Strings.Adapter_ExceptionDuringAdapt,
                            this.GetExportDisplayName(),
                            this.ToContractName,
                            this.FromContractName), exception)
                    });

                }

                CheckAdaptation(adaptedExport);

                return adaptedExport;
            }

            private void CheckAdaptation(Export adaptedExport)
            {
                // If an export was not provided, then the 
                // adapter could not adapt from the export.
                if (adaptedExport == null)
                {
                    return;
                }

                if (!StringComparer.Ordinal.Equals(adaptedExport.Definition.ContractName, ToContractName))
                {   // The returned export must match the adapter's 'To' contract name

                    throw new CompositionException(new CompositionError[]
                    {
                        new CompositionError(string.Format(
                            CultureInfo.CurrentCulture,
                            Strings.Adapter_ContractMismatch, 
                            this.GetExportDisplayName(), 
                            this.ToContractName,
                            adaptedExport.Definition.ContractName))
                    });
                }
            }

            private string GetContractName(string name)
            {
                // Similar to the ExportAttribute, we allow the contract name to be 
                // adapted to/from to be either specified as a type or as a string
                object contractObject = null;
                string contractName = null;
                if (this.Export.Metadata.TryGetValue(name, out contractObject))
                {
                    Type contractType = contractObject as Type;
                    if (contractType != null)
                    {
                        contractName = AttributedModelServices.GetContractName(contractType);
                    }
                    else
                    {
                        contractName = contractObject as string;
                    }
                }
                return contractName;
            }

            public bool CanAdaptFrom(string contractName)
            {
                // We have missing to/from contract names, say we can adapt, 
                // so that we throw an error during adaption alerting the user
                // of the fact.
                if (HasNullOrEmptyContracts())
                {
                    return true;
                }

                return StringComparer.Ordinal.Equals(ToContractName, contractName);
            }

            public void EnsureWellFormedAdapter()
            {
                if (HasNullOrEmptyContracts())
                {   // Null or empty 'from' or 'to' contract

                    throw new CompositionException(new CompositionError[]
                    {
                        new CompositionError(string.Format(CultureInfo.CurrentCulture, Strings.Adapter_CannotAdaptNullOrEmptyFromOrToContract, this.GetExportDisplayName(), this.ToContractName))
                    });
                }

                if (StringComparer.Ordinal.Equals(this.ToContractName, this.FromContractName))
                {   // 'From' and 'to' contracts match

                    throw new CompositionException(new CompositionError[]
                    {
                        new CompositionError(string.Format(CultureInfo.CurrentCulture, Strings.Adapter_CannotAdaptFromAndToSameContract, this.GetExportDisplayName(), this.ToContractName))
                    });
                }

                if (_adaptMethod == null)
                {
                    _adaptMethod = GetAdapterMethod();
                }
            }

            private Func<Export, Export> GetAdapterMethod()
            {
                // TODO: If the predictive composition feature
                // does not get implemented and hence this continues 
                // to throw, add additional context here.
                object value = this.Export.Value;

                ExportedDelegate exportedDelegate = value as ExportedDelegate;
                if (exportedDelegate != null)
                {
                    value = exportedDelegate.CreateDelegate(typeof(Func<Export, Export>));
                }

                Func<Export, Export> method = value as Func<Export, Export>;
                if (method == null)
                {
                    throw new CompositionException(new CompositionError[]
                    {
                        new CompositionError(string.Format(
                                CultureInfo.CurrentCulture,
                                value == null ? Strings.Adapter_TypeNull : Strings.Adapter_TypeMismatch,
                                this.GetExportDisplayName(),
                                value == null ? null : value.GetType().FullName,
                                typeof(Func<Export, Export>).FullName
                            ))
                    });
                }

                return method;
            }

            private bool HasNullOrEmptyContracts()
            {
                return String.IsNullOrEmpty(this.ToContractName) || String.IsNullOrEmpty(this.FromContractName);
            }
        }
    }
}
