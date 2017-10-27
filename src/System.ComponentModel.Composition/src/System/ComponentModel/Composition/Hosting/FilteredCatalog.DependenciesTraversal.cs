// -----------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Primitives;
using System.ComponentModel.Composition.ReflectionModel;
using System.Linq;
using Microsoft.Internal;

namespace System.ComponentModel.Composition.Hosting
{
    public partial class FilteredCatalog
    {
        internal class DependenciesTraversal : IComposablePartCatalogTraversal
        {
            private IEnumerable<ComposablePartDefinition> _parts;
            private Func<ImportDefinition, bool> _importFilter;
            private Dictionary<string, List<ComposablePartDefinition>> _exportersIndex;

            public DependenciesTraversal(FilteredCatalog catalog, Func<ImportDefinition, bool> importFilter)
            {
                Assumes.NotNull(catalog);
                Assumes.NotNull(importFilter);

                this._parts = catalog._innerCatalog;
                this._importFilter = importFilter;
            }

            public void Initialize()
            {
                this.BuildExportersIndex();
            }

            private void BuildExportersIndex()
            {
                this._exportersIndex = new Dictionary<string, List<ComposablePartDefinition>>();
                foreach (ComposablePartDefinition part in this._parts)
                {
                    foreach (var export in part.ExportDefinitions)
                    {
                        this.AddToExportersIndex(export.ContractName, part);
                    }
                }
            }

            private void AddToExportersIndex(string contractName, ComposablePartDefinition part)
            {
                List<ComposablePartDefinition> parts = null;
                if (!this._exportersIndex.TryGetValue(contractName, out parts))
                {
                    parts = new List<ComposablePartDefinition>();
                    this._exportersIndex.Add(contractName, parts);
                }
                parts.Add(part);
            }

            public bool TryTraverse(ComposablePartDefinition part, out IEnumerable<ComposablePartDefinition> reachableParts)
            {
                reachableParts = null;
                List<ComposablePartDefinition> reachablePartList = null;

                // Go through all part imports
                foreach (ImportDefinition import in part.ImportDefinitions.Where(this._importFilter))
                {
                    // Find all parts that we know will import each export
                    List<ComposablePartDefinition> candidateReachableParts = null;
                    foreach (var contractName in import.GetCandidateContractNames(part))
                    {
                        if (this._exportersIndex.TryGetValue(contractName, out candidateReachableParts))
                        {
                            // find if they actually match
                            foreach (var candidateReachablePart in candidateReachableParts)
                            {
                                foreach (ExportDefinition export in candidateReachablePart.ExportDefinitions)
                                {
                                    if (import.IsImportDependentOnPart(candidateReachablePart, export, part.IsGeneric() != candidateReachablePart.IsGeneric()))
                                    {
                                        if (reachablePartList == null)
                                        {
                                            reachablePartList = new List<ComposablePartDefinition>();
                                        }
                                        reachablePartList.Add(candidateReachablePart);
                                    }
                                }
                            }
                        }
                    }
                }

                reachableParts = reachablePartList;
                return (reachableParts != null);
            }
        }
    }
}
