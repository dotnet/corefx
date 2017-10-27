// -----------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using Microsoft.Internal;

namespace System.ComponentModel.Composition.Primitives
{
    // Because the debugger displays only the members available on ICompositionElement 
    // when viewing CompositionError.Element in the watch and data tips windows, we 
    // need this proxy so that the underlying object wrapped by the CompositionElement 
    // placeholder is displayed by default.
    internal class CompositionElementDebuggerProxy
    {
        private readonly CompositionElement _element;

        public CompositionElementDebuggerProxy(CompositionElement element) 
        {
            Requires.NotNull(element, "element");

            this._element = element;
        }

        public string DisplayName
        {
            get { return this._element.DisplayName; }
        }

        public ICompositionElement Origin
        {
            get { return this._element.Origin; }
        }

        public object UnderlyingObject
        {
            get { return this._element.UnderlyingObject; }
        }
    }
}