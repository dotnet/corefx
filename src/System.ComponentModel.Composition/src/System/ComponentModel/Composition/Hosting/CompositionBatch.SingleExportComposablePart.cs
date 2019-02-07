// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.ComponentModel.Composition.Primitives;
using System.Diagnostics.Contracts;
using System.Linq;

namespace System.ComponentModel.Composition.Hosting
{
    public partial class CompositionBatch
    {
        // Represents a part that exports a single export
        private class SingleExportComposablePart : ComposablePart
        {
            private readonly Export _export;

            public SingleExportComposablePart(Export export)
            {
                _export = export ?? throw new ArgumentNullException(nameof(export));
            }

            public override IDictionary<string, object> Metadata
            {
                get { return MetadataServices.EmptyMetadata; }
            }

            public override IEnumerable<ExportDefinition> ExportDefinitions
            {
                get { return new ExportDefinition[] { _export.Definition }; }
            }

            public override IEnumerable<ImportDefinition> ImportDefinitions
            {
                get { return Enumerable.Empty<ImportDefinition>(); }
            }

            public override object GetExportedValue(ExportDefinition definition)
            {
                if (definition == null)
                {
                    throw new ArgumentNullException(nameof(definition));
                }

                if (definition != _export.Definition)
                {
                    throw ExceptionBuilder.CreateExportDefinitionNotOnThisComposablePart(nameof(definition));
                }

                return _export.Value;
            }

            public override void SetImport(ImportDefinition definition, IEnumerable<Export> exports)
            {
                if (definition == null)
                {
                    throw new ArgumentNullException(nameof(definition));
                }

                if (exports == null)
                {
                    throw new ArgumentNullException(nameof(exports));
                }

                if (!Contract.ForAll(exports, (export) => export != null))
                {
                    throw ExceptionBuilder.CreateContainsNullElement(nameof(exports));
                }

                throw ExceptionBuilder.CreateImportDefinitionNotOnThisComposablePart(nameof(definition));
            }
        }
    }
}
