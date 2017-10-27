
// -----------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Microsoft.Internal;
using Microsoft.Internal.Collections;
using System.Threading;

namespace System.ComponentModel.Composition.ReflectionModel
{
    internal sealed class DisposableReflectionComposablePart : ReflectionComposablePart, IDisposable
    {
        private volatile int _isDisposed = 0;

        public DisposableReflectionComposablePart(ReflectionComposablePartDefinition definition)
            : base(definition)
        {
        }

        protected override void ReleaseInstanceIfNecessary(object instance)
        {
            IDisposable disposable = instance as IDisposable;
            if (disposable != null)
            {
                disposable.Dispose();
            }
        }

        protected override void EnsureRunning()
        {
            base.EnsureRunning();
            if (this._isDisposed == 1)
            {
                throw ExceptionBuilder.CreateObjectDisposed(this);
            }
        }

        void IDisposable.Dispose()
        {
            // NOTE : According to http://msdn.microsoft.com/en-us/library/4bw5ewxy.aspx, the warning is bogus when used with Interlocked API.
#pragma warning disable 420
            if (Interlocked.CompareExchange(ref this._isDisposed, 1, 0) == 0)
#pragma warning restore 420
            {
                this.ReleaseInstanceIfNecessary(this.CachedInstance);
            }
        }
    }
}
