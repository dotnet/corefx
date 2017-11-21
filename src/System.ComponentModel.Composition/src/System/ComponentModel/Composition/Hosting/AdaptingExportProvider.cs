// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Primitives;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.ComponentModel.Composition.Adaptation;
using Microsoft.Internal;
using Microsoft.Internal.Collections;

namespace System.ComponentModel.Composition.Hosting
{
    public partial class AdaptingExportProvider : ExportProvider, IDisposable
    {
        private static AdapterDefinition[] EmptyAdapterArray = new AdapterDefinition[0];

        private object _lock = new object();
        private volatile bool _isDisposed = false;
        private volatile bool _isRunning = false;
        private ExportProvider _sourceProvider;
        private AdapterDefinition[] _adapters;
        private static ImportDefinition _adapterImport = new ContractBasedImportDefinition(AdaptationConstants.AdapterContractName,
                                                         (string)null,
                                                         new Dictionary<string, Type> { 
                                                                        {AdaptationConstants.AdapterFromContractMetadataName, typeof(object)},
                                                                        {AdaptationConstants.AdapterToContractMetadataName, typeof(object)} },
                                                         ImportCardinality.ZeroOrMore,
                                                         true,                              /* Recomposable */
                                                         false,                            /* Prerequisite */
                                                         CreationPolicy.Any);
        /// <summary>
        ///     Initializes a new instance of the <see cref="AdaptingExportProvider"/> class.
        /// </summary>
        public AdaptingExportProvider()
        {
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (!this._isDisposed)
                {
                    ExportProvider sourceProviderToUnsubscribeFrom = null;
                    try
                    {
                        if (!this._isDisposed)
                        {
                            lock(this._lock)
                            {
                                sourceProviderToUnsubscribeFrom = this._sourceProvider;
                                this._sourceProvider = null;

                                this._isDisposed = true;
                            }
                        }
                    }
                    finally
                    {
                        if (sourceProviderToUnsubscribeFrom != null)
                        {
                            sourceProviderToUnsubscribeFrom.ExportsChanging -= this.OnExportsChangingInternal;
                        }
                    }
                }
            }
        }

        /// <summary>
        ///     Gets the export provider which provides the provider access to additional
        ///     exports.
        /// </summary>
        /// <value>
        ///     The <see cref="ExportProvider"/> which provides the 
        ///     <see cref="AdaptingExportProvider"/> access to additional
        ///     <see cref="Export"/> objects. The default is <see langword="null"/>.
        /// </value>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="value"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        ///     This property has already been set.
        ///     <para>
        ///         -or-
        ///     </para>
        ///     The methods on the <see cref="AdaptingExportProvider"/> 
        ///     have already been accessed.
        /// </exception>
        /// <exception cref="ObjectDisposedException">
        ///     The <see cref="AdaptingExportProvider"/> has been disposed of.
        /// </exception>
        /// <remarks>
        ///     This property must be set before accessing any methods on the 
        ///     <see cref="AdaptingExportProvider"/>.
        /// </remarks>
        public ExportProvider SourceProvider
        {
            get
            {
                this.ThrowIfDisposed();

                return this._sourceProvider;
            }
            set
            {
                this.ThrowIfDisposed();

                Requires.NotNull(value, "value");
                lock(this._lock)
                {
                    this.EnsureCanSet(this._sourceProvider);
                    this._sourceProvider = value;
                }

                // This should be safe to do outside the lock, because only the first setter will ever win
                // and others will throw
                this._sourceProvider.ExportsChanged += this.OnExportsChangedInternal;
                this._sourceProvider.ExportsChanging += this.OnExportsChangingInternal;
            }
        }
              
        /// <summary>
        /// Returns all exports that match the conditions of the specified import.
        /// </summary>
        /// <param name="definition">The <see cref="ImportDefinition"/> that defines the conditions of the
        /// <see cref="Export"/> to get.</param>
        /// <returns></returns>
        /// <result>
        /// An <see cref="IEnumerable{T}"/> of <see cref="Export"/> objects that match
        /// the conditions defined by <see cref="ImportDefinition"/>, if found; otherwise, an
        /// empty <see cref="IEnumerable{T}"/>.
        /// </result>
        /// <remarks>
        /// 	<note type="inheritinfo">
        /// The implementers should not treat the cardinality-related mismatches as errors, and are not
        /// expected to throw exceptions in those cases.
        /// For instance, if the import requests exactly one export and the provider has no matching exports or more than one,
        /// it should return an empty <see cref="IEnumerable{T}"/> of <see cref="Export"/>.
        /// </note>
        /// </remarks>
        protected override IEnumerable<Export> GetExportsCore(ImportDefinition definition, AtomicComposition atomicComposition)
        {
            this.ThrowIfDisposed();
            this.EnsureRunning();

            IEnumerable<Export> exports;
            this.SourceProvider.TryGetExports(definition, atomicComposition, out exports);

            IEnumerable<AdapterDefinition> adapters = this.GetFilteredAdapters(definition, atomicComposition);
            if (adapters != null && adapters.Any())
            { // Avoid getting the constraint unless we actually need to
                exports = exports.Concat(GetAdaptedExports(adapters, definition.Cardinality, null, GetConstraint(definition), atomicComposition));
            }

            return exports;
        }

        private IEnumerable<Export> GetAdaptedExports(IEnumerable<AdapterDefinition> adapters, ImportCardinality cardinality,
                Func<ExportDefinition, bool> fromConstraint, Func<ExportDefinition, bool> toConstraint, AtomicComposition atomicComposition)
        {
            var result = CompositionResult.SucceededResult;
            var adaptedExports = new List<Export>();
            foreach (AdapterDefinition adapter in adapters)
            {
                // We validate the adapter every attempted usage of it, that
                // way if there is an error, then it is raised everytime 
                // someone attempts to request the contract name it adapts to
                result = result.MergeResult(AdaptationHelpers.TryInvoke(() => adapter.EnsureWellFormedAdapter()));
                if (result.Succeeded)
                {
                    CompositionResult<IEnumerable<Export>> adaptedExportsResult =
                        this.GetAdaptedExportsFromAdapter(adapter, cardinality, fromConstraint, toConstraint, atomicComposition);
                    result = result.MergeErrors(adaptedExportsResult.Errors);

                    if (adaptedExportsResult.Succeeded)
                    {
                        adaptedExports.AddRange(adaptedExportsResult.Value);
                    }
                }
            }

            result.ThrowOnErrors(atomicComposition);
            return adaptedExports;
        }

        private CompositionResult<IEnumerable<Export>> GetAdaptedExportsFromAdapter(AdapterDefinition adapter,
            ImportCardinality cardinality, Func<ExportDefinition, bool> fromConstraint,
            Func<ExportDefinition, bool> toConstraint, AtomicComposition atomicComposition)
        {
            // Query the container for all exports that the specified adapter 
            // can adapt, then run each one through the adapter, returning the ones 
            // that come out as non-null and also meet the specified constraint

            Assumes.NotNull(adapter);

            // Get exports to be adapted using the same cardinality as the original request, but as neither recomposable nor prerequisite
            ImportDefinition adapterImport = new ContractBasedImportDefinition(adapter.FromContractName, (String)null, (IEnumerable<KeyValuePair<string, Type>>)null, cardinality, false, false, CreationPolicy.Any);

            IEnumerable<Export> exportsToAdapt = this.SourceProvider.GetExports(adapterImport, atomicComposition);

            List<Export> adaptedExports = new List<Export>();

            foreach (Export exportToAdapt in exportsToAdapt)
            {
                if (fromConstraint == null || fromConstraint(exportToAdapt.Definition))
                {
                    CompositionResult<Export> adaptedExportResult = AdaptationHelpers.TryInvoke(() => adapter.Adapt(exportToAdapt));
                    if (!adaptedExportResult.Succeeded)
                    {
                        // Once we've hit an error, skip this adapter
                        return adaptedExportResult.ToResult<IEnumerable<Export>>();
                    }

                    Export adaptedExport = adaptedExportResult.Value;
                    if (adaptedExport != null && (toConstraint == null || toConstraint(adaptedExport.Definition)))
                    {
                        adaptedExports.Add(adaptedExport);
                    }
                }
            }

            return new CompositionResult<IEnumerable<Export>>(adaptedExports);
        }

        private AdapterDefinition[] GetAdapters(AtomicComposition atomicComposition)
        {
            // Pick up the apropriate adapters for the atomicComposition
            AdapterDefinition[] adapters = AdaptationHelpers.GetValueFromAtomicComposition(atomicComposition, this, this._adapters);
            if (adapters != null)
            {
                return adapters;
            }

            // Recompute the adapters if the default turned out to be null - this must mean that there
            // isn't a nested atomicComposition with an alternative, and so the array appropriate to the
            // null atomicComposition is actually what we want here and we can update our cached value that
            // represents the atomicComposition-free default state
            adapters = BuildAdapterArray(null);

            Thread.MemoryBarrier();
            this._adapters = adapters;

            return adapters;
        }

        private AdapterDefinition[] GetAdapterDefinitionsFromExportDefinitions(IEnumerable<ExportDefinition> adapterExports, AtomicComposition atomicComposition)
        {
            return GetAdapters(atomicComposition).Where(adapter => adapterExports.Contains(adapter.Export.Definition)).ToArray();
        }

        private AdapterDefinition[] BuildAdapterArray(AtomicComposition atomicComposition)
        {
            using (var localAtomicComposition = new AtomicComposition(atomicComposition))
            {
                // The imports of adapters cannot be adapted to prevent runaway recursion
                localAtomicComposition.SetValue(this, EmptyAdapterArray);
                return this.SourceProvider.GetExports(_adapterImport, localAtomicComposition)
                    .Select(export => new AdapterDefinition(export))
                    .ToArray();

                // Local atomicComposition is never committed because we want this to be a strictly
                // temporary arrangement that gets rolled back once the adapters are determined
            }
        }

        private IEnumerable<ExportDefinition> GetAdaptedExportDefinitions(IEnumerable<AdapterDefinition> adapters, AtomicComposition atomicComposition)
        {
            var exports = GetAdaptedExports(adapters, ImportCardinality.ZeroOrMore, null, null, atomicComposition);
            return exports.Select((export) => export.Definition);
        }

        // Since this relies rather heavily on preserving ExportDefinition identity we should
        // make an assertion that this is reasonable, or consider alternatives.  A cleaner solution would
        // be adapters that have two method: adapt export defintion, and adapt exported value
        private IEnumerable<ExportDefinition> GetAdaptedExportDefinitions(IEnumerable<ExportDefinition> sourceExportDefinitions,
            IEnumerable<AdapterDefinition> adapters, AtomicComposition atomicComposition)
        {
            var contractNames = new HashSet<String>(sourceExportDefinitions.Select(export => export.ContractName));
            var filteredAdapters = adapters.Where(adapter => contractNames.Contains(adapter.FromContractName));
            var adaptedExports = GetAdaptedExports(filteredAdapters,
                ImportCardinality.ZeroOrMore,
                exportDefinitionToAdapt => sourceExportDefinitions.Contains(exportDefinitionToAdapt),
                null, atomicComposition);
            return adaptedExports.Select(export => export.Definition);
        }

        private ExportsChangeEventArgs AdaptExportsChangedEventArgs(ExportsChangeEventArgs e, IEnumerable<AdapterDefinition> adapters, 
            IEnumerable<ExportDefinition> otherAdded, IEnumerable<ExportDefinition> otherRemoved, AtomicComposition atomicComposition)
        {
            // Determine what export changes from an instigating event result in additional export changes
            // courtesy of the potential adapters - note the null atomicComposition on removed adapters because the
            // removals can only be found outside the atomicComposition!
            var addedByAdapters = GetAdaptedExportDefinitions(e.AddedExports, adapters, atomicComposition);
            var removedByAdapters = GetAdaptedExportDefinitions(e.RemovedExports, adapters, null);

            return new ExportsChangeEventArgs(
                e.AddedExports.ConcatAllowingNull(addedByAdapters).ConcatAllowingNull(otherAdded),
                e.RemovedExports.ConcatAllowingNull(removedByAdapters).ConcatAllowingNull(otherRemoved),
                atomicComposition);
        }

        private void OnExportsChangedInternal(object sender, ExportsChangeEventArgs e)
        {
            var adaptedExportsEventArgs = AdaptExportsChangedEventArgs(e, GetAdapters(null), null, null, null);

            this.OnExportsChanged(adaptedExportsEventArgs);
        }

        private void OnExportsChangingInternal(object sender, ExportsChangeEventArgs e)
        {
            var addedAdapters = e.AddedExports
                .Where(export => export.ContractName.Equals(AdaptationConstants.AdapterContractName));
            var removedAdapters = e.RemovedExports
                .Where(export => export.ContractName.Equals(AdaptationConstants.AdapterContractName));

            // If no adapters changed then use the simple path and just propagate the changes
            if (!(addedAdapters.Any() || removedAdapters.Any()))
            {
                var adaptedEventArgs = AdaptExportsChangedEventArgs(e, GetAdapters(e.AtomicComposition), null, null, e.AtomicComposition);
                this.OnExportsChanging(adaptedEventArgs);
                return;
            }

            // Otherwise we need to build atomicCompositional state representing the new
            // adapter collection and determine extra possible added/removed exports
            using (var localAtomicComposition = new AtomicComposition(e.AtomicComposition))
            {
                // Handle the change in adapters in a atomicCompositional fashion
                AdapterDefinition[] localAdapters = BuildAdapterArray(localAtomicComposition);
                localAtomicComposition.AddCompleteAction(() => this._adapters = localAdapters);
                localAtomicComposition.SetValue(this, localAdapters);

                // Determine what exports have been removed solely as a result of removing
                // adapters (using the parent atomicComposition since it's what was present that is
                // relevant, not what is avaiable in this atomicComposition)
                var removedAdapterDefinitions = GetAdapterDefinitionsFromExportDefinitions(removedAdapters, e.AtomicComposition);
                var removedAdaptedExports = GetAdaptedExportDefinitions(removedAdapterDefinitions, e.AtomicComposition);

                // Determine what exports have been added as a result of adding adapters
                var addedAdapterDefinitions = GetAdapterDefinitionsFromExportDefinitions(addedAdapters, localAtomicComposition);
                var addedAdaptedExports = GetAdaptedExportDefinitions(addedAdapterDefinitions, localAtomicComposition);

                // Determine the adapters that haven't changed, but may produce visible changes as a
                // side-effect of exports being added/removed
                var stableAdapters = localAdapters.Where(adapter => !addedAdapters.Contains(adapter.Export.Definition));

                var adaptedEventArgs = AdaptExportsChangedEventArgs(e, 
                    stableAdapters, addedAdaptedExports, removedAdaptedExports, localAtomicComposition);

                this.OnExportsChanging(adaptedEventArgs);
                localAtomicComposition.Complete();
            }
        }

        private IEnumerable<AdapterDefinition> GetFilteredAdapters(ImportDefinition definition, AtomicComposition atomicComposition)
        {
            var adapters = GetAdapters(atomicComposition);
            if (adapters.Length == 0)
            {
                return null;
            }

            // If we have access to the contract name, such as pulling it directly from the import,
            // then we can query the adapters based on that, otherwise, we have to return them all.
            ContractBasedImportDefinition contractBasedDefinition = definition as ContractBasedImportDefinition;
            if (contractBasedDefinition == null)
            {
                return adapters;
            }

            return adapters.Where(adapter =>
            {
                return adapter.CanAdaptFrom(contractBasedDefinition.ContractName);
            });
        }

        private static Func<ExportDefinition, bool> GetConstraint(ImportDefinition definition)
        {
            ContractBasedImportDefinition contractDefinition = definition as ContractBasedImportDefinition;

            if (contractDefinition != null)
            {
                return exportDefinition => contractDefinition.IsConstraintSatisfiedBy(exportDefinition);
            }

            return definition.Constraint.Compile();
        }

        [DebuggerStepThrough]
        private void ThrowIfDisposed()
        {
            if (this._isDisposed)
            {
                throw new ObjectDisposedException(this.GetType().ToString());
            }
        }

        [DebuggerStepThrough]
        private void EnsureCanRun()
        {
            if (this.SourceProvider == null)
            {
                throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, Strings.ObjectMustBeInitialized, "SourceProvider")); // NOLOC
            }
        }

        [DebuggerStepThrough]
        private void EnsureRunning()
        {
            if (!this._isRunning)
            {
                lock(this._lock)
                {
                    if (!this._isRunning)
                    {
                        this.EnsureCanRun();
                        this._isRunning = true;
                    }
                }
            }
        }

        [DebuggerStepThrough]
        private void EnsureCanSet<T>(T currentValue)
            where T : class
        {
            if ((this._isRunning) || (currentValue != null))
            {
                throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, Strings.ObjectAlreadyInitialized));
            }
        }
    }
}
