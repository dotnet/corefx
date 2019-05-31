// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.ComponentModel.Composition.Primitives;
using System.Linq;

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
                if (catalog == null)
                {
                    throw new ArgumentNullException(nameof(catalog));
                }
                if (importFilter == null)
                {
                    throw new ArgumentNullException(nameof(importFilter));
                }

                _parts = catalog._innerCatalog;
                _importFilter = importFilter;
            }

            public void Initialize()
            {
                BuildImportersIndex();
            }

            private void BuildImportersIndex()
            {
                _importersIndex = new Dictionary<string, List<ComposablePartDefinition>>();
                foreach (ComposablePartDefinition part in _parts)
                {
                    foreach (var import in part.ImportDefinitions)
                    {
                        foreach (var contractName in import.GetCandidateContractNames(part))
                        {
                            AddToImportersIndex(contractName, part);
                        }
                    }
                }
            }

            private void AddToImportersIndex(string contractName, ComposablePartDefinition part)
            {
                List<ComposablePartDefinition> parts = null;
                if (!_importersIndex.TryGetValue(contractName, out parts))
                {
                    parts = new List<ComposablePartDefinition>();
                    _importersIndex.Add(contractName, parts);
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
                    if (_importersIndex.TryGetValue(export.ContractName, out candidateReachableParts))
                    {
                        // find if they actually match
                        foreach (var candidateReachablePart in candidateReachableParts)
                        {
                            foreach (ImportDefinition import in candidateReachablePart.ImportDefinitions.Where(_importFilter))
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
