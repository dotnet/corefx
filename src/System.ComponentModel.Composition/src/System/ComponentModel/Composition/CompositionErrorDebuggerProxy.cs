// -----------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using Microsoft.Internal;

namespace System.ComponentModel.Composition
{
    internal class CompositionErrorDebuggerProxy
    {
        private readonly CompositionError _error;

        public CompositionErrorDebuggerProxy(CompositionError error)
        {
            Requires.NotNull(error, "error");

            this._error = error;
        }

        public string Description
        {
            get { return this._error.Description; }
        }

        public Exception Exception
        {
            get { return this._error.Exception; }
        }

        public ICompositionElement Element
        {
            get { return this._error.Element; }
        }
    }
}