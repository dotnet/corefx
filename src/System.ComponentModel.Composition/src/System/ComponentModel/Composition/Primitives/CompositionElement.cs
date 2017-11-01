// -----------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.Diagnostics;
using Microsoft.Internal;

namespace System.ComponentModel.Composition.Primitives
{
    // Represents the ICompositionElement placeholder for an 
    // object that does not implement ICompositionElement
    [DebuggerTypeProxy(typeof(CompositionElementDebuggerProxy))]
    internal class CompositionElement : SerializableCompositionElement
    {
        private static readonly ICompositionElement UnknownOrigin = new SerializableCompositionElement(SR.CompositionElement_UnknownOrigin, (ICompositionElement)null);
        private readonly object _underlyingObject;

        public CompositionElement(object underlyingObject)
            : base(underlyingObject.ToString(), UnknownOrigin)
        {
            this._underlyingObject = underlyingObject;
        }

        public object UnderlyingObject
        {
            get { return _underlyingObject; }
        }
    }
}
