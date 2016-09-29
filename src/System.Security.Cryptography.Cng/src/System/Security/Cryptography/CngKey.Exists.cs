// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

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
        //
        // Check to see if a key already exists
        //

        public static bool Exists(string keyName)
        {
            return Exists(keyName, provider: CngProvider.MicrosoftSoftwareKeyStorageProvider);
        }

        public static bool Exists(string keyName, CngProvider provider)
        {
            return Exists(keyName, provider, options: CngKeyOpenOptions.None);
        }

        public static bool Exists(string keyName, CngProvider provider, CngKeyOpenOptions options)
        {
            if (keyName == null)
                throw new ArgumentNullException(nameof(keyName));

            if (provider == null)
                throw new ArgumentNullException(nameof(provider));

            using (SafeNCryptProviderHandle providerHandle = provider.OpenStorageProvider())
            {
                SafeNCryptKeyHandle keyHandle = null;
                try
                {
                    ErrorCode errorCode = Interop.NCrypt.NCryptOpenKey(providerHandle, out keyHandle, keyName, 0, options);
                    if (errorCode == ErrorCode.NTE_BAD_KEYSET)
                        return false;
                    if (errorCode != ErrorCode.ERROR_SUCCESS)
                        throw errorCode.ToCryptographicException();
                    return true;
                }
                finally
                {
                    if (keyHandle != null)
                        keyHandle.Dispose();
                }
            }
        }
    }
}

