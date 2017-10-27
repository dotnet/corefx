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
        /// <summary>
        /// Implementation of IComposablePartTraversal supporting the Dependents traveral pattern.
        /// The implementation is optimized for a situation when the traversal is expected to be rather short-lived - that is,
        /// if the chains of dependecies are rather small. To achieve that we do a very minimal structure prep upfront - merely creating a contract-based
        /// index of imports - and the verify the full match of imports during the traversal. Given that most parts have a very few imports this should perform well.
        /// </summary>
        internal class DependentsTraversal : IComposablePartCatalogTraversal
        {
            private IEnumerable<ComposablePartDefinition> _parts;
            private Func<ImportDefinition, bool> _importFilter;
            private Dictionary<string, List<ComposablePartDefinition>> _importersIndex;

            public DependentsTraversal(FilteredCatalog catalog, Func<ImportDefinition, bool> importFilter)
            {
                Assumes.NotNull(catalog);
                Assumes.NotNull(importFilter);

                this._parts = catalog._innerCatalog;
                this._importFilter = importFilter;
            }

            public void Initialize()
            {
                this.BuildImportersIndex();
            }

            private void BuildImportersIndex()
            {
                this._importersIndex = new Dictionary<string, List<ComposablePartDefinition>>();
                foreach (ComposablePartDefinition part in this._parts)
                {
                    foreach (var import in part.ImportDefinitions)
                    {
                        foreach (var contractName in import.GetCandidateContractNames(part))
                        {
                            this.AddToImportersIndex(contractName, part);
                        }
                    }
                }
            }


            private void AddToImportersIndex(string contractName, ComposablePartDefinition part)
            {
                List<ComposablePartDefinition> parts = null;
                if (!this._importersIndex.TryGetValue(contractName, out parts))
                {
                    parts = new List<ComposablePartDefinition>();
                    this._importersIndex.Add(contractName, parts);
                }
                parts.Add(part);
            }

            public bool TryTraverse(ComposablePartDefinition part, out IEnumerable<ComposablePartDefinition> reachableParts)
            {
                reachableParts = null;
                List<ComposablePartDefinition> reachablePartList = null;

                // Go through all part exports
                foreach (ExportDefinition export in part.ExportDefinitions)
                {
                    // Find all parts that we know will import each export
                    List<ComposablePartDefinition> candidateReachableParts = null;
                    if (this._importersIndex.TryGetValue(export.ContractName, out candidateReachableParts))
                    {
                        // find if they actually match
                        foreach (var candidateReachablePart in candidateReachableParts)
                        {
                            foreach (ImportDefinition import in candidateReachablePart.ImportDefinitions.Where(this._importFilter))
                            {
                                if (import.IsImportDependentOnPart(part, export, part.IsGeneric() != candidateReachablePart.IsGeneric()))
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

                reachableParts = reachablePartList;
                return (reachableParts != null);
            }
        }
    }
}
