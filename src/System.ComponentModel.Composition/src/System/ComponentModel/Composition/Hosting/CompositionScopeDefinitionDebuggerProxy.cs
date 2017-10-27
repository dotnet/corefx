// -----------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Internal;
using Microsoft.Internal.Collections;
using System.ComponentModel.Composition.Primitives;

namespace System.ComponentModel.Composition.Hosting
{
    // This proxy is needed to pretty up CompositionScopeDefinitionCatalog.Parts; IQueryable<T> 
    // instances are not displayed in a very friendly way in the debugger.
    internal class CompositionScopeDefinitionDebuggerProxy
    {
        private readonly CompositionScopeDefinition _compositionScopeDefinition;

        public CompositionScopeDefinitionDebuggerProxy(CompositionScopeDefinition compositionScopeDefinition) 
        {
            Requires.NotNull(compositionScopeDefinition, "compositionScopeDefinition");

            this._compositionScopeDefinition = compositionScopeDefinition;
        }

        public ReadOnlyCollection<ComposablePartDefinition> Parts
        {
            get { return this._compositionScopeDefinition.Parts.ToReadOnlyCollection(); }
        }
        
        public IEnumerable<ExportDefinition> PublicSurface
        {
            get
            {
                return this._compositionScopeDefinition.PublicSurface.ToReadOnlyCollection();
            }
        } 

        public virtual IEnumerable<CompositionScopeDefinition> Children
        {
            get
            {
                return this._compositionScopeDefinition.Children.ToReadOnlyCollection();
            }
        }


    }
}
