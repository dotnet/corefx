// -----------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.ComponentModel.Composition.Primitives;
using System.Globalization;
using Microsoft.Internal;
using System.Threading;
using System.Collections.Generic;

namespace System.ComponentModel.Composition
{
    partial class ExportServices
    {
        private sealed class DisposableLazy<T, TMetadataView> : Lazy<T, TMetadataView>, IDisposable
        {
            private IDisposable _disposable; 

            public DisposableLazy(Func<T> valueFactory, TMetadataView metadataView, IDisposable disposable, LazyThreadSafetyMode mode)
                : base(valueFactory, metadataView, mode)
            {
                Assumes.NotNull(disposable);

                this._disposable = disposable;
            }

            void IDisposable.Dispose()
            {
                this._disposable.Dispose();
            }
        }

        private sealed class DisposableLazy<T> : Lazy<T>, IDisposable
        {
            private IDisposable _disposable;

            public DisposableLazy(Func<T> valueFactory, IDisposable disposable, LazyThreadSafetyMode mode)
                : base(valueFactory, mode)
            {
                Assumes.NotNull(disposable);

                this._disposable = disposable;
            }

            void IDisposable.Dispose()
            {
                this._disposable.Dispose();
            }
        }
    }
}
