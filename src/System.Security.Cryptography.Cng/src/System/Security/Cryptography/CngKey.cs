// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Diagnostics;

using Microsoft.Win32.SafeHandles;

using Internal.Cryptography;

using ErrorCode = Interop.NCrypt.ErrorCode;

namespace System.Security.Cryptography
{
    /// <summary>
    ///     Managed representation of an NCrypt key
    /// </summary>
    public sealed partial class CngKey : IDisposable
    {
        private CngKey(SafeNCryptProviderHandle providerHandle, SafeNCryptKeyHandle keyHandle)
        {
            Debug.Assert(keyHandle != null && !keyHandle.IsInvalid && !keyHandle.IsClosed);
            Debug.Assert(providerHandle != null && !providerHandle.IsInvalid && !providerHandle.IsClosed);

            _providerHandle = providerHandle;
            _keyHandle = keyHandle;
        }

        public void Dispose()
        {
            if (_providerHandle != null)
            {
                _providerHandle.Dispose();
            }

            if (_keyHandle != null)
            {
                _keyHandle.Dispose();
            }
        }

        private readonly SafeNCryptKeyHandle _keyHandle;
        private readonly SafeNCryptProviderHandle _providerHandle;
    }
}

