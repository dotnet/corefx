// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Composition.Hosting.Core;
using System.Composition.Runtime;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Composition.Demos.ExportUnrecognizedConcreteTypes.Extension
{
    public class UnrecognizedConcreteTypeSource : ExportDescriptorProvider
    {
        public override IEnumerable<ExportDescriptorPromise> GetExportDescriptors(CompositionContract contract, DependencyAccessor definitionAccessor)
        {
            if (contract.ContractType.IsAbstract ||
                !contract.ContractType.IsClass ||
                !contract.Equals(new CompositionContract(contract.ContractType)))
                return NoExportDescriptors;

            if (!definitionAccessor.ResolveDependencies("test", contract, false).Any())
                return NoExportDescriptors;

            return new[] { new ExportDescriptorPromise(
                contract,
                contract.ContractType.Name,
                false,
                NoDependencies,
                _ => ExportDescriptor.Create((c, o) => {
                    var instance = Activator.CreateInstance(contract.ContractType);
                    if (instance is IDisposable) c.AddBoundInstance((IDisposable)instance);
                    return instance;
                }, NoMetadata)) };
        }
    }
}
