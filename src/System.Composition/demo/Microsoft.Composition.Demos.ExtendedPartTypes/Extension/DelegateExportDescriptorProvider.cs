// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Composition.Hosting.Core;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Composition.Demos.ExtendedPartTypes.Extension
{
    class DelegateExportDescriptorProvider : SinglePartExportDescriptorProvider
    {
        private CompositeActivator _activator;

        public DelegateExportDescriptorProvider(Func<object> exportedInstanceFactory, Type contractType, string contractName, IDictionary<string, object> metadata, bool isShared)
            : base(contractType, contractName, metadata)
        {
            if (exportedInstanceFactory == null) throw new ArgumentNullException("exportedInstanceFactory");

            // Runs the factory method, validates the result and registers it for disposal if necessary.
            CompositeActivator constructor = (c, o) =>
            {
                var result = exportedInstanceFactory();
                if (result == null)
                    throw new InvalidOperationException("Delegate factory returned null.");

                if (result is IDisposable)
                    c.AddBoundInstance((IDisposable)result);

                return result;
            };

            if (isShared)
            {
                var sharingId = LifetimeContext.AllocateSharingId();
                _activator = (c, o) =>
                {
                    // Find the root composition scope.
                    var scope = c.FindContextWithin(null);
                    if (scope == c)
                    {
                        // We're already in the root scope, create the instance
                        return scope.GetOrCreate(sharingId, o, constructor);
                    }
                    else
                    {
                        // Composition is moving up the hierarchy of scopes; run
                        // a new operation in the root scope.
                        return CompositionOperation.Run(scope, (c1, o1) => c1.GetOrCreate(sharingId, o1, constructor));
                    }
                };
            }
            else
            {
                _activator = constructor;
            }
        }

        public override IEnumerable<ExportDescriptorPromise> GetExportDescriptors(CompositionContract contract, DependencyAccessor descriptorAccessor)
        {
            if (IsSupportedContract(contract))
                yield return new ExportDescriptorPromise(contract, "factory delegate", true, NoDependencies, _ => ExportDescriptor.Create(_activator, Metadata));
        }
    }
}
