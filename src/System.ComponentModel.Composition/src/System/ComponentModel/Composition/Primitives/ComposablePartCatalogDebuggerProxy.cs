// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Collections.ObjectModel;

namespace System.ComponentModel.Composition.Primitives
{
    // This proxy is needed to pretty up ComposablePartCatalog.Parts; IQueryable<T> 
    // instances are not displayed in a very friendly way in the debugger.
    internal class ComposablePartCatalogDebuggerProxy
    {
        private readonly ComposablePartCatalog _catalog;

        public ComposablePartCatalogDebuggerProxy(ComposablePartCatalog catalog)
        {
            _catalog = catalog ?? throw new ArgumentNullException(nameof(catalog));
        }

        // NOTE: This shouldn't be cached, so that on every query of
        // the current value of the underlying catalog is respected.
        // We use ReadOnlyCollection as arrays do not have the 
        // appropriate debugger display attributes applied to them.
        public ReadOnlyCollection<ComposablePartDefinition> Parts => _catalog.Parts.ToReadOnlyCollection();
    }
}
