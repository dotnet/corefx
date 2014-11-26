// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.Composition.Hosting;
using System.Composition.Hosting.Core;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Composition.Demos.CatalogHosting
{
    class PrimitivePart
    {
        private readonly bool _isShared;
        private readonly string _boundary;
        private CompositeActivator _activator;
        private ComposablePartDefinition _part;
        private Func<string, Type> _typeNameResolver;

        public PrimitivePart(ComposablePartDefinition part, Func<string, Type> typeNameResolver, Func<CreationPolicy, Tuple<bool, string>> creationPolicyMapping)
        {
            var creationPolicy = CreationPolicy.Any;

            object creationPolicyMetadata;
            if (part.Metadata.TryGetValue(CompositionConstants.PartCreationPolicyMetadataName, out creationPolicyMetadata) &&
                creationPolicyMetadata is CreationPolicy)
                creationPolicy = (CreationPolicy)creationPolicyMetadata;

            var sharing = creationPolicyMapping(creationPolicy);
            _isShared = sharing.Item1;
            _boundary = sharing.Item2;

            _part = part;
            _typeNameResolver = typeNameResolver;
        }

        public bool IsShared { get { return _isShared; } }

        public IEnumerable<CompositionDependency> GetDependencies(DependencyAccessor descriptorAccessor)
        {
            return _part.ImportDefinitions
                .SelectMany(def => ImportDefinitionToDependency(def, descriptorAccessor));
        }

        IEnumerable<CompositionDependency> ImportDefinitionToDependency(ImportDefinition def, DependencyAccessor descriptorAccessor)
        {
            var cbid = def as ContractBasedImportDefinition;
            if (cbid == null)
                throw new CompositionFailedException("Only typed (contract-based) import definitions are supported.");

            var contract = ImportDefinitionToContract(cbid);

            switch (cbid.Cardinality)
            {
                case ImportCardinality.ExactlyOne:
                    yield return descriptorAccessor.ResolveRequiredDependency(def, contract, def.IsPrerequisite);
                    break;

                case ImportCardinality.ZeroOrOne:
                    CompositionDependency result;
                    if (descriptorAccessor.TryResolveOptionalDependency(def, contract, def.IsPrerequisite, out result))
                        yield return result;
                    break;

                case ImportCardinality.ZeroOrMore:
                    foreach (var item in descriptorAccessor.ResolveDependencies(def, contract, def.IsPrerequisite))
                        yield return item;
                    break;

                default:
                    throw new CompositionFailedException("Unsupported cardinality.");
            }
        }

        CompositionContract ImportDefinitionToContract(ContractBasedImportDefinition cbid)
        {
            var typeId = cbid.RequiredTypeIdentity;
            if (typeId == null)
                throw new CompositionFailedException("Dynamic or 'object' imports are not supported'.");

            var contractType = _typeNameResolver(PrimitiveAdaptation.ParseTypeIdentity(typeId));
            var contractName = cbid.ContractName.Equals(typeId) ? null : cbid.ContractName;

            if (cbid.RequiredCreationPolicy != CreationPolicy.Any)
                throw new CompositionFailedException("Required creation policy cannot be specified.");

            // Required metadata is ignored; we will let the primitives fail later.

            return new CompositionContract(contractType, contractName);
        }

        public CompositeActivator GetActivator(IEnumerable<CompositionDependency> dependencies)
        {
            if (_activator == null)
                _activator = CreateActivator(dependencies);

            return _activator;
        }

        CompositeActivator CreateActivator(IEnumerable<CompositionDependency> dependencies)
        {
            var dependenciesByImport = dependencies.GroupBy(d => (ContractBasedImportDefinition)d.Site, d => d.Target.GetDescriptor());
            var prereqs = dependenciesByImport.Where(d => d.Key.IsPrerequisite).ToArray();
            var nonprereqs = dependenciesByImport.Where(d => !d.Key.IsPrerequisite).ToArray();
            var unset = _part.ImportDefinitions.Where(id => !prereqs.Concat(nonprereqs).Any(k => k.Key.Equals((ContractBasedImportDefinition)id))).ToArray();

            CompositeActivator construct = (c, o) =>
            {
                var result = _part.CreatePart();
                if (result is IDisposable)
                    c.AddBoundInstance((IDisposable)result);

                foreach (var pre in prereqs)
                    result.SetImport(pre.Key, DependenciesToExports(c, pre));

                foreach (var un in unset)
                    result.SetImport(un, Enumerable.Empty<Export>());

                o.AddNonPrerequisiteAction(() =>
                {
                    foreach (var np in nonprereqs)
                        result.SetImport(np.Key, DependenciesToExports(c, np));
                });

                o.AddPostCompositionAction(() =>
                {
                    result.Activate();
                });

                return result;
            };

            if (!IsShared)
                return construct;

            var sharingId = LifetimeContext.AllocateSharingId();
            CompositeActivator constructAndShare = (c, o) => c.GetOrCreate(sharingId, o, construct);

            return (c, o) =>
            {
                var scope = c.FindContextWithin(_boundary);
                return scope.Equals(c) ?
                    constructAndShare(scope, o) :
                    CompositionOperation.Run(scope, constructAndShare);
            };
        }

        static IEnumerable<Export> DependenciesToExports(LifetimeContext c, IGrouping<ContractBasedImportDefinition, ExportDescriptor> dependenciesByImport)
        {
            return dependenciesByImport
                .Where(d => RequiredMetadataKeysArePresent(d.Metadata, dependenciesByImport.Key))
                .Select(d => new Export(dependenciesByImport.Key.ContractName, d.Metadata, () => CompositionOperation.Run(c, d.Activator)));
        }

        static bool RequiredMetadataKeysArePresent(IDictionary<string, object> metadata, ContractBasedImportDefinition contract)
        {
            foreach (var rm in contract.RequiredMetadata)
            {
                if (rm.Key != CompositionConstants.ExportTypeIdentityMetadataName &&
                    rm.Key != CompositionConstants.ImportSourceMetadataName)
                {
                    if (!metadata.ContainsKey(rm.Key))
                        return false;
                }
            }
            return true;
        }
    }
}
