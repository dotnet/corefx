// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


namespace System.ComponentModel.Composition.Primitives
{
    internal class ComposablePartExceptionDebuggerProxy
    {
        private readonly ComposablePartException _exception;

        public ComposablePartExceptionDebuggerProxy(ComposablePartException exception)
        {
            _exception = exception ?? throw new ArgumentNullException(nameof(exception));
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
