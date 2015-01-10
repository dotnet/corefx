// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Primitives;
using System.Composition.Hosting.Core;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Composition.Demos.CatalogHosting
{
    class PrimitiveExport
    {
        private readonly ExportDefinition _export;
        private readonly PrimitivePart _part;

        public PrimitiveExport(ExportDefinition export, PrimitivePart part)
        {
            _export = export;
            _part = part;
        }

        public IDictionary<string, object> Metadata { get { return _export.Metadata; } }

        public ExportDescriptorPromise GetPromise(CompositionContract contract, DependencyAccessor descriptorAccessor)
        {
            var origin = _export.ToString();
            if (_export is ICompositionElement)
                origin = ((ICompositionElement)_export).DisplayName;

            return new ExportDescriptorPromise(contract, origin, _part.IsShared, () => _part.GetDependencies(descriptorAccessor), d =>
            {
                var partActivator = _part.GetActivator(d);
                CompositeActivator exportActivator = (c, o) => ((ComposablePart)partActivator(c, o)).GetExportedValue(_export);
                return ExportDescriptor.Create(exportActivator, _export.Metadata);
            });
        }
    }
}
