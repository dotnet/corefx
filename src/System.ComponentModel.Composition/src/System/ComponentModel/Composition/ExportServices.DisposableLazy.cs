// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading;

namespace System.ComponentModel.Composition
{
    internal partial class ExportServices
    {
        private sealed class DisposableLazy<T, TMetadataView> : Lazy<T, TMetadataView>, IDisposable
        {
            private readonly IDisposable _disposable;

            public DisposableLazy(Func<T> valueFactory, TMetadataView metadataView, IDisposable disposable, LazyThreadSafetyMode mode)
                : base(valueFactory, metadataView, mode)
            {
                if (disposable == null)
                {
                    throw new ArgumentNullException(nameof(disposable));
                }

                _disposable = disposable;
            }

            void IDisposable.Dispose()
            {
                _disposable.Dispose();
            }
        }

        private sealed class DisposableLazy<T> : Lazy<T>, IDisposable
        {
            private readonly IDisposable _disposable;

            public DisposableLazy(Func<T> valueFactory, IDisposable disposable, LazyThreadSafetyMode mode)
                : base(valueFactory, mode)
            {
                if (disposable == null)
                {
                    throw new ArgumentNullException(nameof(disposable));
                }

                _disposable = disposable;
            }

            void IDisposable.Dispose()
            {
                _disposable.Dispose();
            }
        }
    }
}
