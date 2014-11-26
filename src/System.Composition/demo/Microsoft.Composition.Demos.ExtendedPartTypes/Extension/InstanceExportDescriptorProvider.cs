// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Composition.Hosting.Core;
using System.Linq;
using System.Text;

namespace Microsoft.Composition.Demos.ExtendedPartTypes.Extension
{
    // This one-instance-per-provider design is not efficient for more than a few instances;
    // we're just aiming to show the mechanics here.
    class InstanceExportDescriptorProvider : SinglePartExportDescriptorProvider
    {
        private object _exportedInstance;

        public InstanceExportDescriptorProvider(object exportedInstance, Type contractType, string contractName, IDictionary<string, object> metadata)
            : base(contractType, contractName, metadata)
        {
            if (exportedInstance == null) throw new ArgumentNullException("exportedInstance");
            _exportedInstance = exportedInstance;
        }

        public override IEnumerable<ExportDescriptorPromise> GetExportDescriptors(CompositionContract contract, DependencyAccessor descriptorAccessor)
        {
            if (IsSupportedContract(contract))
                yield return new ExportDescriptorPromise(contract, _exportedInstance.ToString(), true, NoDependencies, _ =>
                    ExportDescriptor.Create((c, o) => _exportedInstance, Metadata));
        }
    }
}
