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

namespace System.ComponentModel.Composition
{
    internal class ImportCardinalityMismatchExceptionDebuggerProxy
    {
        private readonly ImportCardinalityMismatchException _exception;

        public ImportCardinalityMismatchExceptionDebuggerProxy(ImportCardinalityMismatchException exception)
        {
            Requires.NotNull(exception, "exception");

            this._exception = exception;
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