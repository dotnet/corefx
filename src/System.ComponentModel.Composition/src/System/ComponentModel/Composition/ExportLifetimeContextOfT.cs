// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.ComponentModel.Composition
{
    public sealed class ExportLifetimeContext<T> : IDisposable
    {
        private readonly T _value;
        private readonly Action _disposeAction;

        public ExportLifetimeContext(T value, Action disposeAction)
        {
            _value = value;
            _disposeAction = disposeAction;
        }

        public T Value
        {
            get
            {
                return _value;
            }
        }

        public void Dispose()
        {
            if (_disposeAction != null)
            {
                _disposeAction.Invoke();
            }
        }
    }
}

