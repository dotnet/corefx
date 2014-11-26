// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Composition.Hosting.Core;
using System.Composition.Runtime;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Composition.Demos.DefaultOnly.Extension
{
    public class DefaultExportDescriptorProvider : ExportDescriptorProvider
    {
        public override IEnumerable<ExportDescriptorPromise> GetExportDescriptors(CompositionContract contract, DependencyAccessor descriptorAccessor)
        {
            // Avoid trying to create defaults-of-defaults-of...
            if (contract.ContractName != null && contract.ContractName.StartsWith(Constants.DefaultContractNamePrefix))
                return NoExportDescriptors;

            var implementations = descriptorAccessor.ResolveDependencies("test for default", contract, false);
            if (implementations.Any())
                return NoExportDescriptors;

            var defaultImplementationDiscriminator = Constants.DefaultContractNamePrefix + (contract.ContractName ?? "");
            IDictionary<string, object> copiedConstraints = null;
            if (contract.MetadataConstraints != null)
                copiedConstraints = contract.MetadataConstraints.ToDictionary(k => k.Key, k => k.Value);
            var defaultImplementationContract = new CompositionContract(contract.ContractType, defaultImplementationDiscriminator, copiedConstraints);

            CompositionDependency defaultImplementation;
            if (!descriptorAccessor.TryResolveOptionalDependency("default", defaultImplementationContract, true, out defaultImplementation))
                return NoExportDescriptors;

            return new[] { new ExportDescriptorPromise(
                contract,
                "Default Implementation",
                false,
                () => new[] { defaultImplementation },
                _ => {
                    var defaultDescriptor = defaultImplementation.Target.GetDescriptor();
                    return ExportDescriptor.Create((c, o) => defaultDescriptor.Activator(c, o), defaultDescriptor.Metadata);
                })};
        }
    }
}
