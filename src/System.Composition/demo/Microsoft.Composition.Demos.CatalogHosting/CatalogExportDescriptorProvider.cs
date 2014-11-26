// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.Composition.Hosting.Core;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Composition.Demos.CatalogHosting
{
    public class CatalogExportDescriptorProvider : ExportDescriptorProvider
    {
        private readonly IDictionary<CompositionContract, ICollection<PrimitiveExport>> _parts;

        public CatalogExportDescriptorProvider(
            ComposablePartCatalog catalog,
            Func<string, Type> typeNameResolver = null,
            Func<CreationPolicy, Tuple<bool, string>> creationPolicyMapping = null)
        {
            if (catalog == null) throw new ArgumentNullException("catalog");

            _parts = MapPrimitiveParts(
                catalog,
                typeNameResolver ?? PrimitiveAdaptation.DefaultTypeNameResolver,
                creationPolicyMapping ?? PrimitiveAdaptation.DefaultCreationPolicyMapping);

            var incpc = catalog as INotifyComposablePartCatalogChanged;
            if (incpc != null)
            {
                incpc.Changing += (s, e) => { throw new InvalidOperationException("Recomposition is not supported by this container; catalogs cannot be changed."); };
            }
        }

        IDictionary<CompositionContract, ICollection<PrimitiveExport>> MapPrimitiveParts(ComposablePartCatalog catalog, Func<string, Type> typeNameResolver, Func<CreationPolicy, Tuple<bool, string>> creationPolicyMapping)
        {
            var result = new Dictionary<CompositionContract, ICollection<PrimitiveExport>>();

            foreach (var part in catalog)
            {
                var primitivePart = new PrimitivePart(part, typeNameResolver, creationPolicyMapping);
                foreach (var export in part.ExportDefinitions)
                {
                    var contract = MapExportedContract(export, typeNameResolver);
                    ICollection<PrimitiveExport> existing;
                    if (!result.TryGetValue(contract, out existing))
                    {
                        existing = new List<PrimitiveExport>();
                        result.Add(contract, existing);
                    }
                    existing.Add(new PrimitiveExport(export, primitivePart));
                }
            }

            return result;
        }

        CompositionContract MapExportedContract(ExportDefinition export, Func<string, Type> typeNameResolver)
        {
            object typeId;
            if (!export.Metadata.TryGetValue(CompositionConstants.ExportTypeIdentityMetadataName, out typeId) ||
                !(typeId is string))
            {
                throw new NotSupportedException("Only typed exports are supported.");
            }

            var contractType = typeNameResolver(PrimitiveAdaptation.ParseTypeIdentity((string)typeId));
            var contractName = export.ContractName.Equals(typeId) ? null : export.ContractName;

            return new CompositionContract(contractType, contractName);
        }

        public override IEnumerable<ExportDescriptorPromise> GetExportDescriptors(
            CompositionContract contract,
            DependencyAccessor descriptorAccessor)
        {
            var unconstrained = new CompositionContract(contract.ContractType, contract.ContractName);

            ICollection<PrimitiveExport> candidates;
            if (!_parts.TryGetValue(unconstrained, out candidates))
                return NoExportDescriptors;

            return candidates
                .Where(px =>
                {
                    if (contract.MetadataConstraints != null)
                    {
                        var subsetOfConstraints = contract.MetadataConstraints.Where(c => px.Metadata.ContainsKey(c.Key)).ToDictionary(c => c.Key, c => px.Metadata[c.Key]);
                        var constrainedSubset = new CompositionContract(contract.ContractType, contract.ContractName,
                            subsetOfConstraints.Count == 0 ? null : subsetOfConstraints);

                        if (!contract.Equals(constrainedSubset))
                            return false;
                    }

                    return true;
                })
                .Select(px => px.GetPromise(contract, descriptorAccessor));
        }
    }
}
