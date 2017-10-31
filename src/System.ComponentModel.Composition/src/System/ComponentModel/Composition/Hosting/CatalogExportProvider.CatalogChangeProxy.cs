// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Primitives;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using Microsoft.Internal;
using Microsoft.Internal.Collections;

namespace System.ComponentModel.Composition.Hosting
{
    public partial class CatalogExportProvider : ExportProvider, IDisposable
    {
        private class CatalogChangeProxy : ComposablePartCatalog
        {
            private ComposablePartCatalog _originalCatalog;
            private List<ComposablePartDefinition> _addedParts;
            private HashSet<ComposablePartDefinition> _removedParts;

            public CatalogChangeProxy(ComposablePartCatalog originalCatalog,
                IEnumerable<ComposablePartDefinition> addedParts,
                IEnumerable<ComposablePartDefinition> removedParts)
            {
                this._originalCatalog = originalCatalog;
                this._addedParts = new List<ComposablePartDefinition>(addedParts);
                this._removedParts = new HashSet<ComposablePartDefinition>(removedParts);
            }

            public override IEnumerator<ComposablePartDefinition> GetEnumerator()
            {
                return this._originalCatalog.Concat(this._addedParts).Except(this._removedParts).GetEnumerator();
            }

            public override IEnumerable<Tuple<ComposablePartDefinition, ExportDefinition>> GetExports(
                ImportDefinition definition)
            {
                Requires.NotNull(definition, nameof(definition));

                var originalExports = this._originalCatalog.GetExports(definition);
                var trimmedExports = originalExports.Where(partAndExport =>
                    !this._removedParts.Contains(partAndExport.Item1));

                var addedExports = new List<Tuple<ComposablePartDefinition, ExportDefinition>>();
                foreach (var part in this._addedParts)
                {
                    foreach (var export in part.ExportDefinitions)
                    {
                        if (definition.IsConstraintSatisfiedBy(export))
                        {
                            addedExports.Add(new Tuple<ComposablePartDefinition, ExportDefinition>(part, export));
                        }
                    }
                }
                return trimmedExports.Concat(addedExports);
            }
        }
    }
}
