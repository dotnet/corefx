// -----------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Primitives;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using Microsoft.Internal;
using Microsoft.Internal.Collections;

namespace System.ComponentModel.Composition.Hosting
{
    public partial class ImportEngine
    {
        /// <summary>
        ///     Used by the <see cref="ImportEngine"/> to manage the composition of a given part.
        ///     It stores things like the list of disposable exports used to satisfy the imports as
        ///     well as the caching of the exports discovered during previewing of a part.
        /// </summary>
        private class PartManager
        {
            private Dictionary<ImportDefinition, List<IDisposable>> _importedDisposableExports;
            private Dictionary<ImportDefinition, Export[]> _importCache;
            private string[] _importedContractNames;
            private ComposablePart _part;
            private ImportState _state = ImportState.NoImportsSatisfied;
            private readonly ImportEngine _importEngine;

            public PartManager(ImportEngine importEngine, ComposablePart part)
            {
                this._importEngine = importEngine;
                this._part = part;
            }

            public ComposablePart Part
            {
                get
                {
                    return this._part;
                }
            }

            public ImportState State
            {
                get
                {
                    using (this._importEngine._lock.LockStateForRead())
                    {
                        return this._state;
                    }
                }
                set
                {
                    using (this._importEngine._lock.LockStateForWrite())
                    {
                        this._state = value;
                    }
                }
            }

            public bool TrackingImports { get; set; }

            public IEnumerable<string> GetImportedContractNames()
            {
                if (this.Part == null)
                {
                    return Enumerable.Empty<string>();
                }

                if (this._importedContractNames == null)
                {
                    this._importedContractNames = this.Part.ImportDefinitions.Select(import => import.ContractName ?? ImportDefinition.EmptyContractName).Distinct().ToArray();
                }
                return this._importedContractNames;
            }

            public CompositionResult TrySetImport(ImportDefinition import, Export[] exports)
            {
                try
                {
                    this.Part.SetImport(import, exports);
                    UpdateDisposableDependencies(import, exports);
                    return CompositionResult.SucceededResult;
                }
                catch (CompositionException ex)
                {   // Pulling on one of the exports failed

                    return new CompositionResult(
                        ErrorBuilder.CreatePartCannotSetImport(Part, import, ex));
                }
                catch (ComposablePartException ex)
                {   // Type mismatch between export and import

                    return new CompositionResult(
                        ErrorBuilder.CreatePartCannotSetImport(Part, import, ex));
                }
            }

            public void SetSavedImport(ImportDefinition import, Export[] exports, AtomicComposition atomicComposition)
            {
                if (atomicComposition != null)
                {
                    var savedExports = this.GetSavedImport(import);

                    // Add a revert action to revert the stored exports
                    // in the case that this atomicComposition gets rolled back.
                    atomicComposition.AddRevertAction(() =>
                        this.SetSavedImport(import, savedExports, null));
                }

                if (this._importCache == null)
                {
                    this._importCache = new Dictionary<ImportDefinition, Export[]>();
                }

                this._importCache[import] = exports;
            }

            public Export[] GetSavedImport(ImportDefinition import)
            {
                Export[] exports = null;
                if (this._importCache != null)
                {
                    // We don't care about the return value we just want the exports
                    // and if it isn't present we just return the initialized null value
                    this._importCache.TryGetValue(import, out exports);
                }
                return exports;
            }

            public void ClearSavedImports()
            {
                this._importCache = null;
            }

            public CompositionResult TryOnComposed()
            {
                try
                {
                    this.Part.Activate();
                    return CompositionResult.SucceededResult;
                }
                catch (ComposablePartException ex)
                {   // Type failed to be constructed, imports could not be set, etc
                    return new CompositionResult(
                        ErrorBuilder.CreatePartCannotActivate(this.Part, ex));
                }
            }

            public void UpdateDisposableDependencies(ImportDefinition import, Export[] exports)
            {
                // Determine if there are any new disposable exports, optimizing for the most
                // likely case, which is that there aren't any
                List<IDisposable> disposableExports = null;
                foreach (var export in exports)
                {
                    IDisposable disposableExport = export as IDisposable;
                    if (disposableExport != null)
                    {
                        if (disposableExports == null)
                        {
                            disposableExports = new List<IDisposable>();
                        }
                        disposableExports.Add(disposableExport);
                    }
                }

                // Dispose any existing references previously set on this import
                List<IDisposable> oldDisposableExports = null;
                if (this._importedDisposableExports != null &&
                    this._importedDisposableExports.TryGetValue(import, out oldDisposableExports))
                {
                    oldDisposableExports.ForEach(disposable => disposable.Dispose());

                    // If there aren't any replacements, get rid of the old storage
                    if (disposableExports == null)
                    {
                        this._importedDisposableExports.Remove(import);
                        if (!this._importedDisposableExports.FastAny())
                        {
                            this._importedDisposableExports = null;
                        }

                        return;
                    }
                }

                // Record the new collection
                if (disposableExports != null)
                {
                    if (this._importedDisposableExports == null)
                    {
                        this._importedDisposableExports = new Dictionary<ImportDefinition, List<IDisposable>>();
                    }
                    this._importedDisposableExports[import] = disposableExports;
                }
            }

            public void DisposeAllDependencies()
            {
                if (this._importedDisposableExports != null)
                {
                    IEnumerable<IDisposable> dependencies = this._importedDisposableExports.Values
                        .SelectMany(exports => exports);

                    this._importedDisposableExports = null;

                    dependencies.ForEach(disposableExport => disposableExport.Dispose());
                }
            }
        }
    }
}
