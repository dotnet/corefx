// -----------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.Diagnostics;
using System.Reflection;

using Microsoft.Internal;
using Microsoft.Internal.Collections;

namespace System.ComponentModel.Composition.Primitives
{
    internal class ComposablePartExceptionDebuggerProxy
    {
        private readonly ComposablePartException _exception;

        public ComposablePartExceptionDebuggerProxy(ComposablePartException exception)
        {
            Requires.NotNull(exception, "exception");

            this._exception = exception;
        }

        public ICompositionElement Element
        {
            get { return _exception.Element; }
        }

        public Exception InnerException 
        { 
            get { return _exception.InnerException; }
        }

        public string Message
        {
            get { return _exception.Message; }
        }
    }
}