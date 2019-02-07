// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;

namespace System.ComponentModel.Composition.Primitives
{
    // Represents the ICompositionElement placeholder for an 
    // object that does not implement ICompositionElement
    [DebuggerTypeProxy(typeof(CompositionElementDebuggerProxy))]
    internal class CompositionElement : ICompositionElement
    {
        private static readonly ICompositionElement s_unknownOrigin = new CompositionElement(SR.CompositionElement_UnknownOrigin, null);

        public CompositionElement(object underlyingObject)
            : this(underlyingObject.ToString(), s_unknownOrigin)
        {
            UnderlyingObject = underlyingObject;
        }

        public CompositionElement(string displayName, ICompositionElement origin)
        {
            DisplayName = displayName ?? string.Empty;
            Origin = origin;
        }

        public string DisplayName { get; }

        public ICompositionElement Origin { get; }

        public override string ToString() => DisplayName;

        public object UnderlyingObject { get; }
    }
}
