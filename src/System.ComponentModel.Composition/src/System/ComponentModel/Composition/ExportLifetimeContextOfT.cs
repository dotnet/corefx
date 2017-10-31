// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
using System;
using System.ComponentModel.Composition.Primitives;
using System.Linq;

namespace System.ComponentModel.Composition
{
    public sealed class ExportLifetimeContext<T> : IDisposable
    {
        private readonly T _value;
        private readonly Action _disposeAction;

        public ExportLifetimeContext(T value, Action disposeAction)
        {
            this._value = value;
            this._disposeAction = disposeAction;
        }

        public T Value
        {
            get
            {
                return this._value;
            }
        }

        public void Dispose()
        {
            if (this._disposeAction != null)
            {
                this._disposeAction.Invoke();
            }
        }
    }
}

