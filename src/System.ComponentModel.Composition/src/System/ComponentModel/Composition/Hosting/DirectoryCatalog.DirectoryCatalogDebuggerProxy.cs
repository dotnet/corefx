// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition.Primitives;
using System.Linq;
using System.Reflection;

namespace System.ComponentModel.Composition.Hosting
{
    public partial class DirectoryCatalog
    {
        internal class DirectoryCatalogDebuggerProxy
        {
            private readonly DirectoryCatalog _catalog;

            public DirectoryCatalogDebuggerProxy(DirectoryCatalog catalog)
            {
                _catalog = catalog ?? throw new ArgumentNullException(nameof(catalog));
            }

            public ReadOnlyCollection<Assembly> Assemblies => 
                _catalog._assemblyCatalogs.Values.Select(catalog => catalog.Assembly).ToReadOnlyCollection();

            public ReflectionContext ReflectionContext => _catalog._reflectionContext;

            public string SearchPattern => _catalog.SearchPattern;

            public string Path => _catalog._path;

            public string FullPath => _catalog._fullPath;

            public ReadOnlyCollection<string> LoadedFiles => _catalog._loadedFiles;

            // NOTE: This shouldn't be cached, so that on every query of
            // the current value of the underlying catalog is respected.
            // We use ReadOnlyCollection as arrays do not have the 
            // appropriate debugger display attributes applied to them.
            public ReadOnlyCollection<ComposablePartDefinition> Parts => _catalog.Parts.ToReadOnlyCollection();
        }
    }
}
