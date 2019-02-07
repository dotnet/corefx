// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel.Composition.Primitives;

namespace System.ComponentModel.Composition
{
    internal class CompositionErrorDebuggerProxy
    {
        private readonly CompositionError _error;

        public CompositionErrorDebuggerProxy(CompositionError error)
        {
            _error = error ?? throw new ArgumentNullException(nameof(error));
        }

        public string Description
        {
            get { return _error.Description; }
        }

        public Exception Exception
        {
            get { return _error.Exception; }
        }

        public ICompositionElement Element
        {
            get { return _error.Element; }
        }
    }
}
