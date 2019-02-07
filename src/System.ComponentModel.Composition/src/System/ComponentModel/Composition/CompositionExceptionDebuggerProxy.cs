// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace System.ComponentModel.Composition
{
    internal class CompositionExceptionDebuggerProxy
    {
        private readonly CompositionException _exception;

        public CompositionExceptionDebuggerProxy(CompositionException exception)
        {
            _exception = exception ?? throw new ArgumentNullException(nameof(exception));
        }

        public ReadOnlyCollection<Exception> Exceptions
        {
            get
            {
                var errors = new List<Exception>();

                // In here return a collection of all of the exceptions in the Errors collection
                foreach (CompositionError error in _exception.Errors)
                {
                    if (error.Exception != null)
                    {
                        errors.Add(error.Exception);
                    }
                }
                return errors.ToReadOnlyCollection();
            }
        }

        public string Message => _exception.Message;

        public ReadOnlyCollection<Exception> RootCauses => _exception.RootCauses;
    }
}
