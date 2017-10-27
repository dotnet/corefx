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
    internal class CompositionExceptionDebuggerProxy
    {
        private readonly CompositionException _exception;

        public CompositionExceptionDebuggerProxy(CompositionException exception)
        {
            Requires.NotNull(exception, "exception");

            this._exception = exception;
        }

        public ReadOnlyCollection<Exception> Exceptions
        {
            get
            {
                var errors = new List<Exception>();

                // In here return a collection of all of the exceptions in the Errors collection
                foreach (var error in _exception.Errors)
                {
                    if (error.Exception != null)
                    {
                        errors.Add(error.Exception);
                    }
                }
                return errors.ToReadOnlyCollection<Exception>();
            }
        }

        public string Message
        {
            get { return _exception.Message; }
        }

        public ReadOnlyCollection<Exception> RootCauses
        {
            get { return _exception.RootCauses; }
        }
    }
}