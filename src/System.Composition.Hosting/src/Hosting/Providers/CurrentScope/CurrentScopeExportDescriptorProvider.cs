// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// -----------------------------------------------------------------------
// Copyright © Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------

using System.Collections.Generic;
using System.Composition.Hosting.Core;
using System.Composition.Runtime;

namespace System.Composition.Hosting.Providers.CurrentScope
{
    class CurrentScopeExportDescriptorProvider : ExportDescriptorProvider
    {
        private static readonly CompositionContract _CurrentScopeContract = new CompositionContract(typeof(CompositionContext));

        public override IEnumerable<ExportDescriptorPromise> GetExportDescriptors(CompositionContract contract, DependencyAccessor definitionAccessor)
        {
            if (!contract.Equals(_CurrentScopeContract))
                return NoExportDescriptors;

            return new[] { new ExportDescriptorPromise(
                contract,
                typeof(CompositionContext).Name,
                true,
                NoDependencies,
                _ => ExportDescriptor.Create((c, o) => c, NoMetadata)) };
        }
    }
}
