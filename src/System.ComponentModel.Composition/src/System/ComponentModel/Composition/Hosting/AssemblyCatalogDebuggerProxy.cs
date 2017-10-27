// -----------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition.Primitives;
using System.ComponentModel.Composition.ReflectionModel;
using System.Linq;
using System.Reflection;
using Microsoft.Internal;
using Microsoft.Internal.Collections;

namespace System.ComponentModel.Composition.Hosting
{
    internal class AssemblyCatalogDebuggerProxy
    {
        private readonly AssemblyCatalog _catalog;

        public AssemblyCatalogDebuggerProxy(AssemblyCatalog catalog)
        {
            Requires.NotNull(catalog, "catalog");

            this._catalog = catalog;
        }

        public Assembly Assembly
        {
            get { return this._catalog.Assembly; }
        }

        public ReadOnlyCollection<ComposablePartDefinition> Parts
        {
            // NOTE: This shouldn't be cached, so that on every query of
            // the current value of the underlying catalog is respected.
            // We use ReadOnlyCollection as arrays do not have the 
            // appropriate debugger display attributes applied to them.
            get { return this._catalog.Parts.ToReadOnlyCollection(); }
        }
    }
}